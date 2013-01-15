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

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	/// <summary>
	/// Contains a list of fields that will be updated when processing duplicates. This is the list
	/// in the server partition configuration.
	/// </summary>
	public class StudyMatchingMap
	{
		#region Public Properties

		[DicomField(DicomTags.PatientsName)]
		public string PatientsName { get; set; }

		[DicomField(DicomTags.PatientId)]
		public string PatientId { get; set; }

		[DicomField(DicomTags.IssuerOfPatientId)]
		public string IssuerOfPatientId { get; set; }

		[DicomField(DicomTags.PatientsBirthDate)]
		public string PatientsBirthDate { get; set; }

		[DicomField(DicomTags.PatientsSex)]
		public string PatientsSex { get; set; }

		[DicomField(DicomTags.AccessionNumber)]
		public string AccessionNumber { get; set; }

		[DicomField(DicomTags.StudyInstanceUid)]
		public string StudyInstanceUid { get; set; }

		[DicomField(DicomTags.StudyId)]
		public string StudyId { get; set; }

		[DicomField(DicomTags.StudyDescription)]
		public string StudyDescription { get; set; }

		[DicomField(DicomTags.StudyDate)]
		public string StudyDate { get; set; }

		[DicomField(DicomTags.StudyTime)]
		public string StudyTime { get; set; }

		#endregion
	}
}