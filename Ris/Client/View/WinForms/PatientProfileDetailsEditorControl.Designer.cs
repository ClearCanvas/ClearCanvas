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
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._sex = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._dateOfDeath = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
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
			this._billingInformation = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _middleName
			// 
			this._middleName.LabelText = "Middle Name";
			this._middleName.Location = new System.Drawing.Point(280, 2);
			this._middleName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.PasswordChar = '\0';
			this._middleName.Size = new System.Drawing.Size(122, 38);
			this._middleName.TabIndex = 2;
			this._middleName.ToolTip = null;
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(141, 2);
			this._givenName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(118, 38);
			this._givenName.TabIndex = 1;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(2, 2);
			this._familyName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(116, 38);
			this._familyName.TabIndex = 0;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _sex
			// 
			this._sex.DataSource = null;
			this._sex.DisplayMember = "";
			this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._sex.LabelText = "Sex";
			this._sex.Location = new System.Drawing.Point(2, 44);
			this._sex.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._sex.Name = "_sex";
			this._sex.Size = new System.Drawing.Size(116, 44);
			this._sex.TabIndex = 3;
			this._sex.Value = null;
			// 
			// _dateOfDeath
			// 
			this._dateOfDeath.LabelText = "Date of Death";
			this._dateOfDeath.Location = new System.Drawing.Point(280, 44);
			this._dateOfDeath.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._dateOfDeath.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dateOfDeath.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dateOfDeath.Name = "_dateOfDeath";
			this._dateOfDeath.Nullable = true;
			this._dateOfDeath.Size = new System.Drawing.Size(122, 44);
			this._dateOfDeath.TabIndex = 5;
			this._dateOfDeath.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._billingInformation, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(424, 299);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.Controls.Add(this._dateOfDeath, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this._familyName, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._givenName, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this._sex, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._middleName, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this._dateOfBirth, 1, 1);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(417, 90);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _dateOfBirth
			// 
			this._dateOfBirth.LabelText = "Date of Birth";
			this._dateOfBirth.Location = new System.Drawing.Point(141, 44);
			this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._dateOfBirth.Mask = "";
			this._dateOfBirth.Name = "_dateOfBirth";
			this._dateOfBirth.PasswordChar = '\0';
			this._dateOfBirth.Size = new System.Drawing.Size(118, 41);
			this._dateOfBirth.TabIndex = 4;
			this._dateOfBirth.ToolTip = null;
			this._dateOfBirth.Value = null;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(418, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Patient Identifiers";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this._healthcard, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._mrnAuthority, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._insurer, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this._healthcardVersionCode, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._healthcardExpiry, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this._mrn, 0, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 112);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 3;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(418, 131);
			this.tableLayoutPanel3.TabIndex = 2;
			// 
			// _healthcard
			// 
			this._healthcard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._healthcard.AutoSize = true;
			this._healthcard.LabelText = "Healthcard";
			this._healthcard.Location = new System.Drawing.Point(2, 47);
			this._healthcard.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.PasswordChar = '\0';
			this._healthcard.Size = new System.Drawing.Size(190, 40);
			this._healthcard.TabIndex = 2;
			this._healthcard.ToolTip = null;
			this._healthcard.Value = null;
			// 
			// _mrnAuthority
			// 
			this._mrnAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mrnAuthority.AutoSize = true;
			this._mrnAuthority.DataSource = null;
			this._mrnAuthority.DisplayMember = "";
			this._mrnAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._mrnAuthority.LabelText = "Information Authority";
			this._mrnAuthority.Location = new System.Drawing.Point(211, 2);
			this._mrnAuthority.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
			this._mrnAuthority.Name = "_mrnAuthority";
			this._mrnAuthority.Size = new System.Drawing.Size(190, 41);
			this._mrnAuthority.TabIndex = 1;
			this._mrnAuthority.Value = null;
			// 
			// _insurer
			// 
			this._insurer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._insurer.AutoSize = true;
			this._insurer.DataSource = null;
			this._insurer.DisplayMember = "";
			this._insurer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._insurer.LabelText = "Insurer";
			this._insurer.Location = new System.Drawing.Point(211, 47);
			this._insurer.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
			this._insurer.Name = "_insurer";
			this._insurer.Size = new System.Drawing.Size(190, 41);
			this._insurer.TabIndex = 3;
			this._insurer.Value = null;
			// 
			// _healthcardVersionCode
			// 
			this._healthcardVersionCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._healthcardVersionCode.AutoSize = true;
			this._healthcardVersionCode.LabelText = "Version Code";
			this._healthcardVersionCode.Location = new System.Drawing.Point(2, 92);
			this._healthcardVersionCode.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
			this._healthcardVersionCode.Mask = "";
			this._healthcardVersionCode.Name = "_healthcardVersionCode";
			this._healthcardVersionCode.PasswordChar = '\0';
			this._healthcardVersionCode.Size = new System.Drawing.Size(190, 40);
			this._healthcardVersionCode.TabIndex = 4;
			this._healthcardVersionCode.ToolTip = null;
			this._healthcardVersionCode.Value = null;
			// 
			// _healthcardExpiry
			// 
			this._healthcardExpiry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._healthcardExpiry.AutoSize = true;
			this._healthcardExpiry.LabelText = "Healthcard Expiry Date";
			this._healthcardExpiry.Location = new System.Drawing.Point(211, 92);
			this._healthcardExpiry.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
			this._healthcardExpiry.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._healthcardExpiry.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._healthcardExpiry.Name = "_healthcardExpiry";
			this._healthcardExpiry.Nullable = true;
			this._healthcardExpiry.Size = new System.Drawing.Size(190, 40);
			this._healthcardExpiry.TabIndex = 5;
			this._healthcardExpiry.Value = null;
			// 
			// _mrn
			// 
			this._mrn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mrn.AutoSize = true;
			this._mrn.LabelText = "MRN";
			this._mrn.Location = new System.Drawing.Point(2, 2);
			this._mrn.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._mrn.Mask = "";
			this._mrn.Name = "_mrn";
			this._mrn.PasswordChar = '\0';
			this._mrn.Size = new System.Drawing.Size(192, 40);
			this._mrn.TabIndex = 0;
			this._mrn.ToolTip = null;
			this._mrn.Value = null;
			// 
			// _billingInformation
			// 
			this._billingInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._billingInformation.AutoSize = true;
			this._billingInformation.LabelText = "Billing/Insurance information";
			this._billingInformation.Location = new System.Drawing.Point(2, 248);
			this._billingInformation.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
			this._billingInformation.Mask = "";
			this._billingInformation.Name = "_billingInformation";
			this._billingInformation.PasswordChar = '\0';
			this._billingInformation.Size = new System.Drawing.Size(407, 40);
			this._billingInformation.TabIndex = 3;
			this._billingInformation.ToolTip = null;
			this._billingInformation.Value = null;
			// 
			// PatientProfileDetailsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "PatientProfileDetailsEditorControl";
			this.Size = new System.Drawing.Size(429, 301);
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