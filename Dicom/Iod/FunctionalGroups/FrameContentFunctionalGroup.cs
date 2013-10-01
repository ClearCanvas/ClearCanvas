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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.FunctionalGroups
{
	/// <summary>
	/// Frame Content Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.2 (Table C.7.6.16-3)</remarks>
	public class FrameContentFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameContentFunctionalGroup"/> class.
		/// </summary>
		public FrameContentFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameContentFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameContentFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.FrameContentSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.FrameAcquisitionNumber;
				yield return DicomTags.FrameReferenceDatetime;
				yield return DicomTags.FrameAcquisitionDatetime;
				yield return DicomTags.FrameAcquisitionDuration;
				yield return DicomTags.CardiacCyclePosition;
				yield return DicomTags.RespiratoryCyclePosition;
				yield return DicomTags.DimensionIndexValues;
				yield return DicomTags.TemporalPositionIndex;
				yield return DicomTags.StackId;
				yield return DicomTags.InStackPositionNumber;
				yield return DicomTags.FrameComments;
				yield return DicomTags.FrameLabel;
			}
		}

		public override void InitializeAttributes()
		{
			FrameContentSequence = null;
		}

		/// <summary>
		/// Gets or sets the value of FrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public FrameContentSequenceItem FrameContentSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameContentSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new FrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.FrameContentSequence];
				if (value == null)
				{
					const string msg = "FrameContentSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the FrameContentSequence in the underlying collection. Type 1.
		/// </summary>
		public FrameContentSequenceItem CreateFrameContentSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.FrameContentSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new FrameContentSequenceItem(dicomSequenceItem);
				return sequenceType;
			}
			return new FrameContentSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}

	/// <summary>
	/// Frame Content Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.2 (Table C.7.6.16-3)</remarks>
	public class FrameContentSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameContentSequenceItem"/> class.
		/// </summary>
		public FrameContentSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FrameContentSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public FrameContentSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of FrameAcquisitionNumber in the underlying collection. Type 3.
		/// </summary>
		public int? FrameAcquisitionNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.FrameAcquisitionNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameAcquisitionNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameAcquisitionNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameReferenceDate and FrameReferenceTime in the underlying collection.  Type 1C.
		/// </summary>
		public DateTime? FrameReferenceDateTime
		{
			get
			{
				var datetime = DicomAttributeProvider[DicomTags.FrameReferenceDatetime].GetString(0, string.Empty);
				return DateTimeParser.Parse(datetime);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameReferenceDatetime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameReferenceDatetime].SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameAcquisitionDate and FrameAcquisitionTime in the underlying collection.  Type 1C.
		/// </summary>
		public DateTime? FrameAcquisitionDateTime
		{
			get
			{
				var datetime = DicomAttributeProvider[DicomTags.FrameAcquisitionDatetime].GetString(0, string.Empty);
				return DateTimeParser.Parse(datetime);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameAcquisitionDatetime] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameAcquisitionDatetime].SetStringValue(DateTimeParser.ToDicomString(value.Value, false));
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameAcquisitionDuration in the underlying collection. Type 1C.
		/// </summary>
		public double? FrameAcquisitionDuration
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.FrameAcquisitionDuration].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.FrameAcquisitionDuration] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameAcquisitionDuration].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of CardiacCyclePosition in the underlying collection. Type 3.
		/// </summary>
		public string CardiacCyclePosition
		{
			get { return DicomAttributeProvider[DicomTags.CardiacCyclePosition].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.CardiacCyclePosition] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.CardiacCyclePosition].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RespiratoryCyclePosition in the underlying collection. Type 3.
		/// </summary>
		public string RespiratoryCyclePosition
		{
			get { return DicomAttributeProvider[DicomTags.RespiratoryCyclePosition].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.RespiratoryCyclePosition] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RespiratoryCyclePosition].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DimensionIndexValues in the underlying collection. Type 1C.
		/// </summary>
		public uint[] DimensionIndexValues
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DimensionIndexValues];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new uint[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetUInt32(n, 0);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.DimensionIndexValues] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.DimensionIndexValues];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetUInt32(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of TemporalPositionIndex in the underlying collection. Type 1C.
		/// </summary>
		public int? TemporalPositionIndex
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.TemporalPositionIndex].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.TemporalPositionIndex] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.TemporalPositionIndex].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StackId in the underlying collection. Type 1C.
		/// </summary>
		public string StackId
		{
			get { return DicomAttributeProvider[DicomTags.StackId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.StackId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StackId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InStackPositionNumber in the underlying collection. Type 1C.
		/// </summary>
		public int? InStackPositionNumber
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.InStackPositionNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InStackPositionNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InStackPositionNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameComments in the underlying collection. Type 3.
		/// </summary>
		public string FrameComments
		{
			get { return DicomAttributeProvider[DicomTags.FrameComments].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.FrameComments] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameComments].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of FrameLabel in the underlying collection. Type 3.
		/// </summary>
		public string FrameLabel
		{
			get { return DicomAttributeProvider[DicomTags.FrameLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.FrameLabel] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.FrameLabel].SetString(0, value);
			}
		}
	}
}