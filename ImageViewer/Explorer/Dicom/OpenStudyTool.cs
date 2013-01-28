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
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarOpenStudy", "OpenStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuOpenStudy", "OpenStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]
	[Tooltip("activate", "TooltipOpenStudy")]
	[IconSet("activate", "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png", "Icons.OpenToolSmall.png")]

	[ViewerActionPermission("activate", ImageViewer.AuthorityTokens.Study.Open)]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : StudyBrowserTool
	{
		public override void Initialize()
		{
			base.Initialize();

			SetDoubleClickHandler();
		}

		public void OpenStudy()
		{
			try
			{
			    int numberOfSelectedStudies = Context.SelectedStudies.Count;
                if (Context.SelectedStudies.Count == 0)
					return;

				if (!PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.Study.Open))
				{
					Context.DesktopWindow.ShowMessageBox(SR.MessageOpenStudyPermissionDenied, MessageBoxActions.Ok);
					return;
				}

				int numberOfLoadableStudies = GetNumberOfLoadableStudies();
				if (numberOfLoadableStudies != numberOfSelectedStudies)
				{
					int numberOfNonLoadableStudies = numberOfSelectedStudies - numberOfLoadableStudies;
					string message;
					if (numberOfSelectedStudies == 1)
					{
						message = SR.MessageCannotOpenNonStreamingStudy;
					}
					else
					{
						if (numberOfNonLoadableStudies == 1)
							message = SR.MessageOneNonStreamingStudyCannotBeOpened;
						else 
							message = String.Format(SR.MessageFormatXNonStreamingStudiesCannotBeOpened, numberOfNonLoadableStudies);
					}

					Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
					return;
				}

				if (Context.SelectedServers.Count == 1 && Context.SelectedServers[0].IsLocal)
				{
					// #10746:  Workstation: the user must be warned when opening studies that are being processed
					// This implementation does not cover all the possible cases of when a study might be modified.
					// For example: if a study is being retrieved, WQI failed and deleted, the study is technically
					// not complete and user should be warned.  The risk of such cases are mitigated by the fact the
					// user is warned about the failed WQI.  This implementation is only meant to warn user if the
					// study is "being" or "about to" be modified before opening the study.
					try
					{
						Platform.Log(LogLevel.Debug, "Querying for a StudyUpdate work items that are in progress for the studies that are being opened.");

						var isStudyBeingProcessed = Context.SelectedStudies.Any(study =>
						{
							var request = new WorkItemQueryRequest { StudyInstanceUid = study.StudyInstanceUid };
							IEnumerable<WorkItemData> workItems = null;

							Platform.GetService<IWorkItemService>(s => workItems = s.Query(request).Items);
							return workItems.Any(IsNonTerminalStudyUpdateItem);
						});

						var message = this.Context.SelectedStudies.Count > 1 ? SR.MessageLoadStudiesBeingProcessed : SR.MessageLoadStudyBeingProcessed;
						if (isStudyBeingProcessed && DialogBoxAction.No == Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
							return;
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Debug, e);
					}
				}

				var helper = new OpenStudyHelper
				                 {
				                     WindowBehaviour = ViewerLaunchSettings.WindowBehaviour,
				                     AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer,
                                     //The user has elected to ignore "in use" studies.
                                     StudyLoaderOptions = new StudyLoaderOptions(true)
				                 };

				foreach (var study in Context.SelectedStudies)
					helper.AddStudy(study.StudyInstanceUid, study.Server);

				helper.Title = ImageViewerComponent.CreateTitle(GetSelectedPatients());
				helper.OpenStudies();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private void SetDoubleClickHandler()
		{
			if (GetAtLeastOneServerSupportsLoading() || base.Context.SelectedServers.Count == 0)
				Context.DefaultActionHandler = OpenStudy;
		}
		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
            Visible = GetAtLeastOneServerSupportsLoading(); 
		    Enabled = Context.SelectedStudies.Count > 0 && GetAtLeastOneServerSupportsLoading();
		    SetDoubleClickHandler();
		}

	    private bool GetAtLeastOneServerSupportsLoading()
		{
		    return Context.SelectedServers.AnySupport<IStudyLoader>();
		}

		private int GetNumberOfLoadableStudies()
		{
		    return base.Context.SelectedStudies.Count(s => s.Server.IsSupported<IStudyLoader>());
		}

		private IEnumerable<IPatientData> GetSelectedPatients()
		{
		    return Context.SelectedStudies.Cast<IPatientData>();
		}

		private static bool IsNonTerminalStudyUpdateItem(WorkItemData item)
		{
			if (item.Request.ConcurrencyType != WorkItemConcurrency.StudyUpdate)
				return false;

			switch (item.Status)
			{
				case WorkItemStatusEnum.Pending:
				case WorkItemStatusEnum.InProgress:
				case WorkItemStatusEnum.Idle:
					return true;
				default:
					return false;
			}
		}
	}
}
