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

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="AttachedDocumentPreviewComponent"/>
	/// </summary>
	public partial class AttachedDocumentPreviewComponentControl : ApplicationComponentUserControl
	{
		private readonly AttachedDocumentPreviewComponent _component;

		public AttachedDocumentPreviewComponentControl(AttachedDocumentPreviewComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_attachments.ShowToolbar = true;

			_attachments.Table = _component.AttachmentTable;
			_attachments.MenuModel = _component.AttachmentActionModel;
			_attachments.ToolbarModel = _component.AttachmentActionModel;

			_attachments.DataBindings.Add("Selection", _component, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void AttachedDocumentPreviewComponentControl_Load(object sender, System.EventArgs e)
		{
			_component.OnControlLoad();
		}

		private void _attachments_ItemDoubleClicked(object sender, System.EventArgs e)
		{
			_component.DoubleClickedSelectedAttachment();
		}

	}
}
