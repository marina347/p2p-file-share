using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace CommonResources {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
			
			Debug.WriteLine("je li zadnji:" + Sizes.IsLastChunk(2500, 0));
			Debug.WriteLine("koliko ima bajtova: " + Sizes.LastChunkSize(2500));
			Debug.WriteLine("koliko podchunkova ima: " + Sizes.LastChunkPartsCount(2500));
			Debug.WriteLine("zadnji ima bajtova:" + Sizes.LastChunkPartSize(2500));
			Debug.WriteLine("koliko podchunkova ima: " + Sizes.LastChunkPartsCount(2048));
			Debug.WriteLine("zadnji ima bajtova:" + Sizes.LastChunkPartSize(2048));

		}
	}
}
