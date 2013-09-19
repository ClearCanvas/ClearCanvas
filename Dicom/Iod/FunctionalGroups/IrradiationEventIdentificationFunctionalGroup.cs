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
	/// Irradiation Event Identification Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.18 (Table C.7.6.16-19)</remarks>
	public class IrradiationEventIdentificationFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IrradiationEventIdentificationFunctionalGroup"/> class.
		/// </summary>
		public IrradiationEventIdentificationFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IrradiationEventIdentificationFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IrradiationEventIdentificationFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IrradiationEventIdentificationSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.IrradiationEventUid; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of IrradiationEventIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public IrradiationEventIdentificationSequenceItem IrradiationEventIdentificationSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IrradiationEventIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new IrradiationEventIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.IrradiationEventIdentificationSequence];
				if (value == null)
				{
					const string msg = "IrradiationEventIdentificationSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the IrradiationEventIdentificationSequence in the underlying collection. Type 1.
		/// </summary>
		public IrradiationEventIdentificationSequenceItem CreateIrradiationEventIdentificationSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.IrradiationEventIdentificationSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new IrradiationEventIdentificationSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new IrradiationEventIdentificationSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Irradiation Event Identification Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.18 (Table C.7.6.16-19)</remarks>
	public class IrradiationEventIdentificationSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IrradiationEventIdentificationSequenceItem"/> class.
		/// </summary>
		public IrradiationEventIdentificationSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IrradiationEventIdentificationSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public IrradiationEventIdentificationSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of IrradiationEventUid in the underlying collection. Type 1.
		/// </summary>
		public string IrradiationEventUid
		{
			get { return DicomAttributeProvider[DicomTags.IrradiationEventUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "IrradiationEventUid is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.IrradiationEventUid].SetString(0, value);
			}
		}
	}
}