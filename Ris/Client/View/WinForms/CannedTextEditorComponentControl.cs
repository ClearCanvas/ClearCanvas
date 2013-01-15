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
    /// Provides a Windows Forms user-interface for <see cref="CannedTextEditorComponent"/>
    /// </summary>
    public partial class CannedTextEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly CannedTextEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CannedTextEditorComponentControl(CannedTextEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_groups.Enabled = !_component.IsReadOnly;
			_category.Enabled = !_component.IsReadOnly;
			_name.ReadOnly = _component.IsReadOnly;
			_text.ReadOnly = _component.IsReadOnly;

            _typeGroupBox.Enabled = !_component.IsReadOnly && _component.CanChangeType;

            _radioGroup.Checked = _component.IsEditingGroup;
            _radioPersonal.DataBindings.Add("Checked", _component, "IsEditingPersonal", true, DataSourceUpdateMode.OnPropertyChanged);

            _groups.DataSource = _component.StaffGroupChoices;
			_groups.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatStaffGroup(args.ListItem); };
			_groups.DataBindings.Add("Visible", _component, "IsEditingGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_groups.DataBindings.Add("Value", _component, "StaffGroup", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.DataBindings.Add("Text", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_text.DataBindings.Add("Value", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
