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
using ClearCanvas.ImageViewer.Common.Auditing;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public partial class ImageExportComponent
	{
		private class MultipleImageExporter
		{
			private readonly ImageExportComponent _exportComponent;

			private IShelf _progressComponentShelf;
			private IBackgroundTaskContext _taskContext;

			private volatile MultipleImageExportFileNamingStrategy _fileNamingStrategy;

			private volatile int _progress = 0;
			private volatile Exception _error;
			private volatile List<IPresentationImage> _imagesToDispose;

			public MultipleImageExporter(ImageExportComponent exportComponent)
			{
				_exportComponent = exportComponent;

				_fileNamingStrategy = new MultipleImageExportFileNamingStrategy(_exportComponent.ExportFilePath);
				_imagesToDispose = new List<IPresentationImage>();
			}

			#region Private Properties (wrap parent component properties)

			private IDesktopWindow DesktopWindow
			{
				get { return _exportComponent.Host.DesktopWindow; }	
			}

			private string FileExtension
			{
				get { return _exportComponent.SelectedImageExporter.FileExtensions[0]; }
			}

			private int NumberOfImagesToExport
			{
				get { return _exportComponent.NumberOfImagesToExport; }	
			}

			private IImageExporter SelectedImageExporter
			{
				get { return _exportComponent.SelectedImageExporter; }	
			}

			private List<IClipboardItem> ItemsToExport
			{
				get { return _exportComponent.ItemsToExport; }
			}

			#endregion 

			private bool IsAsynchronous
			{
				get { return _taskContext != null; }	
			}

			public void Run()
			{
				if (NumberOfImagesToExport <= 5)
				{
					BlockingOperation.Run(delegate { Export(null); });
					Complete();
				}
				else
				{
					ItemsToExport.ForEach(delegate(IClipboardItem item) { item.Lock(); });

					BackgroundTask task = new BackgroundTask(Export, true) {ThreadUICulture = Application.CurrentUICulture};

					ProgressDialogComponent progressComponent = new ProgressDialogComponent(task, true, ProgressBarStyle.Blocks);

					_progressComponentShelf = LaunchAsShelf(DesktopWindow, progressComponent,
																SR.TitleExportingImages, "ExportingImages",
																ShelfDisplayHint.DockFloat);

					_progressComponentShelf.Closed += delegate
														{
															Complete();
															task.Dispose();
														};
				}
			}

			private void Export(IBackgroundTaskContext context)
			{
				EventResult result = EventResult.Success;
				AuditedInstances exportedInstances = GetInstancesForAudit(ItemsToExport, this._exportComponent.ExportFilePath);

				try
				{
					_taskContext = context;
					Export();
				}
				catch (Exception e)
				{
					_error = e;
					result = EventResult.SeriousFailure;
					Platform.Log(LogLevel.Error, e);
				}
				finally
				{
					AuditHelper.LogExportStudies(exportedInstances, EventSource.CurrentUser, result);
					_imagesToDispose.ForEach(delegate(IPresentationImage image) { image.Dispose(); });
				}
			}

			private void Export()
			{
				ReportProgress(SR.MessageExportingImages, _progress);

				foreach (ClipboardItem clipboardItem in ItemsToExport)
				{
					if (IsAsynchronous && _taskContext.CancelRequested)
						break;

					ExportImageParams exportParams = GetExportParams(clipboardItem);

					if (clipboardItem.Item is IPresentationImage)
						ExportSingleImage((IPresentationImage)clipboardItem.Item, exportParams);
					else if (clipboardItem.Item is IDisplaySet)
						ExportDisplaySet((IDisplaySet)clipboardItem.Item, exportParams);
				}

				if (IsAsynchronous)
				{
					if (_taskContext.CancelRequested)
					{
						ReportProgress(SR.MessageCancelled, _progress);
						_taskContext.Cancel();
					}
					else
					{
						ReportProgress(SR.MessageExportComplete, _progress);
						_taskContext.Complete();
					}
				}
			}

			private void ExportSingleImage(IPresentationImage image, ExportImageParams exportParams)
			{
				image = GetImageForExport(image);

				string fileName = _fileNamingStrategy.GetSingleImageFileName(image, FileExtension);
				SelectedImageExporter.Export(image, fileName, exportParams);

				ReportProgress(fileName, ++_progress);
			}

			private void ExportDisplaySet(IDisplaySet displaySet, ExportImageParams exportParams)
			{
				foreach (ImageFileNamePair pair in _fileNamingStrategy.GetImagesAndFileNames(displaySet, FileExtension))
				{
					if (IsAsynchronous && _taskContext.CancelRequested)
						break;

					IPresentationImage image = GetImageForExport(pair.Image);
					SelectedImageExporter.Export(image, pair.FileName, exportParams);

					ReportProgress(pair.FileName, ++_progress);
				}
			}

			private IPresentationImage GetImageForExport(IPresentationImage image)
			{
				if (IsAsynchronous)
				{
					// A graphic should belong to (and be disposed on) a single thread 
					// and should not be rendered on multiple threads, so we clone it.
					// Technically, we should be doing the clone on the main thread
					// and then passing it to the worker, but that would require blocking
					// the main thread while we cloned all the images.
					_imagesToDispose.Add(image = ImageExporter.ClonePresentationImage(image));
				}

				return image;
			}

			private ExportImageParams GetExportParams(ClipboardItem clipboardItem)
			{
				return _exportComponent.GetExportParams(clipboardItem);
			}

			private void ReportProgress(string message, int currentStep)
			{
				if (_taskContext == null)
					return;

				int percent = Math.Min((int)(currentStep / (float)NumberOfImagesToExport * 100), 100);
				_taskContext.ReportProgress(new BackgroundTaskProgress(percent, message));
			}

			private void Complete()
			{
				if (IsAsynchronous)
					ItemsToExport.ForEach(delegate(IClipboardItem item) { item.Unlock(); });

				_progressComponentShelf = null;

				//notify the owner that we're done.
				_exportComponent.OnMultipleImageExportComplete(_error);
				_error = null;

				_taskContext = null;
			}
		}
	}
}
