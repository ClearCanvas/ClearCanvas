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

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class CineApplicationComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CineApplicationComponentControl));
			this._cineSpeed = new System.Windows.Forms.TrackBar();
			this._minLabel = new System.Windows.Forms.Label();
			this._fastestLabel = new System.Windows.Forms.Label();
			this._startReverseButton = new System.Windows.Forms.Button();
			this._startForwardButton = new System.Windows.Forms.Button();
			this._stopButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).BeginInit();
			this.SuspendLayout();
			// 
			// _cineSpeed
			// 
			this._cineSpeed.LargeChange = 10;
			resources.ApplyResources(this._cineSpeed, "_cineSpeed");
			this._cineSpeed.Maximum = 100;
			this._cineSpeed.Name = "_cineSpeed";
			this._cineSpeed.TickFrequency = 10;
			// 
			// _minLabel
			// 
			resources.ApplyResources(this._minLabel, "_minLabel");
			this._minLabel.Name = "_minLabel";
			// 
			// _fastestLabel
			// 
			resources.ApplyResources(this._fastestLabel, "_fastestLabel");
			this._fastestLabel.Name = "_fastestLabel";
			// 
			// _startReverseButton
			// 
			resources.ApplyResources(this._startReverseButton, "_startReverseButton");
			this._startReverseButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.PlayReverse;
			this._startReverseButton.Name = "_startReverseButton";
			this._startReverseButton.UseVisualStyleBackColor = true;
			this._startReverseButton.Click += new System.EventHandler(this.StartReverseButtonClicked);
			// 
			// _startForwardButton
			// 
			resources.ApplyResources(this._startForwardButton, "_startForwardButton");
			this._startForwardButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.Play;
			this._startForwardButton.Name = "_startForwardButton";
			this._startForwardButton.UseVisualStyleBackColor = true;
			this._startForwardButton.Click += new System.EventHandler(this.StartForwardButtonClicked);
			// 
			// _stopButton
			// 
			resources.ApplyResources(this._stopButton, "_stopButton");
			this._stopButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.Stop;
			this._stopButton.Name = "_stopButton";
			this._stopButton.UseVisualStyleBackColor = true;
			this._stopButton.Click += new System.EventHandler(this.StopButtonClicked);
			// 
			// CineApplicationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._startForwardButton);
			this.Controls.Add(this._fastestLabel);
			this.Controls.Add(this._minLabel);
			this.Controls.Add(this._cineSpeed);
			this.Controls.Add(this._stopButton);
			this.Controls.Add(this._startReverseButton);
			this.Name = "CineApplicationComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _startReverseButton;
		private System.Windows.Forms.Button _stopButton;
		private System.Windows.Forms.TrackBar _cineSpeed;
		private System.Windows.Forms.Label _minLabel;
		private System.Windows.Forms.Label _fastestLabel;
		private System.Windows.Forms.Button _startForwardButton;
    }
}
