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

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    partial class ExtensionBrowserControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtensionBrowserControl));
			this._pluginTreeView = new System.Windows.Forms.TreeView();
			this._tabView = new System.Windows.Forms.TabControl();
			this._extPointViewTabPage = new System.Windows.Forms.TabPage();
			this._extPointTreeView = new System.Windows.Forms.TreeView();
			this._pluginViewTabPage = new System.Windows.Forms.TabPage();
			this._tabView.SuspendLayout();
			this._extPointViewTabPage.SuspendLayout();
			this._pluginViewTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pluginTreeView
			// 
			resources.ApplyResources(this._pluginTreeView, "_pluginTreeView");
			this._pluginTreeView.Name = "_pluginTreeView";
			// 
			// _tabView
			// 
			this._tabView.Controls.Add(this._extPointViewTabPage);
			this._tabView.Controls.Add(this._pluginViewTabPage);
			resources.ApplyResources(this._tabView, "_tabView");
			this._tabView.Name = "_tabView";
			this._tabView.SelectedIndex = 0;
			// 
			// _extPointViewTabPage
			// 
			this._extPointViewTabPage.Controls.Add(this._extPointTreeView);
			resources.ApplyResources(this._extPointViewTabPage, "_extPointViewTabPage");
			this._extPointViewTabPage.Name = "_extPointViewTabPage";
			this._extPointViewTabPage.UseVisualStyleBackColor = true;
			// 
			// _extPointTreeView
			// 
			resources.ApplyResources(this._extPointTreeView, "_extPointTreeView");
			this._extPointTreeView.Name = "_extPointTreeView";
			// 
			// _pluginViewTabPage
			// 
			this._pluginViewTabPage.Controls.Add(this._pluginTreeView);
			resources.ApplyResources(this._pluginViewTabPage, "_pluginViewTabPage");
			this._pluginViewTabPage.Name = "_pluginViewTabPage";
			this._pluginViewTabPage.UseVisualStyleBackColor = true;
			// 
			// ExtensionBrowserControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tabView);
			this.Name = "ExtensionBrowserControl";
			this._tabView.ResumeLayout(false);
			this._extPointViewTabPage.ResumeLayout(false);
			this._pluginViewTabPage.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _pluginTreeView;
        private System.Windows.Forms.TabControl _tabView;
        private System.Windows.Forms.TabPage _pluginViewTabPage;
        private System.Windows.Forms.TabPage _extPointViewTabPage;
        private System.Windows.Forms.TreeView _extPointTreeView;
    }
}
