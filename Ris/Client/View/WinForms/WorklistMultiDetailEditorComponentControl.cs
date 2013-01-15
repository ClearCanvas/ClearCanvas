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
    /// Provides a Windows Forms user-interface for <see cref="WorklistMultiDetailEditorComponent"/>
    /// </summary>
    public partial class WorklistMultiDetailEditorComponentControl : ApplicationComponentUserControl
    {
        private WorklistMultiDetailEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistMultiDetailEditorComponentControl(WorklistMultiDetailEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _worklistCategory.DataSource = _component.CategoryChoices;
            _worklistCategory.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);

            _defaultName.DataBindings.Add("Value", _component, "DefaultWorklistName", true, DataSourceUpdateMode.OnPropertyChanged);

            _worklistClassTableView.MenuModel = _component.WorklistActionModel;
            _worklistClassTableView.ToolbarModel = _component.WorklistActionModel;
            _worklistClassTableView.Table = _component.WorklistTable;
            _worklistClassTableView.DataBindings.Add("Selection", _component, "SelectedWorklist", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _worklistClassTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.EditSelectedWorklist();
        }
    }
}
