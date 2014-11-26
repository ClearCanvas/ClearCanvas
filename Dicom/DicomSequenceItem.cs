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

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// A class representing a DICOM Sequence Item.
    /// </summary>
    public class DicomSequenceItem : DicomAttributeCollection, IReadOnlyDicomSequenceItem
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DicomSequenceItem() : base(0x00000000,0xFFFFFFFF)
        {
        }

        /// <summary>
        /// Internal constructor used when making a copy of a <see cref="DicomAttributeCollection"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyBinary"></param>
        /// <param name="copyPrivate"></param>
        /// <param name="copyUnknown"></param>
        internal DicomSequenceItem(DicomAttributeCollection source, bool copyBinary, bool copyPrivate, bool copyUnknown)
            : base(source, copyBinary, copyPrivate, copyUnknown)
        {
        }
        #endregion

        #region Public Overridden Methods
        /// <summary>
        /// Create a copy of this DicomSequenceItem.
        /// </summary>
        /// <returns>The copied DicomSequenceItem.</returns>
        public override DicomAttributeCollection Copy()
        {
        	return Copy(true, true, true);
        }

    	/// <summary>
    	/// Creates a copy of this DicomSequenceItem.
    	/// </summary>
    	/// <param name="copyBinary">When set to false, the copy will not include <see cref="DicomAttribute"/>
    	/// instances that are of type <see cref="DicomAttributeOB"/>, <see cref="DicomAttributeOW"/>,
    	/// or <see cref="DicomAttributeOF"/>.</param>
    	/// <param name="copyPrivate">When set to false, the copy will not include Private tags</param>
    	/// <param name="copyUnknown">When set to false, the copy will not include UN VR tags</param>
    	/// <returns>The copied DicomSequenceItem.</returns>
    	public override DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown)
        {
            return new DicomSequenceItem(this,copyBinary,copyPrivate,copyUnknown);
        }
        #endregion

	    #region IReadOnlyDicomSequenceItem implementation

	    IReadOnlyDicomAttribute IReadOnlyDicomSequenceItem.GetAttribute(uint dicomTag)
	    {
		    return GetAttribute(dicomTag);
	    }

	    IReadOnlyDicomAttribute IReadOnlyDicomSequenceItem.GetAttribute(DicomTag dicomTag)
	    {
		    return GetAttribute(dicomTag);
	    }

	    #endregion
    }
}
