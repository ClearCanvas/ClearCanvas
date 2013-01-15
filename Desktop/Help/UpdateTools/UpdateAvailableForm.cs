#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	internal partial class UpdateAvailableForm : Form
	{
		private string _downloadLink;

		private UpdateAvailableForm()
		{
			InitializeComponent();
		}

		public static void Show(string text, string downloadLink)
		{
			UpdateAvailableForm form = new UpdateAvailableForm();
			form._text.Text = text;
			form._downloadLink = downloadLink ?? "";

			form.ShowDialog();
		}

		private void OnOk(object sender, EventArgs e)
		{
			Close();
		}

		private void OnDownloadNow(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (String.IsNullOrEmpty(_downloadLink))
				return;

			try
			{
				Process.Start(_downloadLink);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, String.Format("Failed to navigate to download link '{0}'.", _downloadLink));
			}
		}
	}
}
