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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocolGroupEditorComponent"/>
    /// </summary>
    public partial class ProtocolGroupEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocolGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupEditorComponentControl(ProtocolGroupEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _codesSelector.AvailableItemsTable = _component.AvailableProtocolCodes;
            _codesSelector.SelectedItemsTable = _component.SelectedProtocolCodes;
            _codesSelector.DataBindings.Add("SelectedItemsTableSelection", _component, "SelectedProtocolCodesSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _codesSelector.AppendToSelectedItemsActionModel(_component.SelectedProtocolCodesActionModel);
            
            _codesSelector.ItemAdded += OnItemsAddedOrRemoved;
            _codesSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _readingGroupsSelector.AvailableItemsTable = _component.AvailableReadingGroups;
            _readingGroupsSelector.SelectedItemsTable = _component.SelectedReadingGroups;
            _readingGroupsSelector.ItemAdded += OnItemsAddedOrRemoved;
            _readingGroupsSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
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
