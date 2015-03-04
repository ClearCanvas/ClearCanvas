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

using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a file dialog extension filter.
	/// </summary>
	public interface IFileExtensionFilter
	{
		/// <summary>
		/// Gets the filter expression of the filter (e.g *.txt).
		/// </summary>
		string Filter { get; }

		/// <summary>
		/// Gets the user-friendly description displayed for the filter (e.g. Text Files (*.txt)).
		/// </summary>
		string Description { get; }
	}

	/// <summary>
	/// Describes a file dialog extension filter.
	/// </summary>
	public class FileExtensionFilter : IFileExtensionFilter
	{
		/// <summary>
		/// Initializes a new instance of <see cref="FileExtensionFilter"/>.
		/// </summary>
		public FileExtensionFilter() {}

		/// <summary>
		/// Initializes a new instance of <see cref="FileExtensionFilter"/>.
		/// </summary>
		public FileExtensionFilter(string filter, [param : Localizable(true)] string description)
		{
			Filter = filter;
			Description = description;
		}

		public string Filter { get; set; }

		[Localizable(true)]
		public string Description { get; set; }

		public override string ToString()
		{
			return !string.IsNullOrEmpty(Description) ? Description : Filter;
		}

		#region Standard Filters

		public static readonly IFileExtensionFilter AllFiles = new StandardFileExtensionFilter("*.*", SR.LabelAllFilesFilter);

		public static readonly IFileExtensionFilter DicomFiles = new StandardFileExtensionFilter("*.dcm", SR.LabelDicomFilesFilter);
		public static readonly IFileExtensionFilter DicomDirFiles = new StandardFileExtensionFilter("DICOMDIR", SR.LabelDicomDirFilter);

		public static readonly IFileExtensionFilter TxtFiles = new StandardFileExtensionFilter("*.txt", SR.LabelTextFilesFilter);
		public static readonly IFileExtensionFilter LogFiles = new StandardFileExtensionFilter("*.log", SR.LabelLogFilesFilter);
		public static readonly IFileExtensionFilter XmlFiles = new StandardFileExtensionFilter("*.xml", SR.LabelXmlFilesFilter);

		public static readonly IFileExtensionFilter RtfFiles = new StandardFileExtensionFilter("*.rtf", SR.LabelRtfFilesFilter);
		public static readonly IFileExtensionFilter PdfFiles = new StandardFileExtensionFilter("*.pdf", SR.LabelPdfFilesFilter);

		public static readonly IFileExtensionFilter RawFiles = new StandardFileExtensionFilter("*.raw", SR.LabelRawFilesFilter);

		public static readonly IFileExtensionFilter GifFiles = new StandardFileExtensionFilter("*.gif", SR.LabelGifFilesFilter);
		public static readonly IFileExtensionFilter BmpFiles = new StandardFileExtensionFilter("*.bmp", SR.LabelBmpFilesFilter);
		public static readonly IFileExtensionFilter JpgFiles = new StandardFileExtensionFilter("*.jpg;*.jif;*.jpe;*.jpeg", SR.LabelJpegFilesFilter);
		public static readonly IFileExtensionFilter PngFiles = new StandardFileExtensionFilter("*.png", SR.LabelPngFilesFilter);
		public static readonly IFileExtensionFilter TifFiles = new StandardFileExtensionFilter("*.tif;*.tiff", SR.LabelTiffFilesFilter);

		public static readonly IFileExtensionFilter ZipFiles = new StandardFileExtensionFilter("*.zip", SR.LabelZipFilesFilter);
		public static readonly IFileExtensionFilter TarFiles = new StandardFileExtensionFilter("*.tar", SR.LabelTarFilesFilter);
		public static readonly IFileExtensionFilter TarGzFiles = new StandardFileExtensionFilter("*.tar.gz", SR.LabelTarGzFilesFilter);
		public static readonly IFileExtensionFilter GzFiles = new StandardFileExtensionFilter("*.gz", SR.LabelGzFilesFilter);
		public static readonly IFileExtensionFilter TarBz2Files = new StandardFileExtensionFilter("*.tar.bz2", SR.LabelTarBz2FilesFilter);
		public static readonly IFileExtensionFilter Bz2Files = new StandardFileExtensionFilter("*.bz2", SR.LabelBz2FilesFilter);

		private class StandardFileExtensionFilter : IFileExtensionFilter
		{
			public string Filter { get; private set; }
			public string Description { get; private set; }

			public StandardFileExtensionFilter(string filter, string description)
			{
				Filter = filter;
				Description = description;
			}

			public override string ToString()
			{
				return !string.IsNullOrEmpty(Description) ? Description : Filter;
			}
		}

		#endregion
	}
}