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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	public partial class BiographyOrderReportsComponentControl : ApplicationComponentUserControl
	{
		private BiographyOrderReportsComponent _component;
		private ActionModelNode _toolbarActionModel;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiographyOrderReportsComponentControl(BiographyOrderReportsComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			Control reportPreview = (Control) _component.ReportPreviewComponentHost.ComponentView.GuiElement;
			reportPreview.Dock = DockStyle.Fill;
			_reportPreviewPanel.Controls.Add(reportPreview);

			_reports.DataSource = _component.Reports;
			_reports.DataBindings.Add("Value", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);
			_reports.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatReportListItem(e.ListItem); };

			_toolbarActionModel = _component.ActionModel;
			ToolStripBuilder.BuildToolbar(_toolstrip.Items, _toolbarActionModel.ChildNodes);

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			_reports.DataSource = _component.Reports;
		}
	}
}
