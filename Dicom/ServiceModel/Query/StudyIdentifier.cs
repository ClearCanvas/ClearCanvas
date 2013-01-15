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
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
    public interface IStudyIdentifier : IStudyData, IIdentifier
    {
    }

    /// <summary>
    /// Query identifier for a study.
    /// </summary>
    [DataContract(Namespace = QueryNamespace.Value)]
    public class StudyIdentifier : Identifier, IStudyIdentifier
    {
        #region Private Fields

        #endregion

        #region Public Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StudyIdentifier()
        {
        }

        public StudyIdentifier(IStudyIdentifier other)
            : base(other)
        {
            CopyFrom(other);
        }

        public StudyIdentifier(IStudyData other, IIdentifier identifier)
            : base(identifier)
        {
            CopyFrom(other);
        }

        public StudyIdentifier(IStudyData other)
        {
            CopyFrom(other);
        }

        /// <summary>
        /// Creates an instance of <see cref="StudyIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        public StudyIdentifier(DicomAttributeCollection attributes)
            : base(attributes)
        {
        }

        #endregion

        private void CopyFrom(IStudyData other)
        {
            if (other == null)
                return;

            ReferringPhysiciansName = other.ReferringPhysiciansName;
            AccessionNumber = other.AccessionNumber;
            StudyDescription = other.StudyDescription;
            StudyId = other.StudyId;
            StudyDate = other.StudyDate;
            StudyTime = other.StudyTime;
            /// TODO (CR Jun 2012): Technically, should copy the buffers.
            ModalitiesInStudy = other.ModalitiesInStudy;
            SopClassesInStudy = other.SopClassesInStudy;
            StudyInstanceUid = other.StudyInstanceUid;
            NumberOfStudyRelatedSeries = other.NumberOfStudyRelatedSeries;
            NumberOfStudyRelatedInstances = other.NumberOfStudyRelatedInstances;
        }

        public override string ToString()
        {
            return String.Format("{0} | {1}", StudyDescription, StudyInstanceUid);
        }

        #region Public Properties

        /// <summary>
        /// Gets the level of the query - STUDY.
        /// </summary>
        public override string QueryRetrieveLevel
        {
            get { return "STUDY"; }
        }

        /// <summary>
        /// Gets or sets the Study Instance Uid of the identified study.
        /// </summary>
        [DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = true)]
        public string StudyInstanceUid { get; set; }

        /// <summary>
        /// Gets or sets the modalities in the identified study.
        /// </summary>
        [DicomField(DicomTags.SopClassesInStudy, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string[] SopClassesInStudy { get; set; }

        /// <summary>
        /// Gets or sets the modalities in the identified study.
        /// </summary>
        [DicomField(DicomTags.ModalitiesInStudy, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string[] ModalitiesInStudy { get; set; }

        /// <summary>
        /// Gets or sets the study description of the identified study.
        /// </summary>
        [DicomField(DicomTags.StudyDescription, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string StudyDescription { get; set; }

        /// <summary>
        /// Gets or sets the study id of the identified study.
        /// </summary>
        [DicomField(DicomTags.StudyId, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string StudyId { get; set; }

        /// <summary>
        /// Gets or sets the study date of the identified study.
        /// </summary>
        [DicomField(DicomTags.StudyDate, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string StudyDate { get; set; }

        /// <summary>
        /// Gets or sets the study time of the identified study.
        /// </summary>
        [DicomField(DicomTags.StudyTime, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string StudyTime { get; set; }

        /// <summary>
        /// Gets or sets the accession number of the identified study.
        /// </summary>
        [DicomField(DicomTags.AccessionNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string AccessionNumber { get; set; }

        [DicomField(DicomTags.ReferringPhysiciansName, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public string ReferringPhysiciansName { get; set; }

        /// <summary>
        /// Gets or sets the number of series belonging to the identified study.
        /// </summary>
        [DicomField(DicomTags.NumberOfStudyRelatedSeries, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public int? NumberOfStudyRelatedSeries { get; set; }

        /// <summary>
        /// Gets or sets the number of composite object instances belonging to the identified study.
        /// </summary>
        [DicomField(DicomTags.NumberOfStudyRelatedInstances, CreateEmptyElement = true, SetNullValueIfEmpty = true),
         DataMember(IsRequired = false)]
        public int? NumberOfStudyRelatedInstances { get; set; }

        #endregion
    }
}