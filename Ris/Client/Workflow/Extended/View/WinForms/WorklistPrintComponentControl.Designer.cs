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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class WorklistPrintComponentControl
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
			this._printButton = new System.Windows.Forms.Button();
			this._closeButton = new System.Windows.Forms.Button();
			this._previewPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _printButton
			// 
			this._printButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._printButton.Location = new System.Drawing.Point(534, 506);
			this._printButton.Name = "_printButton";
			this._printButton.Size = new System.Drawing.Size(75, 23);
			this._printButton.TabIndex = 1;
			this._printButton.Text = "Print";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _closeButton
			// 
			this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeButton.Location = new System.Drawing.Point(615, 506);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(75, 23);
			this._closeButton.TabIndex = 2;
			this._closeButton.Text = "Close";
			this._closeButton.UseVisualStyleBackColor = true;
			this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
			// 
			// _previewPanel
			// 
			this._previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._previewPanel.Location = new System.Drawing.Point(4, 4);
			this._previewPanel.Name = "_previewPanel";
			this._previewPanel.Size = new System.Drawing.Size(686, 496);
			this._previewPanel.TabIndex = 0;
			// 
			// WorklistPrintComponentControl
			// 
			this.AcceptButton = this._printButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._closeButton;
			this.Controls.Add(this._previewPanel);
			this.Controls.Add(this._closeButton);
			this.Controls.Add(this._printButton);
			this.Name = "WorklistPrintComponentControl";
			this.Size = new System.Drawing.Size(693, 532);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _printButton;
		private System.Windows.Forms.Button _closeButton;
		private System.Windows.Forms.Panel _previewPanel;
    }
}
