using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace client {
	class FileTransferCenter {
		public static FileTransferCenter ftc;
		Dictionary<int, ActiveFileResources> activeFileResources;
		
		public NetworkFileStateManager nfsm;
		public NetworkDataManager ndm;
		public NetworkConnectivityManager ncm;
		public TransferStarter ts;

		internal Dictionary<int, ActiveFileResources> ActiveFileResources {
			get {
				return activeFileResources;
			}

			set {
				activeFileResources = value;
			}
		}

		public FileTransferCenter() {
			ActiveFileResources = new Dictionary<int, ActiveFileResources>();
			nfsm = new NetworkFileStateManager();
			ndm = new NetworkDataManager();
			ncm = new NetworkConnectivityManager();
			ts = new TransferStarter();
		}

		public void AddFile(ActiveFileResources afr) {
			ActiveFileResources.Add(afr.Fs.FileDescription.FileId, afr);
			if(afr.Fdm!=null)
				afr.Fdm.Start();
		}
		public FileDownloadManager GetFdm(int fileId) {
			return ActiveFileResources[fileId].Fdm;
		}
		
		public FileUploadManager GetFum(int fileId) {
			return ActiveFileResources[fileId].Fum;
		}

		public FileState GetFs(int fileId) {
			return activeFileResources.ContainsKey(fileId)?activeFileResources[fileId].Fs:null;
		}
	}
}
