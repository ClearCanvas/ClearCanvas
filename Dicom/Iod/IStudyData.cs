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
	public interface IStudyRootData : IStudyData, IPatientData
	{}

	public interface IStudyData
	{
		/// <summary>
		/// Gets the Study Instance UID of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

        /// <summary>
        /// Gets the sop classes in the identified study.
        /// </summary>
        [DicomField(DicomTags.SopClassesInStudy)]
        string[] SopClassesInStudy { get; }
        
        /// <summary>
		/// Gets the modalities in the identified study.
		/// </summary>
		[DicomField(DicomTags.ModalitiesInStudy)]
		string[] ModalitiesInStudy { get; }

		/// <summary>
		/// Gets the study description of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyDescription)]
		string StudyDescription { get; }

		/// <summary>
		/// Gets the study ID of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyId)]
		string StudyId { get; }

		/// <summary>
		/// Gets the study date of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyDate)]
		string StudyDate { get; }

		/// <summary>
		/// Gets the study time of the identified study.
		/// </summary>
		[DicomField(DicomTags.StudyTime)]
		string StudyTime { get; }

		/// <summary>
		/// Gets the accession number of the identified study.
		/// </summary>
		[DicomField(DicomTags.AccessionNumber)]
		string AccessionNumber { get; }

		[DicomField(DicomTags.ReferringPhysiciansName)]
		string ReferringPhysiciansName { get; }

		/// <summary>
		/// Gets the number of series belonging to the identified study.
		/// </summary>
		[DicomField(DicomTags.NumberOfStudyRelatedSeries)]
		int? NumberOfStudyRelatedSeries { get; }

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified study.
		/// </summary>
		[DicomField(DicomTags.NumberOfStudyRelatedInstances)]
		int? NumberOfStudyRelatedInstances { get; }
	}
}