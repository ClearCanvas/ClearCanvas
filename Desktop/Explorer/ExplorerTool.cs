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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionPoint()]
	public sealed class HealthcareArtifactExplorerExtensionPoint : ExtensionPoint<IHealthcareArtifactExplorer>
	{
	}

	[MenuAction("show", "global-menus/MenuFile/MenuExplorer", "Show", KeyStroke = XKeys.Control | XKeys.E)]
	[IconSet("show", "Icons.ExplorerToolSmall.png", "Icons.ExplorerToolMedium.png", "Icons.ExplorerToolLarge.png")]
	[GroupHint("show", "Application.Browsing.Explorer")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class ExplorerTool : Tool<IDesktopToolContext>
	{
    	private bool _isMainWindowTool = false;
		private static int _toolCount = 0;

		private static IDesktopObject _desktopObject;

		public ExplorerTool()
		{
		}

		private static bool IsWorkspaceUnclosable
		{
			get
			{
				return ExplorerLocalSettings.Default.ExplorerIsPrimary;
			}
		}

		private static bool IsWorkspaceClosable
		{
			get
			{
				return !ExplorerLocalSettings.Default.ExplorerIsPrimary;
			}
		}

		private static bool LaunchAsShelf
		{
			get
			{
				if (ExplorerLocalSettings.Default.ExplorerIsPrimary)
					return false;

				return ExplorerSettings.Default.LaunchAsShelf;
			}	
		}

		private static bool LaunchAtStartup
		{
			get
			{
				if (ExplorerLocalSettings.Default.ExplorerIsPrimary)
					return true;

				return ExplorerSettings.Default.LaunchAtStartup;
			}
		}

		public override IActionSet Actions
		{
			get
			{
				if (IsWorkspaceUnclosable || !_isMainWindowTool || GetExplorers().Count == 0)
				{
					return new ActionSet();
				}
				else
				{
					return base.Actions;
				}
			}
		}

		public override void Initialize()
		{
			if (_toolCount == 0)
				_isMainWindowTool = true;
			
			++_toolCount;

			if (_isMainWindowTool && LaunchAtStartup)
				ShowInternal();

			base.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			--_toolCount;

			if (_isMainWindowTool)
				CloseChildDesktopWindows();

			base.Dispose(disposing);
		}

		public void Show()
		{
			BlockingOperation.Run(delegate { ShowInternal(); });
		}

		private void ShowInternal()
		{
			if (_desktopObject != null)
			{
				_desktopObject.Activate();
				return;
			}

			List<TabPage> pages = new List<TabPage>();
			foreach (IHealthcareArtifactExplorer explorer in GetExplorers())
			{
				IApplicationComponent component = explorer.Component;
				if (component != null)
					pages.Add(new TabPage(explorer.Name, component));
			}

			if (pages.Count == 0)
				return;

			TabComponentContainer container = new TabComponentContainer();
			foreach (TabPage page in pages)
				container.Pages.Add(page);

			if (LaunchAsShelf)
			{
				ShelfCreationArgs args = new ShelfCreationArgs();
				args.Component = container;
				args.Title = SR.TitleExplorer;
				args.Name = "Explorer";
				args.DisplayHint = ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide;

				_desktopObject = ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow, args);
			}
			else
			{
				WorkspaceCreationArgs args = new WorkspaceCreationArgs();
				args.Component = container;
				args.Title = SR.TitleExplorer;
				args.Name = "Explorer";
				args.UserClosable = IsWorkspaceClosable;

				_desktopObject = ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, args);
			}

			_desktopObject.Closed += delegate { _desktopObject = null; };
		}

		internal static List<IHealthcareArtifactExplorer> GetExplorers()
		{
			List<IHealthcareArtifactExplorer> healthcareArtifactExplorers = new List<IHealthcareArtifactExplorer>();
			try
			{
				HealthcareArtifactExplorerExtensionPoint xp = new HealthcareArtifactExplorerExtensionPoint();
				object[] extensions = xp.CreateExtensions();
				foreach (IHealthcareArtifactExplorer explorer in extensions)
				{
					if (explorer.IsAvailable)
						healthcareArtifactExplorers.Add(explorer);
				}
			}
			catch (NotSupportedException)
			{
			}

			return healthcareArtifactExplorers;
		}

		private void CloseChildDesktopWindows()
		{
			List<DesktopWindow> childWindowsToClose = new List<DesktopWindow>();

			// We can't just iterate through the collection and close them,
			// because closing a window changes the collection.  So instead,
			// we create a list of the child windows then iterate through
			// that list and close them.
			foreach (DesktopWindow window in Application.DesktopWindows)
			{
				// Child windows are all those other than the one
				// this tool is hosted by
				if (window != this.Context.DesktopWindow)
					childWindowsToClose.Add(window);
			}

			foreach (DesktopWindow window in childWindowsToClose)
				window.Close();
		}
	}
}
