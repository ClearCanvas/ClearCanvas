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

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe.View.WinForms
{
    partial class DynamicTeComponentControl
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
			this._createDynamicTeButton = new System.Windows.Forms.Button();
			this._probabilityMapVisible = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._thresholdControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this._opacityControl = new ClearCanvas.Desktop.View.WinForms.TrackBarUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _createDynamicTeButton
			// 
			this._createDynamicTeButton.Location = new System.Drawing.Point(18, 20);
			this._createDynamicTeButton.Name = "_createDynamicTeButton";
			this._createDynamicTeButton.Size = new System.Drawing.Size(164, 23);
			this._createDynamicTeButton.TabIndex = 0;
			this._createDynamicTeButton.Text = "Create Dynamic TE Series";
			this._createDynamicTeButton.UseVisualStyleBackColor = true;
			// 
			// _probabilityMapVisible
			// 
			this._probabilityMapVisible.AutoSize = true;
			this._probabilityMapVisible.Location = new System.Drawing.Point(15, 31);
			this._probabilityMapVisible.Name = "_probabilityMapVisible";
			this._probabilityMapVisible.Size = new System.Drawing.Size(56, 17);
			this._probabilityMapVisible.TabIndex = 1;
			this._probabilityMapVisible.Text = "Visible";
			this._probabilityMapVisible.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._opacityControl);
			this.groupBox1.Controls.Add(this._thresholdControl);
			this.groupBox1.Controls.Add(this._probabilityMapVisible);
			this.groupBox1.Location = new System.Drawing.Point(18, 68);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(279, 165);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Probability Map Overlay";
			// 
			// _thresholdControl
			// 
			this._thresholdControl.AutoSize = true;
			this._thresholdControl.DecimalPlaces = 0;
			this._thresholdControl.Location = new System.Drawing.Point(6, 79);
			this._thresholdControl.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._thresholdControl.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._thresholdControl.Name = "_thresholdControl";
			this._thresholdControl.Size = new System.Drawing.Size(245, 26);
			this._thresholdControl.TabIndex = 2;
			this._thresholdControl.TrackBarIncrements = 10;
			this._thresholdControl.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// _opacityControl
			// 
			this._opacityControl.AutoSize = true;
			this._opacityControl.DecimalPlaces = 0;
			this._opacityControl.Location = new System.Drawing.Point(6, 133);
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
			this._opacityControl.Size = new System.Drawing.Size(245, 26);
			this._opacityControl.TabIndex = 3;
			this._opacityControl.TrackBarIncrements = 10;
			this._opacityControl.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Threshold";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 117);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Opacity";
			// 
			// DynamicTeComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._createDynamicTeButton);
			this.Name = "DynamicTeComponentControl";
			this.Size = new System.Drawing.Size(321, 263);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _createDynamicTeButton;
		private System.Windows.Forms.CheckBox _probabilityMapVisible;
		private System.Windows.Forms.GroupBox groupBox1;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _thresholdControl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TrackBarUpDown _opacityControl;
    }
}
