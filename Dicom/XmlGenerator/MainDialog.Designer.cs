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

namespace ClearCanvas.Dicom.XmlGenerator
{
    partial class MainDialog
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
            this._buttonLoadFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._buttonLoadDirectory = new System.Windows.Forms.Button();
            this._buttonGenerateXml = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._buttonLoadXml = new System.Windows.Forms.Button();
            this._buttonGenerateGzipXml = new System.Windows.Forms.Button();
            this._buttonLoadGzipXml = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonLoadFile
            // 
            this._buttonLoadFile.Location = new System.Drawing.Point(12, 12);
            this._buttonLoadFile.Name = "_buttonLoadFile";
            this._buttonLoadFile.Size = new System.Drawing.Size(99, 23);
            this._buttonLoadFile.TabIndex = 0;
            this._buttonLoadFile.Text = "Load File";
            this._buttonLoadFile.UseVisualStyleBackColor = true;
            this._buttonLoadFile.Click += new System.EventHandler(this.ButtonLoadFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "File.dcm";
            // 
            // _buttonLoadDirectory
            // 
            this._buttonLoadDirectory.Location = new System.Drawing.Point(12, 54);
            this._buttonLoadDirectory.Name = "_buttonLoadDirectory";
            this._buttonLoadDirectory.Size = new System.Drawing.Size(99, 23);
            this._buttonLoadDirectory.TabIndex = 1;
            this._buttonLoadDirectory.Text = "Load Directory";
            this._buttonLoadDirectory.UseVisualStyleBackColor = true;
            this._buttonLoadDirectory.Click += new System.EventHandler(this._buttonLoadDirectory_Click);
            // 
            // _buttonGenerateXml
            // 
            this._buttonGenerateXml.Location = new System.Drawing.Point(12, 98);
            this._buttonGenerateXml.Name = "_buttonGenerateXml";
            this._buttonGenerateXml.Size = new System.Drawing.Size(98, 23);
            this._buttonGenerateXml.TabIndex = 2;
            this._buttonGenerateXml.Text = "Generate XML";
            this._buttonGenerateXml.UseVisualStyleBackColor = true;
            this._buttonGenerateXml.Click += new System.EventHandler(this._buttonGenerateXml_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "XML files|*.xml";
            // 
            // _buttonLoadXml
            // 
            this._buttonLoadXml.Location = new System.Drawing.Point(12, 140);
            this._buttonLoadXml.Name = "_buttonLoadXml";
            this._buttonLoadXml.Size = new System.Drawing.Size(98, 23);
            this._buttonLoadXml.TabIndex = 3;
            this._buttonLoadXml.Text = "Load XML";
            this._buttonLoadXml.UseVisualStyleBackColor = true;
            this._buttonLoadXml.Click += new System.EventHandler(this._buttonLoadXml_Click);
            // 
            // _buttonGenerateGzipXml
            // 
            this._buttonGenerateGzipXml.Location = new System.Drawing.Point(148, 98);
            this._buttonGenerateGzipXml.Name = "_buttonGenerateGzipXml";
            this._buttonGenerateGzipXml.Size = new System.Drawing.Size(109, 23);
            this._buttonGenerateGzipXml.TabIndex = 4;
            this._buttonGenerateGzipXml.Text = "Generate Gzip XML";
            this._buttonGenerateGzipXml.UseVisualStyleBackColor = true;
            this._buttonGenerateGzipXml.Click += new System.EventHandler(this._buttonGenerateGzipXml_Click);
            // 
            // _buttonLoadGzipXml
            // 
            this._buttonLoadGzipXml.Location = new System.Drawing.Point(148, 140);
            this._buttonLoadGzipXml.Name = "_buttonLoadGzipXml";
            this._buttonLoadGzipXml.Size = new System.Drawing.Size(109, 23);
            this._buttonLoadGzipXml.TabIndex = 5;
            this._buttonLoadGzipXml.Text = "Load Gzip Xml";
            this._buttonLoadGzipXml.UseVisualStyleBackColor = true;
            this._buttonLoadGzipXml.Click += new System.EventHandler(this._buttonLoadGzipXml_Click);
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 199);
            this.Controls.Add(this._buttonLoadGzipXml);
            this.Controls.Add(this._buttonGenerateGzipXml);
            this.Controls.Add(this._buttonLoadXml);
            this.Controls.Add(this._buttonGenerateXml);
            this.Controls.Add(this._buttonLoadDirectory);
            this.Controls.Add(this._buttonLoadFile);
            this.Name = "MainDialog";
            this.Text = "XmlGenerator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonLoadFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button _buttonLoadDirectory;
        private System.Windows.Forms.Button _buttonGenerateXml;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button _buttonLoadXml;
        private System.Windows.Forms.Button _buttonGenerateGzipXml;
        private System.Windows.Forms.Button _buttonLoadGzipXml;
    }
}