using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace CommonResources {
	public class SocketResourcesManager {
		public static SocketResourcesManager srm;
		NetworkInputHandler nih;
		Stack<byte[]> buffers;
		private void PopulateBuffers() {
			for (int i = 0; i < 5000; i++) {
				buffers.Push(new byte[1536]);
			}
		}

		private void SocketSendFinished(object sender, SocketAsyncEventArgs e) {
			lock (buffers) {
				buffers.Push(e.Buffer);
			}
		}

		public SocketResourcesManager(NetworkInputHandler nihInput) {
			this.nih = nihInput;
			buffers = new Stack<byte[]>();
			PopulateBuffers();
		}

		public byte[] ProvideBuffer() {
			lock (buffers) {
				return buffers.Pop();
			}
		}
		
		public SocketAsyncEventArgs ProvideSendSargs(byte[] msg, int msgLength, EndPoint ep, EventHandler<SocketAsyncEventArgs> givenMethodInput) {
            try {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(msg, 0, msgLength);
                saea.RemoteEndPoint = ep;
                saea.Completed += new EventHandler<SocketAsyncEventArgs>(SocketSendFinished);
                if (givenMethodInput != null)
                    saea.Completed += givenMethodInput;
                return saea;
            }
            catch {
                return null;
            }
		}
		
		public SocketAsyncEventArgs ProvideRecvSargs(EventHandler<SocketAsyncEventArgs> givenMethodInput, int port) {
            try {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                saea.SetBuffer(this.ProvideBuffer(), 0, 1536);
                if (givenMethodInput != null)
                    saea.Completed += givenMethodInput;
                saea.RemoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                return saea;
            }
            catch {
                return null;
            }
		}

		private void CopyAndForwardData(object obj) {
			SocketAsyncEventArgs saea = obj as SocketAsyncEventArgs;
			byte[] data = new byte[saea.BytesTransferred];
			Buffer.BlockCopy(saea.Buffer, 0, data, 0, saea.BytesTransferred);
			lock (buffers) {
				buffers.Push(saea.Buffer);
			}
			this.nih.HandleMessage(saea.RemoteEndPoint, data);
		}

		public void ProcessBuffer(SocketAsyncEventArgs saea) {
			CopyAndForwardData(saea);
		}
	}
}
