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

namespace ClearCanvas.Dicom.DataDictionaryGenerator
{
    partial class MainForm
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
            this.OpenFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OpenTransferSyntax = new System.Windows.Forms.Button();
            this.GenerateCode = new System.Windows.Forms.Button();
            this.openFileDialog_TransferSyntax = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // OpenFile
            // 
            this.OpenFile.Location = new System.Drawing.Point(12, 12);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(151, 23);
            this.OpenFile.TabIndex = 0;
            this.OpenFile.Text = "Open XML Part 6 File";
            this.OpenFile.UseVisualStyleBackColor = true;
            this.OpenFile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OpenFile_MouseDown);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "04v06_06.xml";
            this.openFileDialog1.Filter = "XML files|*.xml";
            this.openFileDialog1.Title = "Select xml representation of Part 6 of the DICOM Standard";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // OpenTransferSyntax
            // 
            this.OpenTransferSyntax.Location = new System.Drawing.Point(12, 41);
            this.OpenTransferSyntax.Name = "OpenTransferSyntax";
            this.OpenTransferSyntax.Size = new System.Drawing.Size(151, 23);
            this.OpenTransferSyntax.TabIndex = 1;
            this.OpenTransferSyntax.Text = "Load Transfer Syntax XML File";
            this.OpenTransferSyntax.UseVisualStyleBackColor = true;
            this.OpenTransferSyntax.Click += new System.EventHandler(this.OpenTransferSyntax_Click);
            // 
            // GenerateCode
            // 
            this.GenerateCode.Location = new System.Drawing.Point(12, 70);
            this.GenerateCode.Name = "GenerateCode";
            this.GenerateCode.Size = new System.Drawing.Size(151, 23);
            this.GenerateCode.TabIndex = 2;
            this.GenerateCode.Text = "Generate Code";
            this.GenerateCode.UseVisualStyleBackColor = true;
            this.GenerateCode.Click += new System.EventHandler(this.GenerateCode_Click);
            // 
            // openFileDialog_TransferSyntax
            // 
            this.openFileDialog_TransferSyntax.FileName = "TransferSyntax.xml";
            this.openFileDialog_TransferSyntax.Filter = "XML files|*.xml";
            this.openFileDialog_TransferSyntax.Title = "Select XML Transfer Syntax File";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 144);
            this.Controls.Add(this.GenerateCode);
            this.Controls.Add(this.OpenTransferSyntax);
            this.Controls.Add(this.OpenFile);
            this.Name = "MainForm";
            this.Text = "Dictionary Generator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal System.Windows.Forms.Button OpenFile;
        private System.Windows.Forms.Button OpenTransferSyntax;
        private System.Windows.Forms.Button GenerateCode;
        private System.Windows.Forms.OpenFileDialog openFileDialog_TransferSyntax;
    }
}

