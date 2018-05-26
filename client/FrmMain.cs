using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;

namespace client {
	public partial class FrmMain : Form {

		public FrmMain() {
			InitializeComponent();
			InitForm();
			StartApp(NetworkResolver.clientPort1);
			fileTransfersToolStripMenuItem_Click(null, null);
		}
		
		public FrmMain(string tek) {
			InitializeComponent();
			InitForm();
			this.Text = tek;
			if(tek == "CLIENT") {
				StartApp(NetworkResolver.clientPort1);
			}
			else {
				StartApp(NetworkResolver.clientPort2);
			}
			fileTransfersToolStripMenuItem_Click(null, null);
		}


		public void StartApp(int port) {
			string fileName = "master.dat";
			ApplicationLiveData.ald = new ApplicationLiveData(port);
			ClientCommunicationCenter.commCenter = new ClientCommunicationCenter(NetworkResolver.ProvideClientSocket(port), port);

			ApplicationLiveData.ald.LocalEndPoint = ClientCommunicationCenter.commCenter.Socket.LocalEndPoint;
			ApplicationLiveData.ald.Port = port;
			
			ClientNetworkInputHandler.nic = new ClientNetworkInputHandler();
			SocketResourcesManager.srm = new SocketResourcesManager(ClientNetworkInputHandler.nic);
			ClientCommunicationCenter.commCenter.StartListening();

			ApplicationLiveData.ald.ServerEndPoint = NetworkResolver.ResolveServerEndPoint();
			MasterFileManager.mfm = new MasterFileManager(fileName, this);
			MasterFileManager.mfm.InitMasterFile();
			FileRegistrationManager.frm = new FileRegistrationManager();
			ClientHashReceiver.chr = new ClientHashReceiver();
			NewFileTransferManager.nftm = new NewFileTransferManager();
			UdpHpClient.uhc = new UdpHpClient();
			FileTransferCenter.ftc = new FileTransferCenter();
		}

		private void InitForm() {
			Utils.PrepareDescriptorFolder();
			Utils.PrepareFilesFolder();
			FrmFileTransfers.frmFileTransfers = new FrmFileTransfers();
			FrmSearch.frmSearch = new FrmSearch();
		}

		public void ReadyToBegin() {//t+
			
		}

        private void btnBrowseIzvorisna_Click(object sender, EventArgs e) {//t+...
			if (FileRegistrationManager.frm.RegistrationProcessStarted == true) {
				MessageBox.Show("Proces registriranja datoteke je već pokrenut!");
				return;
			}
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                FileInfo fi = new FileInfo(openFileDialog1.FileName);
                FileDescription fd = new FileDescription();
				fd.FileName = Utils.GetFileNameFromPath(fi.Name);
                fd.Path = fi.DirectoryName;
                fd.FileSize = (uint)fi.Length;
                fd.FileExtension = fi.Extension;
				FileRegistrationManager.frm.StartRegistrationProcess(fd);
            }
        }
		
		private void FrmGlavna_FormClosing(object sender, FormClosingEventArgs e) {
			if(FileTransferCenter.ftc != null && FileTransferCenter.ftc.ActiveFileResources!=null)
				foreach(ActiveFileResources afr in FileTransferCenter.ftc.ActiveFileResources.Values) {
					afr.Fs.FileManager.CloseFileStreams();
				}
			if(MasterFileManager.mfm!=null)
				MasterFileManager.mfm.CloseFileStreams();
		}

		private void ChangeForm(Form frm) {
			//if we're already on wanted form, we return
			if (this.ActiveMdiChild != null && this.ActiveMdiChild == frm) {
				return;
			}
			else if(this.ActiveMdiChild != null)//else if there is current form, we hide it
				this.ActiveMdiChild.Hide();
			//and set this form as current
			frm.MdiParent = this;
			frm.Dock = DockStyle.Fill;
			frm.Show();
		}

		private void fileTransfersToolStripMenuItem_Click(object sender, EventArgs e) {
			ChangeForm(FrmFileTransfers.frmFileTransfers);
			FrmFileTransfers.frmFileTransfers.timer1.Stop();
			FrmFileTransfers.frmFileTransfers.timer1.Start();
		}

		private void searchToolStripMenuItem_Click(object sender, EventArgs e) {
			ChangeForm(FrmSearch.frmSearch);
		}
	}
}
