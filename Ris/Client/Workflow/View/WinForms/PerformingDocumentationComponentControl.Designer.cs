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
    partial class PerformingDocumentationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformingDocumentationComponentControl));
			this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._btnComplete = new System.Windows.Forms.Button();
			this._btnSave = new System.Windows.Forms.Button();
			this._assignedRadiologistLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._orderDocumentationPanel = new System.Windows.Forms.Panel();
			this._overviewLayoutPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _overviewLayoutPanel
			// 
			resources.ApplyResources(this._overviewLayoutPanel, "_overviewLayoutPanel");
			this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
			this._overviewLayoutPanel.Controls.Add(this.panel1, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._orderDocumentationPanel, 0, 1);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			// 
			// _bannerPanel
			// 
			resources.ApplyResources(this._bannerPanel, "_bannerPanel");
			this._bannerPanel.Name = "_bannerPanel";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._btnComplete);
			this.panel1.Controls.Add(this._btnSave);
			this.panel1.Controls.Add(this._assignedRadiologistLookup);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _btnComplete
			// 
			resources.ApplyResources(this._btnComplete, "_btnComplete");
			this._btnComplete.Name = "_btnComplete";
			this._btnComplete.UseVisualStyleBackColor = true;
			this._btnComplete.Click += new System.EventHandler(this._btnComplete_Click);
			// 
			// _btnSave
			// 
			resources.ApplyResources(this._btnSave, "_btnSave");
			this._btnSave.Name = "_btnSave";
			this._btnSave.UseVisualStyleBackColor = true;
			this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
			// 
			// _assignedRadiologistLookup
			// 
			resources.ApplyResources(this._assignedRadiologistLookup, "_assignedRadiologistLookup");
			this._assignedRadiologistLookup.Name = "_assignedRadiologistLookup";
			this._assignedRadiologistLookup.Value = null;
			// 
			// _orderDocumentationPanel
			// 
			resources.ApplyResources(this._orderDocumentationPanel, "_orderDocumentationPanel");
			this._orderDocumentationPanel.Name = "_orderDocumentationPanel";
			// 
			// PerformingDocumentationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "PerformingDocumentationComponentControl";
			this._overviewLayoutPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _overviewLayoutPanel;
        private System.Windows.Forms.Button _btnComplete;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.Panel _orderDocumentationPanel;
        private System.Windows.Forms.Panel panel1;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _assignedRadiologistLookup;
    }
}
