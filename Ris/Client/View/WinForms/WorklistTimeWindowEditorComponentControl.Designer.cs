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
    partial class WorklistTimeWindowEditorComponentControl
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
			this._slidingScale = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._fromCheckBox = new System.Windows.Forms.CheckBox();
			this._toCheckBox = new System.Windows.Forms.CheckBox();
			this._toFixed = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._fromFixed = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._fixedWindowRadioButton = new System.Windows.Forms.RadioButton();
			this._slidingWindowRadioButton = new System.Windows.Forms.RadioButton();
			this._groupBox = new System.Windows.Forms.GroupBox();
			this._toSliding = new ClearCanvas.Ris.Client.View.WinForms.DescriptiveSpinControl();
			this._fromSliding = new ClearCanvas.Ris.Client.View.WinForms.DescriptiveSpinControl();
			this._groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _slidingScale
			// 
			this._slidingScale.DataSource = null;
			this._slidingScale.DisplayMember = "";
			this._slidingScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._slidingScale.LabelText = "Scale";
			this._slidingScale.Location = new System.Drawing.Point(105, 68);
			this._slidingScale.Margin = new System.Windows.Forms.Padding(2);
			this._slidingScale.Name = "_slidingScale";
			this._slidingScale.Size = new System.Drawing.Size(91, 45);
			this._slidingScale.TabIndex = 4;
			this._slidingScale.Value = null;
			// 
			// _fromCheckBox
			// 
			this._fromCheckBox.AutoSize = true;
			this._fromCheckBox.Location = new System.Drawing.Point(108, 32);
			this._fromCheckBox.Name = "_fromCheckBox";
			this._fromCheckBox.Size = new System.Drawing.Size(49, 17);
			this._fromCheckBox.TabIndex = 2;
			this._fromCheckBox.Text = "From";
			this._fromCheckBox.UseVisualStyleBackColor = true;
			// 
			// _toCheckBox
			// 
			this._toCheckBox.AutoSize = true;
			this._toCheckBox.Location = new System.Drawing.Point(288, 32);
			this._toCheckBox.Name = "_toCheckBox";
			this._toCheckBox.Size = new System.Drawing.Size(39, 17);
			this._toCheckBox.TabIndex = 3;
			this._toCheckBox.Text = "To";
			this._toCheckBox.UseVisualStyleBackColor = true;
			// 
			// _toFixed
			// 
			this._toFixed.LabelText = "To";
			this._toFixed.Location = new System.Drawing.Point(285, 175);
			this._toFixed.Margin = new System.Windows.Forms.Padding(2);
			this._toFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._toFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._toFixed.Name = "_toFixed";
			this._toFixed.Size = new System.Drawing.Size(150, 41);
			this._toFixed.TabIndex = 8;
			this._toFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
			// 
			// _fromFixed
			// 
			this._fromFixed.LabelText = "From";
			this._fromFixed.Location = new System.Drawing.Point(105, 175);
			this._fromFixed.Margin = new System.Windows.Forms.Padding(2);
			this._fromFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._fromFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._fromFixed.Name = "_fromFixed";
			this._fromFixed.Size = new System.Drawing.Size(150, 41);
			this._fromFixed.TabIndex = 7;
			this._fromFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
			// 
			// _fixedWindowRadioButton
			// 
			this._fixedWindowRadioButton.AutoSize = true;
			this._fixedWindowRadioButton.Location = new System.Drawing.Point(23, 175);
			this._fixedWindowRadioButton.Name = "_fixedWindowRadioButton";
			this._fixedWindowRadioButton.Size = new System.Drawing.Size(50, 17);
			this._fixedWindowRadioButton.TabIndex = 1;
			this._fixedWindowRadioButton.TabStop = true;
			this._fixedWindowRadioButton.Text = "Fixed";
			this._fixedWindowRadioButton.UseVisualStyleBackColor = true;
			// 
			// _slidingWindowRadioButton
			// 
			this._slidingWindowRadioButton.AutoSize = true;
			this._slidingWindowRadioButton.Location = new System.Drawing.Point(23, 75);
			this._slidingWindowRadioButton.Name = "_slidingWindowRadioButton";
			this._slidingWindowRadioButton.Size = new System.Drawing.Size(56, 17);
			this._slidingWindowRadioButton.TabIndex = 0;
			this._slidingWindowRadioButton.TabStop = true;
			this._slidingWindowRadioButton.Text = "Sliding";
			this._slidingWindowRadioButton.UseVisualStyleBackColor = true;
			// 
			// _groupBox
			// 
			this._groupBox.Controls.Add(this._toSliding);
			this._groupBox.Controls.Add(this._fromSliding);
			this._groupBox.Controls.Add(this._slidingScale);
			this._groupBox.Controls.Add(this._fromCheckBox);
			this._groupBox.Controls.Add(this._toCheckBox);
			this._groupBox.Controls.Add(this._toFixed);
			this._groupBox.Controls.Add(this._fromFixed);
			this._groupBox.Controls.Add(this._fixedWindowRadioButton);
			this._groupBox.Controls.Add(this._slidingWindowRadioButton);
			this._groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._groupBox.Location = new System.Drawing.Point(0, 0);
			this._groupBox.Name = "_groupBox";
			this._groupBox.Size = new System.Drawing.Size(455, 246);
			this._groupBox.TabIndex = 0;
			this._groupBox.TabStop = false;
			this._groupBox.Text = "Time Window";
			// 
			// _toSliding
			// 
			this._toSliding.Format = null;
			this._toSliding.Location = new System.Drawing.Point(288, 118);
			this._toSliding.Name = "_toSliding";
			this._toSliding.Size = new System.Drawing.Size(120, 20);
			this._toSliding.TabIndex = 6;
			// 
			// _fromSliding
			// 
			this._fromSliding.Format = null;
			this._fromSliding.Location = new System.Drawing.Point(108, 118);
			this._fromSliding.Name = "_fromSliding";
			this._fromSliding.Size = new System.Drawing.Size(120, 20);
			this._fromSliding.TabIndex = 5;
			// 
			// WorklistTimeWindowEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupBox);
			this.Name = "WorklistTimeWindowEditorComponentControl";
			this.Size = new System.Drawing.Size(455, 246);
			this._groupBox.ResumeLayout(false);
			this._groupBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _slidingScale;
        private System.Windows.Forms.CheckBox _fromCheckBox;
		private System.Windows.Forms.CheckBox _toCheckBox;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _toFixed;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _fromFixed;
        private System.Windows.Forms.RadioButton _fixedWindowRadioButton;
        private System.Windows.Forms.RadioButton _slidingWindowRadioButton;
        private System.Windows.Forms.GroupBox _groupBox;
		private DescriptiveSpinControl _fromSliding;
		private DescriptiveSpinControl _toSliding;
    }
}
