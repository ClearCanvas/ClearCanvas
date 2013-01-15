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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
	[MenuAction("show", "global-menus/MenuTools/MenuUtilities/MenuExtensionBrowser", "Show", Flags = ClickActionFlags.CheckAction)]
	[ActionPermission("show", AuthorityTokens.Desktop.ExtensionBrowser)]
	[GroupHint("show", "Application.Browsing.Extensions")]

    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ExtensionBrowserTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

        public ExtensionBrowserTool()
		{
        }

        public void Show()
        {
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
            {
				ExtensionBrowserComponent browser = new ExtensionBrowserComponent();

            	_shelf = ApplicationComponent.LaunchAsShelf(
            		this.Context.DesktopWindow,
            		browser,
            		SR.TitleExtensionBrowser,
            		"Extension Browser",
            		ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide);

				_shelf.Closed += OnShelfClosed;
            }
        }

		private void OnShelfClosed(object sender, ClosedEventArgs e)
		{
			_shelf = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (_shelf != null)
				_shelf.Closed -= OnShelfClosed;

			base.Dispose(disposing);
		}
    }
}
