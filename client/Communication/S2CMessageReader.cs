using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class S2CMessageReader: MessageReader {

		public int ReadAcceptedFileId() {
			return BitConverter.ToInt32(msg, s(1)+sizeof(bool));
		}

		public int ReadDesignatedAppId() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public bool ReadFileAcceptance() {
			return BitConverter.ToBoolean(msg, s(1));
		}

		public int ReadRequestedHashId() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public int[] ReadClientsList() {
			int[] clients = new int[(msg.Length/4)-2];
			Buffer.BlockCopy(msg, s(2), clients, 0, msg.Length -s(2));
			return clients;
		}

		public int ReadClientListFileId() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public int ReadClientId() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public int ReadClientAvaliability() {
			return BitConverter.ToInt32(msg, s(2));
		}

		public int ReadResendChunkId() {
			return BitConverter.ToInt32(msg, s(1));
		}
        public int ReadResendChunkIdFileId() {
            return BitConverter.ToInt32(msg, s(2));
        }
        public ChunkHash ReadChunkHash() {
			ChunkHash ch = new ChunkHash();
			ch.ChunkId = BitConverter.ToInt32(msg, s(1));
			Buffer.BlockCopy(msg, s(2), ch.Hash, 0, msg.Length - s(2));
			return ch;
		}

		public List<FileDescription> ReadFileDescriptions() {
			List<FileDescription> list = new List<FileDescription>();
			int pos = s(1);
			while (pos < msg.Length) {
				FileDescription fd = new FileDescription();
				fd.FileId = BitConverter.ToInt32(msg, pos);
				pos += s(1);
				fd.FileSize = BitConverter.ToUInt32(msg, pos);
				pos += s(1);
				int stringLength = BitConverter.ToInt32(msg, pos);
				pos += s(1);
				string fullName = ASCIIEncoding.ASCII.GetString(msg, pos, stringLength);
				pos += stringLength;
				string[] array = fullName.Split('\\');
				if(array.Length == 1) {
					fd.FileName = fullName;
				}
				else {
					fd.FileExtension = array[1];
					fd.FileName = array[0];
				}
				list.Add(fd);
			}
			return list;
		}

		public S2CMessageReader(byte[]msg):base(msg){

		}
	}
}
