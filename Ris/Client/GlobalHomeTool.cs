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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class FolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>
	{
	}

	[MenuAction("launch", "global-menus/MenuFile/MenuHome", "Launch")]
	[Tooltip("launch", "TooltipGoToHomepage")]
	[IconSet("launch", "Icons.GlobalHomeToolSmall.png", "Icons.GlobalHomeToolMedium.png", "Icons.GlobalHomeToolLarge.png")]
	[VisibleStateObserver("launch", "Visible", "VisibleChanged")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View)]

	[MenuAction("toggleDowntimeMode", "global-menus/MenuTools/MenuDowntime/MenuDowntimeRecoveryMode", "ToggleDowntimeMode", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("toggleDowntimeMode", "DowntimeModeChecked", "DowntimeModeCheckedChanged")]
	[ActionPermission("toggleDowntimeMode", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View,
		ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class GlobalHomeTool : WorklistPreviewHomeTool<FolderSystemExtensionPoint>
	{
		private static IWorkspace _workspace;
		private static DesktopWindow _risWindow;

		protected override IWorkspace Workspace
		{
			get { return _workspace; }
			set { _workspace = value; }
		}

		/// <summary>
		/// Gets whether or not user is staff, has appropriate authorization, and user setting says to show home
		/// </summary>
		internal bool CanShowHome
		{
			get
			{
				return LoginSession.Current != null && LoginSession.Current.IsStaff
						&& Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.HomePage.View)
						&& HomePageSettings.Default.ShowHomepageOnStartUp;
			}
		}

		internal void PerformLaunch()
		{
			// automatically launch home page on startup, only if current user is a Staff
			if (CanShowHome && _risWindow == null)
			{
				Launch();

				// bug 3087: remember which window is the RIS window, so that we don't launch this
				// in the viewer window
				_risWindow = this.Context.DesktopWindow;
			}
		}

		public void ToggleDowntimeMode()
		{
			DowntimeRecovery.InDowntimeRecoveryMode = !DowntimeRecovery.InDowntimeRecoveryMode;
			this.Restart();
		}

		public event EventHandler DowntimeModeCheckedChanged
		{
			add { DowntimeRecovery.InDowntimeRecoveryModeChanged += value; }
			remove { DowntimeRecovery.InDowntimeRecoveryModeChanged -= value; }
		}

		public bool DowntimeModeChecked
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode; }
		}

		public override string Title
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode ? string.Format("{0} ({1})", SR.TitleHome, SR.TitleDowntimeRecovery) : SR.TitleHome; }
		}

		protected override bool IsUserClosableWorkspace
		{
			get { return !HomePageSettings.Default.PreventHomepageFromClosing; }
		}

		public bool Visible
		{
			get
			{
				// bug 3087: only visible in the RIS window
				return LoginSession.Current != null && LoginSession.Current.IsStaff &&
					(_risWindow == null || _risWindow == this.Context.DesktopWindow);
			}
		}

		public event EventHandler VisibleChanged
		{
			add { }
			remove { }
		}
	}
}
