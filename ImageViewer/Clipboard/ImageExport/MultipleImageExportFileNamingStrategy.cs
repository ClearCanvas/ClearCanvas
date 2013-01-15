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
using System.IO;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal class ImageFileNamePair
	{
		public readonly IPresentationImage Image;
		public readonly string FileName;

		public ImageFileNamePair(IPresentationImage image, string fileName)
		{
			this.Image = image;
			this.FileName = fileName;
		}
	}

	internal class MultipleImageExportFileNamingStrategy
	{
		private readonly string _baseDirectory;
		private int _singleImageStartNumber;

		public MultipleImageExportFileNamingStrategy(string baseDirectory)
		{
			_baseDirectory = baseDirectory;
			_singleImageStartNumber = 1;
		}

		public string GetSingleImageFileName(IPresentationImage image, string fileExtension)
		{
			return GetImageFileName(_baseDirectory, "Image", fileExtension, 0, ref _singleImageStartNumber);
		}

		public IEnumerable<ImageFileNamePair> GetImagesAndFileNames(IDisplaySet displaySet, string fileExtension)
		{
			string displaySetDirectory = GetDisplaySetDirectory(displaySet, _baseDirectory);
			Directory.CreateDirectory(displaySetDirectory);

			int numberOfZeroes = displaySet.PresentationImages.Count.ToString().Length;

			int startNumber = 1;
			foreach(IPresentationImage image in displaySet.PresentationImages)
			{
				string filePath = GetImageFileName(displaySetDirectory, "Image", fileExtension, numberOfZeroes, ref startNumber);
				yield return new ImageFileNamePair(image, filePath);
			}
		}

		private static string GetDisplaySetDirectoryPrefix(IDisplaySet displaySet)
		{
			string prefix = "DisplaySet";

			if (displaySet.PresentationImages.Count > 0 &&
					displaySet.PresentationImages[0] is IImageSopProvider)
			{
				IImageSopProvider provider = (IImageSopProvider)displaySet.PresentationImages[0];
				string seriesDescription = (provider.ImageSop.SeriesDescription ?? "").Trim();

				if (!String.IsNullOrEmpty(seriesDescription))
				{
					prefix = seriesDescription;
					if (prefix.Length > 16)
						prefix = prefix.Remove(16);
				}
				else
				{
					string modality = (provider.ImageSop.Modality ?? "").Trim();
					if (!String.IsNullOrEmpty(modality))
					{
						prefix = String.Format("{0}{1}", modality, provider.ImageSop.SeriesNumber);
						if (provider.ImageSop.NumberOfFrames > 1)
							prefix = String.Format("{0}.{1}", prefix, provider.ImageSop.InstanceNumber);
					}
				}
			}

			//This method returns a number of characters, including :\/*?
			foreach (char invalidChar in Path.GetInvalidFileNameChars())
				prefix = prefix.Replace(invalidChar, ' '); //replace invalid characters with spaces.

			return prefix.Trim();
		}

		private static string GetDisplaySetDirectory(IDisplaySet displaySet, string baseDirectory)
		{
			string prefix = GetDisplaySetDirectoryPrefix(displaySet);

			int i = 0;
			string directory = prefix;

			while (true)
			{
				string path = String.Format("{0}\\{1}", baseDirectory, directory);
				if (!Directory.Exists(path))
					return path;

				directory = string.Format("{0}{1}", prefix, ++i);
			}
		}

		private static string GetImageFileName(string baseDirectory, string prefix, string fileExtension, int numberOfZeros, ref int startNumber)
		{
			do
			{
				string numericPortion;
				if (numberOfZeros == 0)
				{
					numericPortion = startNumber.ToString();
				}
				else
				{
					string format = new string('0', numberOfZeros);
					numericPortion = startNumber.ToString(format);
				}

				string path = String.Format("{0}\\{1}{2}.{3}", baseDirectory, prefix, numericPortion, fileExtension);
				if (!File.Exists(path))
					return path;

				++startNumber;

			} while (true);
		}
	}
}
