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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Device Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.12 (Table C.7-18)</remarks>
	public class DeviceModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceModuleIod"/> class.
		/// </summary>	
		public DeviceModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public DeviceModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.DeviceSequence; }
		}

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			DeviceSequence = null;
		}

		/// <summary>
		/// Checks if this module appears to be non-empty.
		/// </summary>
		/// <returns>True if the module appears to be non-empty; False otherwise.</returns>
		public bool HasValues()
		{
			return !(IsNullOrEmpty(DeviceSequence));
		}

		/// <summary>
		/// Gets or sets the value of DeviceSequence in the underlying collection. Type 1.
		/// </summary>
		public DeviceSequenceItem[] DeviceSequence
		{
			get
			{
				var dicomAttribute = DicomAttributeProvider[DicomTags.DeviceSequence];
				if (dicomAttribute.IsNull || dicomAttribute.IsEmpty)
					return null;

				var result = new DeviceSequenceItem[dicomAttribute.Count];
				var items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new DeviceSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					const string msg = "DeviceSequence is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}

				var result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				DicomAttributeProvider[DicomTags.DeviceSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a DeviceSequence item. Does not modify the DeviceSequence in the underlying collection.
		/// </summary>
		public DeviceSequenceItem CreateDeviceSequenceItem()
		{
			var iodBase = new DeviceSequenceItem(new DicomSequenceItem());
			return iodBase;
		}
	}
}