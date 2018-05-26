using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;
using System.Diagnostics;

namespace client {
	class TransferStarter {
		public void AllLocalFilesStartTransfer() {
			foreach(FileDescription fd in MasterFileManager.mfm.FileDescriptions.Values) {
				ExistingFileStartTransfer(fd);
			}
		}
		public void ExistingFileStartTransfer(FileDescription fd) {
			FileState fs = MasterFileManager.mfm.ProvideFileState(fd.FileId);
			ActiveFileResources afr = null;
			if (fd.Fts == FileTransferState.Finished && fd.Ftm == FileTransferMode.OnlyUpload) {
				Debug.WriteLine("ExistingFileStartTransfer ONLY UPLOAD:" + fd.FileId);
				afr = new ActiveFileResources(fs, new FileUploadManager(fs, false));
			}
			else if(fd.Fts == FileTransferState.Normal) {
				if(fd.Ftm == FileTransferMode.DownloadAndUpload) {
					Debug.WriteLine("ExistingFileStartTransfer DOWNLOAD AND UPLOAD:" + fd.FileId);
					afr = new ActiveFileResources(fs, new FileDownloadManager(fd.FileId, fs, false), new FileUploadManager(fs, false));
				}
				else if(fd.Ftm == FileTransferMode.OnlyDownload) {
					Debug.WriteLine("ExistingFileStartTransfer ONLY DOWNLOAD:" + fd.FileId);
					afr = new ActiveFileResources(fs, new FileDownloadManager(fd.FileId, fs, false));
				}
			}
			
			if (afr != null) {
				FileTransferCenter.ftc.AddFile(afr);
			}
			FrmFileTransfers.frmFileTransfers.ShowNewFile(fs);
		}
		public void NewFileStartTransfer(FileDescription fd, ChunkHash[] hashes) {
			Debug.WriteLine("NewFileStartTransfer:" + fd.FileId);
			fd.Ftm = FileTransferMode.DownloadAndUpload;
			fd.Fts = FileTransferState.Normal;
			
			NewFileTransferManager.nftm.NewDownloadFile(fd, hashes);
			
			MasterFileManager.mfm.RecordNewFile(fd);
			
			FileState fs = MasterFileManager.mfm.ProvideFileState(fd.FileId);
			
			ActiveFileResources afr =
				new ActiveFileResources(
					fs, 
					new FileDownloadManager(fd.FileId, fs, false), 
					new FileUploadManager(fs, false)
				);
			FileTransferCenter.ftc.AddFile(afr);
			
			FrmFileTransfers.frmFileTransfers.ShowNewFile(fs);
		}
		public void LocalFileInitTransfer(FileDescription fd) {
			Debug.WriteLine("LocalFileInitTransfer:" + fd.FileId);
			fd.Ftm = FileTransferMode.OnlyUpload;
            fd.Fts = FileTransferState.Finished;
            NewFileTransferManager.nftm.NewUploadFile(fd, fd.Path);
            MasterFileManager.mfm.RecordNewFile(fd);
            
            FileState fs = MasterFileManager.mfm.ProvideFileState(fd.FileId);    
            ActiveFileResources afr =
				new ActiveFileResources(
					fs,
					new FileUploadManager(fs, false)
				);
			FileTransferCenter.ftc.AddFile(afr);

			FrmFileTransfers.frmFileTransfers.ShowNewFile(fs);
		}
    }
}
