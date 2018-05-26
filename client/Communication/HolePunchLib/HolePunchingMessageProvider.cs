using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class HolePunchingMessageProvider: MessageProvider {
		
		public OutputMessage ProvideRequestHolePunchMessage(int ep) {
			AddData(MessageType.c2s_HolePunchRequest);
			AddData(ApplicationLiveData.ald.ApplicationId);
			AddData(ep);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideTryHolePunchMessage() {
			AddData(MessageType.c2c_GotHolePunch);
			AddData(ApplicationLiveData.ald.ApplicationId);
			return this.GenerateOutputMessage();
		}

		public OutputMessage ProvideHolePunchSuceededMessage() {
			AddData(MessageType.c2c_HolePunchSuceeded);
			AddData(ApplicationLiveData.ald.ApplicationId);
			return this.GenerateOutputMessage();
		}

		public HolePunchingMessageProvider(byte[] array):base(array) {

		}
	}
}
