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

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    partial class UserEditorComponentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserEditorComponentControl));
            this._authorityGroups = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._userId = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._passwordExpiryDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._displayName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._accountEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this._emailAddress = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _authorityGroups
            // 
            this._authorityGroups.ColumnHeaderTooltip = null;
            resources.ApplyResources(this._authorityGroups, "_authorityGroups");
            this._authorityGroups.MultiSelect = false;
            this._authorityGroups.Name = "_authorityGroups";
            this._authorityGroups.ReadOnly = false;
            this._authorityGroups.ShowToolbar = false;
            this._authorityGroups.SortButtonTooltip = null;
            // 
            // _userId
            // 
            resources.ApplyResources(this._userId, "_userId");
            this._userId.Name = "_userId";
            this._userId.ToolTip = null;
            this._userId.Value = null;
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
            // _validFrom
            // 
            resources.ApplyResources(this._validFrom, "_validFrom");
            this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.Value = null;
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._emailAddress);
            this.groupBox2.Controls.Add(this._passwordExpiryDate);
            this.groupBox2.Controls.Add(this._displayName);
            this.groupBox2.Controls.Add(this._accountEnabledCheckBox);
            this.groupBox2.Controls.Add(this._validFrom);
            this.groupBox2.Controls.Add(this._validUntil);
            this.groupBox2.Controls.Add(this._userId);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // _passwordExpiryDate
            // 
            resources.ApplyResources(this._passwordExpiryDate, "_passwordExpiryDate");
            this._passwordExpiryDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._passwordExpiryDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._passwordExpiryDate.Name = "_passwordExpiryDate";
            this._passwordExpiryDate.Nullable = true;
            this._passwordExpiryDate.Value = null;
            // 
            // _displayName
            // 
            resources.ApplyResources(this._displayName, "_displayName");
            this._displayName.Name = "_displayName";
            this._displayName.ToolTip = null;
            this._displayName.Value = null;
            // 
            // _accountEnabledCheckBox
            // 
            resources.ApplyResources(this._accountEnabledCheckBox, "_accountEnabledCheckBox");
            this._accountEnabledCheckBox.Name = "_accountEnabledCheckBox";
            this._accountEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _emailAddress
            // 
            resources.ApplyResources(this._emailAddress, "_emailAddress");
            this._emailAddress.Name = "_emailAddress";
            this._emailAddress.ToolTip = null;
            this._emailAddress.Value = null;
            // 
            // UserEditorComponentControl
            // 
            this.AcceptButton = this._acceptButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this._authorityGroups);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._acceptButton);
            this.Name = "UserEditorComponentControl";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _authorityGroups;
        private ClearCanvas.Desktop.View.WinForms.TextField _userId;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox _accountEnabledCheckBox;
        private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TextField _displayName;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _passwordExpiryDate;
        private ClearCanvas.Desktop.View.WinForms.TextField _emailAddress;

    }
}
