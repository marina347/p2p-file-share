using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
namespace client {
	public partial class FrmSearch : Form {
		public static FrmSearch frmSearch;
		public FrmSearch() {
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
            
		}
        
        public void HideColumns() {
          
            this.dgvFiles.Columns[1].Visible = false;
            this.dgvFiles.Columns[5].Visible = false;
            this.dgvFiles.Columns[6].Visible = false;

            dgvFiles.Columns[2].HeaderText = "Name";
            dgvFiles.Columns[4].HeaderText = "Size (bytes)";
            dgvFiles.Columns[3].HeaderText = "Extension";
            dgvFiles.Columns[0].HeaderText = "ID";
        }
		private void button1_Click(object sender, EventArgs e) {
            ShowFiles();
			
		}
        private void ShowFiles() {
            FileSearchAndRegistrationMessageProvider fsrmp = new FileSearchAndRegistrationMessageProvider(SocketResourcesManager.srm.ProvideBuffer());
            OutputMessage om = fsrmp.ProvideFileSearchByKeywordRequestMessage(txtSearch.Text);
            ClientCommunicationCenter.commCenter.SendMessageToServer(om);
        }
        
		public void ShowSearchResults(List<FileDescription> list) {
			
			dgvFiles.BeginInvoke((MethodInvoker)delegate ()
			{
				dgvFiles.DataSource = null;
				dgvFiles.DataSource = list;
                HideColumns();
			});
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
			FileDescription fd = dgvFiles.Rows[e.RowIndex].DataBoundItem as FileDescription;
			if (MasterFileManager.mfm.FileDescriptions.ContainsKey(fd.FileId)) {
				MessageBox.Show("File already owned!");
				return;
			}
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.InitialDirectory = "C:\\Users";
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
				string path = dialog.FileName;
				fd.Path = path;
				fd.Ftm = FileTransferMode.DownloadAndUpload;
				fd.Fts = FileTransferState.Normal;
				ClientHashReceiver.chr.InitReceiver(fd);
			}

		}

        private void FrmSearch_Load(object sender, EventArgs e) {
            ShowFiles();
        }
    }
}
