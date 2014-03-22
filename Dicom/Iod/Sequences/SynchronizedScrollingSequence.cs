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
    /// SynchronizedScrollingSequence
    /// </summary>
    public class SynchronizedScrollingSequence : SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedScrollingSequence"/> class.
        /// </summary>
        public SynchronizedScrollingSequence() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedScrollingSequence"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public SynchronizedScrollingSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Gets or sets the value of DisplaySetScrollingGroup in the underlying collection. Type 1.
        /// </summary>
         public string DisplaySetScrollingGroup
        {
            get { return DicomAttributeProvider[DicomTags.DisplaySetScrollingGroup].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "DisplaySetScrollingGroup is Type 1 Required.");
                DicomAttributeProvider[DicomTags.DisplaySetScrollingGroup].SetStringValue(value);
            }
        }
    }
}
