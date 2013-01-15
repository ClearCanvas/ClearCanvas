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
	/// Provides a Windows Forms user-interface for <see cref="VisitDetailsEditorComponent"/>
	/// </summary>
	public partial class VisitDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly VisitDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public VisitDetailsEditorComponentControl(VisitDetailsEditorComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			_visitNumber.DataBindings.Add("Value", _component, "VisitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_visitNumberAssigningAuthority.DataSource = _component.VisitNumberAssigningAuthorityChoices;
			_visitNumberAssigningAuthority.DataBindings.Add("Value", _component, "VisitNumberAssigningAuthority", true, DataSourceUpdateMode.OnPropertyChanged);

			_admitDateTime.DataBindings.Add("Value", _component, "AdmitDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_dischargeDateTime.DataBindings.Add("Value", _component, "DischargeDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_dischargeDisposition.DataBindings.Add("Value", _component, "DischargeDisposition", true, DataSourceUpdateMode.OnPropertyChanged);

			_vip.DataBindings.Add("Checked", _component, "Vip", true, DataSourceUpdateMode.OnPropertyChanged);
			_preadmitNumber.DataBindings.Add("Value", _component, "PreAdmitNumber", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientClass.DataSource = _component.PatientClassChoices;
			_patientClass.DataBindings.Add("Value", _component, "PatientClass", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientType.DataSource = _component.PatientTypeChoices;
			_patientType.DataBindings.Add("Value", _component, "PatientType", true, DataSourceUpdateMode.OnPropertyChanged);

			_admissionType.DataSource = _component.AdmissionTypeChoices;
			_admissionType.DataBindings.Add("Value", _component, "AdmissionType", true, DataSourceUpdateMode.OnPropertyChanged);

			_visitStatus.DataSource = _component.VisitStatusChoices;
			_visitStatus.DataBindings.Add("Value", _component, "VisitStatus", true, DataSourceUpdateMode.OnPropertyChanged);

			_facility.DataSource = _component.FacilityChoices;
			_facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.Format += delegate(object sender, ListControlConvertEventArgs e)
								{
									e.Value = _component.FormatFacility(e.ListItem);
								};

			_currentLocation.DataSource = _component.CurrentLocationChoices;
			_currentLocation.DataBindings.Add("Value", _component, "CurrentLocation", true, DataSourceUpdateMode.OnPropertyChanged);
			_currentLocation.Format += delegate(object sender, ListControlConvertEventArgs e)
								{
									e.Value = _component.FormatCurrentLocation(e.ListItem);
								};

			_currentRoom.DataBindings.Add("Value", _component, "CurrentRoom", true, DataSourceUpdateMode.OnPropertyChanged);
			_currentBed.DataBindings.Add("Value", _component, "CurrentBed", true, DataSourceUpdateMode.OnPropertyChanged);

			_ambulatoryStatus.DataSource = _component.AmbulatoryStatusChoices;
			_ambulatoryStatus.DataBindings.Add("Value", _component, "AmbulatoryStatus", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
