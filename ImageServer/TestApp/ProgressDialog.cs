using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.TestApp
{
	public partial class ProgressDialog : Form
	{
		public delegate void UpdateProgressDelegate(string message, int percentage);
		
		public ProgressDialog()
		{
			InitializeComponent();
		}

		public void Update(string message, int percentage)
		{
			if (!this.Disposing && !this.IsDisposed)
				this.BeginInvoke(new UpdateProgressDelegate(UpdateProgress), new object[] { message, percentage });

		}

		private void UpdateProgress(string message, int percentage)
		{
			_message.Text = message;
			_progressBar.Value = percentage;
			Invalidate();
		}
	}
}
