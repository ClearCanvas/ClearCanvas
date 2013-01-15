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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Patient Identification Module, as per Part 3, C.4.13
    /// </summary>
    public class PerformedProcedureStepRelationshipModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedProcedureStepRelationshipModuleIod"/> class.
        /// </summary>
        public PerformedProcedureStepRelationshipModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedProcedureStepRelationshipModuleIod"/> class.
        /// </summary>
		public PerformedProcedureStepRelationshipModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the patients.
        /// </summary>
        /// <value>The name of the patients.</value>
        public PersonName PatientsName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.PatientsName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.PatientsName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        /// <value>The patient id.</value>
        public string PatientId
        {
            get { return base.DicomAttributeProvider[DicomTags.PatientId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the issuer of patient id.
        /// </summary>
        /// <value>The issuer of patient id.</value>
        public string IssuerOfPatientId
        {
            get { return base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the patients birth date (only, no time).
        /// </summary>
        /// <value>The patients birth date.</value>
        public DateTime? PatientsBirthDate
        {
        	get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeProvider, 0, DicomTags.PatientsBirthDate, 0);  }
        
            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider, 0, DicomTags.PatientsBirthDate, 0); }
        }

        /// <summary>
        /// Gets or sets the patients sex.
        /// </summary>
        /// <value>The patients sex.</value>
        public PatientsSex PatientsSex
        {
            get { return IodBase.ParseEnum<PatientsSex>(base.DicomAttributeProvider[DicomTags.PatientsSex].GetString(0, String.Empty), PatientsSex.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PatientsSex], value); }
        }

        /// <summary>
        /// Gets the referenced patient sequence list.
        /// </summary>
        /// <value>The referenced patient sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedPatientSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets the scheduled step attributes sequence list.
        /// </summary>
        /// <value>The scheduled step attributes sequence list.</value>
        public SequenceIodList<ScheduledStepAttributesSequenceIod> ScheduledStepAttributesSequenceList
        {
            get
            {
                return new SequenceIodList<ScheduledStepAttributesSequenceIod>(base.DicomAttributeProvider[DicomTags.ScheduledStepAttributesSequence] as DicomAttributeSQ);
            }
        }

        #endregion

    }

    
}
