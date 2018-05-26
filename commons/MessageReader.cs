using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Diagnostics;

namespace CommonResources {
	public class MessageReader {
		protected byte[] msg;

		protected int s(int multiplier) {
			return sizeof(int) * multiplier;
		}

		public MessageReader(byte[] msgInput) {
			this.msg = msgInput;
		}

		public int readClientId() {
			if (msg.Length == 0) {
				Debug.WriteLine("FALSE MESSAGE");
				return -1;
			}
			return BitConverter.ToInt32(msg, 4);
		}

		public MessageType MsgType() {
			if(msg.Length == 0) {
				Debug.WriteLine("FALSE MESSAGE");
				return (MessageType) (- 1);
			}
			return (MessageType)BitConverter.ToInt32(msg, 0);
		}

	}
}
