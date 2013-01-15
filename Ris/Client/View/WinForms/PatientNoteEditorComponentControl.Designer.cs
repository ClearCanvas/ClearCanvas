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
    partial class PatientNoteEditorComponentControl
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
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this._comment = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._expiryDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._expiryDate, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(341, 364);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._category);
			this.flowLayoutPanel1.Controls.Add(this._description);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(335, 118);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// _category
			// 
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(2, 2);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(314, 41);
			this._category.TabIndex = 0;
			this._category.Value = null;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 47);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.Size = new System.Drawing.Size(314, 69);
			this._description.TabIndex = 1;
			this._description.TabStop = false;
			this._description.Value = null;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this._comment);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 127);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(335, 153);
			this.flowLayoutPanel2.TabIndex = 1;
			// 
			// _comment
			// 
			this._comment.LabelText = "Comment";
			this._comment.Location = new System.Drawing.Point(2, 2);
			this._comment.Margin = new System.Windows.Forms.Padding(2);
			this._comment.Name = "_comment";
			this._comment.Size = new System.Drawing.Size(314, 149);
			this._comment.TabIndex = 0;
			this._comment.Value = null;
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.AutoSize = true;
			this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel3.Controls.Add(this._cancelButton);
			this.flowLayoutPanel3.Controls.Add(this._acceptButton);
			this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 331);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel3.Size = new System.Drawing.Size(335, 30);
			this.flowLayoutPanel3.TabIndex = 3;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(257, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(176, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _expiryDate
			// 
			this._expiryDate.LabelText = "Expiry Date";
			this._expiryDate.Location = new System.Drawing.Point(2, 285);
			this._expiryDate.Margin = new System.Windows.Forms.Padding(2);
			this._expiryDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._expiryDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._expiryDate.Name = "_expiryDate";
			this._expiryDate.Nullable = true;
			this._expiryDate.Size = new System.Drawing.Size(150, 41);
			this._expiryDate.TabIndex = 2;
			this._expiryDate.Value = new System.DateTime(2008, 4, 16, 12, 51, 2, 187);
			// 
			// PatientNoteEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PatientNoteEditorComponentControl";
			this.Size = new System.Drawing.Size(341, 364);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel3.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _comment;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _expiryDate;

    }
}
