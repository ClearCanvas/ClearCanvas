#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class SeriesDetailsComponentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesDetailsComponentControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._dob = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDate = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._seriesTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._close = new System.Windows.Forms.Button();
			this._refresh = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._accessionNumber);
			this.splitContainer1.Panel1.Controls.Add(this._dob);
			this.splitContainer1.Panel1.Controls.Add(this._studyDescription);
			this.splitContainer1.Panel1.Controls.Add(this._studyDate);
			this.splitContainer1.Panel1.Controls.Add(this._patientId);
			this.splitContainer1.Panel1.Controls.Add(this._patientsName);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._seriesTable);
			// 
			// _accessionNumber
			// 
			resources.ApplyResources(this._accessionNumber, "_accessionNumber");
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.ReadOnly = true;
			this._accessionNumber.ToolTip = null;
			this._accessionNumber.Value = null;
			// 
			// _dob
			// 
			resources.ApplyResources(this._dob, "_dob");
			this._dob.Name = "_dob";
			this._dob.ReadOnly = true;
			this._dob.ToolTip = null;
			this._dob.Value = null;
			// 
			// _studyDescription
			// 
			resources.ApplyResources(this._studyDescription, "_studyDescription");
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.ReadOnly = true;
			this._studyDescription.ToolTip = null;
			this._studyDescription.Value = null;
			// 
			// _studyDate
			// 
			resources.ApplyResources(this._studyDate, "_studyDate");
			this._studyDate.Name = "_studyDate";
			this._studyDate.ReadOnly = true;
			this._studyDate.ToolTip = null;
			this._studyDate.Value = null;
			// 
			// _patientId
			// 
			resources.ApplyResources(this._patientId, "_patientId");
			this._patientId.Name = "_patientId";
			this._patientId.ReadOnly = true;
			this._patientId.ToolTip = null;
			this._patientId.Value = null;
			// 
			// _patientsName
			// 
			resources.ApplyResources(this._patientsName, "_patientsName");
			this._patientsName.Name = "_patientsName";
			this._patientsName.ReadOnly = true;
			this._patientsName.ToolTip = null;
			this._patientsName.Value = null;
			// 
			// _seriesTable
			// 
			this._seriesTable.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._seriesTable, "_seriesTable");
			this._seriesTable.Name = "_seriesTable";
			this._seriesTable.ReadOnly = false;
			this._seriesTable.SortButtonTooltip = null;
			this._seriesTable.SelectionChanged += new System.EventHandler(this._seriesTable_SelectionChanged);
			// 
			// _close
			// 
			resources.ApplyResources(this._close, "_close");
			this._close.Name = "_close";
			this._close.UseVisualStyleBackColor = true;
			this._close.Click += new System.EventHandler(this._close_Click);
			// 
			// _refresh
			// 
			resources.ApplyResources(this._refresh, "_refresh");
			this._refresh.Name = "_refresh";
			this._refresh.UseVisualStyleBackColor = true;
			this._refresh.Click += new System.EventHandler(this._refresh_Click);
			// 
			// SeriesDetailsComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._refresh);
			this.Controls.Add(this._close);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SeriesDetailsComponentControl";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDate;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientId;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.TableView _seriesTable;
		private ClearCanvas.Desktop.View.WinForms.TextField _dob;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private System.Windows.Forms.Button _close;
		private System.Windows.Forms.Button _refresh;


	}
}
