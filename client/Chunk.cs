using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.Diagnostics;

namespace client {
	public class Chunk {
		EndPointManager endPointManager;
		int chunkId;
		int numberOfReceived;
		BitArray received;
		byte[] data;
		int chunkChunkNumbers;
		
		public bool IsFinished {
			get {
				return numberOfReceived == 128;
			}
		}

		public byte[] Data {
			get {
				return data;
			}

			set {
				data = value;
			}
		}

		public BitArray Received {
			get {
				return received;
			}

			set {
				received = value;
			}
		}
		public int ChunkId {
			get {
				return chunkId;
			}

		}

		public Chunk(int chunkId, EndPointManager endPointManagerUlaz, int chunkChunkNumbers, int chunkSize) {
			this.chunkChunkNumbers = chunkChunkNumbers;
			this.endPointManager = endPointManagerUlaz;
			this.chunkId = chunkId;
			Received = new BitArray(this.chunkChunkNumbers, false);
			numberOfReceived = 0;
			Data = new byte[chunkSize];
		}
	
		public void ChunkPartArrived(int chunkPartId, byte[] chunkPart) {
			lock (received) {
				if (!received.Get(chunkPartId)) { 
					Received.Set(chunkPartId, true);
					numberOfReceived++;
					Buffer.BlockCopy(chunkPart, 0, data, chunkPartId * Sizes.ChunkPartSize, chunkPart.Length);
					Debug.WriteLine("chunk "+chunkId+" PRIMIO: " + numberOfReceived + "/" + chunkChunkNumbers);
					if(numberOfReceived == chunkChunkNumbers) {
						endPointManager.ChunkFinished(ChunkId, Data);
					}
				}
			}
		}
	}
}
