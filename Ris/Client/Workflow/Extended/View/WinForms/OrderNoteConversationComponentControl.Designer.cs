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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    partial class OrderNoteConversationComponentControl
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
			this._componentTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this._replyGroupBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this._onBehalf = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._recipients = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._templateSelectionPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._template = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._softKeyFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this._urgent = new System.Windows.Forms.CheckBox();
			this._replyBody = new System.Windows.Forms.RichTextBox();
			this._notesGroupBox = new System.Windows.Forms.GroupBox();
			this._orderNotesPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._completeButton = new System.Windows.Forms.Button();
			this._componentTableLayout.SuspendLayout();
			this._replyGroupBox.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this._templateSelectionPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this._notesGroupBox.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _componentTableLayout
			// 
			this._componentTableLayout.AutoSize = true;
			this._componentTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._componentTableLayout.ColumnCount = 1;
			this._componentTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._componentTableLayout.Controls.Add(this._replyGroupBox, 0, 1);
			this._componentTableLayout.Controls.Add(this._notesGroupBox, 0, 0);
			this._componentTableLayout.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this._componentTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this._componentTableLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
			this._componentTableLayout.Location = new System.Drawing.Point(0, 0);
			this._componentTableLayout.Margin = new System.Windows.Forms.Padding(0);
			this._componentTableLayout.Name = "_componentTableLayout";
			this._componentTableLayout.RowCount = 3;
			this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._componentTableLayout.Size = new System.Drawing.Size(850, 600);
			this._componentTableLayout.TabIndex = 0;
			// 
			// _replyGroupBox
			// 
			this._replyGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._replyGroupBox.Controls.Add(this.tableLayoutPanel1);
			this._replyGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._replyGroupBox.Location = new System.Drawing.Point(3, 285);
			this._replyGroupBox.Name = "_replyGroupBox";
			this._replyGroupBox.Size = new System.Drawing.Size(844, 276);
			this._replyGroupBox.TabIndex = 0;
			this._replyGroupBox.TabStop = false;
			this._replyGroupBox.Text = "Add a note to the conversation";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 269F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(838, 257);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this._onBehalf);
			this.panel2.Controls.Add(this._recipients);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(572, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(263, 251);
			this.panel2.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(238, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Require Acknowledgement from (Staff or Groups)";
			// 
			// _onBehalf
			// 
			this._onBehalf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._onBehalf.AutoSize = true;
			this._onBehalf.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._onBehalf.DataSource = null;
			this._onBehalf.DisplayMember = "";
			this._onBehalf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._onBehalf.LabelText = "Send on behalf of  (Group)";
			this._onBehalf.Location = new System.Drawing.Point(2, 9);
			this._onBehalf.Margin = new System.Windows.Forms.Padding(2);
			this._onBehalf.Name = "_onBehalf";
			this._onBehalf.Size = new System.Drawing.Size(235, 41);
			this._onBehalf.TabIndex = 0;
			this._onBehalf.Value = null;
			// 
			// _recipients
			// 
			this._recipients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._recipients.Location = new System.Drawing.Point(3, 81);
			this._recipients.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
			this._recipients.Name = "_recipients";
			this._recipients.ReadOnly = false;
			this._recipients.Size = new System.Drawing.Size(234, 167);
			this._recipients.TabIndex = 2;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._templateSelectionPanel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._softKeyFlowPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 2);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(563, 251);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _templateSelectionPanel
			// 
			this._templateSelectionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._templateSelectionPanel.AutoScroll = true;
			this._templateSelectionPanel.Controls.Add(this._template);
			this._templateSelectionPanel.Location = new System.Drawing.Point(3, 3);
			this._templateSelectionPanel.Name = "_templateSelectionPanel";
			this._templateSelectionPanel.Size = new System.Drawing.Size(557, 47);
			this._templateSelectionPanel.TabIndex = 0;
			// 
			// _template
			// 
			this._template.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._template.AutoSize = true;
			this._template.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._template.DataSource = null;
			this._template.DisplayMember = "";
			this._template.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._template.LabelText = "Select an action";
			this._template.Location = new System.Drawing.Point(2, 2);
			this._template.Margin = new System.Windows.Forms.Padding(2);
			this._template.Name = "_template";
			this._template.Size = new System.Drawing.Size(235, 41);
			this._template.TabIndex = 0;
			this._template.Value = null;
			// 
			// _softKeyFlowPanel
			// 
			this._softKeyFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._softKeyFlowPanel.AutoScroll = true;
			this._softKeyFlowPanel.Location = new System.Drawing.Point(3, 56);
			this._softKeyFlowPanel.Name = "_softKeyFlowPanel";
			this._softKeyFlowPanel.Size = new System.Drawing.Size(557, 31);
			this._softKeyFlowPanel.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this._urgent);
			this.panel1.Controls.Add(this._replyBody);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 93);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(557, 155);
			this.panel1.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(30, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Note";
			// 
			// _urgent
			// 
			this._urgent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._urgent.AutoSize = true;
			this._urgent.Location = new System.Drawing.Point(483, 11);
			this._urgent.Name = "_urgent";
			this._urgent.Size = new System.Drawing.Size(58, 17);
			this._urgent.TabIndex = 2;
			this._urgent.Text = "Urgent";
			this._urgent.UseVisualStyleBackColor = true;
			// 
			// _replyBody
			// 
			this._replyBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._replyBody.AutoWordSelection = true;
			this._replyBody.DetectUrls = false;
			this._replyBody.Location = new System.Drawing.Point(3, 31);
			this._replyBody.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
			this._replyBody.Name = "_replyBody";
			this._replyBody.Size = new System.Drawing.Size(539, 133);
			this._replyBody.TabIndex = 1;
			this._replyBody.Text = "";
			// 
			// _notesGroupBox
			// 
			this._notesGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._notesGroupBox.Controls.Add(this._orderNotesPanel);
			this._notesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._notesGroupBox.Location = new System.Drawing.Point(3, 3);
			this._notesGroupBox.Name = "_notesGroupBox";
			this._notesGroupBox.Size = new System.Drawing.Size(844, 276);
			this._notesGroupBox.TabIndex = 2;
			this._notesGroupBox.TabStop = false;
			this._notesGroupBox.Text = "Conversation";
			// 
			// _orderNotesPanel
			// 
			this._orderNotesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesPanel.Location = new System.Drawing.Point(3, 16);
			this._orderNotesPanel.Name = "_orderNotesPanel";
			this._orderNotesPanel.Size = new System.Drawing.Size(838, 257);
			this._orderNotesPanel.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._completeButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 567);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(844, 30);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(766, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _completeButton
			// 
			this._completeButton.AutoSize = true;
			this._completeButton.Location = new System.Drawing.Point(678, 3);
			this._completeButton.Name = "_completeButton";
			this._completeButton.Size = new System.Drawing.Size(82, 23);
			this._completeButton.TabIndex = 0;
			this._completeButton.Text = "Acknowledge";
			this._completeButton.UseVisualStyleBackColor = true;
			this._completeButton.Click += new System.EventHandler(this._completeButton_Click);
			// 
			// OrderNoteConversationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._componentTableLayout);
			this.Name = "OrderNoteConversationComponentControl";
			this.Size = new System.Drawing.Size(850, 600);
			this._componentTableLayout.ResumeLayout(false);
			this._componentTableLayout.PerformLayout();
			this._replyGroupBox.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this._templateSelectionPanel.ResumeLayout(false);
			this._templateSelectionPanel.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this._notesGroupBox.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _componentTableLayout;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _completeButton;
		private System.Windows.Forms.GroupBox _notesGroupBox;
		private System.Windows.Forms.GroupBox _replyGroupBox;
		private System.Windows.Forms.CheckBox _urgent;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _onBehalf;
		private System.Windows.Forms.Panel _orderNotesPanel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.RichTextBox _replyBody;
		private System.Windows.Forms.FlowLayoutPanel _softKeyFlowPanel;
		private ClearCanvas.Desktop.View.WinForms.TableView _recipients;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.FlowLayoutPanel _templateSelectionPanel;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _template;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
    }
}
