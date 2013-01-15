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
	/// Provides a Windows Forms user-interface for <see cref="PriorReportComponent"/>
	/// </summary>
	public partial class PriorReportComponentControl : ApplicationComponentUserControl
	{
		private PriorReportComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PriorReportComponentControl(PriorReportComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			Control reportViewer = (Control)_component.ReportViewComponentHost.ComponentView.GuiElement;
			reportViewer.Dock = DockStyle.Fill;
			splitContainer1.Panel2.Controls.Add(reportViewer);

			_reportList.Table = _component.Reports;
			_reportList.DataBindings.Add("Selection", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioRelevantPriors.DataBindings.Add("Checked", _component, "RelevantPriorsOnly", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioAllPriors.DataBindings.Add("Checked", _component, "AllPriors", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
