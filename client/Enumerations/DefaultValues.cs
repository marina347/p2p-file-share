using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace client {
	class DefaultValues {
		
		public static FileTransferMode defaultFileTransferMode {
			get {
				return FileTransferMode.DownloadAndUpload;
			}
		}

		public static FileTransferState defaultFileTransferState {
			get {
				return FileTransferState.Normal;
			}
		}
	}
}
