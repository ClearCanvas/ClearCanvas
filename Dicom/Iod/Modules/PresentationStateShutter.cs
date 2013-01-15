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
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// PresentationStateShutter Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.12 (Table C.11.12-1)</remarks>
	public class PresentationStateShutterModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateShutterModuleIod"/> class.
		/// </summary>	
		public PresentationStateShutterModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentationStateShutterModuleIod"/> class.
		/// </summary>
		public PresentationStateShutterModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public void InitializeAttributes()
		{
			this.ShutterPresentationColorCielabValue = null;
			this.ShutterPresentationValue = null;
		}

		/// <summary>
		/// Gets or sets the shutter presentation value.  Type 1C.
		/// </summary>
		public int? ShutterPresentationValue
		{
			get
			{
				DicomAttribute attribute;
				if (base.DicomAttributeProvider.TryGetAttribute(DicomTags.ShutterPresentationValue, out attribute))
					return attribute.GetInt32(0, 0);
				else
					return null;
			}
			set
			{
				if (!value.HasValue)
					base.DicomAttributeProvider[DicomTags.ShutterPresentationValue] = null;
				else
					base.DicomAttributeProvider[DicomTags.ShutterPresentationValue].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the shutter presentation color value.  Type 1C.
		/// </summary>
		public CIELabColor? ShutterPresentationColorCielabValue
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeProvider[DicomTags.ShutterPresentationColorCielabValue];
				if (attribute.IsEmpty || attribute.IsNull)
					return null;

				ushort[] values = attribute.Values as ushort[];
				if (values != null && values.Length >= 3)
					return new CIELabColor(values[0], values[1], values[2]);
				else
					return null;
			}
			set
			{
				if (!value.HasValue)
					base.DicomAttributeProvider[DicomTags.ShutterPresentationColorCielabValue] = null;
				else
					base.DicomAttributeProvider[DicomTags.ShutterPresentationColorCielabValue].Values = value.Value.ToArray();
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ShutterPresentationColorCielabValue;
				yield return DicomTags.ShutterPresentationValue;
			}
		}
	}
}