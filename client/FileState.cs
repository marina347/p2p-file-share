using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.Security.Cryptography;

namespace client {
	public class FileState {
		FileDescription fileDescription;
		private List<int> finishedChunks;
		private List<int> unfinishedChunks;
		private int finishedChunksNumber;
		BitArray chunksState;

		FileIOManager fileManager;

		public static byte[] BitArrayToByteArray(BitArray bits) {
			byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
			bits.CopyTo(ret, 0);
			return ret;
		}

		public byte[] GetChunksState() {
			if (fileDescription.Fts != FileTransferState.Finished)
				return BitArrayToByteArray(chunksState);
			else {
				BitArray ba = new BitArray(NumberOfChunks, true);
				return BitArrayToByteArray(ba);
			}
		}

		public bool ChunkFinished(int chunkId) {
			ChunksState.Set(chunkId, true);
			FinishedChunksNumber++;
			return true;
		}

		public bool CheckChunkHash(Chunk chunk) {
			
			int chunkCount = Sizes.GetChunksNumber(this.FileDescription.FileSize);
			byte[] correctHash = fileManager.ReadHashFromDescriptor(chunk.ChunkId, chunkCount);
			byte[] calculatedHash = new SHA256Managed().ComputeHash(chunk.Data);
			for(int i=0; i<Sizes.HashSizeByte; i++) {
				if (correctHash[i] != calculatedHash[i])
					return false;
			}
			return true;
		}

		public FileDescription FileDescription {
			get {
				return fileDescription;
			}

			set {
				fileDescription = value;
			}
		}

		public int NumberOfChunks{
			get {
				return Sizes.GetChunksNumber(this.FileDescription.FileSize);
			}
		}

		public List<int> FinishedChunks {
			get {
				return finishedChunks;
			}

			set {
				finishedChunks = value;
			}
		}

		public List<int> UnfinishedChunks {
			get {
				return unfinishedChunks;
			}

			set {
				unfinishedChunks = value;
			}
		}

		public BitArray ChunksState {
			get {
				return chunksState;
			}

			set {
				chunksState = value;
			}
		}

		internal FileIOManager FileManager {
			get {
				return fileManager;
			}

			set {
				fileManager = value;
			}
		}

		public int FinishedChunksNumber {
			get {
				return finishedChunksNumber;
			}

			set {
				finishedChunksNumber = value;
			}
		}
	}
}
