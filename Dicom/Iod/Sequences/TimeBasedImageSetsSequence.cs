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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    ///TimeBasedImageSetsSequence
    /// </summary>
    public class TimeBasedImageSetsSequence:SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBasedImageSetsSequence"/> class.
		/// </summary>
		public TimeBasedImageSetsSequence() : base() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="TimeBasedImageSetsSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public TimeBasedImageSetsSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Gets or sets the value of ImageSetNumber in the underlying collection. Type 2.
        /// </summary>
        public ushort ImageSetNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.ImageSetNumber].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ImageSetNumber].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of ImageSetSelectorCategory in the underlying collection. Type 1.
        /// </summary>
        public string ImageSetSelectorCategory
        {
            get { return DicomAttributeProvider[DicomTags.ImageSetSelectorCategory].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "ImageSetSelectorCategory is Type 1 Required.");
                DicomAttributeProvider[DicomTags.ImageSetSelectorCategory].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of RelativeTime in the underlying collection. Type 1C.
        /// </summary>
        public ushort RelativeTime
        {
            get { return base.DicomAttributeProvider[DicomTags.RelativeTime].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.RelativeTime].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of RelativeTimeUnits in the underlying collection. Type 1C.
        /// </summary>
        public string RelativeTimeUnits
        {
            get { return DicomAttributeProvider[DicomTags.RelativeTimeUnits].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.RelativeTimeUnits].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of AbstractPriorValue in the underlying collection. Type 1C.
        /// </summary>
        public short AbstractPriorValue
        {
            get { return base.DicomAttributeProvider[DicomTags.AbstractPriorValue].GetInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.AbstractPriorValue].SetInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the value of AbstractPriorCodeSequence in the underlying collection. Type 1C.
        /// </summary>
        public CodeSequenceMacro AbstractPriorCodeSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.AbstractPriorCodeSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;
                return new CodeSequenceMacro(((DicomSequenceItem[])dicomAttribute.Values)[0]);
            }
            set
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.AbstractPriorCodeSequence];
                dicomAttribute.Values = new[] { value.DicomSequenceItem };
            }
        }

        /// <summary>
        /// Gets or sets the value of ImageSetLabel in the underlying collection. Type 3.
        /// </summary>
        public string ImageSetLabel
        {
            get { return DicomAttributeProvider[DicomTags.ImageSetLabel].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ImageSetLabel].SetStringValue(value);
            }
        }
    }
}
