#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
    partial class SearchComponentControl
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
			this._accession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._fromDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._untilDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._keepOpen = new System.Windows.Forms.CheckBox();
			this._searchButton = new System.Windows.Forms.Button();
			this._clearButton = new System.Windows.Forms.Button();
			this._outerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._diagnosticService = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._procedureType = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this.panel1 = new System.Windows.Forms.Panel();
			this._showDeactivatedChoices = new System.Windows.Forms.CheckBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this._outerFlowLayoutPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _accession
			// 
			this._accession.LabelText = "Accession #";
			this._accession.Location = new System.Drawing.Point(2, 2);
			this._accession.Margin = new System.Windows.Forms.Padding(2);
			this._accession.Mask = "";
			this._accession.Name = "_accession";
			this._accession.PasswordChar = '\0';
			this._accession.Size = new System.Drawing.Size(206, 42);
			this._accession.TabIndex = 0;
			this._accession.ToolTip = null;
			this._accession.Value = null;
			// 
			// _mrn
			// 
			this._mrn.LabelText = "MRN";
			this._mrn.Location = new System.Drawing.Point(212, 2);
			this._mrn.Margin = new System.Windows.Forms.Padding(2);
			this._mrn.Mask = "";
			this._mrn.Name = "_mrn";
			this._mrn.PasswordChar = '\0';
			this._mrn.Size = new System.Drawing.Size(206, 42);
			this._mrn.TabIndex = 1;
			this._mrn.ToolTip = null;
			this._mrn.Value = null;
			// 
			// _healthcard
			// 
			this._healthcard.LabelText = "Healthcard #";
			this._healthcard.Location = new System.Drawing.Point(2, 48);
			this._healthcard.Margin = new System.Windows.Forms.Padding(2);
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.PasswordChar = '\0';
			this._healthcard.Size = new System.Drawing.Size(206, 42);
			this._healthcard.TabIndex = 2;
			this._healthcard.ToolTip = null;
			this._healthcard.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(212, 48);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(206, 42);
			this._familyName.TabIndex = 3;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(2, 94);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(206, 42);
			this._givenName.TabIndex = 4;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _fromDate
			// 
			this._fromDate.LabelText = "From Date";
			this._fromDate.Location = new System.Drawing.Point(2, 186);
			this._fromDate.Margin = new System.Windows.Forms.Padding(2);
			this._fromDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._fromDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._fromDate.Name = "_fromDate";
			this._fromDate.Nullable = true;
			this._fromDate.Size = new System.Drawing.Size(206, 42);
			this._fromDate.TabIndex = 8;
			this._fromDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 44, 343);
			// 
			// _untilDate
			// 
			this._untilDate.LabelText = "Until Date";
			this._untilDate.Location = new System.Drawing.Point(212, 186);
			this._untilDate.Margin = new System.Windows.Forms.Padding(2);
			this._untilDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._untilDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._untilDate.Name = "_untilDate";
			this._untilDate.Nullable = true;
			this._untilDate.Size = new System.Drawing.Size(206, 42);
			this._untilDate.TabIndex = 9;
			this._untilDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 47, 203);
			// 
			// _keepOpen
			// 
			this._keepOpen.AutoSize = true;
			this._keepOpen.Location = new System.Drawing.Point(4, 21);
			this._keepOpen.Name = "_keepOpen";
			this._keepOpen.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._keepOpen.Size = new System.Drawing.Size(109, 23);
			this._keepOpen.TabIndex = 1;
			this._keepOpen.Text = "Keep dialog open";
			this._keepOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._keepOpen.UseVisualStyleBackColor = true;
			// 
			// _searchButton
			// 
			this._searchButton.Location = new System.Drawing.Point(3, 19);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(98, 24);
			this._searchButton.TabIndex = 0;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _clearButton
			// 
			this._clearButton.Location = new System.Drawing.Point(107, 19);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(98, 24);
			this._clearButton.TabIndex = 1;
			this._clearButton.Text = "Clear";
			this._clearButton.UseVisualStyleBackColor = true;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// _outerFlowLayoutPanel
			// 
			this._outerFlowLayoutPanel.Controls.Add(this._accession);
			this._outerFlowLayoutPanel.Controls.Add(this._mrn);
			this._outerFlowLayoutPanel.Controls.Add(this._healthcard);
			this._outerFlowLayoutPanel.Controls.Add(this._familyName);
			this._outerFlowLayoutPanel.Controls.Add(this._givenName);
			this._outerFlowLayoutPanel.Controls.Add(this._orderingPractitioner);
			this._outerFlowLayoutPanel.Controls.Add(this._diagnosticService);
			this._outerFlowLayoutPanel.Controls.Add(this._procedureType);
			this._outerFlowLayoutPanel.Controls.Add(this._fromDate);
			this._outerFlowLayoutPanel.Controls.Add(this._untilDate);
			this._outerFlowLayoutPanel.Controls.Add(this.panel1);
			this._outerFlowLayoutPanel.Controls.Add(this.panel2);
			this._outerFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._outerFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._outerFlowLayoutPanel.Name = "_outerFlowLayoutPanel";
			this._outerFlowLayoutPanel.Size = new System.Drawing.Size(455, 300);
			this._outerFlowLayoutPanel.TabIndex = 0;
			// 
			// _orderingPractitioner
			// 
			this._orderingPractitioner.LabelText = "Ordered By";
			this._orderingPractitioner.Location = new System.Drawing.Point(212, 94);
			this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Size = new System.Drawing.Size(206, 42);
			this._orderingPractitioner.TabIndex = 5;
			this._orderingPractitioner.Value = null;
			// 
			// _diagnosticService
			// 
			this._diagnosticService.LabelText = "Imaging Service";
			this._diagnosticService.Location = new System.Drawing.Point(2, 140);
			this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
			this._diagnosticService.Name = "_diagnosticService";
			this._diagnosticService.Size = new System.Drawing.Size(206, 42);
			this._diagnosticService.TabIndex = 6;
			this._diagnosticService.Value = null;
			// 
			// _procedureType
			// 
			this._procedureType.LabelText = "Procedure Type";
			this._procedureType.Location = new System.Drawing.Point(212, 140);
			this._procedureType.Margin = new System.Windows.Forms.Padding(2);
			this._procedureType.Name = "_procedureType";
			this._procedureType.Size = new System.Drawing.Size(206, 42);
			this._procedureType.TabIndex = 7;
			this._procedureType.Value = null;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._showDeactivatedChoices);
			this.panel1.Controls.Add(this._keepOpen);
			this.panel1.Location = new System.Drawing.Point(2, 232);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(206, 49);
			this.panel1.TabIndex = 10;
			// 
			// _showDeactivatedChoices
			// 
			this._showDeactivatedChoices.AutoSize = true;
			this._showDeactivatedChoices.Location = new System.Drawing.Point(4, 1);
			this._showDeactivatedChoices.Name = "_showDeactivatedChoices";
			this._showDeactivatedChoices.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._showDeactivatedChoices.Size = new System.Drawing.Size(201, 23);
			this._showDeactivatedChoices.TabIndex = 0;
			this._showDeactivatedChoices.Text = "Include inactive items in filter choices";
			this._showDeactivatedChoices.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._showDeactivatedChoices.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._clearButton);
			this.panel2.Controls.Add(this._searchButton);
			this.panel2.Location = new System.Drawing.Point(212, 232);
			this.panel2.Margin = new System.Windows.Forms.Padding(2);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(206, 49);
			this.panel2.TabIndex = 11;
			// 
			// SearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._outerFlowLayoutPanel);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SearchComponentControl";
			this.Size = new System.Drawing.Size(455, 300);
			this._outerFlowLayoutPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _accession;
		private LookupField _orderingPractitioner;
		private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
		private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
		private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _fromDate;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _untilDate;
		private System.Windows.Forms.CheckBox _keepOpen;
		private System.Windows.Forms.Button _searchButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
		private LookupField _procedureType;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.FlowLayoutPanel _outerFlowLayoutPanel;
		private LookupField _diagnosticService;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.CheckBox _showDeactivatedChoices;
    }
}
