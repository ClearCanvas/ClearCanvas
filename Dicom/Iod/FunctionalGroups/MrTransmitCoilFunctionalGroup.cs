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
	/// MR Transmit Coil Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.8 (Table C.8-95)</remarks>
	public class MrTransmitCoilFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrTransmitCoilFunctionalGroup"/> class.
		/// </summary>
		public MrTransmitCoilFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrTransmitCoilFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrTransmitCoilFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrTransmitCoilSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.TransmitCoilName;
				yield return DicomTags.TransmitCoilManufacturerName;
				yield return DicomTags.TransmitCoilType;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrTransmitCoilSequence in the underlying collection. Type 1.
		/// </summary>
		public MrTransmitCoilSequenceItem MrTransmitCoilSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrTransmitCoilSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrTransmitCoilSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrTransmitCoilSequence];
				if (value == null)
				{
					const string msg = "MrTransmitCoilSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrTransmitCoilSequence in the underlying collection. Type 1.
		/// </summary>
		public MrTransmitCoilSequenceItem CreateMrTransmitCoilSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrTransmitCoilSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrTransmitCoilSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrTransmitCoilSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Transmit Coil Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.8 (Table C.8-95)</remarks>
	public class MrTransmitCoilSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrTransmitCoilSequenceItem"/> class.
		/// </summary>
		public MrTransmitCoilSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrTransmitCoilSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrTransmitCoilSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}