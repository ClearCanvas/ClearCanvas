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
    partial class ProductVerificationTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.ComponentName = new System.Windows.Forms.Label();
            this.ManifestVerified = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(202, 119);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 45);
            this.button1.TabIndex = 0;
            this.button1.Text = "Verify";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ComponentName
            // 
            this.ComponentName.AutoSize = true;
            this.ComponentName.Location = new System.Drawing.Point(124, 197);
            this.ComponentName.Name = "ComponentName";
            this.ComponentName.Size = new System.Drawing.Size(89, 13);
            this.ComponentName.TabIndex = 1;
            this.ComponentName.Text = "ComponentName";
            // 
            // ManifestVerified
            // 
            this.ManifestVerified.AutoSize = true;
            this.ManifestVerified.Location = new System.Drawing.Point(124, 225);
            this.ManifestVerified.Name = "ManifestVerified";
            this.ManifestVerified.Size = new System.Drawing.Size(82, 13);
            this.ManifestVerified.TabIndex = 1;
            this.ManifestVerified.Text = "ManifestVerified";
            // 
            // ProductVerificationTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 301);
            this.Controls.Add(this.ManifestVerified);
            this.Controls.Add(this.ComponentName);
            this.Controls.Add(this.button1);
            this.Name = "ProductVerificationTest";
            this.Text = "ProductVerificationTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label ComponentName;
        private System.Windows.Forms.Label ManifestVerified;
    }
}