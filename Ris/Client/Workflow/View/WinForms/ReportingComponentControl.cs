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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ReportingComponent"/>
	/// </summary>
	public partial class ReportingComponentControl : ApplicationComponentUserControl
	{
		private readonly ReportingComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReportingComponentControl(ReportingComponent component)
			: base(component)
		{
			_component = component;

			if (_component.UserCancelled)
				return;

			InitializeComponent();

			_overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

			var banner = (Control)_component.BannerHost.ComponentView.GuiElement;
			banner.Dock = DockStyle.Fill;
			_bannerPanel.Controls.Add(banner);

			var reportEditor = (Control)_component.ReportEditorHost.ComponentView.GuiElement;
			reportEditor.Dock = DockStyle.Fill;
			_reportEditorPanel.Controls.Add(reportEditor);

			var rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);

			_statusText.DataBindings.Add("Text", _component, "StatusText", true, DataSourceUpdateMode.OnPropertyChanged);
			_statusText.DataBindings.Add("Visible", _component, "StatusTextVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_imagesUnavailable.DataBindings.Add("Visible", _component, "ImageAvailabilityMessageVisible");
			_imagesUnavailable.DataBindings.Add("Text", _component, "ImageAvailabilityMessage");

			_hasErrors.DataBindings.Add("Text", _component, "HasErrorsText", true, DataSourceUpdateMode.OnPropertyChanged);
			_hasErrors.DataBindings.Add("Visible", _component, "HasErrorsVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_reportNextItem.DataBindings.Add("Checked", _component, "ReportNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
			_reportNextItem.DataBindings.Add("Enabled", _component, "ReportNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_verifyButton.DataBindings.Add("Enabled", _component, "VerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_submitForReviewButton.DataBindings.Add("Enabled", _component, "SubmitForReviewEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_returnToInterpreterButton.DataBindings.Add("Enabled", _component, "ReturnToInterpreterEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_sendToTranscriptionButton.DataBindings.Add("Enabled", _component, "SendToTranscriptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.LookupHandler = _component.SupervisorLookupHandler;
			_supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);
			_rememberSupervisorCheckbox.DataBindings.Add("Checked", _component, "RememberSupervisor", true, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.Visible = _component.SupervisorVisible;
			_rememberSupervisorCheckbox.Visible = _component.RememberSupervisorVisible;

			_priority.DataSource = _component.PriorityChoices;
			_priority.DataBindings.Add("Value", _component, "Priority", true, DataSourceUpdateMode.OnPropertyChanged);

			_verifyButton.Visible = _component.VerifyReportVisible;
			_submitForReviewButton.Visible = _component.SubmitForReviewVisible;
			_returnToInterpreterButton.Visible = _component.ReturnToInterpreterVisible;
			_sendToTranscriptionButton.Visible = _component.SendToTranscriptionVisible;

			_skipButton.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_saveButton.DataBindings.Add("Enabled", _component, "SaveReportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;

			_reportedProcedures.DataBindings.Add("Text", _component, "ProceduresText", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StatusText")
			{
				_statusText.Refresh();
			}
		}


		private void _verifyButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Verify();
			}
		}

		private void _submitForReviewButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SubmitForReview();
			}
		}

		private void _sendToInterpreterButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.ReturnToInterpreter();
			}
		}

		private void _sendToTranscriptionButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SendToTranscription();
			}
		}

		private void _saveButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SaveReport();
			}
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.CancelEditing();
			}
		}

		private void _skipButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Skip();
			}
		}
	}
}
