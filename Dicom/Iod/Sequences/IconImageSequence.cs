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
	/// <summary>
	/// IconImage Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.1 (Table C.7-9)</remarks>
	public class IconImageSequence : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IconImageSequence"/> class.
		/// </summary>
		public IconImageSequence() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IconImageSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IconImageSequence(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}
	}
}