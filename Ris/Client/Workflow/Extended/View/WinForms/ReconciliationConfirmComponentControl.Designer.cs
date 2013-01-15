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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class ReconciliationConfirmComponentControl
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
			this._sourceTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._targetTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._cancelButton = new System.Windows.Forms.Button();
			this._continueButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _sourceTable
			// 
			this._sourceTable.Location = new System.Drawing.Point(13, 36);
			this._sourceTable.MultiSelect = false;
			this._sourceTable.Name = "_sourceTable";
			this._sourceTable.ReadOnly = false;
			this._sourceTable.ShowToolbar = false;
			this._sourceTable.Size = new System.Drawing.Size(540, 170);
			this._sourceTable.TabIndex = 1;
			// 
			// _targetTable
			// 
			this._targetTable.Location = new System.Drawing.Point(13, 243);
			this._targetTable.MultiSelect = false;
			this._targetTable.Name = "_targetTable";
			this._targetTable.ReadOnly = false;
			this._targetTable.ShowToolbar = false;
			this._targetTable.Size = new System.Drawing.Size(540, 174);
			this._targetTable.TabIndex = 3;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(478, 430);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _continueButton
			// 
			this._continueButton.Location = new System.Drawing.Point(400, 430);
			this._continueButton.Margin = new System.Windows.Forms.Padding(2);
			this._continueButton.Name = "_continueButton";
			this._continueButton.Size = new System.Drawing.Size(75, 23);
			this._continueButton.TabIndex = 4;
			this._continueButton.Text = "Continue";
			this._continueButton.UseVisualStyleBackColor = true;
			this._continueButton.Click += new System.EventHandler(this._continueButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 19);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "The following profiles";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 226);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "will be linked with these profiles";
			// 
			// ReconciliationConfirmComponentControl
			// 
			this.AcceptButton = this._continueButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._continueButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._targetTable);
			this.Controls.Add(this._sourceTable);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ReconciliationConfirmComponentControl";
			this.Size = new System.Drawing.Size(571, 459);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _sourceTable;
        private ClearCanvas.Desktop.View.WinForms.TableView _targetTable;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _continueButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
