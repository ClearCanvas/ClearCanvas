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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkQueueAdminComponentControl));
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
			resources.ApplyResources(this._queue, "_queue");
			this._queue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._queue.MultiSelect = false;
			this._queue.Name = "_queue";
			this._queue.ReadOnly = false;
			this._queue.SelectionChanged += new System.EventHandler(this._queue_SelectionChanged);
			// 
			// _messageTypeDroplist
			// 
			resources.ApplyResources(this._messageTypeDroplist, "_messageTypeDroplist");
			this._messageTypeDroplist.Name = "_messageTypeDroplist";
			// 
			// _scheduledOption
			// 
			resources.ApplyResources(this._scheduledOption, "_scheduledOption");
			this._scheduledOption.Checked = true;
			this._scheduledOption.Name = "_scheduledOption";
			this._scheduledOption.TabStop = true;
			this._scheduledOption.UseVisualStyleBackColor = true;
			// 
			// _previewPanel
			// 
			this._previewPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			resources.ApplyResources(this._previewPanel, "_previewPanel");
			this._previewPanel.Name = "_previewPanel";
			// 
			// _statusDroplist
			// 
			resources.ApplyResources(this._statusDroplist, "_statusDroplist");
			this._statusDroplist.Name = "_statusDroplist";
			// 
			// _processedOption
			// 
			resources.ApplyResources(this._processedOption, "_processedOption");
			this._processedOption.Name = "_processedOption";
			this._processedOption.UseVisualStyleBackColor = true;
			// 
			// _filterGroupBox
			// 
			resources.ApplyResources(this._filterGroupBox, "_filterGroupBox");
			this._filterGroupBox.Controls.Add(this._showAll);
			this._filterGroupBox.Controls.Add(this._searchButton);
			this._filterGroupBox.Controls.Add(this._timeGroupBox);
			this._filterGroupBox.Controls.Add(this._messageTypeDroplist);
			this._filterGroupBox.Controls.Add(this._statusDroplist);
			this._filterGroupBox.Name = "_filterGroupBox";
			this._filterGroupBox.TabStop = false;
			// 
			// _showAll
			// 
			resources.ApplyResources(this._showAll, "_showAll");
			this._showAll.Name = "_showAll";
			this._showAll.UseVisualStyleBackColor = true;
			this._showAll.Click += new System.EventHandler(this._showAll_Click);
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.Name = "_searchButton";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _timeGroupBox
			// 
			resources.ApplyResources(this._timeGroupBox, "_timeGroupBox");
			this._timeGroupBox.Controls.Add(this._processedOption);
			this._timeGroupBox.Controls.Add(this._scheduledOption);
			this._timeGroupBox.Controls.Add(this._startTime);
			this._timeGroupBox.Controls.Add(this._endTime);
			this._timeGroupBox.Name = "_timeGroupBox";
			this._timeGroupBox.TabStop = false;
			// 
			// _startTime
			// 
			resources.ApplyResources(this._startTime, "_startTime");
			this._startTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._startTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._startTime.Name = "_startTime";
			this._startTime.Nullable = true;
			this._startTime.ShowTime = true;
			this._startTime.Value = null;
			// 
			// _endTime
			// 
			resources.ApplyResources(this._endTime, "_endTime");
			this._endTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._endTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._endTime.Name = "_endTime";
			this._endTime.Nullable = true;
			this._endTime.ShowTime = true;
			this._endTime.Value = null;
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._previewPanel);
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this._queue, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this._filterGroupBox, 0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// WorkQueueAdminComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.checkBox1);
			this.Name = "WorkQueueAdminComponentControl";
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
