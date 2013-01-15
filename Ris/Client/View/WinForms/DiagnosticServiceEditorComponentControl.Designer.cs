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
	partial class DiagnosticServiceEditorComponentControl
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
			this._itemSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._idBox = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._nameBox = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._procedureTypeGroupBox = new System.Windows.Forms.GroupBox();
			this._procedureTypeGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _itemSelector
			// 
			this._itemSelector.AvailableItemsTable = null;
			this._itemSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._itemSelector.Location = new System.Drawing.Point(3, 16);
			this._itemSelector.Name = "_itemSelector";
			this._itemSelector.ReadOnly = false;
			this._itemSelector.SelectedItemsTable = null;
			this._itemSelector.Size = new System.Drawing.Size(544, 406);
			this._itemSelector.TabIndex = 0;
			// 
			// _idBox
			// 
			this._idBox.LabelText = "ID";
			this._idBox.Location = new System.Drawing.Point(3, 3);
			this._idBox.Mask = "";
			this._idBox.Name = "_idBox";
			this._idBox.PasswordChar = '\0';
			this._idBox.Size = new System.Drawing.Size(523, 41);
			this._idBox.TabIndex = 0;
			this._idBox.ToolTip = null;
			this._idBox.Value = null;
			// 
			// _nameBox
			// 
			this._nameBox.LabelText = "Name";
			this._nameBox.Location = new System.Drawing.Point(3, 50);
			this._nameBox.Mask = "";
			this._nameBox.Name = "_nameBox";
			this._nameBox.PasswordChar = '\0';
			this._nameBox.Size = new System.Drawing.Size(523, 41);
			this._nameBox.TabIndex = 1;
			this._nameBox.ToolTip = null;
			this._nameBox.Value = null;
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(390, 527);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 3;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(471, 527);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _procedureTypeGroupBox
			// 
			this._procedureTypeGroupBox.Controls.Add(this._itemSelector);
			this._procedureTypeGroupBox.Location = new System.Drawing.Point(3, 97);
			this._procedureTypeGroupBox.Name = "_procedureTypeGroupBox";
			this._procedureTypeGroupBox.Size = new System.Drawing.Size(550, 425);
			this._procedureTypeGroupBox.TabIndex = 2;
			this._procedureTypeGroupBox.TabStop = false;
			this._procedureTypeGroupBox.Text = "Procedure Types";
			// 
			// DiagnosticServiceEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._procedureTypeGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._nameBox);
			this.Controls.Add(this._idBox);
			this.Name = "DiagnosticServiceEditorComponentControl";
			this.Size = new System.Drawing.Size(556, 561);
			this._procedureTypeGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.ListItemSelector _itemSelector;
		private ClearCanvas.Desktop.View.WinForms.TextField _idBox;
		private ClearCanvas.Desktop.View.WinForms.TextField _nameBox;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.GroupBox _procedureTypeGroupBox;
	}
}