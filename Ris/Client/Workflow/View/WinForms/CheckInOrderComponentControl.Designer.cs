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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckInOrderComponentControl));
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._orderTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._checkInDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._checkInTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _orderTableView
			// 
			resources.ApplyResources(this._orderTableView, "_orderTableView");
			this._orderTableView.Name = "_orderTableView";
			this._orderTableView.ReadOnly = false;
			this._orderTableView.ShowToolbar = false;
			// 
			// _checkInDate
			// 
			resources.ApplyResources(this._checkInDate, "_checkInDate");
			this._checkInDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._checkInDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._checkInDate.Name = "_checkInDate";
			this._checkInDate.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// _checkInTime
			// 
			resources.ApplyResources(this._checkInTime, "_checkInTime");
			this._checkInTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._checkInTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._checkInTime.Name = "_checkInTime";
			this._checkInTime.ShowDate = false;
			this._checkInTime.ShowTime = true;
			this._checkInTime.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// CheckInOrderComponentControl
			// 
			this.AcceptButton = this._okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._checkInTime);
			this.Controls.Add(this._checkInDate);
			this.Controls.Add(this._orderTableView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Name = "CheckInOrderComponentControl";
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
