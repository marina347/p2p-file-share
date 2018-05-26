using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CommonResources {
	public class NetworkResolver {
		public static int serverPort = 9005;
		public static int clientPort1 = 9003;
		public static int clientPort2 = 9004;
		public static int clientPort3 = 9002;

		private static string serverDomain = "www.marna.duckdns.org";

		public static EndPoint ResolveServerEndPoint() {
			IPHostEntry ipHostInfo = Dns.GetHostEntry(serverDomain); 
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			//return new IPEndPoint(ipAddress, serverPort);
			return new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), serverPort);
		}

		private static string GetLocalIPAddress() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					return ip.ToString();
				}
			}
			return null;
		}
		
		private static Socket MakeUdpSocket(EndPoint ep) {
			Socket socket = new Socket(
						AddressFamily.InterNetwork,
						SocketType.Dgram,
						ProtocolType.Udp
					);
			socket.Bind(ep);
			return socket;
		}
		
		public static Socket ProvideServerSocket() {
			return MakeUdpSocket(ResolveServerEndPoint());
		}

		public static Socket ProvideClientSocket(int port) {
			return MakeUdpSocket(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), port));
		}
	}
}
