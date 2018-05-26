using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CommonResources;
namespace server {
	public class C2SMessageReader: MessageReader {
		public C2SMessageReader(byte[] msg) :base(msg) {

		}

		public int ClientEP() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public FileDescription ReadFileDescription() {
			FileDescription fd = new FileDescription();
			fd.FileId = BitConverter.ToInt32(msg, s(2));
			fd.FileSize = BitConverter.ToUInt32(msg, s(3));
			string nameAndExt = ASCIIEncoding.ASCII.GetString(msg, s(4), msg.Length - s(4));
			fd.FileName = nameAndExt.Split(';')[0];
			fd.FileExtension = nameAndExt.Split(';')[1];
			return fd;
		}

		public uint ReadRegisteringFileSize() {
			return BitConverter.ToUInt32(msg, s(2));
		}

		public string ReadRegisteringFileFullName() {
			return ASCIIEncoding.ASCII.GetString(msg, s(3), msg.Length - s(3));
		}
		
		public int ReadRegisteringFileId() {
			return BitConverter.ToInt32(msg, s(2));
		}

		public int ReadChunkId() {
			return BitConverter.ToInt32(msg, s(3));
		}
		
		public int ReadFileId() {
			return BitConverter.ToInt32(msg, s(2));
		}

		public string ReadFileNameKeyword() {
			return ASCIIEncoding.ASCII.GetString(msg, s(2), msg.Length - s(2));
		}

		public ChunkHash ReadChunkHash() {
			ChunkHash ch = new ChunkHash();
			ch.ChunkId = BitConverter.ToInt32(msg, s(3));
			Buffer.BlockCopy(msg, s(4), ch.Hash, 0, msg.Length - s(4));
			return ch;
		}

		public int ReadRequestedClientId() {
			return BitConverter.ToInt32(msg, s(2));
		}

		public IPEndPoint ClientPrivateEP() {
			byte[] ipAddress = new byte[4];
			Buffer.BlockCopy(msg, s(2), ipAddress,0 , 4);
			return new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt32(msg, s(2)+4));
		}

		public int PunchClientEP() {
			int punchEp = BitConverter.ToInt32(msg, s(2));
			return punchEp;
		}

	}
}
