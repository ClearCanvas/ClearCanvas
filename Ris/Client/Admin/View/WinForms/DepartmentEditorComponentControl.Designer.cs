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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class DepartmentEditorComponentControl
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
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._id = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._facility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(11, 54);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.PasswordChar = '\0';
			this._description.Size = new System.Drawing.Size(434, 41);
			this._description.TabIndex = 2;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(165, 9);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(280, 41);
			this._name.TabIndex = 1;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _id
			// 
			this._id.LabelText = "ID";
			this._id.Location = new System.Drawing.Point(11, 9);
			this._id.Margin = new System.Windows.Forms.Padding(2);
			this._id.Mask = "";
			this._id.Name = "_id";
			this._id.PasswordChar = '\0';
			this._id.Size = new System.Drawing.Size(134, 41);
			this._id.TabIndex = 0;
			this._id.ToolTip = null;
			this._id.Value = null;
			// 
			// _facility
			// 
			this._facility.DataSource = null;
			this._facility.DisplayMember = "";
			this._facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._facility.LabelText = "Facility";
			this._facility.Location = new System.Drawing.Point(11, 100);
			this._facility.Margin = new System.Windows.Forms.Padding(2);
			this._facility.Name = "_facility";
			this._facility.Size = new System.Drawing.Size(257, 41);
			this._facility.TabIndex = 3;
			this._facility.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(368, 158);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(287, 158);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 4;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// DepartmentEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._description);
			this.Controls.Add(this._name);
			this.Controls.Add(this._id);
			this.Controls.Add(this._facility);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "DepartmentEditorComponentControl";
			this.Size = new System.Drawing.Size(474, 184);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _description;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextField _id;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _facility;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;

	}
}
