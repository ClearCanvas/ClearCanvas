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
    ///ImageSetSelectorSequence
    /// </summary>
    public class ImageSetSelectorSequence : SequenceIodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSetSelectorSequence"/> class.
        /// </summary>
        public ImageSetSelectorSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSetSelectorSequence"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ImageSetSelectorSequence(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }

        /// <summary>
        /// Gets or sets the value of ImageSetSelectorUsageFlag in the underlying collection. Type 1.
        /// string instead of enum
        /// </summary>
        public string ImageSetSelectorUsageFlag
        {
            get { return DicomAttributeProvider[DicomTags.ImageSetSelectorUsageFlag].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "ImageSetSelectorUsageFlag is Type 1 Required.");
                DicomAttributeProvider[DicomTags.ImageSetSelectorUsageFlag].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorAttribute in the underlying collection. Type 1.
        /// </summary>
        public string SelectorAttribute
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAttribute].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "SelectorAttribute is Type 1 Required.");
                DicomAttributeProvider[DicomTags.SelectorAttribute].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of SelectorAttributeVR in the underlying collection. Type 1.
        /// string instead of enum
        /// </summary>
        public string SelectorAttributeVR
        {
            get { return DicomAttributeProvider[DicomTags.SelectorAttributeVr].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "SelectorAttributeVr] is Type 1 Required.");
                DicomAttributeProvider[DicomTags.SelectorAttributeVr].SetStringValue(value);
            }
        }

        public HangingProtocolSelectorAttributeValueMacro HangingProtocolSelectorAttributeValueMacro { set; get; }//implementing

        public HangingProtocolSelectorAttributeValueMacro HangingProtocolSelectorAttributeContextMacro { set; get; }//implementing

        /// <summary>
        /// Gets or sets the value of SelectorValueNumber in the underlying collection. Type 1.
        /// string instead of enum
        /// </summary>
        public string SelectorValueNumber
        {
            get { return DicomAttributeProvider[DicomTags.SelectorValueNumber].ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "SelectorValueNumber] is Type 1 Required.");
                DicomAttributeProvider[DicomTags.SelectorValueNumber].SetStringValue(value);
            }
        }
    }
}

