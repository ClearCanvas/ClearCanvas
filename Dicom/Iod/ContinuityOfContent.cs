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
	/// Enumerated values for the <see cref="DicomTags.ContinuityOfContent"/> attribute specifying for a CONTAINER whether or not
	/// its contained Content Items are logically linked in a
	/// continuous textual flow, or are separate items.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.18.8 (Table C.18.8-1)</remarks>
	public enum ContinuityOfContent
	{
		/// <summary>
		/// SEPARATE
		/// </summary>
		Separate,

		/// <summary>
		/// CONTINUOUS
		/// </summary>
		Continuous,

		/// <summary>
		/// Represents the unknown status, which is equivalent to the null value.
		/// </summary>
		Unknown
	}
}