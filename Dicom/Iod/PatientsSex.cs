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
	/// Enumerated values for the <see cref="DicomTags.PatientsSex"/> attribute indicating the sex of the named patient.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	public enum PatientsSex
	{
		/// <summary>
		/// male.
		/// </summary>
		M,

		/// <summary>
		/// female.
		/// </summary>
		F,

		/// <summary>
		/// other.
		/// </summary>
		O,

		/// <summary>
		/// Represents the null value.
		/// </summary>
		None
	}
}
