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
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof (StartupActionProviderExtensionPoint))]
	public class GlobalHomeStartupActionProvider : IStartupActionProvider
	{
		public string Name
		{
			get { return SR.TitleHome; }
		}

		public string Description
		{
			get { return SR.MessageHomeDescription; }
		}

		public void Startup(IDesktopWindow mainDesktopWindow)
		{
			using (var tool = new GlobalHomeTool())
			{
				tool.SetContext(new StartupActionContext {DesktopWindow = mainDesktopWindow as DesktopWindow});
				tool.Initialize();

				var canShowHome = tool.CanShowHome;

				// allow subclasses opportunity to execute actions before the Home workspace is created
				OnBeforeStartup(mainDesktopWindow, canShowHome);

				// create and show the Home workspace
				if (canShowHome) tool.PerformLaunch();

				// allow subclasses opportunity to execute actions after the Home workspace is created
				OnAfterStartup(mainDesktopWindow, canShowHome);

				// reactivate the Home workspace
				if (canShowHome) tool.Launch();
			}
		}

		protected virtual void OnBeforeStartup(IDesktopWindow mainDesktopWindow, bool canShowHome) {}
		protected virtual void OnAfterStartup(IDesktopWindow mainDesktopWindow, bool canShowHome) {}

		private class StartupActionContext : IDesktopToolContext
		{
			public DesktopWindow DesktopWindow { get; set; }
		}
	}
}