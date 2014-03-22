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


namespace ClearCanvas.Dicom.Iod.Sequences
{
    public class NavigationIndicatorSequence:SequenceIodBase
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="NavigationIndicatorSequence"/> class.
		/// </summary>
		public NavigationIndicatorSequence() : base() {}

		/// <summary>
        /// Initializes a new instance of the <see cref="NavigationIndicatorSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public NavigationIndicatorSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

        /// <summary>
        /// Gets or sets the value of NavigationDisplaySet in the underlying collection. Type 1C.
        /// </summary>
        public string NavigationDisplaySet
        {
            get { return DicomAttributeProvider[DicomTags.NavigationDisplaySet].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.NavigationDisplaySet].SetStringValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of ReferenceDisplaySets in the underlying collection. Type 1.
        /// </summary>
        public string ReferenceDisplaySets
        {
            get { return DicomAttributeProvider[DicomTags.ReferenceDisplaySets].ToString(); }
            set
            {
                DicomAttributeProvider[DicomTags.ReferenceDisplaySets].SetStringValue(value);
            }
        }
    }
}
