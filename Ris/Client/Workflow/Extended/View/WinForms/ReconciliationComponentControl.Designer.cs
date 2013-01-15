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
    partial class ReconciliationComponentControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._targetTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label2 = new System.Windows.Forms.Label();
			this._reconcileButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._reconciliationTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._diffViewPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._diffViewPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.30541F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 74.69459F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(994, 609);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._targetTableView, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(493, 141);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _targetTableView
			// 
			this._targetTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._targetTableView.AutoSize = true;
			this._targetTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._targetTableView.Location = new System.Drawing.Point(4, 17);
			this._targetTableView.Margin = new System.Windows.Forms.Padding(4);
			this._targetTableView.MultiSelect = false;
			this._targetTableView.Name = "_targetTableView";
			this._targetTableView.ReadOnly = false;
			this._targetTableView.ShowToolbar = false;
			this._targetTableView.Size = new System.Drawing.Size(485, 120);
			this._targetTableView.TabIndex = 0;
			this._targetTableView.TabStop = false;
			this._targetTableView.SelectionChanged += new System.EventHandler(this._targetTableView_SelectionChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(2, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Patient";
			// 
			// _reconcileButton
			// 
			this._reconcileButton.Location = new System.Drawing.Point(327, 2);
			this._reconcileButton.Margin = new System.Windows.Forms.Padding(2);
			this._reconcileButton.Name = "_reconcileButton";
			this._reconcileButton.Size = new System.Drawing.Size(85, 23);
			this._reconcileButton.TabIndex = 0;
			this._reconcileButton.Text = "Reconcile";
			this._reconcileButton.UseVisualStyleBackColor = true;
			this._reconcileButton.Click += new System.EventHandler(this._reconcileButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(416, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._reconcileButton);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(499, 575);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(493, 31);
			this.flowLayoutPanel1.TabIndex = 3;
			// 
			// _reconciliationTableView
			// 
			this._reconciliationTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._reconciliationTableView.AutoSize = true;
			this._reconciliationTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._reconciliationTableView.Location = new System.Drawing.Point(4, 17);
			this._reconciliationTableView.Margin = new System.Windows.Forms.Padding(4);
			this._reconciliationTableView.MultiSelect = false;
			this._reconciliationTableView.Name = "_reconciliationTableView";
			this._reconciliationTableView.ReadOnly = false;
			this._reconciliationTableView.ShowToolbar = false;
			this._reconciliationTableView.Size = new System.Drawing.Size(485, 120);
			this._reconciliationTableView.TabIndex = 1;
			this._reconciliationTableView.TabStop = false;
			this._reconciliationTableView.SelectionChanged += new System.EventHandler(this._reconciliationTableView_SelectionChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(2, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Reconciliation Candidates";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._reconciliationTableView, 0, 1);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(499, 2);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(493, 141);
			this.tableLayoutPanel3.TabIndex = 1;
			// 
			// _diffViewPanel
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._diffViewPanel, 2);
			this._diffViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diffViewPanel.Location = new System.Drawing.Point(2, 147);
			this._diffViewPanel.Margin = new System.Windows.Forms.Padding(2);
			this._diffViewPanel.Name = "_diffViewPanel";
			this._diffViewPanel.Size = new System.Drawing.Size(990, 424);
			this._diffViewPanel.TabIndex = 2;
			// 
			// ReconciliationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ReconciliationComponentControl";
			this.Size = new System.Drawing.Size(994, 609);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label2;
        private ClearCanvas.Desktop.View.WinForms.TableView _targetTableView;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TableView _reconciliationTableView;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _reconcileButton;
		private System.Windows.Forms.Panel _diffViewPanel;
    }
}
