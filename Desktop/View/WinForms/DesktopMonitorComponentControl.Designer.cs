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
    partial class DesktopMonitorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesktopMonitorComponentControl));
			this._windows = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._openWindow = new System.Windows.Forms.Button();
			this._closeWindow = new System.Windows.Forms.Button();
			this._activateWindow = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._closeShelf = new System.Windows.Forms.Button();
			this._hideShelf = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this._activateShelf = new System.Windows.Forms.Button();
			this._showShelf = new System.Windows.Forms.Button();
			this._openShelf = new System.Windows.Forms.Button();
			this._shelves = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label2 = new System.Windows.Forms.Label();
			this._activateWorkspace = new System.Windows.Forms.Button();
			this._closeWorkspace = new System.Windows.Forms.Button();
			this._openWorkspace = new System.Windows.Forms.Button();
			this._workspaces = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._events = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _windows
			// 
			this._windows.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._windows, "_windows");
			this._windows.MultiSelect = false;
			this._windows.Name = "_windows";
			this._windows.ReadOnly = false;
			this._windows.ShowToolbar = false;
			this._windows.SortButtonTooltip = null;
			// 
			// _openWindow
			// 
			resources.ApplyResources(this._openWindow, "_openWindow");
			this._openWindow.Name = "_openWindow";
			this._openWindow.UseVisualStyleBackColor = true;
			this._openWindow.Click += new System.EventHandler(this._openWindow_Click);
			// 
			// _closeWindow
			// 
			resources.ApplyResources(this._closeWindow, "_closeWindow");
			this._closeWindow.Name = "_closeWindow";
			this._closeWindow.UseVisualStyleBackColor = true;
			this._closeWindow.Click += new System.EventHandler(this._closeWindow_Click);
			// 
			// _activateWindow
			// 
			resources.ApplyResources(this._activateWindow, "_activateWindow");
			this._activateWindow.Name = "_activateWindow";
			this._activateWindow.UseVisualStyleBackColor = true;
			this._activateWindow.Click += new System.EventHandler(this._activateWindow_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._closeShelf);
			this.groupBox2.Controls.Add(this._hideShelf);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this._activateShelf);
			this.groupBox2.Controls.Add(this._showShelf);
			this.groupBox2.Controls.Add(this._openShelf);
			this.groupBox2.Controls.Add(this._shelves);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this._activateWorkspace);
			this.groupBox2.Controls.Add(this._closeWorkspace);
			this.groupBox2.Controls.Add(this._openWorkspace);
			this.groupBox2.Controls.Add(this._workspaces);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// _closeShelf
			// 
			resources.ApplyResources(this._closeShelf, "_closeShelf");
			this._closeShelf.Name = "_closeShelf";
			this._closeShelf.UseVisualStyleBackColor = true;
			this._closeShelf.Click += new System.EventHandler(this._closeShelf_Click);
			// 
			// _hideShelf
			// 
			resources.ApplyResources(this._hideShelf, "_hideShelf");
			this._hideShelf.Name = "_hideShelf";
			this._hideShelf.UseVisualStyleBackColor = true;
			this._hideShelf.Click += new System.EventHandler(this._hideShelf_Click);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// _activateShelf
			// 
			resources.ApplyResources(this._activateShelf, "_activateShelf");
			this._activateShelf.Name = "_activateShelf";
			this._activateShelf.UseVisualStyleBackColor = true;
			this._activateShelf.Click += new System.EventHandler(this._activateShelf_Click);
			// 
			// _showShelf
			// 
			resources.ApplyResources(this._showShelf, "_showShelf");
			this._showShelf.Name = "_showShelf";
			this._showShelf.UseVisualStyleBackColor = true;
			this._showShelf.Click += new System.EventHandler(this._showShelf_Click);
			// 
			// _openShelf
			// 
			resources.ApplyResources(this._openShelf, "_openShelf");
			this._openShelf.Name = "_openShelf";
			this._openShelf.UseVisualStyleBackColor = true;
			this._openShelf.Click += new System.EventHandler(this._openShelf_Click);
			// 
			// _shelves
			// 
			this._shelves.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._shelves, "_shelves");
			this._shelves.MultiSelect = false;
			this._shelves.Name = "_shelves";
			this._shelves.ReadOnly = false;
			this._shelves.ShowToolbar = false;
			this._shelves.SortButtonTooltip = null;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _activateWorkspace
			// 
			resources.ApplyResources(this._activateWorkspace, "_activateWorkspace");
			this._activateWorkspace.Name = "_activateWorkspace";
			this._activateWorkspace.UseVisualStyleBackColor = true;
			this._activateWorkspace.Click += new System.EventHandler(this._activateWorkspace_Click);
			// 
			// _closeWorkspace
			// 
			resources.ApplyResources(this._closeWorkspace, "_closeWorkspace");
			this._closeWorkspace.Name = "_closeWorkspace";
			this._closeWorkspace.UseVisualStyleBackColor = true;
			this._closeWorkspace.Click += new System.EventHandler(this._closeWorkspace_Click);
			// 
			// _openWorkspace
			// 
			resources.ApplyResources(this._openWorkspace, "_openWorkspace");
			this._openWorkspace.Name = "_openWorkspace";
			this._openWorkspace.UseVisualStyleBackColor = true;
			this._openWorkspace.Click += new System.EventHandler(this._openWorkspace_Click);
			// 
			// _workspaces
			// 
			this._workspaces.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._workspaces, "_workspaces");
			this._workspaces.MultiSelect = false;
			this._workspaces.Name = "_workspaces";
			this._workspaces.ReadOnly = false;
			this._workspaces.ShowToolbar = false;
			this._workspaces.SortButtonTooltip = null;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._events);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// _events
			// 
			this._events.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._events, "_events");
			this._events.MultiSelect = false;
			this._events.Name = "_events";
			this._events.ReadOnly = false;
			this._events.ShowToolbar = false;
			this._events.SortButtonTooltip = null;
			// 
			// DesktopMonitorComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._activateWindow);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this._closeWindow);
			this.Controls.Add(this._openWindow);
			this.Controls.Add(this._windows);
			this.Name = "DesktopMonitorComponentControl";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

        private TableView _windows;
        private System.Windows.Forms.Button _activateWindow;
        private System.Windows.Forms.Button _closeWindow;
        private System.Windows.Forms.Button _openWindow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button _activateWorkspace;
        private System.Windows.Forms.Button _closeWorkspace;
        private System.Windows.Forms.Button _openWorkspace;
        private TableView _workspaces;
        private System.Windows.Forms.Button _closeShelf;
        private System.Windows.Forms.Button _hideShelf;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _activateShelf;
        private System.Windows.Forms.Button _showShelf;
        private System.Windows.Forms.Button _openShelf;
        private TableView _shelves;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private TableView _events;
    }
}
