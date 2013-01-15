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
    partial class OrderEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderEditorComponentControl));
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._patient = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._orderingPractitionerContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this._visit = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._visitSummaryButton = new System.Windows.Forms.Button();
			this._schedulingRequestTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._diagnosticService = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._schedulingRequestDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._indication = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.panel1 = new System.Windows.Forms.Panel();
			this._downtimeAccession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._proceduresTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._recipientContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._recipientLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._recipientsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._addConsultantButton = new System.Windows.Forms.Button();
			this._mainTab = new System.Windows.Forms.TabControl();
			this._generalPage = new System.Windows.Forms.TabPage();
			this._notesPage = new System.Windows.Forms.TabPage();
			this._copiesToRecipients = new System.Windows.Forms.TabPage();
			this._attachmentsPage = new System.Windows.Forms.TabPage();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this._mainTab.SuspendLayout();
			this._generalPage.SuspendLayout();
			this._copiesToRecipients.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(81, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(2, 2);
			this._acceptButton.Margin = new System.Windows.Forms.Padding(2);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._placeOrderButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(517, 588);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(158, 27);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoScroll = true;
			this.tableLayoutPanel3.AutoScrollMinSize = new System.Drawing.Size(-1, 450);
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
			this.tableLayoutPanel3.Controls.Add(this._patient, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._orderingPractitionerContactPoint, 0, 4);
			this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 6);
			this.tableLayoutPanel3.Controls.Add(this._schedulingRequestTime, 1, 7);
			this.tableLayoutPanel3.Controls.Add(this._diagnosticService, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._schedulingRequestDate, 0, 7);
			this.tableLayoutPanel3.Controls.Add(this._priority, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this._orderingPractitioner, 1, 3);
			this.tableLayoutPanel3.Controls.Add(this._indication, 0, 5);
			this.tableLayoutPanel3.Controls.Add(this._orderingFacility, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.panel1, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this._proceduresTableView, 0, 8);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 9;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(662, 551);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _patient
			// 
			this._patient.AutoSize = true;
			this._patient.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.SetColumnSpan(this._patient, 2);
			this._patient.Dock = System.Windows.Forms.DockStyle.Fill;
			this._patient.LabelText = "Patient";
			this._patient.Location = new System.Drawing.Point(2, 2);
			this._patient.Margin = new System.Windows.Forms.Padding(2);
			this._patient.Name = "_patient";
			this._patient.Size = new System.Drawing.Size(642, 43);
			this._patient.TabIndex = 0;
			this._patient.Value = null;
			// 
			// _orderingPractitionerContactPoint
			// 
			this.tableLayoutPanel3.SetColumnSpan(this._orderingPractitionerContactPoint, 2);
			this._orderingPractitionerContactPoint.DataSource = null;
			this._orderingPractitionerContactPoint.DisplayMember = "";
			this._orderingPractitionerContactPoint.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingPractitionerContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._orderingPractitionerContactPoint.LabelText = "Ordering Practitioner Contact Point";
			this._orderingPractitionerContactPoint.Location = new System.Drawing.Point(2, 188);
			this._orderingPractitionerContactPoint.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitionerContactPoint.Name = "_orderingPractitionerContactPoint";
			this._orderingPractitionerContactPoint.Size = new System.Drawing.Size(642, 46);
			this._orderingPractitionerContactPoint.TabIndex = 5;
			this._orderingPractitionerContactPoint.Value = null;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel4, 2);
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this._visit, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this._visitSummaryButton, 1, 0);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 280);
			this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 1;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(646, 45);
			this.tableLayoutPanel4.TabIndex = 7;
			// 
			// _visit
			// 
			this._visit.DataSource = null;
			this._visit.DisplayMember = "";
			this._visit.Dock = System.Windows.Forms.DockStyle.Fill;
			this._visit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._visit.LabelText = "Visit";
			this._visit.Location = new System.Drawing.Point(2, 2);
			this._visit.Margin = new System.Windows.Forms.Padding(2);
			this._visit.Name = "_visit";
			this._visit.Size = new System.Drawing.Size(612, 41);
			this._visit.TabIndex = 0;
			this._visit.Value = null;
			// 
			// _visitSummaryButton
			// 
			this._visitSummaryButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._visitSummaryButton.Image = ((System.Drawing.Image)(resources.GetObject("_visitSummaryButton.Image")));
			this._visitSummaryButton.Location = new System.Drawing.Point(619, 18);
			this._visitSummaryButton.Name = "_visitSummaryButton";
			this._visitSummaryButton.Size = new System.Drawing.Size(24, 24);
			this._visitSummaryButton.TabIndex = 1;
			this._visitSummaryButton.UseVisualStyleBackColor = true;
			this._visitSummaryButton.Click += new System.EventHandler(this._visitSummaryButton_Click);
			// 
			// _schedulingRequestTime
			// 
			this._schedulingRequestTime.AutoSize = true;
			this._schedulingRequestTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._schedulingRequestTime.Dock = System.Windows.Forms.DockStyle.Fill;
			this._schedulingRequestTime.LabelText = "Requested Schedule Time";
			this._schedulingRequestTime.Location = new System.Drawing.Point(325, 327);
			this._schedulingRequestTime.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingRequestTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestTime.Name = "_schedulingRequestTime";
			this._schedulingRequestTime.Nullable = true;
			this._schedulingRequestTime.ShowDate = false;
			this._schedulingRequestTime.ShowTime = true;
			this._schedulingRequestTime.Size = new System.Drawing.Size(319, 40);
			this._schedulingRequestTime.TabIndex = 8;
			this._schedulingRequestTime.Value = null;
			// 
			// _diagnosticService
			// 
			this._diagnosticService.AutoSize = true;
			this._diagnosticService.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.SetColumnSpan(this._diagnosticService, 2);
			this._diagnosticService.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diagnosticService.LabelText = "Imaging Service";
			this._diagnosticService.Location = new System.Drawing.Point(2, 94);
			this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
			this._diagnosticService.Name = "_diagnosticService";
			this._diagnosticService.Size = new System.Drawing.Size(642, 43);
			this._diagnosticService.TabIndex = 2;
			this._diagnosticService.Value = null;
			// 
			// _schedulingRequestDate
			// 
			this._schedulingRequestDate.AutoSize = true;
			this._schedulingRequestDate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._schedulingRequestDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._schedulingRequestDate.LabelText = "Requested Schedule Date";
			this._schedulingRequestDate.Location = new System.Drawing.Point(2, 327);
			this._schedulingRequestDate.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingRequestDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestDate.Name = "_schedulingRequestDate";
			this._schedulingRequestDate.Nullable = true;
			this._schedulingRequestDate.Size = new System.Drawing.Size(319, 40);
			this._schedulingRequestDate.TabIndex = 7;
			this._schedulingRequestDate.Value = null;
			// 
			// _priority
			// 
			this._priority.AutoSize = true;
			this._priority.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._priority.DataSource = null;
			this._priority.DisplayMember = "";
			this._priority.Dock = System.Windows.Forms.DockStyle.Fill;
			this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._priority.LabelText = "Priority";
			this._priority.Location = new System.Drawing.Point(2, 141);
			this._priority.Margin = new System.Windows.Forms.Padding(2);
			this._priority.Name = "_priority";
			this._priority.Size = new System.Drawing.Size(319, 43);
			this._priority.TabIndex = 3;
			this._priority.Value = null;
			// 
			// _orderingPractitioner
			// 
			this._orderingPractitioner.AutoSize = true;
			this._orderingPractitioner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._orderingPractitioner.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingPractitioner.LabelText = "Ordering Practitioner";
			this._orderingPractitioner.Location = new System.Drawing.Point(325, 141);
			this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Size = new System.Drawing.Size(319, 43);
			this._orderingPractitioner.TabIndex = 4;
			this._orderingPractitioner.Value = null;
			// 
			// _indication
			// 
			this._indication.AutoSize = true;
			this._indication.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.SetColumnSpan(this._indication, 2);
			this._indication.Dock = System.Windows.Forms.DockStyle.Fill;
			this._indication.LabelText = "Indication";
			this._indication.Location = new System.Drawing.Point(2, 238);
			this._indication.Margin = new System.Windows.Forms.Padding(2);
			this._indication.Mask = "";
			this._indication.Name = "_indication";
			this._indication.PasswordChar = '\0';
			this._indication.Size = new System.Drawing.Size(642, 40);
			this._indication.TabIndex = 6;
			this._indication.ToolTip = null;
			this._indication.Value = null;
			// 
			// _orderingFacility
			// 
			this._orderingFacility.AutoSize = true;
			this._orderingFacility.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._orderingFacility.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingFacility.LabelText = "Ordering Facility";
			this._orderingFacility.Location = new System.Drawing.Point(2, 49);
			this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
			this._orderingFacility.Mask = "";
			this._orderingFacility.Name = "_orderingFacility";
			this._orderingFacility.PasswordChar = '\0';
			this._orderingFacility.ReadOnly = true;
			this._orderingFacility.Size = new System.Drawing.Size(319, 41);
			this._orderingFacility.TabIndex = 1;
			this._orderingFacility.ToolTip = null;
			this._orderingFacility.Value = null;
			// 
			// panel1
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.panel1, 2);
			this.panel1.Controls.Add(this._downtimeAccession);
			this.panel1.Controls.Add(this._reorderReason);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(326, 50);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(333, 39);
			this.panel1.TabIndex = 1;
			// 
			// _downtimeAccession
			// 
			this._downtimeAccession.Dock = System.Windows.Forms.DockStyle.Fill;
			this._downtimeAccession.LabelText = "Downtime Accession #";
			this._downtimeAccession.Location = new System.Drawing.Point(0, 0);
			this._downtimeAccession.Margin = new System.Windows.Forms.Padding(2);
			this._downtimeAccession.Mask = "";
			this._downtimeAccession.Name = "_downtimeAccession";
			this._downtimeAccession.PasswordChar = '\0';
			this._downtimeAccession.Size = new System.Drawing.Size(333, 39);
			this._downtimeAccession.TabIndex = 0;
			this._downtimeAccession.ToolTip = null;
			this._downtimeAccession.Value = null;
			// 
			// _reorderReason
			// 
			this._reorderReason.DataSource = null;
			this._reorderReason.DisplayMember = "";
			this._reorderReason.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reorderReason.LabelText = "Re-order Reason";
			this._reorderReason.Location = new System.Drawing.Point(0, 0);
			this._reorderReason.Margin = new System.Windows.Forms.Padding(2);
			this._reorderReason.Name = "_reorderReason";
			this._reorderReason.Size = new System.Drawing.Size(333, 39);
			this._reorderReason.TabIndex = 0;
			this._reorderReason.Value = null;
			// 
			// _proceduresTableView
			// 
			this._proceduresTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._proceduresTableView.ColumnHeaderTooltip = null;
			this.tableLayoutPanel3.SetColumnSpan(this._proceduresTableView, 2);
			this._proceduresTableView.FilterTextBoxWidth = 132;
			this._proceduresTableView.Location = new System.Drawing.Point(0, 369);
			this._proceduresTableView.Margin = new System.Windows.Forms.Padding(0);
			this._proceduresTableView.Name = "_proceduresTableView";
			this._proceduresTableView.ReadOnly = false;
			this._proceduresTableView.ShowToolbar = false;
			this._proceduresTableView.Size = new System.Drawing.Size(646, 182);
			this._proceduresTableView.SortButtonTooltip = null;
			this._proceduresTableView.TabIndex = 9;
			this._proceduresTableView.ItemDoubleClicked += new System.EventHandler(this._proceduresTableView_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._recipientContactPoint, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._recipientLookup, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._recipientsTableView, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._addConsultantButton, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(659, 551);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _recipientContactPoint
			// 
			this._recipientContactPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._recipientContactPoint.AutoSize = true;
			this._recipientContactPoint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._recipientContactPoint.DataSource = null;
			this._recipientContactPoint.DisplayMember = "";
			this._recipientContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._recipientContactPoint.LabelText = "Practitioner Contact Point";
			this._recipientContactPoint.Location = new System.Drawing.Point(2, 49);
			this._recipientContactPoint.Margin = new System.Windows.Forms.Padding(2);
			this._recipientContactPoint.Name = "_recipientContactPoint";
			this._recipientContactPoint.Size = new System.Drawing.Size(576, 41);
			this._recipientContactPoint.TabIndex = 6;
			this._recipientContactPoint.Value = null;
			// 
			// _recipientLookup
			// 
			this._recipientLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._recipientLookup.AutoSize = true;
			this._recipientLookup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._recipientLookup.LabelText = "Find Practitioner";
			this._recipientLookup.Location = new System.Drawing.Point(2, 2);
			this._recipientLookup.Margin = new System.Windows.Forms.Padding(2);
			this._recipientLookup.Name = "_recipientLookup";
			this._recipientLookup.Size = new System.Drawing.Size(576, 43);
			this._recipientLookup.TabIndex = 4;
			this._recipientLookup.Value = null;
			// 
			// _recipientsTableView
			// 
			this._recipientsTableView.AutoSize = true;
			this._recipientsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._recipientsTableView.ColumnHeaderTooltip = null;
			this.tableLayoutPanel1.SetColumnSpan(this._recipientsTableView, 2);
			this._recipientsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._recipientsTableView.FilterTextBoxWidth = 132;
			this._recipientsTableView.Location = new System.Drawing.Point(0, 92);
			this._recipientsTableView.Margin = new System.Windows.Forms.Padding(0);
			this._recipientsTableView.MultiSelect = false;
			this._recipientsTableView.Name = "_recipientsTableView";
			this._recipientsTableView.ReadOnly = false;
			this._recipientsTableView.ShowToolbar = false;
			this._recipientsTableView.Size = new System.Drawing.Size(659, 459);
			this._recipientsTableView.SortButtonTooltip = null;
			this._recipientsTableView.TabIndex = 2;
			// 
			// _addConsultantButton
			// 
			this._addConsultantButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._addConsultantButton.Location = new System.Drawing.Point(582, 65);
			this._addConsultantButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 4);
			this._addConsultantButton.Name = "_addConsultantButton";
			this._addConsultantButton.Size = new System.Drawing.Size(75, 23);
			this._addConsultantButton.TabIndex = 5;
			this._addConsultantButton.Text = "Add";
			this._addConsultantButton.UseVisualStyleBackColor = true;
			this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
			// 
			// _mainTab
			// 
			this._mainTab.Controls.Add(this._generalPage);
			this._mainTab.Controls.Add(this._notesPage);
			this._mainTab.Controls.Add(this._copiesToRecipients);
			this._mainTab.Controls.Add(this._attachmentsPage);
			this._mainTab.Location = new System.Drawing.Point(3, 3);
			this._mainTab.Name = "_mainTab";
			this._mainTab.SelectedIndex = 0;
			this._mainTab.Size = new System.Drawing.Size(673, 583);
			this._mainTab.TabIndex = 0;
			// 
			// _generalPage
			// 
			this._generalPage.Controls.Add(this.tableLayoutPanel3);
			this._generalPage.Location = new System.Drawing.Point(4, 22);
			this._generalPage.Name = "_generalPage";
			this._generalPage.Padding = new System.Windows.Forms.Padding(3);
			this._generalPage.Size = new System.Drawing.Size(665, 557);
			this._generalPage.TabIndex = 0;
			this._generalPage.Text = "General";
			this._generalPage.UseVisualStyleBackColor = true;
			// 
			// _notesPage
			// 
			this._notesPage.Location = new System.Drawing.Point(4, 22);
			this._notesPage.Name = "_notesPage";
			this._notesPage.Padding = new System.Windows.Forms.Padding(3);
			this._notesPage.Size = new System.Drawing.Size(665, 557);
			this._notesPage.TabIndex = 1;
			this._notesPage.Text = "Notes";
			this._notesPage.UseVisualStyleBackColor = true;
			// 
			// _copiesToRecipients
			// 
			this._copiesToRecipients.Controls.Add(this.tableLayoutPanel1);
			this._copiesToRecipients.Location = new System.Drawing.Point(4, 22);
			this._copiesToRecipients.Name = "_copiesToRecipients";
			this._copiesToRecipients.Padding = new System.Windows.Forms.Padding(3);
			this._copiesToRecipients.Size = new System.Drawing.Size(665, 557);
			this._copiesToRecipients.TabIndex = 2;
			this._copiesToRecipients.Text = "Copies To";
			this._copiesToRecipients.UseVisualStyleBackColor = true;
			// 
			// _attachmentsPage
			// 
			this._attachmentsPage.Location = new System.Drawing.Point(4, 22);
			this._attachmentsPage.Name = "_attachmentsPage";
			this._attachmentsPage.Padding = new System.Windows.Forms.Padding(3);
			this._attachmentsPage.Size = new System.Drawing.Size(665, 557);
			this._attachmentsPage.TabIndex = 4;
			this._attachmentsPage.Text = "Attachments";
			this._attachmentsPage.UseVisualStyleBackColor = true;
			// 
			// OrderEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this._mainTab);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "OrderEditorComponentControl";
			this.Size = new System.Drawing.Size(678, 623);
			this.Load += new System.EventHandler(this.OrderEditorComponentControl_Load);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this._mainTab.ResumeLayout(false);
			this._generalPage.ResumeLayout(false);
			this._copiesToRecipients.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private Desktop.View.WinForms.ComboBoxField _orderingPractitionerContactPoint;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private Desktop.View.WinForms.ComboBoxField _visit;
		private System.Windows.Forms.Button _visitSummaryButton;
		private Desktop.View.WinForms.DateTimeField _schedulingRequestTime;
		private LookupField _diagnosticService;
		private Desktop.View.WinForms.DateTimeField _schedulingRequestDate;
		private Desktop.View.WinForms.ComboBoxField _priority;
		private LookupField _orderingPractitioner;
		private Desktop.View.WinForms.TextField _indication;
		private Desktop.View.WinForms.TextField _orderingFacility;
		private System.Windows.Forms.Panel panel1;
		private Desktop.View.WinForms.TextField _downtimeAccession;
		private Desktop.View.WinForms.ComboBoxField _reorderReason;
		private Desktop.View.WinForms.TableView _proceduresTableView;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private Desktop.View.WinForms.ComboBoxField _recipientContactPoint;
		private LookupField _recipientLookup;
		private Desktop.View.WinForms.TableView _recipientsTableView;
		private System.Windows.Forms.Button _addConsultantButton;
		private System.Windows.Forms.TabControl _mainTab;
		private System.Windows.Forms.TabPage _generalPage;
		private System.Windows.Forms.TabPage _notesPage;
		private System.Windows.Forms.TabPage _copiesToRecipients;
		private System.Windows.Forms.TabPage _attachmentsPage;
		private LookupField _patient;

    }
}
