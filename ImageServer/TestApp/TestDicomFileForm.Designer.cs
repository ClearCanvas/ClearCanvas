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

namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestDicomFileForm
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
			this.checkBoxLoadTest = new System.Windows.Forms.CheckBox();
			this.buttonScanDirectory = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.button1 = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.buttonSearchForStudies = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// checkBoxLoadTest
			// 
			this.checkBoxLoadTest.AutoSize = true;
			this.checkBoxLoadTest.Location = new System.Drawing.Point(12, 12);
			this.checkBoxLoadTest.Name = "checkBoxLoadTest";
			this.checkBoxLoadTest.Size = new System.Drawing.Size(71, 17);
			this.checkBoxLoadTest.TabIndex = 0;
			this.checkBoxLoadTest.Text = "LoadTest";
			this.checkBoxLoadTest.UseVisualStyleBackColor = true;
			this.checkBoxLoadTest.CheckedChanged += new System.EventHandler(this.checkBoxLoadTest_CheckedChanged);
			// 
			// buttonScanDirectory
			// 
			this.buttonScanDirectory.Location = new System.Drawing.Point(13, 36);
			this.buttonScanDirectory.Name = "buttonScanDirectory";
			this.buttonScanDirectory.Size = new System.Drawing.Size(162, 23);
			this.buttonScanDirectory.TabIndex = 1;
			this.buttonScanDirectory.Text = "Scan Directories";
			this.buttonScanDirectory.UseVisualStyleBackColor = true;
			this.buttonScanDirectory.Click += new System.EventHandler(this.buttonSelectDirectory_Click);
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.Description = "Select Folder for Scanning";
			this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 94);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(162, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Modify File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			this.openFileDialog.Filter = "DICOM Files|*.dcm|All files|*.*";
			// 
			// buttonSearchForStudies
			// 
			this.buttonSearchForStudies.Location = new System.Drawing.Point(12, 65);
			this.buttonSearchForStudies.Name = "buttonSearchForStudies";
			this.buttonSearchForStudies.Size = new System.Drawing.Size(163, 23);
			this.buttonSearchForStudies.TabIndex = 3;
			this.buttonSearchForStudies.Text = "Search For Studies";
			this.buttonSearchForStudies.UseVisualStyleBackColor = true;
			this.buttonSearchForStudies.Click += new System.EventHandler(this.buttonSearchForStudies_Click);
			// 
			// TestDicomFileForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
			this.Controls.Add(this.buttonSearchForStudies);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.buttonScanDirectory);
			this.Controls.Add(this.checkBoxLoadTest);
			this.Name = "TestDicomFileForm";
			this.Text = "DICOM File Tests";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLoadTest;
        private System.Windows.Forms.Button buttonScanDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button buttonSearchForStudies;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

