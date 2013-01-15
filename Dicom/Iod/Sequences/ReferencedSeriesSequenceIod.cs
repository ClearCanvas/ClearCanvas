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
    /// Referenced Series Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table 10.4, pg 78</remarks>
    public class ReferencedSeriesSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencedSeriesSequenceIod"/> class.
        /// </summary>
        public ReferencedSeriesSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencedSeriesSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ReferencedSeriesSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Unique identifier of the Series containing the referenced Instances.
        /// </summary>
        /// <value>The series instance uid.</value>
        public string SeriesInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.SeriesInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Sequence of Items each providing a reference to an Instance that is part of the
        /// Series defined by Series Instance UID (0020,000E) in the enclosing Item. 
        /// One or more Items shall be present.
        /// </summary>
        /// <value>The referenced film session sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedFilmSessionSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedInstanceSequence] as DicomAttributeSQ);
            }
        }        
       #endregion
    }

}
