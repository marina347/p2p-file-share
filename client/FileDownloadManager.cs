using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CommonResources;
using System.Diagnostics;

namespace client {
	public class FileDownloadManager {
		List<int> peersWithFile;

		FileState fs;
		int fileId;
		Dictionary<int, EndPointManager> endPointManagers;
		BitArray requested;
		int lastChunk;
		Timer timer;
		Timer timerFileState;
		Timer timerResend;
		bool paused;
		bool allDownloaded;
		
		public FileDownloadManager(int fileIdInput, FileState fs, bool paused) {
			
			this.fs = fs;
			this.fileId = fileIdInput;
			this.endPointManagers = new Dictionary<int, EndPointManager>();
			this.paused = paused;
		}

		public void Start() {
			Debug.WriteLine("FDM file "+fileId+" started");
			allDownloaded = false;
			timer = new Timer();
			timer.Interval = 5000;
			timer.Elapsed += CheckAndRequest;
			timerFileState = new Timer();
			timerFileState.Interval = 5000;
			timerFileState.Elapsed += CheckFileState;
			timerResend = new Timer();
			timerResend.Interval = 600;
			timerResend.Elapsed += CheckResend;
			requested = new BitArray(fs.NumberOfChunks, false);
			GetFileOwners();
			if (paused) return;
			timer.Start();
			timerFileState.Start();
			timerResend.Start();
		}

		private void GetFileOwners() {
			ConnectivityMessageProvider cmp = new ConnectivityMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = cmp.ProvideFileClientListRequest(fileId);
			ClientCommunicationCenter.commCenter.SendMessageTo(ApplicationLiveData.ald.ServerEndPoint, om);
		}

		public void ClientListArrived(int[] list) {
			peersWithFile = new List<int>(list);
			string klijenti = "";
			foreach(int kl in list) {
				klijenti += kl + " ";
			}
			Debug.WriteLine("Dohvatio listu: " + klijenti);
			FileTransferCenter.ftc.ncm.RequestConnection(this, peersWithFile);
		}

		private void CheckAndRequest(object sender, EventArgs e) {
			
			Debug.WriteLine("CheckAndRequest");
			while (lastChunk!=fs.ChunksState.Length && fs.ChunksState.Get(lastChunk))
				lastChunk++;
			lock (endPointManagers) {
				int c = lastChunk;
				foreach (EndPointManager epm in endPointManagers.Values) {
					if (epm.ClientFileState != null && epm.AllChunksFinished) {
						
						Dictionary<int, Chunk> chunks = new Dictionary<int, Chunk>();
						for(; c<fs.NumberOfChunks && chunks.Count < 8; c++) {
							if (fs.ChunksState.Get(c) == false 
							&& this.requested.Get(c) == false 
							&& epm.ClientFileState.Get(c)==true) {
								int chunkPartCount, chunkSize;
								if(c != fs.NumberOfChunks - 1) {
									chunkPartCount = Sizes.ChunkPartCount;
									chunkSize = Sizes.ChunkSize;
								}
								else {
									chunkPartCount = Sizes.LastChunkPartsCount(fs.FileDescription.FileSize);
									chunkSize = Sizes.LastChunkSize(fs.FileDescription.FileSize);
								}
								chunks.Add(c, new Chunk(c, epm, chunkPartCount, chunkSize));
								this.requested.Set(c, true);
							}
						}
						if(chunks.Count != 0) { 
							epm.AssignNewChunks(chunks);
							FileTransferCenter.ftc.ndm.RequestChunks(epm.EndPointId, this.fileId, chunks.Keys.ToArray());
						}
					}
				}
			}
		}

		public void CheckResend(object sender, EventArgs e) {
			long currentTimeMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			foreach (EndPointManager epm in endPointManagers.Values) {
				if (epm.ClientFileState != null && !epm.AllChunksFinished) {
					if(currentTimeMilliseconds - epm.LastReceived > 300) {
						
							foreach(Chunk ch in epm.Chunks.Values) {
								
								for (int partId = 0; partId < ch.Received.Length; partId++) {
								if (!ch.Received.Get(partId)) {
									FileTransferCenter.ftc.ndm.RequestChunkPartResend(
										epm.EndPointId,
										this.fs.FileDescription.FileId,
										ch.ChunkId,
										partId);
									Debug.WriteLine("resend request chunk " + ch.ChunkId + " part " + partId);
								}
							}
						}
					}
				}
			}
		}

		private void CheckFileState(object sender, EventArgs e) {
			foreach (EndPointManager epm in endPointManagers.Values) {
				
					FileTransferCenter.ftc.nfsm.RequestFileState(epm.EndPointId, this.fileId);
					Debug.WriteLine(ApplicationLiveData.ald.ApplicationId + ": Requested clients " + epm.EndPointId + "state");
				
			}
		}

		public void NewClientAvailable(int ep) {
			Debug.WriteLine("FDM: NewClientAvailable:"+ep);
			if (endPointManagers.ContainsKey(ep))
				return;
			
			endPointManagers.Add(ep, new EndPointManager(this, ep));
			FileTransferCenter.ftc.nfsm.RequestFileState(ep, this.fileId);
			Debug.WriteLine(ApplicationLiveData.ald.ApplicationId + ": Requested clients " + ep + "state");
		}
		
		public void ClientLostConnection(int ep) {
			Debug.WriteLine("FDM: lost connection:" + ep);
			EndPointManager epm = null;
			lock (endPointManagers) {
				epm = endPointManagers[ep];
				endPointManagers.Remove(ep);
			}
			foreach (Chunk ch in epm.Chunks.Values) {
				if (ch.IsFinished == false) {
						requested.Set(ch.ChunkId, false);
				}
			}
			
		}

		public void ClientsFileStateArrived(int ep, BitArray state) {
			endPointManagers[ep].ClientFileState = state;
			Debug.WriteLine("FDM "+ApplicationLiveData.ald.ApplicationId+": Aquired clients " + ep + "state");
			CheckAndRequest(null, null);
		}
		
		public void ChunkFinished(EndPointManager epm, Chunk chunk) {
			lock (epm) { 
				if (fs.CheckChunkHash(chunk)) {
					epm.ChunkConfirmed(chunk.ChunkId);
					fs.FileManager.WriteChunk(chunk.ChunkId, chunk.Data);
					fs.ChunkFinished(chunk.ChunkId);
					Debug.WriteLine("fid:"+this.fileId+" CHUNK "+chunk.ChunkId+" finished");
					if (allDownloaded == false && fs.FinishedChunksNumber == fs.NumberOfChunks) {
						allDownloaded = true;
						fs.FileDescription.Ftm = FileTransferMode.OnlyUpload;
						fs.FileDescription.Fts = FileTransferState.Finished;
						MasterFileManager.mfm.UpdateFileDescription(fs.FileDescription);
						this.timer.Stop();
						this.timerFileState.Stop();
						this.timerResend.Stop();
					}
				}
				else {
					
					epm.ChunkConfirmed(chunk.ChunkId);
					
					this.requested.Set(chunk.ChunkId, false);
				}
			}
		}
		public void ChunkPartArrived(int sender, int chunkId, int chunkPartId, byte[] chunkPart) {
			endPointManagers[sender].ChunkPartArrived(chunkId, chunkPartId, chunkPart);
		}
		
		public void Pause() {
			timer.Stop();
			timerFileState.Stop();
			timerResend.Stop();
		}

		public void UnPause() {
			timer.Start();
			timerFileState.Start();
			timerResend.Start();
		}
		
	}
}
