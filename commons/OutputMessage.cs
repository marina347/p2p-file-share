using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public class OutputMessage {
		private int msgLength;
		private byte[] msg;
		
		public OutputMessage(byte[] msg, int msgLength) {
			this.msgLength = msgLength;
			this.msg = msg;
		}
		public int MsgLength {
			get {
				return msgLength;
			}

			set {
				msgLength = value;
			}
		}

		public byte[] Msg {
			get {
				return msg;
			}

			set {
				msg = value;
			}
		}
	}
}
