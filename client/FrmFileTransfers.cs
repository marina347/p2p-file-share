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
using System.Diagnostics;

namespace client {
	public partial class FrmFileTransfers : Form {
		public static FrmFileTransfers frmFileTransfers;
		List<FileShow> fileShowList;
		DateTime regTimeout;
		public FrmFileTransfers() {
			InitializeComponent();
			fileShowList = new List<FileShow>();
			BindingSource bs = new BindingSource();
			bs.DataSource = new BindingList<FileShow>(fileShowList);
			dgvFiles.DataSource = new BindingList<FileShow>(fileShowList);
            HideColumns();

		}

		public void ShowNewFile(FileState fs) {
			dgvFiles.BeginInvoke((MethodInvoker)delegate ()
			{
				lock (dgvFiles) {
					fileShowList.Add(new FileShow(fs));
					dgvFiles.DataSource = null;
					dgvFiles.DataSource = new BindingList<FileShow>(fileShowList);
					HideColumns();
				}
			});
		}

		private void btnBrowseIzvorisna_Click(object sender, EventArgs e) { 
			if((DateTime.Now.Ticks - regTimeout.Ticks) / TimeSpan.TicksPerMillisecond > 5000) {
				FileRegistrationManager.frm.CloseRegistrationProcess();
			}
			if (FileRegistrationManager.frm.RegistrationProcessStarted == true ) {
				MessageBox.Show("File registration has already started!");
				return;
			}
            FileRegistrationManager.frm.RegistrationProcessStarted = false;
            openFileDialog1.Multiselect = false;
			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				FileInfo fi = new FileInfo(openFileDialog1.FileName);
				FileDescription fd = new FileDescription();
				fd.FileName = Utils.GetFileNameFromPath(fi.Name);
				fd.Path = fi.DirectoryName;
				fd.FileSize = (uint)fi.Length;
				fd.FileExtension = fi.Extension.Trim('.');
				regTimeout = DateTime.Now;
				FileRegistrationManager.frm.StartRegistrationProcess(fd);
		
			}
			
		}

        public void HideColumns() {
            dgvFiles.Columns[0].HeaderText = "Name";
            dgvFiles.Columns[2].HeaderText = "Size ";
            dgvFiles.Columns[1].HeaderText = "Extension";
            dgvFiles.Columns[3].HeaderText = "Percentage";
        }

		private void timer1_Tick(object sender, EventArgs e) {

			dgvFiles.BeginInvoke((MethodInvoker)delegate ()
			{
				lock (dgvFiles) {
					int rowIndex = 0;
					if (dgvFiles.Rows.Count > 0)
						rowIndex = dgvFiles.FirstDisplayedScrollingRowIndex;

					dgvFiles.DataSource = null;
					dgvFiles.DataSource = new BindingList<FileShow>(fileShowList);
					HideColumns();

					if (rowIndex != 0 && rowIndex != -1  && rowIndex < dgvFiles.Rows.Count)
						dgvFiles.FirstDisplayedScrollingRowIndex = rowIndex;
					
				}
			});
				
		}
	}
}
