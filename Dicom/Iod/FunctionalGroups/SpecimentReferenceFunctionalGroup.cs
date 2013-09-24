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

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Specimen Reference Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.3 (Table C.8.12.6.3-1)</remarks>
	public class SpecimenReferenceFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenReferenceFunctionalGroup"/> class.
		/// </summary>
		public SpecimenReferenceFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenReferenceFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public SpecimenReferenceFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.SpecimenReferenceSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.SpecimenUid; }
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of SpecimenReferenceSequence in the underlying collection. Type 2.
		/// </summary>
		public SpecimenReferenceSequenceItem[] SpecimenReferenceSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SpecimenReferenceSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new SpecimenReferenceSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new SpecimenReferenceSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.SpecimenReferenceSequence].SetNullValue();
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.SpecimenReferenceSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a SpecimenReferenceSequence item. Does not modify the SpecimenReferenceSequence in the underlying collection.
		/// </summary>
		public SpecimenReferenceSequenceItem CreateSpecimenReferenceSequenceItem()
		{
			var iodBase = new SpecimenReferenceSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Specimen Reference Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.12.6.3 (Table C.8.12.6.3-1)</remarks>
	public class SpecimenReferenceSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenReferenceSequenceItem"/> class.
		/// </summary>
		public SpecimenReferenceSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenReferenceSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public SpecimenReferenceSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of SpecimenUid in the underlying collection. Type 1.
		/// </summary>
		public string SpecimenUid
		{
			get { return DicomAttributeProvider[DicomTags.SpecimenUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "SpecimenUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.SpecimenUid].SetString(0, value);
			}
		}
	}
}