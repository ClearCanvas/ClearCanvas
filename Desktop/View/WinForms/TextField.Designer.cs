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

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TextField
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextField));
			this._label = new System.Windows.Forms.Label();
			this._textFieldToolTip = new System.Windows.Forms.ToolTip(this.components);
			this._textBox = new Clifton.Windows.Forms.NullableMaskedEdit();
			this.SuspendLayout();
			// 
			// _label
			// 
			resources.ApplyResources(this._label, "_label");
			this._label.Name = "_label";
			// 
			// _textBox
			// 
			this._textBox.AllowDrop = true;
			resources.ApplyResources(this._textBox, "_textBox");
			this._textBox.AutoAdvance = true;
			this._textBox.EditMask = "";
			this._textBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
			this._textBox.Name = "_textBox";
			this._textBox.NullTextDisplayValue = null;
			this._textBox.NullTextReturnValue = null;
			this._textBox.NullValue = null;
			this._textBox.SelectGroup = true;
			this._textBox.SkipLiterals = false;
			this._textBox.Text = null;
			this._textBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this._textBox.Value = null;
			this._textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this._textBox_KeyDown);
			this._textBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._textBox_DragDrop);
			this._textBox.DragEnter += new System.Windows.Forms.DragEventHandler(this._textBox_DragEnter);
			this._textBox.MouseHover += new System.EventHandler(this._textBox_MouseHover);
			this._textBox.MouseLeave += new System.EventHandler(this._textBox_MouseLeave);
			// 
			// TextField
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._textBox);
			this.Controls.Add(this._label);
			this.Name = "TextField";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private Clifton.Windows.Forms.NullableMaskedEdit _textBox;
        private System.Windows.Forms.ToolTip _textFieldToolTip;
        private System.ComponentModel.IContainer components;
    }
}
