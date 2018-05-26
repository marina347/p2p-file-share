using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	public class FileSearchAndRegistrationMessageProvider : MessageProvider {
		public OutputMessage ProvideRequestFileRegistration(String fullFileName, uint fileSize) {
			AddData(MessageType.ft_FileRegistrationRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileSize);
			AddData(fullFileName);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideResponseChunksMessage(int fileId, int chunkId, byte[] hashValue) {
			AddData(MessageType.ft_ChunkHash);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(chunkId);
			AddData(hashValue);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileTransferRequestMessage(int fileId) {
			AddData(MessageType.ft_FileInitTransferRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileSearchByKeywordRequestMessage(string keyword) {
			AddData(MessageType.c2s_FileSearchByNameRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(keyword);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideFileDeletedMessage(int fileId) {
			AddData(MessageType.c2s_FileDeleted);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}
		public FileSearchAndRegistrationMessageProvider(byte[] buffer) : base(buffer) {

		}
	}
}
