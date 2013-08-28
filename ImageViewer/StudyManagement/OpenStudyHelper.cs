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
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	#region OpenStudyArgs

	/// <summary>
	/// Holds the parameters that specify the studies to be opened using the <see cref="OpenStudyHelper"/>
	/// </summary>
	[Obsolete("This class will be removed in a future version.  Please use an instance of OpenStudyHelper instead.")]
	public class OpenStudyArgs
	{
		private readonly string[] _studyInstanceUids;
		private readonly WindowBehaviour _windowBehaviour;
		private readonly IDicomServiceNode _server;

		/// <summary>
		/// Constructs a new <see cref="OpenStudyArgs"/> using the specified parameters.
		/// </summary>
		/// <param name="studyInstanceUids">The Study Instance UIDs of the studies to be opened.</param>
		/// <param name="server">The server from which the study should be loaded.</param>
		/// <param name="windowBehaviour">The window launch options.</param>
		public OpenStudyArgs(string[] studyInstanceUids, IDicomServiceNode server, WindowBehaviour windowBehaviour)
		{
			Platform.CheckForNullReference(studyInstanceUids, "studyInstanceUids");
            Platform.CheckForNullReference(server, "server");

			if (studyInstanceUids.Length == 0)
				throw new ArgumentException("studyInstanceUids array cannot be empty.");

			_studyInstanceUids = studyInstanceUids;
			_server = server;
			_windowBehaviour = windowBehaviour;
		}

		/// <summary>
		/// Gets the Study Instance UIDs of the studies to be opened.
		/// </summary>
		public string[] StudyInstanceUids
		{
			get { return _studyInstanceUids; }
		}

		public IDicomServiceNode Server
		{
			get { return _server; }
		}

		/// <summary>
		/// Gets the window launch options.
		/// </summary>
		public WindowBehaviour WindowBehaviour
		{
			get { return _windowBehaviour; }
		}
	}

	#endregion

	/// <summary>
	/// Helper class to create, populate and launch an <see cref="ImageViewerComponent"/>.
	/// </summary>
	public class OpenStudyHelper
	{
        /// <summary>
        /// Decisions made by the user when study is being processed
        /// </summary>
        enum StudyInUseUserDecision
        {
            /// <summary>
            /// User elects to continue opening the study
            /// </summary>
            Continue,

            /// <summary>
            /// User elects not to open the study
            /// </summary>
            Cancel
        }

		#region Private Fields

		private readonly List<LoadStudyArgs> _studiesToOpen = new List<LoadStudyArgs>();

		#endregion

		/// <summary>
		/// Constructs a new instance of <see cref="OpenStudyHelper"/>.
		/// </summary>
		public OpenStudyHelper()
		{
			LoadPriors = true;
			WindowBehaviour = WindowBehaviour.Auto;
		    StudyLoaderOptions = new StudyLoaderOptions();
		}

		#region Launch Options

		/// <summary>
		/// Gets or sets the <see cref="WindowBehaviour"/> for launching the <see cref="ImageViewerComponent"/>.
		/// </summary>
		public WindowBehaviour WindowBehaviour { get; set; }

		/// <summary>
		/// Gets or sets the workspace title for the <see cref="ImageViewerComponent"/>.
		/// </summary>
		/// <remarks>
		/// This value may be null, indicating that the component should automatically generate an appropriate title.
		/// </remarks>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets whether or not the <see cref="ImageViewerComponent"/> should load any prior studies.
		/// </summary>
		public bool LoadPriors { get; set; }

        /// <summary>
        /// Options for the study loader.
        /// </summary>
        protected StudyLoaderOptions StudyLoaderOptions { get; set; }
		
        /// <summary>
		/// Gets or sets whether or not to allow an empty viewer to be opened (e.g. with no studies loaded).
		/// </summary>
		public bool AllowEmptyViewer { get; set; }

		/// <summary>
		/// Gets or sets the owner <see cref="IDesktopWindow"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is the <see cref="IDesktopWindow"/> on which error messages will be shown and
		/// on which the <see cref="ImageViewerComponent"/> will launch if not launching in a new window.
		/// </para>
		/// <para>
		/// This value may be null, thus indicating that the currently active <see cref="IDesktopWindow"/> should be used.
		/// </para>
		/// </remarks>
		public IDesktopWindow DesktopWindow { get; set; }
		
		#endregion

		#region Instance Methods

		#region Public

		/// <summary>
		/// Adds a study to the list of studies to be opened.
		/// </summary>
		public void AddStudy(string studyInstanceUid, IDicomServiceNode server)
		{
            _studiesToOpen.Add(new LoadStudyArgs(studyInstanceUid, server, StudyLoaderOptions));
		}

		/// <summary>
		/// Creates the <see cref="ImageViewerComponent"/>, loads the specified studies,
		/// and launches the <see cref="ImageViewerComponent"/>.
		/// </summary>
		public ImageViewerComponent OpenStudies()
		{
			ImageViewerComponent viewer = null;

			BlockingOperation.Run(delegate { viewer = LoadAndOpenStudies(); });

			return viewer;
		}

		#endregion

		#region Private

		private ImageViewerComponent LoadAndOpenStudies()
		{
			var codeClock = new CodeClock();
			codeClock.Start();

			var viewer = CreateViewer(LoadPriors);
			var desktopWindow = DesktopWindow ?? Application.ActiveDesktopWindow;

			try
			{
				viewer.LoadStudies(_studiesToOpen);
			}
            catch(InUseLoadStudyException)
            {
                if (!HandleStudyInUseError(viewer))
                {
                    viewer.Dispose();
                    return null;
                }
            }
            catch(LoadMultipleStudiesException ex)
            {
                // Note: although there may be other errors, we only need to handle exceptions caused by study being processed.
                // Other errors will (probably) happen again when all studies are reloaded and will be handled in HandleStudyInUseError.
                if (ex.InUseCount > 0)
                {
                    if (!HandleStudyInUseError(viewer))
                    {
                        viewer.Dispose();
                        return null;
                    }
                }
                else
                {
                    ExceptionHandler.Report(ex, SR.MessageFailedToOpenStudy, desktopWindow);
                }
            }
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToOpenStudy, desktopWindow);
			}

			if (!AnySopsLoaded(viewer) && !AllowEmptyViewer)
			{
				viewer.Dispose();
				return null;
			}

			var args = new LaunchImageViewerArgs(WindowBehaviour) {Title = Title};
			ImageViewerComponent.Launch(viewer, args);

			codeClock.Stop(); // note: the time will be skewed if the workstation prompts users for action
			Platform.Log(LogLevel.Debug, string.Format("TTFI: {0}", codeClock));

			return viewer;
		}

        private void SetStudyLoadOption(bool ignoreInUse)
        {
            StudyLoaderOptions.IgnoreInUse = ignoreInUse;

            // Ensure _studiesToOpen is updated
            foreach (var studyLoadArg in _studiesToOpen)
            {
                studyLoadArg.StudyLoaderOptions.IgnoreInUse = ignoreInUse;
            }
        }

        /// <summary>
        /// Prompt user for decision when at least one of the studies opened was being processed on the local or remote server
        /// </summary>
        /// <returns></returns>
        private StudyInUseUserDecision PromptUser()
        {
            var desktopWindow = DesktopWindow ?? Application.ActiveDesktopWindow;

            var message = _studiesToOpen.Count > 1 ? SR.MessageLoadStudiesBeingProcessed : SR.MessageLoadStudyBeingProcessed;

            if (desktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.No)
            {
                return StudyInUseUserDecision.Cancel;
            }

            return StudyInUseUserDecision.Continue;
        }

        /// <summary>
        /// Handle the situation where at least one of the study being loaded is in use. User can decide to continue loading or cancel the operation.
        /// In former case, all studies will be reloaded. Caller should check the value returned by this method.
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns>False if the operation should be aborted. Caller is responsible for releasing any resource used by the <see cref="ImageViewerComponent"/>.</returns>
        private bool HandleStudyInUseError(ImageViewerComponent viewer)
        {
            var desktopWindow = DesktopWindow ?? Application.ActiveDesktopWindow;
            var action = PromptUser();
            switch (action)
            {
                case StudyInUseUserDecision.Continue:
                    try
                    {
                        SetStudyLoadOption(true);
                        viewer.LoadStudies(_studiesToOpen);
                        return true;
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, SR.MessageFailedToOpenStudy, desktopWindow);
                    }
                    return false;

                case StudyInUseUserDecision.Cancel:
                    return false;

                default:
                    throw new NotImplementedException("Implement this action based on user's decision");
            }

        }

		#endregion
		#endregion

		#region Static Helpers

		#region Private

		private static ImageViewerComponent CreateViewer(bool loadPriors)
		{
			if (loadPriors)
				return new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			else
				return new ImageViewerComponent(LayoutManagerCreationParameters.Extended, PriorStudyFinder.Null);
		}

		private static bool AnySopsLoaded(IImageViewer imageViewer)
		{
			foreach (Patient patient in imageViewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					foreach (Series series in study.Series)
					{
						foreach (Sop sop in series.Sops)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		#endregion
		#endregion
	}
}
