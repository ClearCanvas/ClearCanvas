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
	/// MR Timing and Related Parameters Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.2 (Table C.8-89)</remarks>
	public class MrTimingAndRelatedParametersFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrTimingAndRelatedParametersFunctionalGroup"/> class.
		/// </summary>
		public MrTimingAndRelatedParametersFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrTimingAndRelatedParametersFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrTimingAndRelatedParametersFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrTimingAndRelatedParametersSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.RepetitionTime;
				yield return DicomTags.FlipAngle;
				yield return DicomTags.EchoTrainLength;
				yield return DicomTags.RfEchoTrainLength;
				yield return DicomTags.GradientEchoTrainLength;
				yield return DicomTags.SpecificAbsorptionRateSequence;
				yield return DicomTags.GradientOutputType;
				yield return DicomTags.OperatingModeSequence;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrTimingAndRelatedParametersSequence in the underlying collection. Type 1.
		/// </summary>
		public MrTimingAndRelatedParametersSequenceItem MrTimingAndRelatedParametersSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrTimingAndRelatedParametersSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrTimingAndRelatedParametersSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrTimingAndRelatedParametersSequence];
				if (value == null)
				{
					const string msg = "MrTimingAndRelatedParametersSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrTimingAndRelatedParametersSequence in the underlying collection. Type 1.
		/// </summary>
		public MrTimingAndRelatedParametersSequenceItem CreateMrTimingAndRelatedParametersSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrTimingAndRelatedParametersSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrTimingAndRelatedParametersSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrTimingAndRelatedParametersSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Timing and Related Parameters Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.2 (Table C.8-89)</remarks>
	public class MrTimingAndRelatedParametersSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrTimingAndRelatedParametersSequenceItem"/> class.
		/// </summary>
		public MrTimingAndRelatedParametersSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrTimingAndRelatedParametersSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrTimingAndRelatedParametersSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		// TODO: implement the functional group sequence items
	}
}