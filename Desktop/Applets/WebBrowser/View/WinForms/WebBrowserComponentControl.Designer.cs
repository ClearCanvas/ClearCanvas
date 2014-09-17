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

namespace ClearCanvas.Desktop.Applets.WebBrowser.View.WinForms
{
    partial class WebBrowserComponentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebBrowserComponentControl));
            this._browser = new System.Windows.Forms.WebBrowser();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this._statusBar = new System.Windows.Forms.StatusStrip();
            this._browserProgress = new System.Windows.Forms.ToolStripProgressBar();
            this._browserStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this._toolbar = new System.Windows.Forms.ToolStrip();
            this._back = new System.Windows.Forms.ToolStripButton();
            this._forward = new System.Windows.Forms.ToolStripButton();
            this._stop = new System.Windows.Forms.ToolStripButton();
            this._refresh = new System.Windows.Forms.ToolStripButton();
            this._address = new System.Windows.Forms.ToolStripComboBox();
            this._go = new System.Windows.Forms.ToolStripButton();
            this._progressLogo = new System.Windows.Forms.ToolStripLabel();
            this._shortcutToolbar = new System.Windows.Forms.ToolStrip();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this._statusBar.SuspendLayout();
            this._toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // _browser
            // 
            this._browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browser.Location = new System.Drawing.Point(0, 0);
            this._browser.MinimumSize = new System.Drawing.Size(20, 20);
            this._browser.Name = "_browser";
            this._browser.ScriptErrorsSuppressed = true;
            this._browser.Size = new System.Drawing.Size(584, 440);
            this._browser.TabIndex = 0;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this._statusBar);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this._browser);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(584, 440);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(584, 526);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._toolbar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._shortcutToolbar);
            // 
            // _statusBar
            // 
            this._statusBar.Dock = System.Windows.Forms.DockStyle.None;
            this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._browserProgress,
            this._browserStatus});
            this._statusBar.Location = new System.Drawing.Point(0, 0);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Size = new System.Drawing.Size(584, 22);
            this._statusBar.TabIndex = 1;
            this._statusBar.Text = "statusStrip1";
            // 
            // _browserProgress
            // 
            this._browserProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._browserProgress.Name = "_browserProgress";
            this._browserProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // _browserStatus
            // 
            this._browserStatus.Name = "_browserStatus";
            this._browserStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // _toolbar
            // 
            this._toolbar.Dock = System.Windows.Forms.DockStyle.None;
            this._toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolbar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this._toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._back,
            this._forward,
            this._stop,
            this._refresh,
            this._address,
            this._go,
            this._progressLogo});
            this._toolbar.Location = new System.Drawing.Point(0, 0);
            this._toolbar.Name = "_toolbar";
            this._toolbar.Size = new System.Drawing.Size(584, 39);
            this._toolbar.Stretch = true;
            this._toolbar.TabIndex = 0;
            // 
            // _back
            // 
            this._back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._back.Image = ((System.Drawing.Image)(resources.GetObject("_back.Image")));
            this._back.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._back.Name = "_back";
            this._back.Size = new System.Drawing.Size(36, 36);
            this._back.Text = "Back";
            // 
            // _forward
            // 
            this._forward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._forward.Image = ((System.Drawing.Image)(resources.GetObject("_forward.Image")));
            this._forward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._forward.Name = "_forward";
            this._forward.Size = new System.Drawing.Size(36, 36);
            this._forward.Text = "Forward";
            // 
            // _stop
            // 
            this._stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._stop.Image = ((System.Drawing.Image)(resources.GetObject("_stop.Image")));
            this._stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._stop.Name = "_stop";
            this._stop.Size = new System.Drawing.Size(36, 36);
            this._stop.Text = "toolStripButton1";
            this._stop.ToolTipText = "Stop";
            // 
            // _refresh
            // 
            this._refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._refresh.Image = ((System.Drawing.Image)(resources.GetObject("_refresh.Image")));
            this._refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._refresh.Name = "_refresh";
            this._refresh.Size = new System.Drawing.Size(36, 36);
            this._refresh.Text = "toolStripButton1";
            this._refresh.ToolTipText = "Refresh";
            // 
            // _address
            // 
            this._address.Name = "_address";
            this._address.Size = new System.Drawing.Size(350, 39);
            this._address.ToolTipText = "Address";
            // 
            // _go
            // 
            this._go.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._go.Image = ((System.Drawing.Image)(resources.GetObject("_go.Image")));
            this._go.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._go.Name = "_go";
            this._go.Size = new System.Drawing.Size(36, 36);
            this._go.Text = "toolStripButton1";
            this._go.ToolTipText = "Go";
            // 
            // _progressLogo
            // 
            this._progressLogo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._progressLogo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._progressLogo.Name = "_progressLogo";
            this._progressLogo.Size = new System.Drawing.Size(0, 36);
            // 
            // _shortcutToolbar
            // 
            this._shortcutToolbar.Dock = System.Windows.Forms.DockStyle.None;
            this._shortcutToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._shortcutToolbar.Location = new System.Drawing.Point(0, 39);
            this._shortcutToolbar.Name = "_shortcutToolbar";
            this._shortcutToolbar.Size = new System.Drawing.Size(584, 25);
            this._shortcutToolbar.Stretch = true;
            this._shortcutToolbar.TabIndex = 1;
            // 
            // WebBrowserComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "WebBrowserComponentControl";
            this.Size = new System.Drawing.Size(584, 526);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this._statusBar.ResumeLayout(false);
            this._statusBar.PerformLayout();
            this._toolbar.ResumeLayout(false);
            this._toolbar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.WebBrowser _browser;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStrip _toolbar;
		private System.Windows.Forms.ToolStripButton _back;
		private System.Windows.Forms.ToolStripButton _forward;
		private System.Windows.Forms.ToolStripButton _stop;
		private System.Windows.Forms.ToolStripButton _refresh;
		private System.Windows.Forms.ToolStripComboBox _address;
		private System.Windows.Forms.ToolStripButton _go;
		private System.Windows.Forms.ToolStripLabel _progressLogo;
		private System.Windows.Forms.StatusStrip _statusBar;
		private System.Windows.Forms.ToolStripStatusLabel _browserStatus;
		private System.Windows.Forms.ToolStripProgressBar _browserProgress;
		private System.Windows.Forms.ToolStrip _shortcutToolbar;
    }
}
