using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CommonResources {
	public interface NetworkInputHandler {
		void HandleMessage(EndPoint remoteEndPoint, byte[] msg);
	}
}
