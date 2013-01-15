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

using System.ComponentModel;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Tools.Reporting.KeyImages;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	public partial class KeyImageInformationEditorComponentControl : ApplicationComponentUserControl
	{
		private KeyImageInformationEditorComponent _component;

		public KeyImageInformationEditorComponentControl(KeyImageInformationEditorComponent component) 
			: base(component)
		{
			_component = component;
			InitializeComponent();

			foreach (KeyObjectSelectionDocumentTitle title in KeyImageInformationEditorComponent.StandardDocumentTitles)
			{
				cboTitle.Items.Add(title);
			}

			base.CancelButton = _cancelButton;
			base.AcceptButton = _okButton;

			cboTitle.DataBindings.Add("SelectedItem", component, "DocumentTitle");
			txtDesc.DataBindings.Add("Text", component, "Description");
			txtSeriesDesc.DataBindings.Add("Text", component, "SeriesDescription");
		}

		private void OnOk(object sender, System.EventArgs e)
		{
			_component.Accept();
		}

		private void OnCancel(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}