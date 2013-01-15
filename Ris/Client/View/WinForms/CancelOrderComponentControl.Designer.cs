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
    partial class CancelOrderComponentControl
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
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._proceduresTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(319, 280);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(238, 280);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 4;
			this._okButton.Text = "Confirm";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelReason
			// 
			this._cancelReason.DataSource = null;
			this._cancelReason.DisplayMember = "";
			this._cancelReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cancelReason.LabelText = "Cancel Reason";
			this._cancelReason.Location = new System.Drawing.Point(6, 223);
			this._cancelReason.Margin = new System.Windows.Forms.Padding(2);
			this._cancelReason.Name = "_cancelReason";
			this._cancelReason.Size = new System.Drawing.Size(386, 41);
			this._cancelReason.TabIndex = 3;
			this._cancelReason.Value = null;
			// 
			// _proceduresTableView
			// 
			this._proceduresTableView.ColumnHeaderTooltip = null;
			this._proceduresTableView.FilterTextBoxWidth = 132;
			this._proceduresTableView.Location = new System.Drawing.Point(6, 31);
			this._proceduresTableView.Margin = new System.Windows.Forms.Padding(0);
			this._proceduresTableView.Name = "_proceduresTableView";
			this._proceduresTableView.ShowToolbar = false;
			this._proceduresTableView.Size = new System.Drawing.Size(386, 147);
			this._proceduresTableView.SortButtonTooltip = null;
			this._proceduresTableView.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(281, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Canceling this order will cancel all procedures (listed here):";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 186);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(290, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "(To cancel individual procedures, use Modify Order instead.)";
			// 
			// CancelOrderComponentControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.label2);
			this.Controls.Add(this._proceduresTableView);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._cancelReason);
			this.Controls.Add(this._okButton);
			this.Name = "CancelOrderComponentControl";
			this.Size = new System.Drawing.Size(401, 314);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _cancelReason;
		private Desktop.View.WinForms.TableView _proceduresTableView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}
