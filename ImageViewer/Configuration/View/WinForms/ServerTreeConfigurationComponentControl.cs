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
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class ServerTreeConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly ServerTreeConfigurationComponent _component;

		public ServerTreeConfigurationComponentControl(ServerTreeConfigurationComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_description.DataBindings.Add("Text", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
			_splitContainer.Panel2.Controls.Add((UserControl)_component.ServerTreeHost.ComponentView.GuiElement);
			_splitContainer.Panel2.Controls[0].Dock = DockStyle.Fill;
		}
	}
}