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
	/// MR Echo Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.4 (Table C.8-91)</remarks>
	public class MrEchoFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrEchoFunctionalGroup"/> class.
		/// </summary>
		public MrEchoFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrEchoFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrEchoFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.MrEchoSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get { yield return DicomTags.EffectiveEchoTime; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of MrEchoSequence in the underlying collection. Type 1.
		/// </summary>
		public MrEchoSequenceItem MrEchoSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrEchoSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new MrEchoSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MrEchoSequence];
				if (value == null)
				{
					const string msg = "MrEchoSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MrEchoSequence in the underlying collection. Type 1.
		/// </summary>
		public MrEchoSequenceItem CreateMrEchoSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MrEchoSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new MrEchoSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new MrEchoSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// MR Echo Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.13.5.4 (Table C.8-91)</remarks>
	public class MrEchoSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MrEchoSequenceItem"/> class.
		/// </summary>
		public MrEchoSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MrEchoSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public MrEchoSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of EffectiveEchoTime in the underlying collection. Type 1C.
		/// </summary>
		public double? EffectiveEchoTime
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.EffectiveEchoTime].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.EffectiveEchoTime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.EffectiveEchoTime].SetFloat64(0, value.Value);
			}
		}
	}
}