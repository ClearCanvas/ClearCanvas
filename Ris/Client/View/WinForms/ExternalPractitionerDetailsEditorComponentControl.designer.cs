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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalPractitionerDetailsEditorComponentControl));
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
			resources.ApplyResources(this._licenseNumber, "_licenseNumber");
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.Value = null;
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
			// _billingNumber
			// 
			resources.ApplyResources(this._billingNumber, "_billingNumber");
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.Value = null;
			// 
			// _isVerified
			// 
			resources.ApplyResources(this._isVerified, "_isVerified");
			this._isVerified.Name = "_isVerified";
			this._isVerified.UseVisualStyleBackColor = true;
			// 
			// _lastVerified
			// 
			resources.ApplyResources(this._lastVerified, "_lastVerified");
			this._lastVerified.Name = "_lastVerified";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._warning, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _warning
			// 
			resources.ApplyResources(this._warning, "_warning");
			this._warning.BackColor = System.Drawing.Color.White;
			this._warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this._warning, 2);
			this._warning.ForeColor = System.Drawing.Color.Red;
			this._warning.Name = "_warning";
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
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// ExternalPractitionerDetailsEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerDetailsEditorComponentControl";
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
