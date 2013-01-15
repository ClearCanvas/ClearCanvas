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

using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DesktopForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
				if (_mainMenu != null)
				{
					_mainMenu.Dispose();
					_mainMenu = null;
				}

				if (_toolbar != null)
				{
					_toolbar.Dispose();
					_toolbar = null;
				}

				if (_toolStripContainer != null)
				{
					ToolStripSettings.Default.PropertyChanged -= OnToolStripSettingsPropertyChanged;

					_toolStripContainer.Dispose();
					_toolStripContainer = null;
				}

				if (_dockingManager != null)
				{
					_dockingManager.TabControlCreated -= OnDockingManagerTabControlCreated;
					_dockingManager.InnerControl = null;
					_dockingManager.Dispose();
					_dockingManager = null;
				}

				if (_tabbedGroups != null)
				{
					_tabbedGroups.TabControlCreated -= OnTabbedGroupsTabControlCreated;
					_tabbedGroups.Dispose();
					_tabbedGroups = null;
				}
				
				if (_components != null)
					_components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesktopForm));
            this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._tabbedGroups = new Crownwood.DotNetMagic.Controls.TabbedGroups();
            this._toolbar = new System.Windows.Forms.ToolStrip();
            this._layoutTable = new System.Windows.Forms.TableLayoutPanel();
            this._mainMenu = new System.Windows.Forms.MenuStrip();
            this._toolStripContainer.ContentPanel.SuspendLayout();
            this._toolStripContainer.TopToolStripPanel.SuspendLayout();
            this._toolStripContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tabbedGroups)).BeginInit();
            this._layoutTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStripContainer
            // 
            // 
            // _toolStripContainer.ContentPanel
            // 
            this._toolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this._toolStripContainer.ContentPanel.Controls.Add(this._tabbedGroups);
            resources.ApplyResources(this._toolStripContainer.ContentPanel, "_toolStripContainer.ContentPanel");
            resources.ApplyResources(this._toolStripContainer, "_toolStripContainer");
            this._toolStripContainer.Name = "_toolStripContainer";
            // 
            // _toolStripContainer.TopToolStripPanel
            // 
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._toolbar);
            // 
            // _tabbedGroups
            // 
            this._tabbedGroups.AllowDrop = true;
            this._tabbedGroups.AtLeastOneLeaf = true;
            this._tabbedGroups.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this._tabbedGroups, "_tabbedGroups");
            this._tabbedGroups.Name = "_tabbedGroups";
            this._tabbedGroups.OfficeStyleNormal = Crownwood.DotNetMagic.Controls.OfficeStyle.Light;
            this._tabbedGroups.OfficeStyleProminent = Crownwood.DotNetMagic.Controls.OfficeStyle.Light;
            this._tabbedGroups.OfficeStyleSelected = Crownwood.DotNetMagic.Controls.OfficeStyle.Light;
            this._tabbedGroups.ProminentLeaf = null;
            this._tabbedGroups.ResizeBarColor = System.Drawing.SystemColors.Control;
            this._tabbedGroups.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
            // 
            // _toolbar
            // 
            this._toolbar.AllowItemReorder = true;
            resources.ApplyResources(this._toolbar, "_toolbar");
            this._toolbar.ImageScalingSize = new System.Drawing.Size(48, 48);
            this._toolbar.Name = "_toolbar";
            this._toolbar.Stretch = true;
            // 
            // _layoutTable
            // 
            resources.ApplyResources(this._layoutTable, "_layoutTable");
            this._layoutTable.Controls.Add(this._mainMenu, 0, 0);
            this._layoutTable.Controls.Add(this._toolStripContainer, 0, 1);
            this._layoutTable.Name = "_layoutTable";
            // 
            // _mainMenu
            // 
            resources.ApplyResources(this._mainMenu, "_mainMenu");
            this._mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this._mainMenu.Name = "_mainMenu";
            // 
            // DesktopForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._layoutTable);
            this.IsMdiContainer = true;
            this.Name = "DesktopForm";
            this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
            this._toolStripContainer.ContentPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.PerformLayout();
            this._toolStripContainer.ResumeLayout(false);
            this._toolStripContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tabbedGroups)).EndInit();
            this._layoutTable.ResumeLayout(false);
            this._layoutTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.DotNetMagic.Docking.DockingManager _dockingManager;
        private TableLayoutPanel _layoutTable;
        private MenuStrip _mainMenu;
        private ToolStripContainer _toolStripContainer;
        private Crownwood.DotNetMagic.Controls.TabbedGroups _tabbedGroups;
        private ToolStrip _toolbar;
	}
}
