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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// SoftcopyPresentationLut Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.6 (Table C.11.6-1)</remarks>
	public class SoftcopyPresentationLutModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SoftcopyPresentationLutModuleIod"/> class.
		/// </summary>	
		public SoftcopyPresentationLutModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SoftcopyPresentationLutModuleIod"/> class.
		/// </summary>
		public SoftcopyPresentationLutModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.PresentationLutSequence = null;
			this.PresentationLutShape = PresentationLutShape.None;
		}

		/// <summary>
		/// Gets or sets the value of PresentationLutSequence in the underlying collection. Type 1C.
		/// </summary>
		public PresentationLutSequenceItem PresentationLutSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PresentationLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PresentationLutSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.PresentationLutSequence];
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.PresentationLutSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of PresentationLutShape in the underlying collection. Type 1C.
		/// </summary>
		public PresentationLutShape PresentationLutShape
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.PresentationLutShape].GetString(0, string.Empty), PresentationLutShape.None); }
			set
			{
				if (value == PresentationLutShape.None)
				{
					base.DicomAttributeProvider[DicomTags.PresentationLutShape] = null;
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PresentationLutShape], value);
			}
		}

		/// <summary>
		/// PresentationLut Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.6 (Table C.11.6-1)</remarks>
		public class PresentationLutSequenceItem : SequenceIodBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PresentationLutSequenceItem"/> class.
			/// </summary>
			public PresentationLutSequenceItem() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="PresentationLutSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public PresentationLutSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Gets or sets the value of LutDescriptor in the underlying collection. Type 1.
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
						throw new ArgumentNullException("value", "LutDescriptor is Type 1 Required.");
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
			/// Gets or sets the value of LutData in the underlying collection. Type 1.
			/// </summary>
			public byte[] LutData
			{
				get
				{
					DicomAttribute attribute = base.DicomAttributeProvider[DicomTags.LutData];
					if (attribute.IsNull || attribute.IsEmpty)
						return null;
					return (byte[]) attribute.Values;
				}
				set
				{
					if (value == null)
						throw new ArgumentOutOfRangeException("value", "LutData is Type 1 Required.");
					base.DicomAttributeProvider[DicomTags.LutData].Values = value;
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.PresentationLutSequence;
				yield return DicomTags.PresentationLutShape;
			}
		}
	}
}