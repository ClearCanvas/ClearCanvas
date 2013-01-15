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
    partial class FoldersComponentControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoldersComponentControl));
            this._folderTree = new System.Windows.Forms.TreeView();
            this._folderTreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._toolbar = new System.Windows.Forms.ToolStrip();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _folderTree
            // 
            this._folderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._folderTree.HideSelection = false;
            this._folderTree.ImageIndex = 0;
            this._folderTree.ImageList = this._folderTreeImageList;
            this._folderTree.Location = new System.Drawing.Point(2, 27);
            this._folderTree.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._folderTree.Name = "_folderTree";
            this._folderTree.SelectedImageIndex = 1;
            this._folderTree.ShowRootLines = false;
            this._folderTree.Size = new System.Drawing.Size(172, 384);
            this._folderTree.TabIndex = 0;
            this._folderTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._folderTree_AfterSelect);
            // 
            // _folderTreeImageList
            // 
            this._folderTreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_folderTreeImageList.ImageStream")));
            this._folderTreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this._folderTreeImageList.Images.SetKeyName(0, "");
            this._folderTreeImageList.Images.SetKeyName(1, "");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._folderTree, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._toolbar, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(176, 409);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _toolbar
            // 
            this._toolbar.Location = new System.Drawing.Point(0, 0);
            this._toolbar.Name = "_toolbar";
            this._toolbar.Size = new System.Drawing.Size(176, 25);
            this._toolbar.TabIndex = 1;
            this._toolbar.Text = "toolStrip1";
            // 
            // FoldersComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FoldersComponentControl";
            this.Size = new System.Drawing.Size(176, 409);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _folderTree;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip _toolbar;
        private System.Windows.Forms.ImageList _folderTreeImageList;
    }
}
