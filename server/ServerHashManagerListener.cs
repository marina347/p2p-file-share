using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using server.BazaF;

namespace server {
	class ServerHashManagerListener: HashManagerListener {
		public ServerHashManagerListener() {
			
		}

		public void HashDownloadFinished(ChunkHash[] hashes, int fileId, int senderId) {
			Owner o = new Owner();
			o.ApplicationId = senderId;
			o.FileId = fileId;
			o.Add();
			ChunkHashModel.Add(hashes, fileId);
			SendFinishedNotification(fileId, senderId);
		}

		public void RequestResend(int chunkHashId, int senderId, int fileId){
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = smp.ProvideResendHashRequestMessage(chunkHashId,fileId);
			ServerCommunicationCenter.commCenter.SendMessageTo(senderId, om);
		}

		private void SendFinishedNotification(int fileId, int clientEp) {
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = smp.ProvideFileRegisteredMessage(fileId);
			ServerCommunicationCenter.commCenter.SendMessageTo(clientEp, om);
		}
	}
}
