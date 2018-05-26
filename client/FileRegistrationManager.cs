using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;

namespace client {
	public class FileRegistrationManager {
		public static FileRegistrationManager frm;
		bool registrationProcessStarted;
		bool sendAllInProgress;
		FileDescription fd;
		FileStream fileStream;
        private bool accepted;
		public bool StartRegistrationProcess(FileDescription fileDescription) {
			if (RegistrationProcessStarted)
				return false;
			else {
                Accepted = false;
				RegistrationProcessStarted = true;
				Fd = fileDescription;
				SendFileRegistrationRequest();
				return true;
			}
		}

		public void FileAccepted() {
            Accepted = true;
			fileStream = new FileStream(fd.Path + "\\" + fd.FileName + "." + fd.FileExtension, FileMode.Open, FileAccess.Read);
		}

		private void SendFileRegistrationRequest() {
			Debug.WriteLine("Sending request for registration");
			FileSearchAndRegistrationMessageProvider frmp = new FileSearchAndRegistrationMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = frmp.ProvideRequestFileRegistration(fd.FileName + "." + fd.FileExtension, fd.FileSize);
			ClientCommunicationCenter.commCenter.SendMessageToServer(om);
		}

		public bool SendAllHashes() {
			if (!RegistrationProcessStarted)
				return false;
			else {
                
                RegistrationProcessStarted = true;
				sendAllInProgress = true;
				int count = Sizes.GetChunksNumber(fd.FileSize);
                
				for (int i = 0; i < count - 1; i++) {
                    if (i != count - 1) {
                        if (ReadChunk(0) == null) { return false; }
                        SendHash(i, MakeHash(ReadChunk(i)));
                    }

				}
				SendHash(count - 1, MakeHash(ReadLastChunk(count - 1)));
				sendAllInProgress = false;
				return true;
			}
		}

		public byte[] MakeHash(byte[] chunk) {
			return new SHA256Managed().ComputeHash(chunk);
		}

		private byte[] ReadChunk(int chunkId) {
            if (fileStream == null) return null;
			lock (fileStream) {
				byte[] polje = new byte[Sizes.ChunkSize];
				Debug.WriteLine("read at " + chunkId);
				
				fileStream.Seek(Sizes.ChunkSize * chunkId, SeekOrigin.Begin);
				fileStream.Read(polje, 0, Sizes.ChunkSize);
				return polje;
			}
		}

		private byte[] ReadLastChunk(int chunkId) {
			lock (fileStream) {
				int bytesLeft = (int)fd.FileSize - chunkId * Sizes.ChunkSize;
				byte[] polje = new byte[bytesLeft];
				fileStream.Seek(Sizes.ChunkSize * chunkId, SeekOrigin.Begin);
				fileStream.Read(polje, 0, bytesLeft);
				return polje;
			}
		}

		private void SendHash(int chunkId, byte[] hash) {
			FileSearchAndRegistrationMessageProvider frmp = new FileSearchAndRegistrationMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = frmp.ProvideResponseChunksMessage(fd.FileId, chunkId, hash);
			ClientCommunicationCenter.commCenter.SendMessageToServer(om);
		}

		public void ResendHash(int chunkId) {
			if (sendAllInProgress)
				return;
			if (chunkId != Sizes.GetChunksNumber(fd.FileSize) + 1)
				SendHash(chunkId, MakeHash(ReadChunk(chunkId)));
			else
				SendHash(chunkId, MakeHash(ReadLastChunk(chunkId)));
		}

		public void CloseRegistrationProcess() {
			if (fileStream != null) {
				fileStream.Close();
				fileStream = null;
			}
			RegistrationProcessStarted = false;
            Accepted = false;
		}

		public bool RegistrationProcessStarted {
			get {
				return registrationProcessStarted;
			}

			set {
				registrationProcessStarted = value;
			}
		}

		public FileDescription Fd {
			get {
				return fd;
			}

			set {
				fd = value;
			}
		}

        public bool Accepted {
            get {
                return accepted;
            }

            set {
                accepted = value;
            }
        }
    }
}
