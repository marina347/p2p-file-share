using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonResources {
	public class Sizes {
		public const int ChunkPartSize = 1024;
		public const int ChunkPartCount = 256;
        public const int ChunkSize = ChunkPartSize * ChunkPartCount;
        public const int HashSizeByte = 32;
		public const int FileDescriptionDriveSize = sizeof(int) * 4 + 200;
		
		public static int GetIntervalForHashes(int chunksLeft) {
			
			int k = (int)((chunksLeft) * 0.012279f) * 1000;
			if (k <= 1000)
				k = 1000;
			return k;
		}

		public static int GetChunksNumber(uint fileSize) {
			return fileSize % ChunkSize == 0?
				 (int)(fileSize / ChunkSize):
				 (int)(fileSize / ChunkSize) + 1;
		}

		public static int GetByteNumberForFileState(uint fileSize) {
			int chNumber = GetChunksNumber(fileSize);
			return chNumber % 8 == 0? 
				chNumber / 8:
				chNumber / 8 + 1;
		}

		public static bool IsLastChunk(uint fileSize, int chunkId) {
			int chNumber = GetChunksNumber(fileSize);
			return chNumber - 1 == chunkId;
		}

		public static int LastChunkSize(uint fileSize) {
			int lastChunkBytes = (int)fileSize % Sizes.ChunkSize;
			return lastChunkBytes == 0 ?
				Sizes.ChunkSize :
				lastChunkBytes;
		}
		public static int LastChunkPartsCount(uint fileSize) {
			int lastChunkBytes = LastChunkSize(fileSize);
			return lastChunkBytes % Sizes.ChunkPartSize == 0?
				lastChunkBytes / Sizes.ChunkPartSize: 
				lastChunkBytes / Sizes.ChunkPartSize +1;
		}

		public static int LastChunkPartSize(uint fileSize) {
			int lastChunkPartBytes = LastChunkSize(fileSize)%Sizes.ChunkPartSize;
			return lastChunkPartBytes == 0 ? 
				Sizes.ChunkPartSize:
				lastChunkPartBytes;
		}

	}
}
