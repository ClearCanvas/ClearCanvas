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
    public partial class AddressEditorControl : ApplicationComponentUserControl
    {
        private AddressEditorComponent _component;

        public AddressEditorControl(AddressEditorComponent component)
			:base(component)
        {
            InitializeComponent();
            _component = component;

            _type.DataSource = _component.AddressTypeChoices;
            _type.DataBindings.Add("Value", _component, "AddressType", true, DataSourceUpdateMode.OnPropertyChanged);
            _type.DataBindings.Add("Enabled", _component, "AddressTypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _province.DataSource = _component.ProvinceChoices;
            _province.DataBindings.Add("Value", _component, "Province", true, DataSourceUpdateMode.OnPropertyChanged);

            _country.DataSource = _component.CountryChoices;
            _country.DataBindings.Add("Value", _component, "Country", true, DataSourceUpdateMode.OnPropertyChanged);

            _street.DataBindings.Add("Value", _component, "Street", true, DataSourceUpdateMode.OnPropertyChanged);
            _unit.DataBindings.Add("Value", _component, "Unit", true, DataSourceUpdateMode.OnPropertyChanged);
            _city.DataBindings.Add("Value", _component, "City", true, DataSourceUpdateMode.OnPropertyChanged);
            _postalCode.DataBindings.Add("Value", _component, "PostalCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _validFrom.DataBindings.Add("Value", _component, "ValidFrom", true, DataSourceUpdateMode.OnPropertyChanged);
            _validUntil.DataBindings.Add("Value", _component, "ValidUntil", true, DataSourceUpdateMode.OnPropertyChanged);

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
