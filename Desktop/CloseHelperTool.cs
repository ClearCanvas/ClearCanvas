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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides "Close Assistant" services, which inform the user of workspaces that require attention prior
    /// to a desktop window close or application quit.
    /// </summary>
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class CloseHelperTool : Tool<IDesktopToolContext>
    {
        private Shelf _closeHelperShelf;

		/// <summary>
		/// Constructor.
		/// </summary>
        public CloseHelperTool()
        {

        }

    	///<summary>
    	/// Called by the framework to allow the tool to initialize itself.
    	///</summary>
    	/// <remarks>
		/// This method will be called after <see cref="ITool.SetContext" /> has been called, which guarantees that 
		/// the tool will have access to its context when this method is called.
		/// </remarks>
    	public override void Initialize()
        {
            this.Context.DesktopWindow.Closing += new EventHandler<ClosingEventArgs>(WindowClosingEventHandler);

            base.Initialize();
        }

        private void WindowClosingEventHandler(object sender, ClosingEventArgs e)
        {
            // if interaction not allowed, or already cancelled, don't do anything here
            if (e.Interaction != UserInteraction.Allowed || e.Cancel)
                return;

            // find all the workspaces that can't be closed
            DesktopWindow window = (DesktopWindow)sender;
            bool showHelper = CollectionUtils.Contains<Workspace>(window.Workspaces,
                delegate(Workspace w) { return !w.QueryCloseReady(); });

            if (showHelper)
            {
                e.Cancel = true;
                ShowShelf(window);
            }
        }

        private void ShowShelf(DesktopWindow window)
        {
            // the shelf is not currently open
            if (_closeHelperShelf == null)
            {
                // launch it
                CloseHelperComponent component = new CloseHelperComponent();
                _closeHelperShelf = ApplicationComponent.LaunchAsShelf(window, component,
					SR.TitleCloseAssistant,
					"Close Assistant",
                    ShelfDisplayHint.DockLeft);
                _closeHelperShelf.Closed += delegate { _closeHelperShelf = null; };
            }
            else
            {
                _closeHelperShelf.Activate();
            }

            CloseHelperComponent helper = (CloseHelperComponent)_closeHelperShelf.Component;
            helper.Refresh();
        }
    }
}
