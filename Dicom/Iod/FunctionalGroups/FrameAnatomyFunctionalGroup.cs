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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Frame Anatomy Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.8 (Table C.7.6.16-9)</remarks>
	public class FrameAnatomyFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameAnatomyFunctionalGroup"/> class.
		/// </summary>
		public FrameAnatomyFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameAnatomyFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameAnatomyFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FrameAnatomySequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameLaterality;
				yield return DicomTags.AnatomicRegionSequence;
				yield return DicomTags.PrimaryAnatomicStructureSequence;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FrameAnatomySequence in the underlying collection. Type 1.
		/// </summary>
		public FrameAnatomySequenceItem FrameAnatomySequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameAnatomySequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FrameAnatomySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameAnatomySequence];
				if (value == null)
				{
					const string msg = "FrameAnatomySequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FrameAnatomySequence in the underlying collection. Type 1.
		/// </summary>
		public FrameAnatomySequenceItem CreateFrameAnatomySequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FrameAnatomySequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FrameAnatomySequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FrameAnatomySequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Frame Anatomy Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.8 (Table C.7.6.16-9)</remarks>
	public class FrameAnatomySequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameAnatomySequenceItem"/> class.
		/// </summary>
		public FrameAnatomySequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameAnatomySequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameAnatomySequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of FrameLaterality in the underlying collection. Type 1.
		/// </summary>
		public string FrameLaterality
		{
			get { return DicomAttributeProvider[DicomTags.FrameLaterality].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "FrameLaterality is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.FrameLaterality].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of AnatomicRegionSequence in the underlying collection. Type 1.
		/// </summary>
		public AnatomicRegionSequenceItem AnatomicRegionSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.AnatomicRegionSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new AnatomicRegionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.AnatomicRegionSequence];
				if (value == null)
				{
					const string msg = "AnatomicRegionSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the AnatomicRegionSequence in the underlying collection. Type 1.
		/// </summary>
		public AnatomicRegionSequenceItem CreateAnatomicRegionSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.AnatomicRegionSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new AnatomicRegionSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new AnatomicRegionSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of PrimaryAnatomicStructureSequence in the underlying collection. Type 3.
		/// </summary>
		public PrimaryAnatomicStructureSequenceItem[] PrimaryAnatomicStructureSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new PrimaryAnatomicStructureSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PrimaryAnatomicStructureSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.PrimaryAnatomicStructureSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PrimaryAnatomicStructureSequence item. Does not modify the PrimaryAnatomicStructureSequence in the underlying collection.
		/// </summary>
		public PrimaryAnatomicStructureSequenceItem CreatePrimaryAnatomicStructureSequenceItem()
		{
			var iodBase = new PrimaryAnatomicStructureSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}
}