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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformedProcedureComponentControl));
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
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _splitContainerDocumentationDetails
			// 
			resources.ApplyResources(this._splitContainerDocumentationDetails, "_splitContainerDocumentationDetails");
			this._splitContainerDocumentationDetails.Name = "_splitContainerDocumentationDetails";
			// 
			// _splitContainerDocumentationDetails.Panel1
			// 
			this._splitContainerDocumentationDetails.Panel1.Controls.Add(this.tableLayoutPanel1);
			// 
			// _splitContainerDocumentationDetails.Panel2
			// 
			this._splitContainerDocumentationDetails.Panel2.Controls.Add(this._mppsDetailsPanel);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._mppsTableView, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._procedurePlanSummary, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _mppsTableView
			// 
			resources.ApplyResources(this._mppsTableView, "_mppsTableView");
			this._mppsTableView.MultiSelect = false;
			this._mppsTableView.Name = "_mppsTableView";
			this._mppsTableView.ReadOnly = false;
			// 
			// _procedurePlanSummary
			// 
			resources.ApplyResources(this._procedurePlanSummary, "_procedurePlanSummary");
			this._procedurePlanSummary.Name = "_procedurePlanSummary";
			this._procedurePlanSummary.ReadOnly = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _mppsDetailsPanel
			// 
			resources.ApplyResources(this._mppsDetailsPanel, "_mppsDetailsPanel");
			this._mppsDetailsPanel.Name = "_mppsDetailsPanel";
			// 
			// PerformedProcedureComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "PerformedProcedureComponentControl";
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
