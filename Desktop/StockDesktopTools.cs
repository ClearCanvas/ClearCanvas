#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Implementation of standard desktop tools.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
    public partial class StockDesktopTools
    {
		/// <summary>
		/// Closes the <see	cref="IDesktopWindow"/> that owns this tool.
		/// </summary>
		/// <remarks>
		/// For internal framework use only.
		/// </remarks>
		[MenuAction("exit", "global-menus/MenuFile/MenuCloseWindow", "CloseWindow", KeyStroke = XKeys.Alt | XKeys.F4)]
		[GroupHint("exit", "Application.Exit")]
		[ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWindowTool : Tool<IDesktopToolContext>
        {
			/// <summary>
			/// Constructor.
			/// </summary>
            public CloseWindowTool()
            {
            }
			
			/// <summary>
			/// Closes the <see	cref="IDesktopWindow"/> that owns this tool.
			/// </summary>
			public void CloseWindow()
			{
                this.Context.DesktopWindow.Close(UserInteraction.Allowed, CloseReason.UserInterface);
			}
        }

		/// <summary>
		/// Closes the active <see cref="IWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// For internal framework use only.
		/// </remarks>
		[MenuAction("closeWorkspace", "global-menus/MenuFile/MenuCloseWorkspace", "CloseWorkspace", KeyStroke = XKeys.Control | XKeys.F4)]
        [EnabledStateObserver("closeWorkspace", "Enabled", "EnabledChanged")]
		[GroupHint("closeWorkspace", "Application.Workspace.Close")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWorkspaceTool : Tool<IDesktopToolContext>
        {
            private event EventHandler _enabledChanged;
            
			/// <summary>
			/// Constructor.
			/// </summary>
            public CloseWorkspaceTool()
            {
            }

			/// <summary>
			/// Disposes of this object; override to do custom cleanup.
			/// </summary>
			/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
			protected override void Dispose(bool disposing)
			{
				this.Context.DesktopWindow.Workspaces.ItemOpened -= WorkspacesChanged;
				this.Context.DesktopWindow.Workspaces.ItemClosed -= WorkspacesChanged;
				this.Context.DesktopWindow.Workspaces.ItemActivationChanged -= WorkspacesChanged;

				base.Dispose(disposing);
			}

			///<summary>
			/// Called by the framework to allow the tool to initialize itself.  This method will
			/// be called after <see cref="!:SetContext" /> has been called, which guarantees that 
			/// the tool will have access to its context when this method is called.
			///</summary>
			public override void Initialize()
            {
                base.Initialize();

                this.Context.DesktopWindow.Workspaces.ItemOpened += WorkspacesChanged;
                this.Context.DesktopWindow.Workspaces.ItemClosed += WorkspacesChanged;
                this.Context.DesktopWindow.Workspaces.ItemActivationChanged += WorkspacesChanged;
            }
			
			/// <summary>
			/// Closes the active <see cref="IWorkspace"/>.
			/// </summary>
            public void CloseWorkspace()
            {
                IDesktopWindow window = this.Context.DesktopWindow;
                IWorkspace activeWorkspace = window.ActiveWorkspace;
                if (activeWorkspace != null)
                {
                    activeWorkspace.Close();
                }
            }

			/// <summary>
			/// Gets the enabled state of the tool.
			/// </summary>
            public bool Enabled
            {
                get
                {
                    return this.Context.DesktopWindow.Workspaces.Count > 0 
                        && this.Context.DesktopWindow.ActiveWorkspace.UserClosable;
                }
            }

			/// <summary>
			/// Raised when the <see cref="Enabled"/> property has changed.
			/// </summary>
            public event EventHandler EnabledChanged
            {
                add { _enabledChanged += value; }
                remove { _enabledChanged -= value; }
            }

            private void WorkspacesChanged(object sender, ItemEventArgs<Workspace> e)
            {
                EventsHelper.Fire(_enabledChanged, this, new EventArgs());
            }
        }
    }
}
