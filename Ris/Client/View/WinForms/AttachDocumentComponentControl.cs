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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class AttachDocumentComponentControl : ApplicationComponentUserControl
	{
		private readonly AttachDocumentComponent _component;

		public AttachDocumentComponentControl(AttachDocumentComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_file.DataBindings.Add("Value", _component, "FilePath", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _browseButton_Click(object sender, EventArgs e)
		{
			_component.BrowseFiles();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			using(new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Accept();
			}
		}
	}
}
