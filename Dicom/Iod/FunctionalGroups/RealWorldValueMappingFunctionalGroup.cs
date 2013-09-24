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
	/// Real World Value Mapping Functional Group Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.11 (Table C.7.6.16-12)</remarks>
	public class RealWorldValueMappingFunctionalGroup : FunctionalGroupMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RealWorldValueMappingFunctionalGroup"/> class.
		/// </summary>
		public RealWorldValueMappingFunctionalGroup() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RealWorldValueMappingFunctionalGroup"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RealWorldValueMappingFunctionalGroup(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		public override IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.RealWorldValueMappingSequence; }
		}

		public override IEnumerable<uint> NestedTags
		{
			get
			{
				yield return DicomTags.RealWorldValueFirstValueMapped;
				yield return DicomTags.RealWorldValueLastValueMapped;
				yield return DicomTags.RealWorldValueIntercept;
				yield return DicomTags.RealWorldValueSlope;
				yield return DicomTags.RealWorldValueLutData;
				yield return DicomTags.LutExplanation;
				yield return DicomTags.LutLabel;
				yield return DicomTags.MeasurementUnitsCodeSequence;
			}
		}

		public override bool CanHaveMultipleItems
		{
			get { return true; }
		}

		public override void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of RealWorldValueMappingSequence in the underlying collection. Type 1.
		/// </summary>
		public RealWorldValueMappingSequenceItem[] RealWorldValueMappingSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RealWorldValueMappingSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new RealWorldValueMappingSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RealWorldValueMappingSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "RealWorldValueMappingSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.RealWorldValueMappingSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a RealWorldValueMappingSequence item. Does not modify the RealWorldValueMappingSequence in the underlying collection.
		/// </summary>
		public RealWorldValueMappingSequenceItem CreateRealWorldValueMappingSequenceItem()
		{
			var iodBase = new RealWorldValueMappingSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}

	/// <summary>
	/// Real World Value Mapping Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.16.2.11 (Table C.7.6.16-12)</remarks>
	public class RealWorldValueMappingSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RealWorldValueMappingSequenceItem"/> class.
		/// </summary>
		public RealWorldValueMappingSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="RealWorldValueMappingSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public RealWorldValueMappingSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of RealWorldValueFristValueMapped in the underlying collection. Type 1.
		/// </summary>
		public int RealWorldValueFirstValueMapped
		{
			get { return DicomAttributeProvider[DicomTags.RealWorldValueFirstValueMapped].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.RealWorldValueFirstValueMapped].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RealWorldValueLastValueMapped in the underlying collection. Type 1.
		/// </summary>
		public int RealWorldValueLastValueMapped
		{
			get { return DicomAttributeProvider[DicomTags.RealWorldValueLastValueMapped].GetInt32(0, 0); }
			set { DicomAttributeProvider[DicomTags.RealWorldValueLastValueMapped].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of RealWorldValueIntercept in the underlying collection. Type 1C.
		/// </summary>
		public double? RealWorldValueIntercept
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RealWorldValueIntercept].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RealWorldValueIntercept] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RealWorldValueIntercept].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RealWorldValueSlope in the underlying collection. Type 1C.
		/// </summary>
		public double? RealWorldValueSlope
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.RealWorldValueSlope].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.RealWorldValueSlope] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.RealWorldValueSlope].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RealWorldValueLutData in the underlying collection. Type 1C.
		/// </summary>
		public double[] RealWorldValueLutData
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.RealWorldValueLutData];
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
					DicomAttributeProvider[DicomTags.RealWorldValueLutData] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.RealWorldValueLutData];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of LutExplanation in the underlying collection. Type 1.
		/// </summary>
		public string LutExplanation
		{
			get { return DicomAttributeProvider[DicomTags.LutExplanation].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "LutExplanation is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.LutExplanation].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of LutLabel in the underlying collection. Type 1.
		/// </summary>
		public string LutLabel
		{
			get { return DicomAttributeProvider[DicomTags.LutLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "LutLabel is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.LutLabel].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of MeasurementUnitsCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro MeasurementUnitsCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MeasurementUnitsCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.MeasurementUnitsCodeSequence];
				if (value == null)
				{
					const string msg = "MeasurementUnitsCodeSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				dicomAttribute.Values = new[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the MeasurementUnitsCodeSequence in the underlying collection. Type 1.
		/// </summary>
		public CodeSequenceMacro CreateMeasurementUnitsCodeSequence()
		{
			var dicomAttribute = DicomAttributeProvider[DicomTags.MeasurementUnitsCodeSequence];
			if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
			{
				var dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new[] {dicomSequenceItem};
				var sequenceType = new CodeSequenceMacro(dicomSequenceItem);
				return sequenceType;
			}
			return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}
	}
}