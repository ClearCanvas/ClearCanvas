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
    /// As per Dicom Doc 3, Table C.4-12 (pg 248)
    /// </summary>
    public class ImagingServiceRequestModule : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingServiceRequestModule"/> class.
        /// </summary>
        public ImagingServiceRequestModule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingServiceRequestModule"/> class.
        /// </summary>
		public ImagingServiceRequestModule(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the imaging service request comments.
        /// </summary>
        /// <value>The imaging service request comments.</value>
        public string ImagingServiceRequestComments
        {
            get { return base.DicomAttributeProvider[DicomTags.ImagingServiceRequestComments].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ImagingServiceRequestComments].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the requesting physician.
        /// </summary>
        /// <value>The requesting physician.</value>
        public PersonName RequestingPhysician
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.RequestingPhysician].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.RequestingPhysician].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the name of the referring physicians.
        /// </summary>
        /// <value>The name of the referring physicians.</value>
        public PersonName ReferringPhysiciansName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        /// <value>The requesting service.</value>
        public string RequestingService
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestingService].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestingService].SetString(0, value); }
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
        /// Gets or sets the issue date of imaging service request.
        /// </summary>
        /// <value>The issue date of imaging service request.</value>
        public DateTime? IssueDateOfImagingServiceRequest
        {
        	get { return DateTimeParser.ParseDateAndTime(String.Empty, 
        					base.DicomAttributeProvider[DicomTags.IssueDateOfImagingServiceRequest].GetString(0, String.Empty), 
                  base.DicomAttributeProvider[DicomTags.IssueTimeOfImagingServiceRequest].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider[DicomTags.IssueDateOfImagingServiceRequest], base.DicomAttributeProvider[DicomTags.IssueTimeOfImagingServiceRequest]); }
        }

        /// <summary>
        /// Gets or sets the placer order number imaging service request.
        /// </summary>
        /// <value>The placer order number imaging service request.</value>
        public string PlacerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeProvider[DicomTags.PlacerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PlacerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the filler order number imaging service request.
        /// </summary>
        /// <value>The filler order number imaging service request.</value>
        public string FillerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeProvider[DicomTags.FillerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.FillerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the admission id.
        /// </summary>
        /// <value>The admission id.</value>
        public string AdmissionId
        {
            get { return base.DicomAttributeProvider[DicomTags.AdmissionId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.AdmissionId].SetString(0, value); }
        }
        
        #endregion

    }
}
