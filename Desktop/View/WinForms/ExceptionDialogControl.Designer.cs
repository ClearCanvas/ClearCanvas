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

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class ExceptionDialogControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialogControl));
			this._flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._detailButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._quitButton = new System.Windows.Forms.Button();
			this._description = new System.Windows.Forms.TextBox();
			this._warningIcon = new System.Windows.Forms.PictureBox();
			this._detailTree = new System.Windows.Forms.TreeView();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._messagePane = new System.Windows.Forms.Panel();
			this._flowLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._warningIcon)).BeginInit();
			this._contextMenu.SuspendLayout();
			this._messagePane.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowLayoutPanel
			// 
			resources.ApplyResources(this._flowLayoutPanel, "_flowLayoutPanel");
			this._flowLayoutPanel.Controls.Add(this._detailButton);
			this._flowLayoutPanel.Controls.Add(this._okButton);
			this._flowLayoutPanel.Controls.Add(this._quitButton);
			this._flowLayoutPanel.Name = "_flowLayoutPanel";
			// 
			// _detailButton
			// 
			resources.ApplyResources(this._detailButton, "_detailButton");
			this._detailButton.Name = "_detailButton";
			this._detailButton.UseVisualStyleBackColor = true;
			this._detailButton.Click += new System.EventHandler(this._detailButton_Click);
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// _quitButton
			// 
			resources.ApplyResources(this._quitButton, "_quitButton");
			this._quitButton.Name = "_quitButton";
			this._quitButton.UseVisualStyleBackColor = true;
			// 
			// _description
			// 
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			// 
			// _warningIcon
			// 
			resources.ApplyResources(this._warningIcon, "_warningIcon");
			this._warningIcon.Image = global::ClearCanvas.Desktop.View.WinForms.SR.Stop;
			this._warningIcon.Name = "_warningIcon";
			this._warningIcon.TabStop = false;
			// 
			// _detailTree
			// 
			this._detailTree.ContextMenuStrip = this._contextMenu;
			resources.ApplyResources(this._detailTree, "_detailTree");
			this._detailTree.Name = "_detailTree";
			// 
			// _contextMenu
			// 
			this._contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
			this._contextMenu.Name = "contextMenuStrip1";
			resources.ApplyResources(this._contextMenu, "_contextMenu");
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// _messagePane
			// 
			this._messagePane.Controls.Add(this._description);
			this._messagePane.Controls.Add(this._warningIcon);
			resources.ApplyResources(this._messagePane, "_messagePane");
			this._messagePane.Name = "_messagePane";
			// 
			// ExceptionDialogControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._messagePane);
			this.Controls.Add(this._flowLayoutPanel);
			this.Controls.Add(this._detailTree);
			this.MinimumSize = new System.Drawing.Size(440, 152);
			this.Name = "ExceptionDialogControl";
			this._flowLayoutPanel.ResumeLayout(false);
			this._flowLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._warningIcon)).EndInit();
			this._contextMenu.ResumeLayout(false);
			this._messagePane.ResumeLayout(false);
			this._messagePane.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TextBox _description;
        private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanel;
        private System.Windows.Forms.Button _detailButton;
        private System.Windows.Forms.Button _quitButton;
        private System.Windows.Forms.PictureBox _warningIcon;
        private System.Windows.Forms.TreeView _detailTree;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Panel _messagePane;


    }
}
