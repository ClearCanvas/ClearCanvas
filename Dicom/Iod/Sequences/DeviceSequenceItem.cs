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

using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// Device Sequence Item
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.12 (Table C.7-18)</remarks>
	public class DeviceSequenceItem : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceSequenceItem"/> class.
		/// </summary>
		public DeviceSequenceItem() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The DICOM sequence item.</param>
		public DeviceSequenceItem(DicomSequenceItem dicomSequenceItem)
			: base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of Manufacturer in the underlying collection. Type 3.
		/// </summary>
		public string Manufacturer
		{
			get { return DicomAttributeProvider[DicomTags.Manufacturer].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.Manufacturer] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.Manufacturer].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ManufacturersModelName in the underlying collection. Type 3.
		/// </summary>
		public string ManufacturersModelName
		{
			get { return DicomAttributeProvider[DicomTags.ManufacturersModelName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.ManufacturersModelName] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.ManufacturersModelName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceSerialNumber in the underlying collection. Type 3.
		/// </summary>
		public string DeviceSerialNumber
		{
			get { return DicomAttributeProvider[DicomTags.DeviceSerialNumber].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DeviceSerialNumber] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceSerialNumber].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceId in the underlying collection. Type 3.
		/// </summary>
		public string DeviceId
		{
			get { return DicomAttributeProvider[DicomTags.DeviceId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DeviceId] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceLength in the underlying collection. Type 3.
		/// </summary>
		public double? DeviceLength
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.DeviceLength].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DeviceLength] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceLength].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceDiameter in the underlying collection. Type 3.
		/// </summary>
		public double? DeviceDiameter
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.DeviceDiameter].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DeviceDiameter] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceDiameter].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceDiameterUnits in the underlying collection. Type 2C.
		/// </summary>
		public string DeviceDiameterUnits
		{
			get { return DicomAttributeProvider[DicomTags.DeviceDiameterUnits].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DeviceDiameterUnits] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceDiameterUnits].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceVolume in the underlying collection. Type 3.
		/// </summary>
		public double? DeviceVolume
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.DeviceVolume].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.DeviceVolume] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceVolume].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of InterMarkerDistance in the underlying collection. Type 3.
		/// </summary>
		public double? InterMarkerDistance
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.InterMarkerDistance].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.InterMarkerDistance] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.InterMarkerDistance].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DeviceDescription in the underlying collection. Type 3.
		/// </summary>
		public string DeviceDescription
		{
			get { return DicomAttributeProvider[DicomTags.DeviceDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					DicomAttributeProvider[DicomTags.DeviceDescription] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.DeviceDescription].SetString(0, value);
			}
		}
	}
}