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
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class LookupHandlerCellEditorControl : UserControl
	{
		private LookupHandlerCellEditor _editor;

		public LookupHandlerCellEditorControl()
		{
			InitializeComponent();
			_suggestBox.ValueChanged += OnSuggestBoxOnValueChanged;
		}

		/// <summary>
		/// Sets the editor on which this control is operating - the control is re-used by the <see cref="TableView"/>.
		/// </summary>
		/// <param name="editor"></param>
		public void SetEditor(LookupHandlerCellEditor editor)
		{
			_editor = editor;

			// change sugg provider
			_suggestBox.SuggestionProvider = _editor.LookupHandler.SuggestionProvider;

			// update value
			_suggestBox.Value = _editor.Value;
		}

		private void _findButton_Click(object sender, EventArgs e)
		{
			try
			{
				object result;
				var resolved = _editor.LookupHandler.Resolve(_suggestBox.Text, true, out result);
				if (resolved)
				{
					_suggestBox.Value = result;
				}
			}
			catch (Exception ex)
			{
				// not much we can do here if Resolve throws an exception
				Platform.Log(LogLevel.Error, ex);
			}
		}

		private void OnSuggestBoxOnValueChanged(object sender, EventArgs e)
		{
			_editor.Value = _suggestBox.Value;
		}

		private void _suggestBox_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _editor.LookupHandler.FormatItem(e.ListItem);
		}

		private void LookupHandlerCellEditorControl_Load(object sender, EventArgs e)
		{
			_suggestBox.Value = _editor.Value;
		}

	}
}
