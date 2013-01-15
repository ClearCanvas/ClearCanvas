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
	/// Interface to an <see cref="ISopDataSource"/> whose internal source is
	/// a local <see cref="DicomFile"/>.
	/// </summary>
	public interface ILocalSopDataSource : IDicomMessageSopDataSource
	{
		//TODO (later): remove due to thread safety issues.
		
		/// <summary>
		/// Gets the source <see cref="DicomFile"/>.
		/// </summary>
		/// <remarks>See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
		/// This property will likely be removed in a future version due to thread-safety concerns.</remarks>
		DicomFile File { get; } 

		/// <summary>
		/// Gets the filename of the source <see cref="DicomFile"/>.
		/// </summary>
		string Filename { get; }
	}
}
