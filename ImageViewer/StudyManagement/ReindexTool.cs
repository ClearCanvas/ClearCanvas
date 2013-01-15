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
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    [MenuAction("reindex", "global-menus/MenuTools/MenuReindex", "Reindex")]
    [ViewerActionPermissionAttribute("reindex", AuthorityTokens.Administration.ReIndex)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class ReindexTool : Tool<IDesktopToolContext>
    {
        public override IActionSet Actions
        {
            get
            {
                if (!WorkItemActivityMonitor.IsSupported)
                    return new ActionSet();

                return base.Actions;
            }
        }

        public void Reindex()
        {
            StartReindex(this.Context.DesktopWindow);
        }

        internal static void StartReindex(IDesktopWindow desktopWindow)
        {
            if (!WorkItemActivityMonitor.IsRunning)
            {
                desktopWindow.ShowMessageBox(SR.MessageReindexServiceNotRunning, MessageBoxActions.Ok);
                return;
            }

            if (!PermissionsHelper.IsInRole(AuthorityTokens.Administration.ReIndex))
            {
                desktopWindow.ShowMessageBox(SR.WarningReindexPermission, MessageBoxActions.Ok);
                return;
            }

            string linkText = SR.LinkOpenActivityMonitor;

            try
            {
                string message;
                var client = new ReindexFilestoreBridge();
                client.Reindex();

                if (client.WorkItem.Status == WorkItemStatusEnum.InProgress)
                    message = SR.MessageReindexInProgress;
                else if (client.WorkItem.Status == WorkItemStatusEnum.Idle)
                    message = SR.MessageReindexInProgress;
                else if (client.WorkItem.Status == WorkItemStatusEnum.Pending)
                    message = SR.MessageReindexScheduled;
                else
                    message = SR.MessageFailedToStartReindex;

				desktopWindow.ShowAlert(AlertLevel.Info, message, linkText, ActivityMonitorManager.Show, true);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageFailedToStartReindex, desktopWindow);
            }
        }
    }
}
