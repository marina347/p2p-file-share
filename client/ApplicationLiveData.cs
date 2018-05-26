using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace client {
	public class ApplicationLiveData {
		public static ApplicationLiveData ald;
		int applicationId;
		bool ready;
		
		EndPoint serverEndPoint;
		EndPoint localEndPoint;
		int port;

		public ApplicationLiveData(int portUlaz) {
			applicationId = -1;
			ready = false;
			this.Port = portUlaz;
		}

		public int ApplicationId {
			get {
				return applicationId;
			}

			set {
				applicationId = value;
			}
		}

		public bool Ready {
			get {
				return ready;
			}

			set {
				ready = value;
			}
		}

		public EndPoint ServerEndPoint {
			get {
				return serverEndPoint;
			}

			set {
				serverEndPoint = value;
			}
		}
		public EndPoint LocalEndPoint {
			get {
				return localEndPoint;
			}

			set {
				localEndPoint = value;
			}
		}

		public int Port {
			get {
				return port;
			}

			set {
				port = value;
			}
		}
	}
}
