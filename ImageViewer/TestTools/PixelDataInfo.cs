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

using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.TestTools
{
	[Cloneable(true)]
	internal class PixelDataInfo
	{
		private PhotometricInterpretation _internalPhotometricInterpretation;

		public PixelDataInfo()
		{}

		public PixelDataInfo(DicomAttributeCollection dataset)
		{
			dataset.LoadDicomFields(this);

			AspectRatio = PixelAspectRatio.FromString(dataset[DicomTags.PixelAspectRatio].ToString()) ?? new PixelAspectRatio(0, 0);
			PixelSpacing = PixelSpacing.FromString(dataset[DicomTags.PixelSpacing].ToString()) ?? new PixelSpacing(0, 0);
			ImagerPixelSpacing = PixelSpacing.FromString(dataset[DicomTags.ImagerPixelSpacing].ToString()) ?? new PixelSpacing(0, 0);
		}

		[DicomField(DicomTags.Rows)]
		public int Rows { get; set; }

		[DicomField(DicomTags.Columns)]
		public int Columns { get; set; }

		[DicomField(DicomTags.BitsAllocated)]
		public int BitsAllocated { get; set; }
			
		[DicomField(DicomTags.BitsStored)]
		public int BitsStored { get; set; }

		[DicomField(DicomTags.HighBit)]
		public int HighBit { get; set; }
			
		[DicomField(DicomTags.PixelRepresentation)]
		public int PixelRepresentation { get; set; }
			
		[DicomField(DicomTags.PhotometricInterpretation)]
		public string PhotometricInterpretation { get; set; }

		[DicomField(DicomTags.PlanarConfiguration)]
		public int PlanarConfiguration { get; set; }

		[DicomField(DicomTags.SeriesDescription)]
		public string SeriesDescription { get; set; }

		[DicomField(DicomTags.PixelData)]
		public byte[] PixelData { get; set; }

		public PixelAspectRatio AspectRatio { get; set; }
		public PixelSpacing PixelSpacing { get; set; }
		public PixelSpacing ImagerPixelSpacing { get; set; }

		public bool IsCalibrated
		{
			get { return !PixelSpacing.IsNull || !ImagerPixelSpacing.IsNull; }	
		}

		public bool IsSquare
		{
			get
			{
				if (!AspectRatio.IsNull && !FloatComparer.AreEqual(1, AspectRatio.Value))
					return false;
				if (!PixelSpacing.IsNull && !FloatComparer.AreEqual(1, (float)PixelSpacing.AspectRatio))
					return false;
				if (!ImagerPixelSpacing.IsNull && !FloatComparer.AreEqual(1, (float)ImagerPixelSpacing.AspectRatio))
					return false;

				return true;
			}	
		}

		public bool IsColor { get { return InternalPhotometricInterpretation.IsColor; } }

		private PhotometricInterpretation InternalPhotometricInterpretation
		{
			get
			{
				if (_internalPhotometricInterpretation == null)
					_internalPhotometricInterpretation = ClearCanvas.Dicom.Iod.PhotometricInterpretation.FromCodeString(PhotometricInterpretation);
				return _internalPhotometricInterpretation;
			}
		}

		public PixelSpacing GetPixelSpacing()
		{
			return PixelSpacing.IsNull ? ImagerPixelSpacing : PixelSpacing;
		}

		public SizeF? GetRealDimensions()
		{
			PixelSpacing spacing = GetPixelSpacing();
			if (spacing.IsNull)
				return null;

			return new SizeF((float)spacing.Column * Columns, (float)spacing.Row * Rows);
		}

		public SizeF GetEffectiveDimensions()
		{
			if (AspectRatio.IsNull)
				return new SizeF(Columns, Rows);

			if (AspectRatio.Value >= 1)
				return new SizeF(Columns, Rows * AspectRatio.Value);

			return new SizeF(Columns / AspectRatio.Value, Rows);
		}


		//pixel data is not cloned.
		public PixelDataInfo Clone()
		{
			PixelDataInfo clone = (PixelDataInfo)CloneBuilder.Clone(this);
			clone.AspectRatio = new PixelAspectRatio(AspectRatio.Row, AspectRatio.Column);
			clone.PixelSpacing = new PixelSpacing(PixelSpacing.Row, PixelSpacing.Column);
			clone.ImagerPixelSpacing = new PixelSpacing(ImagerPixelSpacing.Row, ImagerPixelSpacing.Column);
			return clone;
		}

		public void UpdateDataSet(DicomAttributeCollection dataset)
		{
			dataset.SaveDicomFields(this);

			if (IsColor)
			{
				dataset[DicomTags.PixelRepresentation].SetEmptyValue(); ;
			}
			else
			{
				dataset[DicomTags.PlanarConfiguration].SetEmptyValue(); ;
			}

			if (PixelSpacing.IsNull && ImagerPixelSpacing.IsNull)
				dataset[DicomTags.PixelAspectRatio].SetStringValue(AspectRatio.ToString());
			
			if (!PixelSpacing.IsNull)
				dataset[DicomTags.PixelSpacing].SetStringValue(PixelSpacing.ToString());
			else
				dataset[DicomTags.PixelSpacing].SetEmptyValue();

			if (!ImagerPixelSpacing.IsNull)
				dataset[DicomTags.ImagerPixelSpacing].SetStringValue(ImagerPixelSpacing.ToString());
			else
				dataset[DicomTags.ImagerPixelSpacing].SetEmptyValue();
		}

		public PixelData GetPixelData()
		{
			if (!IsColor)
			{
				byte[] pixelData = PixelData ?? new byte[Rows * Columns * BitsAllocated / 8];
				return new GrayscalePixelData(Rows, Columns, BitsAllocated, BitsStored,
				                              HighBit, PixelRepresentation != 0, pixelData);
			}
			else
			{
				if (PixelData == null)
				{
					return new ColorPixelData(Rows, Columns, new byte[Rows * Columns * 4]);
				}
				else
				{
					byte[] original = PixelData;
					byte[] argb = new byte[Rows * Columns * 4];
					ColorSpaceConverter.ToArgb(InternalPhotometricInterpretation, PlanarConfiguration, original, argb);
					return new ColorPixelData(Rows, Columns, argb);	
				}
			}
		}

		public void SetPixelData(PixelData pixeldata)
		{
			if (IsColor)
			{
				_internalPhotometricInterpretation = null;
				PhotometricInterpretation = "RGB";

				byte[] final = new byte[3 * Rows * Columns];
				ArgbToRgb(pixeldata.Raw, final);
				PixelData = final;
			}
			else
			{
				PixelData = pixeldata.Raw;
			}
		}

		private static unsafe void ArgbToRgb(byte[] source, byte[] destination)
		{
			fixed (byte* src = source)
			{
				fixed (byte* dst = destination)
				{
					byte* psrc = src;
					byte* pdst = dst;
					for (int i = 0; i < destination.Length / 3; ++i)
					{
						*pdst++ = *psrc++;
						*pdst++ = *psrc++;
						*pdst++ = *psrc++;
						psrc++;
					}
				}
			}
		}
	}
}