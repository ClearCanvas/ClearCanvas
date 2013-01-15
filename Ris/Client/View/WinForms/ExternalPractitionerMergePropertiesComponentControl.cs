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
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerMergePropertiesComponent"/>.
	/// </summary>
	public partial class ExternalPractitionerMergePropertiesComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerMergePropertiesComponent _component;

		public ExternalPractitionerMergePropertiesComponentControl(ExternalPractitionerMergePropertiesComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_instruction.DataBindings.Add("Text", _component, "Instruction", true, DataSourceUpdateMode.OnPropertyChanged);

			_name.DataSource = _component.NameChoices;
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.DataBindings.Add("Enabled", _component, "NameEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatName(e.ListItem); };

			_licenseNumber.DataSource = _component.LicenseNumberChoices;
			_licenseNumber.DataBindings.Add("Value", _component, "LicenseNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_licenseNumber.DataBindings.Add("Enabled", _component, "LicenseNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_billingNumber.DataSource = _component.BillingNumberChoices;
			_billingNumber.DataBindings.Add("Value", _component, "BillingNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_billingNumber.DataBindings.Add("Enabled", _component, "BillingNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_extendedProperties.Items.AddRange(_component.ExtendedPropertyChoices);

			_component.AllPropertiesChanged += OnAllPropertiesChanged;
			_component.SaveRequested += OnSaveRequested;
		}

		private void OnSaveRequested(object sender, System.EventArgs e)
		{
			_component.ExtendedProperties = _extendedProperties.CurrentValues;
		}

		private void OnAllPropertiesChanged(object sender, System.EventArgs e)
		{
			_name.DataSource = _component.NameChoices;
			_licenseNumber.DataSource = _component.LicenseNumberChoices;
			_billingNumber.DataSource = _component.BillingNumberChoices;

			_extendedProperties.Items.Clear();
			_extendedProperties.Items.AddRange(_component.ExtendedPropertyChoices);
		}
	}
}
