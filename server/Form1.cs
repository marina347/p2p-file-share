using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using server.BazaF;
using System.Net;
using CommonResources;
using System.Data.SqlClient;
using System.Diagnostics;

namespace server {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		
			
			Database s = Database.Instance;
			ServerNetworkInputHandler snih = new ServerNetworkInputHandler();
			SocketResourcesManager.srm = new SocketResourcesManager(snih);
			ServerCommunicationCenter.commCenter = 
				new ServerCommunicationCenter(
					NetworkResolver.ProvideServerSocket(), 
					NetworkResolver.serverPort);
			ServerCommunicationCenter.commCenter.StartListening();
			ServerObjectGraph.sog = new ServerObjectGraph();
			UdpHpServer.uhps = new UdpHpServer();
			
		}

		private void button1_Click(object sender, EventArgs e) {
			ChunkHashModel.DeleteAll();
			BazaF.Owner.DeleteAll();
			File.DeleteAll();
		}

		private void timer1_Tick(object sender, EventArgs e) {

		}
	}
}
