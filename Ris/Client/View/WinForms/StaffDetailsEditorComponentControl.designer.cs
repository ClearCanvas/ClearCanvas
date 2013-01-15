#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
    partial class StaffDetailsEditorComponentControl
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
			this._staffType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._staffId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._sex = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._title = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._userLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.SuspendLayout();
			// 
			// _staffType
			// 
			this._staffType.DataSource = null;
			this._staffType.DisplayMember = "";
			this._staffType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._staffType.LabelText = "Staff Type";
			this._staffType.Location = new System.Drawing.Point(12, 2);
			this._staffType.Margin = new System.Windows.Forms.Padding(2);
			this._staffType.Name = "_staffType";
			this._staffType.Size = new System.Drawing.Size(150, 41);
			this._staffType.TabIndex = 0;
			this._staffType.Value = null;
			// 
			// _middleName
			// 
			this._middleName.LabelText = "Middle Name";
			this._middleName.Location = new System.Drawing.Point(12, 139);
			this._middleName.Margin = new System.Windows.Forms.Padding(2);
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.PasswordChar = '\0';
			this._middleName.Size = new System.Drawing.Size(150, 41);
			this._middleName.TabIndex = 5;
			this._middleName.ToolTip = null;
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(178, 94);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(150, 41);
			this._givenName.TabIndex = 4;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(12, 94);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(150, 41);
			this._familyName.TabIndex = 3;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _staffId
			// 
			this._staffId.LabelText = "Staff ID";
			this._staffId.Location = new System.Drawing.Point(178, 2);
			this._staffId.Margin = new System.Windows.Forms.Padding(2);
			this._staffId.Mask = "";
			this._staffId.Name = "_staffId";
			this._staffId.PasswordChar = '\0';
			this._staffId.Size = new System.Drawing.Size(150, 41);
			this._staffId.TabIndex = 1;
			this._staffId.ToolTip = null;
			this._staffId.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.LabelText = "Billing # (if applicable)";
			this._billingNumber.Location = new System.Drawing.Point(178, 229);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.PasswordChar = '\0';
			this._billingNumber.Size = new System.Drawing.Size(150, 41);
			this._billingNumber.TabIndex = 9;
			this._billingNumber.ToolTip = null;
			this._billingNumber.Value = null;
			// 
			// _licenseNumber
			// 
			this._licenseNumber.LabelText = "License # (if applicable)";
			this._licenseNumber.Location = new System.Drawing.Point(12, 229);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.PasswordChar = '\0';
			this._licenseNumber.Size = new System.Drawing.Size(150, 41);
			this._licenseNumber.TabIndex = 8;
			this._licenseNumber.ToolTip = null;
			this._licenseNumber.Value = null;
			// 
			// _sex
			// 
			this._sex.DataSource = null;
			this._sex.DisplayMember = "";
			this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._sex.LabelText = "Sex";
			this._sex.Location = new System.Drawing.Point(178, 139);
			this._sex.Margin = new System.Windows.Forms.Padding(2);
			this._sex.Name = "_sex";
			this._sex.Size = new System.Drawing.Size(150, 41);
			this._sex.TabIndex = 6;
			this._sex.Value = null;
			// 
			// _title
			// 
			this._title.LabelText = "Title";
			this._title.Location = new System.Drawing.Point(12, 184);
			this._title.Margin = new System.Windows.Forms.Padding(2);
			this._title.Mask = "";
			this._title.Name = "_title";
			this._title.PasswordChar = '\0';
			this._title.Size = new System.Drawing.Size(316, 41);
			this._title.TabIndex = 7;
			this._title.ToolTip = null;
			this._title.Value = null;
			// 
			// _userLookup
			// 
			this._userLookup.LabelText = "User ID";
			this._userLookup.Location = new System.Drawing.Point(12, 47);
			this._userLookup.Margin = new System.Windows.Forms.Padding(2);
			this._userLookup.Name = "_userLookup";
			this._userLookup.Size = new System.Drawing.Size(316, 41);
			this._userLookup.TabIndex = 2;
			this._userLookup.Value = null;
			// 
			// StaffDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._userLookup);
			this.Controls.Add(this._title);
			this.Controls.Add(this._sex);
			this.Controls.Add(this._billingNumber);
			this.Controls.Add(this._licenseNumber);
			this.Controls.Add(this._staffId);
			this.Controls.Add(this._middleName);
			this.Controls.Add(this._givenName);
			this.Controls.Add(this._familyName);
			this.Controls.Add(this._staffType);
			this.Name = "StaffDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(352, 287);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _staffType;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _staffId;
        private ClearCanvas.Desktop.View.WinForms.TextField _billingNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _licenseNumber;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _sex;
		private ClearCanvas.Desktop.View.WinForms.TextField _title;
		private LookupField _userLookup;

    }
}
