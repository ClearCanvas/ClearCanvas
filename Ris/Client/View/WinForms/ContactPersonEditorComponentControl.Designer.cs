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
    partial class ContactPersonEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactPersonEditorComponentControl));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._businessPhone = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._homePhone = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._address = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._relationship = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this._businessPhone, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._homePhone, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._address, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._name, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._type, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._relationship, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
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
			// _businessPhone
			// 
			resources.ApplyResources(this._businessPhone, "_businessPhone");
			this._businessPhone.Mask = "";
			this._businessPhone.Name = "_businessPhone";
			this._businessPhone.Value = null;
			// 
			// _homePhone
			// 
			resources.ApplyResources(this._homePhone, "_homePhone");
			this._homePhone.Mask = "";
			this._homePhone.Name = "_homePhone";
			this._homePhone.Value = null;
			// 
			// _address
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._address, 2);
			resources.ApplyResources(this._address, "_address");
			this._address.Mask = "";
			this._address.Name = "_address";
			this._address.Value = null;
			// 
			// _name
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._name, 2);
			resources.ApplyResources(this._name, "_name");
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Value = null;
			// 
			// _type
			// 
			this._type.DataSource = null;
			this._type.DisplayMember = "";
			resources.ApplyResources(this._type, "_type");
			this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._type.Name = "_type";
			this._type.Value = null;
			// 
			// _relationship
			// 
			this._relationship.DataSource = null;
			this._relationship.DisplayMember = "";
			resources.ApplyResources(this._relationship, "_relationship");
			this._relationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._relationship.Name = "_relationship";
			this._relationship.Value = null;
			// 
			// ContactPersonEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ContactPersonEditorComponentControl";
			this.Load += new System.EventHandler(this.ContactPersonEditorComponentControl_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _businessPhone;
        private ClearCanvas.Desktop.View.WinForms.TextField _homePhone;
        private ClearCanvas.Desktop.View.WinForms.TextField _address;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _relationship;
    }
}
