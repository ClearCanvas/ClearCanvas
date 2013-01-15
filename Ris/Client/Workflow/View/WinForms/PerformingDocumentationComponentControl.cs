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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PerformingDocumentationComponent"/>
    /// </summary>
    public partial class PerformingDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly PerformingDocumentationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformingDocumentationComponentControl(PerformingDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

            Control banner = (Control) _component.BannerHost.ComponentView.GuiElement;
            banner.Dock = DockStyle.Fill;
            _bannerPanel.Controls.Add(banner);

            Control documentationTabs = (Control)_component.DocumentationHost.ComponentView.GuiElement;
            documentationTabs.Dock = DockStyle.Fill;
            _orderDocumentationPanel.Controls.Add(documentationTabs);

            _btnComplete.DataBindings.Add("Enabled", _component, "CompleteEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_btnComplete.DataBindings.Add("Visible", _component, "CompleteVisible", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnSave.DataBindings.Add("Text", _component, "SaveText", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnSave.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _assignedRadiologistLookup.LookupHandler = _component.RadiologistLookupHandler;
            _assignedRadiologistLookup.DataBindings.Add("Value", _component, "AssignedRadiologist", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            using(new CursorManager(Cursors.WaitCursor))
            {
                _component.SaveDocumentation();
            }
        }

        private void _btnComplete_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.CompleteDocumentation();
            }
        }
    }
}
