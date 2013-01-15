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
    partial class PerformedProcedureComponentControl
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
			this.panel1 = new System.Windows.Forms.Panel();
			this._splitContainerDocumentationDetails = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._mppsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._procedurePlanSummary = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._mppsDetailsPanel = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._splitContainerDocumentationDetails)).BeginInit();
			this._splitContainerDocumentationDetails.Panel1.SuspendLayout();
			this._splitContainerDocumentationDetails.Panel2.SuspendLayout();
			this._splitContainerDocumentationDetails.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this._splitContainerDocumentationDetails);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 5);
			this.panel1.Size = new System.Drawing.Size(760, 509);
			this.panel1.TabIndex = 2;
			// 
			// _splitContainerDocumentationDetails
			// 
			this._splitContainerDocumentationDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainerDocumentationDetails.Location = new System.Drawing.Point(4, 4);
			this._splitContainerDocumentationDetails.Name = "_splitContainerDocumentationDetails";
			this._splitContainerDocumentationDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainerDocumentationDetails.Panel1
			// 
			this._splitContainerDocumentationDetails.Panel1.Controls.Add(this.tableLayoutPanel1);
			// 
			// _splitContainerDocumentationDetails.Panel2
			// 
			this._splitContainerDocumentationDetails.Panel2.Controls.Add(this._mppsDetailsPanel);
			this._splitContainerDocumentationDetails.Size = new System.Drawing.Size(752, 500);
			this._splitContainerDocumentationDetails.SplitterDistance = 213;
			this._splitContainerDocumentationDetails.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this._mppsTableView, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._procedurePlanSummary, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(752, 213);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _mppsTableView
			// 
			this._mppsTableView.ColumnHeaderTooltip = null;
			this._mppsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mppsTableView.Location = new System.Drawing.Point(379, 16);
			this._mppsTableView.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._mppsTableView.MultiSelect = false;
			this._mppsTableView.Name = "_mppsTableView";
			this._mppsTableView.ReadOnly = false;
			this._mppsTableView.Size = new System.Drawing.Size(373, 194);
			this._mppsTableView.SortButtonTooltip = null;
			this._mppsTableView.TabIndex = 1;
			// 
			// _procedurePlanSummary
			// 
			this._procedurePlanSummary.ColumnHeaderTooltip = null;
			this._procedurePlanSummary.Dock = System.Windows.Forms.DockStyle.Fill;
			this._procedurePlanSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._procedurePlanSummary.Location = new System.Drawing.Point(0, 16);
			this._procedurePlanSummary.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this._procedurePlanSummary.Name = "_procedurePlanSummary";
			this._procedurePlanSummary.ReadOnly = false;
			this._procedurePlanSummary.Size = new System.Drawing.Size(373, 194);
			this._procedurePlanSummary.SortButtonTooltip = null;
			this._procedurePlanSummary.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Modality Procedure Steps";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(379, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Performed Procedure Steps";
			// 
			// _mppsDetailsPanel
			// 
			this._mppsDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mppsDetailsPanel.Location = new System.Drawing.Point(0, 0);
			this._mppsDetailsPanel.Name = "_mppsDetailsPanel";
			this._mppsDetailsPanel.Size = new System.Drawing.Size(752, 283);
			this._mppsDetailsPanel.TabIndex = 0;
			// 
			// PerformedProcedureComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "PerformedProcedureComponentControl";
			this.Size = new System.Drawing.Size(760, 509);
			this.panel1.ResumeLayout(false);
			this._splitContainerDocumentationDetails.Panel1.ResumeLayout(false);
			this._splitContainerDocumentationDetails.Panel1.PerformLayout();
			this._splitContainerDocumentationDetails.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._splitContainerDocumentationDetails)).EndInit();
			this._splitContainerDocumentationDetails.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContainerDocumentationDetails;
        private ClearCanvas.Desktop.View.WinForms.TableView _mppsTableView;
		private System.Windows.Forms.Panel _mppsDetailsPanel;
        private ClearCanvas.Desktop.View.WinForms.TableView _procedurePlanSummary;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;

    }
}
