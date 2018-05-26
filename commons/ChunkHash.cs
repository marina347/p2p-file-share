using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public class ChunkHash {
		int chunkId;
		byte[] hash;

		public ChunkHash() {
			hash = new byte[Sizes.HashSizeByte];
		}

		public int ChunkId {
			get {
				return chunkId;
			}

			set {
				chunkId = value;
			}
		}

		public byte[] Hash {
			get {
				return hash;
			}

			set {
				hash = value;
			}
		}
	}
}
