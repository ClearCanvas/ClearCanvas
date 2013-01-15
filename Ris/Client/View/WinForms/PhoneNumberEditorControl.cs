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
    public partial class PhoneNumberEditorControl : ApplicationComponentUserControl
    {
        private PhoneNumberEditorComponent _component;

        public PhoneNumberEditorControl(PhoneNumberEditorComponent component)
			: base(component)
        {
            InitializeComponent();
            _component = component;

            _phoneType.DataSource = _component.PhoneTypeChoices;
            _phoneType.DataBindings.Add("Value", _component, "PhoneType", true, DataSourceUpdateMode.OnPropertyChanged);
            _phoneType.DataBindings.Add("Enabled", _component, "PhoneTypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _countryCode.DataBindings.Add("Value", _component, "CountryCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _areaCode.DataBindings.Add("Value", _component, "AreaCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _number.DataBindings.Add("Value", _component, "Number", true, DataSourceUpdateMode.OnPropertyChanged);
            _extension.DataBindings.Add("Value", _component, "Extension", true, DataSourceUpdateMode.OnPropertyChanged);

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

        private void PhoneNumberEditorControl_Load(object sender, EventArgs e)
        {
            _number.Mask = _component.PhoneNumberMask;
        }
    }
}
