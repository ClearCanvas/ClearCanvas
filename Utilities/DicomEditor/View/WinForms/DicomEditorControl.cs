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
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
	public partial class DicomEditorControl : UserControl
	{
		private readonly DicomEditorComponent _dicomEditorComponent;

		public DicomEditorControl(DicomEditorComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			InitializeComponent();

			_dicomEditorComponent = component;
			_dicomEditorComponent.CommitChangesRequested += OnDicomEditorComponentCommitChangesRequested;

			_dicomTagTable.Table = _dicomEditorComponent.DicomTagData;
			_dicomTagTable.ToolbarModel = _dicomEditorComponent.ToolbarModel;
			_dicomTagTable.MenuModel = _dicomEditorComponent.ContextMenuModel;
			_dicomTagTable.SelectionChanged += OnDicomTagTableSelectionChanged;
			_dicomTagTable.MultiLine = true;

			_dicomEditorTitleBar.DataBindings.Add("Text", _dicomEditorComponent, "DicomFileTitle", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnDicomEditorComponentCommitChangesRequested(object sender, EventArgs e)
		{
			_dicomTagTable.EndEdit();
		}

		private void OnDicomTagTableSelectionChanged(object sender, EventArgs e)
		{
			_dicomEditorComponent.SetSelection(_dicomTagTable.Selection);
		}
	}
}