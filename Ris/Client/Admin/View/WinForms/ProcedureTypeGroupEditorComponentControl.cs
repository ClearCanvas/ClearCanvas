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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProcedureTypeGroupEditorComponent"/>
	/// </summary>
	public partial class ProcedureTypeGroupEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ProcedureTypeGroupEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ProcedureTypeGroupEditorComponentControl(ProcedureTypeGroupEditorComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.DataBindings.Add("Enabled", _component, "CategoryEnabled");
			_includeDeactivatedItems.DataBindings.Add("Checked", _component, "IncludeDeactivatedProcedureTypes", true,
													  DataSourceUpdateMode.OnPropertyChanged);

			_procedureTypesSelector.AvailableItemsTable = _component.AvailableProcedureTypes;
			_procedureTypesSelector.SelectedItemsTable = _component.SelectedProcedureTypes;

			_procedureTypesSelector.ItemAdded += OnItemsAddedOrRemoved;
			_procedureTypesSelector.ItemRemoved += OnItemsAddedOrRemoved;

			_acceptButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnItemsAddedOrRemoved(object sender, EventArgs e)
		{
			_component.ItemsAddedOrRemoved();
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
