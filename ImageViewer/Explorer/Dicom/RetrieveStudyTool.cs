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
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy", "RetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy", "RetrieveStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.RetrieveStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [VisibleStateObserver("activate", "Visible", "VisibleChanged")]
    [Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]
    [ViewerActionPermission("activate", ImageViewer.AuthorityTokens.Study.Retrieve)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public override void Initialize()
		{
			base.Initialize();

			SetDoubleClickHandler();
		}

		private void RetrieveStudy()
		{
		    //TODO (Marmot):Restore.
         
            if (!Enabled || Context.SelectedServers.IsLocalServer || Context.SelectedStudy == null)
                return;

            try
            {
                var client = new DicomRetrieveBridge();
				if (Context.SelectedStudies.Count > 1)
				{
					var count = ProcessItemsAsync(Context.SelectedStudies, study => client.RetrieveStudy(study.Server, study), false);
					AlertMultipleStudiesRetrieved(count);
				}
				else if (Context.SelectedStudies.Count == 1)
				{
					var study = Context.SelectedStudies.First();
					client.RetrieveStudy(study.Server, study);
					AlertStudyRetrieved(study);
				}
            }
            catch (EndpointNotFoundException)
            {
                 Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageErrorRetrievingStudies, Context.DesktopWindow);
            }        
		}

		private void AlertMultipleStudiesRetrieved(int count)
		{
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
			                                string.Format(SR.MessageFormatRetrieveStudiesScheduled, count),
			                                SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}

		private void AlertStudyRetrieved(StudyTableItem study)
		{
			DateTime? studyDate = DateParser.Parse(study.StudyDate);
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
			                                string.Format(SR.MessageFormatRetrieveStudyScheduled,
			                                              study.PatientsName.FormattedName,
			                                              studyDate.HasValue
			                                              	? Format.Date(studyDate.Value)
			                                              	: string.Empty,
			                                              study.AccessionNumber,
			                                              study.Server.Name),
			                                SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}

		private bool GetAtLeastOneServerSupportsLoading()
		{
            return Context.SelectedServers.AnySupport<IStudyLoader>();
        }

		private void SetDoubleClickHandler()
		{
			if (!GetAtLeastOneServerSupportsLoading() && Context.SelectedServers.Count > 0)
				Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
		
		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
			SetDoubleClickHandler();
		}

		private void UpdateEnabled()
		{
            Visible = !Context.SelectedServers.IsLocalServer;

			Enabled = Context.SelectedStudies.Count > 0
                        && !Context.SelectedServers.IsLocalServer
                        && WorkItemActivityMonitor.IsRunning;
    	}
	}
}
