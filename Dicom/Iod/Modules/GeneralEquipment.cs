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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralEquipment Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2009, Part 3, Section C.7.5.1 (Table C.7-8)</remarks>
	public class GeneralEquipmentModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
		/// </summary>	
		public GeneralEquipmentModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralEquipmentModuleIod"/> class.
		/// </summary>
		public GeneralEquipmentModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets or sets the value of Manufacturer in the underlying collection. Type 2.
		/// </summary>
		public string Manufacturer
		{
			get { return DicomAttributeProvider[DicomTags.Manufacturer].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.Manufacturer].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.Manufacturer].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the values of InstitutionName, InstitutionAddress and InstitutionalDepartmentName in the underlying collection. Type 3.
		/// </summary>
		public Institution Institution
		{
			get { return new Institution(InstitutionName, InstitutionAddress, InstitutionalDepartmentName); }
			set
			{
				InstitutionName = value.Name;
				InstitutionAddress = value.Address;
				InstitutionalDepartmentName = value.DepartmentName;
			}
		}

		/// <summary>
		/// Gets or sets the value of InstitutionName in the underlying collection. Type 3.
		/// </summary>
		public string InstitutionName
		{
			get { return DicomAttributeProvider[DicomTags.InstitutionName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.InstitutionName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InstitutionName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InstitutionAddress in the underlying collection. Type 3.
		/// </summary>
		public string InstitutionAddress
		{
			get { return DicomAttributeProvider[DicomTags.InstitutionAddress].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.InstitutionAddress] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InstitutionAddress].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of StationName in the underlying collection. Type 3.
		/// </summary>
		public string StationName
		{
			get { return DicomAttributeProvider[DicomTags.StationName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.StationName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.StationName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InstitutionalDepartmentName in the underlying collection. Type 3.
		/// </summary>
		public string InstitutionalDepartmentName
		{
			get { return DicomAttributeProvider[DicomTags.InstitutionalDepartmentName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.InstitutionalDepartmentName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InstitutionalDepartmentName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ManufacturersModelName in the underlying collection. Type 3.
		/// </summary>
		public string ManufacturersModelName
		{
			get { return DicomAttributeProvider[DicomTags.ManufacturersModelName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ManufacturersModelName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ManufacturersModelName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceSerialNumber in the underlying collection. Type 3.
		/// </summary>
		public string DeviceSerialNumber
		{
			get { return DicomAttributeProvider[DicomTags.DeviceSerialNumber].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DeviceSerialNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceSerialNumber].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SoftwareVersions in the underlying collection. Type 3.
		/// </summary>
		public string SoftwareVersions
		{
			get { return DicomAttributeProvider[DicomTags.SoftwareVersions].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.SoftwareVersions] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SoftwareVersions].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GantryId in the underlying collection. Type 3.
		/// </summary>
		public string GantryId
		{
			get { return DicomAttributeProvider[DicomTags.GantryId].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.GantryId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.GantryId].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SpatialResolution in the underlying collection. Type 3.
		/// </summary>
		public double? SpatialResolution
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SpatialResolution].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SpatialResolution] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SpatialResolution].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DateOfLastCalibration and TimeOfLastCalibration in the underlying collection. Type 3.
		/// </summary>
		public DateTime? DateTimeOfLastCalibration
		{
			get
			{
				var date = DicomAttributeProvider[DicomTags.DateOfLastCalibration].GetString(0, string.Empty);
				var time = DicomAttributeProvider[DicomTags.TimeOfLastCalibration].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DateOfLastCalibration] = null;
					DicomAttributeProvider[DicomTags.TimeOfLastCalibration] = null;
					return;
				}
				var date = DicomAttributeProvider[DicomTags.DateOfLastCalibration];
				var time = DicomAttributeProvider[DicomTags.TimeOfLastCalibration];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PixelPaddingValue in the underlying collection. Type 3.
		/// </summary>
		public int? PixelPaddingValue
		{
			get
			{
				int result;
				if (DicomAttributeProvider[DicomTags.PixelPaddingValue].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.PixelPaddingValue] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.PixelPaddingValue].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Initializes the attributes of the module to their default values.
		/// </summary>
		public void InitializeAttributes()
		{
			Manufacturer = string.Empty;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.DateOfLastCalibration;
				yield return DicomTags.TimeOfLastCalibration;
				yield return DicomTags.DeviceSerialNumber;
				yield return DicomTags.GantryId;
				yield return DicomTags.InstitutionAddress;
				yield return DicomTags.InstitutionalDepartmentName;
				yield return DicomTags.InstitutionName;
				yield return DicomTags.Manufacturer;
				yield return DicomTags.ManufacturersModelName;
				yield return DicomTags.PixelPaddingValue;
				yield return DicomTags.SoftwareVersions;
				yield return DicomTags.SpatialResolution;
				yield return DicomTags.StationName;
			}
		}
	}
}