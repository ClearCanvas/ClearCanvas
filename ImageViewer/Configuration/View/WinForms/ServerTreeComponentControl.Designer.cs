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
namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	partial class ServerTreeComponentControl
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

				if (_component.ShowLocalServerNode)
				{
                    _aeTreeView.MouseEnter -= OnMouseEnter;
				}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerTreeComponentControl));
			this._titleBar = new ClearCanvas.Desktop.View.WinForms.TitleBar();
			this._aeTreeView = new System.Windows.Forms.TreeView();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this._stateImageList = new System.Windows.Forms.ImageList(this.components);
			this._serverTools = new System.Windows.Forms.ToolStrip();
			this.SuspendLayout();
			// 
			// _titleBar
			// 
			resources.ApplyResources(this._titleBar, "_titleBar");
			this._titleBar.Name = "_titleBar";
			// 
			// _aeTreeView
			// 
			this._aeTreeView.AllowDrop = true;
			this._aeTreeView.ContextMenuStrip = this._contextMenu;
			resources.ApplyResources(this._aeTreeView, "_aeTreeView");
			this._aeTreeView.HideSelection = false;
			this._aeTreeView.ImageList = this._imageList;
			this._aeTreeView.Name = "_aeTreeView";
			this._aeTreeView.StateImageList = this._stateImageList;
			this._aeTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewNodeMouseDoubleClick);
			// 
			// _contextMenu
			// 
			this._contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._contextMenu.Name = "_contextMenu";
			resources.ApplyResources(this._contextMenu, "_contextMenu");
			// 
			// _imageList
			// 
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			this._imageList.Images.SetKeyName(0, "MyComputer.png");
			this._imageList.Images.SetKeyName(1, "Server.png");
			this._imageList.Images.SetKeyName(2, "ServerGroup.png");
			// 
			// _stateImageList
			// 
			this._stateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_stateImageList.ImageStream")));
			this._stateImageList.TransparentColor = System.Drawing.Color.Transparent;
			this._stateImageList.Images.SetKeyName(0, "Unchecked.bmp");
			this._stateImageList.Images.SetKeyName(1, "PartiallyChecked.bmp");
			this._stateImageList.Images.SetKeyName(2, "Checked.bmp");
			// 
			// _serverTools
			// 
			this._serverTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._serverTools.ImageScalingSize = new System.Drawing.Size(24, 24);
			resources.ApplyResources(this._serverTools, "_serverTools");
			this._serverTools.Name = "_serverTools";
			// 
			// ServerTreeComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._aeTreeView);
			this.Controls.Add(this._serverTools);
			this.Controls.Add(this._titleBar);
			this.Name = "ServerTreeComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private ClearCanvas.Desktop.View.WinForms.TitleBar _titleBar;
        private System.Windows.Forms.TreeView _aeTreeView;
		private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.ToolStrip _serverTools;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.Windows.Forms.ImageList _stateImageList;
    }
}
