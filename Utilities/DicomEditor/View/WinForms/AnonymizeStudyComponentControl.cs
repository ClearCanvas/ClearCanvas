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

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="AnonymizeStudyComponent"/>.
	/// </summary>
	public partial class AnonymizeStudyComponentControl : ApplicationComponentUserControl
	{
		private AnonymizeStudyComponent _component;
		private bool _updating = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnonymizeStudyComponentControl(AnonymizeStudyComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			this.AcceptButton = _okButton;
			this.CancelButton = _cancelButton;

			_patientId.DataBindings.Add("Value", _component, "PatientId", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientsName.DataBindings.Add("Value", _component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Value", _component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDate.DataBindings.Add("Value", _component, "StudyDate", true, DataSourceUpdateMode.OnPropertyChanged);
			_dateOfBirth.DataBindings.Add("Value", _component, "PatientsBirthDate", true, DataSourceUpdateMode.OnPropertyChanged);
			_preserveSeriesData.DataBindings.Add("Checked", _component, "PreserveSeriesData", true, DataSourceUpdateMode.OnPropertyChanged);

			_keepPrivateTags.Checked = _component.KeepPrivateTags;
			_keepPrivateTags.CheckedChanged += _keepPrivateTags_CheckedChanged;

			_keepReportsAndAttachments.Checked = _component.KeepReportsAndAttachments;
			_keepReportsAndAttachments.CheckedChanged += _keepReportsAndAttachments_CheckedChanged;

			_preserveSeriesData.Visible = _component.ShowPreserveSeriesData;
			_keepReportsAndAttachments.Visible = _component.ShowKeepReportsAndAttachments;
		}

		private void OnOkButtonClicked(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void OnCancelButtonClicked(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _keepPrivateTags_CheckedChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

			_updating = true;
			try
			{
				_component.KeepPrivateTags = _keepPrivateTags.Checked;
				_keepPrivateTags.Checked = _component.KeepPrivateTags;
				_warningProvider.SetError(_keepPrivateTags, _keepPrivateTags.Checked ? SR.WarningKeepingPrivateTagsIsPotentialPatientPrivacyIssue : string.Empty);
			}
			finally
			{
				_updating = false;
			}
		}

		private void _keepReportsAndAttachments_CheckedChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

			_updating = true;
			try
			{
				_component.KeepReportsAndAttachments = _keepReportsAndAttachments.Checked;
				_keepReportsAndAttachments.Checked = _component.KeepReportsAndAttachments;
				_warningProvider.SetError(_keepReportsAndAttachments, _keepReportsAndAttachments.Checked ? SR.WarningKeepReportsAndAttachmentsIsPotentialPatientPrivacyIssue : string.Empty);
			}
			finally
			{
				_updating = false;
			}
		}
	}
}