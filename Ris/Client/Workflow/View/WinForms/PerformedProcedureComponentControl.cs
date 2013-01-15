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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformedProcedureComponent"/>
    /// </summary>
    public partial class PerformedProcedureComponentControl : ApplicationComponentUserControl
    {
        private PerformedProcedureComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponentControl(PerformedProcedureComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component = component;

            _procedurePlanSummary.Table = _component.ProcedurePlanSummaryTable;
            _procedurePlanSummary.MenuModel = _component.ProcedurePlanTreeActionModel;
            _procedurePlanSummary.ToolbarModel = _component.ProcedurePlanTreeActionModel;

            _mppsTableView.Table = _component.MppsTable;
            _mppsTableView.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);
            _mppsTableView.MenuModel = _component.MppsTableActionModel;
            _mppsTableView.ToolbarModel = _component.MppsTableActionModel;

            Control detailsPage = (Control)_component.DetailsComponentHost.ComponentView.GuiElement;
            detailsPage.Dock = DockStyle.Fill;
            _mppsDetailsPanel.Controls.Add(detailsPage);
        }
    }
}
