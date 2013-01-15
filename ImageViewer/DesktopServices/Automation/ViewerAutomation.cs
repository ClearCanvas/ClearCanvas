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
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Automation;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.DesktopServices.Automation
{
	/// <summary>
	/// For internal use only.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = true, ConfigurationName = "ViewerAutomation", Namespace = AutomationNamespace.Value)]
	public class ViewerAutomation: IViewerAutomation
	{
		private static readonly string _viewerNotFoundReason = "The specified viewer was not found.";

		public ViewerAutomation()
		{
		}

		private static IStudyLocator GetStudyLocator()
		{
			return Platform.GetService<IStudyLocator>();
		}

	    /// TODO (CR Dec 2011): Build this functionality right into ImageViewerComponent?
        public static IImageViewer GetViewer(Viewer viewer)
        {
            return ViewerAutomationTool.GetViewer(viewer.Identifier);
        }

	    #region IViewerAutomation Members

        public GetViewersResult GetViewers(GetViewersRequest request)
        {
            List<Viewer> viewers = new List<Viewer>();

            //The tool stores the viewer ids in order of activation, most recent first
            foreach (Guid viewerId in ViewerAutomationTool.GetViewerIds())
            {
                IImageViewer viewer = ViewerAutomationTool.GetViewer(viewerId);
                if (viewer != null && GetViewerWorkspace(viewer) != null)
                    viewers.Add(new Viewer(viewerId, GetPrimaryStudyIdentifier(viewer)));
            }

            if (viewers.Count == 0)
                throw new FaultException<NoViewersFault>(new NoViewersFault(), "No active viewers were found.");

            return new GetViewersResult {Viewers = viewers};
        }

		[Obsolete("Use GetViewers instead.")]
		public GetActiveViewersResult GetActiveViewers()
		{
			try
			{
				return new GetActiveViewersResult { ActiveViewers = GetViewers(new GetViewersRequest()).Viewers };
			}
			catch (FaultException<NoViewersFault>)
			{
				// translate the exception correctly, since the type of exception is specified in the service contract
				throw new FaultException<NoActiveViewersFault>(new NoActiveViewersFault(), "No active viewers were found.");
			}
		}

		public GetViewerInfoResult GetViewerInfo(GetViewerInfoRequest request)
		{
			if (request == null)
			{
				string message = "The get viewer info request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
									"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			GetViewerInfoResult result = new GetViewerInfoResult();
			result.AdditionalStudyInstanceUids = GetAdditionalStudyInstanceUids(viewer);
			return result;
		}

        public OpenFilesResult OpenFiles(OpenFilesRequest request)
        {
            if (request == null)
            {
                const string message = "The open files request cannot be null.";
                Platform.Log(LogLevel.Debug, message);
                throw new FaultException(message);
            }
            
            if (request.Files == null || request.Files.Count == 0)
            {
                const string message = "At least one file or directory must be specified.";
                Platform.Log(LogLevel.Debug, message);
                throw new FaultException(message);
            }

            var helper = new OpenFilesHelper();
            try
            {
                foreach (var file in request.Files)
                    FileProcessor.Process(file, null, helper.AddFile, true);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                const string message = "There was a problem with the files/directories specified.";
                throw new FaultException<OpenFilesFault>(new OpenFilesFault { FailureDescription = message }, message);
            }

            if (request.WaitForFilesToOpen.HasValue && !request.WaitForFilesToOpen.Value)
            {
                SynchronizationContext.Current.Post(ignore => helper.OpenFiles(), null);
                return new OpenFilesResult();
            }

            try
            {
                helper.HandleErrors = false;
                var viewer = helper.OpenFiles();
                var viewerId = ViewerAutomationTool.GetViewerId(viewer);
                return new OpenFilesResult { Viewer = new Viewer(viewerId.Value, GetPrimaryStudyIdentifier(viewer)) };

            }
            catch (Exception e)
            {
                if (!request.ReportFaultToUser.HasValue || request.ReportFaultToUser.Value)
                {
                    SynchronizationContext.Current.Post(
                        ignore => ExceptionHandler.Report(e, ImageViewer.StudyManagement.SR.MessageFailedToOpenImages, Application.ActiveDesktopWindow), null);
                }

                const string message = "There was a problem opening the files/directories specified in the viewer.";
                throw new FaultException<OpenFilesFault>(new OpenFilesFault { FailureDescription = message }, message);
            }
        }

	    public OpenStudiesResult OpenStudies(OpenStudiesRequest request)
		{
			if (request == null)
			{
				string message = "The open studies request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.StudiesToOpen == null || request.StudiesToOpen.Count == 0)
			{
				string message = "At least one study must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			OpenStudiesResult result = new OpenStudiesResult();
			bool activateIfOpen = request.ActivateIfAlreadyOpen ?? true;

			try
			{
				string primaryStudyInstanceUid = request.StudiesToOpen[0].StudyInstanceUid;
				IImageViewer viewer = null;
				if (activateIfOpen)
				{
					Workspace workspace = GetViewerWorkspace(primaryStudyInstanceUid);
					if (workspace != null)
					{
						viewer = ImageViewerComponent.GetAsImageViewer(workspace);
						workspace.Activate();
					}
				}
				
				if (viewer == null)
					viewer = LaunchViewer(request, primaryStudyInstanceUid);

				Guid? viewerId = ViewerAutomationTool.GetViewerId(viewer);
				if (viewerId == null)
					throw new FaultException("Failed to retrieve the id of the specified viewer.");

				result.Viewer = new Viewer(viewerId.Value, GetPrimaryStudyIdentifier(viewer));
				return result;
			}
			catch(FaultException)
			{
				throw;
			}
			catch(Exception e)
			{
				string message = "An unexpected error has occurred while attempting to open the study(s).";
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		public void ActivateViewer(ActivateViewerRequest request)
		{
			if (request == null)
			{
				string message = "The activate viewer request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer ({0}) was found, " + 
					"but does not appear to be hosted in one of the active workspaces.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			try
			{
				workspace.Activate();
			}
			catch(Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " + 
					"to activate the specified viewer ({0}).", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		public void CloseViewer(CloseViewerRequest request)
		{
			if (request == null)
			{
				string message = "The close viewer request cannot be null.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			if (request.Viewer == null || request.Viewer.Identifier.Equals(Guid.Empty))
			{
				string message = "A valid viewer id must be specified.";
				Platform.Log(LogLevel.Debug, message);
				throw new FaultException(message);
			}

			IImageViewer viewer = ViewerAutomationTool.GetViewer(request.Viewer.Identifier);
			if (viewer == null)
			{
				string message = String.Format("The specified viewer ({0}) was not found, " +
					"likely because it has already been closed by the user.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Debug, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			IWorkspace workspace = GetViewerWorkspace(viewer);
			if (workspace == null)
			{
				string message = String.Format("The specified viewer ({0}) was found, " +
					"but it does not appear to be hosted in one of the active workspaces.", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, message);

				throw new FaultException<ViewerNotFoundFault>(new ViewerNotFoundFault(message), _viewerNotFoundReason);
			}

			try
			{
				workspace.Close(UserInteraction.NotAllowed);
			}
			catch (Exception e)
			{
				string message = String.Format("An unexpected error has occurred while attempting " +
					"to close the specified viewer ({0}).", request.Viewer.Identifier);
				Platform.Log(LogLevel.Error, e, message);
				throw new FaultException(message);
			}
		}

		#endregion

		private static void CompleteOpenStudyInfo(List<OpenStudyInfo> openStudyInfo)
		{
		    var incomplete = openStudyInfo.Where(info => String.IsNullOrEmpty(info.SourceAETitle)).ToList();
			
			//only go looking for studies if the source ae title is unspecified.
			if (incomplete.Count == 0)
				return;

			List<string> incompleteStudyUids = incomplete.Select(info => info.StudyInstanceUid).ToList();

			using (var bridge = new StudyLocatorBridge(GetStudyLocator()))
			{
				LocateFailureInfo[] queryFailures;
				IList<StudyRootStudyIdentifier> foundStudies = bridge.LocateStudyByInstanceUid(incompleteStudyUids, out queryFailures);
				foreach (StudyRootStudyIdentifier study in foundStudies)
				{
					foreach (OpenStudyInfo info in openStudyInfo)
					{
						if (info.StudyInstanceUid == study.StudyInstanceUid)
						{
							info.SourceAETitle = study.RetrieveAeTitle;
							break;
						}
					}
				}

				var unlocated = openStudyInfo.Where(info => string.IsNullOrEmpty(info.SourceAETitle)).Select(info => info.StudyInstanceUid).ToArray();
				if (unlocated.Any())
				{
					var fault = new OpenStudiesFault(string.Format("One or more specified studies could not be opened: {0}", string.Join(", ", unlocated)));
					throw new FaultException<OpenStudiesFault>(fault, fault.FailureDescription);
				}
			}
		}

		private static IImageViewer LaunchViewer(OpenStudiesRequest request, string primaryStudyInstanceUid)
		{
			try
			{
				CompleteOpenStudyInfo(request.StudiesToOpen);
			}
			catch (Exception ex)
			{
				if (request.ReportFaultToUser) SynchronizationContext.Current.Post(ReportLoadFailures, ex);
				throw;
			}
		    
            ImageViewerComponent viewer;
            if (!request.LoadPriors.HasValue || request.LoadPriors.Value)
                viewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
            else
                viewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended, PriorStudyFinder.Null);

			var loadStudyArgs = (from info in request.StudiesToOpen 
                                 let server = ServerDirectory.GetRemoteServersByAETitle(info.SourceAETitle).FirstOrDefault() ?? ServerDirectory.GetLocalServer()
                                 select new LoadStudyArgs(info.StudyInstanceUid, server)).ToList();

		    try
			{
				viewer.LoadStudies(loadStudyArgs);
			}
			catch (Exception e)
			{
				bool faultThrown = false;
				try
				{
					HandleLoadStudiesException(e, primaryStudyInstanceUid, viewer);
				}
				catch
				{
					faultThrown = true;
					viewer.Dispose();
					throw;
				}
				finally
				{
					if (!faultThrown || request.ReportFaultToUser)
						SynchronizationContext.Current.Post(ReportLoadFailures, e);
				}
			}

			ImageViewerComponent.Launch(viewer, new LaunchImageViewerArgs(ViewerLaunchSettings.WindowBehaviour));
			return viewer;
		}

		/// <summary>
		/// As long as the primary study is loaded, even partially, we continue opening the viewer and
		/// just report the loading errors to the user.  If other studies failed to load, we still just
		/// open the viewer and report to the user.
		/// </summary>
		private static void HandleLoadStudiesException(Exception e, string primaryStudyInstanceUid, IImageViewer viewer)
		{
			if (GetPrimaryStudyInstanceUid(viewer) == primaryStudyInstanceUid)
				return; //the primary study was at least partiallly loaded.

			if (e is NotFoundLoadStudyException)
				throw new FaultException<StudyNotFoundFault>(new StudyNotFoundFault(), "The study was not found.");
			if (e is NearlineLoadStudyException)
				throw new FaultException<StudyNearlineFault>(new StudyNearlineFault { IsStudyBeingRestored = ((NearlineLoadStudyException)e).IsStudyBeingRestored }, "The study is nearline.");
			if (e is OfflineLoadStudyException)
				throw new FaultException<StudyOfflineFault>(new StudyOfflineFault(), "The study is offline.");
			if (e is InUseLoadStudyException)
				throw new FaultException<StudyInUseFault>(new StudyInUseFault(), "The study is in use.");
			if (e is StudyLoaderNotFoundException)
			{
				const string reason = "The study cannot be loaded directly from the specified server/location.";
				throw new FaultException<OpenStudiesFault>(new OpenStudiesFault { FailureDescription = reason }, reason);
			}

			throw new FaultException<OpenStudiesFault>(new OpenStudiesFault(), "The primary study could not be loaded.");
		}

		private static void ReportLoadFailures(object loadFailures)
		{
			ExceptionHandler.Report((Exception)loadFailures, Application.ActiveDesktopWindow);
		}

		private static string GetPrimaryStudyInstanceUid(IImageViewer viewer)
		{
			foreach (Patient patient in viewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					return study.StudyInstanceUid;
				}
			}

			return null;
		}

		private static StudyRootStudyIdentifier GetPrimaryStudyIdentifier(IImageViewer viewer)
		{
			// TODO CR (Oct 12): Add support for patient reconciliation (after we move that concept into viewer common)
			return viewer.StudyTree.Patients.SelectMany(p => p.Studies).Select(s => new StudyRootStudyIdentifier(s.GetIdentifier())).FirstOrDefault();
		}

		private static List<string> GetAdditionalStudyInstanceUids(IImageViewer viewer)
		{
			List<string> studyInstanceUids = new List<string>();

			foreach (Patient patient in viewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					studyInstanceUids.Add(study.StudyInstanceUid);
				}
			}

			if (studyInstanceUids.Count > 0)
				studyInstanceUids.RemoveAt(0);

			return studyInstanceUids;
		}

		private static Workspace GetViewerWorkspace(IImageViewer viewer)
		{
			foreach (Workspace workspace in GetViewerWorkspaces())
			{
				IImageViewer workspaceViewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (viewer == workspaceViewer)
					return workspace;
			}

			return null;
		}

		private static Workspace GetViewerWorkspace(string primaryStudyUid)
		{
			foreach (Workspace workspace in GetViewerWorkspaces())
			{
				IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
				if (primaryStudyUid == GetPrimaryStudyInstanceUid(viewer))
					return workspace;
			}

			return null;
		}

		private static IEnumerable<Workspace> GetViewerWorkspaces()
		{
			foreach (DesktopWindow desktopWindow in Application.DesktopWindows)
			{
				foreach (Workspace workspace in desktopWindow.Workspaces)
				{
					IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
					if (viewer != null)
						yield return workspace;
				}
			}
		}
	}
}
