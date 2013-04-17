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
            this.components = new System.ComponentModel.Container();
            this._heapMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._consumeMemory = new System.Windows.Forms.Button();
            this._heldMemoryIncrement = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
            this._heldMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._useHeldMemory = new System.Windows.Forms.Button();
            this._heldMemoryRepeatCount = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
            this._releaseHeldMemory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._collect = new System.Windows.Forms.Button();
            this._consumeMaxMemory = new System.Windows.Forms.Button();
            this._markedMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._memoryDifference = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._markMemory = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._systemFreeMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._processWorkingSet = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._processPrivateBytes = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._processVirtualMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._collectAllLargeObjects = new System.Windows.Forms.Button();
            this._largeObjectRepeatCount = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
            this._releaseLarge = new System.Windows.Forms.Button();
            this._largeObjectBufferSize = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
            this._consumeLarge = new System.Windows.Forms.Button();
            this._largeObjectsHeldMemory = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nonEmptyNumericUpDown1 = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
            this.textField1 = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._heldMemoryIncrement)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._heldMemoryRepeatCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._largeObjectRepeatCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._largeObjectBufferSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nonEmptyNumericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // _heapMemory
            // 
            this._heapMemory.LabelText = "Used (MB)";
            this._heapMemory.Location = new System.Drawing.Point(5, 63);
            this._heapMemory.Margin = new System.Windows.Forms.Padding(2);
            this._heapMemory.Mask = "";
            this._heapMemory.Name = "_heapMemory";
            this._heapMemory.PasswordChar = '\0';
            this._heapMemory.ReadOnly = true;
            this._heapMemory.Size = new System.Drawing.Size(128, 41);
            this._heapMemory.TabIndex = 3;
            this._heapMemory.ToolTip = null;
            this._heapMemory.Value = null;
            // 
            // _consumeMemory
            // 
            this._consumeMemory.Location = new System.Drawing.Point(232, 28);
            this._consumeMemory.Name = "_consumeMemory";
            this._consumeMemory.Size = new System.Drawing.Size(94, 23);
            this._consumeMemory.TabIndex = 3;
            this._consumeMemory.Text = "Consume";
            this.toolTip1.SetToolTip(this._consumeMemory, "Allocate and hold the specified number of buffers.");
            this._consumeMemory.UseVisualStyleBackColor = true;
            this._consumeMemory.Click += new System.EventHandler(this._consumeIncrement_Click);
            // 
            // _heldMemoryIncrement
            // 
            this._heldMemoryIncrement.Location = new System.Drawing.Point(8, 41);
            this._heldMemoryIncrement.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this._heldMemoryIncrement.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._heldMemoryIncrement.Name = "_heldMemoryIncrement";
            this._heldMemoryIncrement.Size = new System.Drawing.Size(125, 20);
            this._heldMemoryIncrement.TabIndex = 1;
            this._heldMemoryIncrement.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _heldMemory
            // 
            this._heldMemory.LabelText = "Held Memory (MB)";
            this._heldMemory.Location = new System.Drawing.Point(4, 66);
            this._heldMemory.Margin = new System.Windows.Forms.Padding(2);
            this._heldMemory.Mask = "";
            this._heldMemory.Name = "_heldMemory";
            this._heldMemory.PasswordChar = '\0';
            this._heldMemory.ReadOnly = true;
            this._heldMemory.Size = new System.Drawing.Size(128, 41);
            this._heldMemory.TabIndex = 5;
            this._heldMemory.ToolTip = "The amount of memory being held that cannot be unloaded by memory management.";
            this._heldMemory.Value = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._useHeldMemory);
            this.groupBox1.Controls.Add(this._heldMemoryRepeatCount);
            this.groupBox1.Controls.Add(this._releaseHeldMemory);
            this.groupBox1.Controls.Add(this._heldMemoryIncrement);
            this.groupBox1.Controls.Add(this._consumeMemory);
            this.groupBox1.Controls.Add(this._heldMemory);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 115);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Held Memory";
            // 
            // _useHeldMemory
            // 
            this._useHeldMemory.Location = new System.Drawing.Point(232, 75);
            this._useHeldMemory.Name = "_useHeldMemory";
            this._useHeldMemory.Size = new System.Drawing.Size(94, 23);
            this._useHeldMemory.TabIndex = 9;
            this._useHeldMemory.Text = "Use";
            this._useHeldMemory.UseVisualStyleBackColor = true;
            this._useHeldMemory.Visible = false;
            this._useHeldMemory.Click += new System.EventHandler(this._useHeldMemory_Click);
            // 
            // _heldMemoryRepeatCount
            // 
            this._heldMemoryRepeatCount.Location = new System.Drawing.Point(156, 40);
            this._heldMemoryRepeatCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._heldMemoryRepeatCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._heldMemoryRepeatCount.Name = "_heldMemoryRepeatCount";
            this._heldMemoryRepeatCount.Size = new System.Drawing.Size(70, 20);
            this._heldMemoryRepeatCount.TabIndex = 2;
            this._heldMemoryRepeatCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _releaseHeldMemory
            // 
            this._releaseHeldMemory.Location = new System.Drawing.Point(232, 51);
            this._releaseHeldMemory.Name = "_releaseHeldMemory";
            this._releaseHeldMemory.Size = new System.Drawing.Size(94, 23);
            this._releaseHeldMemory.TabIndex = 4;
            this._releaseHeldMemory.Text = "Release";
            this.toolTip1.SetToolTip(this._releaseHeldMemory, "Release all held buffers.");
            this._releaseHeldMemory.UseVisualStyleBackColor = true;
            this._releaseHeldMemory.Click += new System.EventHandler(this._releaseHeldMemory_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Buffer Size (KB)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Repeat Count";
            // 
            // _collect
            // 
            this._collect.Location = new System.Drawing.Point(601, 267);
            this._collect.Name = "_collect";
            this._collect.Size = new System.Drawing.Size(114, 23);
            this._collect.TabIndex = 6;
            this._collect.Text = "GC Collect";
            this.toolTip1.SetToolTip(this._collect, "Force a garbage collection.");
            this._collect.UseVisualStyleBackColor = true;
            this._collect.Click += new System.EventHandler(this._collect_Click);
            // 
            // _consumeMaxMemory
            // 
            this._consumeMaxMemory.Location = new System.Drawing.Point(481, 267);
            this._consumeMaxMemory.Name = "_consumeMaxMemory";
            this._consumeMaxMemory.Size = new System.Drawing.Size(114, 23);
            this._consumeMaxMemory.TabIndex = 5;
            this._consumeMaxMemory.Text = "Consume Max";
            this.toolTip1.SetToolTip(this._consumeMaxMemory, "CAREFUL! Consumes the maximum amount of memory the system will allow.");
            this._consumeMaxMemory.UseVisualStyleBackColor = true;
            this._consumeMaxMemory.Click += new System.EventHandler(this._consumeMaxMemory_Click);
            // 
            // _markedMemory
            // 
            this._markedMemory.LabelText = "Mark (MB)";
            this._markedMemory.Location = new System.Drawing.Point(5, 18);
            this._markedMemory.Margin = new System.Windows.Forms.Padding(2);
            this._markedMemory.Mask = "";
            this._markedMemory.Name = "_markedMemory";
            this._markedMemory.PasswordChar = '\0';
            this._markedMemory.ReadOnly = true;
            this._markedMemory.Size = new System.Drawing.Size(81, 41);
            this._markedMemory.TabIndex = 1;
            this._markedMemory.ToolTip = null;
            this._markedMemory.Value = null;
            // 
            // _memoryDifference
            // 
            this._memoryDifference.LabelText = "Difference (MB)";
            this._memoryDifference.Location = new System.Drawing.Point(157, 63);
            this._memoryDifference.Margin = new System.Windows.Forms.Padding(2);
            this._memoryDifference.Mask = "";
            this._memoryDifference.Name = "_memoryDifference";
            this._memoryDifference.PasswordChar = '\0';
            this._memoryDifference.ReadOnly = true;
            this._memoryDifference.Size = new System.Drawing.Size(128, 41);
            this._memoryDifference.TabIndex = 4;
            this._memoryDifference.ToolTip = null;
            this._memoryDifference.Value = null;
            // 
            // _markMemory
            // 
            this._markMemory.Location = new System.Drawing.Point(93, 35);
            this._markMemory.Name = "_markMemory";
            this._markMemory.Size = new System.Drawing.Size(40, 23);
            this._markMemory.TabIndex = 2;
            this._markMemory.Text = "Mark";
            this._markMemory.UseVisualStyleBackColor = true;
            this._markMemory.Click += new System.EventHandler(this._markMemory_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._markedMemory);
            this.groupBox2.Controls.Add(this._heapMemory);
            this.groupBox2.Controls.Add(this._memoryDifference);
            this.groupBox2.Controls.Add(this._markMemory);
            this.groupBox2.Location = new System.Drawing.Point(14, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(345, 114);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Managed Heap";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._systemFreeMemory);
            this.groupBox3.Controls.Add(this._processWorkingSet);
            this.groupBox3.Controls.Add(this._processPrivateBytes);
            this.groupBox3.Controls.Add(this._processVirtualMemory);
            this.groupBox3.Location = new System.Drawing.Point(365, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 114);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Process";
            // 
            // _systemFreeMemory
            // 
            this._systemFreeMemory.LabelText = "System Free (MB)";
            this._systemFreeMemory.Location = new System.Drawing.Point(137, 64);
            this._systemFreeMemory.Margin = new System.Windows.Forms.Padding(2);
            this._systemFreeMemory.Mask = "";
            this._systemFreeMemory.Name = "_systemFreeMemory";
            this._systemFreeMemory.PasswordChar = '\0';
            this._systemFreeMemory.ReadOnly = true;
            this._systemFreeMemory.Size = new System.Drawing.Size(128, 41);
            this._systemFreeMemory.TabIndex = 4;
            this._systemFreeMemory.ToolTip = "The amount of free physical memory available to all processes.";
            this._systemFreeMemory.Value = null;
            // 
            // _processWorkingSet
            // 
            this._processWorkingSet.LabelText = "Working Set (MB)";
            this._processWorkingSet.Location = new System.Drawing.Point(5, 63);
            this._processWorkingSet.Margin = new System.Windows.Forms.Padding(2);
            this._processWorkingSet.Mask = "";
            this._processWorkingSet.Name = "_processWorkingSet";
            this._processWorkingSet.PasswordChar = '\0';
            this._processWorkingSet.ReadOnly = true;
            this._processWorkingSet.Size = new System.Drawing.Size(128, 41);
            this._processWorkingSet.TabIndex = 3;
            this._processWorkingSet.ToolTip = "The amount of physical memory being used by the process (RAM). This can be consid" +
    "erably smaller than what\'s \"in use\" as memory can be paged out.";
            this._processWorkingSet.Value = null;
            // 
            // _processPrivateBytes
            // 
            this._processPrivateBytes.LabelText = "Private Bytes (MB)";
            this._processPrivateBytes.Location = new System.Drawing.Point(137, 17);
            this._processPrivateBytes.Margin = new System.Windows.Forms.Padding(2);
            this._processPrivateBytes.Mask = "";
            this._processPrivateBytes.Name = "_processPrivateBytes";
            this._processPrivateBytes.PasswordChar = '\0';
            this._processPrivateBytes.ReadOnly = true;
            this._processPrivateBytes.Size = new System.Drawing.Size(128, 41);
            this._processPrivateBytes.TabIndex = 2;
            this._processPrivateBytes.ToolTip = "The total amount of memory being used by the process.";
            this._processPrivateBytes.Value = null;
            // 
            // _processVirtualMemory
            // 
            this._processVirtualMemory.LabelText = "Virtual Memory (MB)";
            this._processVirtualMemory.Location = new System.Drawing.Point(5, 18);
            this._processVirtualMemory.Margin = new System.Windows.Forms.Padding(2);
            this._processVirtualMemory.Mask = "";
            this._processVirtualMemory.Name = "_processVirtualMemory";
            this._processVirtualMemory.PasswordChar = '\0';
            this._processVirtualMemory.ReadOnly = true;
            this._processVirtualMemory.Size = new System.Drawing.Size(128, 41);
            this._processVirtualMemory.TabIndex = 1;
            this._processVirtualMemory.ToolTip = "The total virtual address space allocated to the process. This is often considera" +
    "bly larger than what\'s \"in use\".";
            this._processVirtualMemory.Value = null;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this._collectAllLargeObjects);
            this.groupBox4.Controls.Add(this._largeObjectRepeatCount);
            this.groupBox4.Controls.Add(this._releaseLarge);
            this.groupBox4.Controls.Add(this._largeObjectBufferSize);
            this.groupBox4.Controls.Add(this._consumeLarge);
            this.groupBox4.Controls.Add(this._largeObjectsHeldMemory);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(370, 146);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(345, 115);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Large Objects";
            // 
            // _collectAllLargeObjects
            // 
            this._collectAllLargeObjects.Location = new System.Drawing.Point(233, 75);
            this._collectAllLargeObjects.Name = "_collectAllLargeObjects";
            this._collectAllLargeObjects.Size = new System.Drawing.Size(94, 23);
            this._collectAllLargeObjects.TabIndex = 9;
            this._collectAllLargeObjects.Text = "Collect All";
            this.toolTip1.SetToolTip(this._collectAllLargeObjects, "Collects all large objects, including those in open viewers.");
            this._collectAllLargeObjects.UseVisualStyleBackColor = true;
            this._collectAllLargeObjects.Click += new System.EventHandler(this._useLargeObjects_Click);
            // 
            // _largeObjectRepeatCount
            // 
            this._largeObjectRepeatCount.Location = new System.Drawing.Point(157, 37);
            this._largeObjectRepeatCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._largeObjectRepeatCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._largeObjectRepeatCount.Name = "_largeObjectRepeatCount";
            this._largeObjectRepeatCount.Size = new System.Drawing.Size(70, 20);
            this._largeObjectRepeatCount.TabIndex = 2;
            this._largeObjectRepeatCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _releaseLarge
            // 
            this._releaseLarge.Location = new System.Drawing.Point(233, 51);
            this._releaseLarge.Name = "_releaseLarge";
            this._releaseLarge.Size = new System.Drawing.Size(94, 23);
            this._releaseLarge.TabIndex = 4;
            this._releaseLarge.Text = "Release";
            this.toolTip1.SetToolTip(this._releaseLarge, "Release all held \"large object\" buffers.");
            this._releaseLarge.UseVisualStyleBackColor = true;
            this._releaseLarge.Click += new System.EventHandler(this._releaseLarge_Click);
            // 
            // _largeObjectBufferSize
            // 
            this._largeObjectBufferSize.Location = new System.Drawing.Point(9, 38);
            this._largeObjectBufferSize.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this._largeObjectBufferSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._largeObjectBufferSize.Name = "_largeObjectBufferSize";
            this._largeObjectBufferSize.Size = new System.Drawing.Size(125, 20);
            this._largeObjectBufferSize.TabIndex = 1;
            this._largeObjectBufferSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // _consumeLarge
            // 
            this._consumeLarge.Location = new System.Drawing.Point(233, 28);
            this._consumeLarge.Name = "_consumeLarge";
            this._consumeLarge.Size = new System.Drawing.Size(94, 23);
            this._consumeLarge.TabIndex = 3;
            this._consumeLarge.Text = "Consume";
            this.toolTip1.SetToolTip(this._consumeLarge, "Allocate and hold the specified number of \"large object\" buffers, which can be un" +
        "loaded by \"memory management\".");
            this._consumeLarge.UseVisualStyleBackColor = true;
            this._consumeLarge.Click += new System.EventHandler(this._consumeLarge_Click);
            // 
            // _largeObjectsHeldMemory
            // 
            this._largeObjectsHeldMemory.LabelText = "Held Memory (MB)";
            this._largeObjectsHeldMemory.Location = new System.Drawing.Point(5, 63);
            this._largeObjectsHeldMemory.Margin = new System.Windows.Forms.Padding(2);
            this._largeObjectsHeldMemory.Mask = "";
            this._largeObjectsHeldMemory.Name = "_largeObjectsHeldMemory";
            this._largeObjectsHeldMemory.PasswordChar = '\0';
            this._largeObjectsHeldMemory.ReadOnly = true;
            this._largeObjectsHeldMemory.Size = new System.Drawing.Size(128, 41);
            this._largeObjectsHeldMemory.TabIndex = 5;
            this._largeObjectsHeldMemory.ToolTip = null;
            this._largeObjectsHeldMemory.Value = null;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Buffer Size (KB)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(154, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Repeat Count";
            // 
            // nonEmptyNumericUpDown1
            // 
            this.nonEmptyNumericUpDown1.Location = new System.Drawing.Point(157, 33);
            this.nonEmptyNumericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nonEmptyNumericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nonEmptyNumericUpDown1.Name = "nonEmptyNumericUpDown1";
            this.nonEmptyNumericUpDown1.Size = new System.Drawing.Size(70, 20);
            this.nonEmptyNumericUpDown1.TabIndex = 7;
            this.nonEmptyNumericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // textField1
            // 
            this.textField1.LabelText = "Held Memory (MB)";
            this.textField1.Location = new System.Drawing.Point(5, 59);
            this.textField1.Margin = new System.Windows.Forms.Padding(2);
            this.textField1.Mask = "";
            this.textField1.Name = "textField1";
            this.textField1.PasswordChar = '\0';
            this.textField1.ReadOnly = true;
            this.textField1.Size = new System.Drawing.Size(128, 41);
            this.textField1.TabIndex = 4;
            this.textField1.ToolTip = null;
            this.textField1.Value = null;
            // 
            // MemoryAnalysisComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this._collect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._consumeMaxMemory);
            this.Name = "MemoryAnalysisComponentControl";
            this.Size = new System.Drawing.Size(734, 309);
            ((System.ComponentModel.ISupportInitialize)(this._heldMemoryIncrement)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._heldMemoryRepeatCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._largeObjectRepeatCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._largeObjectBufferSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nonEmptyNumericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _heapMemory;
		private System.Windows.Forms.Button _consumeMemory;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _heldMemoryIncrement;
		private ClearCanvas.Desktop.View.WinForms.TextField _heldMemory;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button _releaseHeldMemory;
		private System.Windows.Forms.Button _collect;
		private System.Windows.Forms.Button _consumeMaxMemory;
		private ClearCanvas.Desktop.View.WinForms.TextField _markedMemory;
		private ClearCanvas.Desktop.View.WinForms.TextField _memoryDifference;
        private System.Windows.Forms.Button _markMemory;
        private System.Windows.Forms.Label label2;
        private Desktop.View.WinForms.NonEmptyNumericUpDown _heldMemoryRepeatCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private Desktop.View.WinForms.TextField _processWorkingSet;
        private Desktop.View.WinForms.TextField _processPrivateBytes;
        private Desktop.View.WinForms.TextField _processVirtualMemory;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private Desktop.View.WinForms.NonEmptyNumericUpDown _largeObjectRepeatCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button _releaseLarge;
        private Desktop.View.WinForms.NonEmptyNumericUpDown _largeObjectBufferSize;
        private System.Windows.Forms.Button _consumeLarge;
        private Desktop.View.WinForms.TextField _largeObjectsHeldMemory;
        private Desktop.View.WinForms.NonEmptyNumericUpDown nonEmptyNumericUpDown1;
        private Desktop.View.WinForms.TextField textField1;
        private Desktop.View.WinForms.TextField _systemFreeMemory;
        private System.Windows.Forms.Button _collectAllLargeObjects;
        private System.Windows.Forms.Button _useHeldMemory;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
