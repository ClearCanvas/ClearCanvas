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
using ClearCanvas.ImageViewer.Clipboard.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Reporting.KeyImages;

namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	public partial class KeyImageClipboardComponentControl : UserControl
	{
		private readonly KeyImageClipboardComponent _component;
		private readonly ClipboardComponentControl _clipboardControl;

		public KeyImageClipboardComponentControl(KeyImageClipboardComponent component)
		{
			InitializeComponent();

			_component = component;
			_component.CurrentContextChanged += _component_CurrentContextChanged;

			_clipboardControl = new ClipboardComponentControl(_component) {Dock = DockStyle.Fill};
			_pnlMain.Controls.Add(_clipboardControl);

			_cboCurrentContext.DataSource = _component.AvailableContexts;

			// SelectedItem needs to be explicitly initialized, because binding SelectedValue won't set correctly
			// SelectedValue must be the binding, because SelectedItem doesn't register property changed until control loses focus
			_cboCurrentContext.SelectedItem = _component.CurrentContext;
			_cboCurrentContext.DataBindings.Add("SelectedValue", _component, "CurrentContext", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _component_CurrentContextChanged(object sender, EventArgs e)
		{
			_cboCurrentContext.SelectedItem = _component.CurrentContext;
		}
	}
}