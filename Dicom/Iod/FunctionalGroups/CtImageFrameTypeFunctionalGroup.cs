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
	/// CT Image Frame Type Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.1 (Table C.8-117)</remarks>
	public class CtImageFrameTypeFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtImageFrameTypeFunctionalGroup"/> class.
		/// </summary>
		public CtImageFrameTypeFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtImageFrameTypeFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtImageFrameTypeFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtImageFrameTypeSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameType;
				foreach (var t in CommonCtMrImageDescriptionMacro.DefinedTags) yield return t;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtImageFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public CtImageFrameTypeSequenceItem CtImageFrameTypeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtImageFrameTypeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtImageFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtImageFrameTypeSequence];
				if (value == null)
				{
					const string msg = "CtImageFrameTypeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtImageFrameTypeSequence in the underlying collection. Type 1.
		/// </summary>
		public CtImageFrameTypeSequenceItem CreateCtImageFrameTypeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtImageFrameTypeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtImageFrameTypeSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtImageFrameTypeSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Image Frame Type Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.1 (Table C.8-117)</remarks>
	public class CtImageFrameTypeSequenceItem : CommonCtMrImageDescriptionMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtImageFrameTypeSequenceItem"/> class.
		/// </summary>
		public CtImageFrameTypeSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtImageFrameTypeSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtImageFrameTypeSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of FrameType in the underlying collection. Type 1.
		/// </summary>
		public string FrameType
		{
			get { return DicomAttributeProvider[DicomTags.FrameType].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FrameType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FrameType].SetStringValue(value);
			}
		}
	}
}