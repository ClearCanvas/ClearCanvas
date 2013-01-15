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
    partial class DateTimeEntryComponentControl
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
			this._time = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._date = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _time
			// 
			this._time.LabelText = "Time";
			this._time.Location = new System.Drawing.Point(172, 26);
			this._time.Margin = new System.Windows.Forms.Padding(2);
			this._time.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._time.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._time.Name = "_time";
			this._time.ShowDate = false;
			this._time.ShowTime = true;
			this._time.Size = new System.Drawing.Size(150, 41);
			this._time.TabIndex = 7;
			this._time.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// _date
			// 
			this._date.LabelText = "Date";
			this._date.Location = new System.Drawing.Point(18, 26);
			this._date.Margin = new System.Windows.Forms.Padding(2);
			this._date.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._date.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._date.Name = "_date";
			this._date.Size = new System.Drawing.Size(150, 41);
			this._date.TabIndex = 6;
			this._date.Value = new System.DateTime(2008, 8, 5, 10, 14, 18, 62);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(172, 84);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 8;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(248, 84);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 9;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// DateTimeEntryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._time);
			this.Controls.Add(this._date);
			this.Name = "DateTimeEntryComponentControl";
			this.Size = new System.Drawing.Size(342, 118);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.DateTimeField _time;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _date;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
    }
}
