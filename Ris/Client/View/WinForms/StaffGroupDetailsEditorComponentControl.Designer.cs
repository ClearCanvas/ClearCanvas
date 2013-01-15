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
    partial class StaffGroupDetailsEditorComponentControl
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
			this._electiveCheckbox = new System.Windows.Forms.CheckBox();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.SuspendLayout();
			// 
			// _electiveCheckbox
			// 
			this._electiveCheckbox.AutoSize = true;
			this._electiveCheckbox.Location = new System.Drawing.Point(8, 114);
			this._electiveCheckbox.Name = "_electiveCheckbox";
			this._electiveCheckbox.Size = new System.Drawing.Size(64, 17);
			this._electiveCheckbox.TabIndex = 2;
			this._electiveCheckbox.Text = "Elective";
			this._electiveCheckbox.UseVisualStyleBackColor = true;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 57);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.PasswordChar = '\0';
			this._description.Size = new System.Drawing.Size(424, 41);
			this._description.TabIndex = 1;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 2);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(424, 41);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// StaffGroupDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._electiveCheckbox);
			this.Controls.Add(this._description);
			this.Controls.Add(this._name);
			this.Name = "StaffGroupDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(434, 144);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox _electiveCheckbox;
		private ClearCanvas.Desktop.View.WinForms.TextField _description;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
    }
}
