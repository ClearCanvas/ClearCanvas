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
	/// Temporal Position Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.23 (Table C.7.6.16.2.23-1)</remarks>
	public class TemporalPositionFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemporalPositionFunctionalGroup"/> class.
		/// </summary>
		public TemporalPositionFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TemporalPositionFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public TemporalPositionFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.TemporalPositionSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.TemporalPositionTimeOffset; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of TemporalPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public TemporalPositionSequenceItem TemporalPositionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.TemporalPositionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new TemporalPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.TemporalPositionSequence];
				if (value == null)
				{
					const string msg = "TemporalPositionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the TemporalPositionSequence in the underlying collection. Type 1.
		/// </summary>
		public TemporalPositionSequenceItem CreateTemporalPositionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.TemporalPositionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new TemporalPositionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new TemporalPositionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Temporal Position Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.23 (Table C.7.6.16.2.23-1)</remarks>
	public class TemporalPositionSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemporalPositionSequenceItem"/> class.
		/// </summary>
		public TemporalPositionSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TemporalPositionSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public TemporalPositionSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of TemporalPositionTimeOffset in the underlying collection. Type 1.
		/// </summary>
		public double TemporalPositionTimeOffset
		{
			get { return DicomAttributeProvider[DicomTags.TemporalPositionTimeOffset].GetFloat64(0, 0); }
			set { DicomAttributeProvider[DicomTags.TemporalPositionTimeOffset].SetFloat64(0, value); }
		}
	}
}