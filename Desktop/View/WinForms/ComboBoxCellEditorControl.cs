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
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class ComboBoxCellEditorControl : UserControl
	{
		private ComboBoxCellEditor _editor;

		public ComboBoxCellEditorControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Sets the editor on which this control is operating - the control is re-used by the <see cref="TableView"/>.
		/// </summary>
		/// <param name="editor"></param>
		public void SetEditor(ComboBoxCellEditor editor)
		{
			_editor = editor;

			// update value
			_comboBox.DataSource = _editor.GetChoices();
			_comboBox.SelectedItem = _editor.Value;
		}

		private void _comboBox_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			_editor.Value = _comboBox.SelectedItem;
		}

		private void ComboBoxCellEditorControl_Load(object sender, System.EventArgs e)
		{
			_comboBox.DataSource = _editor.GetChoices();
			_comboBox.SelectedItem = _editor.Value;
		}

		private void _comboBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _editor.FormatItem(e.ListItem);
		}
	}
}
