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
    partial class MergeOrdersComponentControl
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
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._mergedOrderPreviewPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this._ordersTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(443, 594);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 4;
			this._acceptButton.Text = "Merge";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(524, 594);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _mergedOrderPreviewPanel
			// 
			this._mergedOrderPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mergedOrderPreviewPanel.Location = new System.Drawing.Point(3, 164);
			this._mergedOrderPreviewPanel.Name = "_mergedOrderPreviewPanel";
			this._mergedOrderPreviewPanel.Size = new System.Drawing.Size(599, 424);
			this._mergedOrderPreviewPanel.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 148);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(125, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Preview of Merged Order";
			// 
			// _ordersTableView
			// 
			this._ordersTableView.AutoScroll = true;
			this._ordersTableView.ColumnHeaderTooltip = null;
			this._ordersTableView.FilterTextBoxWidth = 132;
			this._ordersTableView.Location = new System.Drawing.Point(3, 24);
			this._ordersTableView.Margin = new System.Windows.Forms.Padding(0);
			this._ordersTableView.MultiSelect = false;
			this._ordersTableView.Name = "_ordersTableView";
			this._ordersTableView.ReadOnly = false;
			this._ordersTableView.ShowToolbar = false;
			this._ordersTableView.Size = new System.Drawing.Size(596, 115);
			this._ordersTableView.SortButtonTooltip = null;
			this._ordersTableView.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(384, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Select the order to retain - the other orders will be merged into the selected or" +
    "der";
			// 
			// MergeOrdersComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.label2);
			this.Controls.Add(this._ordersTableView);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._mergedOrderPreviewPanel);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "MergeOrdersComponentControl";
			this.Size = new System.Drawing.Size(605, 623);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Panel _mergedOrderPreviewPanel;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TableView _ordersTableView;
		private System.Windows.Forms.Label label2;
    }
}
