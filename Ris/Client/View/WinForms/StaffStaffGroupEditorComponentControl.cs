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
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StaffStaffGroupEditorComponent"/>
    /// </summary>
    public partial class StaffStaffGroupEditorComponentControl : ApplicationComponentUserControl
    {
        private StaffStaffGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffStaffGroupEditorComponentControl(StaffStaffGroupEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _staffGroupSelector.AvailableItemsTable = _component.AvailableGroupsTable;
            _staffGroupSelector.SelectedItemsTable = _component.SelectedGroupsTable;
            _staffGroupSelector.ItemAdded += OnItemsAddedOrRemoved;
            _staffGroupSelector.ItemRemoved += OnItemsAddedOrRemoved;
        	_staffGroupSelector.ReadOnly = _component.ReadOnly;
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
        {
            _component.ItemsAddedOrRemoved();
        }
    }
}
