using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByteSizeLib;

namespace client {
	class FileShow {
		FileState fs;

		public FileShow(FileState fs) {
			this.fs = fs;
		}

		public string FileName {
			get {
				return fs.FileDescription.FileName;
			}
			set { }
		}

		public string FileExtension {
			get {
				return fs.FileDescription.FileExtension;
			}
			set { }
		}

		public string FileSize {
			get {
				return ByteSize.FromBytes(fs.FileDescription.FileSize).ToString();
			}
			set { }
		}

		public string PercentageDownload {
			get {
				return String.Format("{0:P2}.", (float)fs.FinishedChunksNumber / fs.NumberOfChunks);
			}
			set { }
		}

	}
}
