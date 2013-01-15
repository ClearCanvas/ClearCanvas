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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("go", "imageviewer-contextmenu/Change Aspect Ratio", "Go")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ChangePixelAspectRatioTool : ImageViewerTool
	{
		private class NullContext : IBackgroundTaskContext
		{
			#region IBackgroundTaskContext Members

			public object UserState
			{
				get { return null; }
			}

			public bool CancelRequested
			{
				get { return false; }
			}

			public void ReportProgress(BackgroundTaskProgress progress)
			{
			}

			public void Complete(params object[] results)
			{
			}

			public void Cancel()
			{
			}

			public void Error(Exception e)
			{
				throw e;
			}

			#endregion
		}

		private volatile ChangePixelAspectRatioComponent _component;
		private volatile string _outputDirectory;
		private volatile List<string> _dicomFileNames;

		public ChangePixelAspectRatioTool()
		{
		}

		private IEnumerable<IPresentationImage> GetImages(bool singleImage)
		{
			if (singleImage)
			{
				yield return Context.Viewer.SelectedPresentationImage;
			}
			else
			{
				foreach (var displaySet in Context.Viewer.SelectedImageBox.DisplaySet.PresentationImages)
					yield return displaySet;
			}
		}
		
		public void Go()
		{
			_component = new ChangePixelAspectRatioComponent();
			if (ApplicationComponentExitCode.Accepted !=
				ApplicationComponent.LaunchAsDialog(Context.DesktopWindow, _component, "Change Aspect Ratio"))
				return;

			FileDialogResult fileDialog = Context.DesktopWindow.ShowSelectFolderDialogBox(new SelectFolderDialogCreationArgs());
			if (fileDialog.Action != DialogBoxAction.Ok)
				return;

			_outputDirectory = fileDialog.FileName;

			_dicomFileNames = CollectionUtils.Map(GetImages(!_component.ConvertWholeDisplaySet),
						(IPresentationImage image) => ((LocalSopDataSource)((IImageSopProvider)image).ImageSop.DataSource).Filename);

			try
			{
				if (_dicomFileNames.Count > 5)
				{
					var task = new BackgroundTask(Go, true);
					ProgressDialog.Show(task, Context.DesktopWindow, true, ProgressBarStyle.Continuous);
				}
				else
				{
					BlockingOperation.Run(() => Go(new NullContext()));
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private void Go(IBackgroundTaskContext context)
		{
			string studyUid = DicomUid.GenerateUid().UID;
			string seriesUid = DicomUid.GenerateUid().UID;

			PixelAspectRatioChanger changer = 
				new PixelAspectRatioChanger
              	{
              		IncreasePixelDimensions = _component.IncreasePixelDimensions,
              		NewAspectRatio = new PixelAspectRatio(_component.AspectRatioRow, _component.AspectRatioColumn),
              		RemoveCalibration = _component.RemoveCalibration
              	};

			int i = 0;
			context.ReportProgress(new BackgroundTaskProgress(i, _dicomFileNames.Count, "Exporting ..."));

			try
			{
				foreach (string originalFile in _dicomFileNames)
				{
					var file = new DicomFile(originalFile);
					file.Load(DicomReadOptions.None);

					string sopInstanceUid = DicomUid.GenerateUid().UID;

					file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyUid);
					file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesUid);
					file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);

					changer.ChangeAspectRatio(file);

					string outputFileName = Path.Combine(_outputDirectory, String.Format("{0}.dcm", sopInstanceUid));
					file.Save(outputFileName);

					if (context.CancelRequested)
					{
						context.Cancel();
						return;
					}

					context.ReportProgress(new BackgroundTaskProgress(++i, _dicomFileNames.Count + 1, "Exporting ..."));
				}
			}
			catch (Exception e)
			{
				context.Error(e);
				return;
			}

			context.Complete();
		}


	}
}