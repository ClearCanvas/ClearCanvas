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
	/// CT Reconstruction Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.7 (Table C.8-123)</remarks>
	public class CtReconstructionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtReconstructionFunctionalGroup"/> class.
		/// </summary>
		public CtReconstructionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtReconstructionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtReconstructionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.CtReconstructionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.ReconstructionAlgorithm;
				yield return DicomTags.ConvolutionKernel;
				yield return DicomTags.ConvolutionKernelGroup;
				yield return DicomTags.ReconstructionDiameter;
				yield return DicomTags.ReconstructionFieldOfView;
				yield return DicomTags.ReconstructionPixelSpacing;
				yield return DicomTags.ReconstructionAngle;
				yield return DicomTags.ImageFilter;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of CtReconstructionSequence in the underlying collection. Type 1.
		/// </summary>
		public CtReconstructionSequenceItem CtReconstructionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtReconstructionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CtReconstructionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.CtReconstructionSequence];
				if (value == null)
				{
					const string msg = "CtReconstructionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the CtReconstructionSequence in the underlying collection. Type 1.
		/// </summary>
		public CtReconstructionSequenceItem CreateCtReconstructionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.CtReconstructionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CtReconstructionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new CtReconstructionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// CT Reconstruction Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.15.3.7 (Table C.8-123)</remarks>
	public class CtReconstructionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CtReconstructionSequenceItem"/> class.
		/// </summary>
		public CtReconstructionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="CtReconstructionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public CtReconstructionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}