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
    partial class PrintReportComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReportComponentControl));
			this._contactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._practitionerLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._printButton = new System.Windows.Forms.Button();
			this._closeButton = new System.Windows.Forms.Button();
			this._closeOnPrint = new System.Windows.Forms.CheckBox();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _contactPoint
			// 
			this._contactPoint.DataSource = null;
			this._contactPoint.DisplayMember = "";
			this._contactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._contactPoint, "_contactPoint");
			this._contactPoint.Name = "_contactPoint";
			this._contactPoint.Value = null;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._practitionerLookup);
			this.groupBox2.Controls.Add(this._contactPoint);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// _practitionerLookup
			// 
			resources.ApplyResources(this._practitionerLookup, "_practitionerLookup");
			this._practitionerLookup.Name = "_practitionerLookup";
			this._practitionerLookup.Value = null;
			// 
			// _printButton
			// 
			resources.ApplyResources(this._printButton, "_printButton");
			this._printButton.Name = "_printButton";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _closeButton
			// 
			resources.ApplyResources(this._closeButton, "_closeButton");
			this._closeButton.Name = "_closeButton";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			// 
			// _closeOnPrint
			// 
			resources.ApplyResources(this._closeOnPrint, "_closeOnPrint");
			this._closeOnPrint.Name = "_closeOnPrint";
			this._closeOnPrint.UseVisualStyleBackColor = true;
			// 
			// PrintReportComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._closeButton;
			this.Controls.Add(this._closeOnPrint);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this._printButton);
			this.Name = "PrintReportComponentControl";
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _contactPoint;
		private LookupField _practitionerLookup;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button _printButton;
		private System.Windows.Forms.Button _closeButton;
		private System.Windows.Forms.CheckBox _closeOnPrint;
    }
}
