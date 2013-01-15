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
    /// Referenced Sop Class and Instance Sequence, consisting of Referenced SOP Class UID (0008,1150)
    /// and Referenced SOP Instance UID (0008,1155), and optionally .
    /// <para>This is mainly for the different sequences in the the Basic Film Box Relationship
    /// Module (Part 3, Table C 13.4, pg 867) such as Referenced Film Session Sequence, Referenced Image Box Sequence, Referenced Basic Annotation Box Sequence,
    /// etc., but there may be other uses for it.</para>
    /// </summary>
    /// <remarks>As per Part 3, Table C 13.4, pg 867</remarks>
    public class ReferencedInstanceSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencedInstanceSequenceIod"/> class.
        /// </summary>
        public ReferencedInstanceSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencedInstanceSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ReferencedInstanceSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Uniquely identifies the referenced SOP Class. (0008,1150)
        /// </summary>
        /// <value>The referenced sop class uid.</value>
        public string ReferencedSopClassUid
        {
            get { return base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ReferencedSopClassUid].SetString(0, value); }
        }

        /// <summary>
        /// Uniquely identifies the referenced SOP Instance. (0008,1155)
        /// </summary>
        /// <value>The referenced sop instance uid.</value>
        public string ReferencedSopInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ReferencedSopInstanceUid].SetString(0, value); }
        }
        
       #endregion
    }

}
