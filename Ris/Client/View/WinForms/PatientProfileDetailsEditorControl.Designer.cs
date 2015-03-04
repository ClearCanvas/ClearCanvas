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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PatientProfileDetailsEditorControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientProfileDetailsEditorControl));
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._sex = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._dateOfDeath = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._billingInformation = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._mrnAuthority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._insurer = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._healthcardVersionCode = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._healthcardExpiry = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _middleName
			// 
			resources.ApplyResources(this._middleName, "_middleName");
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			resources.ApplyResources(this._givenName, "_givenName");
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			resources.ApplyResources(this._familyName, "_familyName");
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.Value = null;
			// 
			// _sex
			// 
			this._sex.DataSource = null;
			this._sex.DisplayMember = "";
			this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._sex, "_sex");
			this._sex.Name = "_sex";
			this._sex.Value = null;
			// 
			// _dateOfDeath
			// 
			resources.ApplyResources(this._dateOfDeath, "_dateOfDeath");
			this._dateOfDeath.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dateOfDeath.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dateOfDeath.Name = "_dateOfDeath";
			this._dateOfDeath.Nullable = true;
			this._dateOfDeath.Value = null;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._billingInformation, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _billingInformation
			// 
			resources.ApplyResources(this._billingInformation, "_billingInformation");
			this._billingInformation.Mask = "";
			this._billingInformation.Name = "_billingInformation";
			this._billingInformation.Value = null;
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this._dateOfDeath, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this._familyName, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._givenName, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this._sex, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._middleName, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this._dateOfBirth, 1, 1);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// _dateOfBirth
			// 
			resources.ApplyResources(this._dateOfBirth, "_dateOfBirth");
			this._dateOfBirth.Mask = "";
			this._dateOfBirth.Name = "_dateOfBirth";
			this._dateOfBirth.Value = null;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
			this.tableLayoutPanel3.Controls.Add(this._healthcard, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._mrnAuthority, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._insurer, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this._healthcardVersionCode, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._healthcardExpiry, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this._mrn, 0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// _healthcard
			// 
			resources.ApplyResources(this._healthcard, "_healthcard");
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.Value = null;
			// 
			// _mrnAuthority
			// 
			resources.ApplyResources(this._mrnAuthority, "_mrnAuthority");
			this._mrnAuthority.DataSource = null;
			this._mrnAuthority.DisplayMember = "";
			this._mrnAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._mrnAuthority.Name = "_mrnAuthority";
			this._mrnAuthority.Value = null;
			// 
			// _insurer
			// 
			resources.ApplyResources(this._insurer, "_insurer");
			this._insurer.DataSource = null;
			this._insurer.DisplayMember = "";
			this._insurer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._insurer.Name = "_insurer";
			this._insurer.Value = null;
			// 
			// _healthcardVersionCode
			// 
			resources.ApplyResources(this._healthcardVersionCode, "_healthcardVersionCode");
			this._healthcardVersionCode.Mask = "";
			this._healthcardVersionCode.Name = "_healthcardVersionCode";
			this._healthcardVersionCode.Value = null;
			// 
			// _healthcardExpiry
			// 
			resources.ApplyResources(this._healthcardExpiry, "_healthcardExpiry");
			this._healthcardExpiry.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._healthcardExpiry.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._healthcardExpiry.Name = "_healthcardExpiry";
			this._healthcardExpiry.Nullable = true;
			this._healthcardExpiry.Value = null;
			// 
			// _mrn
			// 
			resources.ApplyResources(this._mrn, "_mrn");
			this._mrn.Mask = "";
			this._mrn.Name = "_mrn";
			this._mrn.Value = null;
			// 
			// PatientProfileDetailsEditorControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PatientProfileDetailsEditorControl";
			this.Load += new System.EventHandler(this.PatientEditorControl_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _sex;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _dateOfDeath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _mrnAuthority;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _insurer;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _healthcardExpiry;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcardVersionCode;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.TextField _dateOfBirth;
		private Desktop.View.WinForms.TextField _billingInformation;
    }
}