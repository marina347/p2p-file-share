using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommonResources;

namespace server {
	class ServerCommunicationCenter:CommunicationCenter {
		public static ServerCommunicationCenter commCenter;

		private Dictionary<int, ClientEndPoint> clientsEndPoint;
		protected override EndPoint GetClientEndPoint(int ep) {
			return ClientsEndPoint[ep].PublicEndPoint;
		}

		public Dictionary<int, ClientEndPoint> ClientsEndPoint {
			get {
				return clientsEndPoint;
			}

			set {
				clientsEndPoint = value;
			}
		}

		public void ClientConnected(int ep, ClientEndPoint cep) {
			lock (clientsEndPoint) { 
				if (clientsEndPoint.ContainsKey(ep)) {
					clientsEndPoint[ep] = cep;
				}
				else {
					clientsEndPoint.Add(ep, cep);
				}
			}
		}

		public ServerCommunicationCenter(Socket sok, int port) : base(sok, port) {
			clientsEndPoint = new Dictionary<int, ClientEndPoint>();
		}
	}
}
