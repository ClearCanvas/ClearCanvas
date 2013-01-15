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
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="StaffDetailsEditorComponent"/>
	/// </summary>
	public partial class StaffDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly StaffDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public StaffDetailsEditorComponentControl(StaffDetailsEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_userLookup.LookupHandler = _component.UserLookupHandler;
			_userLookup.DataBindings.Add("Value", _component, "SelectedUser", true, DataSourceUpdateMode.OnPropertyChanged);

			_familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
			_givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
			_middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);

			_sex.DataSource = _component.SexChoices;
			_sex.DataBindings.Add("Value", _component, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);

			_staffId.DataBindings.Add("Value", _component, "StaffId", true, DataSourceUpdateMode.OnPropertyChanged);

			_staffType.DataSource = _component.StaffTypeChoices;
			_staffType.DataBindings.Add("Value", _component, "StaffType", true, DataSourceUpdateMode.OnPropertyChanged);

			_title.DataBindings.Add("Value", _component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_billingNumber.DataBindings.Add("Value", _component, "BillingNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			SetReadOnlyMode(_component.ReadOnly);
		}

		private void SetReadOnlyMode(bool readOnly)
		{
			_familyName.ReadOnly = readOnly;
			_givenName.ReadOnly = readOnly;
			_middleName.ReadOnly = readOnly;
			_staffId.ReadOnly = readOnly;
			_title.ReadOnly = readOnly;
			_licenseNumber.ReadOnly = readOnly;
			_billingNumber.ReadOnly = readOnly;

			_userLookup.Enabled = !readOnly && _component.IsUserAdmin;
			_sex.Enabled = !readOnly;
			_staffType.Enabled = !readOnly;
		}
	}
}
