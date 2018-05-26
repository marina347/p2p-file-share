using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CommonResources;

namespace client {
	class ConnectivityMessageProvider: MessageProvider {
		public OutputMessage ProvideTryEstablishConnectionMessage(int ep) {
			AddData(MessageType.c2s_ClientAvailabilityRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(ep);
			return this.GenerateOutputMessage();
		}
		
		public OutputMessage ProvideFileClientListRequest(int fileId) {
			AddData(MessageType.c2s_FileClientListRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(fileId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideTryConnectRequest() {
			AddData(MessageType.c2s_ConnectRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(((IPEndPoint)ApplicationLiveData.ald.LocalEndPoint).Address.GetAddressBytes());
			AddData(((IPEndPoint)ApplicationLiveData.ald.LocalEndPoint).Port);
			return this.GenerateOutputMessage();
		}

		public ConnectivityMessageProvider(byte[] buffer) : base(buffer) {

		}
	}
}
