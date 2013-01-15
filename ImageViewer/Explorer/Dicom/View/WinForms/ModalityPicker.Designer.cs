#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class ModalityPicker
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModalityPicker));
			this._showModalityListButton = new System.Windows.Forms.Button();
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._modalitiesString = new System.Windows.Forms.Label();
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _showModalityListButton
			// 
			resources.ApplyResources(this._showModalityListButton, "_showModalityListButton");
			this._showModalityListButton.Name = "_showModalityListButton";
			this._showModalityListButton.TabStop = false;
			this._showModalityListButton.UseVisualStyleBackColor = true;
			this._showModalityListButton.Click += new System.EventHandler(this.OnShowModalityListClick);
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.AllowMerge = false;
			resources.ApplyResources(this._contextMenuStrip, "_contextMenuStrip");
			this._contextMenuStrip.DropShadowEnabled = false;
			this._contextMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this._contextMenuStrip.Name = "contextMenuStrip1";
			this._contextMenuStrip.ShowImageMargin = false;
			this._contextMenuStrip.ShowItemToolTips = false;
			// 
			// _modalitiesString
			// 
			resources.ApplyResources(this._modalitiesString, "_modalitiesString");
			this._modalitiesString.AutoEllipsis = true;
			this._modalitiesString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._modalitiesString.Name = "_modalitiesString";
			// 
			// _label
			// 
			resources.ApplyResources(this._label, "_label");
			this._label.Name = "_label";
			// 
			// ModalityPicker
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._label);
			this.Controls.Add(this._modalitiesString);
			this.Controls.Add(this._showModalityListButton);
			this.Name = "ModalityPicker";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _showModalityListButton;
		private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
		private System.Windows.Forms.Label _modalitiesString;
		private System.Windows.Forms.Label _label;

	}
}
