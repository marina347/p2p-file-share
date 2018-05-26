using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public abstract class MessageProvider {
		protected int offset;
		protected byte[] msg;

		protected void AddData(bool data) {
			BitConverter.GetBytes(data).CopyTo(msg, offset);
			offset += sizeof(bool);
		}

		protected void AddData(long data) {
			BitConverter.GetBytes(data).CopyTo(msg, offset);
			offset += sizeof(long);
		}

		protected void AddData(uint data) {
			BitConverter.GetBytes(data).CopyTo(msg, offset);
			offset += sizeof(int);
		}

		protected void AddData(int data) {
			BitConverter.GetBytes(data).CopyTo(msg, offset);
			offset += sizeof(int);
		}

		protected void AddData(byte[] data) {
			Buffer.BlockCopy(data, 0, msg, offset, data.Length);
			offset += data.Length;
		}
		
		protected void AddData(byte[] data, int start, int count) {
			
			Buffer.BlockCopy(data, start, msg, offset, count);
			offset += count;
		}

		protected void AddData(int[] data) {
			Buffer.BlockCopy(data, 0, msg, offset, data.Length*sizeof(int));
			offset += data.Length * sizeof(int);
		}

		protected void AddData(MessageType mt) {
			BitConverter.GetBytes((int)mt).CopyTo(msg, offset);
			offset += sizeof(int);
		}
		
		protected void AddData(String data) {
			byte[] bytesData = ASCIIEncoding.ASCII.GetBytes(data);
			bytesData.CopyTo(msg, offset);
			offset += bytesData.Length;
		}

		public MessageProvider(byte[] buffer) {
			this.msg = buffer;
			this.offset = 0;
		}
		
		protected OutputMessage GenerateOutputMessage() {
			return new OutputMessage(msg, offset);
		}
	}
}
