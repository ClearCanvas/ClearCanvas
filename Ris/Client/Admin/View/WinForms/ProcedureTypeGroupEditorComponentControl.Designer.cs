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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ProcedureTypeGroupEditorComponentControl
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._procedureTypesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._includeDeactivatedItems = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(660, 566);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 534);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(654, 29);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(576, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(495, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._includeDeactivatedItems);
			this.groupBox1.Controls.Add(this._procedureTypesSelector);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 127);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(654, 401);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Procedure Types";
			// 
			// _procedureTypesSelector
			// 
			this._procedureTypesSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._procedureTypesSelector.AutoSize = true;
			this._procedureTypesSelector.AvailableItemsTable = null;
			this._procedureTypesSelector.Location = new System.Drawing.Point(3, 35);
			this._procedureTypesSelector.Name = "_procedureTypesSelector";
			this._procedureTypesSelector.ReadOnly = false;
			this._procedureTypesSelector.SelectedItemsTable = null;
			this._procedureTypesSelector.Size = new System.Drawing.Size(648, 363);
			this._procedureTypesSelector.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this._name, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._category, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this._description, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(654, 118);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _name
			// 
			this._name.AutoSize = true;
			this._name.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._name.Dock = System.Windows.Forms.DockStyle.Fill;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 2);
			this._name.Margin = new System.Windows.Forms.Padding(2, 2, 20, 2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(305, 41);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _category
			// 
			this._category.AutoSize = true;
			this._category.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.Dock = System.Windows.Forms.DockStyle.Fill;
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._category.LabelText = "Class";
			this._category.Location = new System.Drawing.Point(329, 2);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(323, 41);
			this._category.TabIndex = 1;
			this._category.Value = null;
			// 
			// _description
			// 
			this._description.AutoSize = true;
			this._description.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._description, 2);
			this._description.Dock = System.Windows.Forms.DockStyle.Fill;
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 47);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(650, 69);
			this._description.TabIndex = 2;
			this._description.Value = null;
			// 
			// _includeDeactivatedItems
			// 
			this._includeDeactivatedItems.AutoSize = true;
			this._includeDeactivatedItems.Location = new System.Drawing.Point(6, 19);
			this._includeDeactivatedItems.Name = "_includeDeactivatedItems";
			this._includeDeactivatedItems.Size = new System.Drawing.Size(180, 17);
			this._includeDeactivatedItems.TabIndex = 1;
			this._includeDeactivatedItems.Text = "Include inactive procedure types";
			this._includeDeactivatedItems.UseVisualStyleBackColor = true;
			// 
			// ProcedureTypeGroupEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProcedureTypeGroupEditorComponentControl";
			this.Size = new System.Drawing.Size(660, 566);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _procedureTypesSelector;
		private System.Windows.Forms.CheckBox _includeDeactivatedItems;
    }
}
