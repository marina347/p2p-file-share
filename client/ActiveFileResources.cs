using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client {
	class ActiveFileResources {
		FileDownloadManager fdm;
		FileUploadManager fum;
		FileState fs;

		internal FileDownloadManager Fdm {
			get {
				return fdm;
			}
		}

		internal FileUploadManager Fum {
			get {
				return fum;
			}
		}

		internal FileState Fs {
			get {
				return fs;
			}
		}

		public ActiveFileResources(FileState fsInput) {
			this.fs = fsInput;
			this.fdm = null;
			this.fum = null;
		}

		public ActiveFileResources(FileState fsInput, FileDownloadManager fdmInput) {
			this.fs = fsInput;
			this.fdm = fdmInput;
			this.fum = null;
		}

		public ActiveFileResources(FileState fsInput, FileUploadManager fumInput) {
			this.fs = fsInput;
			this.fdm = null;
			this.fum = fumInput;
		}
		public ActiveFileResources(FileState fsInput, FileDownloadManager fdmInput, FileUploadManager fumInput) {
			this.fs = fsInput;
			this.fdm = fdmInput;
			this.fum = fumInput;
		}
	}
}
