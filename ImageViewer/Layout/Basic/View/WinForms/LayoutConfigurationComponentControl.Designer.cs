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

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class LayoutConfigurationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutConfigurationComponentControl));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._imageBoxLabelRows = new System.Windows.Forms.Label();
			this._imageBoxRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._imageBoxLabelColumns = new System.Windows.Forms.Label();
			this._imageBoxColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._tileLabelRows = new System.Windows.Forms.Label();
			this._tileRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._tileLabelColumns = new System.Windows.Forms.Label();
			this._tileColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._modality = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._imageBoxLabelRows);
			this.groupBox1.Controls.Add(this._imageBoxRows);
			this.groupBox1.Controls.Add(this._imageBoxLabelColumns);
			this.groupBox1.Controls.Add(this._imageBoxColumns);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// _imageBoxLabelRows
			// 
			resources.ApplyResources(this._imageBoxLabelRows, "_imageBoxLabelRows");
			this._imageBoxLabelRows.Name = "_imageBoxLabelRows";
			// 
			// _imageBoxRows
			// 
			resources.ApplyResources(this._imageBoxRows, "_imageBoxRows");
			this._imageBoxRows.Name = "_imageBoxRows";
			// 
			// _imageBoxLabelColumns
			// 
			resources.ApplyResources(this._imageBoxLabelColumns, "_imageBoxLabelColumns");
			this._imageBoxLabelColumns.Name = "_imageBoxLabelColumns";
			// 
			// _imageBoxColumns
			// 
			resources.ApplyResources(this._imageBoxColumns, "_imageBoxColumns");
			this._imageBoxColumns.Name = "_imageBoxColumns";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._tileLabelRows);
			this.groupBox2.Controls.Add(this._tileRows);
			this.groupBox2.Controls.Add(this._tileLabelColumns);
			this.groupBox2.Controls.Add(this._tileColumns);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// _tileLabelRows
			// 
			resources.ApplyResources(this._tileLabelRows, "_tileLabelRows");
			this._tileLabelRows.Name = "_tileLabelRows";
			// 
			// _tileRows
			// 
			resources.ApplyResources(this._tileRows, "_tileRows");
			this._tileRows.Name = "_tileRows";
			// 
			// _tileLabelColumns
			// 
			resources.ApplyResources(this._tileLabelColumns, "_tileLabelColumns");
			this._tileLabelColumns.Name = "_tileLabelColumns";
			// 
			// _tileColumns
			// 
			resources.ApplyResources(this._tileColumns, "_tileColumns");
			this._tileColumns.Name = "_tileColumns";
			// 
			// _modality
			// 
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.FormattingEnabled = true;
			resources.ApplyResources(this._modality, "_modality");
			this._modality.Name = "_modality";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// LayoutConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._modality);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "LayoutConfigurationComponentControl";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxColumns;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label _imageBoxLabelColumns;
		private System.Windows.Forms.Label _imageBoxLabelRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxRows;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label _tileLabelRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileRows;
		private System.Windows.Forms.Label _tileLabelColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileColumns;
		private System.Windows.Forms.ComboBox _modality;
		private System.Windows.Forms.Label label1;
    }
}
