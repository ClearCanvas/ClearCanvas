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

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    partial class DicomSendConfigurationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DicomSendConfigurationComponentControl));
			this._retryDelayValue = new System.Windows.Forms.NumericUpDown();
			this._retryDelayUnits = new System.Windows.Forms.ComboBox();
			this._maxNumberOfRetries = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._retryDelayValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._maxNumberOfRetries)).BeginInit();
			this.SuspendLayout();
			// 
			// _retryDelayValue
			// 
			resources.ApplyResources(this._retryDelayValue, "_retryDelayValue");
			this._retryDelayValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._retryDelayValue.Name = "_retryDelayValue";
			this._retryDelayValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _retryDelayUnits
			// 
			this._retryDelayUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._retryDelayUnits.FormattingEnabled = true;
			resources.ApplyResources(this._retryDelayUnits, "_retryDelayUnits");
			this._retryDelayUnits.Name = "_retryDelayUnits";
			// 
			// _maxNumberOfRetries
			// 
			resources.ApplyResources(this._maxNumberOfRetries, "_maxNumberOfRetries");
			this._maxNumberOfRetries.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this._maxNumberOfRetries.Name = "_maxNumberOfRetries";
			this._maxNumberOfRetries.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// DicomSendConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._maxNumberOfRetries);
			this.Controls.Add(this._retryDelayValue);
			this.Controls.Add(this._retryDelayUnits);
			this.Name = "DicomSendConfigurationComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._retryDelayValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._maxNumberOfRetries)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.NumericUpDown _retryDelayValue;
		private System.Windows.Forms.ComboBox _retryDelayUnits;
		private System.Windows.Forms.NumericUpDown _maxNumberOfRetries;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}
