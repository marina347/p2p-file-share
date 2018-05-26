using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public class FileDescription {
		int fileId;
		string path;
		string fileName;
		string fileExtension;
		uint fileSize;
		
		FileTransferMode ftm;
		FileTransferState fts;
		public int FileId {
			get {
				return fileId;
			}

			set {
				fileId = value;
			}
		}

		public string Path {
			get {
				return path;
			}

			set {
				path = value;
			}
		}
        public string FileName {
            get {
                return fileName;
            }

            set {
                fileName = value;
            }
        }

        public string FileExtension {
            get {
                return fileExtension;
            }

            set {
                fileExtension = value;
            }
        }

        public uint FileSize {
			get {
				return fileSize;
			}

			set {
				fileSize = value;
			}
		}
		public FileTransferMode Ftm {
			get {
				return ftm;
			}

			set {
				ftm = value;
			}
		}

		public FileTransferState Fts {
			get {
				return fts;
			}

			set {
				fts = value;
			}
		}
	}
}
