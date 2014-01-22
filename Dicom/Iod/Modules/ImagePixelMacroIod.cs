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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Image Pixel Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.3 (Table C.7-11b)</remarks>
	public class ImagePixelMacroIod : IodBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ImagePixelMacroIod"/> class.
		/// </summary>
		public ImagePixelMacroIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImagePixelMacroIod"/> class.
		/// </summary>
		public ImagePixelMacroIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the samples per pixel.  Number of samples (planes) in this image.
		/// <para>
		/// Samples per Pixel (0028,0002) is the number of separate planes in this image. One, three, and
		/// four image planes are defined. Other numbers of image planes are allowed, but their meaning is
		/// not defined by this Standard. 
		/// </para>
		/// <para>
		/// For monochrome (gray scale) and palette color images, the number of planes is 1. 
		/// </para>
		/// <para>
		/// For RGB and other three vector color models, the value of this attribute is 3. 
		/// </para>
		/// <para>
		/// For four vector color models, the value of this attribute is 4.
		/// </para>
		/// </summary>
		/// <value>The samples per pixel.</value>
		/// <remarks>See Part 3, C.7.6.3.1.1 for more info.</remarks>
		public ushort SamplesPerPixel
		{
			get { return DicomAttributeProvider[DicomTags.SamplesPerPixel].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.SamplesPerPixel].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the photometric interpretation.
		/// </summary>
		/// <value>The photometric interpretation.</value>
		public PhotometricInterpretation PhotometricInterpretation
		{
			get { return PhotometricInterpretation.FromCodeString(DicomAttributeProvider[DicomTags.PhotometricInterpretation].GetString(0, String.Empty)); }
			set
			{
				if (value == null)
					base.DicomAttributeProvider[DicomTags.PhotometricInterpretation] = null;
				else
					base.DicomAttributeProvider[DicomTags.PhotometricInterpretation].SetStringValue(value.Code);
			}
		}

		/// <summary>
		/// Gets or sets the rows.
		/// </summary>
		/// <value>The rows.</value>
		public ushort Rows
		{
			get { return base.DicomAttributeProvider[DicomTags.Rows].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.Rows].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public ushort Columns
		{
			get { return base.DicomAttributeProvider[DicomTags.Columns].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.Columns].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the bits allocated.
		/// </summary>
		/// <value>The bits allocated.</value>
		public ushort BitsAllocated
		{
			get { return DicomAttributeProvider[DicomTags.BitsAllocated].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.BitsAllocated].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the bits stored.
		/// </summary>
		/// <value>The bits stored.</value>
		public ushort BitsStored
		{
			get { return DicomAttributeProvider[DicomTags.BitsStored].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.BitsStored].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the high bit.
		/// </summary>
		/// <value>The high bit.</value>
		public ushort HighBit
		{
			get { return DicomAttributeProvider[DicomTags.HighBit].GetUInt16(0, 0); }
			set { DicomAttributeProvider[DicomTags.HighBit].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the pixel representation.Data representation of the pixel samples. 
		/// Each sample shall have the same pixel representation. Enumerated Values: 
		/// <para>0000H = unsigned integer. </para>
		/// <para>0001H = 2's complement</para>
		/// </summary>
		/// <value>The pixel representation.</value>
		public string PixelRepresentation
		{
			get { return DicomAttributeProvider[DicomTags.PixelRepresentation].GetString(0, String.Empty); }
			set { DicomAttributeProvider[DicomTags.PixelRepresentation].SetString(0, value); }
		}

		/// <summary>
		/// Gets or sets the pixel data.
		/// </summary>
		/// <value>The pixel data.</value>
		public byte[] PixelData
		{
			get
			{
				DicomAttribute attribute = DicomAttributeProvider[DicomTags.PixelData];
				if (!attribute.IsNull && !attribute.IsEmpty)
					return (byte[]) DicomAttributeProvider[DicomTags.PixelData].Values;
				else
					return null;
			}
			set { DicomAttributeProvider[DicomTags.PixelData].Values = value; }
		}

		/// <summary>
		/// Gets or sets the planar configuration.
		/// </summary>
		/// <value>The planar configuration.</value>
		public ushort PlanarConfiguration
		{
			get { return base.DicomAttributeProvider[DicomTags.PlanarConfiguration].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.PlanarConfiguration].SetUInt16(0, value); }
		}

		public PixelAspectRatio PixelAspectRatio
		{
			get { return PixelAspectRatio.FromString(base.DicomAttributeProvider[DicomTags.PixelAspectRatio].ToString()); }
			set
			{
				if (value == null || value.IsNull)
					base.DicomAttributeProvider[DicomTags.PixelAspectRatio].SetNullValue();
				else
					base.DicomAttributeProvider[DicomTags.PixelAspectRatio].SetStringValue(value.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the smallest image pixel value.
		/// </summary>
		/// <value>The smallest image pixel value.</value>
		public ushort SmallestImagePixelValue
		{
			get { return base.DicomAttributeProvider[DicomTags.SmallestImagePixelValue].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.SmallestImagePixelValue].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the largest image pixel value.
		/// </summary>
		/// <value>The largest image pixel value.</value>
		public ushort LargestImagePixelValue
		{
			get { return base.DicomAttributeProvider[DicomTags.LargestImagePixelValue].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.LargestImagePixelValue].SetUInt16(0, value); }
		}

		/// <summary>
		/// Gets or sets the red palette color lookup table descriptor.
		/// </summary>
		/// <value>The red palette color lookup table descriptor.</value>
		public ushort RedPaletteColorLookupTableDescriptor
		{
			get { return base.DicomAttributeProvider[DicomTags.RedPaletteColorLookupTableDescriptor].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.RedPaletteColorLookupTableDescriptor].SetUInt16(0, value); }
		}

		public ushort GreenPaletteColorLookupTableDescriptor
		{
			get { return base.DicomAttributeProvider[DicomTags.GreenPaletteColorLookupTableDescriptor].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.GreenPaletteColorLookupTableDescriptor].SetUInt16(0, value); }
		}

		public ushort BluePaletteColorLookupTableDescriptor
		{
			get { return base.DicomAttributeProvider[DicomTags.BluePaletteColorLookupTableDescriptor].GetUInt16(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.BluePaletteColorLookupTableDescriptor].SetUInt16(0, value); }
		}

		//TODO: Red Palette Color Lookup Table Data
		//TODO: Green Palette Color Lookup Table Data
		//TODO: Blue Palette Color Lookup Table Data
		//TODO: IccProfile

		#endregion
	}
}