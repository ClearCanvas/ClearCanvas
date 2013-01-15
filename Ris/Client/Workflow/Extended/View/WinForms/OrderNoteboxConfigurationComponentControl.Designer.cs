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

using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class OrderNoteboxConfigurationComponentControl
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
			this._foldersTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._addGroupButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._groupLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.SuspendLayout();
			// 
			// _foldersTableView
			// 
			this._foldersTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._foldersTableView.Location = new System.Drawing.Point(3, 85);
			this._foldersTableView.MultiSelect = false;
			this._foldersTableView.Name = "_foldersTableView";
			this._foldersTableView.ReadOnly = false;
			this._foldersTableView.Size = new System.Drawing.Size(461, 248);
			this._foldersTableView.TabIndex = 3;
			// 
			// _addGroupButton
			// 
			this._addGroupButton.Location = new System.Drawing.Point(408, 30);
			this._addGroupButton.Name = "_addGroupButton";
			this._addGroupButton.Size = new System.Drawing.Size(37, 23);
			this._addGroupButton.TabIndex = 1;
			this._addGroupButton.Text = "Add";
			this._addGroupButton.UseVisualStyleBackColor = true;
			this._addGroupButton.Click += new System.EventHandler(this._addGroupButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 69);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Select group folders to display";
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(308, 339);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 4;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(389, 339);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _groupLookup
			// 
			this._groupLookup.AutoSize = true;
			this._groupLookup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._groupLookup.LabelText = "Add a group";
			this._groupLookup.Location = new System.Drawing.Point(3, 12);
			this._groupLookup.Margin = new System.Windows.Forms.Padding(0);
			this._groupLookup.Name = "_groupLookup";
			this._groupLookup.Size = new System.Drawing.Size(402, 41);
			this._groupLookup.TabIndex = 0;
			this._groupLookup.Value = null;
			// 
			// OrderNoteboxConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._addGroupButton);
			this.Controls.Add(this._groupLookup);
			this.Controls.Add(this._foldersTableView);
			this.Name = "OrderNoteboxConfigurationComponentControl";
			this.Size = new System.Drawing.Size(467, 365);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _foldersTableView;
		private LookupField _groupLookup;
		private System.Windows.Forms.Button _addGroupButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
    }
}
