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
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SettingsManagementComponent"/>
	/// </summary>
	public partial class SettingsManagementComponentControl : ApplicationComponentUserControl
	{
		private SettingsManagementComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public SettingsManagementComponentControl(SettingsManagementComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			_settingsGroupTableView.Table = _component.SettingsGroupTable;
			_settingsGroupTableView.DataBindings.Add("Selection", _component, "SelectedSettingsGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_settingsGroupTableView.ToolbarModel = _component.SettingsGroupsActionModel;

			_valueTableView.Table = _component.SettingsPropertiesTable;
			_valueTableView.DataBindings.Add("Selection", _component, "SelectedSettingsProperty", true, DataSourceUpdateMode.OnPropertyChanged);
			_valueTableView.ToolbarModel = _component.SettingsPropertiesActionModel;

			_valueTableView.ItemDoubleClicked += new EventHandler(ValueTableItemDoubleClicked);
		}

		private void ValueTableItemDoubleClicked(object sender, EventArgs e)
		{
			_component.SettingsPropertyDoubleClicked();
		}
	}
}