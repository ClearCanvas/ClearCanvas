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

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Scheduled Step Attributes Sequence (0040,0270)
    /// </summary>
    /// <remarks>As per Dicom Doc 3, C.4-13 (pg 253)</remarks>
    public class ScheduledStepAttributesSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledStepAttributesSequuenceIod"/> class.
        /// </summary>
        public ScheduledStepAttributesSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledStepAttributesSequuenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ScheduledStepAttributesSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
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

        //TODO: Referenced Study Sequence

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
        /// Gets or sets the requested procedure id.
        /// </summary>
        /// <value>The requested procedure id.</value>
        public string RequestedProcedureId
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureId].SetString(0, value); }
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

        // TODO: Requested Procedure Code Sequence

        /// <summary>
        /// Gets or sets the scheduled procedure step id.
        /// </summary>
        /// <value>The scheduled procedure step id.</value>
        public string ScheduledProcedureStepId
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the scheduled procedure step description.
        /// </summary>
        /// <value>The scheduled procedure step description.</value>
        public string ScheduledProcedureStepDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepDescription].SetString(0, value); }
        }

        //TODO: >Scheduled Protocol Code Sequence
        
        #endregion

    }


}
