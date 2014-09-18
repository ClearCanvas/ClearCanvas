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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class VisitDetailsEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisitDetailsEditorComponentControl));
			this._visitNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._visitNumberAssigningAuthority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._admitDateTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._dischargeDateTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._dischargeDisposition = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._preadmitNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._vip = new System.Windows.Forms.CheckBox();
			this._patientClass = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._visitStatus = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._patientType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._ambulatoryStatus = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._admissionType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._currentLocation = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._facility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._currentBed = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._currentRoom = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.SuspendLayout();
			// 
			// _visitNumber
			// 
			resources.ApplyResources(this._visitNumber, "_visitNumber");
			this._visitNumber.Mask = "";
			this._visitNumber.Name = "_visitNumber";
			this._visitNumber.Value = null;
			// 
			// _visitNumberAssigningAuthority
			// 
			resources.ApplyResources(this._visitNumberAssigningAuthority, "_visitNumberAssigningAuthority");
			this._visitNumberAssigningAuthority.DataSource = null;
			this._visitNumberAssigningAuthority.DisplayMember = "";
			this._visitNumberAssigningAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._visitNumberAssigningAuthority.Name = "_visitNumberAssigningAuthority";
			this._visitNumberAssigningAuthority.Value = null;
			// 
			// _admitDateTime
			// 
			resources.ApplyResources(this._admitDateTime, "_admitDateTime");
			this._admitDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._admitDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._admitDateTime.Name = "_admitDateTime";
			this._admitDateTime.Nullable = true;
			this._admitDateTime.ShowTime = true;
			this._admitDateTime.Value = null;
			// 
			// _dischargeDateTime
			// 
			resources.ApplyResources(this._dischargeDateTime, "_dischargeDateTime");
			this._dischargeDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dischargeDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dischargeDateTime.Name = "_dischargeDateTime";
			this._dischargeDateTime.Nullable = true;
			this._dischargeDateTime.ShowTime = true;
			this._dischargeDateTime.Value = null;
			// 
			// _dischargeDisposition
			// 
			resources.ApplyResources(this._dischargeDisposition, "_dischargeDisposition");
			this._dischargeDisposition.Mask = "";
			this._dischargeDisposition.Name = "_dischargeDisposition";
			this._dischargeDisposition.Value = null;
			// 
			// _preadmitNumber
			// 
			resources.ApplyResources(this._preadmitNumber, "_preadmitNumber");
			this._preadmitNumber.Mask = "";
			this._preadmitNumber.Name = "_preadmitNumber";
			this._preadmitNumber.Value = null;
			// 
			// _vip
			// 
			resources.ApplyResources(this._vip, "_vip");
			this._vip.Name = "_vip";
			this._vip.UseVisualStyleBackColor = true;
			// 
			// _patientClass
			// 
			resources.ApplyResources(this._patientClass, "_patientClass");
			this._patientClass.DataSource = null;
			this._patientClass.DisplayMember = "";
			this._patientClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._patientClass.Name = "_patientClass";
			this._patientClass.Value = null;
			// 
			// _visitStatus
			// 
			resources.ApplyResources(this._visitStatus, "_visitStatus");
			this._visitStatus.DataSource = null;
			this._visitStatus.DisplayMember = "";
			this._visitStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._visitStatus.Name = "_visitStatus";
			this._visitStatus.Value = null;
			// 
			// _patientType
			// 
			resources.ApplyResources(this._patientType, "_patientType");
			this._patientType.DataSource = null;
			this._patientType.DisplayMember = "";
			this._patientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._patientType.Name = "_patientType";
			this._patientType.Value = null;
			// 
			// _ambulatoryStatus
			// 
			resources.ApplyResources(this._ambulatoryStatus, "_ambulatoryStatus");
			this._ambulatoryStatus.DataSource = null;
			this._ambulatoryStatus.DisplayMember = "";
			this._ambulatoryStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ambulatoryStatus.Name = "_ambulatoryStatus";
			this._ambulatoryStatus.Value = null;
			// 
			// _admissionType
			// 
			this._admissionType.DataSource = null;
			this._admissionType.DisplayMember = "";
			this._admissionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._admissionType, "_admissionType");
			this._admissionType.Name = "_admissionType";
			this._admissionType.Value = null;
			// 
			// _currentLocation
			// 
			this._currentLocation.DataSource = null;
			this._currentLocation.DisplayMember = "";
			this._currentLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._currentLocation, "_currentLocation");
			this._currentLocation.Name = "_currentLocation";
			this._currentLocation.Value = null;
			// 
			// _facility
			// 
			this._facility.DataSource = null;
			this._facility.DisplayMember = "";
			this._facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._facility, "_facility");
			this._facility.Name = "_facility";
			this._facility.Value = null;
			// 
			// _currentBed
			// 
			resources.ApplyResources(this._currentBed, "_currentBed");
			this._currentBed.Mask = "";
			this._currentBed.Name = "_currentBed";
			this._currentBed.Value = null;
			// 
			// _currentRoom
			// 
			resources.ApplyResources(this._currentRoom, "_currentRoom");
			this._currentRoom.Mask = "";
			this._currentRoom.Name = "_currentRoom";
			this._currentRoom.Value = null;
			// 
			// VisitDetailsEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._currentBed);
			this.Controls.Add(this._currentRoom);
			this.Controls.Add(this._vip);
			this.Controls.Add(this._currentLocation);
			this.Controls.Add(this._ambulatoryStatus);
			this.Controls.Add(this._dischargeDisposition);
			this.Controls.Add(this._visitNumber);
			this.Controls.Add(this._visitStatus);
			this.Controls.Add(this._dischargeDateTime);
			this.Controls.Add(this._facility);
			this.Controls.Add(this._admissionType);
			this.Controls.Add(this._admitDateTime);
			this.Controls.Add(this._visitNumberAssigningAuthority);
			this.Controls.Add(this._preadmitNumber);
			this.Controls.Add(this._patientClass);
			this.Controls.Add(this._patientType);
			this.Name = "VisitDetailsEditorComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _visitNumber;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _visitNumberAssigningAuthority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _admitDateTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _dischargeDateTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _dischargeDisposition;
        private ClearCanvas.Desktop.View.WinForms.TextField _preadmitNumber;
        private System.Windows.Forms.CheckBox _vip;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _patientClass;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _visitStatus;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _patientType;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _ambulatoryStatus;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _admissionType;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _currentLocation;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _facility;
		private ClearCanvas.Desktop.View.WinForms.TextField _currentBed;
		private ClearCanvas.Desktop.View.WinForms.TextField _currentRoom;
    }
}
