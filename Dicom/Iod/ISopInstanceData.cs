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

namespace ClearCanvas.Dicom.Iod
{
	public interface ISopInstanceData
	{
		/// <summary>
		/// Gets the Study Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

		/// <summary>
		/// Gets the Series Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid)]
		string SeriesInstanceUid { get; }

		/// <summary>
		/// Gets the Sop Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopInstanceUid)]
		string SopInstanceUid { get; }

		/// <summary>
		/// Gets the Sop Class Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopClassUid)]
		string SopClassUid { get; }

		/// <summary>
		/// Gets the Instance Number of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.InstanceNumber)]
		int InstanceNumber { get; }
	}
}