using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CommonResources;
using System.Windows.Forms;
using System.Diagnostics;

namespace client {
	class ClientNetworkInputHandler: NetworkInputHandler {
		bool initAll;
		private void HandleResponseConnect(S2CMessageReader mr) {
			Debug.WriteLine("Connected!");
			FileTransferCenter.ftc.ncm.ConnectionWithServerEstablished();
			FileTransferCenter.ftc.ts.AllLocalFilesStartTransfer();
			
			string delFiles = "";
			foreach(FileDescription fd in MasterFileManager.mfm.DeletedFiles.Values) {
				FileSearchAndRegistrationMessageProvider fsmp = new FileSearchAndRegistrationMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
				OutputMessage msg = fsmp.ProvideFileDeletedMessage(fd.FileId);
				ClientCommunicationCenter.commCenter.SendMessageToServer(msg);
				delFiles += fd.FileName+"."+fd.FileExtension+",";
			}
			
			if (delFiles != "") {
				delFiles.TrimEnd(',');
				MessageBox.Show("System has detected that some files: " + delFiles + Environment.NewLine + " have been deleted from hard drive!");
			}
			initAll = true;
		}
		private void HandleRequestedClientAvailable(S2CMessageReader mr) {
			UdpHpClient.uhc.RequestHolePunch(mr.ReadClientId());
		}

		private void HandleFileUploadResponse(S2CMessageReader mr) {
			if (mr.ReadFileAcceptance() == true) {
				Debug.WriteLine("File accepted:" + mr.ReadAcceptedFileId());
				FileRegistrationManager.frm.Fd.FileId = mr.ReadAcceptedFileId();
				FileRegistrationManager.frm.FileAccepted();
				FileRegistrationManager.frm.SendAllHashes();
				MessageBox.Show("File is registered successfully! Sharing will be started in a few moments!");
			}
			else {
				MessageBox.Show("Error found while registering file! It maybe exists in system!");
				FileRegistrationManager.frm.CloseRegistrationProcess();
			}
		}

		private void HandleResendChunkHashRequest(S2CMessageReader mr) {
            lock (FileRegistrationManager.frm) {
                if (FileRegistrationManager.frm.Accepted == false) {
                    FileRegistrationManager.frm.Fd.FileId = mr.ReadResendChunkIdFileId();
                    FileRegistrationManager.frm.FileAccepted();
                    FileRegistrationManager.frm.SendAllHashes();
                }
                else {
                    FileRegistrationManager.frm.ResendHash(mr.ReadResendChunkId());
                }
            }
		}

		private void HandleFileRegisterFinished(S2CMessageReader mr) {
			FileDescription fd = FileRegistrationManager.frm.Fd;
			FileRegistrationManager.frm.CloseRegistrationProcess();
			FileTransferCenter.ftc.ts.LocalFileInitTransfer(fd);
			
		}

		private void HandleFileClientListResponse(S2CMessageReader mr) {
			int fileId = mr.ReadClientListFileId();
			FileTransferCenter.ftc.GetFdm(fileId).ClientListArrived(mr.ReadClientsList());
		}
		private void HandleChunkHashResponse(S2CMessageReader mr) {
			ClientHashReceiver.chr.Hm.HashArrived(mr.ReadChunkHash());
		}
		
		private void HandleHolePunchSuceeded(C2CMessageReader mr, EndPoint endPoint) {
			UdpHpClient.uhc.OnHolePunchSuceeded(mr.ClientEP(), endPoint);
		}

		private void HandleGotHolePunch(C2CMessageReader mr, EndPoint endPoint) {
			UdpHpClient.uhc.OnGotHolePunch(mr.ClientEP(), endPoint);
		}

		private void HandleTryHolePunch(C2CMessageReader mr) {
			UdpHpClient.uhc.OnTryHolePunch(mr.ProvidedEndPoint());
		}
		private void HandleFileStateRequest(C2CMessageReader mr) {
			//if (ClientCommunicationCenter.commCenter.ConnectedWith(mr.ClientEP()))
				FileTransferCenter.ftc.nfsm.FileStateRequested(
				mr.ClientEP(), 
				mr.ReadFileId());
		}

		private void HandleFileStateResponse(C2CMessageReader mr) {
			//if (ClientCommunicationCenter.commCenter.ConnectedWith(mr.ClientEP()))
				FileTransferCenter.ftc.nfsm.RequestedStateAquired(
				mr.ClientEP(), 
				mr.ReadFileId(), 
				mr.ReadChunksState());
		}

		private void HandleSendChunksRequest(C2CMessageReader mr) {
			//if (ClientCommunicationCenter.commCenter.ConnectedWith(mr.ClientEP()))
				FileTransferCenter.ftc.ndm.ChunksRequested(
				mr.ClientEP(), 
				mr.ReadFileId(), 
				mr.ReadRequestedChunks());
		}

		private void HandleChunkChunkData(C2CMessageReader mr) {
			//if (ClientCommunicationCenter.commCenter.ConnectedWith(mr.ClientEP()))
				FileTransferCenter.ftc.ndm.ChunksPartArrived(
				mr.ClientEP(), 
				mr.ReadFileId(), 
				mr.ReadChunkId(), 
				mr.ReadChunkPartId(), 
				mr.ReadChunkPartData());
		}

		private void HandleChunkPartResendRequest(C2CMessageReader mr) {
			//if (ClientCommunicationCenter.commCenter.ConnectedWith(mr.ClientEP()))
				FileTransferCenter.ftc.ndm.ResendChunkPartRequested(
				mr.ClientEP(),
				mr.ReadFileId(),
				mr.ReadChunkId(),
				mr.ReadChunkPartId());
		}
		private void HandleFileSearchByNameResponse(S2CMessageReader mr) {
			List<FileDescription> list = mr.ReadFileDescriptions();
			FrmSearch.frmSearch.ShowSearchResults(list);
		}
		private void HandleApplicationRegisterResponse(S2CMessageReader mr) {
			int id = mr.ReadDesignatedAppId();
			MasterFileManager.mfm.ApplicationIdAquired(id);
		}
		public static ClientNetworkInputHandler nic;
		public void HandleMessage(EndPoint remoteEndPoint, byte[] msg) {
			if (ApplicationLiveData.ald==null
				||ApplicationLiveData.ald.ServerEndPoint == null 
				||(!remoteEndPoint.Equals(ApplicationLiveData.ald.ServerEndPoint) && initAll == false))
				return;
			
			MessageReader mr = new MessageReader(msg);

			switch (mr.MsgType()) {
				
				case MessageType.s2c_ConnectResponse:
					this.HandleResponseConnect(new S2CMessageReader(msg));
					break;
				case MessageType.s2c_RequestedClientAvailable:
					this.HandleRequestedClientAvailable(new S2CMessageReader(msg));
					break;
				case MessageType.ft_FileRegisterResponse:
					this.HandleFileUploadResponse(new S2CMessageReader(msg));
					break;
				case MessageType.ft_ResendChunkHashRequest:
					this.HandleResendChunkHashRequest(new S2CMessageReader(msg));
					break;
				case MessageType.ft_FileRegisterFinished:
					this.HandleFileRegisterFinished(new S2CMessageReader(msg));
					break;
				case MessageType.s2c_FileClientListResponse:
					this.HandleFileClientListResponse(new S2CMessageReader(msg));
					break;
				case MessageType.ft_ChunkHash:
					this.HandleChunkHashResponse(new S2CMessageReader(msg));
					break;
				
				case MessageType.s2c_TryHolePunch:
					this.HandleTryHolePunch(new C2CMessageReader(msg));
					break;
				case MessageType.s2c_TryLocalHolePunch:
					this.HandleTryHolePunch(new C2CMessageReader(msg));
					break;
				case MessageType.c2c_GotHolePunch:
					this.HandleGotHolePunch(new C2CMessageReader(msg), remoteEndPoint);
					break;
				case MessageType.c2c_HolePunchSuceeded:
					this.HandleHolePunchSuceeded(new C2CMessageReader(msg), remoteEndPoint);
					break;
				
				case MessageType.ft_FileStateRequest:
					this.HandleFileStateRequest(new C2CMessageReader(msg));
					break;
				case MessageType.ft_FileStateResponse:
					this.HandleFileStateResponse(new C2CMessageReader(msg));
					break;
				case MessageType.ft_SendChunksRequest:
					this.HandleSendChunksRequest(new C2CMessageReader(msg));
					break;
				case MessageType.ft_ChunkPartData:
					this.HandleChunkChunkData(new C2CMessageReader(msg));
					break;
				case MessageType.ft_ChunkPartResendRequest:
					this.HandleChunkPartResendRequest(new C2CMessageReader(msg));
					break;
				
				case MessageType.s2c_FileSearchByNameResponse:
					this.HandleFileSearchByNameResponse(new S2CMessageReader(msg));
					break;
				case MessageType.c2s_ApplicationRegisterResponse:
					this.HandleApplicationRegisterResponse(new S2CMessageReader(msg));
					break;


				default:
					break;
			}
		}
	}
}
