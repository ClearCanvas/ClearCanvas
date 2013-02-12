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
					try
					{
						Platform.Log(LogLevel.Debug, "Querying for a StudyUpdate work items that are in progress for the studies that are being opened.");

						var isStudyBeingProcessed = Context.SelectedStudies.Any(study =>
						{
							var request = new WorkItemQueryRequest { StudyInstanceUid = study.StudyInstanceUid };
							IEnumerable<WorkItemData> workItems = null;

							Platform.GetService<IWorkItemService>(s => workItems = s.Query(request).Items);
							return workItems.Any(IsWorkItemInUse);
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

		private static bool IsWorkItemInUse(WorkItemData item)
		{
			// #10746: Workstation: the user must be warned when opening studies that are being processed
			// #10860: Workstation should warn user when opening a study with failed work items
			// We include all 'Active' WQI statuses. We also want to capture any failed items because it is 
			// likely to result in partial studies.  This should be consistent with the LocalStoreStudyLoader.
			if (item.Request.ConcurrencyType != WorkItemConcurrency.StudyUpdate &&
				item.Request.ConcurrencyType != WorkItemConcurrency.StudyDelete &&
				item.Request.ConcurrencyType != WorkItemConcurrency.StudyUpdateTrigger)
				return false;

			switch (item.Status)
			{
				case WorkItemStatusEnum.Pending:
				case WorkItemStatusEnum.InProgress:
				case WorkItemStatusEnum.Idle:
					return true;

				case WorkItemStatusEnum.Failed:
					return true;

				default:
					return false;
			}
		}
	}
}
