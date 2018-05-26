using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace server {
	class ServerObjectGraph {
		public static ServerObjectGraph sog;
		public ServerObjectGraph() {
			fileRegistrations = new Dictionary<int, HashManager>();
			hashSenders = new Dictionary<int, HashSender>();
		}

		Dictionary<int, HashManager> fileRegistrations;
		Dictionary<int, HashSender> hashSenders;

		internal Dictionary<int, HashManager> FileRegistrations {
			get {
				return fileRegistrations;
			}
		}

		internal Dictionary<int, HashSender> HashSenders {
			get {
				return hashSenders;
			}

			set {
				hashSenders = value;
			}
		}
	}
}
