using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommonResources;
using System.Diagnostics;

namespace server {
	public class UdpHpServer {
		public static UdpHpServer uhps;
		public UdpHpServer() {

		}

		private void SendHolePunchData(int ep1,int  ep2) {
            Debug.WriteLine("send HP"+ep1+"|"+ep2);
            ServerMessageProvider smpFor2 = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage omFor2 = smpFor2.ProvideTryHolePunchMessage(ServerCommunicationCenter.commCenter.ClientsEndPoint[ep1].PublicEndPoint);

			ServerMessageProvider smpFor1 = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage omFor1 = smpFor1.ProvideTryHolePunchMessage(ServerCommunicationCenter.commCenter.ClientsEndPoint[ep2].PublicEndPoint);

			ServerCommunicationCenter.commCenter.SendMessageTo(ep1, omFor1);
			ServerCommunicationCenter.commCenter.SendMessageTo(ep2, omFor2);
		}

		private void SendLocalHolePunchData(int requester, int ep2) {
			ServerMessageProvider smpFor2 = new ServerMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage omFor2 = smpFor2.ProvideTryLocalHolePunchMessage(ServerCommunicationCenter.commCenter.ClientsEndPoint[requester].PrivateEndPoint);
			ServerCommunicationCenter.commCenter.SendMessageTo(ep2, omFor2);
		}
		
		public void OnRequestHolePunch(int requester, int ep2) {
            Debug.WriteLine("send LOCAL HP" + requester + "|" + ep2);
            
            if (ASCIIEncoding.ASCII.GetString(ServerCommunicationCenter.commCenter.ClientsEndPoint[requester].PublicAddress) ==
				ASCIIEncoding.ASCII.GetString(ServerCommunicationCenter.commCenter.ClientsEndPoint[ep2].PublicAddress)){
				SendLocalHolePunchData(requester, ep2);
			}
			else{
				SendHolePunchData(requester, ep2);
			}
		}
	}
}
