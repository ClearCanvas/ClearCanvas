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

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    partial class ChangePixelAspectRatioComponentControl
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
			this._ok = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			this._increasePixelDimensions = new System.Windows.Forms.CheckBox();
			this._removeCalibration = new System.Windows.Forms.CheckBox();
			this._convertDisplaySet = new System.Windows.Forms.CheckBox();
			this._row = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._column = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._row)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._column)).BeginInit();
			this.SuspendLayout();
			// 
			// _ok
			// 
			this._ok.Location = new System.Drawing.Point(18, 126);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(75, 23);
			this._ok.TabIndex = 7;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			// 
			// _cancel
			// 
			this._cancel.Location = new System.Drawing.Point(99, 126);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(75, 23);
			this._cancel.TabIndex = 8;
			this._cancel.Text = "Cancel";
			this._cancel.UseVisualStyleBackColor = true;
			// 
			// _increasePixelDimensions
			// 
			this._increasePixelDimensions.AutoSize = true;
			this._increasePixelDimensions.Location = new System.Drawing.Point(18, 97);
			this._increasePixelDimensions.Name = "_increasePixelDimensions";
			this._increasePixelDimensions.Size = new System.Drawing.Size(146, 17);
			this._increasePixelDimensions.TabIndex = 6;
			this._increasePixelDimensions.Text = "Increase pixel dimensions";
			this._increasePixelDimensions.UseVisualStyleBackColor = true;
			// 
			// _removeCalibration
			// 
			this._removeCalibration.AutoSize = true;
			this._removeCalibration.Location = new System.Drawing.Point(18, 74);
			this._removeCalibration.Name = "_removeCalibration";
			this._removeCalibration.Size = new System.Drawing.Size(117, 17);
			this._removeCalibration.TabIndex = 5;
			this._removeCalibration.Text = "Remove calibration";
			this._removeCalibration.UseVisualStyleBackColor = true;
			// 
			// _convertDisplaySet
			// 
			this._convertDisplaySet.AutoSize = true;
			this._convertDisplaySet.Location = new System.Drawing.Point(18, 53);
			this._convertDisplaySet.Name = "_convertDisplaySet";
			this._convertDisplaySet.Size = new System.Drawing.Size(144, 17);
			this._convertDisplaySet.TabIndex = 4;
			this._convertDisplaySet.Text = "Convert entire display set";
			this._convertDisplaySet.UseVisualStyleBackColor = true;
			// 
			// _row
			// 
			this._row.Location = new System.Drawing.Point(18, 28);
			this._row.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._row.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._row.Name = "_row";
			this._row.Size = new System.Drawing.Size(51, 20);
			this._row.TabIndex = 2;
			this._row.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _column
			// 
			this._column.Location = new System.Drawing.Point(99, 28);
			this._column.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._column.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._column.Name = "_column";
			this._column.Size = new System.Drawing.Size(51, 20);
			this._column.TabIndex = 3;
			this._column.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Row";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(96, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Column";
			// 
			// ChangePixelAspectRatioComponentControl
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancel;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._column);
			this.Controls.Add(this._row);
			this.Controls.Add(this._convertDisplaySet);
			this.Controls.Add(this._removeCalibration);
			this.Controls.Add(this._increasePixelDimensions);
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._ok);
			this.Name = "ChangePixelAspectRatioComponentControl";
			this.Size = new System.Drawing.Size(193, 167);
			((System.ComponentModel.ISupportInitialize)(this._row)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._column)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Button _cancel;
		private System.Windows.Forms.CheckBox _increasePixelDimensions;
		private System.Windows.Forms.CheckBox _removeCalibration;
		private System.Windows.Forms.CheckBox _convertDisplaySet;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _row;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _column;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}
