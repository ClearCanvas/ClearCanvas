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
			this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this._orderDocumentationPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._btnComplete = new System.Windows.Forms.Button();
			this._btnSave = new System.Windows.Forms.Button();
			this._assignedRadiologistLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._overviewLayoutPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _overviewLayoutPanel
			// 
			this._overviewLayoutPanel.ColumnCount = 1;
			this._overviewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
			this._overviewLayoutPanel.Controls.Add(this.panel1, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._orderDocumentationPanel, 0, 1);
			this._overviewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._overviewLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			this._overviewLayoutPanel.RowCount = 3;
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._overviewLayoutPanel.Size = new System.Drawing.Size(1033, 628);
			this._overviewLayoutPanel.TabIndex = 0;
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(1027, 89);
			this._bannerPanel.TabIndex = 0;
			// 
			// _orderDocumentationPanel
			// 
			this._orderDocumentationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderDocumentationPanel.Location = new System.Drawing.Point(3, 98);
			this._orderDocumentationPanel.Name = "_orderDocumentationPanel";
			this._orderDocumentationPanel.Size = new System.Drawing.Size(1027, 476);
			this._orderDocumentationPanel.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._btnComplete);
			this.panel1.Controls.Add(this._btnSave);
			this.panel1.Controls.Add(this._assignedRadiologistLookup);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 580);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1027, 45);
			this.panel1.TabIndex = 2;
			// 
			// _btnComplete
			// 
			this._btnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnComplete.Location = new System.Drawing.Point(838, 19);
			this._btnComplete.Name = "_btnComplete";
			this._btnComplete.Size = new System.Drawing.Size(105, 23);
			this._btnComplete.TabIndex = 1;
			this._btnComplete.Text = "Complete";
			this._btnComplete.UseVisualStyleBackColor = true;
			this._btnComplete.Click += new System.EventHandler(this._btnComplete_Click);
			// 
			// _btnSave
			// 
			this._btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSave.Location = new System.Drawing.Point(949, 19);
			this._btnSave.Name = "_btnSave";
			this._btnSave.Size = new System.Drawing.Size(75, 23);
			this._btnSave.TabIndex = 2;
			this._btnSave.Text = "Save";
			this._btnSave.UseVisualStyleBackColor = true;
			this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
			// 
			// _assignedRadiologistLookup
			// 
			this._assignedRadiologistLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._assignedRadiologistLookup.AutoSize = true;
			this._assignedRadiologistLookup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._assignedRadiologistLookup.LabelText = "Assigned Radiologist:";
			this._assignedRadiologistLookup.Location = new System.Drawing.Point(576, 0);
			this._assignedRadiologistLookup.Margin = new System.Windows.Forms.Padding(2);
			this._assignedRadiologistLookup.Name = "_assignedRadiologistLookup";
			this._assignedRadiologistLookup.Size = new System.Drawing.Size(230, 43);
			this._assignedRadiologistLookup.TabIndex = 0;
			this._assignedRadiologistLookup.Value = null;
			// 
			// PerformingDocumentationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "PerformingDocumentationComponentControl";
			this.Size = new System.Drawing.Size(1033, 628);
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
