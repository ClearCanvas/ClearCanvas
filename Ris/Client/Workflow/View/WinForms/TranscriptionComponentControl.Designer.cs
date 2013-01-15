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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class TranscriptionComponentControl
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
			this._reportEditorSplitContainer = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._transcriptiontEditorPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._completeButton = new System.Windows.Forms.Button();
			this._rejectButton = new System.Windows.Forms.Button();
			this._submitForReviewButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._reportNextItem = new System.Windows.Forms.CheckBox();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._statusText = new System.Windows.Forms.Label();
			this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this._reportEditorSplitContainer.Panel1.SuspendLayout();
			this._reportEditorSplitContainer.Panel2.SuspendLayout();
			this._reportEditorSplitContainer.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this._overviewLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _reportEditorSplitContainer
			// 
			this._reportEditorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorSplitContainer.Location = new System.Drawing.Point(3, 123);
			this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
			// 
			// _reportEditorSplitContainer.Panel1
			// 
			this._reportEditorSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// _reportEditorSplitContainer.Panel2
			// 
			this._reportEditorSplitContainer.Panel2.Controls.Add(this._rightHandPanel);
			this._reportEditorSplitContainer.Size = new System.Drawing.Size(977, 791);
			this._reportEditorSplitContainer.SplitterDistance = 461;
			this._reportEditorSplitContainer.TabIndex = 0;
			this._reportEditorSplitContainer.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this._supervisor, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this._transcriptiontEditorPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 3);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(461, 791);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _supervisor
			// 
			this._supervisor.AutoSize = true;
			this._supervisor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._supervisor, 2);
			this._supervisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._supervisor.LabelText = "Supervising Transcriptionist:";
			this._supervisor.Location = new System.Drawing.Point(2, 688);
			this._supervisor.Margin = new System.Windows.Forms.Padding(2, 2, 25, 2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Size = new System.Drawing.Size(434, 43);
			this._supervisor.TabIndex = 4;
			this._supervisor.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(383, 736);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _transcriptiontEditorPanel
			// 
			this._transcriptiontEditorPanel.AutoSize = true;
			this._transcriptiontEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._transcriptiontEditorPanel, 2);
			this._transcriptiontEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._transcriptiontEditorPanel.Location = new System.Drawing.Point(3, 3);
			this._transcriptiontEditorPanel.Name = "_transcriptiontEditorPanel";
			this._transcriptiontEditorPanel.Size = new System.Drawing.Size(455, 680);
			this._transcriptiontEditorPanel.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._completeButton);
			this.flowLayoutPanel1.Controls.Add(this._rejectButton);
			this.flowLayoutPanel1.Controls.Add(this._submitForReviewButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._btnSkip);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 733);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(380, 58);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _completeButton
			// 
			this._completeButton.Location = new System.Drawing.Point(3, 3);
			this._completeButton.Name = "_completeButton";
			this._completeButton.Size = new System.Drawing.Size(75, 23);
			this._completeButton.TabIndex = 0;
			this._completeButton.Text = "Complete";
			this._completeButton.UseVisualStyleBackColor = true;
			this._completeButton.Click += new System.EventHandler(this._completeButton_Click);
			// 
			// _rejectButton
			// 
			this._rejectButton.Location = new System.Drawing.Point(84, 3);
			this._rejectButton.Name = "_rejectButton";
			this._rejectButton.Size = new System.Drawing.Size(75, 23);
			this._rejectButton.TabIndex = 2;
			this._rejectButton.Text = "Reject";
			this._rejectButton.UseVisualStyleBackColor = true;
			this._rejectButton.Click += new System.EventHandler(this._rejectButton_Click);
			// 
			// _submitForReviewButton
			// 
			this._submitForReviewButton.Location = new System.Drawing.Point(165, 3);
			this._submitForReviewButton.Name = "_submitForReviewButton";
			this._submitForReviewButton.Size = new System.Drawing.Size(103, 23);
			this._submitForReviewButton.TabIndex = 9;
			this._submitForReviewButton.Text = "Submit for Review";
			this._submitForReviewButton.UseVisualStyleBackColor = true;
			this._submitForReviewButton.Click += new System.EventHandler(this._submitForReviewButton_Click);
			// 
			// _saveButton
			// 
			this._saveButton.Location = new System.Drawing.Point(274, 3);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(75, 23);
			this._saveButton.TabIndex = 3;
			this._saveButton.Text = "Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(3, 32);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 7;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _reportNextItem
			// 
			this._reportNextItem.AutoSize = true;
			this._reportNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportNextItem.Location = new System.Drawing.Point(84, 32);
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.Size = new System.Drawing.Size(104, 23);
			this._reportNextItem.TabIndex = 8;
			this._reportNextItem.Text = "Go To Next Item";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(512, 791);
			this._rightHandPanel.TabIndex = 0;
			// 
			// _statusText
			// 
			this._statusText.AutoSize = true;
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Location = new System.Drawing.Point(3, 98);
			this._statusText.Margin = new System.Windows.Forms.Padding(3);
			this._statusText.Name = "_statusText";
			this._statusText.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._statusText.Size = new System.Drawing.Size(977, 19);
			this._statusText.TabIndex = 2;
			this._statusText.Text = "Transcribing from X worklist - Y items available - Z items completed";
			// 
			// _overviewLayoutPanel
			// 
			this._overviewLayoutPanel.ColumnCount = 1;
			this._overviewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.Controls.Add(this._statusText, 0, 1);
			this._overviewLayoutPanel.Controls.Add(this._reportEditorSplitContainer, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
			this._overviewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._overviewLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			this._overviewLayoutPanel.RowCount = 3;
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.Size = new System.Drawing.Size(983, 917);
			this._overviewLayoutPanel.TabIndex = 1;
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(977, 89);
			this._bannerPanel.TabIndex = 0;
			// 
			// TranscriptionComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "TranscriptionComponentControl";
			this.Size = new System.Drawing.Size(983, 917);
			this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
			this._reportEditorSplitContainer.Panel1.PerformLayout();
			this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
			this._reportEditorSplitContainer.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this._overviewLayoutPanel.ResumeLayout(false);
			this._overviewLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Panel _transcriptiontEditorPanel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _completeButton;
		private System.Windows.Forms.Button _rejectButton;
		private System.Windows.Forms.Button _saveButton;
		private System.Windows.Forms.Button _btnSkip;
		private System.Windows.Forms.CheckBox _reportNextItem;
		private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.TableLayoutPanel _overviewLayoutPanel;
		private System.Windows.Forms.Panel _bannerPanel;
		private System.Windows.Forms.Button _submitForReviewButton;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
    }
}
