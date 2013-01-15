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
using System.Globalization;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	static class DynamicTeSeriesCreator
	{
		public static void Create(IDesktopWindow desktopWindow, IImageViewer viewer)
		{
			IDisplaySet selectedDisplaySet = viewer.SelectedImageBox.DisplaySet;
			string name = String.Format("{0} - Dynamic TE", selectedDisplaySet.Name);
			IDisplaySet t2DisplaySet = new DisplaySet(name, "");

			double currentSliceLocation = 0.0;

			BackgroundTask task = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					int i = 0;

					foreach (IPresentationImage image in selectedDisplaySet.PresentationImages)
					{
						IImageSopProvider imageSopProvider = image as IImageSopProvider;

						if (imageSopProvider == null)
							continue;

						ImageSop imageSop = imageSopProvider.ImageSop;
						Frame frame = imageSopProvider.Frame;

						if (frame.SliceLocation != currentSliceLocation)
						{
							currentSliceLocation = frame.SliceLocation;

							try
							{
								DynamicTePresentationImage t2Image = CreateT2Image(imageSop, frame);
								t2DisplaySet.PresentationImages.Add(t2Image);
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Error, e);
								desktopWindow.ShowMessageBox("Unable to create T2 series.  Please check the log for details.",
								                             MessageBoxActions.Ok);
								break;
							}

						}

						string message = String.Format("Processing {0} of {1} images", i, selectedDisplaySet.PresentationImages.Count);
						i++;

						BackgroundTaskProgress progress = new BackgroundTaskProgress(i, selectedDisplaySet.PresentationImages.Count, message);
						context.ReportProgress(progress);
					}
				}, false);

			ProgressDialog.Show(task, desktopWindow, true, ProgressBarStyle.Blocks);

			viewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(t2DisplaySet);
		}

		private static DynamicTePresentationImage CreateT2Image(ImageSop imageSop, Frame frame)
		{
			DicomFile pdMap = FindMap(imageSop.StudyInstanceUID, frame.SliceLocation, "PD");
			pdMap.Load(DicomReadOptions.Default);

			DicomFile t2Map = FindMap(imageSop.StudyInstanceUID, frame.SliceLocation, "T2");
			t2Map.Load(DicomReadOptions.Default);

			DicomFile probMap = FindMap(imageSop.StudyInstanceUID, frame.SliceLocation, "CHI2PROB");
			probMap.Load(DicomReadOptions.Default);

			DynamicTePresentationImage t2Image = new DynamicTePresentationImage(
				frame,
				(byte[])pdMap.DataSet[DicomTags.PixelData].Values,
				(byte[])t2Map.DataSet[DicomTags.PixelData].Values,
				(byte[])probMap.DataSet[DicomTags.PixelData].Values);

			t2Image.DynamicTe.Te = 50.0f;
			return t2Image;
		}

		private static DicomFile FindMap(string studyUID, double sliceLocation, string suffix)
		{
			string directory = String.Format(".\\T2_MAPS\\{0}", studyUID);
			string[] files;

			try
			{
				files = Directory.GetFiles(directory);
			}
			catch (DirectoryNotFoundException e)
			{
				Platform.Log(LogLevel.Error, e);
				throw;
			}

			CultureInfo ci = new CultureInfo("en-US");

			foreach (string file in files)
			{
				string str = String.Format("loc{0}_{1}", sliceLocation.ToString("F2", ci), suffix);

				if (file.Contains(str))
					return new DicomFile(file);
			}

			return null;
		}
	}
}
