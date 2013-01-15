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

using System.Drawing;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class FolderContentsComponentControl
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
			this._folderContentsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._statusStrip.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _folderContentsTableView
			// 
			this._folderContentsTableView.ColumnHeaderTooltip = "Sort Page By";
			this._folderContentsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderContentsTableView.Location = new System.Drawing.Point(0, 0);
			this._folderContentsTableView.Margin = new System.Windows.Forms.Padding(0);
			this._folderContentsTableView.MultiSelect = false;
			this._folderContentsTableView.Name = "_folderContentsTableView";
			this._folderContentsTableView.ReadOnly = false;
			this._folderContentsTableView.Size = new System.Drawing.Size(358, 478);
			this._folderContentsTableView.SortButtonTooltip = "Sort Page By";
			this._folderContentsTableView.SortButtonVisible = true;
			this._folderContentsTableView.TabIndex = 0;
			this._folderContentsTableView.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._folderContentsTableView_ItemDrag);
			this._folderContentsTableView.ItemDoubleClicked += new System.EventHandler(this._folderContentsTableView_ItemDoubleClicked);
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusText,
            this._progressBar});
			this._statusStrip.Location = new System.Drawing.Point(0, 478);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(358, 22);
			this._statusStrip.SizingGrip = false;
			this._statusStrip.TabIndex = 1;
			this._statusStrip.Text = "statusStrip1";
			// 
			// _statusText
			// 
			this._statusText.Name = "_statusText";
			this._statusText.Size = new System.Drawing.Size(343, 17);
			this._statusText.Spring = true;
			this._statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _progressBar
			// 
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(100, 16);
			this._progressBar.Visible = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._statusStrip, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._folderContentsTableView, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(358, 500);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// FolderContentsComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FolderContentsComponentControl";
			this.Size = new System.Drawing.Size(358, 500);
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _folderContentsTableView;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel _statusText;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
