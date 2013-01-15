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
    partial class MemoryAnalysisComponentControl
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
			this._heapMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._consumeMemory = new System.Windows.Forms.Button();
			this._memoryIncrement = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._heldMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._releaseHeldMemory = new System.Windows.Forms.Button();
			this._collect = new System.Windows.Forms.Button();
			this._consumeMaxMemory = new System.Windows.Forms.Button();
			this._markedMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._memoryDifference = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._markMemory = new System.Windows.Forms.Button();
			this._largeObjectMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._unloadPixelData = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._memoryIncrement)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _heapMemory
			// 
			this._heapMemory.LabelText = "Heap Memory";
			this._heapMemory.Location = new System.Drawing.Point(12, 60);
			this._heapMemory.Margin = new System.Windows.Forms.Padding(2);
			this._heapMemory.Mask = "";
			this._heapMemory.Name = "_heapMemory";
			this._heapMemory.PasswordChar = '\0';
			this._heapMemory.ReadOnly = true;
			this._heapMemory.Size = new System.Drawing.Size(128, 41);
			this._heapMemory.TabIndex = 0;
			this._heapMemory.ToolTip = null;
			this._heapMemory.Value = null;
			// 
			// _consumeMemory
			// 
			this._consumeMemory.Location = new System.Drawing.Point(166, 26);
			this._consumeMemory.Name = "_consumeMemory";
			this._consumeMemory.Size = new System.Drawing.Size(94, 23);
			this._consumeMemory.TabIndex = 2;
			this._consumeMemory.Text = "Consume";
			this._consumeMemory.UseVisualStyleBackColor = true;
			this._consumeMemory.Click += new System.EventHandler(this._consumeIncrement_Click);
			// 
			// _memoryIncrement
			// 
			this._memoryIncrement.Location = new System.Drawing.Point(19, 28);
			this._memoryIncrement.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._memoryIncrement.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._memoryIncrement.Name = "_memoryIncrement";
			this._memoryIncrement.Size = new System.Drawing.Size(125, 20);
			this._memoryIncrement.TabIndex = 3;
			this._memoryIncrement.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// _heldMemory
			// 
			this._heldMemory.LabelText = "Held Memory";
			this._heldMemory.Location = new System.Drawing.Point(16, 53);
			this._heldMemory.Margin = new System.Windows.Forms.Padding(2);
			this._heldMemory.Mask = "";
			this._heldMemory.Name = "_heldMemory";
			this._heldMemory.PasswordChar = '\0';
			this._heldMemory.ReadOnly = true;
			this._heldMemory.Size = new System.Drawing.Size(128, 41);
			this._heldMemory.TabIndex = 4;
			this._heldMemory.ToolTip = null;
			this._heldMemory.Value = null;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._releaseHeldMemory);
			this.groupBox1.Controls.Add(this._memoryIncrement);
			this.groupBox1.Controls.Add(this._consumeMemory);
			this.groupBox1.Controls.Add(this._heldMemory);
			this.groupBox1.Location = new System.Drawing.Point(12, 106);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 111);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			// 
			// _releaseHeldMemory
			// 
			this._releaseHeldMemory.Location = new System.Drawing.Point(166, 69);
			this._releaseHeldMemory.Name = "_releaseHeldMemory";
			this._releaseHeldMemory.Size = new System.Drawing.Size(94, 23);
			this._releaseHeldMemory.TabIndex = 5;
			this._releaseHeldMemory.Text = "Release";
			this._releaseHeldMemory.UseVisualStyleBackColor = true;
			this._releaseHeldMemory.Click += new System.EventHandler(this._releaseHeldMemory_Click);
			// 
			// _collect
			// 
			this._collect.Location = new System.Drawing.Point(178, 261);
			this._collect.Name = "_collect";
			this._collect.Size = new System.Drawing.Size(114, 23);
			this._collect.TabIndex = 6;
			this._collect.Text = "GC Collect";
			this._collect.UseVisualStyleBackColor = true;
			this._collect.Click += new System.EventHandler(this._collect_Click);
			// 
			// _consumeMaxMemory
			// 
			this._consumeMaxMemory.Location = new System.Drawing.Point(178, 232);
			this._consumeMaxMemory.Name = "_consumeMaxMemory";
			this._consumeMaxMemory.Size = new System.Drawing.Size(114, 23);
			this._consumeMaxMemory.TabIndex = 1;
			this._consumeMaxMemory.Text = "Consume Max";
			this._consumeMaxMemory.UseVisualStyleBackColor = true;
			this._consumeMaxMemory.Click += new System.EventHandler(this._consumeMaxMemory_Click);
			// 
			// _markedMemory
			// 
			this._markedMemory.LabelText = "Mark";
			this._markedMemory.Location = new System.Drawing.Point(12, 15);
			this._markedMemory.Margin = new System.Windows.Forms.Padding(2);
			this._markedMemory.Mask = "";
			this._markedMemory.Name = "_markedMemory";
			this._markedMemory.PasswordChar = '\0';
			this._markedMemory.ReadOnly = true;
			this._markedMemory.Size = new System.Drawing.Size(81, 41);
			this._markedMemory.TabIndex = 7;
			this._markedMemory.ToolTip = null;
			this._markedMemory.Value = null;
			// 
			// _memoryDifference
			// 
			this._memoryDifference.LabelText = "Difference";
			this._memoryDifference.Location = new System.Drawing.Point(164, 60);
			this._memoryDifference.Margin = new System.Windows.Forms.Padding(2);
			this._memoryDifference.Mask = "";
			this._memoryDifference.Name = "_memoryDifference";
			this._memoryDifference.PasswordChar = '\0';
			this._memoryDifference.ReadOnly = true;
			this._memoryDifference.Size = new System.Drawing.Size(128, 41);
			this._memoryDifference.TabIndex = 8;
			this._memoryDifference.ToolTip = null;
			this._memoryDifference.Value = null;
			// 
			// _markMemory
			// 
			this._markMemory.Location = new System.Drawing.Point(100, 32);
			this._markMemory.Name = "_markMemory";
			this._markMemory.Size = new System.Drawing.Size(40, 23);
			this._markMemory.TabIndex = 6;
			this._markMemory.Text = "Mark";
			this._markMemory.UseVisualStyleBackColor = true;
			this._markMemory.Click += new System.EventHandler(this._markMemory_Click);
			// 
			// _largeObjectMemory
			// 
			this._largeObjectMemory.LabelText = "Large Objects";
			this._largeObjectMemory.Location = new System.Drawing.Point(12, 232);
			this._largeObjectMemory.Margin = new System.Windows.Forms.Padding(2);
			this._largeObjectMemory.Mask = "";
			this._largeObjectMemory.Name = "_largeObjectMemory";
			this._largeObjectMemory.PasswordChar = '\0';
			this._largeObjectMemory.ReadOnly = true;
			this._largeObjectMemory.Size = new System.Drawing.Size(128, 41);
			this._largeObjectMemory.TabIndex = 9;
			this._largeObjectMemory.ToolTip = null;
			this._largeObjectMemory.Value = null;
			// 
			// _unloadPixelData
			// 
			this._unloadPixelData.Location = new System.Drawing.Point(178, 290);
			this._unloadPixelData.Name = "_unloadPixelData";
			this._unloadPixelData.Size = new System.Drawing.Size(114, 23);
			this._unloadPixelData.TabIndex = 10;
			this._unloadPixelData.Text = "Unload Pixel Data";
			this._unloadPixelData.UseVisualStyleBackColor = true;
			this._unloadPixelData.Click += new System.EventHandler(this._unloadPixelData_Click);
			// 
			// MemoryAnalysisComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._unloadPixelData);
			this.Controls.Add(this._largeObjectMemory);
			this.Controls.Add(this._markMemory);
			this.Controls.Add(this._memoryDifference);
			this.Controls.Add(this._markedMemory);
			this.Controls.Add(this._collect);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._consumeMaxMemory);
			this.Controls.Add(this._heapMemory);
			this.Name = "MemoryAnalysisComponentControl";
			this.Size = new System.Drawing.Size(312, 336);
			((System.ComponentModel.ISupportInitialize)(this._memoryIncrement)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _heapMemory;
		private System.Windows.Forms.Button _consumeMemory;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _memoryIncrement;
		private ClearCanvas.Desktop.View.WinForms.TextField _heldMemory;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button _releaseHeldMemory;
		private System.Windows.Forms.Button _collect;
		private System.Windows.Forms.Button _consumeMaxMemory;
		private ClearCanvas.Desktop.View.WinForms.TextField _markedMemory;
		private ClearCanvas.Desktop.View.WinForms.TextField _memoryDifference;
		private System.Windows.Forms.Button _markMemory;
		private ClearCanvas.Desktop.View.WinForms.TextField _largeObjectMemory;
		private System.Windows.Forms.Button _unloadPixelData;
    }
}
