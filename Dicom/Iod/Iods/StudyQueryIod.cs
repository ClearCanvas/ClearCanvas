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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Query Retrieve items.  This is a replacement for the <see cref="ClearCanvas.Dicom.QueryResult"/>
    /// </summary>
    public class StudyQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StudyQueryIod"/> class.
        /// </summary>
        public StudyQueryIod()
            :base()
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudyQueryIod"/> class.
        /// </summary>
		public StudyQueryIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider)
        {
            SetAttributeFromEnum(DicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the study instance uid.
        /// </summary>
        /// <value>The study instance uid.</value>
        public string StudyInstanceUid
        {
            get { return DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
        }

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
        /// Gets or sets the modalities in study.
        /// </summary>
        /// <value>The modalities in study.</value>
        public string ModalitiesInStudy
        {
            get { return DicomAttributeProvider[DicomTags.ModalitiesInStudy].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.ModalitiesInStudy].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study description.
        /// </summary>
        /// <value>The study description.</value>
        public string StudyDescription
        {
            get { return DicomAttributeProvider[DicomTags.StudyDescription].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.StudyDescription].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study id.
        /// </summary>
        /// <value>The study id.</value>
        public string StudyId
        {
            get { return DicomAttributeProvider[DicomTags.StudyId].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.StudyId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study date.
        /// </summary>
        /// <value>The study date.</value>
        public DateTime? StudyDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeProvider[DicomTags.StudyDate].GetString(0, String.Empty), 
                    DicomAttributeProvider[DicomTags.StudyTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeProvider[DicomTags.StudyDate], DicomAttributeProvider[DicomTags.StudyTime]); }
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>The accession number.</value>
        public string AccessionNumber
        {
            get { return DicomAttributeProvider[DicomTags.AccessionNumber].GetString(0, String.Empty); }
            set { DicomAttributeProvider[DicomTags.AccessionNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the number of study related instances.
        /// </summary>
        /// <value>The number of study related instances.</value>
        public uint NumberOfStudyRelatedInstances
        {
            get { return DicomAttributeProvider[DicomTags.NumberOfStudyRelatedInstances].GetUInt32(0, 0); }
            set { DicomAttributeProvider[DicomTags.NumberOfStudyRelatedInstances].SetUInt32(0, value); }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a query retrieve request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeProvider);
        }

        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
			Platform.CheckForNullReference(dicomAttributeProvider, "dicomAttributeProvider");

			PatientQueryIod.SetCommonTags(dicomAttributeProvider);

			SetAttributeFromEnum(dicomAttributeProvider[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);

			dicomAttributeProvider[DicomTags.StudyInstanceUid].SetNullValue();
			dicomAttributeProvider[DicomTags.StudyId].SetNullValue();
			dicomAttributeProvider[DicomTags.StudyDate].SetNullValue();
			dicomAttributeProvider[DicomTags.StudyTime].SetNullValue();
			dicomAttributeProvider[DicomTags.StudyDescription].SetNullValue();
			dicomAttributeProvider[DicomTags.AccessionNumber].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfStudyRelatedInstances].SetNullValue();
			dicomAttributeProvider[DicomTags.NumberOfStudyRelatedSeries].SetNullValue();
			dicomAttributeProvider[DicomTags.ModalitiesInStudy].SetNullValue();
			dicomAttributeProvider[DicomTags.RequestingPhysician].SetNullValue();
			dicomAttributeProvider[DicomTags.ReferringPhysiciansName].SetNullValue();
        }
        #endregion
    }

}
