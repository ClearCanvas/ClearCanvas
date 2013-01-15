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
    partial class PriorReportComponentControl
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._reportList = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._radioAllPriors = new System.Windows.Forms.RadioButton();
			this._radioRelevantPriors = new System.Windows.Forms.RadioButton();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(3, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitContainer1.Panel2.Margin = new System.Windows.Forms.Padding(3);
			this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(1);
			this.splitContainer1.Size = new System.Drawing.Size(437, 425);
			this.splitContainer1.SplitterDistance = 181;
			this.splitContainer1.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._reportList, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(437, 181);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _reportList
			// 
			this._reportList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._reportList.Location = new System.Drawing.Point(0, 51);
			this._reportList.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this._reportList.MultiSelect = false;
			this._reportList.Name = "_reportList";
			this._reportList.ReadOnly = false;
			this._reportList.ShowToolbar = false;
			this._reportList.Size = new System.Drawing.Size(437, 127);
			this._reportList.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._radioAllPriors);
			this.groupBox1.Controls.Add(this._radioRelevantPriors);
			this.groupBox1.Location = new System.Drawing.Point(0, 3);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(437, 42);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter";
			// 
			// _radioAllPriors
			// 
			this._radioAllPriors.AutoSize = true;
			this._radioAllPriors.Location = new System.Drawing.Point(90, 19);
			this._radioAllPriors.Name = "_radioAllPriors";
			this._radioAllPriors.Size = new System.Drawing.Size(36, 17);
			this._radioAllPriors.TabIndex = 1;
			this._radioAllPriors.TabStop = true;
			this._radioAllPriors.Text = "All";
			this._radioAllPriors.UseVisualStyleBackColor = true;
			// 
			// _radioRelevantPriors
			// 
			this._radioRelevantPriors.AutoSize = true;
			this._radioRelevantPriors.Location = new System.Drawing.Point(6, 19);
			this._radioRelevantPriors.Name = "_radioRelevantPriors";
			this._radioRelevantPriors.Size = new System.Drawing.Size(68, 17);
			this._radioRelevantPriors.TabIndex = 0;
			this._radioRelevantPriors.TabStop = true;
			this._radioRelevantPriors.Text = "Relevant";
			this._radioRelevantPriors.UseVisualStyleBackColor = true;
			// 
			// PriorReportComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.splitContainer1);
			this.Name = "PriorReportComponentControl";
			this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.Size = new System.Drawing.Size(443, 428);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _reportList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton _radioAllPriors;
        private System.Windows.Forms.RadioButton _radioRelevantPriors;

    }
}
