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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StaffDetailsEditorComponentControl));
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
			resources.ApplyResources(this._staffType, "_staffType");
			this._staffType.Name = "_staffType";
			this._staffType.Value = null;
			// 
			// _middleName
			// 
			resources.ApplyResources(this._middleName, "_middleName");
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			resources.ApplyResources(this._givenName, "_givenName");
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			resources.ApplyResources(this._familyName, "_familyName");
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.Value = null;
			// 
			// _staffId
			// 
			resources.ApplyResources(this._staffId, "_staffId");
			this._staffId.Mask = "";
			this._staffId.Name = "_staffId";
			this._staffId.Value = null;
			// 
			// _billingNumber
			// 
			resources.ApplyResources(this._billingNumber, "_billingNumber");
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.Value = null;
			// 
			// _licenseNumber
			// 
			resources.ApplyResources(this._licenseNumber, "_licenseNumber");
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.Value = null;
			// 
			// _sex
			// 
			this._sex.DataSource = null;
			this._sex.DisplayMember = "";
			this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._sex, "_sex");
			this._sex.Name = "_sex";
			this._sex.Value = null;
			// 
			// _title
			// 
			resources.ApplyResources(this._title, "_title");
			this._title.Mask = "";
			this._title.Name = "_title";
			this._title.Value = null;
			// 
			// _userLookup
			// 
			resources.ApplyResources(this._userLookup, "_userLookup");
			this._userLookup.Name = "_userLookup";
			this._userLookup.Value = null;
			// 
			// StaffDetailsEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
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
