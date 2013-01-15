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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="LocationEditorComponent"/>
	/// </summary>
	public partial class LocationEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly LocationEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public LocationEditorComponentControl(LocationEditorComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_id.DataBindings.Add("Value", _component, "Id", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

			_facility.DataSource = _component.FacilityChoices;
			_facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.Format += delegate(object sender, ListControlConvertEventArgs e)
								{
									e.Value = _component.FormatFacility(e.ListItem);
								};

			_building.DataBindings.Add("Value", _component, "Building", true, DataSourceUpdateMode.OnPropertyChanged);
			_floor.DataBindings.Add("Value", _component, "Floor", true, DataSourceUpdateMode.OnPropertyChanged);
			_pointOfCare.DataBindings.Add("Value", _component, "PointOfCare", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
