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
	/// MR Metabolite Map Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.12 (Table C.8-99)</remarks>
	public class MrMetaboliteMapFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrMetaboliteMapFunctionalGroup"/> class.
		/// </summary>
		public MrMetaboliteMapFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrMetaboliteMapFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrMetaboliteMapFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrMetaboliteMapSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.MetaboliteMapDescription;
				yield return DicomTags.MetaboliteMapCodeSequence;
				yield return DicomTags.ChemicalShiftSequence;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrMetaboliteMapSequence in the underlying collection. Type 1.
		/// </summary>
		public MrMetaboliteMapSequenceItem MrMetaboliteMapSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrMetaboliteMapSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrMetaboliteMapSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrMetaboliteMapSequence];
				if (value == null)
				{
					const string msg = "MrMetaboliteMapSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrMetaboliteMapSequence in the underlying collection. Type 1.
		/// </summary>
		public MrMetaboliteMapSequenceItem CreateMrMetaboliteMapSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrMetaboliteMapSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrMetaboliteMapSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrMetaboliteMapSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Metabolite Map Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.12 (Table C.8-99)</remarks>
	public class MrMetaboliteMapSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrMetaboliteMapSequenceItem"/> class.
		/// </summary>
		public MrMetaboliteMapSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrMetaboliteMapSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrMetaboliteMapSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}