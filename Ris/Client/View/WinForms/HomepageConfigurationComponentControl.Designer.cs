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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class HomepageConfigurationComponentControl
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
			this._showHomepageOnStartup = new System.Windows.Forms.CheckBox();
			this._preventHompageClosing = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _showHomepageOnStartup
			// 
			this._showHomepageOnStartup.AutoSize = true;
			this._showHomepageOnStartup.Location = new System.Drawing.Point(48, 41);
			this._showHomepageOnStartup.Name = "_showHomepageOnStartup";
			this._showHomepageOnStartup.Size = new System.Drawing.Size(165, 17);
			this._showHomepageOnStartup.TabIndex = 0;
			this._showHomepageOnStartup.Text = "Show Homepage on Start-Up";
			this._showHomepageOnStartup.UseVisualStyleBackColor = true;
			// 
			// _preventHompageClosing
			// 
			this._preventHompageClosing.AutoSize = true;
			this._preventHompageClosing.Location = new System.Drawing.Point(48, 89);
			this._preventHompageClosing.Name = "_preventHompageClosing";
			this._preventHompageClosing.Size = new System.Drawing.Size(204, 17);
			this._preventHompageClosing.TabIndex = 1;
			this._preventHompageClosing.Text = "Prevent Homepage from being closed";
			this._preventHompageClosing.UseVisualStyleBackColor = true;
			// 
			// HomepageConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._preventHompageClosing);
			this.Controls.Add(this._showHomepageOnStartup);
			this.Name = "HomepageConfigurationComponentControl";
			this.Size = new System.Drawing.Size(380, 191);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox _showHomepageOnStartup;
		private System.Windows.Forms.CheckBox _preventHompageClosing;
    }
}
