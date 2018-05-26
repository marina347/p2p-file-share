using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CommonResources {

	public class Utils {
		public static string GetLocalIPAddress() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					return ip.ToString();
				}
			}
			return null;
		}
		public static void PrepareDescriptorFolder() {
			bool exists = System.IO.Directory.Exists("descriptors");

			if (!exists)
				System.IO.Directory.CreateDirectory("descriptors");
		}

		public static void PrepareFilesFolder() {
			bool exists = System.IO.Directory.Exists("dfiles");

			if (!exists)
				System.IO.Directory.CreateDirectory("dfiles");
		}

		public static BitArray GetBitForByte(byte[] arr) {
			BitArray bitArray = new BitArray(arr.Length);
			for (int i = 0; i < arr.Length; i++) {
				if (arr[i] == 0) {
					bitArray.Set(i, false);
				}
				else
					bitArray.Set(i, true);
			}
			return bitArray;
		}
	
		public static string GetFileNameFromPath(string path) {
			path.TrimEnd('\0');
			string fullFileName = path.Split('\\').Last();
			if (!fullFileName.Contains(".")) {
				return fullFileName;
			}
			else {
				string ext = fullFileName.Split('.').Last();
				string name = fullFileName.Substring(0, fullFileName.Length - ext.Length - 1);
				return name;
			}
		}

		public static string GetFileExtensionFromPath(string path) {
			path.TrimEnd('\0');
			string fullFileName = path.Split('\\').Last();
			if (!fullFileName.Contains(".")) {
				return "";
			}
			else {
				string ext = fullFileName.Split('.').Last();
				return ext;
			}
		}

		public static string GetOnlyPathFromFilePath(string path) {
			path.TrimEnd('\0');
			string name = GetFileNameFromPath(path);
			string ext = GetFileExtensionFromPath(path);
			string trimBy = name;
			if (ext != "")
				trimBy += "." + ext;
			return path.TrimEnd(trimBy.ToCharArray());
		}
	}
	
}
