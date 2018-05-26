using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.Threading;
using System.Diagnostics;

namespace client {
	class FileUploadManager {
		FileState fs;
		bool paused;
		public FileUploadManager(FileState fs, bool pausedInput) {
			this.fs = fs;
			paused = pausedInput;
		}

		public void SendChunks(int requester, int[] chunks) {
			if (paused) return;
			for(int i=0; i<chunks.Length; i++) {
				if (fs.NumberOfChunks - 1 != chunks[i])
					SendChunk(requester, chunks[i]);
				else
					SendLastChunk(requester, chunks[i]);
			}
		}

		private void SendChunk(int requester, int chunk) {
			byte[] chunkData = fs.FileManager.ReadChunkData(chunk);
			for (int i = 0; i < Sizes.ChunkPartCount; i++) {
				SendChunkPart(
					requester,
					chunk, 
					i, chunkData,
					i * Sizes.ChunkPartSize,
					Sizes.ChunkPartSize);
				
			}
		}
		
		private void SendLastChunk(int requester, int chunk) {
			byte[] chunkData = fs.FileManager.ReadChunkData(chunk);
			int chunkPartsCount = Sizes.LastChunkPartsCount(fs.FileDescription.FileSize);
			for (int i = 0; i < chunkPartsCount-1; i++) {
				SendChunkPart(requester, chunk, i, chunkData, i * Sizes.ChunkPartSize, Sizes.ChunkPartSize);
				
			}
			int lastChunkPartSize = Sizes.LastChunkPartSize(fs.FileDescription.FileSize);
			SendChunkPart(
				requester, 
				chunk, 
				chunkPartsCount- 1,
				chunkData,
				(chunkPartsCount - 1) * Sizes.ChunkPartSize,
				lastChunkPartSize);
		}
		
		private void SendChunkPart(int requester,  int chunkId, int chunkPartId, byte[] data, int start, int count) {
			FileTransferCenter.ftc.ndm.SendChunkPart(
				requester, 
				fs.FileDescription.FileId,
				chunkId,
				chunkPartId,
				data,
				start,
				count
			);
			Debug.WriteLine("sent chunk " + chunkId + " part +" + chunkPartId);
		}
		
		public void ResendChunkPart(int requester, int chunkId, int chunkPartId) {
			if (paused) return;
			byte[] chunkPart;
			
			if (chunkId == fs.NumberOfChunks - 1) {
				
				if (Sizes.LastChunkPartsCount(fs.FileDescription.FileSize)-1 == chunkPartId) {
					chunkPart = fs.FileManager.ReadLastChunkLastPartData(chunkId, chunkPartId);
					SendChunkPart(requester, chunkId, chunkPartId,chunkPart, 0, chunkPart.Length);
					return;
				}
			}
			chunkPart = fs.FileManager.GetDataOfChunkPart(chunkId, chunkPartId);
			SendChunkPart(requester, chunkId, chunkPartId, chunkPart, 0, chunkPart.Length);
		}

		public void Pause() {
			paused = true;
		}

		public void UnPause() {
			paused = false;
		}

	}
}
