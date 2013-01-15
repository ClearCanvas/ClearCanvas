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
			this._okButton.Location = new System.Drawing.Point(410, 2);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 0;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _firstName
			// 
			this._firstName.LabelText = "First Name";
			this._firstName.Location = new System.Drawing.Point(149, 5);
			this._firstName.Margin = new System.Windows.Forms.Padding(2);
			this._firstName.Mask = "";
			this._firstName.Name = "_firstName";
			this._firstName.PasswordChar = '\0';
			this._firstName.Size = new System.Drawing.Size(150, 41);
			this._firstName.TabIndex = 1;
			this._firstName.ToolTip = null;
			this._firstName.Value = null;
			this._firstName.Leave += new System.EventHandler(this._field_Leave);
			this._firstName.Enter += new System.EventHandler(this._field_Enter);
			// 
			// _buttonsPanel
			// 
			this._buttonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._buttonsPanel.Controls.Add(this._cancelButton);
			this._buttonsPanel.Controls.Add(this._okButton);
			this._buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._buttonsPanel.Location = new System.Drawing.Point(2, 379);
			this._buttonsPanel.Margin = new System.Windows.Forms.Padding(2);
			this._buttonsPanel.Name = "_buttonsPanel";
			this._buttonsPanel.Size = new System.Drawing.Size(566, 30);
			this._buttonsPanel.TabIndex = 2;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(489, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _lastName
			// 
			this._lastName.LabelText = "Last Name";
			this._lastName.Location = new System.Drawing.Point(0, 5);
			this._lastName.Margin = new System.Windows.Forms.Padding(2);
			this._lastName.Mask = "";
			this._lastName.Name = "_lastName";
			this._lastName.PasswordChar = '\0';
			this._lastName.Size = new System.Drawing.Size(152, 41);
			this._lastName.TabIndex = 0;
			this._lastName.ToolTip = null;
			this._lastName.Value = null;
			this._lastName.Leave += new System.EventHandler(this._field_Leave);
			this._lastName.Enter += new System.EventHandler(this._field_Enter);
			// 
			// _staffTableView
			// 
			this._staffTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._staffTableView.Location = new System.Drawing.Point(4, 64);
			this._staffTableView.Margin = new System.Windows.Forms.Padding(4);
			this._staffTableView.Name = "_staffTableView";
			this._staffTableView.ReadOnly = false;
			this._staffTableView.Size = new System.Drawing.Size(562, 309);
			this._staffTableView.TabIndex = 1;
			this._staffTableView.ItemDoubleClicked += new System.EventHandler(this._staffTableView_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._staffTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._buttonsPanel, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 411);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Controls.Add(this._lastName);
			this.panel1.Controls.Add(this._firstName);
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(563, 54);
			this.panel1.TabIndex = 0;
			// 
			// _clearButton
			// 
			this._clearButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Location = new System.Drawing.Point(332, 18);
			this._clearButton.Margin = new System.Windows.Forms.Padding(0);
			this._clearButton.Name = "_clearButton";
			this._clearButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._clearButton.Size = new System.Drawing.Size(30, 30);
			this._clearButton.TabIndex = 5;
			this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip1.SetToolTip(this._clearButton, "Clear search query");
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _searchButton
			// 
			this._searchButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Location = new System.Drawing.Point(301, 18);
			this._searchButton.Margin = new System.Windows.Forms.Padding(0);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(30, 30);
			this._searchButton.TabIndex = 4;
			this.toolTip1.SetToolTip(this._searchButton, "Search");
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// StaffSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StaffSummaryComponentControl";
			this.Size = new System.Drawing.Size(570, 411);
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
