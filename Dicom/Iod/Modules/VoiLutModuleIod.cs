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
using ClearCanvas.Dicom.Iod.Macros.VoiLut;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// VoiLut Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.11.2 (Table C.11-2)</remarks>
	public class VoiLutModuleIod : IodBase, IVoiLutMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutModuleIod"/> class.
		/// </summary>	
		public VoiLutModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public VoiLutModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.VoiLutSequence;
				yield return DicomTags.WindowCenter;
				yield return DicomTags.WindowWidth;
				yield return DicomTags.WindowCenterWidthExplanation;
				yield return DicomTags.VoiLutFunction;
			}
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes() {}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(VoiLutSequence)
			         && IsNullOrEmpty(WindowCenter)
			         && IsNullOrEmpty(WindowWidth)
			         && IsNullOrEmpty(WindowCenterWidthExplanation)
			         && IsNullOrEmpty(VoiLutFunction));
		}

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

		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { throw new InvalidOperationException(); }
			set { throw new InvalidOperationException(); }
		}
	}
}