using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client {
	public class EndPointManager {
		FileDownloadManager fileDownloadManager;
		BitArray clientFileState;
		int numOfWantedChunks;
		int numOfFinishedChunks;
		Dictionary<int, Chunk> chunks;
		BitArray finishedChunks;
		int endPointId;

		long lastReceived;
		public EndPointManager(FileDownloadManager fileDownloadManagerInput, int endPointIdInput) {
			this.fileDownloadManager = fileDownloadManagerInput;
			this.endPointId = endPointIdInput;
			numOfFinishedChunks = 0;
			numOfWantedChunks = 0;
		}

		public void AssignNewChunks(Dictionary<int, Chunk>  chunksIdInput) {
			chunks = chunksIdInput;
			finishedChunks = new BitArray(chunks.Count, false);
			numOfWantedChunks = chunks.Count;
			numOfFinishedChunks = 0;
			LastReceived = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public void ChunkPartArrived(int chunkId, int chunkPartId, byte[] chunkPart) {
			LastReceived = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			chunks[chunkId].ChunkPartArrived(chunkPartId, chunkPart);
		}
		
		public void ChunkFinished(int chunkId, byte[] data) {
			fileDownloadManager.ChunkFinished(this, chunks[chunkId]);
		}
		
		public void ChunkConfirmed(int chunkId) {
			numOfFinishedChunks++;
			finishedChunks.Set(chunks.Keys.ToList().IndexOf(chunkId), true);
		}

		public bool AllChunksFinished {
			get {
				return numOfWantedChunks == numOfFinishedChunks;
			}
		}

		public BitArray ClientFileState {
			get {
				return clientFileState;
			}

			set {
				clientFileState = value;
			}
		}

		public int EndPointId {
			get {
				return endPointId;
			}
		}

		public BitArray FinishedChunks {
			get {
				return finishedChunks;
			}
		}

		internal Dictionary<int, Chunk> Chunks {
			get {
				return chunks;
			}

			set {
				chunks = value;
			}
		}

		public long LastReceived {
			get {
				return lastReceived;
			}

			set {
				lastReceived = value;
			}
		}
	}
}
