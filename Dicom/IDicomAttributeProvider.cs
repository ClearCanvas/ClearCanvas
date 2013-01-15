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
	public delegate DicomAttribute DicomAttributeGetter(uint tag);
	public delegate void DicomAttributeSetter(uint tag, DicomAttribute value);
	
	/// <summary>
	/// Interface for classes that provide <see cref="DicomAttribute"/>s.
	/// </summary>
	public interface IDicomAttributeProvider
	{
		/// <summary>
		/// Gets or sets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		DicomAttribute this[DicomTag tag] { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		DicomAttribute this[uint tag] { get; set; }

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		bool TryGetAttribute(uint tag, out DicomAttribute attribute);

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute);
	}
}