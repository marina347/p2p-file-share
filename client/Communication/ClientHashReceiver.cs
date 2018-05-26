using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	public class ClientHashReceiver: HashManagerListener {
		public static ClientHashReceiver chr;
		FileDescription fd;
		HashManager hm;

		public ClientHashReceiver() {
			
		}

		public void InitReceiver(FileDescription fd) {
			this.fd = fd;
			hm = new HashManager(-1, Sizes.GetChunksNumber(fd.FileSize), fd.FileId, this);
			DataMessageProvider dmp = new DataMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = dmp.ProvideFileHashTransferRequestMessage(fd.FileId);
			ClientCommunicationCenter.commCenter.SendMessageToServer(om);
		}

		public void HashDownloadFinished(ChunkHash[] hashes, int fileId, int senderId) {
			
			hashes = hashes.OrderBy(item=>item.ChunkId).ToArray();
			FileTransferCenter.ftc.ts.NewFileStartTransfer(fd, hashes);
		}

		public void RequestResend(int chunkHashId, int senderId, int fileId) {
			DataMessageProvider dmp = new DataMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = dmp.ProvideResendHashRequestMessage(Fd.FileId, chunkHashId);
			ClientCommunicationCenter.commCenter.SendMessageTo(senderId, om);
		}

		public HashManager Hm {
			get {
				return hm;
			}
		}

		public FileDescription Fd {
			get {
				return fd;
			}
		}
	}
}
