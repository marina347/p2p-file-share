using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;
using System.Diagnostics;

namespace client {
	public class UdpHpClient {
		public static UdpHpClient uhc;
		public UdpHpClient() {
		}

		public void RequestHolePunch(int remoteEP) {
			Debug.WriteLine("requesting HP:" + remoteEP);
			HolePunchingMessageProvider hpmp = new HolePunchingMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = hpmp.ProvideRequestHolePunchMessage(remoteEP);
			ClientCommunicationCenter.commCenter.SendMessageToServer(om);
		}

		public void OnTryHolePunch(EndPoint otherClient) {
			Debug.WriteLine("TRY HP:" + otherClient);
			HolePunchingMessageProvider hpmp = new HolePunchingMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = hpmp.ProvideTryHolePunchMessage();
			ClientCommunicationCenter.commCenter.SendMessageTo(otherClient, om);
		}

		public void OnGotHolePunch(int ep, EndPoint endPoint) {
			Debug.WriteLine("GOT HP:" + ep + "address:" + endPoint);
			HolePunchingMessageProvider hpmp = new HolePunchingMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage om = hpmp.ProvideHolePunchSuceededMessage();
			ClientCommunicationCenter.commCenter.SendMessageTo(endPoint, om);
			OnHolePunchSuceeded(ep, endPoint);
		}

		public void OnHolePunchSuceeded(int ep, EndPoint endPoint) {
			
			FileTransferCenter.ftc.ncm.ConnectionEstablished(ep, endPoint);
		}

	}
}
