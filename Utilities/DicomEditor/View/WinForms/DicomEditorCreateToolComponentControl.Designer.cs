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

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    partial class DicomEditorCreateToolComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DicomEditorCreateToolComponentControl));
			this._accept = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			this._groupLabel = new System.Windows.Forms.Label();
			this._group = new System.Windows.Forms.TextBox();
			this._elementLabel = new System.Windows.Forms.Label();
			this._element = new System.Windows.Forms.TextBox();
			this._value = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._vr = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._tagName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.SuspendLayout();
			// 
			// _accept
			// 
			resources.ApplyResources(this._accept, "_accept");
			this._accept.Name = "_accept";
			this._accept.UseVisualStyleBackColor = true;
			this._accept.Click += new System.EventHandler(this._accept_Click);
			// 
			// _cancel
			// 
			resources.ApplyResources(this._cancel, "_cancel");
			this._cancel.Name = "_cancel";
			this._cancel.UseVisualStyleBackColor = true;
			this._cancel.Click += new System.EventHandler(this._cancel_Click);
			// 
			// _groupLabel
			// 
			resources.ApplyResources(this._groupLabel, "_groupLabel");
			this._groupLabel.Name = "_groupLabel";
			// 
			// _group
			// 
			this._group.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this._group, "_group");
			this._group.Name = "_group";
			// 
			// _elementLabel
			// 
			resources.ApplyResources(this._elementLabel, "_elementLabel");
			this._elementLabel.Name = "_elementLabel";
			// 
			// _element
			// 
			this._element.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this._element, "_element");
			this._element.Name = "_element";
			// 
			// _value
			// 
			resources.ApplyResources(this._value, "_value");
			this._value.Name = "_value";
			this._value.ToolTip = null;
			this._value.Value = null;
			// 
			// _vr
			// 
			resources.ApplyResources(this._vr, "_vr");
			this._vr.Name = "_vr";
			this._vr.ToolTip = null;
			this._vr.Value = null;
			// 
			// _tagName
			// 
			resources.ApplyResources(this._tagName, "_tagName");
			this._tagName.Name = "_tagName";
			this._tagName.ToolTip = null;
			this._tagName.Value = null;
			// 
			// DicomEditorCreateToolComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tagName);
			this.Controls.Add(this._vr);
			this.Controls.Add(this._value);
			this.Controls.Add(this._elementLabel);
			this.Controls.Add(this._element);
			this.Controls.Add(this._groupLabel);
			this.Controls.Add(this._group);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._accept);
			this.Name = "DicomEditorCreateToolComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _accept;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Label _groupLabel;
        private System.Windows.Forms.TextBox _group;
        private System.Windows.Forms.Label _elementLabel;
        private System.Windows.Forms.TextBox _element;
        private ClearCanvas.Desktop.View.WinForms.TextField _value;
        private ClearCanvas.Desktop.View.WinForms.TextField _vr;
        private ClearCanvas.Desktop.View.WinForms.TextField _tagName;
    }
}
