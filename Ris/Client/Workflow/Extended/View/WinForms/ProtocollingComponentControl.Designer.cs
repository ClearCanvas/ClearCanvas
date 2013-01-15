namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class ProtocollingComponentControl
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
			this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._verticalSplitContainer = new System.Windows.Forms.SplitContainer();
			this._tableLayouOuter = new System.Windows.Forms.TableLayoutPanel();
			this._actionsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnSubmitForApproval = new System.Windows.Forms.Button();
			this._btnReject = new System.Windows.Forms.Button();
			this._btnSave = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._protocolNextItem = new System.Windows.Forms.CheckBox();
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._horizontalSplitContainer = new System.Windows.Forms.SplitContainer();
			this._protocolledProcedures = new System.Windows.Forms.Label();
			this._protocolEditorPanel = new System.Windows.Forms.Panel();
			this._orderNotesGroupBox = new System.Windows.Forms.GroupBox();
			this._orderNotesPanel = new System.Windows.Forms.Panel();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._orderSummaryPanel = new System.Windows.Forms.Panel();
			this._statusText = new System.Windows.Forms.Label();
			this._overviewLayoutPanel.SuspendLayout();
			this._verticalSplitContainer.Panel1.SuspendLayout();
			this._verticalSplitContainer.Panel2.SuspendLayout();
			this._verticalSplitContainer.SuspendLayout();
			this._tableLayouOuter.SuspendLayout();
			this._actionsFlowLayoutPanel.SuspendLayout();
			this.flowLayoutPanel3.SuspendLayout();
			this._horizontalSplitContainer.Panel1.SuspendLayout();
			this._horizontalSplitContainer.Panel2.SuspendLayout();
			this._horizontalSplitContainer.SuspendLayout();
			this._orderNotesGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _overviewLayoutPanel
			// 
			this._overviewLayoutPanel.ColumnCount = 1;
			this._overviewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.Controls.Add(this._verticalSplitContainer, 0, 2);
			this._overviewLayoutPanel.Controls.Add(this._orderSummaryPanel, 0, 0);
			this._overviewLayoutPanel.Controls.Add(this._statusText, 0, 1);
			this._overviewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._overviewLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
			this._overviewLayoutPanel.RowCount = 3;
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._overviewLayoutPanel.Size = new System.Drawing.Size(1128, 729);
			this._overviewLayoutPanel.TabIndex = 0;
			// 
			// _verticalSplitContainer
			// 
			this._verticalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._verticalSplitContainer.Location = new System.Drawing.Point(3, 123);
			this._verticalSplitContainer.Name = "_verticalSplitContainer";
			// 
			// _verticalSplitContainer.Panel1
			// 
			this._verticalSplitContainer.Panel1.Controls.Add(this._tableLayouOuter);
			// 
			// _verticalSplitContainer.Panel2
			// 
			this._verticalSplitContainer.Panel2.Controls.Add(this._rightHandPanel);
			this._verticalSplitContainer.Size = new System.Drawing.Size(1122, 603);
			this._verticalSplitContainer.SplitterDistance = 562;
			this._verticalSplitContainer.TabIndex = 0;
			// 
			// _tableLayouOuter
			// 
			this._tableLayouOuter.AutoSize = true;
			this._tableLayouOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayouOuter.ColumnCount = 2;
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayouOuter.Controls.Add(this._actionsFlowLayoutPanel, 0, 1);
			this._tableLayouOuter.Controls.Add(this.flowLayoutPanel3, 1, 1);
			this._tableLayouOuter.Controls.Add(this._horizontalSplitContainer, 0, 0);
			this._tableLayouOuter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayouOuter.Location = new System.Drawing.Point(0, 0);
			this._tableLayouOuter.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayouOuter.Name = "_tableLayouOuter";
			this._tableLayouOuter.RowCount = 2;
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayouOuter.Size = new System.Drawing.Size(562, 603);
			this._tableLayouOuter.TabIndex = 0;
			// 
			// _actionsFlowLayoutPanel
			// 
			this._actionsFlowLayoutPanel.AutoSize = true;
			this._actionsFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._actionsFlowLayoutPanel.Controls.Add(this._btnAccept);
			this._actionsFlowLayoutPanel.Controls.Add(this._btnSubmitForApproval);
			this._actionsFlowLayoutPanel.Controls.Add(this._btnReject);
			this._actionsFlowLayoutPanel.Controls.Add(this._btnSave);
			this._actionsFlowLayoutPanel.Controls.Add(this._btnSkip);
			this._actionsFlowLayoutPanel.Controls.Add(this._protocolNextItem);
			this._actionsFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._actionsFlowLayoutPanel.Location = new System.Drawing.Point(3, 548);
			this._actionsFlowLayoutPanel.Name = "_actionsFlowLayoutPanel";
			this._actionsFlowLayoutPanel.Size = new System.Drawing.Size(469, 52);
			this._actionsFlowLayoutPanel.TabIndex = 1;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(3, 3);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 0;
			this._btnAccept.Text = "Verify";
			this._btnAccept.UseVisualStyleBackColor = true;
			this._btnAccept.Click += new System.EventHandler(this._btnAccept_Click);
			// 
			// _btnSubmitForApproval
			// 
			this._btnSubmitForApproval.Location = new System.Drawing.Point(84, 3);
			this._btnSubmitForApproval.Name = "_btnSubmitForApproval";
			this._btnSubmitForApproval.Size = new System.Drawing.Size(75, 23);
			this._btnSubmitForApproval.TabIndex = 1;
			this._btnSubmitForApproval.Text = "For Review";
			this._btnSubmitForApproval.UseVisualStyleBackColor = true;
			this._btnSubmitForApproval.Click += new System.EventHandler(this._btnSubmitForApproval_Click);
			// 
			// _btnReject
			// 
			this._btnReject.Location = new System.Drawing.Point(165, 3);
			this._btnReject.Name = "_btnReject";
			this._btnReject.Size = new System.Drawing.Size(75, 23);
			this._btnReject.TabIndex = 2;
			this._btnReject.Text = "Reject";
			this._btnReject.UseVisualStyleBackColor = true;
			this._btnReject.Click += new System.EventHandler(this._btnReject_Click);
			// 
			// _btnSave
			// 
			this._btnSave.Location = new System.Drawing.Point(246, 3);
			this._btnSave.Name = "_btnSave";
			this._btnSave.Size = new System.Drawing.Size(75, 23);
			this._btnSave.TabIndex = 3;
			this._btnSave.Text = "Save";
			this._btnSave.UseVisualStyleBackColor = true;
			this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(327, 3);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 4;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _protocolNextItem
			// 
			this._protocolNextItem.AutoSize = true;
			this._protocolNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolNextItem.Location = new System.Drawing.Point(3, 32);
			this._protocolNextItem.Name = "_protocolNextItem";
			this._protocolNextItem.Size = new System.Drawing.Size(104, 17);
			this._protocolNextItem.TabIndex = 5;
			this._protocolNextItem.Text = "Go To Next Item";
			this._protocolNextItem.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.AutoSize = true;
			this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel3.Controls.Add(this._btnCancel);
			this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel3.Location = new System.Drawing.Point(478, 548);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.Size = new System.Drawing.Size(81, 52);
			this.flowLayoutPanel3.TabIndex = 2;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(3, 3);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 0;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _horizontalSplitContainer
			// 
			this._tableLayouOuter.SetColumnSpan(this._horizontalSplitContainer, 2);
			this._horizontalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._horizontalSplitContainer.Location = new System.Drawing.Point(3, 3);
			this._horizontalSplitContainer.Name = "_horizontalSplitContainer";
			this._horizontalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _horizontalSplitContainer.Panel1
			// 
			this._horizontalSplitContainer.Panel1.Controls.Add(this._protocolledProcedures);
			this._horizontalSplitContainer.Panel1.Controls.Add(this._protocolEditorPanel);
			// 
			// _horizontalSplitContainer.Panel2
			// 
			this._horizontalSplitContainer.Panel2.Controls.Add(this._orderNotesGroupBox);
			this._horizontalSplitContainer.Size = new System.Drawing.Size(556, 539);
			this._horizontalSplitContainer.SplitterDistance = 412;
			this._horizontalSplitContainer.TabIndex = 0;
			// 
			// _protocolledProcedures
			// 
			this._protocolledProcedures.AutoSize = true;
			this._protocolledProcedures.BackColor = System.Drawing.SystemColors.Control;
			this._protocolledProcedures.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._protocolledProcedures.ForeColor = System.Drawing.SystemColors.ControlText;
			this._protocolledProcedures.Location = new System.Drawing.Point(3, 0);
			this._protocolledProcedures.Name = "_protocolledProcedures";
			this._protocolledProcedures.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._protocolledProcedures.Size = new System.Drawing.Size(179, 19);
			this._protocolledProcedures.TabIndex = 0;
			this._protocolledProcedures.Text = "Protocolled Procedure(s): ";
			// 
			// _protocolEditorPanel
			// 
			this._protocolEditorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._protocolEditorPanel.Location = new System.Drawing.Point(3, 19);
			this._protocolEditorPanel.Margin = new System.Windows.Forms.Padding(0);
			this._protocolEditorPanel.Name = "_protocolEditorPanel";
			this._protocolEditorPanel.Size = new System.Drawing.Size(550, 393);
			this._protocolEditorPanel.TabIndex = 1;
			// 
			// _orderNotesGroupBox
			// 
			this._orderNotesGroupBox.Controls.Add(this._orderNotesPanel);
			this._orderNotesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesGroupBox.Location = new System.Drawing.Point(0, 0);
			this._orderNotesGroupBox.Name = "_orderNotesGroupBox";
			this._orderNotesGroupBox.Size = new System.Drawing.Size(556, 123);
			this._orderNotesGroupBox.TabIndex = 0;
			this._orderNotesGroupBox.TabStop = false;
			this._orderNotesGroupBox.Text = "Notes";
			// 
			// _orderNotesPanel
			// 
			this._orderNotesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesPanel.Location = new System.Drawing.Point(3, 16);
			this._orderNotesPanel.Name = "_orderNotesPanel";
			this._orderNotesPanel.Size = new System.Drawing.Size(550, 104);
			this._orderNotesPanel.TabIndex = 0;
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(556, 603);
			this._rightHandPanel.TabIndex = 0;
			// 
			// _orderSummaryPanel
			// 
			this._orderSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderSummaryPanel.Location = new System.Drawing.Point(3, 3);
			this._orderSummaryPanel.Name = "_orderSummaryPanel";
			this._orderSummaryPanel.Size = new System.Drawing.Size(1122, 89);
			this._orderSummaryPanel.TabIndex = 0;
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
			this._statusText.Size = new System.Drawing.Size(1122, 19);
			this._statusText.TabIndex = 1;
			this._statusText.Text = "Protocolling from X worklist - Y items available - Z items completed";
			// 
			// ProtocollingComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._overviewLayoutPanel);
			this.Name = "ProtocollingComponentControl";
			this.Size = new System.Drawing.Size(1128, 729);
			this._overviewLayoutPanel.ResumeLayout(false);
			this._overviewLayoutPanel.PerformLayout();
			this._verticalSplitContainer.Panel1.ResumeLayout(false);
			this._verticalSplitContainer.Panel1.PerformLayout();
			this._verticalSplitContainer.Panel2.ResumeLayout(false);
			this._verticalSplitContainer.ResumeLayout(false);
			this._tableLayouOuter.ResumeLayout(false);
			this._tableLayouOuter.PerformLayout();
			this._actionsFlowLayoutPanel.ResumeLayout(false);
			this._actionsFlowLayoutPanel.PerformLayout();
			this.flowLayoutPanel3.ResumeLayout(false);
			this._horizontalSplitContainer.Panel1.ResumeLayout(false);
			this._horizontalSplitContainer.Panel1.PerformLayout();
			this._horizontalSplitContainer.Panel2.ResumeLayout(false);
			this._horizontalSplitContainer.ResumeLayout(false);
			this._orderNotesGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _overviewLayoutPanel;
        private System.Windows.Forms.SplitContainer _verticalSplitContainer;
        private System.Windows.Forms.Panel _protocolEditorPanel;
		private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.Panel _orderSummaryPanel;
        private System.Windows.Forms.GroupBox _orderNotesGroupBox;
        private System.Windows.Forms.Panel _orderNotesPanel;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.TableLayoutPanel _tableLayouOuter;
		private System.Windows.Forms.FlowLayoutPanel _actionsFlowLayoutPanel;
		private System.Windows.Forms.Button _btnAccept;
		private System.Windows.Forms.Button _btnSubmitForApproval;
		private System.Windows.Forms.Button _btnReject;
		private System.Windows.Forms.Button _btnSave;
		private System.Windows.Forms.Button _btnSkip;
		private System.Windows.Forms.CheckBox _protocolNextItem;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Label _protocolledProcedures;
		private System.Windows.Forms.SplitContainer _horizontalSplitContainer;
    }
}
