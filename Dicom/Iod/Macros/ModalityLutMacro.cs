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

using ClearCanvas.Dicom.Iod.Macros.ModalityLut;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// ModalityLut Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.1 (Table C.11-1b)</remarks>
	public interface IModalityLutMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of ModalityLutSequence in the underlying collection. Type 1C.
		/// </summary>
		ModalityLutSequenceItem ModalityLutSequence { get; set; }

		/// <summary>
		/// Gets or sets the value of RescaleIntercept in the underlying collection. Type 1C.
		/// </summary>
		double? RescaleIntercept { get; set; }

		/// <summary>
		/// Gets or sets the value of RescaleSlope in the underlying collection. Type 1C.
		/// </summary>
		double? RescaleSlope { get; set; }

		/// <summary>
		/// Gets or sets the value of RescaleType in the underlying collection. Type 1C.
		/// </summary>
		string RescaleType { get; set; }
	}

	/// <summary>
	/// ModalityLut Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.1 (Table C.11-1b)</remarks>
	internal class ModalityLutMacro : SequenceIodBase, IModalityLutMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModalityLutMacro"/> class.
		/// </summary>
		public ModalityLutMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ModalityLutMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public ModalityLutMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of ModalityLutSequence in the underlying collection. Type 1C.
		/// </summary>
		public ModalityLutSequenceItem ModalityLutSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ModalityLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new ModalityLutSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ModalityLutSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ModalityLutSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleIntercept in the underlying collection. Type 1C.
		/// </summary>
		public double? RescaleIntercept
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[DicomTags.RescaleIntercept].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.RescaleIntercept] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RescaleIntercept].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleSlope in the underlying collection. Type 1C.
		/// </summary>
		public double? RescaleSlope
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[DicomTags.RescaleSlope].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.RescaleSlope] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RescaleSlope].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RescaleType in the underlying collection. Type 1C.
		/// </summary>
		public string RescaleType
		{
			get { return base.DicomAttributeProvider[DicomTags.RescaleType].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.RescaleType] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RescaleType].SetString(0, value);
			}
		}
	}

	namespace ModalityLut
	{
		/// <summary>
		/// ModalityLut Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.1 (Table C.11-1b)</remarks>
		public class ModalityLutSequenceItem : SequenceIodBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ModalityLutSequenceItem"/> class.
			/// </summary>
			public ModalityLutSequenceItem() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="ModalityLutSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public ModalityLutSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Gets or sets the value of LutDescriptor in the underlying collection. Type 1C.
			/// </summary>
			public int[] LutDescriptor
			{
				get
				{
					int[] result = new int[3];
					if (base.DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(0, out result[0]))
						if (base.DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(1, out result[1]))
							if (base.DicomAttributeProvider[DicomTags.LutDescriptor].TryGetInt32(2, out result[2]))
								return result;
					return null;
				}
				set
				{
					if (value == null || value.Length != 3)
					{
						base.DicomAttributeProvider[DicomTags.LutDescriptor] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(0, value[0]);
					base.DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(1, value[1]);
					base.DicomAttributeProvider[DicomTags.LutDescriptor].SetInt32(2, value[2]);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutExplanation in the underlying collection. Type 3.
			/// </summary>
			public string LutExplanation
			{
				get { return base.DicomAttributeProvider[DicomTags.LutExplanation].GetString(0, string.Empty); }
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						base.DicomAttributeProvider[DicomTags.LutExplanation] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.LutExplanation].SetString(0, value);
				}
			}

			/// <summary>
			/// Gets or sets the value of ModalityLutType in the underlying collection. Type 1C.
			/// </summary>
			public string ModalityLutType
			{
				get { return base.DicomAttributeProvider[DicomTags.ModalityLutType].GetString(0, string.Empty); }
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						base.DicomAttributeProvider[DicomTags.ModalityLutType] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.ModalityLutType].SetString(0, value);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutData in the underlying collection. Type 1C.
			/// </summary>
			public byte[] LutData
			{
				get
				{
					DicomAttribute attribute = base.DicomAttributeProvider[DicomTags.LutData];
					if (attribute.IsNull || attribute.IsEmpty || attribute.Count == 0)
						return null;
					return (byte[]) attribute.Values;
				}
				set
				{
					if (value == null)
					{
						base.DicomAttributeProvider[DicomTags.LutData] = null;
						return;
					}
					base.DicomAttributeProvider[DicomTags.LutData].Values = value;
				}
			}
		}
	}
}