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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class CustomizeViewerActionModelsComponentControl : CustomUserControl
	{
		private readonly CustomizeViewerActionModelsComponent _component;

		public CustomizeViewerActionModelsComponentControl(CustomizeViewerActionModelsComponent component)
		{
			InitializeComponent();

			AcceptButton = _btnOk;
			CancelButton = _btnCancel;

			_component = component;

			Control control = (Control) _component.TabComponentHost.ComponentView.GuiElement;
			control.Dock = DockStyle.Fill;
			_pnlMain.Controls.Add(control);
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
		    var currentCur = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                _component.Accept();
            }
            finally
            {
                Cursor.Current = currentCur;
            }

		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}