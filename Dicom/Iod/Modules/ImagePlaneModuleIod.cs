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
	/// Image Plane Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.7.6.2 (Table C.7-10)</remarks>
	public class ImagePlaneModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImagePlaneModuleIod"/> class.
		/// </summary>	
		public ImagePlaneModuleIod() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImagePlaneModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeProvider">The DICOM attribute collection.</param>
		public ImagePlaneModuleIod(IDicomAttributeProvider dicomAttributeProvider)
			: base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				yield return DicomTags.PixelSpacing;
				yield return DicomTags.ImageOrientationPatient;
				yield return DicomTags.ImagePositionPatient;
				yield return DicomTags.SliceThickness;
				yield return DicomTags.SliceLocation;
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
			return !(IsNullOrEmpty(PixelSpacing)
			         && IsNullOrEmpty(ImageOrientationPatient)
			         && IsNullOrEmpty(ImagePositionPatient)
			         && IsNullOrEmpty(SliceThickness)
			         && IsNullOrEmpty(SliceLocation));
		}

		/// <summary>
		/// Gets or sets the value of PixelSpacing in the underlying collection. Type 1.
		/// </summary>
		public double[] PixelSpacing
		{
			get
			{
				var result = new double[2];
				if (DicomAttributeProvider[DicomTags.PixelSpacing].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.PixelSpacing].TryGetFloat64(1, out result[1]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 2)
				{
					const string msg = "PixelSpacing is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat64(1, value[1]);
			}
		}

		/// <summary>
		/// Gets or sets the Row component of PixelSpacing in the underlying collection. Type 1.
		/// </summary>
		/// <remarks>
		/// <para>In mm, that is the spacing between the centers of adjacent rows, or vertical spacing.
		/// </para>
		/// <para>See DICOM Standard 2011, Part 3, Section 10.7.1.3 for more info.</para>
		/// </remarks>
		public float PixelSpacingRow
		{
			get { return DicomAttributeProvider[DicomTags.PixelSpacing].GetFloat32(0, 0.0F); }
			set { DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat32(0, value); }
		}

		/// <summary>
		/// Gets or sets the Column component of PixelSpacing in the underlying collection. Type 1.
		/// </summary>
		/// <remarks>
		/// <para>in mm, that is the spacing between the centers of adjacent columns, 
		/// or horizontal spacing.</para>
		/// <para>See DICOM Standard 2011, Part 3, Section 10.7.1.3 for more info.</para>
		/// </remarks>
		public float PixelSpacingColumn
		{
			get { return DicomAttributeProvider[DicomTags.PixelSpacing].GetFloat32(1, 0.0F); }
			set { DicomAttributeProvider[DicomTags.PixelSpacing].SetFloat32(1, value); }
		}

		/// <summary>
		/// Gets or sets the value of ImageOrientationPatient in the underlying collection. Type 1.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Image Orientation (0020,0037) specifies the direction cosines of the first row and the 
		/// first column with respect to the patient. These Attributes shall be provide as a pair. 
		/// Row value for the x, y, and z axes respectively followed by the Column value for the x, y, 
		/// and z axes respectively.</para>
		/// </remarks>
		public double[] ImageOrientationPatient
		{
			get
			{
				var result = new double[6];
				if (DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(2, out result[2])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(3, out result[3])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(4, out result[4])
				    && DicomAttributeProvider[DicomTags.ImageOrientationPatient].TryGetFloat64(5, out result[5]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 6)
				{
					const string msg = "ImageOrientationPatient is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(2, value[2]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(3, value[3]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(4, value[4]);
				DicomAttributeProvider[DicomTags.ImageOrientationPatient].SetFloat64(5, value[5]);
			}
		}

		/// <summary>
		/// Gets or sets the value of ImagePositionPatient in the underlying collection. Type 1.
		/// </summary>
		/// <remarks>
		/// <para>The Image Position (0020,0032) specifies the x, y, and z coordinates of the upper 
		/// left hand corner of the image; it is the center of the first voxel transmitted. 
		/// </para>
		/// </remarks>
		public double[] ImagePositionPatient
		{
			get
			{
				var result = new double[3];
				if (DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(0, out result[0])
				    && DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(1, out result[1])
				    && DicomAttributeProvider[DicomTags.ImagePositionPatient].TryGetFloat64(2, out result[2]))
					return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 3)
				{
					const string msg = "ImagePositionPatient is Type 1 Required.";
					throw new ArgumentNullException("value", msg);
				}
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(0, value[0]);
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(1, value[1]);
				DicomAttributeProvider[DicomTags.ImagePositionPatient].SetFloat64(2, value[2]);
			}
		}

		/// <summary>
		/// Gets or sets the value of SliceThickness in the underlying collection. Type 2.
		/// </summary>
		public double? SliceThickness
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SliceThickness].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SliceThickness].SetNullValue();
					return;
				}
				DicomAttributeProvider[DicomTags.SliceThickness].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SliceLocation in the underlying collection. Type 3.
		/// </summary>
		public double? SliceLocation
		{
			get
			{
				double result;
				if (DicomAttributeProvider[DicomTags.SliceLocation].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					DicomAttributeProvider[DicomTags.SliceLocation] = null;
					return;
				}
				DicomAttributeProvider[DicomTags.SliceLocation].SetFloat64(0, value.Value);
			}
		}
	}
}