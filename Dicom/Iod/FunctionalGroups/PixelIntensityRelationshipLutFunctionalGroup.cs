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
	/// PixelIntensityRelationshipLut Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.13 (Table C.7.6.16-14)</remarks>
	public class PixelIntensityRelationshipLutFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelIntensityRelationshipLutFunctionalGroup"/> class.
		/// </summary>
		public PixelIntensityRelationshipLutFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelIntensityRelationshipLutFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelIntensityRelationshipLutFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PixelIntensityRelationshipLutSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.LutDescriptor;
				yield return DicomTags.LutData;
				yield return DicomTags.LutFunction;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PixelIntensityRelationshipLutSequence in the underlying collection. Type 1.
		/// </summary>
		public PixelIntensityRelationshipLutSequenceItem[] PixelIntensityRelationshipLutSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PixelIntensityRelationshipLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new PixelIntensityRelationshipLutSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PixelIntensityRelationshipLutSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "PixelIntensityRelationshipLutSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.PixelIntensityRelationshipLutSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PixelIntensityRelationshipLutSequence item. Does not modify the PixelIntensityRelationshipLutSequence in the underlying collection.
		/// </summary>
		public PixelIntensityRelationshipLutSequenceItem CreatePixelIntensityRelationshipLutSequenceItem()
		{
			var iodBase = new PixelIntensityRelationshipLutSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// PixelIntensityRelationshipLut Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.13 (Table C.7.6.16-14)</remarks>
	public class PixelIntensityRelationshipLutSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelIntensityRelationshipLutSequenceItem"/> class.
		/// </summary>
		public PixelIntensityRelationshipLutSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelIntensityRelationshipLutSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelIntensityRelationshipLutSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of LutDescriptor in the underlying collection. Type 1.
		/// </summary>
		public int[] LutDescriptor
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.LutDescriptor];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new int[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "LutDescriptor is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.LutDescriptor];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetInt32(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of LutData in the underlying collection. Type 1.
		/// </summary>
		public byte[] LutData
		{
			get
			{
				var attribute = DicomAttributeProvider[DicomTags.LutData];
				if (attribute.IsNull || attribute.IsEmpty)
					return null;
				return (byte[]) attribute.Values;
			}
			set
			{
				if (value == null)
				{
					const string msg = "LutData is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.LutData].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of LutFunction in the underlying collection. Type 1.
		/// </summary>
		public string LutFunction
		{
			get { return DicomAttributeProvider[DicomTags.LutFunction].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "LutFunction is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.LutFunction].SetString(0, value);
			}
		}
	}
}