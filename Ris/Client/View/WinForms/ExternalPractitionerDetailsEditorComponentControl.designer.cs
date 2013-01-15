#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
    partial class ExternalPractitionerDetailsEditorComponentControl
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
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._isVerified = new System.Windows.Forms.CheckBox();
			this._lastVerified = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._warning = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _licenseNumber
			// 
			this._licenseNumber.LabelText = "License Number";
			this._licenseNumber.Location = new System.Drawing.Point(20, 122);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.PasswordChar = '\0';
			this._licenseNumber.Size = new System.Drawing.Size(150, 41);
			this._licenseNumber.TabIndex = 3;
			this._licenseNumber.ToolTip = null;
			this._licenseNumber.Value = null;
			// 
			// _middleName
			// 
			this._middleName.LabelText = "Middle Name";
			this._middleName.Location = new System.Drawing.Point(20, 68);
			this._middleName.Margin = new System.Windows.Forms.Padding(2);
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.PasswordChar = '\0';
			this._middleName.Size = new System.Drawing.Size(150, 41);
			this._middleName.TabIndex = 2;
			this._middleName.ToolTip = null;
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(186, 13);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(150, 41);
			this._givenName.TabIndex = 1;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(20, 13);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(150, 41);
			this._familyName.TabIndex = 0;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.LabelText = "Billing Number";
			this._billingNumber.Location = new System.Drawing.Point(186, 122);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.PasswordChar = '\0';
			this._billingNumber.Size = new System.Drawing.Size(150, 41);
			this._billingNumber.TabIndex = 4;
			this._billingNumber.ToolTip = null;
			this._billingNumber.Value = null;
			// 
			// _isVerified
			// 
			this._isVerified.AutoSize = true;
			this._isVerified.Location = new System.Drawing.Point(24, 194);
			this._isVerified.Name = "_isVerified";
			this._isVerified.Size = new System.Drawing.Size(61, 17);
			this._isVerified.TabIndex = 6;
			this._isVerified.Text = "Verified";
			this._isVerified.UseVisualStyleBackColor = true;
			// 
			// _lastVerified
			// 
			this._lastVerified.AutoSize = true;
			this._lastVerified.Location = new System.Drawing.Point(92, 196);
			this._lastVerified.Name = "_lastVerified";
			this._lastVerified.Size = new System.Drawing.Size(64, 13);
			this._lastVerified.TabIndex = 7;
			this._lastVerified.Text = "Last verified";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._warning, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(360, 254);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// _warning
			// 
			this._warning.AutoSize = true;
			this._warning.BackColor = System.Drawing.Color.White;
			this._warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this._warning, 2);
			this._warning.Dock = System.Windows.Forms.DockStyle.Fill;
			this._warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._warning.ForeColor = System.Drawing.Color.Red;
			this._warning.Location = new System.Drawing.Point(3, 3);
			this._warning.Margin = new System.Windows.Forms.Padding(3);
			this._warning.Name = "_warning";
			this._warning.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._warning.Size = new System.Drawing.Size(354, 22);
			this._warning.TabIndex = 2;
			this._warning.Text = "Warning Message";
			this._warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._warning.Visible = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._familyName);
			this.panel1.Controls.Add(this._lastVerified);
			this.panel1.Controls.Add(this._licenseNumber);
			this.panel1.Controls.Add(this._isVerified);
			this.panel1.Controls.Add(this._givenName);
			this.panel1.Controls.Add(this._billingNumber);
			this.panel1.Controls.Add(this._middleName);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 28);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(360, 226);
			this.panel1.TabIndex = 0;
			// 
			// ExternalPractitionerDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(360, 254);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _licenseNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
		private ClearCanvas.Desktop.View.WinForms.TextField _billingNumber;
		private System.Windows.Forms.CheckBox _isVerified;
		private System.Windows.Forms.Label _lastVerified;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label _warning;

    }
}
