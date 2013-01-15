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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="CannedTextSummaryComponent"/>
    /// </summary>
    public partial class CannedTextSummaryComponentControl : ApplicationComponentUserControl
    {
        private readonly CannedTextSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CannedTextSummaryComponentControl(CannedTextSummaryComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_cannedTexts.Table = _component.SummaryTable;
			_cannedTexts.MenuModel = _component.SummaryTableActionModel;
			_cannedTexts.ToolbarModel = _component.SummaryTableActionModel;
			_cannedTexts.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.CopyCannedTextRequested += _component_CopyCannedTextRequested;
		}

		private void _component_CopyCannedTextRequested(object sender, EventArgs e)
		{
			string fullCannedText = _component.GetFullCannedText();
			if (!string.IsNullOrEmpty(fullCannedText))
				Clipboard.SetDataObject(fullCannedText, true);
		}

		private void _cannedTexts_ItemDrag(object sender, ItemDragEventArgs e)
		{
			string fullCannedText = _component.GetFullCannedText();
			if (!string.IsNullOrEmpty(fullCannedText))
				_cannedTexts.DoDragDrop(fullCannedText, DragDropEffects.All);
		}

		private void _cannedTexts_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.EditSelectedItems();
		}
	}
}
