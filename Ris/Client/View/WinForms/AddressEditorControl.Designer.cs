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
    partial class AddressEditorControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressEditorControl));
			this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._country = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._street = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._city = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._postalCode = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._province = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._unit = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.SuspendLayout();
			// 
			// _type
			// 
			this._type.DataSource = null;
			this._type.DisplayMember = "";
			this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._type, "_type");
			this._type.Name = "_type";
			this._type.Value = null;
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _country
			// 
			this._country.DataSource = null;
			this._country.DisplayMember = "";
			this._country.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._country, "_country");
			this._country.Name = "_country";
			this._country.Value = null;
			// 
			// _street
			// 
			resources.ApplyResources(this._street, "_street");
			this._street.Mask = "";
			this._street.Name = "_street";
			this._street.Value = null;
			// 
			// _validUntil
			// 
			resources.ApplyResources(this._validUntil, "_validUntil");
			this._validUntil.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validUntil.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validUntil.Name = "_validUntil";
			this._validUntil.Nullable = true;
			this._validUntil.Value = new System.DateTime(2006, 7, 26, 16, 37, 6, 765);
			// 
			// _city
			// 
			resources.ApplyResources(this._city, "_city");
			this._city.Mask = "";
			this._city.Name = "_city";
			this._city.Value = null;
			// 
			// _validFrom
			// 
			resources.ApplyResources(this._validFrom, "_validFrom");
			this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._validFrom.Name = "_validFrom";
			this._validFrom.Nullable = true;
			this._validFrom.Value = new System.DateTime(2006, 7, 26, 16, 37, 8, 953);
			// 
			// _postalCode
			// 
			resources.ApplyResources(this._postalCode, "_postalCode");
			this._postalCode.Mask = "";
			this._postalCode.Name = "_postalCode";
			this._postalCode.Value = null;
			// 
			// _province
			// 
			this._province.DataSource = null;
			this._province.DisplayMember = "";
			this._province.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._province, "_province");
			this._province.Name = "_province";
			this._province.Value = null;
			// 
			// _unit
			// 
			resources.ApplyResources(this._unit, "_unit");
			this._unit.Mask = "";
			this._unit.Name = "_unit";
			this._unit.Value = null;
			// 
			// AddressEditorControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._type);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._country);
			this.Controls.Add(this._street);
			this.Controls.Add(this._validUntil);
			this.Controls.Add(this._city);
			this.Controls.Add(this._validFrom);
			this.Controls.Add(this._postalCode);
			this.Controls.Add(this._province);
			this.Controls.Add(this._unit);
			this.Name = "AddressEditorControl";
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _country;
		private ClearCanvas.Desktop.View.WinForms.TextField _street;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
		private ClearCanvas.Desktop.View.WinForms.TextField _city;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
		private ClearCanvas.Desktop.View.WinForms.TextField _postalCode;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _province;
		private ClearCanvas.Desktop.View.WinForms.TextField _unit;

	}
}
