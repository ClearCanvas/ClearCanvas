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
    partial class ExternalPractitionerMergePropertiesComponentControl
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
			this._name = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._extendedProperties = new ClearCanvas.Ris.Client.View.WinForms.ExtendedPropertyChoicesTable();
			this._instruction = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.DataSource = null;
			this._name.DisplayMember = "";
			this._name.Dock = System.Windows.Forms.DockStyle.Fill;
			this._name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 32);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(221, 46);
			this._name.TabIndex = 0;
			this._name.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.DataSource = null;
			this._billingNumber.DisplayMember = "";
			this._billingNumber.Dock = System.Windows.Forms.DockStyle.Fill;
			this._billingNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._billingNumber.LabelText = "Billing #";
			this._billingNumber.Location = new System.Drawing.Point(339, 32);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.Size = new System.Drawing.Size(109, 46);
			this._billingNumber.TabIndex = 2;
			this._billingNumber.Value = null;
			// 
			// _licenseNumber
			// 
			this._licenseNumber.DataSource = null;
			this._licenseNumber.DisplayMember = "";
			this._licenseNumber.Dock = System.Windows.Forms.DockStyle.Fill;
			this._licenseNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._licenseNumber.LabelText = "License #";
			this._licenseNumber.Location = new System.Drawing.Point(227, 32);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.Size = new System.Drawing.Size(108, 46);
			this._licenseNumber.TabIndex = 1;
			this._licenseNumber.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.Controls.Add(this._licenseNumber, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._instruction, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._billingNumber, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this._name, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(450, 300);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// _extendedProperties
			// 
			this._extendedProperties.AutoScroll = true;
			this._extendedProperties.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this._extendedProperties.ColumnCount = 2;
			this._extendedProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this._extendedProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._extendedProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this._extendedProperties.Location = new System.Drawing.Point(3, 16);
			this._extendedProperties.Name = "_extendedProperties";
			this._extendedProperties.RowCount = 1;
			this._extendedProperties.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._extendedProperties.Size = new System.Drawing.Size(438, 195);
			this._extendedProperties.TabIndex = 3;
			// 
			// _instruction
			// 
			this._instruction.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._instruction, 3);
			this._instruction.Dock = System.Windows.Forms.DockStyle.Fill;
			this._instruction.Location = new System.Drawing.Point(3, 0);
			this._instruction.Name = "_instruction";
			this._instruction.Size = new System.Drawing.Size(444, 30);
			this._instruction.TabIndex = 4;
			this._instruction.Text = "Click on the dropdown to resolve conflicted properties.";
			this._instruction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
			this.groupBox1.Controls.Add(this._extendedProperties);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 83);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(444, 214);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Extended Properties";
			// 
			// ExternalPractitionerMergePropertiesComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerMergePropertiesComponentControl";
			this.Size = new System.Drawing.Size(450, 300);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _name;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _billingNumber;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _licenseNumber;
		private ExtendedPropertyChoicesTable _extendedProperties;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _instruction;
		private System.Windows.Forms.GroupBox groupBox1;
    }
}
