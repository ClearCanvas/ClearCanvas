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
    partial class LoginForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this._cancelButton = new System.Windows.Forms.Button();
			this._loginButton = new System.Windows.Forms.Button();
			this._userName = new System.Windows.Forms.TextBox();
			this._password = new System.Windows.Forms.TextBox();
			this._facility = new System.Windows.Forms.ComboBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this._manifest = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _loginButton
			// 
			resources.ApplyResources(this._loginButton, "_loginButton");
			this._loginButton.Name = "_loginButton";
			this._loginButton.UseVisualStyleBackColor = true;
			this._loginButton.Click += new System.EventHandler(this._loginButton_Click);
			// 
			// _userName
			// 
			resources.ApplyResources(this._userName, "_userName");
			this._userName.Name = "_userName";
			this._userName.TextChanged += new System.EventHandler(this._userName_TextChanged);
			// 
			// _password
			// 
			resources.ApplyResources(this._password, "_password");
			this._password.Name = "_password";
			this._password.TextChanged += new System.EventHandler(this._password_TextChanged);
			// 
			// _facility
			// 
			this._facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._facility.FormattingEnabled = true;
			resources.ApplyResources(this._facility, "_facility");
			this._facility.Name = "_facility";
			this._facility.SelectedValueChanged += new System.EventHandler(this._facility_SelectedValueChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.BackgroundImage = global::ClearCanvas.Ris.Client.View.WinForms.SR.UserText;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox2.BackgroundImage = global::ClearCanvas.Ris.Client.View.WinForms.SR.PasswordText;
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
			this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
			// 
			// pictureBox3
			// 
			this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox3.BackgroundImage = global::ClearCanvas.Ris.Client.View.WinForms.SR.FacilityText;
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			this.pictureBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
			this.pictureBox3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
			// 
			// _manifest
			// 
			resources.ApplyResources(this._manifest, "_manifest");
			this._manifest.BackColor = System.Drawing.Color.Transparent;
			this._manifest.ForeColor = System.Drawing.Color.Firebrick;
			this._manifest.Name = "_manifest";
			// 
			// LoginForm
			// 
			this.AcceptButton = this._loginButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::ClearCanvas.Ris.Client.View.WinForms.SR.Splash;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._manifest);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this._facility);
			this.Controls.Add(this._password);
			this.Controls.Add(this._userName);
			this.Controls.Add(this._loginButton);
			this.Controls.Add(this._cancelButton);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LoginForm";
			this.Load += new System.EventHandler(this.LoginForm_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _loginButton;
        private System.Windows.Forms.TextBox _userName;
        private System.Windows.Forms.TextBox _password;
		private System.Windows.Forms.ComboBox _facility;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label _manifest;
    }
}
