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
	/// FramePixelShift Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.14 (Table C.7.6.16-15)</remarks>
	public class FramePixelShiftFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelShiftFunctionalGroup"/> class.
		/// </summary>
		public FramePixelShiftFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelShiftFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FramePixelShiftFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FramePixelShiftSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.SubtractionItemId;
				yield return DicomTags.MaskSubPixelShift;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of FramePixelShiftSequence in the underlying collection. Type 1.
		/// </summary>
		public FramePixelShiftSequenceItem[] FramePixelShiftSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FramePixelShiftSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new FramePixelShiftSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new FramePixelShiftSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "FramePixelShiftSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.FramePixelShiftSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a FramePixelShiftSequence item. Does not modify the FramePixelShiftSequence in the underlying collection.
		/// </summary>
		public FramePixelShiftSequenceItem CreateFramePixelShiftSequenceItem()
		{
			var iodBase = new FramePixelShiftSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Frame Pixel Shift Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.14 (Table C.7.6.16-15)</remarks>
	public class FramePixelShiftSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelShiftSequenceItem"/> class.
		/// </summary>
		public FramePixelShiftSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FramePixelShiftSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FramePixelShiftSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of SubtractionItemId in the underlying collection. Type 1.
		/// </summary>
		public int SubtractionItemId
		{
			get { return DicomAttributeProvider[DicomTags.SubtractionItemId].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.SubtractionItemId].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of MaskSubPixelShift in the underlying collection. Type 1.
		/// </summary>
		public double[] MaskSubPixelShift
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.MaskSubPixelShift].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.MaskSubPixelShift].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					const string msg = "MaskSubPixelShift is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.MaskSubPixelShift].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.MaskSubPixelShift].SetFloat64(1, value[1]);
			}
		}
	}
}