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
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// X-Ray 3D Frame Type Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.5.1 (Table C.8.21.5.1-1)</remarks>
	public class XRay3DFrameTypeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DFrameTypeFunctionalGroup"/> class.
		/// </summary>
		public XRay3DFrameTypeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DFrameTypeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRay3DFrameTypeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.XRay3dFrameTypeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				foreach (var tag in CommonCtMrImageDescriptionMacro.DefinedTags) yield return tag;
				yield return DicomTags.ReconstructionIndex;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of XRay3DFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public XRay3DFrameTypeSequenceItem XRay3DFrameTypeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dFrameTypeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new XRay3DFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dFrameTypeSequence];
				if (value == null)
				{
					const string msg = "XRay3dFrameTypeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the XRay3DFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public XRay3DFrameTypeSequenceItem CreateXRay3DFrameTypeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.XRay3dFrameTypeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new XRay3DFrameTypeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new XRay3DFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// X-Ray 3D Frame Type Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.5.1 (Table C.8.21.5.1-1)</remarks>
	public class XRay3DFrameTypeSequenceItem : CommonCtMrImageDescriptionMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DFrameTypeSequenceItem"/> class.
		/// </summary>
		public XRay3DFrameTypeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRay3DFrameTypeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRay3DFrameTypeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}