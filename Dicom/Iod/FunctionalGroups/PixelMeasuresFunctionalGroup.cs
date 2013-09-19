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
	/// Pixel Measures Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.1 (Table C.7.6.16-2)</remarks>
	public class PixelMeasuresFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelMeasuresFunctionalGroup"/> class.
		/// </summary>
		public PixelMeasuresFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelMeasuresFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelMeasuresFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.PixelMeasuresSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.PixelSpacing;
				yield return DicomTags.SliceThickness;
			}
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of PixelMeasuresSequence in the underlying collection. Type 1.
		/// </summary>
		public PixelMeasuresSequenceItem PixelMeasuresSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PixelMeasuresSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new PixelMeasuresSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.PixelMeasuresSequence];
				if (value == null)
				{
					const string msg = "PixelMeasuresSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the PixelMeasuresSequence in the underlying collection. Type 1.
		/// </summary>
		public PixelMeasuresSequenceItem CreatePixelMeasuresSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.PixelMeasuresSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new PixelMeasuresSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new PixelMeasuresSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Pixel Measures Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.1 (Table C.7.6.16-2)</remarks>
	public class PixelMeasuresSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PixelMeasuresSequenceItem"/> class.
		/// </summary>
		public PixelMeasuresSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PixelMeasuresSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public PixelMeasuresSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of PixelSpacing in the underlying collection. Type 1C.
		/// </summary>
		public double[] PixelSpacing
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.PixelSpacing].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.PixelSpacing].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.PixelSpacing] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of SliceThickness in the underlying collection. Type 1C.
		/// </summary>
		public double? SliceThickness
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SliceThickness].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SliceThickness] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SliceThickness].SetFloat64(0, value.Value);
			}
		}
	}
}