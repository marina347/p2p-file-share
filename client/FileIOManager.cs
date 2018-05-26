using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class FileIOManager {
		FileStream mainFileStream;
		FileStream descriptionFileStream;

		public FileIOManager(FileStream masterFileStream, FileStream fileDescriptionStream) {
			this.mainFileStream = masterFileStream;
			this.descriptionFileStream = fileDescriptionStream;
		}

        public FileIOManager(FileStream masterFileStream) {
            this.mainFileStream = masterFileStream;
        }

		public void CloseFileStreams() {
			lock (mainFileStream) {
				mainFileStream.Close();
			}
			if(descriptionFileStream!=null)
				lock (descriptionFileStream) {
					descriptionFileStream.Close();
				}
		}
		
		public byte[] ReadHashFromDescriptor(int chunkId, int chunkCount) {
			lock (descriptionFileStream) {
				int seekPos = 0;
				
				seekPos += chunkCount;
				
				seekPos += chunkId * Sizes.HashSizeByte;
				descriptionFileStream.Seek(seekPos, SeekOrigin.Begin);
				byte[] hash = new byte[Sizes.HashSizeByte];
				descriptionFileStream.Read(hash, 0, hash.Length);
				return hash;
			}
		}

        public void WriteChunk(int chunkId, byte[] chunkData) {
			
			lock (mainFileStream) {
				mainFileStream.Seek(chunkId * Sizes.ChunkSize, SeekOrigin.Begin);
				mainFileStream.Write(chunkData, 0, chunkData.Length);
				mainFileStream.Flush();
			}
			lock (descriptionFileStream) {
				byte[] array = {1};
				descriptionFileStream.Seek(chunkId, SeekOrigin.Begin);
				descriptionFileStream.Write(array, 0, array.Length);
				descriptionFileStream.Flush();
			}
		}

		public byte[] GetDataOfChunkPart(int chunkId, int chunkPartId) {
			lock (mainFileStream) {
				byte[] array = new byte[Sizes.ChunkPartSize];
				mainFileStream.Seek(chunkId * Sizes.ChunkSize + chunkPartId * Sizes.ChunkPartSize, SeekOrigin.Begin);
				mainFileStream.Read(array, 0, Sizes.ChunkPartSize);
				return array;
			}
		}

		public byte[] ReadLastChunkLastPartData(int chunkId, int chunkPartId) {
			lock (mainFileStream) {
				int sizeToRead = Sizes.LastChunkPartSize((uint)mainFileStream.Length);
				byte[] array = new byte[sizeToRead];
				mainFileStream.Seek(chunkId * Sizes.ChunkSize + chunkPartId * Sizes.ChunkPartSize, SeekOrigin.Begin);
				mainFileStream.Read(array, 0, sizeToRead);
				return array;
			}
		}

		private byte[] ReadLastChunkData(int chunkId) {
			long whatsLeft = mainFileStream.Length - chunkId * Sizes.ChunkSize;
			byte[] array = new byte[whatsLeft];
			mainFileStream.Seek(chunkId * Sizes.ChunkSize, SeekOrigin.Begin);
			mainFileStream.Read(array, 0, (int)whatsLeft);
			return array;
		}

		private byte[] ReadNormalChunkData(int chunkId) {
			byte[] array = new byte[Sizes.ChunkSize];
			mainFileStream.Seek(chunkId * Sizes.ChunkSize, SeekOrigin.Begin);
			mainFileStream.Read(array, 0, Sizes.ChunkSize);
			return array;
		}

		public byte[] ReadChunkData(int chunkId) {
			lock (mainFileStream) {
				if(Sizes.IsLastChunk((uint)mainFileStream.Length, chunkId)) {
					return ReadLastChunkData(chunkId);
				}
				else {
					return ReadNormalChunkData(chunkId);
				}
			}
		}
	}
}
