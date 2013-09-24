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

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Primary Anatomic Structure Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section 10.5 (Table 10-8)</remarks>
	public class PrimaryAnatomicStructureSequenceItem : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryAnatomicStructureSequenceItem"/> class.
		/// </summary>
		public PrimaryAnatomicStructureSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryAnatomicStructureSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PrimaryAnatomicStructureSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PrimaryAnatomicStructureModifierSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro[] PrimaryAnatomicStructureModifierSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureModifierSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new CodeSequenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CodeSequenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureModifierSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureModifierSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PrimaryAnatomicStructureModifierSequence item. Does not modify the PrimaryAnatomicStructureModifierSequence in the underlying collection.
		/// </summary>
		public CodeSequenceMacro CreatePrimaryAnatomicStructureModifierSequenceItem()
		{
			var iodBase = new CodeSequenceMacro(new DicomSequenceItem());
			return iodBase;
		}
	}
}