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
    /// Types of differences when two sets of attributes are compared using <see cref="DicomAttributeCollection.Equals()"/>.
    /// </summary>
    public enum ComparisonResultType
    {
        /// <summary>
        /// Cannot be compared with the target because of its type.
        /// </summary>
        InvalidType,

        /// <summary>
        /// Source and target does not have the same set of attributes.
        /// </summary>
        DifferentAttributeSet,

        /// <summary>
        /// Attributes in the source and target have different values.
        /// </summary>
        DifferentValues
    }
}