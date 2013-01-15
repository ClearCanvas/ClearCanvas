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
	//internal for now b/c the patient root query stuff is
    public interface IPatientRootData : IPatientData
	{
		[DicomField(DicomTags.NumberOfPatientRelatedStudies)]
		int? NumberOfPatientRelatedStudies { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedSeries)]
		int? NumberOfPatientRelatedSeries { get; }

		[DicomField(DicomTags.NumberOfPatientRelatedInstances)]
		int? NumberOfPatientRelatedInstances { get; }
	}

	public interface IPatientData
	{
		[DicomField(DicomTags.PatientId)]
		string PatientId { get; }

		[DicomField(DicomTags.PatientsName)]
		string PatientsName { get; }

		[DicomField(DicomTags.PatientsBirthDate)]
		string PatientsBirthDate { get; }

		[DicomField(DicomTags.PatientsBirthTime)]
		string PatientsBirthTime { get; }

		[DicomField(DicomTags.PatientsSex)]
		string PatientsSex { get; }

		#region Species

		[DicomField(DicomTags.PatientSpeciesDescription)]
		string PatientSpeciesDescription { get; }

		[DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientSpeciesCodeSequence)]
		string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; }

		[DicomField(DicomTags.CodeValue, DicomTags.PatientSpeciesCodeSequence)]
		string PatientSpeciesCodeSequenceCodeValue { get; }

		[DicomField(DicomTags.CodeMeaning, DicomTags.PatientSpeciesCodeSequence)]
		string PatientSpeciesCodeSequenceCodeMeaning { get; }

		#endregion

		#region Breed

		[DicomField(DicomTags.PatientBreedDescription)]
		string PatientBreedDescription { get; }

		[DicomField(DicomTags.CodingSchemeDesignator, DicomTags.PatientBreedCodeSequence)]
		string PatientBreedCodeSequenceCodingSchemeDesignator { get; }

		[DicomField(DicomTags.CodeValue, DicomTags.PatientBreedCodeSequence)]
		string PatientBreedCodeSequenceCodeValue { get; }

		[DicomField(DicomTags.CodeMeaning, DicomTags.PatientBreedCodeSequence)]
		string PatientBreedCodeSequenceCodeMeaning { get; }

		#endregion

		#region Responsible Person/Organization

		[DicomField(DicomTags.ResponsiblePerson)]
		string ResponsiblePerson { get; }

		[DicomField(DicomTags.ResponsiblePersonRole)]
		string ResponsiblePersonRole { get; }

		[DicomField(DicomTags.ResponsibleOrganization)]
		string ResponsibleOrganization { get; }

		#endregion
	}
}