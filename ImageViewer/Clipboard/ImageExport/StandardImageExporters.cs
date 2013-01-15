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

using System.Collections.Generic;
using System.Drawing.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal static class StandardImageExporterFactory
	{
		public static List<IImageExporter> CreateStandardExporters()
		{
			List<IImageExporter> exporters = new List<IImageExporter>();
			
			exporters.Add(new StandardImageExporter("Jpg", "DescriptionJpg", ImageFormat.Jpeg, new string[] { "jpg", "jpeg" }));
			exporters.Add(new StandardImageExporter("Png", "DescriptionPng", ImageFormat.Png, new string[] { "png" }));
			exporters.Add(new StandardImageExporter("Bmp", "DescriptionBmp", ImageFormat.Bmp, new string[] { "bmp" }));
			exporters.Add(new StandardImageExporter("Gif", "DescriptionGif", ImageFormat.Gif, new string[] { "gif" }));
			exporters.Add(new StandardImageExporter("Tiff", "DescriptionTiff", ImageFormat.Tiff, new string[] { "tif", "tiff" }));

			return exporters;
		}
	}

	internal class StandardImageExporter : ImageExporter
	{
		private readonly ImageFormat _imageFormat;

		internal StandardImageExporter( string identifier, string description, ImageFormat imageFormat, string[] fileExtensions)
			: base(identifier, description, fileExtensions)
		{
			Platform.CheckForNullReference(imageFormat, "imageFormat");
			_imageFormat = imageFormat;
		}

		public override void Export(IPresentationImage image, string filePath, ExportImageParams exportParams)
		{
			Export(image, filePath, exportParams, _imageFormat);
		}
	}
}
