using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CommonResources;

namespace client {
	class ClientCommunicationCenter: CommunicationCenter {
		public static ClientCommunicationCenter commCenter;

		private Dictionary<int, EndPoint> connectedClients;
		public Dictionary<int, EndPoint> ConnectedClients {
			get {
				return connectedClients;
			}

			set {
				connectedClients = value;
			}
		}
		public void SendMessageTo(EndPoint ep, OutputMessage om) {
			this.SendMessage(ep, om);
		}
		public void SendMessageToServer(OutputMessage om) {
			this.SendMessage(ApplicationLiveData.ald.ServerEndPoint, om);
		}

		protected override EndPoint GetClientEndPoint(int ep) {
			if (ep == -1)
				return ApplicationLiveData.ald.ServerEndPoint;
			else if (!connectedClients.ContainsKey(ep)) {
				return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969);
			}
			else
				return connectedClients[ep];
		}

		public bool ConnectedWith(int ep) {
			return connectedClients.ContainsKey(ep);
		}
		
		public ClientCommunicationCenter(Socket sok, int port) : base(sok, port) {
			connectedClients = new Dictionary<int, EndPoint>();
		}
	}
}
