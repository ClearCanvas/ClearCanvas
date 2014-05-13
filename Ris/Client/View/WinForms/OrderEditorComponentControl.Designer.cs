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
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._placeOrderButton_Click);
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
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
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// _patient
			// 
			resources.ApplyResources(this._patient, "_patient");
			this.tableLayoutPanel3.SetColumnSpan(this._patient, 2);
			this._patient.Name = "_patient";
			this._patient.Value = null;
			// 
			// _orderingPractitionerContactPoint
			// 
			this.tableLayoutPanel3.SetColumnSpan(this._orderingPractitionerContactPoint, 2);
			this._orderingPractitionerContactPoint.DataSource = null;
			this._orderingPractitionerContactPoint.DisplayMember = "";
			resources.ApplyResources(this._orderingPractitionerContactPoint, "_orderingPractitionerContactPoint");
			this._orderingPractitionerContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._orderingPractitionerContactPoint.Name = "_orderingPractitionerContactPoint";
			this._orderingPractitionerContactPoint.Value = null;
			// 
			// tableLayoutPanel4
			// 
			resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
			this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel4, 2);
			this.tableLayoutPanel4.Controls.Add(this._visit, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this._visitSummaryButton, 1, 0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			// 
			// _visit
			// 
			this._visit.DataSource = null;
			this._visit.DisplayMember = "";
			resources.ApplyResources(this._visit, "_visit");
			this._visit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._visit.Name = "_visit";
			this._visit.Value = null;
			// 
			// _visitSummaryButton
			// 
			resources.ApplyResources(this._visitSummaryButton, "_visitSummaryButton");
			this._visitSummaryButton.Name = "_visitSummaryButton";
			this._visitSummaryButton.UseVisualStyleBackColor = true;
			this._visitSummaryButton.Click += new System.EventHandler(this._visitSummaryButton_Click);
			// 
			// _schedulingRequestTime
			// 
			resources.ApplyResources(this._schedulingRequestTime, "_schedulingRequestTime");
			this._schedulingRequestTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestTime.Name = "_schedulingRequestTime";
			this._schedulingRequestTime.Nullable = true;
			this._schedulingRequestTime.ShowDate = false;
			this._schedulingRequestTime.ShowTime = true;
			this._schedulingRequestTime.Value = null;
			// 
			// _diagnosticService
			// 
			resources.ApplyResources(this._diagnosticService, "_diagnosticService");
			this.tableLayoutPanel3.SetColumnSpan(this._diagnosticService, 2);
			this._diagnosticService.Name = "_diagnosticService";
			this._diagnosticService.Value = null;
			// 
			// _schedulingRequestDate
			// 
			resources.ApplyResources(this._schedulingRequestDate, "_schedulingRequestDate");
			this._schedulingRequestDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestDate.Name = "_schedulingRequestDate";
			this._schedulingRequestDate.Nullable = true;
			this._schedulingRequestDate.Value = null;
			// 
			// _priority
			// 
			resources.ApplyResources(this._priority, "_priority");
			this._priority.DataSource = null;
			this._priority.DisplayMember = "";
			this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._priority.Name = "_priority";
			this._priority.Value = null;
			// 
			// _orderingPractitioner
			// 
			resources.ApplyResources(this._orderingPractitioner, "_orderingPractitioner");
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Value = null;
			// 
			// _indication
			// 
			resources.ApplyResources(this._indication, "_indication");
			this.tableLayoutPanel3.SetColumnSpan(this._indication, 2);
			this._indication.Mask = "";
			this._indication.Name = "_indication";
			this._indication.Value = null;
			// 
			// _orderingFacility
			// 
			resources.ApplyResources(this._orderingFacility, "_orderingFacility");
			this._orderingFacility.Mask = "";
			this._orderingFacility.Name = "_orderingFacility";
			this._orderingFacility.ReadOnly = true;
			this._orderingFacility.Value = null;
			// 
			// panel1
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.panel1, 2);
			this.panel1.Controls.Add(this._downtimeAccession);
			this.panel1.Controls.Add(this._reorderReason);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _downtimeAccession
			// 
			resources.ApplyResources(this._downtimeAccession, "_downtimeAccession");
			this._downtimeAccession.Mask = "";
			this._downtimeAccession.Name = "_downtimeAccession";
			this._downtimeAccession.Value = null;
			// 
			// _reorderReason
			// 
			this._reorderReason.DataSource = null;
			this._reorderReason.DisplayMember = "";
			resources.ApplyResources(this._reorderReason, "_reorderReason");
			this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reorderReason.Name = "_reorderReason";
			this._reorderReason.Value = null;
			// 
			// _proceduresTableView
			// 
			resources.ApplyResources(this._proceduresTableView, "_proceduresTableView");
			this.tableLayoutPanel3.SetColumnSpan(this._proceduresTableView, 2);
			this._proceduresTableView.Name = "_proceduresTableView";
			this._proceduresTableView.ReadOnly = false;
			this._proceduresTableView.ShowToolbar = false;
			this._proceduresTableView.ItemDoubleClicked += new System.EventHandler(this._proceduresTableView_ItemDoubleClicked);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._recipientContactPoint, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._recipientLookup, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._recipientsTableView, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._addConsultantButton, 1, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _recipientContactPoint
			// 
			resources.ApplyResources(this._recipientContactPoint, "_recipientContactPoint");
			this._recipientContactPoint.DataSource = null;
			this._recipientContactPoint.DisplayMember = "";
			this._recipientContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._recipientContactPoint.Name = "_recipientContactPoint";
			this._recipientContactPoint.Value = null;
			// 
			// _recipientLookup
			// 
			resources.ApplyResources(this._recipientLookup, "_recipientLookup");
			this._recipientLookup.Name = "_recipientLookup";
			this._recipientLookup.Value = null;
			// 
			// _recipientsTableView
			// 
			resources.ApplyResources(this._recipientsTableView, "_recipientsTableView");
			this.tableLayoutPanel1.SetColumnSpan(this._recipientsTableView, 2);
			this._recipientsTableView.MultiSelect = false;
			this._recipientsTableView.Name = "_recipientsTableView";
			this._recipientsTableView.ReadOnly = false;
			this._recipientsTableView.ShowToolbar = false;
			// 
			// _addConsultantButton
			// 
			resources.ApplyResources(this._addConsultantButton, "_addConsultantButton");
			this._addConsultantButton.Name = "_addConsultantButton";
			this._addConsultantButton.UseVisualStyleBackColor = true;
			this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
			// 
			// _mainTab
			// 
			this._mainTab.Controls.Add(this._generalPage);
			this._mainTab.Controls.Add(this._notesPage);
			this._mainTab.Controls.Add(this._copiesToRecipients);
			this._mainTab.Controls.Add(this._attachmentsPage);
			resources.ApplyResources(this._mainTab, "_mainTab");
			this._mainTab.Name = "_mainTab";
			this._mainTab.SelectedIndex = 0;
			// 
			// _generalPage
			// 
			this._generalPage.Controls.Add(this.tableLayoutPanel3);
			resources.ApplyResources(this._generalPage, "_generalPage");
			this._generalPage.Name = "_generalPage";
			this._generalPage.UseVisualStyleBackColor = true;
			// 
			// _notesPage
			// 
			resources.ApplyResources(this._notesPage, "_notesPage");
			this._notesPage.Name = "_notesPage";
			this._notesPage.UseVisualStyleBackColor = true;
			// 
			// _copiesToRecipients
			// 
			this._copiesToRecipients.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this._copiesToRecipients, "_copiesToRecipients");
			this._copiesToRecipients.Name = "_copiesToRecipients";
			this._copiesToRecipients.UseVisualStyleBackColor = true;
			// 
			// _attachmentsPage
			// 
			resources.ApplyResources(this._attachmentsPage, "_attachmentsPage");
			this._attachmentsPage.Name = "_attachmentsPage";
			this._attachmentsPage.UseVisualStyleBackColor = true;
			// 
			// OrderEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this._mainTab);
			this.Name = "OrderEditorComponentControl";
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
