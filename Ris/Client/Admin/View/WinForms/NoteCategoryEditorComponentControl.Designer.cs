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
    partial class NoteCategoryEditorComponentControl
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
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._severity = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._category = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(287, 259);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this._cancelButton);
			this.flowLayoutPanel2.Controls.Add(this._acceptButton);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 226);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel2.Size = new System.Drawing.Size(281, 30);
			this.flowLayoutPanel2.TabIndex = 1;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(203, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(122, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._severity);
			this.flowLayoutPanel1.Controls.Add(this._category);
			this.flowLayoutPanel1.Controls.Add(this._description);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(281, 217);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// _severity
			// 
			this._severity.DataSource = null;
			this._severity.DisplayMember = "";
			this._severity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._severity.LabelText = "Severity";
			this._severity.Location = new System.Drawing.Point(2, 2);
			this._severity.Margin = new System.Windows.Forms.Padding(2);
			this._severity.Name = "_severity";
			this._severity.Size = new System.Drawing.Size(150, 41);
			this._severity.TabIndex = 0;
			this._severity.Value = null;
			// 
			// _category
			// 
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(2, 47);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Mask = "";
			this._category.Name = "_category";
			this._category.PasswordChar = '\0';
			this._category.Size = new System.Drawing.Size(150, 41);
			this._category.TabIndex = 1;
			this._category.ToolTip = null;
			this._category.Value = null;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 92);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(276, 123);
			this._description.TabIndex = 2;
			this._description.Value = null;
			// 
			// NoteCategoryEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "NoteCategoryEditorComponentControl";
			this.Size = new System.Drawing.Size(287, 259);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _severity;
        private ClearCanvas.Desktop.View.WinForms.TextField _category;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
    }
}
