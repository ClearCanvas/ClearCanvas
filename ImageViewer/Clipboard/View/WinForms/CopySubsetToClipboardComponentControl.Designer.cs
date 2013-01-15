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

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    partial class CopySubsetToClipboardComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopySubsetToClipboardComponentControl));
			this._radioCopyCustom = new System.Windows.Forms.RadioButton();
			this._radioCopyRange = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this._copyRangeStart = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._copyRangeEnd = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._customRange = new System.Windows.Forms.TextBox();
			this._radioCopyRangeAll = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this._radioCopyRangeAtInterval = new System.Windows.Forms.RadioButton();
			this._copyRangeInterval = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._sendToClipboardButton = new System.Windows.Forms.Button();
			this._betweenGroup = new System.Windows.Forms.GroupBox();
			this._rangeSelectionGroup = new System.Windows.Forms.GroupBox();
			this._radioUsePositionNumber = new System.Windows.Forms.RadioButton();
			this._radioUseInstanceNumber = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this._sourceDisplaySet = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeEnd)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeInterval)).BeginInit();
			this._betweenGroup.SuspendLayout();
			this._rangeSelectionGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _radioCopyCustom
			// 
			resources.ApplyResources(this._radioCopyCustom, "_radioCopyCustom");
			this._radioCopyCustom.Name = "_radioCopyCustom";
			this._radioCopyCustom.UseVisualStyleBackColor = true;
			// 
			// _radioCopyRange
			// 
			resources.ApplyResources(this._radioCopyRange, "_radioCopyRange");
			this._radioCopyRange.Name = "_radioCopyRange";
			this._radioCopyRange.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _copyRangeStart
			// 
			resources.ApplyResources(this._copyRangeStart, "_copyRangeStart");
			this._copyRangeStart.Name = "_copyRangeStart";
			// 
			// _copyRangeEnd
			// 
			resources.ApplyResources(this._copyRangeEnd, "_copyRangeEnd");
			this._copyRangeEnd.Name = "_copyRangeEnd";
			// 
			// _customRange
			// 
			resources.ApplyResources(this._customRange, "_customRange");
			this._customRange.Name = "_customRange";
			// 
			// _radioCopyRangeAll
			// 
			resources.ApplyResources(this._radioCopyRangeAll, "_radioCopyRangeAll");
			this._radioCopyRangeAll.Name = "_radioCopyRangeAll";
			this._radioCopyRangeAll.UseVisualStyleBackColor = true;
			this._radioCopyRangeAll.CheckedChanged += new System.EventHandler(this.OnRangeCopyAllImagesCheckedChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _radioCopyRangeAtInterval
			// 
			resources.ApplyResources(this._radioCopyRangeAtInterval, "_radioCopyRangeAtInterval");
			this._radioCopyRangeAtInterval.Name = "_radioCopyRangeAtInterval";
			this._radioCopyRangeAtInterval.UseVisualStyleBackColor = true;
			this._radioCopyRangeAtInterval.CheckedChanged += new System.EventHandler(this.OnRangeCopyAtIntervalCheckedChanged);
			// 
			// _copyRangeInterval
			// 
			resources.ApplyResources(this._copyRangeInterval, "_copyRangeInterval");
			this._copyRangeInterval.Name = "_copyRangeInterval";
			// 
			// _sendToClipboardButton
			// 
			resources.ApplyResources(this._sendToClipboardButton, "_sendToClipboardButton");
			this._sendToClipboardButton.Name = "_sendToClipboardButton";
			this._sendToClipboardButton.UseVisualStyleBackColor = true;
			this._sendToClipboardButton.Click += new System.EventHandler(this.OnSendToClipboard);
			// 
			// _betweenGroup
			// 
			this._betweenGroup.Controls.Add(this._radioCopyRangeAll);
			this._betweenGroup.Controls.Add(this._copyRangeInterval);
			this._betweenGroup.Controls.Add(this.label1);
			this._betweenGroup.Controls.Add(this._radioCopyRangeAtInterval);
			resources.ApplyResources(this._betweenGroup, "_betweenGroup");
			this._betweenGroup.Name = "_betweenGroup";
			this._betweenGroup.TabStop = false;
			// 
			// _rangeSelectionGroup
			// 
			this._rangeSelectionGroup.Controls.Add(this._radioUsePositionNumber);
			this._rangeSelectionGroup.Controls.Add(this._radioUseInstanceNumber);
			resources.ApplyResources(this._rangeSelectionGroup, "_rangeSelectionGroup");
			this._rangeSelectionGroup.Name = "_rangeSelectionGroup";
			this._rangeSelectionGroup.TabStop = false;
			// 
			// _radioUsePositionNumber
			// 
			resources.ApplyResources(this._radioUsePositionNumber, "_radioUsePositionNumber");
			this._radioUsePositionNumber.Name = "_radioUsePositionNumber";
			this._radioUsePositionNumber.UseVisualStyleBackColor = true;
			// 
			// _radioUseInstanceNumber
			// 
			resources.ApplyResources(this._radioUseInstanceNumber, "_radioUseInstanceNumber");
			this._radioUseInstanceNumber.Name = "_radioUseInstanceNumber";
			this._radioUseInstanceNumber.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// _sourceDisplaySet
			// 
			this._sourceDisplaySet.AutoEllipsis = true;
			this._sourceDisplaySet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this._sourceDisplaySet, "_sourceDisplaySet");
			this._sourceDisplaySet.Name = "_sourceDisplaySet";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// CopySubsetToClipboardComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sourceDisplaySet);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._rangeSelectionGroup);
			this.Controls.Add(this._sendToClipboardButton);
			this.Controls.Add(this._customRange);
			this.Controls.Add(this._copyRangeEnd);
			this.Controls.Add(this._copyRangeStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._radioCopyRange);
			this.Controls.Add(this._radioCopyCustom);
			this.Controls.Add(this._betweenGroup);
			this.Controls.Add(this.label4);
			this.Name = "CopySubsetToClipboardComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._copyRangeStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeEnd)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeInterval)).EndInit();
			this._betweenGroup.ResumeLayout(false);
			this._betweenGroup.PerformLayout();
			this._rangeSelectionGroup.ResumeLayout(false);
			this._rangeSelectionGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.RadioButton _radioCopyCustom;
		private System.Windows.Forms.RadioButton _radioCopyRange;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeStart;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeEnd;
		private System.Windows.Forms.TextBox _customRange;
		private System.Windows.Forms.RadioButton _radioCopyRangeAll;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton _radioCopyRangeAtInterval;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeInterval;
		private System.Windows.Forms.Button _sendToClipboardButton;
		private System.Windows.Forms.GroupBox _betweenGroup;
		private System.Windows.Forms.GroupBox _rangeSelectionGroup;
		private System.Windows.Forms.RadioButton _radioUsePositionNumber;
		private System.Windows.Forms.RadioButton _radioUseInstanceNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label _sourceDisplaySet;
		private System.Windows.Forms.Label label4;
    }
}
