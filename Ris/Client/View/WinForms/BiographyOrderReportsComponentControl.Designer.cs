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
    partial class BiographyOrderReportsComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiographyOrderReportsComponentControl));
			this._reportPreviewPanel = new System.Windows.Forms.Panel();
			this._reports = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._toolstrip = new System.Windows.Forms.ToolStrip();
			this.SuspendLayout();
			// 
			// _reportPreviewPanel
			// 
			resources.ApplyResources(this._reportPreviewPanel, "_reportPreviewPanel");
			this._reportPreviewPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._reportPreviewPanel.Name = "_reportPreviewPanel";
			// 
			// _reports
			// 
			resources.ApplyResources(this._reports, "_reports");
			this._reports.DataSource = null;
			this._reports.DisplayMember = "";
			this._reports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reports.Name = "_reports";
			this._reports.Value = null;
			// 
			// _toolstrip
			// 
			this._toolstrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this._toolstrip, "_toolstrip");
			this._toolstrip.GripMargin = new System.Windows.Forms.Padding(0);
			this._toolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolstrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolstrip.Name = "_toolstrip";
			// 
			// BiographyOrderReportsComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._toolstrip);
			this.Controls.Add(this._reports);
			this.Controls.Add(this._reportPreviewPanel);
			this.Name = "BiographyOrderReportsComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Panel _reportPreviewPanel;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reports;
		private System.Windows.Forms.ToolStrip _toolstrip;
    }
}
