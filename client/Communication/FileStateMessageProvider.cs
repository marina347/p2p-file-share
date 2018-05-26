using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class FileStateMessageProvider: MessageProvider {
		public OutputMessage ProvideResponseFileStateMessage(int fileId, byte[] fs) {
			AddData(MessageType.ft_FileStateResponse);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			AddData(fs);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideRequestFileStateMessage(int fileId) {
			AddData(MessageType.ft_FileStateRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}
		public FileStateMessageProvider(byte[] buffer):base(buffer) {

		}
	}
}
