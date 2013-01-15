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

namespace ClearCanvas.Desktop.View.WinForms
{
	partial class SplashScreen
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
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
			this._status = new System.Windows.Forms.Label();
			this._version = new System.Windows.Forms.Label();
			this._copyright = new System.Windows.Forms.Label();
			this._license = new System.Windows.Forms.Label();
			this._manifest = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _status
			// 
			this._status.AutoEllipsis = true;
			this._status.BackColor = System.Drawing.Color.Transparent;
			this._status.ForeColor = System.Drawing.Color.Black;
			this._status.Location = new System.Drawing.Point(295, 61);
			this._status.Name = "_status";
			this._status.Size = new System.Drawing.Size(354, 13);
			this._status.TabIndex = 1;
			this._status.Text = "Status";
			// 
			// _version
			// 
			this._version.BackColor = System.Drawing.Color.Transparent;
			this._version.ForeColor = System.Drawing.Color.White;
			this._version.Location = new System.Drawing.Point(447, 292);
			this._version.Name = "_version";
			this._version.Size = new System.Drawing.Size(214, 14);
			this._version.TabIndex = 2;
			this._version.Text = "Version";
			// 
			// _copyright
			// 
			this._copyright.BackColor = System.Drawing.Color.Transparent;
			this._copyright.ForeColor = System.Drawing.Color.Black;
			this._copyright.Location = new System.Drawing.Point(375, 340);
			this._copyright.Name = "_copyright";
			this._copyright.Size = new System.Drawing.Size(286, 14);
			this._copyright.TabIndex = 3;
			this._copyright.Text = "Copyright";
			// 
			// _license
			// 
			this._license.AutoSize = true;
			this._license.BackColor = System.Drawing.Color.Transparent;
			this._license.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this._license.ForeColor = System.Drawing.Color.Black;
			this._license.Location = new System.Drawing.Point(617, 9);
			this._license.Name = "_license";
			this._license.Size = new System.Drawing.Size(44, 13);
			this._license.TabIndex = 0;
			this._license.Text = "License";
			this._license.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this._license.Visible = false;
			// 
			// _manifest
			// 
			this._manifest.AutoSize = true;
			this._manifest.BackColor = System.Drawing.Color.Transparent;
			this._manifest.ForeColor = System.Drawing.Color.Firebrick;
			this._manifest.Location = new System.Drawing.Point(447, 314);
			this._manifest.Name = "_manifest";
			this._manifest.Size = new System.Drawing.Size(217, 13);
			this._manifest.TabIndex = 4;
			this._manifest.Text = "Warning: This installation has been modified.";
			// 
			// SplashScreen
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImage = global::ClearCanvas.Desktop.View.WinForms.SR.Splash;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(673, 385);
			this.ControlBox = false;
			this.Controls.Add(this._manifest);
			this.Controls.Add(this._license);
			this.Controls.Add(this._copyright);
			this.Controls.Add(this._version);
			this.Controls.Add(this._status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplashScreen";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ClearCanvas";
			this.Shown += new System.EventHandler(this.SplashScreen_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _status;
		private System.Windows.Forms.Label _version;
		private System.Windows.Forms.Label _copyright;
		private System.Windows.Forms.Label _license;
        private System.Windows.Forms.Label _manifest;
	}
}