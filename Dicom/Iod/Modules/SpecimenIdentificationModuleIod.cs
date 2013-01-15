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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// SpecimenIdentification Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.2 (Table C.7-2a)</remarks>
	public class SpecimenIdentificationModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenIdentificationModuleIod"/> class.
		/// </summary>	
		public SpecimenIdentificationModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecimenIdentificationModuleIod"/> class.
		/// </summary>
		public SpecimenIdentificationModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.SpecimenAccessionNumber = "1";
			this.SpecimenSequence = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			if (this.SpecimenSequence == null && string.IsNullOrEmpty(this.SpecimenAccessionNumber))
				return false;
			return true;
		}

		/// <summary>
		/// Gets or sets the value of SpecimenAccessionNumber in the underlying collection. Type 1.
		/// </summary>
		public string SpecimenAccessionNumber
		{
			get { return base.DicomAttributeProvider[DicomTags.SpecimenAccessionNumberRetired].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SpecimenAccessionNumber is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.SpecimenAccessionNumberRetired].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SpecimenSequence in the underlying collection. Type 2.
		/// </summary>
		public SpecimenSequence[] SpecimenSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.SpecimenSequenceRetired];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				SpecimenSequence[] result = new SpecimenSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new SpecimenSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.SpecimenSequenceRetired].SetNullValue();
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.SpecimenSequenceRetired].Values = result;
			}
		}
	}
}