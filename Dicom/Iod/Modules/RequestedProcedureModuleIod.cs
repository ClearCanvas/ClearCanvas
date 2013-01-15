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

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom Doc 3, Table C.4-11 (pg 248)
    /// </summary>
    public class RequestedProcedureModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PatientModule class.
        /// </summary>
        public RequestedProcedureModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Iod class.
        /// </summary>
        /// <param name="_dicomAttributeCollection"></param>
		public RequestedProcedureModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        public string RequestedProcedureId
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureId].SetString(0, value); }
        }
        public string ReasonForTheRequestedProcedure
        {
            get { return base.DicomAttributeProvider[DicomTags.ReasonForTheRequestedProcedure].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ReasonForTheRequestedProcedure].SetString(0, value); }
        }

        public string RequestedProcedureComments
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureComments].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureComments].SetString(0, value); }
        }

        public string StudyInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study date.
        /// </summary>
        /// <value>The study date.</value>
        public DateTime? StudyDate
        {
            get
            {
                return DateTimeParser.ParseDateAndTime(String.Empty,
                  base.DicomAttributeProvider[DicomTags.StudyDate].GetString(0, String.Empty),
                  base.DicomAttributeProvider[DicomTags.StudyTime].GetString(0, String.Empty));
            }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider[DicomTags.StudyDate], base.DicomAttributeProvider[DicomTags.StudyTime]); }
        }

        /// <summary>
        /// Gets or sets the requested procedure description.
        /// </summary>
        /// <value>The requested procedure description.</value>
        public string RequestedProcedureDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureDescription].SetString(0, value); }
        }

        // TODO: make one with the RequestedProcedurePriority enum
        public RequestedProcedurePriority RequestedProcedurePriority
        {
            get { return IodBase.ParseEnum<RequestedProcedurePriority>(base.DicomAttributeProvider[DicomTags.RequestedProcedurePriority].GetString(0, String.Empty), RequestedProcedurePriority.None); }
            set 
            {
                string stringValue = value == RequestedProcedurePriority.None ? String.Empty : value.ToString().ToUpperInvariant();
                base.DicomAttributeProvider[DicomTags.RequestedProcedurePriority].SetString(0, stringValue); 
            }
        }
        
        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public enum RequestedProcedurePriority
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Stat,
        /// <summary>
        /// 
        /// </summary>
        High,
        /// <summary>
        /// 
        /// </summary>
        Routine,
        /// <summary>
        /// 
        /// </summary>
        Medium,
        /// <summary>
        /// 
        /// </summary>
        Low
    }
}
