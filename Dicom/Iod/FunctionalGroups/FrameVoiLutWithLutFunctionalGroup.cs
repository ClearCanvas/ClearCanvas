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
	/// Frame VOI LUT With LUT Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.10b (Table C.7.6.16-11b)</remarks>
	public class FrameVoiLutWithLutFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutWithLutFunctionalGroup"/> class.
		/// </summary>
		public FrameVoiLutWithLutFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutWithLutFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameVoiLutWithLutFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FrameVoiLutSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.VoiLutSequence;
				yield return DicomTags.WindowCenter;
				yield return DicomTags.WindowWidth;
				yield return DicomTags.WindowCenterWidthExplanation;
				yield return DicomTags.VoiLutFunction;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FrameVoiLutSequence in the underlying collection. Type 1.
		/// </summary>
		public IVoiLutMacro FrameVoiLutSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new VoiLutMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
				if (value == null)
				{
					const string msg = "FrameVoiLutSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				else if (!(value is IIodMacro))
				{
					const string msg = "FrameVoiLutSequence value must implement IIodMacro.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {((IIodMacro) value).DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FrameVoiLutSequence in the underlying collection. Type 1.
		/// </summary>
		public IVoiLutMacro CreateFrameVoiLutSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new VoiLutMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new VoiLutMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}