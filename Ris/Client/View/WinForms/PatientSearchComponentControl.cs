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
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientSearchComponent"/>
    /// </summary>
    public partial class PatientSearchComponentControl : ApplicationComponentUserControl
    {
        private readonly PatientSearchComponent _component;

        public PatientSearchComponentControl(PatientSearchComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            _searchResults.ToolbarModel = _component.ItemsToolbarModel;
            _searchResults.MenuModel = _component.ItemsContextMenuModel;
            _searchResults.Table = _component.Profiles;
            _searchResults.DataBindings.Add("Selection", _component, "SelectedProfile", true, DataSourceUpdateMode.OnPropertyChanged);

            _searchField.DataBindings.Add("Value", _component, "SearchString", true, DataSourceUpdateMode.OnPropertyChanged);
            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

        private void _searchResults_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.OpenPatient();
        }

    }
}
