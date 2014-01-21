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
using ClearCanvas.Dicom.Iod.Macros.VoiLut;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Frame VOI LUT Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.10 (Table C.7.6.16-11)</remarks>
	public class FrameVoiLutFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutFunctionalGroup"/> class.
		/// </summary>
		public FrameVoiLutFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameVoiLutFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FrameVoiLutSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
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
		public FrameVoiLutSequenceItem FrameVoiLutSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FrameVoiLutSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
				if (value == null)
				{
					const string msg = "FrameVoiLutSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FrameVoiLutSequence in the underlying collection. Type 1.
		/// </summary>
		public FrameVoiLutSequenceItem CreateFrameVoiLutSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FrameVoiLutSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FrameVoiLutSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FrameVoiLutSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Frame VOI LUT Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.10 (Table C.7.6.16-11)</remarks>
	public class FrameVoiLutSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutSequenceItem"/> class.
		/// </summary>
		public FrameVoiLutSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameVoiLutSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameVoiLutSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of WindowCenter in the underlying collection. Type 1.
		/// </summary>
		public double[] WindowCenter
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowCenter];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "WindowCenter is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowCenter];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowWidth in the underlying collection. Type 1.
		/// </summary>
		public double[] WindowWidth
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowWidth];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new double[dicomAttribute.Count];
				for (var n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetFloat64(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "WindowWidth is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowWidth];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowCenterWidthExplanation in the underlying collection. Type 3.
		/// </summary>
		public string WindowCenterWidthExplanation
		{
			get { return DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of VoiLutFunction in the underlying collection. Type 3.
		/// </summary>
		public VoiLutFunction VoiLutFunction
		{
			get { return ParseEnum(DicomAttributeProvider[DicomTags.VoiLutFunction].GetString(0, string.Empty), VoiLutFunction.None); }
			set
			{
				if (value == VoiLutFunction.None)
				{
					DicomAttributeProvider[DicomTags.VoiLutFunction] = null;
					return;
				}
				SetAttributeFromEnum(DicomAttributeProvider[DicomTags.VoiLutFunction], value);
			}
		}
	}
}