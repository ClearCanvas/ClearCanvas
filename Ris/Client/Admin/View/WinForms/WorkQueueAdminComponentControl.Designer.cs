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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class WorkQueueAdminComponentControl
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
			this._queue = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._messageTypeDroplist = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._scheduledOption = new System.Windows.Forms.RadioButton();
			this._previewPanel = new System.Windows.Forms.Panel();
			this._statusDroplist = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
			this._processedOption = new System.Windows.Forms.RadioButton();
			this._filterGroupBox = new System.Windows.Forms.GroupBox();
			this._showAll = new System.Windows.Forms.Button();
			this._searchButton = new System.Windows.Forms.Button();
			this._timeGroupBox = new System.Windows.Forms.GroupBox();
			this._startTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._endTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this._filterGroupBox.SuspendLayout();
			this._timeGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _queue
			// 
			this._queue.AutoSize = true;
			this._queue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._queue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._queue.ColumnHeaderTooltip = null;
			this._queue.Dock = System.Windows.Forms.DockStyle.Fill;
			this._queue.Location = new System.Drawing.Point(3, 173);
			this._queue.MultiSelect = false;
			this._queue.Name = "_queue";
			this._queue.ReadOnly = false;
			this._queue.Size = new System.Drawing.Size(643, 707);
			this._queue.SortButtonTooltip = null;
			this._queue.TabIndex = 0;
			this._queue.SelectionChanged += new System.EventHandler(this._queue_SelectionChanged);
			// 
			// _messageTypeDroplist
			// 
			this._messageTypeDroplist.AutoSize = true;
			this._messageTypeDroplist.LabelText = "Type";
			this._messageTypeDroplist.Location = new System.Drawing.Point(5, 18);
			this._messageTypeDroplist.Margin = new System.Windows.Forms.Padding(2);
			this._messageTypeDroplist.Name = "_messageTypeDroplist";
			this._messageTypeDroplist.Size = new System.Drawing.Size(250, 42);
			this._messageTypeDroplist.TabIndex = 2;
			// 
			// _scheduledOption
			// 
			this._scheduledOption.AutoSize = true;
			this._scheduledOption.Checked = true;
			this._scheduledOption.Location = new System.Drawing.Point(10, 19);
			this._scheduledOption.Name = "_scheduledOption";
			this._scheduledOption.Size = new System.Drawing.Size(76, 17);
			this._scheduledOption.TabIndex = 0;
			this._scheduledOption.TabStop = true;
			this._scheduledOption.Text = "Scheduled";
			this._scheduledOption.UseVisualStyleBackColor = true;
			// 
			// _previewPanel
			// 
			this._previewPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._previewPanel.Location = new System.Drawing.Point(0, 12);
			this._previewPanel.Margin = new System.Windows.Forms.Padding(0);
			this._previewPanel.Name = "_previewPanel";
			this._previewPanel.Padding = new System.Windows.Forms.Padding(1);
			this._previewPanel.Size = new System.Drawing.Size(740, 870);
			this._previewPanel.TabIndex = 0;
			// 
			// _statusDroplist
			// 
			this._statusDroplist.AutoSize = true;
			this._statusDroplist.LabelText = "Status";
			this._statusDroplist.Location = new System.Drawing.Point(274, 18);
			this._statusDroplist.Margin = new System.Windows.Forms.Padding(2);
			this._statusDroplist.Name = "_statusDroplist";
			this._statusDroplist.Size = new System.Drawing.Size(250, 42);
			this._statusDroplist.TabIndex = 3;
			// 
			// _processedOption
			// 
			this._processedOption.AutoSize = true;
			this._processedOption.Location = new System.Drawing.Point(10, 42);
			this._processedOption.Name = "_processedOption";
			this._processedOption.Size = new System.Drawing.Size(75, 17);
			this._processedOption.TabIndex = 1;
			this._processedOption.Text = "Processed";
			this._processedOption.UseVisualStyleBackColor = true;
			// 
			// _filterGroupBox
			// 
			this._filterGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._filterGroupBox.Controls.Add(this._showAll);
			this._filterGroupBox.Controls.Add(this._searchButton);
			this._filterGroupBox.Controls.Add(this._timeGroupBox);
			this._filterGroupBox.Controls.Add(this._messageTypeDroplist);
			this._filterGroupBox.Controls.Add(this._statusDroplist);
			this._filterGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._filterGroupBox.Location = new System.Drawing.Point(3, 3);
			this._filterGroupBox.Name = "_filterGroupBox";
			this._filterGroupBox.Size = new System.Drawing.Size(643, 164);
			this._filterGroupBox.TabIndex = 1;
			this._filterGroupBox.TabStop = false;
			this._filterGroupBox.Text = "Filter";
			// 
			// _showAll
			// 
			this._showAll.Location = new System.Drawing.Point(448, 138);
			this._showAll.Name = "_showAll";
			this._showAll.Size = new System.Drawing.Size(75, 23);
			this._showAll.TabIndex = 6;
			this._showAll.Text = "Clear";
			this._showAll.UseVisualStyleBackColor = true;
			this._showAll.Click += new System.EventHandler(this._showAll_Click);
			// 
			// _searchButton
			// 
			this._searchButton.Location = new System.Drawing.Point(367, 138);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(75, 23);
			this._searchButton.TabIndex = 5;
			this._searchButton.Text = "Apply";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _timeGroupBox
			// 
			this._timeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._timeGroupBox.Controls.Add(this._processedOption);
			this._timeGroupBox.Controls.Add(this._scheduledOption);
			this._timeGroupBox.Controls.Add(this._startTime);
			this._timeGroupBox.Controls.Add(this._endTime);
			this._timeGroupBox.Location = new System.Drawing.Point(5, 63);
			this._timeGroupBox.Name = "_timeGroupBox";
			this._timeGroupBox.Size = new System.Drawing.Size(518, 69);
			this._timeGroupBox.TabIndex = 4;
			this._timeGroupBox.TabStop = false;
			this._timeGroupBox.Text = "Time";
			// 
			// _startTime
			// 
			this._startTime.AutoSize = true;
			this._startTime.LabelText = "Start";
			this._startTime.Location = new System.Drawing.Point(90, 19);
			this._startTime.Margin = new System.Windows.Forms.Padding(2);
			this._startTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._startTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._startTime.Name = "_startTime";
			this._startTime.Nullable = true;
			this._startTime.ShowTime = true;
			this._startTime.Size = new System.Drawing.Size(211, 40);
			this._startTime.TabIndex = 2;
			this._startTime.Value = null;
			// 
			// _endTime
			// 
			this._endTime.AutoSize = true;
			this._endTime.LabelText = "End";
			this._endTime.Location = new System.Drawing.Point(305, 19);
			this._endTime.Margin = new System.Windows.Forms.Padding(2);
			this._endTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._endTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._endTime.Name = "_endTime";
			this._endTime.Nullable = true;
			this._endTime.ShowTime = true;
			this._endTime.Size = new System.Drawing.Size(211, 40);
			this._endTime.TabIndex = 3;
			this._endTime.Value = null;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
			this.splitContainer1.Panel1MinSize = 655;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._previewPanel);
			this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 12, 3, 7);
			this.splitContainer1.Size = new System.Drawing.Size(1402, 889);
			this.splitContainer1.SplitterDistance = 655;
			this.splitContainer1.TabIndex = 2;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._queue, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this._filterGroupBox, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(649, 883);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(3, 3);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(15, 14);
			this.checkBox1.TabIndex = 1;
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// WorkQueueAdminComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.checkBox1);
			this.Name = "WorkQueueAdminComponentControl";
			this.Size = new System.Drawing.Size(1402, 889);
			this._filterGroupBox.ResumeLayout(false);
			this._filterGroupBox.PerformLayout();
			this._timeGroupBox.ResumeLayout(false);
			this._timeGroupBox.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _queue;
		private ClearCanvas.Desktop.View.WinForms.DropListPickerField _messageTypeDroplist;
		private System.Windows.Forms.RadioButton _scheduledOption;
		private System.Windows.Forms.Panel _previewPanel;
		private ClearCanvas.Desktop.View.WinForms.DropListPickerField _statusDroplist;
		private System.Windows.Forms.RadioButton _processedOption;
		private System.Windows.Forms.GroupBox _filterGroupBox;
		private System.Windows.Forms.Button _showAll;
		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.GroupBox _timeGroupBox;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _startTime;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _endTime;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.CheckBox checkBox1;
    }
}
