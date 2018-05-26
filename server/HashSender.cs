using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.BazaF;
using CommonResources;
using System.Threading;
using System.Diagnostics;
namespace server {
	class HashSender {
		public static void SendAllHashes(int clientEp, int fileId) {
			List<ChunkHashModel> chmList = ChunkHashModel.GetChunksByFileId(fileId);
			foreach (ChunkHashModel chm in chmList) {
				SendHash(clientEp, chm.HashNumber, chm.Hash);
				Thread.Sleep(5);
			}
		}

		public static void ResendHash(int clientEp, int fileId, int chunkId) {
			SendHash(clientEp, chunkId, ChunkHashModel.GetChunkByChunkIdAndFileId(chunkId, fileId));
		}

		private static void SendHash(int clientEp, int chunkId, byte[] hash) {
			Debug.WriteLine("sent: " + chunkId);
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = smp.ProvideChunkHashDataMessage(chunkId, hash);
			ServerCommunicationCenter.commCenter.SendMessageTo(clientEp, om);
		}
	}
}
