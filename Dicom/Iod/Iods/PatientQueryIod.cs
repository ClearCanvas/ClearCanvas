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

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Patient Query Retrieve items.
    /// </summary>
    public class PatientQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientQueryIod"/> class.
        /// </summary>
        public PatientQueryIod()
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientQueryIod"/> class.
        /// </summary>
		public PatientQueryIod(IDicomAttributeProvider dicomAttributeProvider)
            :base(dicomAttributeProvider)
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        /// <value>The patient id.</value>
        public string PatientId
        {
            get { return DicomAttributeProvider[DicomTags.PatientId].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.PatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>The name of the patients.</value>
        public PersonName PatientsName
        {
            get { return new PersonName(DicomAttributeProvider[DicomTags.PatientsName].GetString(0, String.Empty)); }
            set { DicomAttributeProvider[DicomTags.PatientsName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the patients birth date.
        /// </summary>
        /// <value>The patients birth date.</value>
        public DateTime PatientsBirthDate
        {
            get { return DicomAttributeProvider[DicomTags.PatientsBirthDate].GetDateTime(0, DateTime.MinValue); }
            set { DicomAttributeProvider[DicomTags.PatientsBirthDate].SetDateTime(0, value); }
        }

        /// <summary>
        /// Gets or sets the patients sex.
        /// </summary>
        /// <value>The patients sex.</value>
        public string PatientsSex
        {
            get { return DicomAttributeProvider[DicomTags.PatientsSex].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.PatientsSex].SetString(0, value); }
        }

		/// <summary>
		/// Gets or sets the number of patient related instances.
		/// </summary>
		/// <value>The number of patient related instances.</value>
		public uint NumberOfPatientRelatedInstances
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfPatientRelatedInstances].GetUInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfPatientRelatedInstances].SetUInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the number of patient related series.
		/// </summary>
		/// <value>The number of patient related series.</value>
		public uint NumberOfPatientRelatedSeries
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfPatientRelatedSeries].GetUInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfPatientRelatedSeries].SetUInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the number of patient related studies.
		/// </summary>
		/// <value>The number of patient related studies.</value>
		public uint NumberOfPatientRelatedStudies
		{
			get { return DicomAttributeProvider[DicomTags.NumberOfPatientRelatedStudies].GetUInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.NumberOfPatientRelatedStudies].SetUInt32(0, value); }
		}

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a patient query retrieve request.
        /// </summary>
         public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeProvider);
        }

        /// <summary>
        /// Sets the common tags for a patient query retrieve request.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
			SetAttributeFromEnum(dicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);

			// Always set the Patient 
			dicomAttributeProvider[DicomTags.PatientsName].SetString(0, "*");
			dicomAttributeProvider[DicomTags.PatientId].SetNullValue();
			dicomAttributeProvider[DicomTags.PatientsBirthDate].SetNullValue();
			dicomAttributeProvider[DicomTags.PatientsBirthTime].SetNullValue();
			dicomAttributeProvider[DicomTags.PatientsSex].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfPatientRelatedStudies].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfPatientRelatedSeries].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfPatientRelatedInstances].SetNullValue();
		}
        #endregion
    }

}
