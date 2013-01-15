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

namespace ClearCanvas.Dicom.Iod
{
    /// <summary>
    /// Sequence IOD, subclasses <see cref="Iod"/> to take a <see cref="DicomSequenceItem"/> instead of a <see cref="DicomAttributeCollection"/>.
    /// </summary>
    public abstract class SequenceIodBase : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceIodBase"/> class.
        /// </summary>
        protected SequenceIodBase() : base(new DicomSequenceItem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceIodBase"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        protected SequenceIodBase(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem)
        {
        }

		#endregion

        #region Public Properties
        /// <summary>
        /// Gets the dicom attribute collection as a dicom sequence item.
        /// </summary>
        /// <value>The dicom sequence item.</value>
        public DicomSequenceItem DicomSequenceItem
        {
            get { return base.DicomAttributeProvider as DicomSequenceItem; }
            set { base.DicomAttributeProvider = value; }
        }
        #endregion
    }
}
