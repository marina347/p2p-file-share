using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Timers;
using CommonResources;
using System.Diagnostics;

namespace CommonResources {
	public class HashManager {
		int senderId;
		int fileId;
		int numberOfChunks;
		int received;
		BitArray receivedHash;
		ChunkHash[] hashes;
		Timer timer;
		HashManagerListener hml;

		public int FileId {
			get {
				return fileId;
			}
		}

		public HashManager(int ep, int numberOfChunks, int fileId, HashManagerListener listener) {
			this.hml = listener;
			this.fileId = fileId;
			this.senderId = ep;
			this.numberOfChunks =numberOfChunks;
			receivedHash = new BitArray(numberOfChunks, false);
			hashes = new ChunkHash[numberOfChunks];
			for (int i=0; i<hashes.Length; i++) {
				hashes[i] = new ChunkHash();
				hashes[i].ChunkId = i;
			}
			received = 0;
			timer = new Timer(Sizes.GetIntervalForHashes(numberOfChunks));
			timer.Elapsed += RequestHashResend; 
			timer.Start();
			
		}

		public void HashArrived(ChunkHash ch) {
			if (!receivedHash.Get(ch.ChunkId)) { 
				lock (hashes) {
					hashes[ch.ChunkId].Hash = ch.Hash;
					received++;
					receivedHash.Set(ch.ChunkId, true);

					Debug.WriteLine("sveukupno" + received + "/" + numberOfChunks);
					if (received == numberOfChunks) {
						AllHashesCollected();
					}
				}
			}
		}

		private void AllHashesCollected() {
			hml.HashDownloadFinished(hashes, FileId, senderId);
		}

		public void RequestHashResend(Object source, ElapsedEventArgs e) {
			if (received!=numberOfChunks) {
				for (int i=0; i<numberOfChunks; i++) {
					if (receivedHash.Get(i) == false) {
						hml.RequestResend(i, senderId,fileId);
					}
				}
				timer.Interval = Sizes.GetIntervalForHashes(numberOfChunks-received);
			}
			else {
					timer.Stop();
				
			}
		}
	}
}
