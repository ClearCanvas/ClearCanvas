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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranscriptionComponentControl));
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
			((System.ComponentModel.ISupportInitialize)(this._reportEditorSplitContainer)).BeginInit();
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
			resources.ApplyResources(this._reportEditorSplitContainer, "_reportEditorSplitContainer");
			this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
			// 
			// _reportEditorSplitContainer.Panel1
			// 
			this._reportEditorSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// _reportEditorSplitContainer.Panel2
			// 
			this._reportEditorSplitContainer.Panel2.Controls.Add(this._rightHandPanel);
			this._reportEditorSplitContainer.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this._supervisor, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this._transcriptiontEditorPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// _supervisor
			// 
			resources.ApplyResources(this._supervisor, "_supervisor");
			this.tableLayoutPanel2.SetColumnSpan(this._supervisor, 2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Value = null;
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _transcriptiontEditorPanel
			// 
			resources.ApplyResources(this._transcriptiontEditorPanel, "_transcriptiontEditorPanel");
			this.tableLayoutPanel2.SetColumnSpan(this._transcriptiontEditorPanel, 2);
			this._transcriptiontEditorPanel.Name = "_transcriptiontEditorPanel";
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._completeButton);
			this.flowLayoutPanel1.Controls.Add(this._rejectButton);
			this.flowLayoutPanel1.Controls.Add(this._submitForReviewButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._btnSkip);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _completeButton
			// 
			resources.ApplyResources(this._completeButton, "_completeButton");
			this._completeButton.Name = "_completeButton";
			this._completeButton.UseVisualStyleBackColor = true;
			this._completeButton.Click += new System.EventHandler(this._completeButton_Click);
			// 
			// _rejectButton
			// 
			resources.ApplyResources(this._rejectButton, "_rejectButton");
			this._rejectButton.Name = "_rejectButton";
			this._rejectButton.UseVisualStyleBackColor = true;
			this._rejectButton.Click += new System.EventHandler(this._rejectButton_Click);
			// 
			// _submitForReviewButton
			// 
			resources.ApplyResources(this._submitForReviewButton, "_submitForReviewButton");
			this._submitForReviewButton.Name = "_submitForReviewButton";
			this._submitForReviewButton.UseVisualStyleBackColor = true;
			this._submitForReviewButton.Click += new System.EventHandler(this._submitForReviewButton_Click);
			// 
			// _saveButton
			// 
			resources.ApplyResources(this._saveButton, "_saveButton");
			this._saveButton.Name = "_saveButton";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _btnSkip
			// 
			resources.ApplyResources(this._btnSkip, "_btnSkip");
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _reportNextItem
			// 
			resources.ApplyResources(this._reportNextItem, "_reportNextItem");
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _rightHandPanel
			// 
			resources.ApplyResources(this._rightHandPanel, "_rightHandPanel");
			this._rightHandPanel.Name = "_rightHandPanel";
			// 
			// _statusText
			// 
			resources.ApplyResources(this._statusText, "_statusText");
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Name = "_statusText";
			// 
			// _overviewLayoutPanel
			// 
			resources.ApplyResources(this._overviewLayoutPanel, "_overviewLayoutPanel");
			this._overviewLayoutPanel.Controls.Add(this._statusText, 0, 1);
			this._overviewLayoutPanel.Controls.Add(this._reportEditorSplitContainer, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			// 
			// _bannerPanel
			// 
			resources.ApplyResources(this._bannerPanel, "_bannerPanel");
			this._bannerPanel.Name = "_bannerPanel";
			// 
			// TranscriptionComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "TranscriptionComponentControl";
			this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
			this._reportEditorSplitContainer.Panel1.PerformLayout();
			this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._reportEditorSplitContainer)).EndInit();
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
