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
    public interface IImageIdentifier : ISopInstanceData, IIdentifier
    {
        [DicomField(DicomTags.InstanceNumber)]
        new int? InstanceNumber { get; }
    }

    /// <summary>
    /// Query identifier for a composite object instance.
    /// </summary>
    [DataContract(Namespace = QueryNamespace.Value)]
    public class ImageIdentifier : Identifier, IImageIdentifier
    {
        #region Private Fields

        private int? _instanceNumber;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImageIdentifier()
        {
        }

        public ImageIdentifier(IImageIdentifier other)
            : base(other)
        {
            CopyFrom(other);
            InstanceNumber = other.InstanceNumber;
        }

        public ImageIdentifier(ISopInstanceData other, IIdentifier identifier)
            : base(identifier)
        {
            CopyFrom(other);
        }

        public ImageIdentifier(ISopInstanceData other)
        {
            CopyFrom(other);
        }

        /// <summary>
        /// Creates an instance of <see cref="ImageIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        public ImageIdentifier(DicomAttributeCollection attributes)
            : base(attributes)
        {
        }

        #endregion

        private void CopyFrom(ISopInstanceData other)
        {
            if (other == null)
                return;

            StudyInstanceUid = other.StudyInstanceUid;
            SeriesInstanceUid = other.SeriesInstanceUid;
            SopInstanceUid = other.SopInstanceUid;
            SopClassUid = other.SopClassUid;
            InstanceNumber = other.InstanceNumber;
        }

        public override string ToString()
        {
            return String.Format("{0} | {1}", InstanceNumber, SopInstanceUid);
        }

        #region Public Properties

        /// <summary>
        /// Gets the level of the query - IMAGE.
        /// </summary>
        public override string QueryRetrieveLevel
        {
            get { return "IMAGE"; }
        }

        /// <summary>
        /// Gets or sets the Study Instance Uid of the identified sop instance.
        /// </summary>
        [DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = true)]
        public string StudyInstanceUid { get; set; }

        /// <summary>
        /// Gets or sets the Series Instance Uid of the identified sop instance.
        /// </summary>
        [DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = true)]
        public string SeriesInstanceUid { get; set; }

        /// <summary>
        /// Gets or sets the Sop Instance Uid of the identified sop instance.
        /// </summary>
        [DicomField(DicomTags.SopInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = true)]
        public string SopInstanceUid { get; set; }

        /// <summary>
        /// Gets or sets the Sop Class Uid of the identified sop instance.
        /// </summary>
        [DicomField(DicomTags.SopClassUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = true)]
        public string SopClassUid { get; set; }

        /// <summary>
        /// Gets or sets the Instance Number of the identified sop instance.
        /// </summary>
        [DicomField(DicomTags.InstanceNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        [DataMember(IsRequired = true)]
        public int? InstanceNumber
        {
            get { return _instanceNumber; }
            set { _instanceNumber = value; }
        }

        int ISopInstanceData.InstanceNumber
        {
            get { return _instanceNumber ?? 0; }
        }

        #endregion
    }
}