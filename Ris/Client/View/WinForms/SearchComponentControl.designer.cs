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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchComponentControl));
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
			resources.ApplyResources(this._accession, "_accession");
			this._accession.Mask = "";
			this._accession.Name = "_accession";
			this._accession.Value = null;
			// 
			// _mrn
			// 
			resources.ApplyResources(this._mrn, "_mrn");
			this._mrn.Mask = "";
			this._mrn.Name = "_mrn";
			this._mrn.Value = null;
			// 
			// _healthcard
			// 
			resources.ApplyResources(this._healthcard, "_healthcard");
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.Value = null;
			// 
			// _familyName
			// 
			resources.ApplyResources(this._familyName, "_familyName");
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.Value = null;
			// 
			// _givenName
			// 
			resources.ApplyResources(this._givenName, "_givenName");
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.Value = null;
			// 
			// _fromDate
			// 
			resources.ApplyResources(this._fromDate, "_fromDate");
			this._fromDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._fromDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._fromDate.Name = "_fromDate";
			this._fromDate.Nullable = true;
			this._fromDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 44, 343);
			// 
			// _untilDate
			// 
			resources.ApplyResources(this._untilDate, "_untilDate");
			this._untilDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._untilDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._untilDate.Name = "_untilDate";
			this._untilDate.Nullable = true;
			this._untilDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 47, 203);
			// 
			// _keepOpen
			// 
			resources.ApplyResources(this._keepOpen, "_keepOpen");
			this._keepOpen.Name = "_keepOpen";
			this._keepOpen.UseVisualStyleBackColor = true;
			// 
			// _searchButton
			// 
			resources.ApplyResources(this._searchButton, "_searchButton");
			this._searchButton.Name = "_searchButton";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _clearButton
			// 
			resources.ApplyResources(this._clearButton, "_clearButton");
			this._clearButton.Name = "_clearButton";
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
			resources.ApplyResources(this._outerFlowLayoutPanel, "_outerFlowLayoutPanel");
			this._outerFlowLayoutPanel.Name = "_outerFlowLayoutPanel";
			// 
			// _orderingPractitioner
			// 
			resources.ApplyResources(this._orderingPractitioner, "_orderingPractitioner");
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Value = null;
			// 
			// _diagnosticService
			// 
			resources.ApplyResources(this._diagnosticService, "_diagnosticService");
			this._diagnosticService.Name = "_diagnosticService";
			this._diagnosticService.Value = null;
			// 
			// _procedureType
			// 
			resources.ApplyResources(this._procedureType, "_procedureType");
			this._procedureType.Name = "_procedureType";
			this._procedureType.Value = null;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._showDeactivatedChoices);
			this.panel1.Controls.Add(this._keepOpen);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// _showDeactivatedChoices
			// 
			resources.ApplyResources(this._showDeactivatedChoices, "_showDeactivatedChoices");
			this._showDeactivatedChoices.Name = "_showDeactivatedChoices";
			this._showDeactivatedChoices.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._clearButton);
			this.panel2.Controls.Add(this._searchButton);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// SearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._outerFlowLayoutPanel);
			this.Name = "SearchComponentControl";
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
