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
using ClearCanvas.Dicom.Iod.Macros.VoiLut;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// VOI LUT Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
	public interface IVoiLutMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of VoiLutSequence in the underlying collection. Type 1C.
		/// </summary>
		VoiLutSequenceItem[] VoiLutSequence { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowCenter in the underlying collection. Type 1C.
		/// </summary>
		double[] WindowCenter { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowWidth in the underlying collection. Type 1C.
		/// </summary>
		double[] WindowWidth { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowCenterWidthExplanation in the underlying collection. Type 3.
		/// </summary>
		string[] WindowCenterWidthExplanation { get; set; }

		/// <summary>
		/// Gets or sets the value of VoiLutFunction in the underlying collection. Type 3.
		/// </summary>
		VoiLutFunction VoiLutFunction { get; set; }

		/// <summary>
		/// Gets the number of VOI Data LUTs included in this sequence.
		/// </summary>
		long CountDataLuts { get; }

		/// <summary>
		/// Gets the number of VOI Windows included in this sequence.
		/// </summary>
		long CountWindows { get; }
	}

	/// <summary>
	/// VOI LUT Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
	internal class VoiLutMacro : SequenceIodBase, IVoiLutMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutMacro"/> class.
		/// </summary>
		public VoiLutMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public VoiLutMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		public void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of VoiLutSequence in the underlying collection. Type 1C.
		/// </summary>
		public VoiLutSequenceItem[] VoiLutSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.VoiLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new VoiLutSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new VoiLutSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.VoiLutSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.VoiLutSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a VoiLutSequence item. Does not modify the VoiLutSequence in the underlying collection.
		/// </summary>
		public VoiLutSequenceItem CreateVoiLutSequenceItem()
		{
			var iodBase = new VoiLutSequenceItem(new DicomSequenceItem());
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of WindowCenter in the underlying collection. Type 1C.
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
					DicomAttributeProvider[DicomTags.WindowCenter] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowCenter];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowWidth in the underlying collection. Type 1C.
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
					DicomAttributeProvider[DicomTags.WindowWidth] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowWidth];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetFloat64(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowCenterWidthExplanation in the underlying collection. Type 3.
		/// </summary>
		public string[] WindowCenterWidthExplanation
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var values = new string[dicomAttribute.Count];
				for (int n = 0; n < values.Length; n++)
					values[n] = dicomAttribute.GetString(n, string.Empty);
				return values;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.WindowCenterWidthExplanation];
				for (int n = 0; n < value.Length; n++)
					dicomAttribute.SetString(n, value[n] ?? string.Empty);
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

		/// <summary>
		/// Gets the number of VOI Data LUTs included in this sequence.
		/// </summary>
		public long CountDataLuts
		{
			get
			{
				DicomAttribute attribute = DicomAttributeProvider[DicomTags.VoiLutSequence];
				if (attribute.IsNull || attribute.IsEmpty)
					return 0;
				return attribute.Count;
			}
		}

		/// <summary>
		/// Gets the number of VOI Windows included in this sequence.
		/// </summary>
		public long CountWindows
		{
			get
			{
				DicomAttribute attribute = DicomAttributeProvider[DicomTags.WindowCenter];
				if (attribute.IsNull || attribute.IsEmpty)
					return 0;
				return attribute.Count;
			}
		}
	}

	namespace VoiLut
	{
		/// <summary>
		/// VOI LUT Sequence Item
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
		public class VoiLutSequenceItem : SequenceIodBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="VoiLutSequenceItem"/> class.
			/// </summary>
			public VoiLutSequenceItem() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="VoiLutSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public VoiLutSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Gets or sets the value of LutDescriptor in the underlying collection. Type 1.
			/// </summary>
			public int[] LutDescriptor
			{
				get
				{
					var result = new int[3];
					if (DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(0, out result[0])
					    && DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(1, out result[1])
					    && DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(2, out result[2]))
						return result;
					return null;
				}
				set
				{
					if (value == null || value.Length != 3)
					{
						const string msg = "LutDescriptor is Type 1 Required.";
						throw new ArgumentNullException("value", msg);
					}
					DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(0, value[0]);
					DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(1, value[1]);
					DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(2, value[2]);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutExplanation in the underlying collection. Type 3.
			/// </summary>
			public string LutExplanation
			{
				get { return DicomAttributeProvider[DicomTags.LutExplanation].GetString(0, string.Empty); }
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						DicomAttributeProvider[DicomTags.LutExplanation] = null;
						return;
					}
					DicomAttributeProvider[DicomTags.LutExplanation].SetString(0, value);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutData in the underlying collection. Type 1.
			/// </summary>
			public ushort[] LutData
			{
				get
				{
					DicomAttribute attribute = DicomAttributeProvider[DicomTags.LutData];
					if (attribute.IsNull || attribute.IsEmpty || attribute.Count == 0)
						return null;
					return (ushort[]) attribute.Values;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						const string msg = "LutData is Type 1 Required.";
						throw new ArgumentNullException("value", msg);
					}
					DicomAttributeProvider[DicomTags.LutData].Values = value;
				}
			}
		}

		/// <summary>
		/// Enumerated values for the <see cref="DicomTags.VoiLutFunction"/> attribute describing
		/// a VOI LUT function to apply to the <see cref="IVoiLutMacro.WindowCenter"/> and <see cref="IVoiLutMacro.WindowWidth"/>.
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
		public enum VoiLutFunction
		{
			/// <summary>
			/// Specifies a linear VOI LUT function.
			/// </summary>
			Linear,

			/// <summary>
			/// Specifies a sigmoid VOI LUT function.
			/// </summary>
			Sigmoid,

			/// <summary>
			/// Represents the null value, which is equivalent to the unknown status.
			/// </summary>
			None
		}
	}
}