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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// SC Image Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.6.2 (Table C.8-25)</remarks>
	public class ScImageModuleIod : BasicPixelSpacingCalibrationMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScImageModuleIod"/> class.
		/// </summary>	
		public ScImageModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScImageModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public ScImageModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.DateOfSecondaryCapture;
				yield return DicomTags.TimeOfSecondaryCapture;
				yield return DicomTags.NominalScannedPixelSpacing;
				yield return DicomTags.DocumentClassCodeSequence;
				yield return DicomTags.PixelSpacing;
				yield return DicomTags.PixelSpacingCalibrationDescription;
				yield return DicomTags.PixelSpacingCalibrationType;
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
			return !(IsNullOrEmpty(DateTimeOfSecondaryCapture)
			         && IsNullOrEmpty(NominalScannedPixelSpacing)
			         && IsNullOrEmpty(DocumentClassCodeSequence)
			         && IsNullOrEmpty(PixelSpacing)
			         && IsNullOrEmpty(PixelSpacingCalibrationDescription)
			         && IsNullOrEmpty(PixelSpacingCalibrationType));
		}

		/// <summary>
		/// Gets or sets the value of DateOfSecondaryCapture and TimeOfSecondaryCapture in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? DateTimeOfSecondaryCapture
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.DateOfSecondaryCapture].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.TimeOfSecondaryCapture].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DateOfSecondaryCapture] = null;
					DicomAttributeProvider[DicomTags.TimeOfSecondaryCapture] = null;
					return;
				}
				var date = DicomAttributeProvider[DicomTags.DateOfSecondaryCapture];
				var time = DicomAttributeProvider[DicomTags.TimeOfSecondaryCapture];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of NominalScannedPixelSpacing in the underlying collection. Type 3.
		/// </summary>
		public double[] NominalScannedPixelSpacing
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.NominalScannedPixelSpacing].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the value of DocumentClassCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public CodeSequenceMacro[] DocumentClassCodeSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DocumentClassCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
				{
					return null;
				}

				var result = new CodeSequenceMacro[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new CodeSequenceMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.DocumentClassCodeSequence] = null;
					return;
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DocumentClassCodeSequence].Values = result;
			}
		}
	}
}