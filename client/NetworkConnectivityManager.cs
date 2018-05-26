using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using CommonResources;

namespace client {
	class NetworkConnectivityManager {
		Dictionary<int, Queue<FileDownloadManager>> requestedConnections;
		
		Dictionary<int, Stopwatch> requestedTime;
		Timer timerRequestedConnections;
		
		Dictionary<int, Queue<FileDownloadManager>> offlineClients;
		
		Timer timerOfflineClients;
		Timer timerServerConnect;

		public NetworkConnectivityManager() {
			requestedConnections = new Dictionary<int, Queue<FileDownloadManager>>();
			requestedTime = new Dictionary<int, Stopwatch>();
			offlineClients = new Dictionary<int, Queue<FileDownloadManager>>();
            
			timerOfflineClients = new Timer();
			timerOfflineClients.Tick += new EventHandler(RunOfflineConnections);
			timerOfflineClients.Interval = 150000;
			timerOfflineClients.Start();

			timerRequestedConnections = new Timer();
			timerRequestedConnections.Tick += new EventHandler(RunRequestedConnections);
			timerRequestedConnections.Interval = 5000;
			timerRequestedConnections.Start();

			timerServerConnect = new Timer();
			timerServerConnect.Tick += new EventHandler(ConnectWithServer);
			timerServerConnect.Interval = 1500;
			timerServerConnect.Start();
		}

		public void ConnectionWithServerEstablished() {
			timerServerConnect.Stop();
		}
		public void ConnectWithServer(object sender, EventArgs e) {
			ConnectivityMessageProvider cmp = new ConnectivityMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = cmp.ProvideTryConnectRequest();
			ClientCommunicationCenter.commCenter.SendMessageTo(ApplicationLiveData.ald.ServerEndPoint, msg);
            Debug.WriteLine("Poksuavm se povezat");
		}

		private void TryEstablishConnection(int ep) {
			Debug.WriteLine("TryEstablishConnection: "+ep);
			ConnectivityMessageProvider cmp = new ConnectivityMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
			OutputMessage msg = cmp.ProvideTryEstablishConnectionMessage(ep);
			ClientCommunicationCenter.commCenter.SendMessageTo(ApplicationLiveData.ald.ServerEndPoint, msg);
		}

		private void RunRequestedConnections(object sender, EventArgs e) {
			lock (requestedConnections) {
				foreach (int ep in requestedConnections.Keys) {
					TryEstablishConnection(ep);
				}
				List<int> epsToRemove = new List<int>();
				
				foreach (int ep in requestedTime.Keys) {
					
					if (requestedTime[ep].ElapsedMilliseconds > 45000) {
						
						requestedTime[ep].Stop();
						
						epsToRemove.Add(ep);
					}
				}
				foreach (int ep in epsToRemove) {
					requestedTime.Remove(ep);
					
					offlineClients.Add(ep, requestedConnections[ep]);
					requestedConnections.Remove(ep);
				}
			}
		}

		private void RunOfflineConnections(object sender, EventArgs e) {
			Debug.WriteLine("RunOfflineConnections ");
			foreach (int ep in offlineClients.Keys) {
				TryEstablishConnection(ep);
			}
		}

		public void ClientAvailable(int ep) {
			UdpHpClient.uhc.RequestHolePunch(ep);
			
		}
		
		public void ConnectionEstablished(int ep, EndPoint endPoint) {

			lock (requestedConnections) {
				Debug.WriteLine("SPOJEN SA " + ep);
				if (ClientCommunicationCenter.commCenter.ConnectedClients.ContainsKey(ep))
					return;
				else {

					Queue<FileDownloadManager> fdmWaiting = null;
					ClientCommunicationCenter.commCenter.ConnectedClients.Add(ep, endPoint);
					if (requestedConnections.ContainsKey(ep)) {
						fdmWaiting = requestedConnections[ep];
						requestedConnections.Remove(ep);
						requestedTime.Remove(ep);
					}
					else if (offlineClients.ContainsKey(ep)) {
						fdmWaiting = offlineClients[ep];
						offlineClients.Remove(ep);
					}
					
					if (fdmWaiting != null)
						foreach (FileDownloadManager fdm in fdmWaiting) {
							fdm.NewClientAvailable(ep);
						}
				}
			}

		}
		public void RequestConnection(FileDownloadManager fdm, List<int> klijenti) {
			foreach (int ep in klijenti) {
				NewRequest(fdm, ep);
			}
		}

		public void NewRequest(FileDownloadManager fdm, int ep) {
			
			lock (requestedConnections) { 
				if (ClientCommunicationCenter.commCenter.ConnectedClients.ContainsKey(ep)) {
					fdm.NewClientAvailable(ep);
				}
				else if(requestedConnections.ContainsKey(ep)){
					
					Queue<FileDownloadManager> fdms = requestedConnections[ep];
					fdms.Enqueue(fdm);
				}
				else if(offlineClients.ContainsKey(ep)) {
					Queue<FileDownloadManager> fdms = offlineClients[ep];
					fdms.Enqueue(fdm);
				}
				else {
					
					Queue<FileDownloadManager> fdms = new Queue<FileDownloadManager>();
					fdms.Enqueue(fdm);
					requestedConnections.Add(ep, fdms);
					Stopwatch sp = new Stopwatch();
					sp.Start();
					requestedTime.Add(ep, sp);
					
				}
			}
		}
	}
}
