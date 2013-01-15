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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Tools
{
	[ButtonAction("compose", "clipboard-toolbar/Compose", "Compose")]
	[IconSet("compose", IconScheme.Colour, "Icons.StudyComposerToolSmall.png", "Icons.StudyComposerToolSmall.png", "Icons.StudyComposerToolSmall.png")]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class StudyComposerTool : Tool<IClipboardToolContext>
	{
		private StudyComposerComponent _composer;

		public void Compose()
		{
			if (_composer != null)
				return;

			try
			{
				BackgroundTask task = new BackgroundTask(new BackgroundTaskMethod(InitComposer), true, null);
				ProgressDialogComponent progressDialog = new ProgressDialogComponent(task, true, ProgressBarStyle.Continuous);
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, progressDialog, SR.MessageStartingStudyComposer);

				if (_composer != null)
				{
					SimpleComposerAdapterComponent component = new SimpleComposerAdapterComponent(_composer);
					ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.StudyComposer);
					_composer = null;
				}
			}
			catch(Exception ex)
			{
				ExceptionHandler.Report(ex, base.Context.DesktopWindow);
			}
		}

		private void InitComposer(IBackgroundTaskContext context)
		{
			const float WGT_SCAN = 10f;
			const float WGT_INIT = 90f;
			
			List<IPresentationImage> queue = new List<IPresentationImage>();
			int count = 0;
			int total = this.Context.ClipboardItems.Count;

			context.ReportProgress(new BackgroundTaskProgress(0, SR.MessageScanningClipboard));
			foreach (IClipboardItem item in this.Context.ClipboardItems)
			{
				if (item.Item is IPresentationImage)
					Enqueue(queue, (IPresentationImage) item.Item);
				else if (item.Item is IDisplaySet)
					Enqueue(queue, (IDisplaySet)item.Item);
				else if (item.Item is IImageSet)
					Enqueue(queue, (IImageSet)item.Item);

				count++;
				if (context.CancelRequested)
					break;

				context.ReportProgress(new BackgroundTaskProgress((int)(WGT_SCAN * count / total), SR.MessageScanningClipboard));
			}

			if (!context.CancelRequested) {
				StudyComposerComponent composer = new StudyComposerComponent();
				total = queue.Count;
				for (int n = 0; n < queue.Count; n++)
				{
					if (context.CancelRequested)
						break;

					IPresentationImage image = queue[n];
					context.ReportProgress(new BackgroundTaskProgress((int)(WGT_SCAN + WGT_INIT * n / total), SR.MessageReadingImages));
					composer.InsertImage(image);
				}

				if (!context.CancelRequested)
				{
					context.ReportProgress(new BackgroundTaskProgress(100, SR.Done));
					_composer = composer;
					context.Complete(null);
				}
			}

			if (context.CancelRequested)
				context.Cancel();
		}

		private static int Enqueue(ICollection<IPresentationImage> queue, IPresentationImage pImage)
		{
			queue.Add(pImage);
			return 1;
		}

		private static int Enqueue(ICollection<IPresentationImage> queue, IDisplaySet dSet)
		{
			int count = 0;
			foreach (IPresentationImage pImage in dSet.PresentationImages)
			{
				count += Enqueue(queue, pImage);
			}
			return count;
		}

		private static int Enqueue(ICollection<IPresentationImage> queue, IImageSet iSet)
		{
			int count = 0;
			foreach (IDisplaySet dSet in iSet.DisplaySets)
			{
				count += Enqueue(queue, dSet);
			}
			return count;
		}
	}
}