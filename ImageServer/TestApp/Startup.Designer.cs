#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion


namespace ClearCanvas.ImageServer.TestApp
{
    partial class Startup
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
			this.TestRule = new System.Windows.Forms.Button();
			this.TestHeaderStreamButton = new System.Windows.Forms.Button();
			this.buttonCompression = new System.Windows.Forms.Button();
			this.TestEditStudyButton = new System.Windows.Forms.Button();
			this.RandomImageSender = new System.Windows.Forms.Button();
			this.ExtremeStreaming = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.UsageTracking = new System.Windows.Forms.Button();
			this.DatabaseGenerator = new System.Windows.Forms.Button();
			this.ProductVerify = new System.Windows.Forms.Button();
			this._archiveTestBtn = new System.Windows.Forms.Button();
			this._perfMon = new System.Windows.Forms.Button();
			this._cfindPerformanceTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TestRule
			// 
			this.TestRule.Location = new System.Drawing.Point(28, 37);
			this.TestRule.Name = "TestRule";
			this.TestRule.Size = new System.Drawing.Size(183, 59);
			this.TestRule.TabIndex = 0;
			this.TestRule.Text = "DICOM File Tests";
			this.TestRule.UseVisualStyleBackColor = true;
			this.TestRule.Click += new System.EventHandler(this.TestRule_Click);
			// 
			// TestHeaderStreamButton
			// 
			this.TestHeaderStreamButton.Location = new System.Drawing.Point(258, 37);
			this.TestHeaderStreamButton.Name = "TestHeaderStreamButton";
			this.TestHeaderStreamButton.Size = new System.Drawing.Size(176, 59);
			this.TestHeaderStreamButton.TabIndex = 1;
			this.TestHeaderStreamButton.Text = "Header Retrieval Client";
			this.TestHeaderStreamButton.UseVisualStyleBackColor = true;
			this.TestHeaderStreamButton.Click += new System.EventHandler(this.TestHeaderStreamButton_Click);
			// 
			// buttonCompression
			// 
			this.buttonCompression.Location = new System.Drawing.Point(28, 135);
			this.buttonCompression.Name = "buttonCompression";
			this.buttonCompression.Size = new System.Drawing.Size(183, 59);
			this.buttonCompression.TabIndex = 2;
			this.buttonCompression.Text = "Compression";
			this.buttonCompression.UseVisualStyleBackColor = true;
			this.buttonCompression.Click += new System.EventHandler(this.buttonCompression_Click);
			// 
			// TestEditStudyButton
			// 
			this.TestEditStudyButton.Location = new System.Drawing.Point(258, 135);
			this.TestEditStudyButton.Name = "TestEditStudyButton";
			this.TestEditStudyButton.Size = new System.Drawing.Size(183, 59);
			this.TestEditStudyButton.TabIndex = 2;
			this.TestEditStudyButton.Text = "Edit Study";
			this.TestEditStudyButton.UseVisualStyleBackColor = true;
			this.TestEditStudyButton.Click += new System.EventHandler(this.buttonEditStudy_Click);
			// 
			// RandomImageSender
			// 
			this.RandomImageSender.Location = new System.Drawing.Point(28, 228);
			this.RandomImageSender.Name = "RandomImageSender";
			this.RandomImageSender.Size = new System.Drawing.Size(183, 59);
			this.RandomImageSender.TabIndex = 2;
			this.RandomImageSender.Text = "Random Image Sender";
			this.RandomImageSender.UseVisualStyleBackColor = true;
			this.RandomImageSender.Click += new System.EventHandler(this.RandomImageSender_Click);
			// 
			// ExtremeStreaming
			// 
			this.ExtremeStreaming.Location = new System.Drawing.Point(258, 228);
			this.ExtremeStreaming.Name = "ExtremeStreaming";
			this.ExtremeStreaming.Size = new System.Drawing.Size(183, 59);
			this.ExtremeStreaming.TabIndex = 2;
			this.ExtremeStreaming.Text = "Extreme Streaming";
			this.ExtremeStreaming.UseVisualStyleBackColor = true;
			this.ExtremeStreaming.Click += new System.EventHandler(this.ExtremeStreaming_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(258, 316);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(183, 52);
			this.button1.TabIndex = 3;
			this.button1.Text = "Streaming";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// UsageTracking
			// 
			this.UsageTracking.Enabled = false;
			this.UsageTracking.Location = new System.Drawing.Point(28, 312);
			this.UsageTracking.Name = "UsageTracking";
			this.UsageTracking.Size = new System.Drawing.Size(183, 52);
			this.UsageTracking.TabIndex = 4;
			this.UsageTracking.Text = "Usage Tracking";
			this.UsageTracking.UseVisualStyleBackColor = true;
			this.UsageTracking.Click += new System.EventHandler(this.UsageTracking_Click);
			// 
			// DatabaseGenerator
			// 
			this.DatabaseGenerator.Location = new System.Drawing.Point(259, 395);
			this.DatabaseGenerator.Name = "DatabaseGenerator";
			this.DatabaseGenerator.Size = new System.Drawing.Size(182, 51);
			this.DatabaseGenerator.TabIndex = 4;
			this.DatabaseGenerator.Text = "Database Generator";
			this.DatabaseGenerator.UseVisualStyleBackColor = true;
			this.DatabaseGenerator.Click += new System.EventHandler(this.DatabaseGenerator_Click);
			// 
			// ProductVerify
			// 
			this.ProductVerify.Location = new System.Drawing.Point(28, 395);
			this.ProductVerify.Name = "ProductVerify";
			this.ProductVerify.Size = new System.Drawing.Size(182, 51);
			this.ProductVerify.TabIndex = 4;
			this.ProductVerify.Text = "Product Verify";
			this.ProductVerify.UseVisualStyleBackColor = true;
			this.ProductVerify.Click += new System.EventHandler(this.ProductVerify_Click);
			// 
			// _archiveTestBtn
			// 
			this._archiveTestBtn.Location = new System.Drawing.Point(487, 37);
			this._archiveTestBtn.Name = "_archiveTestBtn";
			this._archiveTestBtn.Size = new System.Drawing.Size(183, 59);
			this._archiveTestBtn.TabIndex = 2;
			this._archiveTestBtn.Text = "Archive Test";
			this._archiveTestBtn.UseVisualStyleBackColor = true;
			this._archiveTestBtn.Click += new System.EventHandler(this._archiveTestBtn_Click);
			// 
			// _perfMon
			// 
			this._perfMon.Location = new System.Drawing.Point(487, 135);
			this._perfMon.Name = "_perfMon";
			this._perfMon.Size = new System.Drawing.Size(183, 59);
			this._perfMon.TabIndex = 2;
			this._perfMon.Text = "Performance Monitor";
			this._perfMon.UseVisualStyleBackColor = true;
			this._perfMon.Click += new System.EventHandler(this._perfMon_Click);
			// 
			// _cfindPerformanceTest
			// 
			this._cfindPerformanceTest.Location = new System.Drawing.Point(487, 228);
			this._cfindPerformanceTest.Name = "_cfindPerformanceTest";
			this._cfindPerformanceTest.Size = new System.Drawing.Size(183, 59);
			this._cfindPerformanceTest.TabIndex = 2;
			this._cfindPerformanceTest.Text = "DICOM C-FIND Performance";
			this._cfindPerformanceTest.UseVisualStyleBackColor = true;
			this._cfindPerformanceTest.Click += new System.EventHandler(this._perfMon_Click);
			// 
			// Startup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(738, 474);
			this.Controls.Add(this.ProductVerify);
			this.Controls.Add(this.DatabaseGenerator);
			this.Controls.Add(this.UsageTracking);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.ExtremeStreaming);
			this.Controls.Add(this._cfindPerformanceTest);
			this.Controls.Add(this._perfMon);
			this.Controls.Add(this._archiveTestBtn);
			this.Controls.Add(this.RandomImageSender);
			this.Controls.Add(this.TestEditStudyButton);
			this.Controls.Add(this.buttonCompression);
			this.Controls.Add(this.TestHeaderStreamButton);
			this.Controls.Add(this.TestRule);
			this.Name = "Startup";
			this.Text = "Startup";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TestRule;
        private System.Windows.Forms.Button TestHeaderStreamButton;
        private System.Windows.Forms.Button buttonCompression;
        private System.Windows.Forms.Button TestEditStudyButton;
        private System.Windows.Forms.Button RandomImageSender;
        private System.Windows.Forms.Button ExtremeStreaming;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button UsageTracking;
        private System.Windows.Forms.Button DatabaseGenerator;
        private System.Windows.Forms.Button ProductVerify;
		private System.Windows.Forms.Button _archiveTestBtn;
		private System.Windows.Forms.Button _perfMon;
		private System.Windows.Forms.Button _cfindPerformanceTest;
    }
}