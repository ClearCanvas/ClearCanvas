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
	public interface ISeriesData
	{
		/// <summary>
		/// Gets the Study Instance UID of the identified series.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

		/// <summary>
		/// Gets the Series Instance UID of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid)]
		string SeriesInstanceUid { get; }

		/// <summary>
		/// Gets the modality of the identified series.
		/// </summary>
		[DicomField(DicomTags.Modality)]
		string Modality { get; }

		/// <summary>
		/// Gets the series description of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesDescription)]
		string SeriesDescription { get; }

		/// <summary>
		/// Gets the series number of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesNumber)]
		int SeriesNumber { get; }

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified series.
		/// </summary>
		[DicomField(DicomTags.NumberOfSeriesRelatedInstances)]
		int? NumberOfSeriesRelatedInstances { get; }
	}
}