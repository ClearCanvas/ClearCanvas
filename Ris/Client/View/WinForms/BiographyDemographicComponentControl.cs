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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyDemographicComponent"/>
	/// </summary>
	public partial class BiographyDemographicComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyDemographicComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyDemographicComponentControl(BiographyDemographicComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_selectedProfile.DataSource = _component.ProfileChoices;
			_selectedProfile.DataBindings.Add("Value", _component, "SelectedProfile", true, DataSourceUpdateMode.OnPropertyChanged);
			_selectedProfile.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatPatientProfile(e.ListItem); };

			var profileViewer = (Control)_component.ProfileViewComponentHost.ComponentView.GuiElement;
			profileViewer.Dock = DockStyle.Fill;
			this._demoHostPanel.Controls.Add(profileViewer);

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			_selectedProfile.DataSource = _component.ProfileChoices;
		}
	}
}
