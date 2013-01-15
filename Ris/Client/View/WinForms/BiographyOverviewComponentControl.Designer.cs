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

using System.Windows.Forms;
namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class BiographyOverviewComponentControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiographyOverviewComponentControl));
            this._alertIcons = new System.Windows.Forms.ImageList(this.components);
            this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this._contentPanel = new System.Windows.Forms.Panel();
            this._overviewLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _alertIcons
            // 
            this._alertIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_alertIcons.ImageStream")));
            this._alertIcons.TransparentColor = System.Drawing.Color.Transparent;
            this._alertIcons.Images.SetKeyName(0, "");
            this._alertIcons.Images.SetKeyName(1, "");
            this._alertIcons.Images.SetKeyName(2, "");
            this._alertIcons.Images.SetKeyName(3, "");
            this._alertIcons.Images.SetKeyName(4, "");
            this._alertIcons.Images.SetKeyName(5, "");
            // 
            // _overviewLayoutPanel
            // 
            this._overviewLayoutPanel.ColumnCount = 1;
            this._overviewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
            this._overviewLayoutPanel.Controls.Add(this._contentPanel, 0, 1);
            this._overviewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._overviewLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
            this._overviewLayoutPanel.RowCount = 2;
            this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._overviewLayoutPanel.Size = new System.Drawing.Size(720, 467);
            this._overviewLayoutPanel.TabIndex = 0;
            // 
            // _bannerPanel
            // 
            this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bannerPanel.Location = new System.Drawing.Point(3, 3);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(714, 89);
            this._bannerPanel.TabIndex = 0;
            // 
            // _contentPanel
            // 
            this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentPanel.Location = new System.Drawing.Point(3, 98);
            this._contentPanel.Name = "_contentPanel";
            this._contentPanel.Size = new System.Drawing.Size(714, 366);
            this._contentPanel.TabIndex = 1;
            // 
            // BiographyOverviewComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._overviewLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "BiographyOverviewComponentControl";
            this.Size = new System.Drawing.Size(720, 467);
            this._overviewLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageList _alertIcons;
        private TableLayoutPanel _overviewLayoutPanel;
        private Panel _bannerPanel;
        private Panel _contentPanel;


    }
}
