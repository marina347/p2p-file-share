using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using CommonResources;

namespace client {
	class NetworkFileStateManager {
		public void RequestFileState(int ep, int fileId) {
			FileStateMessageProvider fsmp = new FileStateMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = fsmp.ProvideRequestFileStateMessage(fileId);
			ClientCommunicationCenter.commCenter.SendMessageTo(ep, msg);
		}

		public void FileStateRequested(int ep, int fileId) {
			FileState fs = FileTransferCenter.ftc.GetFs(fileId);
			if (fs != null) {
				byte[] chunksState = fs.GetChunksState();
				FileStateMessageProvider fsmp = new FileStateMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
				OutputMessage msg = fsmp.ProvideResponseFileStateMessage(fileId, chunksState);
				ClientCommunicationCenter.commCenter.SendMessageTo(ep, msg);
			}
		}
		public void RequestedStateAquired(int ep, int fileId, BitArray state) {
			FileTransferCenter.ftc.GetFdm(fileId).ClientsFileStateArrived(ep, state);
		}
	}
}
