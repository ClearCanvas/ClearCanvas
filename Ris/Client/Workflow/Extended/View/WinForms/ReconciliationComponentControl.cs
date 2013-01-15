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
    /// Provides a Windows Forms user-interface for <see cref="PatientReconciliationComponent"/>
    /// </summary>
    public partial class ReconciliationComponentControl : CustomUserControl
    {
        private ReconciliationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReconciliationComponentControl(ReconciliationComponent component)
        {
            InitializeComponent();

            _component = component;

            _targetTableView.Table = _component.TargetProfileTable;
            _reconciliationTableView.Table = _component.ReconciliationProfileTable;

            Control diffView = (Control)_component.DiffComponentView.GuiElement;
            diffView.Dock = DockStyle.Fill;
            _diffViewPanel.Controls.Add(diffView);

            _reconcileButton.DataBindings.Add("Enabled", _component, "ReconcileEnabled");
        }

        private void _reconcileButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reconcile();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }


        private void _reconciliationTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedReconciliationProfile(_reconciliationTableView.Selection);
            }
        }

        private void _targetTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedTargetProfile(_targetTableView.Selection);
            }
        }

    }
}
