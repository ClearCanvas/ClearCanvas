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
    partial class WorklistDetailEditorComponentControl
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
			this._ownerGroupBox = new System.Windows.Forms.GroupBox();
			this._groups = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._radioGroup = new System.Windows.Forms.RadioButton();
			this._radioPersonal = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._worklistClass = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._classDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this._ownerGroupBox.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._ownerGroupBox, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(543, 328);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// _ownerGroupBox
			// 
			this._ownerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._ownerGroupBox.Controls.Add(this._groups);
			this._ownerGroupBox.Controls.Add(this._radioGroup);
			this._ownerGroupBox.Controls.Add(this._radioPersonal);
			this._ownerGroupBox.Location = new System.Drawing.Point(3, 3);
			this._ownerGroupBox.Name = "_ownerGroupBox";
			this._ownerGroupBox.Size = new System.Drawing.Size(537, 62);
			this._ownerGroupBox.TabIndex = 1;
			this._ownerGroupBox.TabStop = false;
			this._ownerGroupBox.Text = "Owner";
			// 
			// _groups
			// 
			this._groups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._groups.DataSource = null;
			this._groups.DisplayMember = "";
			this._groups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._groups.LabelText = "Staff Group";
			this._groups.Location = new System.Drawing.Point(165, 12);
			this._groups.Margin = new System.Windows.Forms.Padding(2);
			this._groups.Name = "_groups";
			this._groups.Size = new System.Drawing.Size(352, 42);
			this._groups.TabIndex = 2;
			this._groups.Value = null;
			// 
			// _radioGroup
			// 
			this._radioGroup.AutoSize = true;
			this._radioGroup.Location = new System.Drawing.Point(96, 19);
			this._radioGroup.Name = "_radioGroup";
			this._radioGroup.Size = new System.Drawing.Size(54, 17);
			this._radioGroup.TabIndex = 1;
			this._radioGroup.TabStop = true;
			this._radioGroup.Text = "Group";
			this._radioGroup.UseVisualStyleBackColor = true;
			// 
			// _radioPersonal
			// 
			this._radioPersonal.AutoSize = true;
			this._radioPersonal.Location = new System.Drawing.Point(6, 19);
			this._radioPersonal.Name = "_radioPersonal";
			this._radioPersonal.Size = new System.Drawing.Size(66, 17);
			this._radioPersonal.TabIndex = 0;
			this._radioPersonal.TabStop = true;
			this._radioPersonal.Text = "Personal";
			this._radioPersonal.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._category);
			this.panel1.Controls.Add(this._worklistClass);
			this.panel1.Controls.Add(this._name);
			this.panel1.Controls.Add(this._description);
			this.panel1.Controls.Add(this._classDescription);
			this.panel1.Controls.Add(this._okButton);
			this.panel1.Controls.Add(this._cancelButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 71);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(537, 254);
			this.panel1.TabIndex = 0;
			// 
			// _category
			// 
			this._category.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(2, 2);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(226, 41);
			this._category.TabIndex = 6;
			this._category.Value = null;
			// 
			// _worklistClass
			// 
			this._worklistClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._worklistClass.DataSource = null;
			this._worklistClass.DisplayMember = "";
			this._worklistClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._worklistClass.LabelText = "Class";
			this._worklistClass.Location = new System.Drawing.Point(276, 2);
			this._worklistClass.Margin = new System.Windows.Forms.Padding(2);
			this._worklistClass.Name = "_worklistClass";
			this._worklistClass.Size = new System.Drawing.Size(241, 41);
			this._worklistClass.TabIndex = 7;
			this._worklistClass.Value = null;
			// 
			// _name
			// 
			this._name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 92);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(515, 41);
			this._name.TabIndex = 2;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 137);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(515, 66);
			this._description.TabIndex = 3;
			this._description.Value = null;
			// 
			// _classDescription
			// 
			this._classDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._classDescription.LabelText = "Class Description";
			this._classDescription.Location = new System.Drawing.Point(2, 47);
			this._classDescription.Margin = new System.Windows.Forms.Padding(2);
			this._classDescription.Mask = "";
			this._classDescription.Name = "_classDescription";
			this._classDescription.PasswordChar = '\0';
			this._classDescription.ReadOnly = true;
			this._classDescription.Size = new System.Drawing.Size(515, 41);
			this._classDescription.TabIndex = 1;
			this._classDescription.ToolTip = null;
			this._classDescription.Value = null;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(361, 218);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 4;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(442, 218);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// WorklistDetailEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "WorklistDetailEditorComponentControl";
			this.Size = new System.Drawing.Size(543, 328);
			this.tableLayoutPanel1.ResumeLayout(false);
			this._ownerGroupBox.ResumeLayout(false);
			this._ownerGroupBox.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _classDescription;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _worklistClass;
		private System.Windows.Forms.GroupBox _ownerGroupBox;
		private System.Windows.Forms.RadioButton _radioGroup;
		private System.Windows.Forms.RadioButton _radioPersonal;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _groups;


	}
}
