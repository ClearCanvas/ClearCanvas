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
using System.Collections.Generic;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyVisitHistoryComponentControl"/>
	/// </summary>
	public partial class BiographyVisitHistoryComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyVisitHistoryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyVisitHistoryComponentControl(BiographyVisitHistoryComponent component)
		{
			InitializeComponent();
			_component = component;

			_visitList.Table = _component.Visits;
			_visitList.DataBindings.Add("Selection", _component, "SelectedVisit", true, DataSourceUpdateMode.OnPropertyChanged);

			Control detailView = (Control)_component.VisitDetailComponentHost.ComponentView.GuiElement;
			detailView.Dock = DockStyle.Fill;
			_detailPanel.Controls.Add(detailView);
		}
	}
}
