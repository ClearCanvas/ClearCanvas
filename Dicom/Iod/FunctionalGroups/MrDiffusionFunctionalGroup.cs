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
	/// MR Diffusion Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.9 (Table C.8-96)</remarks>
	public class MrDiffusionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrDiffusionFunctionalGroup"/> class.
		/// </summary>
		public MrDiffusionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrDiffusionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrDiffusionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrDiffusionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.DiffusionBValue;
				yield return DicomTags.DiffusionDirectionality;
				yield return DicomTags.DiffusionGradientDirectionSequence;
				yield return DicomTags.DiffusionBMatrixSequence;
				yield return DicomTags.DiffusionAnisotropyType;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrDiffusionSequence in the underlying collection. Type 1.
		/// </summary>
		public MrDiffusionSequenceItem MrDiffusionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrDiffusionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrDiffusionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrDiffusionSequence];
				if (value == null)
				{
					const string msg = "MrDiffusionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrDiffusionSequence in the underlying collection. Type 1.
		/// </summary>
		public MrDiffusionSequenceItem CreateMrDiffusionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrDiffusionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrDiffusionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrDiffusionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Diffusion Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.9 (Table C.8-96)</remarks>
	public class MrDiffusionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrDiffusionSequenceItem"/> class.
		/// </summary>
		public MrDiffusionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrDiffusionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrDiffusionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}