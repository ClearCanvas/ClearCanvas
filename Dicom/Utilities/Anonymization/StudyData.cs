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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	/// <summary>
	/// A class containing commonly anonymized dicom study attributes.
	/// </summary>
	[Cloneable(true)]
	public class StudyData : IStudyRootData
	{
		private string _patientId = "";
		private string _patientsNameRaw = "";
		private string _patientsBirthDateRaw = "";
		private string _patientsSex = "";
		private string _accessionNumber = "";
		private string _studyInstanceUid = "";
		private string _studyDescription = "";
		private string _studyId = "";
		private string _studyDateRaw = "";

		/// <summary>
		/// Constructor.
		/// </summary>
		public StudyData() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		public StudyData(IStudyRootData sourceData)
		{
			if (sourceData != null)
			{
				_patientId = sourceData.PatientId;
				_patientsNameRaw = sourceData.PatientsName;
				_patientsBirthDateRaw = sourceData.PatientsBirthDate;
				_patientsSex = sourceData.PatientsSex;
				_accessionNumber = sourceData.AccessionNumber;
				_studyDescription = sourceData.StudyDescription;
				_studyId = sourceData.StudyId;
				_studyDateRaw = sourceData.StudyDate;
			}
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public PersonName PatientsName
		{
			get { return new PersonName(PatientsNameRaw); }
			set
			{
				string p = null;
				if (value != null)
					p = value.ToString();

				PatientsNameRaw = p ?? "";
			}
		}

		/// <summary>
		/// Gets or sets the study date.
		/// </summary>
		public DateTime? StudyDate
		{
			get { return DateParser.Parse(StudyDateRaw); }
			set
			{
				if (value == null)
					StudyDateRaw = "";
				else
					StudyDateRaw = value.Value.ToString(DateParser.DicomDateFormat) ?? "";
			}
		}

		/// <summary>
		/// Gets or sets the patient's birth date.
		/// </summary>
		public DateTime? PatientsBirthDate
		{
			get { return DateParser.Parse(PatientsBirthDateRaw); }
			set
			{
				if (value == null)
					PatientsBirthDateRaw = "";
				else
					PatientsBirthDateRaw = value.Value.ToString(DateParser.DicomDateFormat) ?? "";
			}
		}

		/// <summary>
		/// Gets or sets the patient id.
		/// </summary>
		[DicomField(DicomTags.PatientId)]
		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the patients name, as a raw string.
		/// </summary>
		[DicomField(DicomTags.PatientsName)]
		public string PatientsNameRaw
		{
			get { return _patientsNameRaw; }
			set { _patientsNameRaw = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the patient's birth date, as a raw string.
		/// </summary>
		[DicomField(DicomTags.PatientsBirthDate)]
		public string PatientsBirthDateRaw
		{
			get { return _patientsBirthDateRaw; }
			set { _patientsBirthDateRaw = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the patient's sex.
		/// </summary>
		[DicomField(DicomTags.PatientsSex)]
		public string PatientsSex
		{
			get { return _patientsSex; }
			set { _patientsSex = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the accession number.
		/// </summary>
		[DicomField(DicomTags.AccessionNumber)]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set { _accessionNumber = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		[DicomField(DicomTags.StudyDescription)]
		public string StudyDescription
		{
			get { return _studyDescription; }
			set { _studyDescription = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the study id.
		/// </summary>
		[DicomField(DicomTags.StudyId)]
		public string StudyId
		{
			get { return _studyId; }
			set { _studyId = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the study date, as a raw string.
		/// </summary>
		[DicomField(DicomTags.StudyDate)]
		public string StudyDateRaw
		{
			get { return _studyDateRaw; }
			set { _studyDateRaw = value ?? ""; }
		}

		internal string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value ?? ""; }
		}

		internal void LoadFrom(DicomFile file)
		{
			file.DataSet.LoadDicomFields(this);
			this.StudyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid];
		}

		internal void SaveTo(DicomFile file)
		{
			file.DataSet.SaveDicomFields(this);
			file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(this.StudyInstanceUid);
		}

		/// <summary>
		/// Creates a deep clone of this instance.
		/// </summary>
		public StudyData Clone()
		{
			return CloneBuilder.Clone(this) as StudyData;
		}

		#region IStudyRootData Implementation

		string IStudyData.StudyInstanceUid
		{
			get { return string.Empty; }
		}

		string[] IStudyData.SopClassesInStudy
		{
			get { return new string[0]; }
		}

		string[] IStudyData.ModalitiesInStudy
		{
			get { return new string[0]; }
		}

		string IStudyData.StudyDate
		{
			get { return StudyDateRaw; }
		}

		string IStudyData.StudyTime
		{
			get { return string.Empty; }
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return string.Empty; }
		}

		int? IStudyData.NumberOfStudyRelatedSeries
		{
			get { return null; }
		}

		int? IStudyData.NumberOfStudyRelatedInstances
		{
			get { return null; }
		}

		string IPatientData.PatientsName
		{
			get { return PatientsNameRaw; }
		}

		string IPatientData.PatientsBirthDate
		{
			get { return PatientsBirthDateRaw; }
		}

		string IPatientData.PatientsBirthTime
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientSpeciesDescription
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientSpeciesCodeSequenceCodeValue
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientSpeciesCodeSequenceCodeMeaning
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientBreedDescription
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientBreedCodeSequenceCodeValue
		{
			get { return string.Empty; }
		}

		string IPatientData.PatientBreedCodeSequenceCodeMeaning
		{
			get { return string.Empty; }
		}

		string IPatientData.ResponsiblePerson
		{
			get { return string.Empty; }
		}

		string IPatientData.ResponsiblePersonRole
		{
			get { return string.Empty; }
		}

		string IPatientData.ResponsibleOrganization
		{
			get { return string.Empty; }
		}

		#endregion
	}
}