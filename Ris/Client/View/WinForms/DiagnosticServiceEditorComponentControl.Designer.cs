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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticServiceEditorComponentControl));
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
			resources.ApplyResources(this._itemSelector, "_itemSelector");
			this._itemSelector.Name = "_itemSelector";
			this._itemSelector.ReadOnly = false;
			this._itemSelector.SelectedItemsTable = null;
			// 
			// _idBox
			// 
			resources.ApplyResources(this._idBox, "_idBox");
			this._idBox.Mask = "";
			this._idBox.Name = "_idBox";
			this._idBox.Value = null;
			// 
			// _nameBox
			// 
			resources.ApplyResources(this._nameBox, "_nameBox");
			this._nameBox.Mask = "";
			this._nameBox.Name = "_nameBox";
			this._nameBox.Value = null;
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
			// _procedureTypeGroupBox
			// 
			this._procedureTypeGroupBox.Controls.Add(this._itemSelector);
			resources.ApplyResources(this._procedureTypeGroupBox, "_procedureTypeGroupBox");
			this._procedureTypeGroupBox.Name = "_procedureTypeGroupBox";
			this._procedureTypeGroupBox.TabStop = false;
			// 
			// DiagnosticServiceEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._procedureTypeGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._nameBox);
			this.Controls.Add(this._idBox);
			this.Name = "DiagnosticServiceEditorComponentControl";
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