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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorklistTimeWindowEditorComponentControl));
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
			resources.ApplyResources(this._slidingScale, "_slidingScale");
			this._slidingScale.Name = "_slidingScale";
			this._slidingScale.Value = null;
			// 
			// _fromCheckBox
			// 
			resources.ApplyResources(this._fromCheckBox, "_fromCheckBox");
			this._fromCheckBox.Name = "_fromCheckBox";
			this._fromCheckBox.UseVisualStyleBackColor = true;
			// 
			// _toCheckBox
			// 
			resources.ApplyResources(this._toCheckBox, "_toCheckBox");
			this._toCheckBox.Name = "_toCheckBox";
			this._toCheckBox.UseVisualStyleBackColor = true;
			// 
			// _toFixed
			// 
			resources.ApplyResources(this._toFixed, "_toFixed");
			this._toFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._toFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._toFixed.Name = "_toFixed";
			this._toFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
			// 
			// _fromFixed
			// 
			resources.ApplyResources(this._fromFixed, "_fromFixed");
			this._fromFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._fromFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._fromFixed.Name = "_fromFixed";
			this._fromFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
			// 
			// _fixedWindowRadioButton
			// 
			resources.ApplyResources(this._fixedWindowRadioButton, "_fixedWindowRadioButton");
			this._fixedWindowRadioButton.Name = "_fixedWindowRadioButton";
			this._fixedWindowRadioButton.TabStop = true;
			this._fixedWindowRadioButton.UseVisualStyleBackColor = true;
			// 
			// _slidingWindowRadioButton
			// 
			resources.ApplyResources(this._slidingWindowRadioButton, "_slidingWindowRadioButton");
			this._slidingWindowRadioButton.Name = "_slidingWindowRadioButton";
			this._slidingWindowRadioButton.TabStop = true;
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
			resources.ApplyResources(this._groupBox, "_groupBox");
			this._groupBox.Name = "_groupBox";
			this._groupBox.TabStop = false;
			// 
			// _toSliding
			// 
			this._toSliding.Format = null;
			resources.ApplyResources(this._toSliding, "_toSliding");
			this._toSliding.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this._toSliding.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
			this._toSliding.Name = "_toSliding";
			this._toSliding.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// _fromSliding
			// 
			this._fromSliding.Format = null;
			resources.ApplyResources(this._fromSliding, "_fromSliding");
			this._fromSliding.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this._fromSliding.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
			this._fromSliding.Name = "_fromSliding";
			this._fromSliding.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// WorklistTimeWindowEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupBox);
			this.Name = "WorklistTimeWindowEditorComponentControl";
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
