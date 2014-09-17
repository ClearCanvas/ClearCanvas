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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class DicomSeriesEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DicomSeriesEditorComponentControl));
			this._seriesNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._seriesDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._seriesInstanceUID = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyInstanceUID = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._numberOfImages = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._numberOfImages)).BeginInit();
			this.SuspendLayout();
			// 
			// _seriesNumber
			// 
			resources.ApplyResources(this._seriesNumber, "_seriesNumber");
			this._seriesNumber.Mask = "";
			this._seriesNumber.Name = "_seriesNumber";
			this._seriesNumber.Value = null;
			// 
			// _seriesDescription
			// 
			resources.ApplyResources(this._seriesDescription, "_seriesDescription");
			this._seriesDescription.Mask = "";
			this._seriesDescription.Name = "_seriesDescription";
			this._seriesDescription.Value = null;
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
			// _seriesInstanceUID
			// 
			resources.ApplyResources(this._seriesInstanceUID, "_seriesInstanceUID");
			this._seriesInstanceUID.Mask = "";
			this._seriesInstanceUID.Name = "_seriesInstanceUID";
			this._seriesInstanceUID.ReadOnly = true;
			this._seriesInstanceUID.Value = null;
			// 
			// _studyInstanceUID
			// 
			resources.ApplyResources(this._studyInstanceUID, "_studyInstanceUID");
			this._studyInstanceUID.Mask = "";
			this._studyInstanceUID.Name = "_studyInstanceUID";
			this._studyInstanceUID.ReadOnly = true;
			this._studyInstanceUID.Value = null;
			// 
			// _numberOfImages
			// 
			resources.ApplyResources(this._numberOfImages, "_numberOfImages");
			this._numberOfImages.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
			this._numberOfImages.Name = "_numberOfImages";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// DicomSeriesEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._numberOfImages);
			this.Controls.Add(this._studyInstanceUID);
			this.Controls.Add(this._seriesInstanceUID);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._seriesDescription);
			this.Controls.Add(this._seriesNumber);
			this.Name = "DicomSeriesEditorComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._numberOfImages)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _seriesNumber;
		private ClearCanvas.Desktop.View.WinForms.TextField _seriesDescription;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _seriesInstanceUID;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyInstanceUID;
		private System.Windows.Forms.NumericUpDown _numberOfImages;
		private System.Windows.Forms.Label label1;
    }
}
