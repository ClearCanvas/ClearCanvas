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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.ViewerIntegration
{
	[ExtensionOf(typeof (StartupActionProviderExtensionPoint))]
	public class IntegratedGlobalHomeTool : IStartupActionProvider
	{
		public string Name
		{
			get { return "TitleHome"; }
		}

		public string Description
		{
			get { return "MessageHomeDescription"; }
		}

		protected bool LaunchExplorerAtStartup
		{
			get
			{
				try
				{
					var type = Type.GetType("ClearCanvas.Desktop.Explorer.ExplorerSettings, ClearCanvas.Desktop.Explorer");
					var instance = ApplicationSettingsHelper.GetDefaultInstance(type);
					return (bool) (instance["LaunchAtStartup"]);
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public void Startup(IDesktopWindow mainDesktopWindow)
		{
			if (LicenseInformation.IsFeatureAuthorized(FeatureTokens.RIS.Core))
			{
				new RisViewerStartupActionProvider(this).Startup(mainDesktopWindow);
			}
			else
			{
				ShowExplorer(mainDesktopWindow, false);
			}
		}

		private class RisViewerStartupActionProvider : GlobalHomeStartupActionProvider
		{
			private readonly IntegratedGlobalHomeTool _owner;

			public RisViewerStartupActionProvider(IntegratedGlobalHomeTool owner)
			{
				_owner = owner;
			}

			protected override void OnBeforeStartup(IDesktopWindow mainDesktopWindow, bool canShowHome) {}

			protected override void OnAfterStartup(IDesktopWindow mainDesktopWindow, bool canShowHome)
			{
				_owner.ShowExplorer(mainDesktopWindow, canShowHome);
			}
		}

		private void ShowExplorer(IDesktopWindow mainDesktopWindow, bool canShowHome)
		{
			if (!canShowHome || LaunchExplorerAtStartup)
			{
				ExplorerTool.ShowExplorer(mainDesktopWindow);
			}
		}
	}
}