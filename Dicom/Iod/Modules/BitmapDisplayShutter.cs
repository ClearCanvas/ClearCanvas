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
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// BitmapDisplayShutter Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.6.15 (Table ?)</remarks>
	public class BitmapDisplayShutterModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BitmapDisplayShutterModuleIod"/> class.
		/// </summary>	
		public BitmapDisplayShutterModuleIod() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BitmapDisplayShutterModuleIod"/> class.
		/// </summary>
		public BitmapDisplayShutterModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets or sets the shutter shape.  Type 1.
		/// </summary>
		public ShutterShape ShutterShape
		{
			get
			{
				ShutterShape returnValue = ShutterShape.None;
				string[] values = base.DicomAttributeProvider[DicomTags.ShutterShape].Values as string[];
				if (values != null && values.Length > 0)
				{
					foreach (string value in values)
					{
						string upperValue = value.ToUpperInvariant();
						if (upperValue == "CIRCULAR")
							returnValue |= Iod.ShutterShape.Circular;
						else if (upperValue == "RECTANGULAR")
							returnValue |= Iod.ShutterShape.Rectangular;
						else if (upperValue == "POLYGONAL")
							returnValue |= Iod.ShutterShape.Polygonal;
						else if (upperValue == "BITMAP")
							returnValue |= Iod.ShutterShape.Bitmap;
					}
				}

				return returnValue;
			}
			set
			{
				if (value == ShutterShape.None)
				{
					base.DicomAttributeProvider[DicomTags.ShutterShape] = null;
				}
				else if ((value & ShutterShape.Bitmap) == ShutterShape.Bitmap)
				{
					base.DicomAttributeProvider[DicomTags.ShutterShape].SetString(0, ShutterShape.Bitmap.ToString().ToUpperInvariant());
				}
				else
				{
					throw new ArgumentException("Only BITMAP is supported in this module.", "value");
				}
			}
		}

		/// <summary>
		/// Gets or sets the zero-based index of the overlay to use as a bitmap display shutter (0-15).
		/// </summary>
		/// <remarks>
		/// Setting this value will automatically update the <see cref="ShutterOverlayGroup"/> tag.
		/// </remarks>
		/// <seealso cref="ShutterOverlayGroup"/>
		/// <seealso cref="ShutterOverlayGroupTagOffset"/>
		public int ShutterOverlayGroupIndex
		{
			get { return (this.ShutterOverlayGroup - 0x6000)/2; }
			set
			{
				if (value < 0 || value > 15)
					throw new ArgumentOutOfRangeException("value", "Value must be between 0 and 15 inclusive.");
				this.ShutterOverlayGroup = (ushort) (value*2 + 0x6000);
			}
		}

		/// <summary>
		/// Gets or sets the DICOM tag value offset from the defined base tags (such as <see cref="DicomTags.OverlayBitPosition"/>).
		/// </summary>
		/// <remarks>
		/// Setting this value will automatically update the <see cref="ShutterOverlayGroup"/> tag.
		/// </remarks>
		/// <seealso cref="ShutterOverlayGroup"/>
		/// <seealso cref="ShutterOverlayGroupIndex"/>
		public uint ShutterOverlayGroupTagOffset
		{
			get { return (uint) ((this.ShutterOverlayGroup - 0x6000) << 16); }
			set { this.ShutterOverlayGroup = (ushort) ((value >> 16) + 0x6000); }
		}

		/// <summary>
		/// Gets or sets the DICOM group number of the overlay to use as a bitmap display shutter. Type 1.
		/// </summary>
		/// <seealso cref="ShutterOverlayGroupTagOffset"/>
		/// <seealso cref="ShutterOverlayGroupIndex"/>
		public ushort ShutterOverlayGroup
		{
			get
			{
				ushort group = base.DicomAttributeProvider[DicomTags.ShutterOverlayGroup].GetUInt16(0, 0);
				if ((group & 0xFFE1) != 0x6000)
					return 0x0000;
				return group;
			}
			set
			{
				if ((value & 0xFFE1) != 0x6000)
					throw new ArgumentOutOfRangeException("value", "Overlay group must be an even value between 0x6000 and 0x601E inclusive.");
				base.DicomAttributeProvider[DicomTags.ShutterOverlayGroup].SetUInt16(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the shutter presentation value.  Type 1.
		/// </summary>
		public ushort? ShutterPresentationValue
		{
			get
			{
				DicomAttribute attribute;
				if (base.DicomAttributeProvider.TryGetAttribute(DicomTags.ShutterPresentationValue, out attribute))
					return attribute.GetUInt16(0, 0);
				else
					return null;
			}
			set
			{
				if (!value.HasValue)
					base.DicomAttributeProvider[DicomTags.ShutterPresentationValue] = null;
				else
					base.DicomAttributeProvider[DicomTags.ShutterPresentationValue].SetUInt16(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the shutter presentation color value.  Type 3.
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
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.ShutterShape;
				yield return DicomTags.ShutterOverlayGroup;
				yield return DicomTags.ShutterPresentationValue;
				yield return DicomTags.ShutterPresentationColorCielabValue;
				yield break;
			}
		}
	}
}