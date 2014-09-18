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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoneNumberEditorControl));
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
			resources.ApplyResources(this._phoneType, "_phoneType");
			this._phoneType.Name = "_phoneType";
			this._phoneType.Value = null;
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _countryCode
			// 
			resources.ApplyResources(this._countryCode, "_countryCode");
			this._countryCode.Mask = "";
			this._countryCode.Name = "_countryCode";
			this._countryCode.Value = null;
			// 
			// _validFrom
			// 
			resources.ApplyResources(this._validFrom, "_validFrom");
			this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validFrom.Name = "_validFrom";
			this._validFrom.Nullable = true;
			this._validFrom.Value = null;
			// 
			// _number
			// 
			resources.ApplyResources(this._number, "_number");
			this._number.Mask = "";
			this._number.Name = "_number";
			this._number.Value = null;
			// 
			// _extension
			// 
			resources.ApplyResources(this._extension, "_extension");
			this._extension.Mask = "";
			this._extension.Name = "_extension";
			this._extension.Value = null;
			// 
			// _areaCode
			// 
			resources.ApplyResources(this._areaCode, "_areaCode");
			this._areaCode.Mask = "";
			this._areaCode.Name = "_areaCode";
			this._areaCode.Value = null;
			// 
			// _validUntil
			// 
			resources.ApplyResources(this._validUntil, "_validUntil");
			this._validUntil.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validUntil.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validUntil.Name = "_validUntil";
			this._validUntil.Nullable = true;
			this._validUntil.Value = null;
			// 
			// PhoneNumberEditorControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
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
