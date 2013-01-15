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
    partial class EmailAddressEditorComponentControl
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
			this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._address = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _validFrom
			// 
			this._validFrom.LabelText = "Valid From";
			this._validFrom.Location = new System.Drawing.Point(3, 49);
			this._validFrom.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
			this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validFrom.Name = "_validFrom";
			this._validFrom.Nullable = true;
			this._validFrom.Size = new System.Drawing.Size(149, 44);
			this._validFrom.TabIndex = 1;
			this._validFrom.Value = null;
			// 
			// _validUntil
			// 
			this._validUntil.LabelText = "Valid Until";
			this._validUntil.Location = new System.Drawing.Point(175, 49);
			this._validUntil.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
			this._validUntil.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validUntil.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validUntil.Name = "_validUntil";
			this._validUntil.Nullable = true;
			this._validUntil.Size = new System.Drawing.Size(149, 44);
			this._validUntil.TabIndex = 2;
			this._validUntil.Value = null;
			// 
			// _address
			// 
			this._address.LabelText = "Email Address";
			this._address.Location = new System.Drawing.Point(3, 5);
			this._address.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
			this._address.Mask = "";
			this._address.Name = "_address";
			this._address.PasswordChar = '\0';
			this._address.Size = new System.Drawing.Size(321, 41);
			this._address.TabIndex = 0;
			this._address.ToolTip = null;
			this._address.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(249, 105);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(168, 105);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 3;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// EmailAddressEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._validFrom);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._validUntil);
			this.Controls.Add(this._address);
			this.Name = "EmailAddressEditorComponentControl";
			this.Size = new System.Drawing.Size(343, 142);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
		private ClearCanvas.Desktop.View.WinForms.TextField _address;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
    }
}
