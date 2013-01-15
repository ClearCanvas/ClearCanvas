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
    partial class EnumerationSummaryComponentControl
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
			this._enumerationName = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._enumerationValuesTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._enumerationClass = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _enumerationName
			// 
			this._enumerationName.DataSource = null;
			this._enumerationName.DisplayMember = "";
			this._enumerationName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._enumerationName.LabelText = "Enumeration";
			this._enumerationName.Location = new System.Drawing.Point(2, 2);
			this._enumerationName.Margin = new System.Windows.Forms.Padding(2);
			this._enumerationName.Name = "_enumerationName";
			this._enumerationName.Size = new System.Drawing.Size(272, 41);
			this._enumerationName.TabIndex = 0;
			this._enumerationName.Value = null;
			// 
			// _enumerationValuesTableView
			// 
			this._enumerationValuesTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._enumerationValuesTableView.Location = new System.Drawing.Point(3, 93);
			this._enumerationValuesTableView.Name = "_enumerationValuesTableView";
			this._enumerationValuesTableView.ReadOnly = false;
			this._enumerationValuesTableView.Size = new System.Drawing.Size(623, 382);
			this._enumerationValuesTableView.TabIndex = 2;
			this._enumerationValuesTableView.ItemDoubleClicked += new System.EventHandler(this._enumerationValuesTableView_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._enumerationName, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._enumerationValuesTableView, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._enumerationClass, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(629, 476);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _enumerationClass
			// 
			this._enumerationClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._enumerationClass.LabelText = "Class Name";
			this._enumerationClass.Location = new System.Drawing.Point(2, 47);
			this._enumerationClass.Margin = new System.Windows.Forms.Padding(2);
			this._enumerationClass.Mask = "";
			this._enumerationClass.Name = "_enumerationClass";
			this._enumerationClass.PasswordChar = '\0';
			this._enumerationClass.ReadOnly = true;
			this._enumerationClass.Size = new System.Drawing.Size(625, 41);
			this._enumerationClass.TabIndex = 1;
			this._enumerationClass.ToolTip = null;
			this._enumerationClass.Value = null;
			// 
			// EnumerationSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "EnumerationSummaryComponentControl";
			this.Size = new System.Drawing.Size(629, 476);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _enumerationName;
        private ClearCanvas.Desktop.View.WinForms.TableView _enumerationValuesTableView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TextField _enumerationClass;
    }
}
