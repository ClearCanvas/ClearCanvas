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
	/// Provides a Windows Forms user-interface for <see cref="PatientNoteEditorComponent"/>
	/// </summary>
	public partial class PatientNoteEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly PatientNoteEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PatientNoteEditorComponentControl(PatientNoteEditorComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_category.DataSource = _component.CategoryChoices;
			_category.Format +=
				delegate(object sender, ListControlConvertEventArgs e)
				{
					 e.Value = _component.FormatNoteCategory(e.ListItem);
				};
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "CategoryDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_comment.DataBindings.Add("Value", _component, "Comment", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_expiryDate.DataBindings.Add("Value", _component, "ExpiryDate", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.Enabled = _component.IsCommentEditable;
			_comment.ReadOnly = !_component.IsCommentEditable;
			_expiryDate.Enabled = _component.IsExpiryDateEditable;
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
