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

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	partial class LocalImageExplorerControl
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
			this.PerformDispose(disposing);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalImageExplorerControl));
			this._folderViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._folderCoordinator = new ClearCanvas.Controls.WinForms.FolderCoordinator(this.components);
			this._splitter = new System.Windows.Forms.Splitter();
			this._folderTreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._btnBack = new System.Windows.Forms.ToolStripSplitButton();
			this._btnForward = new System.Windows.Forms.ToolStripSplitButton();
			this._btnUp = new System.Windows.Forms.ToolStripButton();
			this._separatorScimitar = new System.Windows.Forms.ToolStripSeparator();
			this._btnHome = new System.Windows.Forms.ToolStripButton();
			this._btnRefresh = new System.Windows.Forms.ToolStripButton();
			this._separatorSabre = new System.Windows.Forms.ToolStripSeparator();
			this._btnShowFolders = new System.Windows.Forms.ToolStripButton();
			this._btnViews = new System.Windows.Forms.ToolStripDropDownButton();
			this._mnuTilesView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuIconsView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuListView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuDetailsView = new System.Windows.Forms.ToolStripMenuItem();
			this._addressStrip = new System.Windows.Forms.ToolStrip();
			this._lblAddress = new System.Windows.Forms.ToolStripLabel();
			this._txtAddress = new ClearCanvas.Controls.WinForms.FolderLocationToolStripTextBox();
			this._btnGo = new System.Windows.Forms.ToolStripButton();
			this._largeIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._mediumIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._smallIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._folderView = new ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderView();
			this._folderTree = new ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderTree();
			this._toolStrip.SuspendLayout();
			this._addressStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _folderViewContextMenu
			// 
			this._folderViewContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._folderViewContextMenu.Name = "_folderViewContextMenu";
			resources.ApplyResources(this._folderViewContextMenu, "_folderViewContextMenu");
			this._folderViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._folderViewContextMenu_Opening);
			// 
			// _folderCoordinator
			// 
			this._folderCoordinator.CurrentPidlChanged += new System.EventHandler(this._folderCoordinator_CurrentPidlChanged);
			// 
			// _splitter
			// 
			resources.ApplyResources(this._splitter, "_splitter");
			this._splitter.Name = "_splitter";
			this._splitter.TabStop = false;
			// 
			// _folderTreeContextMenu
			// 
			this._folderTreeContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._folderTreeContextMenu.Name = "_folderTreeContextMenu";
			resources.ApplyResources(this._folderTreeContextMenu, "_folderTreeContextMenu");
			this._folderTreeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._folderTreeContextMenu_Opening);
			// 
			// _toolStrip
			// 
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnBack,
            this._btnForward,
            this._btnUp,
            this._separatorScimitar,
            this._btnHome,
            this._btnRefresh,
            this._separatorSabre,
            this._btnShowFolders,
            this._btnViews});
			resources.ApplyResources(this._toolStrip, "_toolStrip");
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Stretch = true;
			// 
			// _btnBack
			// 
			this._btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnBack, "_btnBack");
			this._btnBack.Name = "_btnBack";
			this._btnBack.ButtonClick += new System.EventHandler(this._btnBack_Click);
			// 
			// _btnForward
			// 
			this._btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnForward, "_btnForward");
			this._btnForward.Name = "_btnForward";
			this._btnForward.ButtonClick += new System.EventHandler(this._btnForward_Click);
			// 
			// _btnUp
			// 
			this._btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnUp, "_btnUp");
			this._btnUp.Name = "_btnUp";
			this._btnUp.Click += new System.EventHandler(this._btnUp_Click);
			// 
			// _separatorScimitar
			// 
			this._separatorScimitar.Name = "_separatorScimitar";
			resources.ApplyResources(this._separatorScimitar, "_separatorScimitar");
			// 
			// _btnHome
			// 
			this._btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnHome, "_btnHome");
			this._btnHome.Name = "_btnHome";
			this._btnHome.Click += new System.EventHandler(this._btnHome_Click);
			// 
			// _btnRefresh
			// 
			this._btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnRefresh, "_btnRefresh");
			this._btnRefresh.Name = "_btnRefresh";
			this._btnRefresh.Click += new System.EventHandler(this._btnRefresh_Click);
			// 
			// _separatorSabre
			// 
			this._separatorSabre.Name = "_separatorSabre";
			resources.ApplyResources(this._separatorSabre, "_separatorSabre");
			// 
			// _btnShowFolders
			// 
			this._btnShowFolders.Checked = true;
			this._btnShowFolders.CheckState = System.Windows.Forms.CheckState.Checked;
			this._btnShowFolders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnShowFolders, "_btnShowFolders");
			this._btnShowFolders.Name = "_btnShowFolders";
			this._btnShowFolders.Click += new System.EventHandler(this._btnShowFolders_Click);
			// 
			// _btnViews
			// 
			this._btnViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnViews.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuTilesView,
            this._mnuIconsView,
            this._mnuListView,
            this._mnuDetailsView});
			resources.ApplyResources(this._btnViews, "_btnViews");
			this._btnViews.Name = "_btnViews";
			// 
			// _mnuTilesView
			// 
			this._mnuTilesView.Name = "_mnuTilesView";
			resources.ApplyResources(this._mnuTilesView, "_mnuTilesView");
			this._mnuTilesView.Click += new System.EventHandler(this._mnuTilesView_Click);
			// 
			// _mnuIconsView
			// 
			this._mnuIconsView.Checked = true;
			this._mnuIconsView.CheckState = System.Windows.Forms.CheckState.Checked;
			this._mnuIconsView.Name = "_mnuIconsView";
			resources.ApplyResources(this._mnuIconsView, "_mnuIconsView");
			this._mnuIconsView.Click += new System.EventHandler(this._mnuIconsView_Click);
			// 
			// _mnuListView
			// 
			this._mnuListView.Name = "_mnuListView";
			resources.ApplyResources(this._mnuListView, "_mnuListView");
			this._mnuListView.Click += new System.EventHandler(this._mnuListView_Click);
			// 
			// _mnuDetailsView
			// 
			this._mnuDetailsView.Name = "_mnuDetailsView";
			resources.ApplyResources(this._mnuDetailsView, "_mnuDetailsView");
			this._mnuDetailsView.Click += new System.EventHandler(this._mnuDetailsView_Click);
			// 
			// _addressStrip
			// 
			this._addressStrip.CanOverflow = false;
			this._addressStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._addressStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._lblAddress,
            this._txtAddress,
            this._btnGo});
			resources.ApplyResources(this._addressStrip, "_addressStrip");
			this._addressStrip.Name = "_addressStrip";
			this._addressStrip.Stretch = true;
			this._addressStrip.TabStop = true;
			// 
			// _lblAddress
			// 
			this._lblAddress.Name = "_lblAddress";
			resources.ApplyResources(this._lblAddress, "_lblAddress");
			// 
			// _txtAddress
			// 
			this._txtAddress.Name = "_txtAddress";
			resources.ApplyResources(this._txtAddress, "_txtAddress");
			this._txtAddress.KeyEnterPressed += new System.EventHandler(this._txtAddress_KeyEnterPressed);
			// 
			// _btnGo
			// 
			this._btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this._btnGo, "_btnGo");
			this._btnGo.Name = "_btnGo";
			this._btnGo.Click += new System.EventHandler(this._btnGo_Click);
			// 
			// _largeIconImageList
			// 
			this._largeIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this._largeIconImageList, "_largeIconImageList");
			this._largeIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _mediumIconImageList
			// 
			this._mediumIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this._mediumIconImageList, "_mediumIconImageList");
			this._mediumIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _smallIconImageList
			// 
			this._smallIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this._smallIconImageList, "_smallIconImageList");
			this._smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _folderView
			// 
			this._folderView.AutoWaitCursor = false;
			this._folderView.ContextMenuStrip = this._folderViewContextMenu;
			resources.ApplyResources(this._folderView, "_folderView");
			this._folderView.FolderCoordinator = this._folderCoordinator;
			this._folderView.Name = "_folderView";
			this._folderView.EndBrowse += new System.EventHandler(this._folderControl_EndBrowse);
			this._folderView.SelectedItemsChanged += new System.EventHandler(this._folderView_SelectedItemsChanged);
			this._folderView.BeginBrowse += new System.EventHandler(this._folderControl_BeginBrowse);
			this._folderView.ItemDoubleClick += new ClearCanvas.Controls.WinForms.FolderViewItemEventHandler(this._folderView_ItemDoubleClick);
			this._folderView.KeyDown += new System.Windows.Forms.KeyEventHandler(this._folderControl_KeyDown);
			this._folderView.ItemKeyEnterPressed += new ClearCanvas.Controls.WinForms.FolderViewItemEventHandler(this._folderView_ItemDoubleClick);
			// 
			// _folderTree
			// 
			this._folderTree.AutoWaitCursor = false;
			this._folderTree.ContextMenuStrip = this._folderTreeContextMenu;
			resources.ApplyResources(this._folderTree, "_folderTree");
			this._folderTree.FolderCoordinator = this._folderCoordinator;
			this._folderTree.Name = "_folderTree";
			this._folderTree.EndBrowse += new System.EventHandler(this._folderControl_EndBrowse);
			this._folderTree.SelectedItemsChanged += new System.EventHandler(this._folderTree_SelectedItemsChanged);
			this._folderTree.BeginBrowse += new System.EventHandler(this._folderControl_BeginBrowse);
			this._folderTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this._folderControl_KeyDown);
			// 
			// LocalImageExplorerControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._folderView);
			this.Controls.Add(this._splitter);
			this.Controls.Add(this._folderTree);
			this.Controls.Add(this._addressStrip);
			this.Controls.Add(this._toolStrip);
			this.Name = "LocalImageExplorerControl";
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this._addressStrip.ResumeLayout(false);
			this._addressStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip _addressStrip;
		private System.Windows.Forms.ToolStripLabel _lblAddress;
		private ClearCanvas.Controls.WinForms.FolderLocationToolStripTextBox _txtAddress;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ToolStripSplitButton _btnBack;
		private System.Windows.Forms.ToolStripSplitButton _btnForward;
		private System.Windows.Forms.ToolStripButton _btnUp;
		private System.Windows.Forms.ToolStripSeparator _separatorScimitar;
		private System.Windows.Forms.ToolStripButton _btnHome;
		private System.Windows.Forms.ToolStripButton _btnRefresh;
		private System.Windows.Forms.ToolStripSeparator _separatorSabre;
		private System.Windows.Forms.ToolStripButton _btnShowFolders;
		private System.Windows.Forms.ToolStripDropDownButton _btnViews;
		private System.Windows.Forms.ToolStripMenuItem _mnuTilesView;
		private System.Windows.Forms.ToolStripMenuItem _mnuIconsView;
		private System.Windows.Forms.ToolStripMenuItem _mnuListView;
		private System.Windows.Forms.ToolStripMenuItem _mnuDetailsView;
		private ClearCanvas.Controls.WinForms.FolderCoordinator _folderCoordinator;
		private ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderTree _folderTree;
		private ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderView _folderView;
		private System.Windows.Forms.ContextMenuStrip _folderViewContextMenu;
		private System.Windows.Forms.ContextMenuStrip _folderTreeContextMenu;
		private System.Windows.Forms.ImageList _largeIconImageList;
		private System.Windows.Forms.ImageList _mediumIconImageList;
		private System.Windows.Forms.ImageList _smallIconImageList;
		private System.Windows.Forms.ToolStripButton _btnGo;
		private System.Windows.Forms.Splitter _splitter;
	}
}
