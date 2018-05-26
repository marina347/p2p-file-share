using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using CommonResources;

namespace client {
	class NetworkDataManager {
		public void RequestChunks(int ep, int fileId, int[] chunks) {
			DataMessageProvider dmp = new DataMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = dmp.ProvideRequestChunksMessage(fileId, chunks);
			ClientCommunicationCenter.commCenter.SendMessageTo(ep, msg);
		}
		
		public void SendChunkPart(int ep, int fileId, int chunkId, int chunkPartId, byte[] data, int start, int count) {
			DataMessageProvider dmp = new DataMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = dmp.ProvideSendChunkPartMessage(fileId, chunkId, chunkPartId, data, start, count);
			ClientCommunicationCenter.commCenter.SendMessageTo(ep, msg);
		}
		
		public void RequestChunkPartResend(int ep, int fileId, int chunkId, int chunkPartId) {
			DataMessageProvider dmp = new DataMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = dmp.ProvideRequestChunkPartResendMessage(fileId, chunkId, chunkPartId);
			ClientCommunicationCenter.commCenter.SendMessageTo(ep, msg);
		}

		public void ChunksRequested(int requester, int fileId, int[] chunks) {
			FileTransferCenter.ftc.GetFum(fileId).SendChunks(requester, chunks);
		}
		
		public void ResendChunkPartRequested(int requester, int fileId, int chunkId, int chunkPartId) {
			FileTransferCenter.ftc.GetFum(fileId).ResendChunkPart(requester, chunkId, chunkPartId);
		}
		
		public void ChunksPartArrived(int sender, int fileId, int chunkId, int chunkPartId, byte[] chunkPart) {
			FileTransferCenter.ftc.GetFdm(fileId).ChunkPartArrived(sender, chunkId, chunkPartId, chunkPart);
		}
	}
}
