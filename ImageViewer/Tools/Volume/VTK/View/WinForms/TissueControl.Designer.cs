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

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK.View.WinForms
{
	partial class TissueControl
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
			this._visibleCheckBox = new System.Windows.Forms.CheckBox();
			this._presetComboBox = new System.Windows.Forms.ComboBox();
			this._presetLabel = new System.Windows.Forms.Label();
			this._windowLabel = new System.Windows.Forms.Label();
			this._levelLabel = new System.Windows.Forms.Label();
			this._opacityLabel = new System.Windows.Forms.Label();
			this._opacityControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this._windowControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this._levelControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this._surfaceRenderingRadio = new System.Windows.Forms.RadioButton();
			this._volumeRenderingRadio = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// _visibleCheckBox
			// 
			this._visibleCheckBox.AutoSize = true;
			this._visibleCheckBox.Location = new System.Drawing.Point(17, 13);
			this._visibleCheckBox.Name = "_visibleCheckBox";
			this._visibleCheckBox.Size = new System.Drawing.Size(56, 17);
			this._visibleCheckBox.TabIndex = 0;
			this._visibleCheckBox.Text = "Visible";
			this._visibleCheckBox.UseVisualStyleBackColor = true;
			// 
			// _presetComboBox
			// 
			this._presetComboBox.FormattingEnabled = true;
			this._presetComboBox.Location = new System.Drawing.Point(99, 104);
			this._presetComboBox.Name = "_presetComboBox";
			this._presetComboBox.Size = new System.Drawing.Size(156, 21);
			this._presetComboBox.TabIndex = 1;
			// 
			// _presetLabel
			// 
			this._presetLabel.AutoSize = true;
			this._presetLabel.Location = new System.Drawing.Point(12, 107);
			this._presetLabel.Name = "_presetLabel";
			this._presetLabel.Size = new System.Drawing.Size(37, 13);
			this._presetLabel.TabIndex = 6;
			this._presetLabel.Text = "Preset";
			// 
			// _windowLabel
			// 
			this._windowLabel.AutoSize = true;
			this._windowLabel.Location = new System.Drawing.Point(12, 199);
			this._windowLabel.Name = "_windowLabel";
			this._windowLabel.Size = new System.Drawing.Size(46, 13);
			this._windowLabel.TabIndex = 7;
			this._windowLabel.Text = "Window";
			// 
			// _levelLabel
			// 
			this._levelLabel.AutoSize = true;
			this._levelLabel.Location = new System.Drawing.Point(12, 248);
			this._levelLabel.Name = "_levelLabel";
			this._levelLabel.Size = new System.Drawing.Size(33, 13);
			this._levelLabel.TabIndex = 8;
			this._levelLabel.Text = "Level";
			// 
			// _opacityLabel
			// 
			this._opacityLabel.AutoSize = true;
			this._opacityLabel.Location = new System.Drawing.Point(12, 152);
			this._opacityLabel.Name = "_opacityLabel";
			this._opacityLabel.Size = new System.Drawing.Size(43, 13);
			this._opacityLabel.TabIndex = 9;
			this._opacityLabel.Text = "Opacity";
			// 
			// _opacityControl
			// 
			this._opacityControl.AutoSize = true;
			this._opacityControl.DecimalPlaces = 2;
			this._opacityControl.Location = new System.Drawing.Point(89, 143);
			this._opacityControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._opacityControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._opacityControl.Name = "_opacityControl";
			this._opacityControl.Size = new System.Drawing.Size(256, 42);
			this._opacityControl.TabIndex = 10;
			this._opacityControl.TrackBarIncrements = 100;
			this._opacityControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// _windowControl
			// 
			this._windowControl.AutoSize = true;
			this._windowControl.DecimalPlaces = 0;
			this._windowControl.Location = new System.Drawing.Point(89, 191);
			this._windowControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._windowControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._windowControl.Name = "_windowControl";
			this._windowControl.Size = new System.Drawing.Size(256, 42);
			this._windowControl.TabIndex = 11;
			this._windowControl.TrackBarIncrements = 100;
			this._windowControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// _levelControl
			// 
			this._levelControl.AutoSize = true;
			this._levelControl.DecimalPlaces = 0;
			this._levelControl.Location = new System.Drawing.Point(89, 239);
			this._levelControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._levelControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._levelControl.Name = "_levelControl";
			this._levelControl.Size = new System.Drawing.Size(256, 42);
			this._levelControl.TabIndex = 12;
			this._levelControl.TrackBarIncrements = 100;
			this._levelControl.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// _surfaceRenderingRadio
			// 
			this._surfaceRenderingRadio.AutoCheck = false;
			this._surfaceRenderingRadio.AutoSize = true;
			this._surfaceRenderingRadio.Location = new System.Drawing.Point(17, 43);
			this._surfaceRenderingRadio.Name = "_surfaceRenderingRadio";
			this._surfaceRenderingRadio.Size = new System.Drawing.Size(114, 17);
			this._surfaceRenderingRadio.TabIndex = 13;
			this._surfaceRenderingRadio.TabStop = true;
			this._surfaceRenderingRadio.Text = "Surface Rendering";
			this._surfaceRenderingRadio.UseVisualStyleBackColor = true;
			// 
			// _volumeRenderingRadio
			// 
			this._volumeRenderingRadio.AutoCheck = false;
			this._volumeRenderingRadio.AutoSize = true;
			this._volumeRenderingRadio.Location = new System.Drawing.Point(17, 66);
			this._volumeRenderingRadio.Name = "_volumeRenderingRadio";
			this._volumeRenderingRadio.Size = new System.Drawing.Size(112, 17);
			this._volumeRenderingRadio.TabIndex = 14;
			this._volumeRenderingRadio.TabStop = true;
			this._volumeRenderingRadio.Text = "Volume Rendering";
			this._volumeRenderingRadio.UseVisualStyleBackColor = true;
			// 
			// TissueControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._volumeRenderingRadio);
			this.Controls.Add(this._surfaceRenderingRadio);
			this.Controls.Add(this._visibleCheckBox);
			this.Controls.Add(this._presetLabel);
			this.Controls.Add(this._presetComboBox);
			this.Controls.Add(this._opacityLabel);
			this.Controls.Add(this._opacityControl);
			this.Controls.Add(this._windowLabel);
			this.Controls.Add(this._windowControl);
			this.Controls.Add(this._levelLabel);
			this.Controls.Add(this._levelControl);
			this.Name = "TissueControl";
			this.Size = new System.Drawing.Size(371, 300);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _visibleCheckBox;
		private System.Windows.Forms.ComboBox _presetComboBox;
		private System.Windows.Forms.Label _presetLabel;
		private System.Windows.Forms.Label _windowLabel;
		private System.Windows.Forms.Label _levelLabel;
		private System.Windows.Forms.Label _opacityLabel;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _opacityControl;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _windowControl;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _levelControl;
		private System.Windows.Forms.RadioButton _surfaceRenderingRadio;
		private System.Windows.Forms.RadioButton _volumeRenderingRadio;

	}
}
