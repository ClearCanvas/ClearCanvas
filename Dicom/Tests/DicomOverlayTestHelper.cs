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

#if	UNIT_TESTS

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Tests
{
	/// <summary>
	/// Helper methods for creating test instances containing DICOM overlays.
	/// </summary>
	public static class DicomOverlayTestHelper
	{
		/// <summary>
		/// Sets the image pixel module on the given dataset.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="pixelData">The pixel data.</param>
		/// <param name="rows">The number of rows per frame.</param>
		/// <param name="columns">The number of columns per frame.</param>
		/// <param name="frames">The number of frames.</param>
		/// <param name="bitsAllocated">The bits allocated per pixel.</param>
		/// <param name="bitsStored">The bits stored per pixel.</param>
		/// <param name="highBit">The position of the high bit in each pixel allocation.</param>
		/// <param name="isSigned">Whether or not pixels are signed.</param>
		/// <exception cref="ArgumentException">Thrown if input values are inconsistent.</exception>
		public static void SetImagePixels(IDicomAttributeProvider dataset, byte[] pixelData, int rows, int columns, int frames, int bitsAllocated, int bitsStored, int highBit, bool isSigned)
		{
			// sanity check
			Platform.CheckForNullReference(dataset, "dataset");
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckArgumentRange(bitsStored, 1, 16, "bitsStored");
			Platform.CheckTrue(highBit >= bitsStored - 1 && highBit <= 15, "highBit must be between bitsStored-1 and 15, inclusive");
			Platform.CheckTrue(bitsAllocated == 8 || bitsAllocated == 16, "bitsAllocated must be 8 or 16");
			Platform.CheckTrue(rows*columns*frames*(bitsAllocated/8) == pixelData.Length, "pixelData must have length equal to rows*columns*frames*bitsAllocated/8");

			// set attributes
			dataset[DicomTags.Rows].SetInt32(0, rows);
			dataset[DicomTags.Columns].SetInt32(0, columns);
			dataset[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
			dataset[DicomTags.BitsStored].SetInt32(0, bitsStored);
			dataset[DicomTags.HighBit].SetInt32(0, highBit);
			dataset[DicomTags.SamplesPerPixel].SetInt32(0, 1);
			dataset[DicomTags.PlanarConfiguration].SetInt32(0, 0);
			dataset[DicomTags.PixelRepresentation].SetInt32(0, isSigned ? 1 : 0);
			dataset[DicomTags.NumberOfFrames].SetInt32(0, frames);
			dataset[DicomTags.PhotometricInterpretation].SetStringValue(PhotometricInterpretation.Monochrome2.Code);
			dataset[FixVR(DicomTags.PixelData, bitsAllocated == 16 ? DicomVr.OWvr : DicomVr.OBvr)].Values = pixelData;
		}

		/// <summary>
		/// Sets an overlay plane module on the given dataset using the OverlayData attribute.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="overlayIndex">The index of the overlay plane.</param>
		/// <param name="overlayData">The overlay data.</param>
		/// <param name="type">The overlay type.</param>
		/// <param name="origin">The overlay origin.</param>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="bigEndian">Whether or not the dataset is big endian.</param>
		/// <param name="useOW">Whether or not OverlayData should have a VR of OW.</param>
		/// <exception cref="ArgumentException">Thrown if input values are inconsistent.</exception>
		public static void AddOverlayPlane(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, bool bigEndian, bool useOW)
		{
			AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, rows, columns, null, null, bigEndian, useOW);
		}

		/// <summary>
		/// Sets an overlay plane module on the given dataset using the OverlayData attribute.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="overlayIndex">The index of the overlay plane.</param>
		/// <param name="overlayData">The overlay data.</param>
		/// <param name="type">The overlay type.</param>
		/// <param name="origin">The overlay origin per frame.</param>
		/// <param name="rows">The number of rows per frame.</param>
		/// <param name="columns">The number of columns per frame.</param>
		/// <param name="frames">The number of frames. If NULL, will not set multiframe attributes.</param>
		/// <param name="bigEndian">Whether or not the dataset is big endian.</param>
		/// <param name="useOW">Whether or not OverlayData should have a VR of OW.</param>
		/// <exception cref="ArgumentException">Thrown if input values are inconsistent.</exception>
		public static void AddOverlayPlane(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, int? frames, bool bigEndian, bool useOW)
		{
			AddOverlayPlane(dataset, overlayIndex, overlayData, type, origin, rows, columns, frames, null, bigEndian, useOW);
		}

		/// <summary>
		/// Sets an overlay plane module on the given dataset using the OverlayData attribute.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="overlayIndex">The index of the overlay plane.</param>
		/// <param name="overlayData">The overlay data.</param>
		/// <param name="type">The overlay type.</param>
		/// <param name="origin">The overlay origin per frame.</param>
		/// <param name="rows">The number of rows per frame.</param>
		/// <param name="columns">The number of columns per frame.</param>
		/// <param name="frames">The number of frames. If NULL, will not set multiframe attributes.</param>
		/// <param name="frameOrigin">The number of the image frame that matches the first overlay frame. If NULL, the attribute is not included and the reader is supposed to assume 1.</param>
		/// <param name="bigEndian">Whether or not the dataset is big endian.</param>
		/// <param name="useOW">Whether or not OverlayData should have a VR of OW.</param>
		/// <exception cref="ArgumentException">Thrown if input values are inconsistent.</exception>
		public static void AddOverlayPlane(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int rows, int columns, int? frames, int? frameOrigin, bool bigEndian, bool useOW)
		{
			// sanity check
			Platform.CheckForNullReference(dataset, "dataset");
			Platform.CheckForNullReference(overlayData, "overlayData");
			Platform.CheckArgumentRange(overlayIndex, 0, 15, "overlayIndex");
			Platform.CheckTrue(rows*columns*frames.GetValueOrDefault(1) == overlayData.Length, "overlayData must have length equal to rows*columns*frames");

			uint tagOffset = ((uint) overlayIndex)*2*0x10000;

			// set basic attributes
			dataset[tagOffset + DicomTags.OverlayRows].SetInt32(0, rows);
			dataset[tagOffset + DicomTags.OverlayColumns].SetInt32(0, columns);
			dataset[tagOffset + DicomTags.OverlayType].SetString(0, type.ToString());
			dataset[tagOffset + DicomTags.OverlayOrigin].SetStringValue(string.Format(@"{0}\{1}", origin.X, origin.Y));
			dataset[tagOffset + DicomTags.OverlayBitsAllocated].SetInt32(0, 1);
			dataset[tagOffset + DicomTags.OverlayBitPosition].SetInt32(0, 0);

			// set multiframe attributes
			if (frames.HasValue)
			{
				dataset[tagOffset + DicomTags.NumberOfFramesInOverlay].SetInt32(0, frames.Value);
				if (frameOrigin.HasValue)
					dataset[tagOffset + DicomTags.ImageFrameOrigin].SetInt32(0, frameOrigin.Value);
				else
					dataset[tagOffset + DicomTags.ImageFrameOrigin].SetEmptyValue();
			}
			else
			{
				dataset[tagOffset + DicomTags.NumberOfFramesInOverlay].SetEmptyValue();
				dataset[tagOffset + DicomTags.ImageFrameOrigin].SetEmptyValue();
			}

			// set overlay data by bit packing
			var packedBitsLength = (int) Math.Ceiling(overlayData.Length/8f);
			var packedBits = new byte[packedBitsLength + (packedBitsLength%2)];
			if (useOW)
			{
				var highByte = bigEndian ? 0 : 1; // in a big endian word, the high byte is first
				var cursor = 0;
				ushort window = 0;
				for (int n = 0; n < overlayData.Length; n++)
				{
					window = (ushort) (((window >> 1) & 0x007FFF) | (overlayData[n] ? 0x008000 : 0x000000));
					if ((n + 1)%16 == 0)
					{
						packedBits[cursor + highByte] = (byte) ((window >> 8) & 0x00FF);
						packedBits[cursor + 1 - highByte] = (byte) (window & 0x00FF);
						cursor += 2;
						window = 0;
					}
				}

				// flush the last window
				if (cursor == packedBits.Length - 1)
				{
					packedBits[cursor + highByte] = (byte) ((window >> 8) & 0x00FF);
					packedBits[cursor + 1 - highByte] = (byte) (window & 0x00FF);
				}
			}
			else
			{
				var cursor = 0;
				byte window = 0;
				for (int n = 0; n < overlayData.Length; n++)
				{
					window = (byte) (((window >> 1) & 0x007F) | (overlayData[n] ? 0x0080 : 0x0000));
					if ((n + 1)%8 == 0)
					{
						packedBits[cursor++] = window;
						window = 0;
					}
				}

				// flush the last window
				if (cursor == packedBits.Length - 1)
					packedBits[cursor] = window;
			}
			dataset[FixVR(tagOffset + DicomTags.OverlayData, useOW ? DicomVr.OWvr : DicomVr.OBvr)].Values = packedBits;
		}

		/// <summary>
		/// Sets an overlay plane module on the given dataset by embedding the overlay in an unused bit of the Pixel Data.
		/// </summary>
		/// <remarks>
		/// Call <see cref="SetImagePixels"/> before calling this method as it updates the current PixelData in the dataset.
		/// </remarks>
		/// <param name="dataset">The dataset.</param>
		/// <param name="overlayIndex">The index of the overlay plane.</param>
		/// <param name="overlayData">The overlay data.</param>
		/// <param name="type">The overlay type.</param>
		/// <param name="origin">The overlay origin per frame.</param>
		/// <param name="bitPosition">The position of the unused bit in the PixelData where the overlay should be stored.</param>
		/// <param name="bigEndian">Whether or not the dataset is big endian.</param>
		/// <exception cref="ArgumentException">Thrown if input values are inconsistent.</exception>
		public static void AddOverlayPlane(IDicomAttributeProvider dataset, int overlayIndex, bool[] overlayData, OverlayType type, Point origin, int bitPosition, bool bigEndian)
		{
			Platform.CheckForNullReference(dataset, "dataset");

			var rows = dataset[DicomTags.Rows].GetInt32(0, 0);
			var columns = dataset[DicomTags.Columns].GetInt32(0, 0);
			var frames = dataset[DicomTags.NumberOfFrames].GetInt32(0, 1);
			var bitsAllocated = dataset[DicomTags.BitsAllocated].GetInt32(0, 16);
			var bitsStored = dataset[DicomTags.BitsStored].GetInt32(0, 16);
			var highBit = dataset[DicomTags.HighBit].GetInt32(0, bitsStored - 1);
			var pixelData = (byte[]) dataset[DicomTags.PixelData].Values;

			// sanity check
			Platform.CheckForNullReference(overlayData, "overlayData");
			Platform.CheckArgumentRange(overlayIndex, 0, 15, "overlayIndex");
			Platform.CheckTrue(bitPosition >= 0 && bitPosition <= 15 && (bitPosition > highBit || bitPosition <= highBit - bitsStored), "bitPosition must specify an unused bit");
			Platform.CheckTrue(rows*columns*frames == overlayData.Length, "overlayData must have length equal to rows*columns*frames");
			Platform.CheckArgumentRange(bitsStored, 1, 16, "bitsStored");
			Platform.CheckTrue(highBit >= bitsStored - 1 && highBit <= 15, "highBit must be between bitsStored-1 and 15, inclusive");
			Platform.CheckTrue(bitsAllocated == 8 || bitsAllocated == 16, "bitsAllocated must be 8 or 16");
			Platform.CheckTrue(rows*columns*frames*(bitsAllocated/8) == pixelData.Length, "pixelData must have length equal to rows*columns*frames*bitsAllocated/8");

			uint tagOffset = ((uint) overlayIndex)*2*0x10000;

			// set basic attributes
			dataset[tagOffset + DicomTags.OverlayRows].SetInt32(0, rows);
			dataset[tagOffset + DicomTags.OverlayColumns].SetInt32(0, columns);
			dataset[tagOffset + DicomTags.OverlayType].SetString(0, type.ToString());
			dataset[tagOffset + DicomTags.OverlayOrigin].SetStringValue(string.Format(@"{0}\{1}", origin.X, origin.Y));
			dataset[tagOffset + DicomTags.OverlayBitsAllocated].SetInt32(0, bitsAllocated);
			dataset[tagOffset + DicomTags.OverlayBitPosition].SetInt32(0, bitPosition);

			// set multiframe attributes
			if (frames > 1)
				dataset[tagOffset + DicomTags.NumberOfFramesInOverlay].SetInt32(0, frames);
			else
				dataset[tagOffset + DicomTags.NumberOfFramesInOverlay].SetEmptyValue();
			dataset[tagOffset + DicomTags.ImageFrameOrigin].SetEmptyValue();

			// set overlay in unused bits of pixel data
			if (bitsAllocated == 16)
			{
				var highByte = bigEndian ? 0 : 1; // in a big endian word, the high byte is first
				var overlayBitMaskLow = (byte) (bitPosition <= 7 ? (1 << bitPosition) : 0);
				var overlayBitMaskHigh = (byte) (bitPosition > 7 ? (1 << (bitPosition - 8)) : 0);
				for (int i = 0; i < overlayData.Length; i++)
				{
					var cursor = i*2;
					pixelData[cursor + highByte] = (byte) ((pixelData[cursor + highByte] & ~overlayBitMaskHigh) | (overlayData[i] ? overlayBitMaskHigh : (byte) 0));
					pixelData[cursor + 1 - highByte] = (byte) ((pixelData[cursor + 1 - highByte] & ~overlayBitMaskLow) | (overlayData[i] ? overlayBitMaskLow : (byte) 0));
				}
			}
			else if (bitsAllocated == 8)
			{
				var overlayBitMask = (byte) (1 << bitPosition);
				for (int i = 0; i < overlayData.Length; i++)
				{
					pixelData[i] = (byte) ((pixelData[i] & ~overlayBitMask) | (overlayData[i] ? overlayBitMask : (byte) 0));
				}
			}
			dataset[DicomTags.PixelData].Values = pixelData;
		}

		/// <summary>
		/// Fixes the VR of the specified DICOM tag, which is especially important for multi-VR tags.
		/// </summary>
		private static DicomTag FixVR(uint tag, DicomVr newVR)
		{
			var dicomTag = DicomTagDictionary.GetDicomTag(tag);
			if (dicomTag.VR == newVR)
				return dicomTag;
			if (!dicomTag.MultiVR)
				throw new ArgumentException("The specified DICOM tag does not support multiple VRs.", "tag");
			return new DicomTag(dicomTag.TagValue, dicomTag.Name, dicomTag.VariableName, newVR, true, dicomTag.VMLow, dicomTag.VMHigh, dicomTag.Retired);
		}
	}
}

#endif