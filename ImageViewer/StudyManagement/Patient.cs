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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM patient.
	/// </summary>
	public class Patient : IPatientData
	{
		private Sop _sop;
		private StudyCollection _studies;

		internal Patient()
		{
		}

		/// <summary>
		/// Gets the collection of <see cref="Study"/> objects that belong
		/// to this <see cref="Patient"/>.
		/// </summary>
		public StudyCollection Studies
		{
			get
			{
				if (_studies == null)
					_studies = new StudyCollection();

				return _studies;
			}
		}
		#region Patient Module

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _sop.PatientId; }
		}

		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public PersonName PatientsName
		{
			get { return _sop.PatientsName; }
		}

		string IPatientData.PatientsName
		{
			get { return _sop.PatientsName; }
		}

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
		{
			get { return _sop.PatientsBirthDate; }
		}

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthTime
		{
			get { return _sop.PatientsBirthTime; }
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
		public string PatientsSex
		{
			get { return _sop.PatientsSex; }
		}

		#region Species

		/// <summary>
		/// Gets the patient species description.
		/// </summary>
		public string PatientSpeciesDescription
		{
			get { return _sop.PatientSpeciesDescription; }
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodingSchemeDesignator
		{
			get { return _sop.PatientSpeciesCodeSequenceCodingSchemeDesignator; }
		}

		/// <summary>
		/// Gets the code value of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodeValue
		{
			get { return _sop.PatientSpeciesCodeSequenceCodeValue; }
		}

		/// <summary>
		/// Gets the code meaning of the patient species code sequence.
		/// </summary>
		public string PatientSpeciesCodeSequenceCodeMeaning
		{
			get { return _sop.PatientSpeciesCodeSequenceCodeMeaning; }
		}

		#endregion

		#region Breed

		/// <summary>
		/// Gets the patient breed description.
		/// </summary>
		public string PatientBreedDescription
		{
			get { return _sop.PatientBreedDescription; }
		}

		/// <summary>
		/// Gets the coding scheme designator of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodingSchemeDesignator
		{
			get { return _sop.PatientBreedCodeSequenceCodingSchemeDesignator; }
		}

		/// <summary>
		/// Gets the code value of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodeValue
		{
			get { return _sop.PatientBreedCodeSequenceCodeValue; }
		}

		/// <summary>
		/// Gets the code meaning of the patient breed code sequence.
		/// </summary>
		public string PatientBreedCodeSequenceCodeMeaning
		{
			get { return _sop.PatientBreedCodeSequenceCodeMeaning; }
		}

		#endregion

		#region Responsible Person/Organization

		/// <summary>
		/// Gets the responsible person for the patient.
		/// </summary>
		public PersonName ResponsiblePerson
		{
			get { return _sop.ResponsiblePerson; }
		}

		string IPatientData.ResponsiblePerson
		{
			get { return _sop.ResponsiblePerson; }
		}

		/// <summary>
		/// Gets the role of the responsible person for the patient.
		/// </summary>
		public string ResponsiblePersonRole
		{
			get { return _sop.ResponsiblePersonRole; }
		}

		/// <summary>
		/// Gets the organization responsible for the patient.
		/// </summary>
		public string ResponsibleOrganization
		{
			get { return _sop.ResponsibleOrganization; }
		}

		#endregion

		#endregion

		/// <summary>
		/// Returns the patient's name and patient ID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0} | {1}", this.PatientsName, this.PatientId);
		}

		internal void SetSop(Sop sop)
		{
			if (sop == null)
				_sop = null;
			else if (_sop == null)
				_sop = sop;
		}
	}
}
