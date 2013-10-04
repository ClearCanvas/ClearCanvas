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

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    partial class AnonymizeStudyComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnonymizeStudyComponentControl));
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._studyDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._preserveSeriesData = new System.Windows.Forms.CheckBox();
			this._keepReportsAndAttachments = new System.Windows.Forms.CheckBox();
			this._tooltipProvider = new System.Windows.Forms.ToolTip(this.components);
			this._warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._keepPrivateTags = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._warningProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this.OnOkButtonClicked);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClicked);
			// 
			// _studyDate
			// 
			resources.ApplyResources(this._studyDate, "_studyDate");
			this._studyDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDate.Name = "_studyDate";
			this._studyDate.Nullable = true;
			this._studyDate.Value = new System.DateTime(2008, 4, 21, 9, 14, 8, 984);
			// 
			// _dateOfBirth
			// 
			resources.ApplyResources(this._dateOfBirth, "_dateOfBirth");
			this._dateOfBirth.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dateOfBirth.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dateOfBirth.Name = "_dateOfBirth";
			this._dateOfBirth.Nullable = true;
			this._dateOfBirth.Value = new System.DateTime(2008, 4, 21, 9, 11, 13, 140);
			// 
			// _patientsName
			// 
			resources.ApplyResources(this._patientsName, "_patientsName");
			this._patientsName.Mask = "";
			this._patientsName.Name = "_patientsName";
			this._patientsName.Value = null;
			// 
			// _accessionNumber
			// 
			resources.ApplyResources(this._accessionNumber, "_accessionNumber");
			this._accessionNumber.Mask = "";
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.Value = null;
			// 
			// _patientId
			// 
			resources.ApplyResources(this._patientId, "_patientId");
			this._patientId.Mask = "";
			this._patientId.Name = "_patientId";
			this._patientId.Value = null;
			// 
			// _studyDescription
			// 
			resources.ApplyResources(this._studyDescription, "_studyDescription");
			this._studyDescription.Mask = "";
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.Value = null;
			// 
			// _preserveSeriesData
			// 
			resources.ApplyResources(this._preserveSeriesData, "_preserveSeriesData");
			this._preserveSeriesData.Name = "_preserveSeriesData";
			this._preserveSeriesData.UseVisualStyleBackColor = true;
			// 
			// _keepReportsAndAttachments
			// 
			resources.ApplyResources(this._keepReportsAndAttachments, "_keepReportsAndAttachments");
			this._keepReportsAndAttachments.Name = "_keepReportsAndAttachments";
			this._tooltipProvider.SetToolTip(this._keepReportsAndAttachments, resources.GetString("_keepReportsAndAttachments.ToolTip"));
			this._keepReportsAndAttachments.UseVisualStyleBackColor = true;
			// 
			// _warningProvider
			// 
			this._warningProvider.ContainerControl = this;
			resources.ApplyResources(this._warningProvider, "_warningProvider");
			// 
			// _keepPrivateTags
			// 
			resources.ApplyResources(this._keepPrivateTags, "_keepPrivateTags");
			this._keepPrivateTags.Name = "_keepPrivateTags";
			this._keepPrivateTags.UseVisualStyleBackColor = true;
			// 
			// AnonymizeStudyComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._keepPrivateTags);
			this.Controls.Add(this._keepReportsAndAttachments);
			this.Controls.Add(this._preserveSeriesData);
			this.Controls.Add(this._studyDescription);
			this.Controls.Add(this._patientId);
			this.Controls.Add(this._accessionNumber);
			this.Controls.Add(this._patientsName);
			this.Controls.Add(this._dateOfBirth);
			this.Controls.Add(this._studyDate);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Name = "AnonymizeStudyComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._warningProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _studyDate;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _dateOfBirth;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientId;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private System.Windows.Forms.CheckBox _preserveSeriesData;
		private System.Windows.Forms.CheckBox _keepReportsAndAttachments;
		private System.Windows.Forms.ToolTip _tooltipProvider;
		private System.Windows.Forms.ErrorProvider _warningProvider;
		private System.Windows.Forms.CheckBox _keepPrivateTags;
    }
}
