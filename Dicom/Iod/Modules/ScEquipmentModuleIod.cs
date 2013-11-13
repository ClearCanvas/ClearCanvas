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
	/// SC Equipment Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.6.1 (Table C.8-24)</remarks>
	public class ScEquipmentModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScEquipmentModuleIod"/> class.
		/// </summary>	
		public ScEquipmentModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScEquipmentModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public ScEquipmentModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.ConversionType;
				yield return DicomTags.Modality;
				yield return DicomTags.SecondaryCaptureDeviceId;
				yield return DicomTags.SecondaryCaptureDeviceManufacturer;
				yield return DicomTags.SecondaryCaptureDeviceManufacturersModelName;
				yield return DicomTags.SecondaryCaptureDeviceSoftwareVersions;
				yield return DicomTags.VideoImageFormatAcquired;
				yield return DicomTags.DigitalImageFormatAcquired;
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
			return !(IsNullOrEmpty(ConversionType)
			         && IsNullOrEmpty(Modality)
			         && IsNullOrEmpty(SecondaryCaptureDeviceId)
			         && IsNullOrEmpty(SecondaryCaptureDeviceManufacturer)
			         && IsNullOrEmpty(SecondaryCaptureDeviceManufacturersModelName)
			         && IsNullOrEmpty(VideoImageFormatAcquired)
			         && IsNullOrEmpty(DigitalImageFormatAcquired));
		}

		/// <summary>
		/// Gets or sets the value of ConversionType in the underlying collection. Type 1.
		/// </summary>
		public string ConversionType
		{
			get { return DicomAttributeProvider[DicomTags.ConversionType].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					const string msg = "ConversionType is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ConversionType].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Modality in the underlying collection. Type 3.
		/// </summary>
		public string Modality
		{
			get { return DicomAttributeProvider[DicomTags.Modality].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.Modality] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Modality].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryCaptureDeviceId in the underlying collection. Type 3.
		/// </summary>
		public string SecondaryCaptureDeviceId
		{
			get { return DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceId].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceId].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryCaptureDeviceManufacturer in the underlying collection. Type 3.
		/// </summary>
		public string SecondaryCaptureDeviceManufacturer
		{
			get { return DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturer].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturer] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturer].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryCaptureDeviceManufacturersModelName in the underlying collection. Type 3.
		/// </summary>
		public string SecondaryCaptureDeviceManufacturersModelName
		{
			get { return DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturersModelName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturersModelName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceManufacturersModelName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SecondaryCaptureDeviceSoftwareVersions in the underlying collection. Type 3.
		/// </summary>
		public string[] SecondaryCaptureDeviceSoftwareVersions
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceSoftwareVersions];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new string[dicomAttribute.Count];
				for (var n = 0; n < result.Length; n++)
					result[n] = dicomAttribute.GetString(n, string.Empty);
				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceSoftwareVersions] = null;
					return;
				}

				var dicomAttribute = DicomAttributeProvider[DicomTags.SecondaryCaptureDeviceSoftwareVersions];
				for (var n = 0; n < value.Length; n++)
					dicomAttribute.SetString(n, value[n]);
			}
		}

		/// <summary>
		/// Gets or sets the value of VideoImageFormatAcquired in the underlying collection. Type 3.
		/// </summary>
		public string VideoImageFormatAcquired
		{
			get { return DicomAttributeProvider[DicomTags.VideoImageFormatAcquired].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.VideoImageFormatAcquired] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.VideoImageFormatAcquired].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DigitalImageFormatAcquired in the underlying collection. Type 3.
		/// </summary>
		public string DigitalImageFormatAcquired
		{
			get { return DicomAttributeProvider[DicomTags.DigitalImageFormatAcquired].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DigitalImageFormatAcquired] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DigitalImageFormatAcquired].SetStringValue(value);
			}
		}
	}
}