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
	/// MR Receive Coil Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.7 (Table C.8-94)</remarks>
	public class MrReceiveCoilFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrReceiveCoilFunctionalGroup"/> class.
		/// </summary>
		public MrReceiveCoilFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrReceiveCoilFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrReceiveCoilFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrReceiveCoilSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ReceiveCoilName;
				yield return DicomTags.ReceiveCoilManufacturerName;
				yield return DicomTags.ReceiveCoilType;
				yield return DicomTags.QuadratureReceiveCoil;
				yield return DicomTags.MultiCoilDefinitionSequence;
				yield return DicomTags.MultiCoilConfiguration;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrReceiveCoilSequence in the underlying collection. Type 1.
		/// </summary>
		public MrReceiveCoilSequenceItem MrReceiveCoilSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrReceiveCoilSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrReceiveCoilSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrReceiveCoilSequence];
				if (value == null)
				{
					const string msg = "MrReceiveCoilSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrReceiveCoilSequence in the underlying collection. Type 1.
		/// </summary>
		public MrReceiveCoilSequenceItem CreateMrReceiveCoilSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrReceiveCoilSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrReceiveCoilSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrReceiveCoilSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Receive Coil Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.7 (Table C.8-94)</remarks>
	public class MrReceiveCoilSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrReceiveCoilSequenceItem"/> class.
		/// </summary>
		public MrReceiveCoilSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrReceiveCoilSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrReceiveCoilSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}