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
    partial class ChangePasswordForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._password = new System.Windows.Forms.TextBox();
			this._userName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._newPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._newPasswordConfirm = new System.Windows.Forms.TextBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(173, 225);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 9;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Enabled = false;
			this._okButton.Location = new System.Drawing.Point(94, 225);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 8;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 57);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Password";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "User";
			// 
			// _password
			// 
			this._password.Location = new System.Drawing.Point(12, 76);
			this._password.Name = "_password";
			this._password.PasswordChar = '*';
			this._password.Size = new System.Drawing.Size(234, 20);
			this._password.TabIndex = 3;
			this._password.TextChanged += new System.EventHandler(this._password_TextChanged);
			// 
			// _userName
			// 
			this._userName.Location = new System.Drawing.Point(12, 26);
			this._userName.Name = "_userName";
			this._userName.ReadOnly = true;
			this._userName.Size = new System.Drawing.Size(234, 20);
			this._userName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 110);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "New Password";
			// 
			// _newPassword
			// 
			this._newPassword.Location = new System.Drawing.Point(12, 129);
			this._newPassword.Name = "_newPassword";
			this._newPassword.PasswordChar = '*';
			this._newPassword.Size = new System.Drawing.Size(234, 20);
			this._newPassword.TabIndex = 5;
			this._newPassword.TextChanged += new System.EventHandler(this._newPassword_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 162);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(116, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Confirm New Password";
			// 
			// _newPasswordConfirm
			// 
			this._newPasswordConfirm.Location = new System.Drawing.Point(12, 181);
			this._newPasswordConfirm.Name = "_newPasswordConfirm";
			this._newPasswordConfirm.PasswordChar = '*';
			this._newPasswordConfirm.Size = new System.Drawing.Size(234, 20);
			this._newPasswordConfirm.TabIndex = 7;
			this._newPasswordConfirm.TextChanged += new System.EventHandler(this._newPasswordConfirm_TextChanged);
			// 
			// _errorProvider
			// 
			this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this._errorProvider.ContainerControl = this;
			// 
			// ChangePasswordForm
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(272, 258);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._newPasswordConfirm);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._newPassword);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._password);
			this.Controls.Add(this._userName);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ChangePasswordForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Password";
			this.Load += new System.EventHandler(this.ChangePasswordForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _password;
        private System.Windows.Forms.TextBox _userName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _newPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _newPasswordConfirm;
        private System.Windows.Forms.ErrorProvider _errorProvider;
    }
}