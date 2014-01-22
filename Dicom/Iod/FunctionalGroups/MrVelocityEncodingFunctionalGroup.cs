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
	/// MR Velocity Encoding Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.13 (Table C.8-100)</remarks>
	public class MrVelocityEncodingFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrVelocityEncodingFunctionalGroup"/> class.
		/// </summary>
		public MrVelocityEncodingFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrVelocityEncodingFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrVelocityEncodingFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrVelocityEncodingSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.VelocityEncodingDirection;
				yield return DicomTags.VelocityEncodingMinimumValue;
				yield return DicomTags.VelocityEncodingMaximumValue;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrVelocityEncodingSequence in the underlying collection. Type 1.
		/// </summary>
		public MrVelocityEncodingSequenceItem[] MrVelocityEncodingSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrVelocityEncodingSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new MrVelocityEncodingSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new MrVelocityEncodingSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "MrVelocityEncodingSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.MrVelocityEncodingSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a MrVelocityEncodingSequence item. Does not modify the MrVelocityEncodingSequence in the underlying collection.
		/// </summary>
		public MrVelocityEncodingSequenceItem CreateMrVelocityEncodingSequenceItem()
		{
			var iodBase = new MrVelocityEncodingSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// MR Velocity Encoding Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.13 (Table C.8-100)</remarks>
	public class MrVelocityEncodingSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrVelocityEncodingSequenceItem"/> class.
		/// </summary>
		public MrVelocityEncodingSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrVelocityEncodingSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrVelocityEncodingSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}