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
	/// Functional Groups Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16 (Table C.7.6.16-1)</remarks>
	public class FunctionalGroupsSequenceItem : SequenceIodBase, IDicomAttributeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupsSequenceItem"/> class.
		/// </summary>
		public FunctionalGroupsSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionalGroupsSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FunctionalGroupsSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public T GetFunctionalGroup<T>()
			where T : FunctionalGroupMacro, new()
		{
			return new T {DicomSequenceItem = DicomSequenceItem};
		}

		public bool HasFunctionalGroup<T>()
			where T : FunctionalGroupMacro, new()
		{
			return GetFunctionalGroup<T>().HasValues();
		}

		#region Implementation of IDicomAttributeProvider

		DicomAttribute IDicomAttributeProvider.this[DicomTag tag]
		{
			get { return DicomAttributeProvider[tag]; }
			set { DicomAttributeProvider[tag] = value; }
		}

		DicomAttribute IDicomAttributeProvider.this[uint tag]
		{
			get { return DicomAttributeProvider[tag]; }
			set { DicomAttributeProvider[tag] = value; }
		}

		public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			return DicomAttributeProvider.TryGetAttribute(tag, out attribute);
		}

		public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			return DicomAttributeProvider.TryGetAttribute(tag, out attribute);
		}

		#endregion
	}
}