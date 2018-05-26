using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.Net;
using server.BazaF;

namespace server {
	public class ServerMessageProvider: MessageProvider {
		public ServerMessageProvider(byte[] b):base(b) {

		}

		public OutputMessage ProvideConnectedMessage() {
			AddData(MessageType.s2c_ConnectResponse);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideIsConnectedMessage() {
			return this.GenerateOutputMessage();
		}

		private void SetHolePunchData(MessageType msgType, IPEndPoint endPoint) {
			AddData(msgType);
			AddData(endPoint.Address.GetAddressBytes());
			AddData(endPoint.Port);
		}

		public OutputMessage ProvideTryHolePunchMessage(IPEndPoint endPoint) {
			SetHolePunchData(MessageType.s2c_TryHolePunch, endPoint);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideTryLocalHolePunchMessage(IPEndPoint endPoint) {
			SetHolePunchData(MessageType.s2c_TryLocalHolePunch, endPoint);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideResendHashRequestMessage(int chunkId, int fileId) {
			AddData(MessageType.ft_ResendChunkHashRequest);
			AddData(chunkId);
            AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideChunkHashDataMessage(int chunkId, byte[] hashValue) {
			AddData(MessageType.ft_ChunkHash);
			AddData(chunkId);
			AddData(hashValue);
			return this.GenerateOutputMessage();
		}
		public OutputMessage ProvideConnectionStateMessage(int state) {
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideNegativeUploadFileMessage() {
			AddData(MessageType.ft_FileRegisterResponse);
			AddData(false);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvidePositiveUploadFileMessage(int fileId) {
			AddData(MessageType.ft_FileRegisterResponse);
			AddData(true);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileRegisteredMessage(int fileId) {
			AddData(MessageType.ft_FileRegisterFinished);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideClientListMessage(int fileId, List<int> clients) {
			AddData(MessageType.s2c_FileClientListResponse);
			AddData(fileId);
			AddData(clients.ToArray());
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideClientAvailableMessage(int clientId) {
			AddData(MessageType.s2c_RequestedClientAvailable);
			AddData(clientId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileSearchByNameMessage(List<File> files) {
			AddData(MessageType.s2c_FileSearchByNameResponse);
			foreach (File f in files) {
				AddData(f.FileId);
				AddData(f.FileSize);
				AddData(f.FileName.Length + f.FileExtension.Length + 1);
				AddData(f.FileName + "\\" + f.FileExtension);
			}
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideApplicationRegisterResponseMessage(int id) {
			AddData(MessageType.c2s_ApplicationRegisterResponse);
			AddData(id);
			return this.GenerateOutputMessage();
		}
		
	}
}
