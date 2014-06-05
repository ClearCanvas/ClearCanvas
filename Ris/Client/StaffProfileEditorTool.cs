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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Allows a user to edit his own staff profile.
	/// </summary>
	[MenuAction("launch", "global-menus/MenuTools/MenuUserProfile/MenuStaffProfile", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.StaffProfile.View)]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.StaffProfile.Update)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
	public class StaffProfileEditorTool : Tool<IDesktopToolContext>
	{
		public void Launch()
		{
			if(LoginSession.Current == null)
			{
				this.Context.DesktopWindow.ShowMessageBox(
					SR.MessageFeatureRequiresWorkstationRestart,
					MessageBoxActions.Ok);
				return;
			}

			try
			{
				if (LoginSession.Current.Staff == null)
				{
					this.Context.DesktopWindow.ShowMessageBox(
						string.Format(SR.FormatUserHasNoStaffProfile, LoginSession.Current.UserName),
						MessageBoxActions.Ok);
					return;
				}

				var component = new StaffEditorComponent(LoginSession.Current.Staff.StaffRef);

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitleStaff);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
