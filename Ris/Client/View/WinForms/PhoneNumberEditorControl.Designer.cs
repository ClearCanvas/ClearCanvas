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
    partial class PhoneNumberEditorControl
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
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._phoneType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._countryCode = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._number = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._extension = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._areaCode = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.SuspendLayout();
			// 
			// _phoneType
			// 
			this._phoneType.DataSource = null;
			this._phoneType.DisplayMember = "";
			this._phoneType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._phoneType.LabelText = "Type";
			this._phoneType.Location = new System.Drawing.Point(3, 5);
			this._phoneType.Margin = new System.Windows.Forms.Padding(2);
			this._phoneType.Name = "_phoneType";
			this._phoneType.Size = new System.Drawing.Size(95, 41);
			this._phoneType.TabIndex = 0;
			this._phoneType.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(244, 144);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 8;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(165, 144);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 7;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _countryCode
			// 
			this._countryCode.LabelText = "Country Code";
			this._countryCode.Location = new System.Drawing.Point(113, 5);
			this._countryCode.Margin = new System.Windows.Forms.Padding(2);
			this._countryCode.Mask = "";
			this._countryCode.Name = "_countryCode";
			this._countryCode.PasswordChar = '\0';
			this._countryCode.Size = new System.Drawing.Size(95, 43);
			this._countryCode.TabIndex = 1;
			this._countryCode.ToolTip = null;
			this._countryCode.Value = null;
			// 
			// _validFrom
			// 
			this._validFrom.LabelText = "Valid From";
			this._validFrom.Location = new System.Drawing.Point(3, 97);
			this._validFrom.Margin = new System.Windows.Forms.Padding(2);
			this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validFrom.Name = "_validFrom";
			this._validFrom.Nullable = true;
			this._validFrom.Size = new System.Drawing.Size(152, 41);
			this._validFrom.TabIndex = 5;
			this._validFrom.Value = null;
			// 
			// _number
			// 
			this._number.LabelText = "Number";
			this._number.Location = new System.Drawing.Point(113, 48);
			this._number.Margin = new System.Windows.Forms.Padding(2);
			this._number.Mask = "";
			this._number.Name = "_number";
			this._number.PasswordChar = '\0';
			this._number.Size = new System.Drawing.Size(95, 41);
			this._number.TabIndex = 3;
			this._number.ToolTip = null;
			this._number.Value = null;
			// 
			// _extension
			// 
			this._extension.LabelText = "Extension";
			this._extension.Location = new System.Drawing.Point(223, 48);
			this._extension.Margin = new System.Windows.Forms.Padding(2);
			this._extension.Mask = "";
			this._extension.Name = "_extension";
			this._extension.PasswordChar = '\0';
			this._extension.Size = new System.Drawing.Size(96, 43);
			this._extension.TabIndex = 4;
			this._extension.Value = null;
			// 
			// _areaCode
			// 
			this._areaCode.LabelText = "Area Code";
			this._areaCode.Location = new System.Drawing.Point(3, 48);
			this._areaCode.Margin = new System.Windows.Forms.Padding(2);
			this._areaCode.Mask = "";
			this._areaCode.Name = "_areaCode";
			this._areaCode.PasswordChar = '\0';
			this._areaCode.Size = new System.Drawing.Size(95, 43);
			this._areaCode.TabIndex = 2;
			this._areaCode.ToolTip = null;
			this._areaCode.Value = null;
			// 
			// _validUntil
			// 
			this._validUntil.LabelText = "Valid Until";
			this._validUntil.Location = new System.Drawing.Point(174, 95);
			this._validUntil.Margin = new System.Windows.Forms.Padding(2);
			this._validUntil.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validUntil.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validUntil.Name = "_validUntil";
			this._validUntil.Nullable = true;
			this._validUntil.Size = new System.Drawing.Size(145, 41);
			this._validUntil.TabIndex = 6;
			this._validUntil.Value = null;
			// 
			// PhoneNumberEditorControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._phoneType);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._countryCode);
			this.Controls.Add(this._validFrom);
			this.Controls.Add(this._number);
			this.Controls.Add(this._extension);
			this.Controls.Add(this._areaCode);
			this.Controls.Add(this._validUntil);
			this.Name = "PhoneNumberEditorControl";
			this.Size = new System.Drawing.Size(339, 178);
			this.Load += new System.EventHandler(this.PhoneNumberEditorControl_Load);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ToolTip toolTip1;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _phoneType;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _countryCode;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
		private ClearCanvas.Desktop.View.WinForms.TextField _number;
		private ClearCanvas.Desktop.View.WinForms.TextField _extension;
		private ClearCanvas.Desktop.View.WinForms.TextField _areaCode;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
    }
}
