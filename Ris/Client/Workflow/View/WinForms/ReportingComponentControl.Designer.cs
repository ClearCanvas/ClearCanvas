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
    partial class ReportingComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportingComponentControl));
			this._reportEditorSplitContainer = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._hasErrors = new System.Windows.Forms.Label();
			this._imagesUnavailable = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this._reportEditorPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._verifyButton = new System.Windows.Forms.Button();
			this._submitForReviewButton = new System.Windows.Forms.Button();
			this._sendToTranscriptionButton = new System.Windows.Forms.Button();
			this._returnToInterpreterButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._skipButton = new System.Windows.Forms.Button();
			this._reportNextItem = new System.Windows.Forms.CheckBox();
			this._reportedProcedures = new System.Windows.Forms.Label();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._rememberSupervisorCheckbox = new System.Windows.Forms.CheckBox();
			this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._statusText = new System.Windows.Forms.Label();
			this._bannerPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this._reportEditorSplitContainer)).BeginInit();
			this._reportEditorSplitContainer.Panel1.SuspendLayout();
			this._reportEditorSplitContainer.Panel2.SuspendLayout();
			this._reportEditorSplitContainer.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
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
			this.tableLayoutPanel2.Controls.Add(this._hasErrors, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._imagesUnavailable, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportEditorPanel, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportedProcedures, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 4);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// _hasErrors
			// 
			resources.ApplyResources(this._hasErrors, "_hasErrors");
			this._hasErrors.BackColor = System.Drawing.Color.White;
			this._hasErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._hasErrors, 2);
			this._hasErrors.ForeColor = System.Drawing.Color.Red;
			this._hasErrors.Name = "_hasErrors";
			// 
			// _imagesUnavailable
			// 
			resources.ApplyResources(this._imagesUnavailable, "_imagesUnavailable");
			this._imagesUnavailable.BackColor = System.Drawing.Color.White;
			this._imagesUnavailable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._imagesUnavailable, 2);
			this._imagesUnavailable.ForeColor = System.Drawing.Color.Red;
			this._imagesUnavailable.Name = "_imagesUnavailable";
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _reportEditorPanel
			// 
			resources.ApplyResources(this._reportEditorPanel, "_reportEditorPanel");
			this.tableLayoutPanel2.SetColumnSpan(this._reportEditorPanel, 2);
			this._reportEditorPanel.Name = "_reportEditorPanel";
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._verifyButton);
			this.flowLayoutPanel1.Controls.Add(this._submitForReviewButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToTranscriptionButton);
			this.flowLayoutPanel1.Controls.Add(this._returnToInterpreterButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._skipButton);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _verifyButton
			// 
			resources.ApplyResources(this._verifyButton, "_verifyButton");
			this._verifyButton.Name = "_verifyButton";
			this._verifyButton.UseVisualStyleBackColor = true;
			this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
			// 
			// _submitForReviewButton
			// 
			resources.ApplyResources(this._submitForReviewButton, "_submitForReviewButton");
			this._submitForReviewButton.Name = "_submitForReviewButton";
			this._submitForReviewButton.UseVisualStyleBackColor = true;
			this._submitForReviewButton.Click += new System.EventHandler(this._submitForReviewButton_Click);
			// 
			// _sendToTranscriptionButton
			// 
			resources.ApplyResources(this._sendToTranscriptionButton, "_sendToTranscriptionButton");
			this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
			this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
			this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
			// 
			// _returnToInterpreterButton
			// 
			resources.ApplyResources(this._returnToInterpreterButton, "_returnToInterpreterButton");
			this._returnToInterpreterButton.Name = "_returnToInterpreterButton";
			this._returnToInterpreterButton.UseVisualStyleBackColor = true;
			this._returnToInterpreterButton.Click += new System.EventHandler(this._sendToInterpreterButton_Click);
			// 
			// _saveButton
			// 
			resources.ApplyResources(this._saveButton, "_saveButton");
			this._saveButton.Name = "_saveButton";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _skipButton
			// 
			resources.ApplyResources(this._skipButton, "_skipButton");
			this._skipButton.Name = "_skipButton";
			this._skipButton.UseVisualStyleBackColor = true;
			this._skipButton.Click += new System.EventHandler(this._skipButton_Click);
			// 
			// _reportNextItem
			// 
			resources.ApplyResources(this._reportNextItem, "_reportNextItem");
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _reportedProcedures
			// 
			resources.ApplyResources(this._reportedProcedures, "_reportedProcedures");
			this._reportedProcedures.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel2.SetColumnSpan(this._reportedProcedures, 2);
			this._reportedProcedures.ForeColor = System.Drawing.SystemColors.ControlText;
			this._reportedProcedures.Name = "_reportedProcedures";
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
			this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 2);
			this.tableLayoutPanel3.Controls.Add(this._supervisor, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._rememberSupervisorCheckbox, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this._priority, 0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// _supervisor
			// 
			resources.ApplyResources(this._supervisor, "_supervisor");
			this._supervisor.Name = "_supervisor";
			this._supervisor.Value = null;
			// 
			// _rememberSupervisorCheckbox
			// 
			resources.ApplyResources(this._rememberSupervisorCheckbox, "_rememberSupervisorCheckbox");
			this._rememberSupervisorCheckbox.Name = "_rememberSupervisorCheckbox";
			this._rememberSupervisorCheckbox.UseVisualStyleBackColor = true;
			// 
			// _priority
			// 
			this._priority.DataSource = null;
			this._priority.DisplayMember = "";
			this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._priority, "_priority");
			this._priority.Name = "_priority";
			this._priority.Value = null;
			// 
			// _rightHandPanel
			// 
			resources.ApplyResources(this._rightHandPanel, "_rightHandPanel");
			this._rightHandPanel.Name = "_rightHandPanel";
			// 
			// _overviewLayoutPanel
			// 
			resources.ApplyResources(this._overviewLayoutPanel, "_overviewLayoutPanel");
			this._overviewLayoutPanel.Controls.Add(this._statusText, 0, 1);
			this._overviewLayoutPanel.Controls.Add(this._reportEditorSplitContainer, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			// 
			// _statusText
			// 
			resources.ApplyResources(this._statusText, "_statusText");
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Name = "_statusText";
			// 
			// _bannerPanel
			// 
			resources.ApplyResources(this._bannerPanel, "_bannerPanel");
			this._bannerPanel.Name = "_bannerPanel";
			// 
			// ReportingComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "ReportingComponentControl";
			this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
			this._reportEditorSplitContainer.Panel1.PerformLayout();
			this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._reportEditorSplitContainer)).EndInit();
			this._reportEditorSplitContainer.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this._overviewLayoutPanel.ResumeLayout(false);
			this._overviewLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
        private System.Windows.Forms.TableLayoutPanel _overviewLayoutPanel;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.Panel _reportEditorPanel;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _submitForReviewButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _skipButton;
		private System.Windows.Forms.CheckBox _reportNextItem;
		private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.Label _reportedProcedures;
		private System.Windows.Forms.Label _imagesUnavailable;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.Label _hasErrors;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.CheckBox _rememberSupervisorCheckbox;
		private System.Windows.Forms.Button _returnToInterpreterButton;
		private Desktop.View.WinForms.ComboBoxField _priority;
    }
}
