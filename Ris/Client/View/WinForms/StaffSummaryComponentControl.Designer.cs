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
    partial class StaffSummaryComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StaffSummaryComponentControl));
			this._okButton = new System.Windows.Forms.Button();
			this._firstName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._lastName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._staffTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._clearButton = new System.Windows.Forms.Button();
			this._searchButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._buttonsPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _firstName
			// 
			resources.ApplyResources(this._firstName, "_firstName");
			this._firstName.Mask = "";
			this._firstName.Name = "_firstName";
			this._firstName.Value = null;
			this._firstName.Enter += new System.EventHandler(this._field_Enter);
			this._firstName.Leave += new System.EventHandler(this._field_Leave);
			// 
			// _buttonsPanel
			// 
			resources.ApplyResources(this._buttonsPanel, "_buttonsPanel");
			this._buttonsPanel.Controls.Add(this._cancelButton);
			this._buttonsPanel.Controls.Add(this._okButton);
			this._buttonsPanel.Name = "_buttonsPanel";
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _lastName
			// 
			resources.ApplyResources(this._lastName, "_lastName");
			this._lastName.Mask = "";
			this._lastName.Name = "_lastName";
			this._lastName.Value = null;
			this._lastName.Enter += new System.EventHandler(this._field_Enter);
			this._lastName.Leave += new System.EventHandler(this._field_Leave);
			// 
			// _staffTableView
			// 
			resources.ApplyResources(this._staffTableView, "_staffTableView");
			this._staffTableView.Name = "_staffTableView";
			this._staffTableView.ReadOnly = false;
			this._staffTableView.ItemDoubleClicked += new System.EventHandler(this._staffTableView_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._staffTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._buttonsPanel, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Controls.Add(this._lastName);
			this.panel1.Controls.Add(this._firstName);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _clearButton
			// 
			resources.ApplyResources(this._clearButton, "_clearButton");
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Name = "_clearButton";
			this.toolTip1.SetToolTip(this._clearButton, resources.GetString("_clearButton.ToolTip"));
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Name = "_searchButton";
			this.toolTip1.SetToolTip(this._searchButton, resources.GetString("_searchButton.ToolTip"));
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// StaffSummaryComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StaffSummaryComponentControl";
			this._buttonsPanel.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _firstName;
        private System.Windows.Forms.FlowLayoutPanel _buttonsPanel;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _lastName;
        private ClearCanvas.Desktop.View.WinForms.TableView _staffTableView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button _searchButton;

    }
}
