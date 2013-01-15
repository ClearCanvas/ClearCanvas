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
    /// Provides a Windows Forms user-interface for <see cref="TranscriptionComponent"/>.
    /// </summary>
    public partial class TranscriptionComponentControl : ApplicationComponentUserControl
    {
        private TranscriptionComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TranscriptionComponentControl(TranscriptionComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

			Control banner = (Control)_component.BannerHost.ComponentView.GuiElement;
			banner.Dock = DockStyle.Fill;
			_bannerPanel.Controls.Add(banner);

			Control transcriptionEditor = (Control)_component.TranscriptionEditorHost.ComponentView.GuiElement;
			transcriptionEditor.Dock = DockStyle.Fill;
			_transcriptiontEditorPanel.Controls.Add(transcriptionEditor);

			Control rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);

			_statusText.DataBindings.Add("Text", _component, "StatusText", true, DataSourceUpdateMode.OnPropertyChanged);
			_statusText.DataBindings.Add("Visible", _component, "StatusTextVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_reportNextItem.DataBindings.Add("Checked", _component, "TranscribeNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
			_reportNextItem.DataBindings.Add("Enabled", _component, "TranscribeNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_completeButton.DataBindings.Add("Enabled", _component, "CompleteEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_rejectButton.DataBindings.Add("Enabled", _component, "RejectEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.LookupHandler = _component.SupervisorLookupHandler;
			_supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);
			_supervisor.Visible = _component.SupervisorVisible;

			_submitForReviewButton.DataBindings.Add("Enabled", _component, "SubmitForReviewEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_submitForReviewButton.Visible = _component.SubmitForReviewVisible;

			_btnSkip.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_saveButton.DataBindings.Add("Enabled", _component, "SaveReportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StatusText")
			{
				_statusText.Refresh();
			}
		}


		private void _completeButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Complete();
			}
		}

		private void _rejectButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Reject();
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

		private void _btnSkip_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Skip();
			}
		}

		private void _submitForReviewButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.SubmitForReview();
			}
		}
	}
}
