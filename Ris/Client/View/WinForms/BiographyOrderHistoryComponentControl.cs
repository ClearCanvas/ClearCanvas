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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyOrderHistoryComponentControl"/>
	/// </summary>
	public partial class BiographyOrderHistoryComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyOrderHistoryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyOrderHistoryComponentControl(BiographyOrderHistoryComponent component)
		{
			InitializeComponent();
			_component = component;

			_orderList.Table = _component.Orders;
			_orderList.DataBindings.Add("Selection", _component, "SelectedOrder", true, DataSourceUpdateMode.OnPropertyChanged);

			// Load initial value
			_banner.Text = _component.BannerText;
			_banner.DataBindings.Add("Text", _component, "BannerText", true, DataSourceUpdateMode.OnPropertyChanged);

			Control content = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			content.Dock = DockStyle.Fill;
			_tabHostPanel.Controls.Add(content);
		}
	}
}
