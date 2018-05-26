using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;
using System.Diagnostics;

namespace client {
	class MasterFileManager {
		private string path;
		private FrmMain frmGlavna;
		private FileStream masterFileStream;
		public MasterFileManager(String path, FrmMain frmMainInput) {
			this.frmGlavna = frmMainInput;
			deletedFiles = new Dictionary<int, FileDescription>();
			fileDescriptions = new Dictionary<int, FileDescription>();
			this.path = path;
		}

		private void WriteFileDescription(FileDescription fd) {
			Debug.WriteLine("write fid:" + fd.FileId + "| pos:" + masterFileStream.Position);
			masterFileStream.Write(BitConverter.GetBytes(fd.FileId), 0, 4);
			masterFileStream.Write(BitConverter.GetBytes(fd.FileSize), 0, 4);
			masterFileStream.Write(BitConverter.GetBytes((int)fd.Fts), 0, 4);
			masterFileStream.Write(BitConverter.GetBytes((int)fd.Ftm), 0, 4);
			string pathAndOther = fd.Path + "\\" + fd.FileName;
			
			if (fd.FileExtension != null && fd.FileExtension != "") {
				pathAndOther += "." + fd.FileExtension;
			}
			masterFileStream.Write(ASCIIEncoding.ASCII.GetBytes(pathAndOther), 0, pathAndOther.Length);
			byte[] array = new byte[200 - pathAndOther.Length];
			masterFileStream.Write(array, 0, array.Length);
			masterFileStream.Flush();
		}

		public void UpdateFileDescription(FileDescription fd) {
			int i = 0;
			int[] fds = fileDescriptions.Keys.ToArray();
			for (; i < fds.Length; i++) {
				if (fds[i] == fd.FileId)
					break;
			}
			lock (masterFileStream) {
				long pos = 4 + Sizes.FileDescriptionDriveSize * i;
				masterFileStream.Seek(pos, SeekOrigin.Begin);
				Debug.WriteLine("update fid:" + fd.FileId + "| pos:" + pos);
				WriteFileDescription(fd);
			}
		}

		public void RecordNewFile(FileDescription fd) {
			lock (masterFileStream) {
				SetStreamEnd();
				WriteFileDescription(fd);
			}
			
			FileDescriptions.Add(fd.FileId, fd);
		}

		private FileDescription ReadFileDescription(int location) {
			lock (masterFileStream) {
				masterFileStream.Seek((long)location, SeekOrigin.Begin);
				byte[] array;
				array = new byte[Sizes.FileDescriptionDriveSize];
				masterFileStream.Read(array, 0, Sizes.FileDescriptionDriveSize);
				FileDescription fd = new FileDescription();
				fd.FileId = BitConverter.ToInt32(array, 0);
				fd.FileSize = BitConverter.ToUInt32(array, 4);
				fd.Fts = (FileTransferState)BitConverter.ToInt32(array, 8);
				fd.Ftm = (FileTransferMode)BitConverter.ToInt32(array, 12);
				string pathAndOther = ASCIIEncoding.ASCII.GetString(array, 16, 200).TrimEnd('\0');
				fd.FileName = Utils.GetFileNameFromPath(pathAndOther);
				fd.FileExtension = Utils.GetFileExtensionFromPath(pathAndOther);
				fd.Path = Utils.GetOnlyPathFromFilePath(pathAndOther);
				if (fd.Fts != FileTransferState.Deleted && !File.Exists(pathAndOther)) {
					fd.Fts = FileTransferState.Deleted;
					masterFileStream.Seek((long)location + sizeof(int) * 2, SeekOrigin.Begin);
					masterFileStream.Write(BitConverter.GetBytes((int)fd.Fts), 0, 4);
                    masterFileStream.Flush();
                    deletedFiles.Add(fd.FileId, fd);
				}
                
				return fd;
			}
		}
       
		private void LoadFileDescriptions() {
			if (masterFileStream != null) {
				int offset = 4;

				int fileSize = (int)masterFileStream.Length;
				while (offset + Sizes.FileDescriptionDriveSize <= fileSize) {
					FileDescription fd = ReadFileDescription(offset);
					if (fd != null && !FileDescriptions.ContainsKey(fd.FileId) && fd.Fts != FileTransferState.Deleted) {
						FileDescriptions.Add(fd.FileId, fd);
						Debug.WriteLine("offset:" + offset);
						Debug.WriteLine("file:" + fd.FileId);
					}
					offset += Sizes.FileDescriptionDriveSize;
				}
			}
		}

		private void SetStreamEnd() {
			masterFileStream.Seek(masterFileStream.Length, SeekOrigin.Begin);
		}

		public bool InitMasterFile() {
			int id = 0;
			try {
				masterFileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
				byte[] idByte = new byte[4];
				masterFileStream.Read(idByte, 0, 4);

				id = BitConverter.ToInt32(idByte, 0);
				ApplicationLiveData.ald.ApplicationId = id;
				LoadFileDescriptions();
				ApplicationLiveData.ald.Ready = true;
				frmGlavna.ReadyToBegin();
				return true;
			}
			catch {
				return false;
			}
		}

		public void ApplicationIdAquired(int id) {
			masterFileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
			byte[] idByte = BitConverter.GetBytes(id);
			masterFileStream.Write(idByte, 0, 4);
			masterFileStream.Flush();
			ApplicationLiveData.ald.ApplicationId = id;
			
			ApplicationLiveData.ald.Ready = true;
			frmGlavna.ReadyToBegin();
		}

		public static MasterFileManager mfm;

		private Dictionary<int, FileDescription> fileDescriptions;
		private Dictionary<int, FileDescription> deletedFiles;

		public FileState ProvideFileState(int fileId) {
			FileDescription fd = FileDescriptions[fileId];
			
			string fullName = fd.FileName;
			if (fd.FileExtension != null && fd.FileExtension != "") {
				fullName += "." + fd.FileExtension;
			}
			FileStream mainFileStream = new FileStream(fd.Path + "\\" + fullName, FileMode.Open, FileAccess.ReadWrite);
			if (fd.Fts != FileTransferState.Deleted && fd.Fts != FileTransferState.Finished) {
				FileStream descriptorFile = new FileStream(".\\descriptors\\descriptor_" + fullName + ".desc", FileMode.Open, FileAccess.ReadWrite);
				FileState fs = new FileState();
				fs.FileDescription = fd;

				byte[] chunksState = new byte[Sizes.GetChunksNumber(fd.FileSize)];
				descriptorFile.Seek(0, SeekOrigin.Begin);
				int numberOfChunks = Sizes.GetChunksNumber(fd.FileSize);
				descriptorFile.Read(chunksState, 0, numberOfChunks);

				fs.ChunksState = Utils.GetBitForByte(chunksState);
				int downloaded = 0;
				foreach (bool b in fs.ChunksState) {
					if (b == true) downloaded++;
				}
				fs.FinishedChunksNumber = downloaded;

				fs.FileManager = new FileIOManager(mainFileStream, descriptorFile);
				return fs;
			}
			else {
				FileState fs = new FileState();
				fs.FileDescription = fd;
				fs.FinishedChunksNumber = fs.NumberOfChunks;
				fs.FileManager = new FileIOManager(mainFileStream);
				return fs;
			}

		}

		public void CloseFileStreams() {
			this.masterFileStream.Close();
		}

		public Dictionary<int, FileDescription> FileDescriptions {
			get {
				return fileDescriptions;
			}
		}

		public Dictionary<int, FileDescription> DeletedFiles {
			get {
				return deletedFiles;
			}
		}
	}
}
