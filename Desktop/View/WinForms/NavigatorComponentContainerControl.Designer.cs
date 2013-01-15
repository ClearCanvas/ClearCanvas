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
    partial class NavigatorComponentContainerControl
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
            if (disposing)
            {
				if (components != null)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigatorComponentContainerControl));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._treeView = new System.Windows.Forms.TreeView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._contentPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._applyButton = new System.Windows.Forms.Button();
			this._nextButton = new System.Windows.Forms.Button();
			this._backButton = new System.Windows.Forms.Button();
			this._titleBar = new ClearCanvas.Desktop.View.WinForms.TitleBar();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._treeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
			// 
			// _treeView
			// 
			resources.ApplyResources(this._treeView, "_treeView");
			this._treeView.HideSelection = false;
			this._treeView.Name = "_treeView";
			this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeView_AfterSelect);
			this._treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeView_BeforeSelect);
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this._contentPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// _contentPanel
			// 
			resources.ApplyResources(this._contentPanel, "_contentPanel");
			this._contentPanel.Name = "_contentPanel";
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.Controls.Add(this._applyButton);
			this.flowLayoutPanel1.Controls.Add(this._nextButton);
			this.flowLayoutPanel1.Controls.Add(this._backButton);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _cancelButton
			// 
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			resources.ApplyResources(this._okButton, "_okButton");
			this._okButton.Name = "_okButton";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _applyButton
			// 
			resources.ApplyResources(this._applyButton, "_applyButton");
			this._applyButton.Name = "_applyButton";
			this._applyButton.UseVisualStyleBackColor = true;
			// 
			// _nextButton
			// 
			resources.ApplyResources(this._nextButton, "_nextButton");
			this._nextButton.Name = "_nextButton";
			this._nextButton.UseVisualStyleBackColor = true;
			this._nextButton.Click += new System.EventHandler(this._nextButton_Click);
			// 
			// _backButton
			// 
			resources.ApplyResources(this._backButton, "_backButton");
			this._backButton.Name = "_backButton";
			this._backButton.UseVisualStyleBackColor = true;
			this._backButton.Click += new System.EventHandler(this._backButton_Click);
			// 
			// _titleBar
			// 
			resources.ApplyResources(this._titleBar, "_titleBar");
			this._titleBar.Name = "_titleBar";
			// 
			// NavigatorComponentContainerControl
			// 
			this.AcceptButton = this._okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.splitContainer1);
			this.Name = "NavigatorComponentContainerControl";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _contentPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _backButton;
        private System.Windows.Forms.Button _nextButton;
        private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Desktop.View.WinForms.TitleBar _titleBar;
		private System.Windows.Forms.Button _applyButton;
    }
}
