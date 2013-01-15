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
	/// Enumerated values for the <see cref="ClearCanvas.Dicom.DicomTags.VerificationFlag"/> attribute indicating whether the Encapsulated Document is verified.
	/// </summary>
	/// <remarks>
	/// <para>As defined in the DICOM Standard 2009, Part 3, Section C.24.2 (Table C.24-2)</para>
	/// </remarks>
	public enum VerificationFlag
	{
		/// <summary>
		/// Represents the null value.
		/// </summary>
		None,

		/// <summary>
		/// Indicates that the Encapsulated Document is not attested by a legally accountable person.
		/// </summary>
		Unverified,

		/// <summary>
		/// Indicates that the Encapsulated Document is attested to (signed) by a Verifying Observer
		/// or Legal Authenticator named in the document, who is accountable for its content.
		/// </summary>
		Verified
	}
}