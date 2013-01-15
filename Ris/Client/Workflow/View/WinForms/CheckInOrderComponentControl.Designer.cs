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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class CheckInOrderComponentControl
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
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._orderTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._checkInDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._checkInTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(325, 254);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "Check-In";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(401, 254);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _orderTableView
			// 
			this._orderTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._orderTableView.Location = new System.Drawing.Point(4, 4);
			this._orderTableView.Name = "_orderTableView";
			this._orderTableView.ReadOnly = false;
			this._orderTableView.ShowToolbar = false;
			this._orderTableView.Size = new System.Drawing.Size(477, 231);
			this._orderTableView.TabIndex = 3;
			// 
			// _checkInDate
			// 
			this._checkInDate.LabelText = "Check-in Date";
			this._checkInDate.Location = new System.Drawing.Point(4, 237);
			this._checkInDate.Margin = new System.Windows.Forms.Padding(2);
			this._checkInDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._checkInDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._checkInDate.Name = "_checkInDate";
			this._checkInDate.Size = new System.Drawing.Size(150, 41);
			this._checkInDate.TabIndex = 4;
			this._checkInDate.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// _checkInTime
			// 
			this._checkInTime.LabelText = "Check-in Time";
			this._checkInTime.Location = new System.Drawing.Point(158, 236);
			this._checkInTime.Margin = new System.Windows.Forms.Padding(2);
			this._checkInTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._checkInTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._checkInTime.Name = "_checkInTime";
			this._checkInTime.ShowDate = false;
			this._checkInTime.ShowTime = true;
			this._checkInTime.Size = new System.Drawing.Size(150, 41);
			this._checkInTime.TabIndex = 5;
			this._checkInTime.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// CheckInOrderComponentControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._checkInTime);
			this.Controls.Add(this._checkInDate);
			this.Controls.Add(this._orderTableView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Name = "CheckInOrderComponentControl";
			this.Size = new System.Drawing.Size(484, 280);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TableView _orderTableView;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _checkInDate;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _checkInTime;
    }
}
