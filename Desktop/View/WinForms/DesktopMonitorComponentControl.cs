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

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DesktopMonitorComponent"/>
    /// </summary>
    public partial class DesktopMonitorComponentControl : ApplicationComponentUserControl
    {
        private DesktopMonitorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopMonitorComponentControl(DesktopMonitorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _windows.Table = _component.Windows;
            _windows.DataBindings.Add("Selection", _component, "SelectedWindow", true, DataSourceUpdateMode.OnPropertyChanged);

            _workspaces.Table = _component.Workspaces;
            _workspaces.DataBindings.Add("Selection", _component, "SelectedWorkspace", true, DataSourceUpdateMode.OnPropertyChanged);
            _shelves.Table = _component.Shelves;
            _shelves.DataBindings.Add("Selection", _component, "SelectedShelf", true, DataSourceUpdateMode.OnPropertyChanged);

            _events.Table = _component.EventLog;
        }

        private void _openWindow_Click(object sender, EventArgs e)
        {
            _component.OpenWindow();
        }

        private void _activateWindow_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedWindow();
        }

        private void _closeWindow_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedWindow();
        }

        private void _openWorkspace_Click(object sender, EventArgs e)
        {
            _component.OpenWorkspace();
        }

        private void _activateWorkspace_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedWorkspace();
        }

        private void _closeWorkspace_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedWorkspace();
        }

        private void _openShelf_Click(object sender, EventArgs e)
        {
            _component.OpenShelf();
        }

        private void _activateShelf_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedShelf();
        }

        private void _showShelf_Click(object sender, EventArgs e)
        {
            _component.ShowSelectedShelf();
        }

        private void _hideShelf_Click(object sender, EventArgs e)
        {
            _component.HideSelectedShelf();
        }

        private void _closeShelf_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedShelf();
        }
    }
}
