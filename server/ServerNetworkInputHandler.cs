using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CommonResources;
using server.BazaF;
using System.Diagnostics;

namespace server {
	public class ServerNetworkInputHandler: NetworkInputHandler {

		public void HandleRequestConnect(EndPoint publicEndPoint, C2SMessageReader mr) {
			Debug.WriteLine("SR: Client"+mr.ClientEP()+" connected");
			int clientEp = mr.ClientEP();
			if(clientEp == -1) {
				Peer p = new Peer();
				p.LastVisit = DateTime.Now;
				p.IpAddress = IPAddress.Any;
				int id = p.Add();
				ServerCommunicationCenter.commCenter.ClientConnected(id, new ClientEndPoint((IPEndPoint)publicEndPoint, mr.ClientPrivateEP()));
				ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
				OutputMessage om = smp.ProvideApplicationRegisterResponseMessage(id);
				ServerCommunicationCenter.commCenter.SendMessage(publicEndPoint, om);
			}
			else {
				ServerCommunicationCenter.commCenter.ClientConnected(mr.ClientEP(), new ClientEndPoint((IPEndPoint)publicEndPoint, mr.ClientPrivateEP()));
				ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
				OutputMessage om = smp.ProvideConnectedMessage();
				ServerCommunicationCenter.commCenter.SendMessageTo(mr.ClientEP(), om);
			}
		}

		public void HandleHolePunchRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            UdpHpServer.uhps.OnRequestHolePunch(mr.ClientEP(), mr.PunchClientEP());
		}

		public void HandleFileClientListRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            int fileId = mr.ReadFileId();
			List<int> clientList = File.GetFileOwnersId(mr.ReadFileId());
			clientList.Remove(mr.ClientEP());
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = smp.ProvideClientListMessage(fileId, clientList);
			ServerCommunicationCenter.commCenter.SendMessageTo(mr.ClientEP(), om);
		}
		
		public void HandleRequestFileUpload(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            Debug.WriteLine("SR: Received request for registration");
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om;
			string fileFullName = mr.ReadRegisteringFileFullName();
			string ext = Utils.GetFileExtensionFromPath(fileFullName);
			string name = Utils.GetFileNameFromPath(fileFullName);

			bool duplikat = File.Exists(name, ext, mr.ReadRegisteringFileSize());
			if (!duplikat) {
				File f = new File();
				f.ApplicationId = mr.ClientEP();
				f.FileExtension = ext;
				f.FileName = name;
				f.FileSize = mr.ReadRegisteringFileSize();
				int fileId = f.Add();
				om = smp.ProvidePositiveUploadFileMessage(fileId);
				HashManager shr = new HashManager(mr.ClientEP(), Sizes.GetChunksNumber(mr.ReadRegisteringFileSize()), fileId, new ServerHashManagerListener());
				Debug.WriteLine("File: " + fileId);
				Debug.WriteLine("FileSize: " + f.FileSize);
				ServerObjectGraph.sog.FileRegistrations.Add(fileId, shr);
				Debug.WriteLine("SR: Registration approved");
			}
			else
				om = smp.ProvideNegativeUploadFileMessage();
			ServerCommunicationCenter.commCenter.SendMessageTo(mr.ClientEP(), om);
		}

		public void HandleNewChunkHash(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            ChunkHash ch = mr.ReadChunkHash();
			int id = mr.ReadRegisteringFileId();
            Debug.WriteLine("hash for:"+ id);
            if (!ServerObjectGraph.sog.FileRegistrations.ContainsKey(id)) {
                return;
            }
			ServerObjectGraph.sog.FileRegistrations[id].HashArrived(ch);
		}

		public void HandleClientAvailabilityRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            if (ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ReadRequestedClientId())) {
				ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
				OutputMessage om = smp.ProvideClientAvailableMessage(mr.ReadRequestedClientId());
				ServerCommunicationCenter.commCenter.SendMessageTo(mr.ClientEP(), om);
			}
		}
		public void HandleFileHashTransferRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            HashSender.SendAllHashes(mr.ClientEP(), mr.ReadFileId());
			Owner owner = new Owner();
			owner.ApplicationId = mr.ClientEP();
			owner.FileId = mr.ReadFileId();
			owner.Add();
		}

		public void HandleResendChunkHashRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            HashSender.ResendHash(mr.ClientEP(), mr.ReadFileId(), mr.ReadChunkId());
		}

		public void HandleFileSearchByNameRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            string keyword = mr.ReadFileNameKeyword();
			List<File> files = File.GetFiles(keyword);
			ServerMessageProvider smp = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = smp.ProvideFileSearchByNameMessage(files);
			ServerCommunicationCenter.commCenter.SendMessageTo(mr.ClientEP(), om);
		}

		public void HandleFileDeletedRequest(C2SMessageReader mr) {
            if (!ServerCommunicationCenter.commCenter.ClientsEndPoint.ContainsKey(mr.ClientEP()))
                return;
            int clientEp = mr.ClientEP();
			int fileId = mr.ReadFileId();
			int affectedRows = Owner.Delete(clientEp, fileId);
		}

		public void HandleMessage(EndPoint endPoint, byte[] msg) {
            C2SMessageReader mr = new C2SMessageReader(msg);
			switch (mr.MsgType()) {
				case MessageType.c2s_ConnectRequest:
					HandleRequestConnect(endPoint, mr);
					break;
				case MessageType.c2s_HolePunchRequest:
					HandleHolePunchRequest(mr);
					break;
				case MessageType.c2c_CloseConnection:
					break;
				case MessageType.ft_FileRegistrationRequest:
					HandleRequestFileUpload(mr);
					break;
				case MessageType.ft_ChunkHash:
					HandleNewChunkHash(mr);
					break;
				case MessageType.c2s_FileClientListRequest:
					HandleFileClientListRequest(mr);
					break;
				case MessageType.c2s_ClientAvailabilityRequest:
					HandleClientAvailabilityRequest(mr);
					break;
				case MessageType.ft_FileHashTransferRequest:
					HandleFileHashTransferRequest(mr);
					break;
				case MessageType.ft_ResendChunkHashRequest:
					HandleResendChunkHashRequest(mr);
					break;
				case MessageType.c2s_FileSearchByNameRequest:
					HandleFileSearchByNameRequest(mr);
					break;

				case MessageType.c2s_FileDeleted:
					HandleFileDeletedRequest(mr);
					break;
				default:
					break;
			}
		}
	}
}
