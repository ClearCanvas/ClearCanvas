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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using ClearCanvas.ImageViewer.Volumes;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	[ButtonAction("open", "global-toolbars/ToolbarMpr/ToolbarOpenSelectionWithMpr", "LaunchMpr")]
	[MenuAction("open", "imageviewer-contextmenu/MenuOpenWithMpr", "LaunchMpr")]
	[MenuAction("open", "global-menus/MenuTools/MenuMpr/MenuOpenSelectionWithMpr", "LaunchMpr")]
	[IconSet("open", "Icons.LaunchMprToolSmall.png", "Icons.LaunchMprToolMedium.png", "Icons.LaunchMprToolLarge.png")]
	[EnabledStateObserver("open", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("open", "Visible", null)]
	[ViewerActionPermission("open", AuthorityTokens.ViewerClinical)]
	[GroupHint("open", "Tools.Volume.MPR")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class LaunchMprTool : ImageViewerTool
	{
		private MprViewerComponent _viewer;

		public bool Visible { get; private set; }

		public override void Initialize()
		{
			base.Initialize();

			Visible = !(ImageViewer is MprViewerComponent);

			Context.Viewer.EventBroker.ImageBoxSelected += OnImageBoxSelected;
			Context.Viewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;
		}

		protected override void Dispose(bool disposing)
		{
			Context.Viewer.EventBroker.ImageBoxSelected -= OnImageBoxSelected;
			Context.Viewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			base.Dispose(disposing);
		}

		public void LaunchMpr()
		{
			Exception exception = null;

			IPresentationImage currentImage = Context.Viewer.SelectedPresentationImage;
			if (currentImage == null)
				return;

			// gather the source frames which MPR will operate on. exceptions are reported.
			BackgroundTaskParams @params;
			try
			{
				@params = new BackgroundTaskParams(FilterSourceFrames(currentImage.ParentDisplaySet, currentImage));
			}
			catch (Exception ex)
			{
				ReportException(ex);
				return;
			}

			// execute the task to create an MPR component. exceptions (either thrown or passed via task) are reported, but any created component must be disposed
			BackgroundTask task = new BackgroundTask(LoadVolume, true, @params);
			task.Terminated += (sender, e) => exception = e.Exception;
			try
			{
				ProgressDialog.Show(task, Context.DesktopWindow);
			}
			catch (Exception ex)
			{
				if (_viewer != null)
				{
					_viewer.Dispose();
					_viewer = null;
				}

				ReportException(ex);
				return;
			}
			finally
			{
				task.Dispose();
			}

			if (exception != null)
			{
				ReportException(exception);
				return;
			}

			// launch the created MPR component as a workspace. any exceptions here are just reported.
			try
			{
				LaunchImageViewerArgs args = new LaunchImageViewerArgs(ViewerLaunchSettings.WindowBehaviour);
				args.Title = _viewer.Title;
				ImageViewerComponent.Launch(_viewer, args);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}
			finally
			{
				_viewer = null;
			}
		}

		private void ReportException(Exception ex)
		{
			ExceptionHandler.Report(ex, SR.ExceptionMprLoadFailure, Context.DesktopWindow);
		}

		private static IEnumerable<Frame> FilterSourceFrames(IDisplaySet displaySet, IPresentationImage currentImage)
		{
			// this method tries to filter the source display set based on the currently selected image before passing it to MPR
			// we need this because sometimes MPR-able content is found in a series concatenated with other frames (e.g. 3-plane loc)
			if (currentImage is IImageSopProvider)
			{
				Frame currentFrame = ((IImageSopProvider) currentImage).Frame;
				string studyInstanceUid = currentFrame.StudyInstanceUid;
				string seriesInstanceUid = currentFrame.SeriesInstanceUid;
				string frameOfReferenceUid = currentFrame.FrameOfReferenceUid;
				ImageOrientationPatient imageOrientationPatient = currentFrame.ImageOrientationPatient;

				// if the current frame is missing any of the matching parameters, then it is always an error
				if (string.IsNullOrEmpty(studyInstanceUid) || string.IsNullOrEmpty(seriesInstanceUid))
					throw new NullSourceSeriesException();
				if (string.IsNullOrEmpty(frameOfReferenceUid))
					throw new NullFrameOfReferenceException();
				if (imageOrientationPatient == null || imageOrientationPatient.IsNull)
					throw new NullImageOrientationException();

				// if the current frame is not a supported pixel format, then it is always an error
				if (currentFrame.BitsAllocated != 16)
					throw new UnsupportedPixelFormatSourceImagesException();

				// perform a very basic filtering of the selected display set based on the currently selected image
				var filteredFrames = new List<Frame>();
				foreach (IPresentationImage image in displaySet.PresentationImages)
				{
					if (image == currentImage)
					{
						filteredFrames.Add(currentFrame);
					}
					else if (image is IImageSopProvider)
					{
						Frame frame = ((IImageSopProvider) image).Frame;
						if (frame.StudyInstanceUid == studyInstanceUid
						    && frame.SeriesInstanceUid == seriesInstanceUid
						    && frame.FrameOfReferenceUid == frameOfReferenceUid
						    && !frame.ImageOrientationPatient.IsNull
						    && frame.ImageOrientationPatient.EqualsWithinTolerance(imageOrientationPatient, .01f))
							filteredFrames.Add(frame);
					}
				}

				// if we found at least 3 frames matching the current image, then return those to MPR
				if (filteredFrames.Count > 3)
					return filteredFrames;

				// JY: #6164 - Error message not accurate for MPR with no location information
				// if we don't find 3 matching frames, then MPR fails on the minimum frames error
				// which masks the fact that there *were* enough frames, just not enough frames matching some aspect filter criteria
				// we don't know what was the specific failed criterion, so we'll just pass all frames unfiltered
				// this lets MPR decide what's wrong with the display set here and throw the correct exception
				return CollectionUtils.Map<IPresentationImage, Frame>(displaySet.PresentationImages, img => img is IImageSopProvider ? ((IImageSopProvider) img).Frame : null);
			}
			else
			{
				throw new UnsupportedSourceImagesException();
			}
		}

		private void LoadVolume(IBackgroundTaskContext context)
		{
			try
			{
				ProgressTask mainTask = new ProgressTask();
				mainTask.AddSubTask("BUILD", 90);
				mainTask.AddSubTask("LAYOUT", 10);

				context.ReportProgress(new BackgroundTaskProgress(mainTask.IntPercent, string.Format(SR.MessageInitializingMpr, mainTask.Progress)));

				BackgroundTaskParams @params = (BackgroundTaskParams) context.UserState;
				Volumes.Volume volume = Volumes.Volume.Create(@params.Frames,
				                                              delegate(int i, int count)
				                                              	{
				                                              		if (context.CancelRequested)
				                                              			throw new BackgroundTaskCancelledException();
				                                              		if (i == 0)
				                                              			mainTask["BUILD"].AddSubTask("", count);
				                                              		mainTask["BUILD"][""].Increment();
				                                              		string message = string.Format(SR.MessageBuildingMprVolumeProgress, mainTask.Progress, i + 1, count, mainTask["BUILD"].Progress);
				                                              		context.ReportProgress(new BackgroundTaskProgress(mainTask.IntPercent, message));
				                                              	});

				mainTask["BUILD"].MarkComplete();
				context.ReportProgress(new BackgroundTaskProgress(mainTask.IntPercent, string.Format(SR.MessagePerformingMprWorkspaceLayout, mainTask.Progress)));

				//call layout here b/c it could take a while
				@params.SynchronizationContext.Send(delegate
				                                    	{
				                                    		_viewer = new MprViewerComponent(volume);
				                                    		_viewer.Layout();
				                                    	}, null);

				mainTask["LAYOUT"].MarkComplete();
				context.ReportProgress(new BackgroundTaskProgress(mainTask.IntPercent, string.Format(SR.MessageDone, mainTask.Progress)));

				context.Complete();
			}
			catch (BackgroundTaskCancelledException)
			{
				context.Cancel();
			}
			catch (Exception ex)
			{
				context.Error(ex);
			}
		}

		private sealed class BackgroundTaskCancelledException : Exception {}

		private class BackgroundTaskParams
		{
			public readonly IEnumerable<Frame> Frames;
			public readonly SynchronizationContext SynchronizationContext;

			public BackgroundTaskParams(IEnumerable<Frame> frames)
			{
				Frames = frames;
				SynchronizationContext = SynchronizationContext.Current;
			}
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			if (e.SelectedPresentationImage != null)
				UpdateEnabled(e.SelectedPresentationImage.ParentDisplaySet);
			else
				UpdateEnabled(null);
		}

		private void OnImageBoxSelected(object sender, ImageBoxSelectedEventArgs e)
		{
			if (e.SelectedImageBox.DisplaySet == null)
				UpdateEnabled(null);
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			UpdateEnabled(e.SelectedDisplaySet);
		}

		private void UpdateEnabled(IDisplaySet selectedDisplaySet)
		{
			Enabled = selectedDisplaySet != null && selectedDisplaySet.PresentationImages.Count > 1;
		}
	}
}