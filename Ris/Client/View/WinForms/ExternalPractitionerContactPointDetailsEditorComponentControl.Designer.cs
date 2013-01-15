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
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(14, 9);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(150, 41);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(14, 55);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.PasswordChar = '\0';
			this._description.Size = new System.Drawing.Size(357, 41);
			this._description.TabIndex = 2;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// _isDefaultContactPoint
			// 
			this._isDefaultContactPoint.AutoSize = true;
			this._isDefaultContactPoint.Location = new System.Drawing.Point(244, 25);
			this._isDefaultContactPoint.Name = "_isDefaultContactPoint";
			this._isDefaultContactPoint.Size = new System.Drawing.Size(127, 17);
			this._isDefaultContactPoint.TabIndex = 1;
			this._isDefaultContactPoint.Text = "Default Contact Point";
			this._isDefaultContactPoint.UseVisualStyleBackColor = true;
			// 
			// _resultCommunicationMode
			// 
			this._resultCommunicationMode.DataSource = null;
			this._resultCommunicationMode.DisplayMember = "";
			this._resultCommunicationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._resultCommunicationMode.LabelText = "Preferred means of result communication";
			this._resultCommunicationMode.Location = new System.Drawing.Point(14, 101);
			this._resultCommunicationMode.Margin = new System.Windows.Forms.Padding(2);
			this._resultCommunicationMode.Name = "_resultCommunicationMode";
			this._resultCommunicationMode.Size = new System.Drawing.Size(357, 41);
			this._resultCommunicationMode.TabIndex = 3;
			this._resultCommunicationMode.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._warning, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(425, 225);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// _warning
			// 
			this._warning.AutoSize = true;
			this._warning.BackColor = System.Drawing.Color.White;
			this._warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this._warning, 2);
			this._warning.Dock = System.Windows.Forms.DockStyle.Fill;
			this._warning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._warning.ForeColor = System.Drawing.Color.Red;
			this._warning.Location = new System.Drawing.Point(3, 3);
			this._warning.Margin = new System.Windows.Forms.Padding(3);
			this._warning.Name = "_warning";
			this._warning.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._warning.Size = new System.Drawing.Size(419, 22);
			this._warning.TabIndex = 1;
			this._warning.Text = "Warning Message";
			this._warning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._warning.Visible = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._informationAuthority);
			this.panel1.Controls.Add(this._name);
			this.panel1.Controls.Add(this._resultCommunicationMode);
			this.panel1.Controls.Add(this._description);
			this.panel1.Controls.Add(this._isDefaultContactPoint);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 28);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(425, 197);
			this.panel1.TabIndex = 0;
			// 
			// _informationAuthority
			// 
			this._informationAuthority.DataSource = null;
			this._informationAuthority.DisplayMember = "";
			this._informationAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._informationAuthority.LabelText = "Associated Information Authority (if applicable)";
			this._informationAuthority.Location = new System.Drawing.Point(14, 147);
			this._informationAuthority.Margin = new System.Windows.Forms.Padding(2);
			this._informationAuthority.Name = "_informationAuthority";
			this._informationAuthority.Size = new System.Drawing.Size(357, 41);
			this._informationAuthority.TabIndex = 4;
			this._informationAuthority.Value = null;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this._hasErrors, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._imagesUnavailable, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportEditorPanel, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportedProcedures, 0, 2);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 6;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 100);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _hasErrors
			// 
			this._hasErrors.AutoSize = true;
			this._hasErrors.BackColor = System.Drawing.Color.White;
			this._hasErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._hasErrors, 2);
			this._hasErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this._hasErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._hasErrors.ForeColor = System.Drawing.Color.Red;
			this._hasErrors.Location = new System.Drawing.Point(3, 23);
			this._hasErrors.Margin = new System.Windows.Forms.Padding(3);
			this._hasErrors.Name = "_hasErrors";
			this._hasErrors.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._hasErrors.Size = new System.Drawing.Size(194, 14);
			this._hasErrors.TabIndex = 1;
			this._hasErrors.Text = "Transcription has flagged the report.";
			this._hasErrors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _imagesUnavailable
			// 
			this._imagesUnavailable.AutoSize = true;
			this._imagesUnavailable.BackColor = System.Drawing.Color.White;
			this._imagesUnavailable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._imagesUnavailable, 2);
			this._imagesUnavailable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._imagesUnavailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._imagesUnavailable.ForeColor = System.Drawing.Color.Red;
			this._imagesUnavailable.Location = new System.Drawing.Point(3, 3);
			this._imagesUnavailable.Margin = new System.Windows.Forms.Padding(3);
			this._imagesUnavailable.Name = "_imagesUnavailable";
			this._imagesUnavailable.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._imagesUnavailable.Size = new System.Drawing.Size(194, 14);
			this._imagesUnavailable.TabIndex = 0;
			this._imagesUnavailable.Text = "Images cannot be opened.";
			this._imagesUnavailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(122, 103);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 14);
			this._cancelButton.TabIndex = 6;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _reportEditorPanel
			// 
			this._reportEditorPanel.AutoSize = true;
			this._reportEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._reportEditorPanel, 2);
			this._reportEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorPanel.Location = new System.Drawing.Point(3, 63);
			this._reportEditorPanel.Name = "_reportEditorPanel";
			this._reportEditorPanel.Size = new System.Drawing.Size(194, 14);
			this._reportEditorPanel.TabIndex = 3;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._verifyButton);
			this.flowLayoutPanel1.Controls.Add(this._submitForReviewButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToTranscriptionButton);
			this.flowLayoutPanel1.Controls.Add(this._returnToInterpreterButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._skipButton);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 100);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(119, 20);
			this.flowLayoutPanel1.TabIndex = 5;
			// 
			// _verifyButton
			// 
			this._verifyButton.Location = new System.Drawing.Point(3, 3);
			this._verifyButton.Name = "_verifyButton";
			this._verifyButton.Size = new System.Drawing.Size(75, 23);
			this._verifyButton.TabIndex = 0;
			this._verifyButton.Text = "Verify";
			this._verifyButton.UseVisualStyleBackColor = true;
			// 
			// _submitForReviewButton
			// 
			this._submitForReviewButton.Location = new System.Drawing.Point(3, 32);
			this._submitForReviewButton.Name = "_submitForReviewButton";
			this._submitForReviewButton.Size = new System.Drawing.Size(83, 23);
			this._submitForReviewButton.TabIndex = 1;
			this._submitForReviewButton.Text = "For Review";
			this._submitForReviewButton.UseVisualStyleBackColor = true;
			// 
			// _sendToTranscriptionButton
			// 
			this._sendToTranscriptionButton.Location = new System.Drawing.Point(3, 61);
			this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
			this._sendToTranscriptionButton.Size = new System.Drawing.Size(145, 23);
			this._sendToTranscriptionButton.TabIndex = 2;
			this._sendToTranscriptionButton.Text = "Send to Transcription";
			this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
			// 
			// _returnToInterpreterButton
			// 
			this._returnToInterpreterButton.Location = new System.Drawing.Point(3, 90);
			this._returnToInterpreterButton.Name = "_returnToInterpreterButton";
			this._returnToInterpreterButton.Size = new System.Drawing.Size(145, 23);
			this._returnToInterpreterButton.TabIndex = 3;
			this._returnToInterpreterButton.Text = "Return to Interpreter";
			this._returnToInterpreterButton.UseVisualStyleBackColor = true;
			// 
			// _saveButton
			// 
			this._saveButton.Location = new System.Drawing.Point(3, 119);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(75, 23);
			this._saveButton.TabIndex = 4;
			this._saveButton.Text = "Save";
			this._saveButton.UseVisualStyleBackColor = true;
			// 
			// _skipButton
			// 
			this._skipButton.Location = new System.Drawing.Point(3, 148);
			this._skipButton.Name = "_skipButton";
			this._skipButton.Size = new System.Drawing.Size(75, 23);
			this._skipButton.TabIndex = 5;
			this._skipButton.Text = "Skip";
			this._skipButton.UseVisualStyleBackColor = true;
			// 
			// _reportNextItem
			// 
			this._reportNextItem.AutoSize = true;
			this._reportNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportNextItem.Location = new System.Drawing.Point(3, 177);
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.Size = new System.Drawing.Size(104, 17);
			this._reportNextItem.TabIndex = 6;
			this._reportNextItem.Text = "Go To Next Item";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _reportedProcedures
			// 
			this._reportedProcedures.AutoSize = true;
			this._reportedProcedures.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel2.SetColumnSpan(this._reportedProcedures, 2);
			this._reportedProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportedProcedures.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._reportedProcedures.ForeColor = System.Drawing.SystemColors.ControlText;
			this._reportedProcedures.Location = new System.Drawing.Point(3, 40);
			this._reportedProcedures.Name = "_reportedProcedures";
			this._reportedProcedures.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._reportedProcedures.Size = new System.Drawing.Size(194, 20);
			this._reportedProcedures.TabIndex = 2;
			this._reportedProcedures.Text = "Reported Procedure(s): ";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this._rememberSupervisorCheckbox, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(200, 100);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _rememberSupervisorCheckbox
			// 
			this._rememberSupervisorCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rememberSupervisorCheckbox.Location = new System.Drawing.Point(110, 3);
			this._rememberSupervisorCheckbox.Name = "_rememberSupervisorCheckbox";
			this._rememberSupervisorCheckbox.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this._rememberSupervisorCheckbox.Size = new System.Drawing.Size(87, 94);
			this._rememberSupervisorCheckbox.TabIndex = 1;
			this._rememberSupervisorCheckbox.Text = "Remember Supervisor?";
			this._rememberSupervisorCheckbox.UseVisualStyleBackColor = true;
			// 
			// _supervisor
			// 
			this._supervisor.AutoSize = true;
			this._supervisor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._supervisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._supervisor.LabelText = "Supervising Radiologist (if applicable):";
			this._supervisor.Location = new System.Drawing.Point(2, 2);
			this._supervisor.Margin = new System.Windows.Forms.Padding(2, 2, 25, 2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Size = new System.Drawing.Size(299, 43);
			this._supervisor.TabIndex = 0;
			this._supervisor.Value = null;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.button1, 1, 5);
			this.tableLayoutPanel4.Controls.Add(this.panel2, 0, 3);
			this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel2, 0, 5);
			this.tableLayoutPanel4.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 6;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel4.Size = new System.Drawing.Size(200, 100);
			this.tableLayoutPanel4.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel4.SetColumnSpan(this.label1, 2);
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(3, 23);
			this.label1.Margin = new System.Windows.Forms.Padding(3);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this.label1.Size = new System.Drawing.Size(194, 14);
			this.label1.TabIndex = 1;
			this.label1.Text = "Transcription has flagged the report.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.White;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel4.SetColumnSpan(this.label2, 2);
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Margin = new System.Windows.Forms.Padding(3);
			this.label2.Name = "label2";
			this.label2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this.label2.Size = new System.Drawing.Size(194, 14);
			this.label2.TabIndex = 0;
			this.label2.Text = "Images cannot be opened.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(122, 103);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 14);
			this.button1.TabIndex = 6;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.SetColumnSpan(this.panel2, 2);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 63);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(194, 14);
			this.panel2.TabIndex = 3;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this.button2);
			this.flowLayoutPanel2.Controls.Add(this.button3);
			this.flowLayoutPanel2.Controls.Add(this.button4);
			this.flowLayoutPanel2.Controls.Add(this.button5);
			this.flowLayoutPanel2.Controls.Add(this.button6);
			this.flowLayoutPanel2.Controls.Add(this.button7);
			this.flowLayoutPanel2.Controls.Add(this.checkBox1);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 100);
			this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(119, 20);
			this.flowLayoutPanel2.TabIndex = 5;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(3, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 0;
			this.button2.Text = "Verify";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(3, 32);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(83, 23);
			this.button3.TabIndex = 1;
			this.button3.Text = "For Review";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(3, 61);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(145, 23);
			this.button4.TabIndex = 2;
			this.button4.Text = "Send to Transcription";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(3, 90);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(145, 23);
			this.button5.TabIndex = 3;
			this.button5.Text = "Return to Interpreter";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(3, 119);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 4;
			this.button6.Text = "Save";
			this.button6.UseVisualStyleBackColor = true;
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(3, 148);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 5;
			this.button7.Text = "Skip";
			this.button7.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.checkBox1.Location = new System.Drawing.Point(3, 177);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(104, 17);
			this.checkBox1.TabIndex = 6;
			this.checkBox1.Text = "Go To Next Item";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel4.SetColumnSpan(this.label3, 2);
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label3.Location = new System.Drawing.Point(3, 40);
			this.label3.Name = "label3";
			this.label3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this.label3.Size = new System.Drawing.Size(194, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Reported Procedure(s): ";
			// 
			// tableLayoutPanel5
			// 
			this.tableLayoutPanel5.AutoSize = true;
			this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel5.ColumnCount = 2;
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.Controls.Add(this.checkBox2, 1, 0);
			this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 1;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(200, 100);
			this.tableLayoutPanel5.TabIndex = 0;
			// 
			// checkBox2
			// 
			this.checkBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.checkBox2.Location = new System.Drawing.Point(110, 3);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.checkBox2.Size = new System.Drawing.Size(87, 94);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Text = "Remember Supervisor?";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// lookupField1
			// 
			this.lookupField1.AutoSize = true;
			this.lookupField1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.lookupField1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lookupField1.LabelText = "Supervising Radiologist (if applicable):";
			this.lookupField1.Location = new System.Drawing.Point(2, 2);
			this.lookupField1.Margin = new System.Windows.Forms.Padding(2, 2, 25, 2);
			this.lookupField1.Name = "lookupField1";
			this.lookupField1.Size = new System.Drawing.Size(299, 43);
			this.lookupField1.TabIndex = 0;
			this.lookupField1.Value = null;
			// 
			// ExternalPractitionerContactPointDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerContactPointDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(425, 225);
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
