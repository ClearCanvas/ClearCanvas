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
    partial class ExternalPractitionerContactPointDetailsEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalPractitionerContactPointDetailsEditorComponentControl));
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._isDefaultContactPoint = new System.Windows.Forms.CheckBox();
			this._resultCommunicationMode = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._warning = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this._informationAuthority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
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
			this._rememberSupervisorCheckbox = new System.Windows.Forms.CheckBox();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.lookupField1 = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.SuspendLayout();
			// 
			// _name
			// 
			resources.ApplyResources(this._name, "_name");
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Value = null;
			// 
			// _description
			// 
			resources.ApplyResources(this._description, "_description");
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.Value = null;
			// 
			// _isDefaultContactPoint
			// 
			resources.ApplyResources(this._isDefaultContactPoint, "_isDefaultContactPoint");
			this._isDefaultContactPoint.Name = "_isDefaultContactPoint";
			this._isDefaultContactPoint.UseVisualStyleBackColor = true;
			// 
			// _resultCommunicationMode
			// 
			this._resultCommunicationMode.DataSource = null;
			this._resultCommunicationMode.DisplayMember = "";
			this._resultCommunicationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._resultCommunicationMode, "_resultCommunicationMode");
			this._resultCommunicationMode.Name = "_resultCommunicationMode";
			this._resultCommunicationMode.Value = null;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._warning, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _warning
			// 
			resources.ApplyResources(this._warning, "_warning");
			this._warning.BackColor = System.Drawing.Color.White;
			this._warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this._warning, 2);
			this._warning.ForeColor = System.Drawing.Color.Red;
			this._warning.Name = "_warning";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._informationAuthority);
			this.panel1.Controls.Add(this._name);
			this.panel1.Controls.Add(this._resultCommunicationMode);
			this.panel1.Controls.Add(this._description);
			this.panel1.Controls.Add(this._isDefaultContactPoint);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _informationAuthority
			// 
			this._informationAuthority.DataSource = null;
			this._informationAuthority.DisplayMember = "";
			this._informationAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this._informationAuthority, "_informationAuthority");
			this._informationAuthority.Name = "_informationAuthority";
			this._informationAuthority.Value = null;
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
			// 
			// _submitForReviewButton
			// 
			resources.ApplyResources(this._submitForReviewButton, "_submitForReviewButton");
			this._submitForReviewButton.Name = "_submitForReviewButton";
			this._submitForReviewButton.UseVisualStyleBackColor = true;
			// 
			// _sendToTranscriptionButton
			// 
			resources.ApplyResources(this._sendToTranscriptionButton, "_sendToTranscriptionButton");
			this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
			this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
			// 
			// _returnToInterpreterButton
			// 
			resources.ApplyResources(this._returnToInterpreterButton, "_returnToInterpreterButton");
			this._returnToInterpreterButton.Name = "_returnToInterpreterButton";
			this._returnToInterpreterButton.UseVisualStyleBackColor = true;
			// 
			// _saveButton
			// 
			resources.ApplyResources(this._saveButton, "_saveButton");
			this._saveButton.Name = "_saveButton";
			this._saveButton.UseVisualStyleBackColor = true;
			// 
			// _skipButton
			// 
			resources.ApplyResources(this._skipButton, "_skipButton");
			this._skipButton.Name = "_skipButton";
			this._skipButton.UseVisualStyleBackColor = true;
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
			this.tableLayoutPanel3.Controls.Add(this._rememberSupervisorCheckbox, 1, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// _rememberSupervisorCheckbox
			// 
			resources.ApplyResources(this._rememberSupervisorCheckbox, "_rememberSupervisorCheckbox");
			this._rememberSupervisorCheckbox.Name = "_rememberSupervisorCheckbox";
			this._rememberSupervisorCheckbox.UseVisualStyleBackColor = true;
			// 
			// _supervisor
			// 
			resources.ApplyResources(this._supervisor, "_supervisor");
			this._supervisor.Name = "_supervisor";
			this._supervisor.Value = null;
			// 
			// tableLayoutPanel4
			// 
			resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
			this.tableLayoutPanel4.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.button1, 1, 5);
			this.tableLayoutPanel4.Controls.Add(this.panel2, 0, 3);
			this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel2, 0, 5);
			this.tableLayoutPanel4.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel4.SetColumnSpan(this.label1, 2);
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.Color.White;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel4.SetColumnSpan(this.label2, 2);
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Name = "label2";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			resources.ApplyResources(this.panel2, "panel2");
			this.tableLayoutPanel4.SetColumnSpan(this.panel2, 2);
			this.panel2.Name = "panel2";
			// 
			// flowLayoutPanel2
			// 
			resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
			this.flowLayoutPanel2.Controls.Add(this.button2);
			this.flowLayoutPanel2.Controls.Add(this.button3);
			this.flowLayoutPanel2.Controls.Add(this.button4);
			this.flowLayoutPanel2.Controls.Add(this.button5);
			this.flowLayoutPanel2.Controls.Add(this.button6);
			this.flowLayoutPanel2.Controls.Add(this.button7);
			this.flowLayoutPanel2.Controls.Add(this.checkBox1);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			resources.ApplyResources(this.button3, "button3");
			this.button3.Name = "button3";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			resources.ApplyResources(this.button4, "button4");
			this.button4.Name = "button4";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			resources.ApplyResources(this.button5, "button5");
			this.button5.Name = "button5";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// button6
			// 
			resources.ApplyResources(this.button6, "button6");
			this.button6.Name = "button6";
			this.button6.UseVisualStyleBackColor = true;
			// 
			// button7
			// 
			resources.ApplyResources(this.button7, "button7");
			this.button7.Name = "button7";
			this.button7.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel4.SetColumnSpan(this.label3, 2);
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Name = "label3";
			// 
			// tableLayoutPanel5
			// 
			resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
			this.tableLayoutPanel5.Controls.Add(this.checkBox2, 1, 0);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			// 
			// checkBox2
			// 
			resources.ApplyResources(this.checkBox2, "checkBox2");
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// lookupField1
			// 
			resources.ApplyResources(this.lookupField1, "lookupField1");
			this.lookupField1.Name = "lookupField1";
			this.lookupField1.Value = null;
			// 
			// ExternalPractitionerContactPointDetailsEditorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerContactPointDetailsEditorComponentControl";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.tableLayoutPanel5.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.TextField _description;
        private System.Windows.Forms.CheckBox _isDefaultContactPoint;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _resultCommunicationMode;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label _hasErrors;
		private System.Windows.Forms.Label _imagesUnavailable;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Panel _reportEditorPanel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _verifyButton;
		private System.Windows.Forms.Button _submitForReviewButton;
		private System.Windows.Forms.Button _sendToTranscriptionButton;
		private System.Windows.Forms.Button _returnToInterpreterButton;
		private System.Windows.Forms.Button _saveButton;
		private System.Windows.Forms.Button _skipButton;
		private System.Windows.Forms.CheckBox _reportNextItem;
		private System.Windows.Forms.Label _reportedProcedures;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.CheckBox _rememberSupervisorCheckbox;
		private LookupField _supervisor;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.CheckBox checkBox2;
		private LookupField lookupField1;
		private System.Windows.Forms.Label _warning;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _informationAuthority;
    }
}
