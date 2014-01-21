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
	/// Radiopharmaceutical Usage Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.19 (Table C.7.6.16-20)</remarks>
	public class RadiopharmaceuticalUsageFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RadiopharmaceuticalUsageFunctionalGroup"/> class.
		/// </summary>
		public RadiopharmaceuticalUsageFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadiopharmaceuticalUsageFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RadiopharmaceuticalUsageFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.RadiopharmaceuticalUsageSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.RadiopharmaceuticalAgentNumber; }
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of RadiopharmaceuticalUsageSequence in the underlying collection. Type 1.
		/// </summary>
		public RadiopharmaceuticalUsageSequenceItem[] RadiopharmaceuticalUsageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RadiopharmaceuticalUsageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new RadiopharmaceuticalUsageSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RadiopharmaceuticalUsageSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "RadiopharmaceuticalUsageSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.RadiopharmaceuticalUsageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a RadiopharmaceuticalUsageSequence item. Does not modify the RadiopharmaceuticalUsageSequence in the underlying collection.
		/// </summary>
		public RadiopharmaceuticalUsageSequenceItem CreateRadiopharmaceuticalUsageSequenceItem()
		{
			var iodBase = new RadiopharmaceuticalUsageSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Radiopharmaceutical Usage Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.19 (Table C.7.6.16-20)</remarks>
	public class RadiopharmaceuticalUsageSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RadiopharmaceuticalUsageSequenceItem"/> class.
		/// </summary>
		public RadiopharmaceuticalUsageSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadiopharmaceuticalUsageSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RadiopharmaceuticalUsageSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of RadiopharmaceuticalAgentNumber in the underlying collection. Type 1.
		/// </summary>
		public int RadiopharmaceuticalAgentNumber
		{
			get { return DicomAttributeProvider[DicomTags.RadiopharmaceuticalAgentNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.RadiopharmaceuticalAgentNumber].SetInt32(0, value); }
		}
	}
}