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
	public partial class ReportingMppsDocumentationComponentControl : ApplicationComponentUserControl
	{
		private readonly ReportingMppsDocumentationComponent _component;

		public ReportingMppsDocumentationComponentControl(ReportingMppsDocumentationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			_mppsTableView.Table = _component.MppsTable;
			_mppsTableView.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);

			_comments.DataBindings.Add("Text", _component, "MppsComments");
		}
	}
}
