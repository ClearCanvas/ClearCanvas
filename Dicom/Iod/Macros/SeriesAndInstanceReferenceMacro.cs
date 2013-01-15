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

using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Macros
{
    /// <summary>
    /// Series and Instance Reference Macro Attributes
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table 10.4 (pg 78)</remarks>
    public class SeriesAndInstanceReferenceMacro : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesAndInstanceReferenceMacro"/> class.
        /// </summary>
        public SeriesAndInstanceReferenceMacro()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesAndInstanceReferenceMacro"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public SeriesAndInstanceReferenceMacro(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Sequence of Items each of which includes the Attributes of one Series. 
        /// One or more Items shall be present. (0008,1115)
        /// </summary>
        /// <value>The referenced series sequence list.</value>
        public SequenceIodList<ReferencedSeriesSequenceIod> ReferencedSeriesSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedSeriesSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedSeriesSequence] as DicomAttributeSQ);
            }
        } 
        #endregion

    }
}
