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

using System.ComponentModel;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerContactPointDetailsEditorComponent"/>
	/// </summary>
	public partial class ExternalPractitionerContactPointDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerContactPointDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointDetailsEditorComponentControl(ExternalPractitionerContactPointDetailsEditorComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			_component.PropertyChanged += _component_PropertyChanged;

			if (_component.HasWarning)
			{
				_warning.Text = _component.WarningMessage;
				_warning.Visible = true;
			}

			_name.DataBindings.Add("Value", _component, "ContactPointName", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "ContactPointDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_isDefaultContactPoint.DataBindings.Add("Checked", _component, "IsDefaultContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_resultCommunicationMode.DataBindings.Add("Value", _component, "SelectedResultCommunicationMode", true, DataSourceUpdateMode.OnPropertyChanged);
			_resultCommunicationMode.DataSource = _component.ResultCommunicationModeChoices;
			_informationAuthority.DataBindings.Add("Value", _component, "SelectedInformationAuthority", true, DataSourceUpdateMode.OnPropertyChanged);
			_informationAuthority.DataSource = _component.InformationAuthorityChoices;
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsDefaultContactPoint")
			{
				if (_isDefaultContactPoint.Checked != _component.IsDefaultContactPoint)
				{
					_isDefaultContactPoint.Checked = _component.IsDefaultContactPoint;
				}
			}
		}
	}
}
