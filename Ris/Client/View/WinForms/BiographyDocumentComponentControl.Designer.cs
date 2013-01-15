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
    partial class BiographyDocumentComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BiographyDocumentComponentControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._documentList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._previewButton = new System.Windows.Forms.Button();
            this._category = new System.Windows.Forms.Label();
            this._reason = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._institution = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._previewImage = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._nextPageButton = new System.Windows.Forms.Button();
            this._prevPageButton = new System.Windows.Forms.Button();
            this._pageIndicator = new System.Windows.Forms.TextBox();
            this._printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this._documentDate = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._previewImage)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._documentList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(853, 716);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 0;
            // 
            // _documentList
            // 
            this._documentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._documentList.Location = new System.Drawing.Point(0, 0);
            this._documentList.MenuModel = null;
            this._documentList.Name = "_documentList";
            this._documentList.ReadOnly = false;
            this._documentList.Selection = selection1;
            this._documentList.Size = new System.Drawing.Size(853, 239);
            this._documentList.TabIndex = 0;
            this._documentList.Table = null;
            this._documentList.ToolbarModel = null;
            this._documentList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._documentList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._documentList.ItemDoubleClicked += new System.EventHandler(this._documentList_ItemDoubleClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(853, 473);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._documentDate);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this._previewButton);
            this.panel3.Controls.Add(this._category);
            this.panel3.Controls.Add(this._reason);
            this.panel3.Controls.Add(this._institution);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(394, 467);
            this.panel3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 13);
            this.label2.TabIndex = 58;
            this.label2.Text = "Created by John Smith on 5/4/2007 12:23pm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Document: 8 x 11 in";
            // 
            // _previewButton
            // 
            this._previewButton.Location = new System.Drawing.Point(7, 239);
            this._previewButton.Name = "_previewButton";
            this._previewButton.Size = new System.Drawing.Size(85, 23);
            this._previewButton.TabIndex = 55;
            this._previewButton.Text = "Preview";
            this._previewButton.UseVisualStyleBackColor = true;
            this._previewButton.Click += new System.EventHandler(this._previewButton_Click);
            // 
            // _category
            // 
            this._category.AutoSize = true;
            this._category.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._category.Location = new System.Drawing.Point(3, 15);
            this._category.Name = "_category";
            this._category.Size = new System.Drawing.Size(95, 23);
            this._category.TabIndex = 5;
            this._category.Text = "Category";
            // 
            // _reason
            // 
            this._reason.LabelText = "Reason:";
            this._reason.Location = new System.Drawing.Point(2, 139);
            this._reason.Margin = new System.Windows.Forms.Padding(2);
            this._reason.Mask = "";
            this._reason.Name = "_reason";
            this._reason.ReadOnly = true;
            this._reason.Size = new System.Drawing.Size(226, 41);
            this._reason.TabIndex = 6;
            this._reason.Value = null;
            // 
            // _institution
            // 
            this._institution.LabelText = "Institution:";
            this._institution.Location = new System.Drawing.Point(2, 49);
            this._institution.Margin = new System.Windows.Forms.Padding(2);
            this._institution.Mask = "";
            this._institution.Name = "_institution";
            this._institution.ReadOnly = true;
            this._institution.Size = new System.Drawing.Size(150, 41);
            this._institution.TabIndex = 0;
            this._institution.Value = null;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this._previewImage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(403, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(447, 467);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // _previewImage
            // 
            this._previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewImage.Image = ((System.Drawing.Image)(resources.GetObject("_previewImage.Image")));
            this._previewImage.Location = new System.Drawing.Point(2, 2);
            this._previewImage.Margin = new System.Windows.Forms.Padding(2);
            this._previewImage.Name = "_previewImage";
            this._previewImage.Size = new System.Drawing.Size(443, 433);
            this._previewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._previewImage.TabIndex = 10;
            this._previewImage.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.Controls.Add(this._nextPageButton);
            this.panel1.Controls.Add(this._prevPageButton);
            this.panel1.Controls.Add(this._pageIndicator);
            this.panel1.Location = new System.Drawing.Point(178, 440);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(90, 24);
            this.panel1.TabIndex = 9;
            // 
            // _nextPageButton
            // 
            this._nextPageButton.Image = ((System.Drawing.Image)(resources.GetObject("_nextPageButton.Image")));
            this._nextPageButton.Location = new System.Drawing.Point(67, 1);
            this._nextPageButton.Margin = new System.Windows.Forms.Padding(1);
            this._nextPageButton.Name = "_nextPageButton";
            this._nextPageButton.Size = new System.Drawing.Size(21, 19);
            this._nextPageButton.TabIndex = 48;
            this._nextPageButton.UseVisualStyleBackColor = true;
            this._nextPageButton.Click += new System.EventHandler(this._nextPageButton_Click);
            // 
            // _prevPageButton
            // 
            this._prevPageButton.Image = ((System.Drawing.Image)(resources.GetObject("_prevPageButton.Image")));
            this._prevPageButton.Location = new System.Drawing.Point(1, 1);
            this._prevPageButton.Margin = new System.Windows.Forms.Padding(1);
            this._prevPageButton.Name = "_prevPageButton";
            this._prevPageButton.Size = new System.Drawing.Size(21, 19);
            this._prevPageButton.TabIndex = 47;
            this._prevPageButton.UseVisualStyleBackColor = true;
            this._prevPageButton.Click += new System.EventHandler(this._prevPageButton_Click);
            // 
            // _pageIndicator
            // 
            this._pageIndicator.Location = new System.Drawing.Point(24, 0);
            this._pageIndicator.Margin = new System.Windows.Forms.Padding(1);
            this._pageIndicator.Name = "_pageIndicator";
            this._pageIndicator.Size = new System.Drawing.Size(41, 20);
            this._pageIndicator.TabIndex = 46;
            this._pageIndicator.Text = "1 of 1";
            this._pageIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _printPreviewDialog
            // 
            this._printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this._printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this._printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this._printPreviewDialog.Enabled = true;
            this._printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("_printPreviewDialog.Icon")));
            this._printPreviewDialog.Name = "_printPreviewDialog";
            this._printPreviewDialog.Visible = false;
            // 
            // _documentDate
            // 
            this._documentDate.LabelText = "Document Date:";
            this._documentDate.Location = new System.Drawing.Point(2, 94);
            this._documentDate.Margin = new System.Windows.Forms.Padding(2);
            this._documentDate.Mask = "";
            this._documentDate.Name = "_documentDate";
            this._documentDate.ReadOnly = true;
            this._documentDate.Size = new System.Drawing.Size(150, 41);
            this._documentDate.TabIndex = 59;
            this._documentDate.Value = null;
            // 
            // BiographyDocumentComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BiographyDocumentComponentControl";
            this.Size = new System.Drawing.Size(853, 716);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._previewImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _documentList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PrintPreviewDialog _printPreviewDialog;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label _category;
        private ClearCanvas.Desktop.View.WinForms.TextField _reason;
        private ClearCanvas.Desktop.View.WinForms.TextField _institution;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _nextPageButton;
        private System.Windows.Forms.Button _prevPageButton;
        private System.Windows.Forms.TextBox _pageIndicator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _previewButton;
        private System.Windows.Forms.PictureBox _previewImage;
        private ClearCanvas.Desktop.View.WinForms.TextField _documentDate;
    }
}
