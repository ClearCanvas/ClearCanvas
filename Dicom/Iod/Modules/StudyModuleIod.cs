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
    /// Study Module IOD (in progress)
    /// </summary>
    public class StudyModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PatientModule class.
        /// </summary>
        public StudyModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Iod class.
        /// </summary>
        /// <param name="_dicomAttributeCollection"></param>
		public StudyModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the study instance uid.
        /// </summary>
        /// <value>The study instance uid.</value>
        public string StudyInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>The accession number.</value>
        public string AccessionNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.AccessionNumber].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.AccessionNumber].SetString(0, value); }
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
        #endregion
    }
}
