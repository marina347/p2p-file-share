using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace server {
	public class ClientEndPoint {
		private IPEndPoint publicEndPoint;
		private IPEndPoint privateEndPoint;
		
		public ClientEndPoint(IPEndPoint publicEndPoint, IPEndPoint privateEndPoint) {
			this.publicEndPoint = publicEndPoint;
			this.privateEndPoint = privateEndPoint;
		}
		
		public byte[] PublicAddress {
			get {
				return publicEndPoint.Address.GetAddressBytes();
			}

		}

		public byte[] PrivateAddress {
			get {
				return privateEndPoint.Address.GetAddressBytes();
			}
		}

		public int PublicPort {
			get {
				return publicEndPoint.Port;
			}
		}

		public int PrivatePort {
			get {
				return privateEndPoint.Port;
			}
		}
		
		public IPEndPoint PublicEndPoint {
			get {
				return publicEndPoint;
			}

			set {
				publicEndPoint = value;
			}
		}

		public IPEndPoint PrivateEndPoint {
			get {
				return privateEndPoint;
			}

			set {
				privateEndPoint = value;
			}
		}
	}
}
