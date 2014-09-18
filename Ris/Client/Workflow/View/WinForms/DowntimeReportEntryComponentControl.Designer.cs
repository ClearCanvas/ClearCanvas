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
    partial class DowntimeReportEntryComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DowntimeReportEntryComponentControl));
			this._radioToBeReported = new System.Windows.Forms.RadioButton();
			this._radioPasteReport = new System.Windows.Forms.RadioButton();
			this._reportText = new ClearCanvas.Desktop.View.WinForms.RichTextField();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._transcriptionistLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._interpreterLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.SuspendLayout();
			// 
			// _radioToBeReported
			// 
			resources.ApplyResources(this._radioToBeReported, "_radioToBeReported");
			this._radioToBeReported.Name = "_radioToBeReported";
			this._radioToBeReported.TabStop = true;
			this._radioToBeReported.UseVisualStyleBackColor = true;
			this._radioToBeReported.CheckedChanged += new System.EventHandler(this._radioToBeReported_CheckedChanged);
			// 
			// _radioPasteReport
			// 
			resources.ApplyResources(this._radioPasteReport, "_radioPasteReport");
			this._radioPasteReport.Name = "_radioPasteReport";
			this._radioPasteReport.TabStop = true;
			this._radioPasteReport.UseVisualStyleBackColor = true;
			this._radioPasteReport.CheckedChanged += new System.EventHandler(this._radioPasteReport_CheckedChanged);
			// 
			// _reportText
			// 
			resources.ApplyResources(this._reportText, "_reportText");
			this._reportText.MaximumLength = 2147483647;
			this._reportText.Name = "_reportText";
			this._reportText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
			this._reportText.Value = null;
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _transcriptionistLookup
			// 
			resources.ApplyResources(this._transcriptionistLookup, "_transcriptionistLookup");
			this._transcriptionistLookup.Name = "_transcriptionistLookup";
			this._transcriptionistLookup.Value = null;
			// 
			// _interpreterLookup
			// 
			resources.ApplyResources(this._interpreterLookup, "_interpreterLookup");
			this._interpreterLookup.Name = "_interpreterLookup";
			this._interpreterLookup.Value = null;
			// 
			// DowntimeReportEntryComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._transcriptionistLookup);
			this.Controls.Add(this._interpreterLookup);
			this.Controls.Add(this._reportText);
			this.Controls.Add(this._radioPasteReport);
			this.Controls.Add(this._radioToBeReported);
			this.Name = "DowntimeReportEntryComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.RadioButton _radioToBeReported;
		private System.Windows.Forms.RadioButton _radioPasteReport;
		private ClearCanvas.Desktop.View.WinForms.RichTextField _reportText;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _interpreterLookup;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _transcriptionistLookup;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
    }
}
