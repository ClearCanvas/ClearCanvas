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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="EnumerationSummaryComponent"/>
    /// </summary>
    public partial class EnumerationSummaryComponentControl : ApplicationComponentUserControl
    {
        private EnumerationSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationSummaryComponentControl(EnumerationSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _enumerationName.DataSource = _component.EnumerationChoices;
            _enumerationName.DataBindings.Add("Value", _component, "SelectedEnumeration", true, DataSourceUpdateMode.OnPropertyChanged);

            _enumerationClass.DataBindings.Add("Value", _component, "SelectedEnumerationClassName", true, DataSourceUpdateMode.OnPropertyChanged);


			_enumerationValuesTableView.Table = _component.SummaryTable;
			_enumerationValuesTableView.MenuModel = _component.SummaryTableActionModel;
			_enumerationValuesTableView.ToolbarModel = _component.SummaryTableActionModel;
			_enumerationValuesTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _enumerationValuesTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedItem();
        }
    }
}
