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
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class SimpleComponentContainerControl : CustomUserControl
	{
		private SimpleComponentContainer _component;

		public SimpleComponentContainerControl(SimpleComponentContainer component)
		{
			_component = component;

			InitializeComponent();

			this.AcceptButton = _okButton;
			this.CancelButton = _cancelButton;

			Control contentControl = _component.ComponentHost.ComponentView.GuiElement as Control;

			// Make the dialog conform to the size of the content
			Size sizeDiff = contentControl.Size - _contentPanel.Size;

			_contentPanel.Controls.Add(contentControl);

			this.Size += sizeDiff;
			contentControl.Dock = DockStyle.Fill;

			_okButton.Click += new EventHandler(OnOkButtonClicked);
			_cancelButton.Click += new EventHandler(OnCancelButtonClicked);
		}

		void OnOkButtonClicked(object sender, EventArgs e)
		{
			_component.OK();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
