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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DowntimeReportEntryComponent"/>
    /// </summary>
    public partial class DowntimeReportEntryComponentControl : ApplicationComponentUserControl
    {
        private DowntimeReportEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DowntimeReportEntryComponentControl(DowntimeReportEntryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

        	_radioPasteReport.Checked = _component.HasReport;
        	_radioToBeReported.Checked = !_component.HasReport;

			_reportText.Enabled = _component.HasReport;
			_interpreterLookup.Enabled = _component.HasReport;
			_transcriptionistLookup.Enabled = _component.HasReport;

        	_reportText.DataBindings.Add("Value", _component, "ReportText", true, DataSourceUpdateMode.OnPropertyChanged);

        	_interpreterLookup.LookupHandler = _component.InterpreterLookupHandler;
        	_transcriptionistLookup.LookupHandler = _component.TranscriptionistLookupHandler;

			_interpreterLookup.DataBindings.Add("Value", _component, "Interpreter", true, DataSourceUpdateMode.OnPropertyChanged);
			_transcriptionistLookup.DataBindings.Add("Value", _component, "Transcriptionist", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _radioToBeReported_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void _radioPasteReport_CheckedChanged(object sender, EventArgs e)
		{
			_component.HasReport = _radioPasteReport.Checked;
			_reportText.Enabled = _component.HasReport;
			_interpreterLookup.Enabled = _component.HasReport;
			_transcriptionistLookup.Enabled = _component.HasReport;
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
