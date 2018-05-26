using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace CommonResources {
	public abstract class CommunicationCenter {
		private Socket socket;
		int port;

		public Socket Socket {
			get {
				return socket;
			}
		}

		public CommunicationCenter(Socket sok, int port) {
			socket = sok;
			this.port = port;
		}
		
		public void StartListening() {
			Socket.ReceiveFromAsync(
				SocketResourcesManager.srm.ProvideRecvSargs(
					new EventHandler<SocketAsyncEventArgs>(MsgReceived),
					port
				)
			);
		}

		private void MsgReceived(object sender, SocketAsyncEventArgs saea) {
			Socket.ReceiveFromAsync(
				SocketResourcesManager.srm.ProvideRecvSargs(
					new EventHandler<SocketAsyncEventArgs>(MsgReceived),
					port
				)
			);
			SocketResourcesManager.srm.ProcessBuffer(saea);
			
		}

		public void SendMessage(EndPoint ep, OutputMessage om) {
			SocketAsyncEventArgs sarg =
				SocketResourcesManager.srm.ProvideSendSargs(
					om.Msg,
					om.MsgLength,
					ep,
					null
				);
			bool k = Socket.SendToAsync(sarg);
			
		}
		public void SendMessageTo(int ep, OutputMessage om) {
			EndPoint endPoint = GetClientEndPoint(ep);
			SendMessage(endPoint, om);
		}

		protected abstract EndPoint GetClientEndPoint(int ep);
	}
}
