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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// An <see cref="ISopDataSource"/> whose underlying data resides in a <see cref="DicomFile"/>
	/// on the local file system.
	/// </summary>
	/// <remarks>
	/// Pixel data is always loaded from the source file on-demand.
	/// </remarks>
	public class LocalSopDataSource : DicomMessageSopDataSource, ILocalSopDataSource
	{
		/// <summary>
		/// Constructs a new <see cref="LocalSopDataSource"/> by loading
		/// the <see cref="DicomFile"/> with the given <paramref name="fileName">file name</paramref>.
		/// </summary>
		/// <param name="fileName">The full path to the file to be loaded.</param>
		public LocalSopDataSource(string fileName)
			: base(new DicomFile(fileName))
		{
		}

		/// <summary>
		/// Constructs a new <see cref="LocalSopDataSource"/> with the given <see cref="DicomFile"/>
		/// as it's underlying data.
		/// </summary>
		/// <param name="localFile">The local file.</param>
		public LocalSopDataSource(DicomFile localFile)
			: base(localFile)
		{
		}

		#region ILocalSopDataSource Members

		/// <summary>
		/// Gets the source <see cref="DicomFile"/>.
		/// </summary>
		/// <remarks>See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
		/// This property will likely be removed in a future version due to thread-safety concerns.</remarks>
		public DicomFile File
		{
			get { return (DicomFile)GetSourceMessage(true); }
		}

		/// <summary>
		/// Gets the filename of the source <see cref="DicomFile"/>.
		/// </summary>
		public string Filename
		{
			get { return ((DicomFile)GetSourceMessage(false)).Filename; }
		}

		/// <summary>
		/// Called by the base class to ensure that all DICOM data attributes are loaded.
		/// </summary>
		protected override void EnsureLoaded()
		{
			File.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		#endregion
	}
}
