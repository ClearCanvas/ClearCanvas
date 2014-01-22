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
	/// Contrast/Bolus Usage Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.12 (Table C.7.6.16-13)</remarks>
	public class ContrastBolusUsageFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastBolusUsageFunctionalGroup"/> class.
		/// </summary>
		public ContrastBolusUsageFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastBolusUsageFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ContrastBolusUsageFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ContrastBolusUsageSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ContrastBolusAgentNumber;
				yield return DicomTags.ContrastBolusAgentAdministered;
				yield return DicomTags.ContrastBolusAgentDetected;
				yield return DicomTags.ContrastBolusAgentPhase;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ContrastBolusUsageSequence in the underlying collection. Type 1.
		/// </summary>
		public ContrastBolusUsageSequenceItem[] ContrastBolusUsageSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ContrastBolusUsageSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new ContrastBolusUsageSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ContrastBolusUsageSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "ContrastBolusUsageSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.ContrastBolusUsageSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ContrastBolusUsageSequence item. Does not modify the ContrastBolusUsageSequence in the underlying collection.
		/// </summary>
		public ContrastBolusUsageSequenceItem CreateContrastBolusUsageSequenceItem()
		{
			var iodBase = new ContrastBolusUsageSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Contrast/Bolus Usage Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.12 (Table C.7.6.16-13)</remarks>
	public class ContrastBolusUsageSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastBolusUsageSequenceItem"/> class.
		/// </summary>
		public ContrastBolusUsageSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContrastBolusUsageSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ContrastBolusUsageSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentNumber in the underlying collection. Type 1.
		/// </summary>
		public int ContrastBolusAgentNumber
		{
			get { return DicomAttributeProvider[DicomTags.ContrastBolusAgentNumber].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.ContrastBolusAgentNumber].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentAdministered in the underlying collection. Type 1.
		/// </summary>
		public ContrastBolusAgentAdministered ContrastBolusAgentAdministered
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.ContrastBolusAgentAdministered].GetString(0, string.Empty), ContrastBolusAgentAdministered.None); }
			set
			{
				if (value == ContrastBolusAgentAdministered.None)
				{
					const string msg = "ContrastBolusAgentAdministered is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.ContrastBolusAgentAdministered], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentDetected in the underlying collection. Type 2.
		/// </summary>
		public ContrastBolusAgentDetected ContrastBolusAgentDetected
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.ContrastBolusAgentDetected].GetString(0, string.Empty), ContrastBolusAgentDetected.None); }
			set
			{
				if (value == ContrastBolusAgentDetected.None)
				{
					DicomAttributeProvider[DicomTags.ContrastBolusAgentDetected].SetNullValue();
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.ContrastBolusAgentDetected], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentPhase in the underlying collection. Type 2C.
		/// </summary>
		public string ContrastBolusAgentPhase
		{
			get { return DicomAttributeProvider[DicomTags.ContrastBolusAgentPhase].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ContrastBolusAgentPhase] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ContrastBolusAgentPhase].SetString(0, value);
			}
		}
	}

	/// <summary>
	/// Enumerated values for the <see cref="ClearCanvas.Dicom.DicomTags.ContrastBolusAgentAdministered"/> attribute.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.12 (Table C.7.6.16-13)</remarks>
	public enum ContrastBolusAgentAdministered
	{
		/// <summary>
		/// Represents the null value.
		/// </summary>
		None,

		/// <summary>
		/// YES
		/// </summary>
		Yes,

		/// <summary>
		/// NO
		/// </summary>
		No
	}

	/// <summary>
	/// Enumerated values for the <see cref="ClearCanvas.Dicom.DicomTags.ContrastBolusAgentDetected"/> attribute.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.12 (Table C.7.6.16-13)</remarks>
	public enum ContrastBolusAgentDetected
	{
		/// <summary>
		/// Represents the null value.
		/// </summary>
		None,

		/// <summary>
		/// YES
		/// </summary>
		Yes,

		/// <summary>
		/// NO
		/// </summary>
		No
	}
}