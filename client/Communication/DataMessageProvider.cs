using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class DataMessageProvider :MessageProvider {
		public OutputMessage ProvideSendChunkPartMessage(int fileId, int chunkId, int chunkPartId, byte[] data, int start, int count) {
			AddData(MessageType.ft_ChunkPartData);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(chunkId);
			AddData(chunkPartId);
			AddData(data, start, count);
			return this.GenerateOutputMessage(); ;
		}

		public OutputMessage ProvideRequestChunkPartResendMessage(int fileId, int chunkId, int chunkPartId) {
			AddData(MessageType.ft_ChunkPartResendRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(chunkId);
			AddData(chunkPartId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideRequestChunksMessage(int fileId,int[] chunks) {
			AddData(MessageType.ft_SendChunksRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(chunks);
			return this.GenerateOutputMessage();
		}
		
		public OutputMessage ProvideResendHashRequestMessage(int fileId, int chunkId) {
			AddData(MessageType.ft_ResendChunkHashRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(chunkId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileHashTransferRequestMessage(int fileId) {
			AddData(MessageType.ft_FileHashTransferRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public DataMessageProvider(byte[] buffer) : base(buffer) {

		}
	}
}
