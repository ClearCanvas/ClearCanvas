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

using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.ModalityLut;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// ModalityLut Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.1 (Table C.11-1)</remarks>
	public class ModalityLutModuleIod : IodBase, IModalityLutMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModalityLutModuleIod"/> class.
		/// </summary>	
		public ModalityLutModuleIod() : base() {}

		//TODO: Modality lut module should take ???

		/// <summary>
		/// Initializes a new instance of the <see cref="ModalityLutModuleIod"/> class.
		/// </summary>
		public ModalityLutModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { return base.DicomAttributeProvider as DicomSequenceItem; }
			set { base.DicomAttributeProvider = value; }
		}

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

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ModalityLutSequence;
				yield return DicomTags.RescaleIntercept;
				yield return DicomTags.RescaleSlope;
				yield return DicomTags.RescaleType;
			}
		}
	}
}