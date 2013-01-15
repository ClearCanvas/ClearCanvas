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
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerDetailsEditorComponent"/>
	/// </summary>
	public partial class ExternalPractitionerDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerDetailsEditorComponentControl(ExternalPractitionerDetailsEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			if (_component.HasWarning)
			{
				_warning.Text = _component.WarningMessage;
				_warning.Visible = true;
			}

			_familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
			_givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
			_middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);

			_licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_billingNumber.DataBindings.Add("Value", _component, "BillingNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_isVerified.DataBindings.Add("Checked", _component, "MarkVerified", true, DataSourceUpdateMode.OnPropertyChanged);
			_lastVerified.Text = _component.LastVerified;
			_isVerified.Visible= _component.CanVerify;
			_lastVerified.Visible = _component.CanVerify;
		}
	}
}
