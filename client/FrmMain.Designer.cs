namespace client {
	partial class FrmMain {
		
		private System.ComponentModel.IContainer components = null;
		
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		
		private void InitializeComponent() {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.msClient = new System.Windows.Forms.MenuStrip();
            this.toolFileTransfer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.msClient.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // msClient
            // 
            this.msClient.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolFileTransfer,
            this.toolSearch});
            this.msClient.Location = new System.Drawing.Point(0, 0);
            this.msClient.Name = "msClient";
            this.msClient.Size = new System.Drawing.Size(1045, 24);
            this.msClient.TabIndex = 2;
            this.msClient.Text = "menuStrip1";
            // 
            // toolFileTransfer
            // 
            this.toolFileTransfer.Name = "toolFileTransfer";
            this.toolFileTransfer.Size = new System.Drawing.Size(85, 20);
            this.toolFileTransfer.Text = "File transfers";
            this.toolFileTransfer.Click += new System.EventHandler(this.fileTransfersToolStripMenuItem_Click);
            // 
            // toolSearch
            // 
            this.toolSearch.Name = "toolSearch";
            this.toolSearch.Size = new System.Drawing.Size(54, 20);
            this.toolSearch.Text = "Search";
            this.toolSearch.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 426);
            this.Controls.Add(this.msClient);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.msClient;
            this.Name = "FrmMain";
            this.Text = "File sharing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmGlavna_FormClosing);
            this.msClient.ResumeLayout(false);
            this.msClient.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.MenuStrip msClient;
		private System.Windows.Forms.ToolStripMenuItem toolFileTransfer;
		private System.Windows.Forms.ToolStripMenuItem toolSearch;
	}
}

