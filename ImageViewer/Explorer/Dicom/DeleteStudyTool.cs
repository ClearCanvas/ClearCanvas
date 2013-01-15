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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarDeleteStudy", "DeleteStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuDeleteStudy", "DeleteStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.DeleteStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [VisibleStateObserver("activate", "Visible", "VisibleChanged")]
    [Tooltip("activate", "TooltipDeleteStudy")]
	[IconSet("activate", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]

    [ViewerActionPermission("activate", ImageViewer.AuthorityTokens.Study.Delete)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
        public void DeleteStudy()
        {
            if (!Enabled)
                return;

            if (AtLeastOneStudyInUse())
                return;

            if (!ConfirmDeletion())
                return;

            try
            {
				var client = new DeleteBridge();
				if (Context.SelectedStudies.Count > 1)
				{
					var count = ProcessItemsAsync(Context.SelectedStudies, client.DeleteStudy, false);
					AlertMultipleStudiesDeleted(count);
				}
				else if(Context.SelectedStudies.Count == 1)
				{
					var study = Context.SelectedStudies.First();
					client.DeleteStudy(study);
					AlertStudyDeleted(study);
				}
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageErrorDeletingStudies, Context.DesktopWindow);
            }
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
            Visible = Context.SelectedServers.AllSupport<IWorkItemService>();

            Enabled = Context.SelectedStudies.Count > 0
                        && Context.SelectedServers.AllSupport<IWorkItemService>()
                        && WorkItemActivityMonitor.IsRunning;
		}

		private bool ConfirmDeletion()
		{
		    string message = Context.SelectedStudies.Count == 1 
		                         ? SR.MessageConfirmDeleteStudy 
		                         : String.Format(SR.MessageConfirmDeleteStudies, Context.SelectedStudies.Count);

			DialogBoxAction action = Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo);

			if (action == DialogBoxAction.Yes)
				return true;
		    return false;
		}

		// This is a total hack to prevent a user from deleting a study
		// that is currently in use.  The proper way of doing this is
		// to lock the study when it's in use.  But for now, this will do.
		private bool AtLeastOneStudyInUse()
		{
			var studiesInUse = GetStudiesInUse();

			var setStudyUidsInUse = new Dictionary<string, string>();
			foreach (var item in studiesInUse)
				setStudyUidsInUse[item.StudyInstanceUid] = item.StudyInstanceUid;

			// No studies in use.  Just return.
			if (setStudyUidsInUse.Count == 0)
				return false;

			string message;

			// Notify the user
			if (Context.SelectedStudies.Count == 1)
			{
				message = SR.MessageSelectedStudyInUse;
			}
			else
			{
				message = setStudyUidsInUse.Count == 1 
                    ? SR.MessageOneOfSelectedStudiesInUse 
                    : String.Format(SR.MessageSomeOfSelectedStudiesInUse, setStudyUidsInUse.Count);
			}

			Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);

			return true;
		}

        private IEnumerable<StudyTableItem> GetStudiesInUse()
		{
			var studiesInUse = new List<StudyTableItem>();
			IEnumerable<IImageViewer> imageViewers = GetImageViewers();

			foreach (var selectedStudy in Context.SelectedStudies)
			{
			    var study = selectedStudy;
			    var matchingStudies = from imageViewer in imageViewers
			                          where imageViewer.StudyTree.GetStudy(study.StudyInstanceUid) != null
			                          select study;

			    studiesInUse.AddRange(matchingStudies);
			}

			return studiesInUse;
		}

		private List<IImageViewer> GetImageViewers()
		{
		    return Context.DesktopWindow.Workspaces
                .Select(ImageViewerComponent.GetAsImageViewer).Where(viewer => viewer != null).ToList();
		}

		private void AlertStudyDeleted(StudyTableItem study)
		{
			var studyDate = DateParser.Parse(study.StudyDate);
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
											string.Format(SR.MessageFormatDeleteStudyScheduled,
											              study.PatientsName.FormattedName,
											              studyDate.HasValue ? Format.Date(studyDate.Value) : string.Empty,
											              study.AccessionNumber),
											SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}

		private void AlertMultipleStudiesDeleted(int count)
		{
			Context.DesktopWindow.ShowAlert(AlertLevel.Info,
											string.Format(SR.MessageFormatDeleteStudiesScheduled, count),
											SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show, true);
		}

	}
}
