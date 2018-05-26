using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using CommonResources;

namespace client {
	class C2CMessageReader: MessageReader {
		public C2CMessageReader(byte[]msg):base(msg) {

		}

		public int ClientEP() {
			return BitConverter.ToInt32(msg, s(1));
		}

		public EndPoint ProvidedEndPoint() {
			byte[] ipAddress = new byte[4];
			Buffer.BlockCopy(msg, s(1), ipAddress, 0,s(1));
			return new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt32(msg, s(2)));
		}

		public int ReadFileId() {
			return BitConverter.ToInt32(msg, s(2));
		}

		public int ReadChunkId() {
			return BitConverter.ToInt32(msg, s(3));
		}

		public int ReadChunkPartId() {
			return BitConverter.ToInt32(msg, s(4));
		}

		public byte[] ReadChunkPartData() {
			byte[] chunkPartData = new byte[msg.Length - s(5)];
			Buffer.BlockCopy(msg, s(5), chunkPartData, 0, msg.Length - s(5));
			return chunkPartData;
		}

		public BitArray ReadChunksState() {
			byte[] fileState = new byte[msg.Length - s(3)];
			Buffer.BlockCopy(msg, s(3), fileState, 0, msg.Length - s(3));
			return new BitArray(fileState);
		}

		public int[] ReadRequestedChunks() {
			int[] requestedChunks = new int[(msg.Length - s(3))/4];
			Buffer.BlockCopy(msg, s(3), requestedChunks, 0, msg.Length - s(3));
			return requestedChunks;
		}
		
		public EndPoint ReadClientPrivateEP() {
			return new IPEndPoint(IPAddress.Any, 3);
		}
		
		public int PunchClientEP() {
			return 0;
		}
	}
}
