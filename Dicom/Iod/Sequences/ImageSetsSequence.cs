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
    ///ImageSetsSequence
    /// </summary>
    public class ImageSetsSequence:SequenceIodBase
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="ImageSetsSequence"/> class.
		/// </summary>
		public ImageSetsSequence() : base() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="ImageSetsSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public ImageSetsSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Gets or sets the value of ImageSetSelectorSequence in the underlying collection. Type 1.
        /// </summary>
        public ImageSetSelectorSequence[] ImageSetSelectorSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ImageSetSelectorSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;

                var result = new ImageSetSelectorSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new ImageSetSelectorSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    const string msg = "DeviceSequence is Type 1 Required.";
                    throw new ArgumentNullException("value", msg);
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.ImageSetSelectorSequence].Values = result;
            }
        }

        /// <summary>
        /// Gets or sets the value of TimeBasedImageSetsSequence in the underlying collection. Type 1.
        /// </summary>
        public TimeBasedImageSetsSequence[] TimeBasedImageSetsSequence
        {
            get
            {
                var dicomAttribute = DicomAttributeProvider[DicomTags.ImageSetSelectorSequence];
                if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
                    return null;

                var result = new TimeBasedImageSetsSequence[dicomAttribute.Count];
                var items = (DicomSequenceItem[])dicomAttribute.Values;
                for (int n = 0; n < items.Length; n++)
                    result[n] = new TimeBasedImageSetsSequence(items[n]);

                return result;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    const string msg = "DeviceSequence is Type 1 Required.";
                    throw new ArgumentNullException("value", msg);
                }

                var result = new DicomSequenceItem[value.Length];
                for (int n = 0; n < value.Length; n++)
                    result[n] = value[n].DicomSequenceItem;

                DicomAttributeProvider[DicomTags.TimeBasedImageSetsSequence].Values = result;
            }
        }
    }
}
