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
	/// X-Ray Object Thickness Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.7 (Table C.8.19.6-7)</remarks>
	public class XRayObjectThicknessFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XRayObjectThicknessFunctionalGroup"/> class.
		/// </summary>
		public XRayObjectThicknessFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="XRayObjectThicknessFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public XRayObjectThicknessFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.ObjectThicknessSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.CalculatedAnatomyThickness; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ObjectThicknessSequence in the underlying collection. Type 1.
		/// </summary>
		public ObjectThicknessSequenceItem ObjectThicknessSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ObjectThicknessSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new ObjectThicknessSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.ObjectThicknessSequence];
				if (value == null)
				{
					const string msg = "ObjectThicknessSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ObjectThicknessSequence in the underlying collection. Type 1.
		/// </summary>
		public ObjectThicknessSequenceItem CreateObjectThicknessSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.ObjectThicknessSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new ObjectThicknessSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new ObjectThicknessSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Object Thickness Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.19.6.7 (Table C.8.19.6-7)</remarks>
	public class ObjectThicknessSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectThicknessSequenceItem"/> class.
		/// </summary>
		public ObjectThicknessSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectThicknessSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public ObjectThicknessSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of CalculatedAnatomyThickness in the underlying collection. Type 1.
		/// </summary>
		public double CalculatedAnatomyThickness
		{
			get { return DicomAttributeProvider[DicomTags.CalculatedAnatomyThickness].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.CalculatedAnatomyThickness].SetFloat64(0, value); }
		}
	}
}