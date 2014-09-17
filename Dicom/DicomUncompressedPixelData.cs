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
using System.Drawing;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Class representing uncompressed pixel data.
	/// </summary>
	public class DicomUncompressedPixelData : DicomPixelData
	{
		#region Private Members

		private readonly List<FrameData> _fd = new List<FrameData>();
		private DicomAttribute _pd;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a <see cref="DicomUncompressedPixelData"/> from the attributes in a DICOM file/message.
		/// </summary>
		/// <param name="dicomMessage">A DICOM file/message from which to initialize the properties of the <see cref="DicomUncompressedPixelData"/>.</param>
		public DicomUncompressedPixelData(DicomMessageBase dicomMessage)
			: base(dicomMessage)
		{
			_pd = dicomMessage.DataSet[DicomTags.PixelData];
			InitializeFrameData(this, _pd);
		}

		/// <summary>
		/// Constructor from an <see cref="DicomAttributeCollection"/> instance.
		/// </summary>
		/// <param name="collection"></param>
		public DicomUncompressedPixelData(DicomAttributeCollection collection)
			: base(collection)
		{
			_pd = collection[DicomTags.PixelData];
			InitializeFrameData(this, _pd);
		}

		/// <summary>
		/// Contructor from a <see cref="DicomCompressedPixelData"/> instance.
		/// </summary>
		/// <param name="compressedPixelData"></param>
		public DicomUncompressedPixelData(DicomCompressedPixelData compressedPixelData)
			: base(compressedPixelData)
		{
			if (BitsAllocated > 8)
			{
				_pd = new DicomAttributeOW(DicomTags.PixelData);
			}
			else
			{
				var pdTag = DicomTagDictionary.GetDicomTag(DicomTags.PixelData);
				var obTag = new DicomTag(DicomTags.PixelData, pdTag.Name, pdTag.VariableName, DicomVr.OBvr, pdTag.MultiVR, pdTag.VMLow, pdTag.VMHigh, pdTag.Retired);
				_pd = new DicomAttributeOB(obTag);
			}
			InitializeFrameData(this, _pd);
		}

		private void InitializeFrameData(DicomUncompressedPixelData owner, DicomAttribute pixelDataAttribute)
		{
			var obAttrib = pixelDataAttribute as DicomAttributeOB;
			if (obAttrib != null && obAttrib.StreamLength > 0)
			{
				for (var n = 0; n < NumberOfFrames; ++n)
					_fd.Add(new FrameDataOB(owner, obAttrib, n));
			}

			var owAttrib = pixelDataAttribute as DicomAttributeOW;
			if (owAttrib != null && owAttrib.StreamLength > 0)
			{
				for (var n = 0; n < NumberOfFrames; ++n)
					_fd.Add(new FrameDataOW(owner, owAttrib, n));
			}
		}

		#endregion

		#region Internal Methods

		internal void UpdatePixelDataAttribute()
		{
			var frameSize = UncompressedFrameSize;
			var frameCount = NumberOfFrames;
			var padding = ((frameCount*frameSize) & 1) == 1 ? 1 : 0;

			using (var stream = ((DicomAttributeBinary) _pd).AsStream())
			{
				stream.Position = 0;
				foreach (var frameData in _fd)
				{
					stream.Write(frameData.GetFrame(), 0, frameSize);
				}
				stream.SetLength(frameCount*frameSize + padding);
			}
		}

		internal byte[] GetData()
		{
			if (_fd.Count == 0) return null;
			return _fd[0].GetFrame();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Update a <see cref="DicomAttributeCollection"/> with the pixel data contained
		/// within this object and also update pixel data related tags.
		/// </summary>
		/// <remarks>
		/// This method will replace the pixel data attribute in <paramref name="dataset"/>
		/// and update other pixel data related tags within the collection.
		/// </remarks>
		/// <param name="dataset">The collection to update.</param>
		public override void UpdateAttributeCollection(DicomAttributeCollection dataset)
		{
			dataset.SaveDicomFields(this);

			if (dataset.Contains(DicomTags.NumberOfFrames) || NumberOfFrames > 1)
				dataset[DicomTags.NumberOfFrames].SetInt32(0, NumberOfFrames);
			if (dataset.Contains(DicomTags.PlanarConfiguration))
				dataset[DicomTags.PlanarConfiguration].SetInt32(0, PlanarConfiguration);
			if (dataset.Contains(DicomTags.LossyImageCompressionRatio))
				dataset[DicomTags.LossyImageCompressionRatio].SetFloat32(0, LossyImageCompressionRatio);
			if (dataset.Contains(DicomTags.LossyImageCompressionMethod))
				dataset[DicomTags.LossyImageCompressionMethod].SetString(0, LossyImageCompressionMethod);
			if (dataset.Contains(DicomTags.RescaleSlope) || DecimalRescaleSlope != 1.0M || DecimalRescaleIntercept != 0.0M)
				dataset[DicomTags.RescaleSlope].SetString(0, RescaleSlope);
			if (dataset.Contains(DicomTags.RescaleIntercept) || DecimalRescaleSlope != 1.0M || DecimalRescaleIntercept != 0.0M)
				dataset[DicomTags.RescaleIntercept].SetString(0, RescaleIntercept);

			if (dataset.Contains(DicomTags.WindowCenter) || LinearVoiLuts.Count > 0)
				Window.SetWindowCenterAndWidth(dataset, LinearVoiLuts);

			dataset[DicomTags.PixelData] = _pd;

			//Remove the palette color lut, if the pixels were translated to RGB
			if (dataset.Contains(DicomTags.RedPaletteColorLookupTableData)
			    && dataset.Contains(DicomTags.BluePaletteColorLookupTableData)
			    && dataset.Contains(DicomTags.GreenPaletteColorLookupTableData)
			    && !HasPaletteColorLut)
			{
				dataset.RemoveAttribute(DicomTags.BluePaletteColorLookupTableDescriptor);
				dataset.RemoveAttribute(DicomTags.BluePaletteColorLookupTableData);
				dataset.RemoveAttribute(DicomTags.RedPaletteColorLookupTableDescriptor);
				dataset.RemoveAttribute(DicomTags.RedPaletteColorLookupTableData);
				dataset.RemoveAttribute(DicomTags.GreenPaletteColorLookupTableDescriptor);
				dataset.RemoveAttribute(DicomTags.GreenPaletteColorLookupTableData);
			}

			UpdatePixelDataAttribute();
		}

		/// <summary>
		/// Update a <see cref="DicomMessageBase"/> with the pixel data contained
		/// within this object and also update pixel data related tags.
		/// </summary>
		/// <param name="message"></param>
		public override void UpdateMessage(DicomMessageBase message)
		{
			UpdateAttributeCollection(message.DataSet);
			DicomFile file = message as DicomFile;
			if (file != null)
				file.TransferSyntax = TransferSyntax;
		}

		/// <summary>
		/// Appends a frame to the pixel data.
		/// </summary>
		/// <param name="frameData">The frame data to append.</param>
		public void AppendFrame(byte[] frameData)
		{
			_fd.Add(new FrameDataBytes(this, frameData));
		}

		public void InsertFrame(int frame, byte[] frameData)
		{
			_fd.Insert(frame, new FrameDataBytes(this, frameData));
		}

		public void RemoveFrame(int frame)
		{
			_fd.RemoveAt(frame);
		}

		/// <summary>
		/// Gets a specific uncompressed frame.
		/// </summary>
		/// <param name="frame">The zero-offset index of the frame to be retrieved.</param>
		/// <param name="photometricInterpretation">The photometric interpretation of the pixel data.</param>
		/// <returns>A byte array containing the frame data.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frame"/> specifies an invalid frame index.</exception>
		/// <exception cref="DicomDataException">Thrown if there was a problem extracting the specified frame from the pixel data buffer.</exception>
		public override byte[] GetFrame(int frame, out string photometricInterpretation)
		{
			if (frame >= NumberOfFrames || frame < 0)
				throw new ArgumentOutOfRangeException("frame");

			photometricInterpretation = PhotometricInterpretation;

			if (frame >= _fd.Count)
				throw new DicomDataException("Requested frame exceeds available pixel data in buffer.");

			return _fd[frame].GetFrame();
		}

		/// <summary>
		/// Replace a specific uncompressed frame.
		/// </summary>
		/// <param name="frame">The zero-offset index of the frame to be replaced.</param>
		/// <param name="frameData">Frame data to replace the current data</param>
		/// <returns>A byte array containing the frame data.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frame"/> specifies an invalid frame index.</exception>
		/// <exception cref="DicomDataException">Thrown if there was a problem extracting the specified frame from the pixel data buffer.</exception>
		public void SetFrame(int frame, byte[] frameData)
		{
			if (frame >= NumberOfFrames || frame < 0)
				throw new ArgumentOutOfRangeException("frame");

			if (frame >= _fd.Count)
				throw new DicomDataException("Requested frame exceeds available pixel data in buffer.");

			_fd[frame] = new FrameDataBytes(this, frameData);
		}

		[Obsolete("Renamed to DicomUncompressedPixelData.SetFrame(int,byte[]).")]
		public void ReplaceFrame(int frame, byte[] frameData)
		{
			SetFrame(frame, frameData);
		}

		public void ConvertPaletteColorToRgb()
		{
			Platform.CheckTrue(PhotometricInterpretation.Equals("PALETTE COLOR"), "Photometric Interpretation Palette Color Check");

			List<FrameData> frames = new List<FrameData>();

			for (int i = 0; i < NumberOfFrames; i++)
			{
				byte[] currentFrame = GetFrame(i);
				byte[] newFrame = new byte[UncompressedFrameSize*3];

				PaletteColorToRgb(BitsAllocated, IsSigned, currentFrame, newFrame, PaletteColorLut);
				frames.Add(new FrameDataBytes(this, newFrame, false));
			}

			// change the Pixel Data attribute so we don't affect the original
			_pd = _pd.Tag.VR.CreateDicomAttribute(_pd.Tag);

			_fd.Clear();
			_fd.AddRange(frames);

			SamplesPerPixel = 3;
			PhotometricInterpretation = "RGB";
			PlanarConfiguration = 0;
			HasPaletteColorLut = false;
		}

		#endregion

		#region Static Methods

		#region Toggle Methods

		/// <summary>
		/// Toggle the planar configuration of a pixel data array
		/// </summary>
		/// <param name="pixelData"></param>
		/// <param name="numValues"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="samplesPerPixel"></param>
		/// <param name="oldPlanerConfiguration"></param>
		public static void TogglePlanarConfiguration(byte[] pixelData, int numValues, int bitsAllocated, int samplesPerPixel, int oldPlanerConfiguration)
		{
			int bytesAllocated = bitsAllocated/8;
			int numPixels = numValues/samplesPerPixel;
			if (bytesAllocated == 1)
			{
				byte[] buffer = new byte[pixelData.Length];
				if (oldPlanerConfiguration == 1)
				{
					for (int n = 0; n < numPixels; n++)
					{
						for (int s = 0; s < samplesPerPixel; s++)
						{
							buffer[n*samplesPerPixel + s] = pixelData[n + numPixels*s];
						}
					}
				}
				else
				{
					for (int n = 0; n < numPixels; n++)
					{
						for (int s = 0; s < samplesPerPixel; s++)
						{
							buffer[n + numPixels*s] = pixelData[n*samplesPerPixel + s];
						}
					}
				}
				Array.Copy(buffer, 0, pixelData, 0, numValues);
			}
			else if (bytesAllocated == 2)
			{
				throw new DicomCodecUnsupportedSopException(String.Format("BitsAllocated={0} is not supported!", bitsAllocated));
			}
			else
				throw new DicomCodecUnsupportedSopException(String.Format("BitsAllocated={0} is not supported!", bitsAllocated));
		}

		/// <summary>
		/// Toggle the pixel representation of a frame
		/// </summary>
		public static unsafe void TogglePixelRepresentation(byte[] frameData, int highBit, int bitsStored, int bitsAllocated, out int rescale)
		{
			if ((bitsAllocated != 8 && bitsAllocated != 16) || bitsStored > bitsAllocated)
				throw new DicomCodecUnsupportedSopException("Invalid bits allocated/stored value(s).");

			int highestBit = bitsAllocated - 1;
			const int lowestBit = 0;
			if (highBit > highestBit || highBit < lowestBit)
				throw new ArgumentException(String.Format("Invalid high bit for {0}-bit pixel data ({1}).", bitsAllocated, highBit));

			int bytesAllocated = bitsAllocated/8;
			int numValues = frameData.Length/bytesAllocated;

			int unusedHighBitsCount = highestBit - highBit;
			int unusedLowBitsCount = GetLowBit(bitsStored, highBit);

			int signShift = unusedHighBitsCount;
			int rightAlignShift = unusedLowBitsCount + unusedHighBitsCount;

			rescale = -GetMinPixelValue(bitsStored, true);

			if (bitsStored < 8 && bitsAllocated <= 8)
			{
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = pFrameData;
					for (int p = 0; p < numValues; p++, pixelData++)
					{
						int pixel = ((sbyte) (*pixelData << signShift)) >> rightAlignShift;
						*pixelData = (byte) (pixel + rescale);
					}
				}
			}
			else if (bitsStored == 8 && bitsAllocated <= 8)
			{
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = pFrameData;
					for (int p = 0; p < numValues; p++, pixelData++)
					{
						int pixel = (sbyte) (*pixelData);
						*pixelData = (byte) (pixel + rescale);
					}
				}
			}
			else if (bitsStored < 16)
			{
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = (ushort*) pFrameData;
					for (int p = 0; p < numValues; p++, pixelData++)
					{
						int pixel = ((short) (*pixelData << signShift)) >> rightAlignShift;
						*pixelData = (ushort) (pixel + rescale);
					}
				}
			}
			else
			{
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = (ushort*) pFrameData;
					for (int p = 0; p < numValues; p++, pixelData++)
					{
						int pixel = (short) (*pixelData);
						*pixelData = (ushort) (pixel + rescale);
					}
				}
			}
		}

		/// <summary>
		/// Change a 8-bit pixel data to a 16-bit buffer
		/// </summary>
		public static unsafe byte[] ToggleBitDepth(byte[] frameData, int length, int uncompressedFrameSize, int bitsStored, int bitsAllocated)
		{
			if ((bitsStored != 8 || bitsAllocated != 16))
				throw new DicomCodecUnsupportedSopException("Invalid bits allocated/stored value(s).");

			if (length == uncompressedFrameSize)
			{
				var b = new byte[length/2];

				fixed (byte* destFrameData = b)
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = (ushort*) pFrameData;
					var destData = destFrameData;

					for (int p = 0; p < length/2; p++, pixelData++, destData++)
					{
						int pixel = (byte) (*pixelData);
						*destData = (byte) (pixel);
					}
				}

				return b;
			}
			else
			{
				var b = new byte[length*2];

				fixed (byte* destFrameData = b)
				fixed (byte* pFrameData = frameData)
				{
					var pixelData = pFrameData;
					var destData = (ushort*) destFrameData;

					for (int p = 0; p < length; p++, pixelData++, destData++)
					{
						int pixel = (sbyte) (*pixelData);
						*destData = (byte) (pixel);
					}
				}

				return b;
			}
		}

		#endregion

		#region Unused Bit Masking

		internal static bool ZeroUnusedBits(Stream stream, int bitsAllocated, int bitsStored, int highBit)
		{
			return ZeroUnusedBits(stream, bitsAllocated, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		internal static unsafe bool ZeroUnusedBits(Stream stream, int bitsAllocated, int bitsStored, int highBit, Endian endian)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException(String.Format("Invalid value for Bits Allocated ({0})", bitsAllocated));

			stream.Position = 0;

			var buffer = new byte[4096];
			var anyChanged = false;
			int bytesRead;

			if (bitsAllocated == 8)
			{
				while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0)
				{
					var result = ZeroUnusedBits(buffer, bitsStored, highBit);
					stream.Seek(-bytesRead, SeekOrigin.Current);
					stream.Write(buffer, 0, bytesRead);
					if (!anyChanged && result)
						anyChanged = true;
				}
			}
			else
			{
				while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0)
				{
					bool result;
					fixed (byte* p = buffer)
						result = ZeroUnusedBits((ushort*) p, bitsStored, highBit, buffer.Length/2, endian);
					stream.Seek(-bytesRead, SeekOrigin.Current);
					stream.Write(buffer, 0, bytesRead);
					if (!anyChanged && result)
						anyChanged = true;
				}
			}
			return anyChanged;
		}

		/// <summary>
		/// Masks
		/// </summary>
		/// <param name="pixelData"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <returns></returns>
		public static bool ZeroUnusedBits(byte[] pixelData, int bitsAllocated, int bitsStored, int highBit)
		{
			return ZeroUnusedBits(pixelData, bitsAllocated, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		public static unsafe bool ZeroUnusedBits(byte[] pixelData, int bitsAllocated, int bitsStored, int highBit, Endian endian)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException(String.Format("Invalid value for Bits Allocated ({0})", bitsAllocated));

			if (bitsAllocated == 8)
				return ZeroUnusedBits(pixelData, bitsStored, highBit);

			fixed (byte* p = pixelData)
				return ZeroUnusedBits((ushort*) p, bitsStored, highBit, pixelData.Length/2, endian);
		}

		public static unsafe bool ZeroUnusedBits(byte[] pixelData, int bitsStored, int highBit)
		{
			fixed (byte* ptr = pixelData)
				return ZeroUnusedBits(ptr, bitsStored, highBit, pixelData.Length);
		}

		public static bool ZeroUnusedBits(ushort[] pixelData, int bitsStored, int highBit)
		{
			return ZeroUnusedBits(pixelData, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		public static unsafe bool ZeroUnusedBits(ushort[] pixelData, int bitsStored, int highBit, Endian endian)
		{
			fixed (ushort* ptr = pixelData)
				return ZeroUnusedBits(ptr, bitsStored, highBit, pixelData.Length, endian);
		}

		private static unsafe bool ZeroUnusedBits(byte* pixelData, int bitsStored, int highBit, int length)
		{
			const int bitsAllocated = 8;
			if (bitsStored > bitsAllocated)
				throw new ArgumentException(String.Format("Bits stored cannot be greater than bits allocated ({0} > {1}).", bitsStored, bitsAllocated));

			const int highestBit = 7;
			const int lowestBit = 0;
			if (highBit > highestBit || highBit < lowestBit)
				throw new ArgumentException(String.Format("Invalid high bit for 8-bit pixel data ({0}).", highBit));

			const byte noOpMask = 0xFF;
			int unusedHighBitsCount = highestBit - highBit;
			int unusedLowBitsCount = GetLowBit(bitsStored, highBit);
			int unusedBitsCount = unusedLowBitsCount + unusedHighBitsCount;
			var mask = (byte) ((noOpMask >> unusedBitsCount) << unusedLowBitsCount);

			var anyChanged = false;
			var p = pixelData;
			for (int i = 0; i < length; ++i, ++p)
			{
				//Optimization: don't make unnecessary assignments.
				var vOld = *p;
				var vNew = vOld;
				vNew &= mask;
				if (vOld == vNew)
					continue;

				*p = vNew;
				if (!anyChanged)
					anyChanged = true;
			}

			// Since most pixel data is "good" and doesn't have anything in the extra bits,
			// this will usually be false.  We return this value so the caller can avoid
			// replacing a frame (for example) unnecessarily.
			return anyChanged;
		}

		private static unsafe bool ZeroUnusedBits(ushort* pixelData, int bitsStored, int highBit, int length, Endian endian)
		{
			const int bitsAllocated = 16;
			if (bitsStored > bitsAllocated)
				throw new ArgumentException(String.Format("Bits stored cannot be greater than bits allocated ({0} > {1}).", bitsStored, bitsAllocated));

			const int highestBit = 15;
			const int lowestBit = 0;
			if (highBit > highestBit || highBit < lowestBit)
				throw new ArgumentException(String.Format("Invalid high bit for 16-bit pixel data ({0}).", highBit));

			const ushort noOpMask = 0xFFFF;
			int unusedHighBitsCount = highestBit - highBit;
			int unusedLowBitsCount = GetLowBit(bitsStored, highBit);
			int unusedBitsCount = unusedLowBitsCount + unusedHighBitsCount;
			var mask = (ushort) ((noOpMask >> unusedBitsCount) << unusedLowBitsCount);

			if (endian != ByteBuffer.LocalMachineEndian) //swap the mask rather than each value.
				mask = unchecked((ushort) ((mask << 8) | (mask >> 8)));

			var anyChanged = false;
			var p = pixelData;
			for (int i = 0; i < length; ++i, ++p)
			{
				//Optimization: don't make unnecessary assignments.
				var vOld = *p;
				var vNew = vOld;
				vNew &= mask;
				if (vOld == vNew)
					continue;

				*p = vNew;
				if (!anyChanged)
					anyChanged = true;
			}

			// Since most pixel data is "good" and doesn't have anything in the extra bits,
			// this will usually be false.  We return this value so the caller can avoid
			// replacing a frame (for example) unnecessarily.
			return anyChanged;
		}

		#endregion

		#region Right Align Pixel Data

		internal static bool RightAlign(Stream stream, int bitsAllocated, int bitsStored, int highBit)
		{
			return RightAlign(stream, bitsAllocated, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		internal static unsafe bool RightAlign(Stream stream, int bitsAllocated, int bitsStored, int highBit, Endian endian)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException(String.Format("Invalid value for Bits Allocated ({0})", bitsAllocated));

			stream.Position = 0;

			var buffer = new byte[4096];
			var anyChanged = false;
			int bytesRead;

			if (bitsAllocated == 8)
			{
				while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0)
				{
					var result = RightAlign(buffer, bitsStored, highBit);
					stream.Seek(-bytesRead, SeekOrigin.Current);
					stream.Write(buffer, 0, bytesRead);
					if (!anyChanged && result)
						anyChanged = true;
				}
			}
			else
			{
				while ((bytesRead = stream.Read(buffer, 0, 4096)) > 0)
				{
					bool result;
					fixed (byte* p = buffer)
						result = RightAlign((ushort*) p, bitsStored, highBit, buffer.Length/2, endian);
					stream.Seek(-bytesRead, SeekOrigin.Current);
					stream.Write(buffer, 0, bytesRead);
					if (!anyChanged && result)
						anyChanged = true;
				}
			}
			return anyChanged;
		}

		public static bool RightAlign(byte[] pixelData, int bitsAllocated, int bitsStored, int highBit)
		{
			return RightAlign(pixelData, bitsAllocated, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		public static unsafe bool RightAlign(byte[] pixelData, int bitsAllocated, int bitsStored, int highBit, Endian endian)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException(String.Format("Invalid value for Bits Allocated ({0})", bitsAllocated));

			if (bitsAllocated == 8)
				return RightAlign(pixelData, bitsStored, highBit);

			fixed (byte* p = pixelData)
				return RightAlign((ushort*) p, bitsStored, highBit, pixelData.Length/2, endian);
		}

		public static unsafe bool RightAlign(byte[] pixelData, int bitsStored, int highBit)
		{
			fixed (byte* ptr = pixelData)
				return RightAlign(ptr, bitsStored, highBit, pixelData.Length);
		}

		public static bool RightAlign(ushort[] pixelData, int bitsStored, int highBit)
		{
			return RightAlign(pixelData, bitsStored, highBit, ByteBuffer.LocalMachineEndian);
		}

		public static unsafe bool RightAlign(ushort[] pixelData, int bitsStored, int highBit, Endian endian)
		{
			fixed (ushort* ptr = pixelData)
				return RightAlign(ptr, bitsStored, highBit, pixelData.Length, endian);
		}

		private static unsafe bool RightAlign(byte* pixelData, int bitsStored, int highBit, int length)
		{
			const int highestBit = 7;
			const int lowestBit = 0;
			if (highBit > highestBit || highBit < lowestBit)
				throw new ArgumentException(String.Format("Invalid high bit for 8-bit pixel data ({0}).", highBit));

			int unusedLowBitsCount = GetLowBit(bitsStored, highBit);
			if (unusedLowBitsCount == 0)
				return false;

			var anyChanged = false;
			var p = pixelData;
			for (int i = 0; i < length; ++i, ++p)
			{
				var value = *p;
				if (value == 0) //Optimization - there are a lot of zeros in pixel data, so we skip the assignment.
					continue;

				value >>= unusedLowBitsCount;
				*p = value;
				if (!anyChanged)
					anyChanged = true;
			}

			return anyChanged;
		}

		private static unsafe bool RightAlign(ushort* pixelData, int bitsStored, int highBit, int length, Endian endian)
		{
			const int highestBit = 15;
			const int lowestBit = 0;
			if (highBit > highestBit || highBit < lowestBit)
				throw new ArgumentException(String.Format("Invalid high bit for 16-bit pixel data ({0}).", highBit));

			int unusedLowBitsCount = GetLowBit(bitsStored, highBit);
			if (unusedLowBitsCount == 0)
				return false;

			bool anyChanged = false;
			if (endian != ByteBuffer.LocalMachineEndian)
			{
				var p = (byte*) pixelData;
				var shiftBuffer = new byte[2];
				fixed (byte* pShiftBuffer = shiftBuffer)
				{
					var pShiftedValue = (ushort*) pShiftBuffer;
					for (int i = 0; i < length; ++i, p += 2)
					{
						//swap to machine representation before right-shifting.
						shiftBuffer[0] = p[1];
						shiftBuffer[1] = p[0];
						var value = *pShiftedValue;
						if (value == 0) //Optimization - there are a lot of zeros in pixel data, so we skip the assignment.
							continue;

						value >>= unusedLowBitsCount;
						*pShiftedValue = value;
						//assign shifted value back.
						p[0] = shiftBuffer[1];
						p[1] = shiftBuffer[0];
						if (!anyChanged)
							anyChanged = true;
					}
				}
			}
			else
			{
				var p = pixelData;
				for (int i = 0; i < length; ++i, ++p)
				{
					var value = *p;
					if (value == 0)
						continue;

					value >>= unusedLowBitsCount;
					*p = value;
					if (!anyChanged)
						anyChanged = true;
				}

				return anyChanged;
			}

			return true;
		}

		#endregion

		#region Normalize Pixel Data (Grayscale)

		public static unsafe void NormalizePixelData(byte[] pixelData, int bitsAllocated, int bitsStored, int highBit, bool isSigned)
		{
			var unusedLowBits = GetLowBit(bitsStored, highBit);
			var highestBit = bitsAllocated - 1;
			var unusedHighBits = highestBit - highBit;

			var leftShift = unusedHighBits;
			var rightShift = unusedHighBits + unusedLowBits;

			if (bitsAllocated == 16)
			{
				var length = pixelData.Length/2;
				fixed (byte* p = pixelData)
				{
					if (isSigned)
					{
						var pPixelData = (short*) p;
						for (int i = 0; i < length; ++i)
						{
							// sign-fill the unused bits by shifting it left, then shifting it back
							var value = *pPixelData;
							value <<= leftShift;
							value >>= rightShift;
							*pPixelData++ = value;
						}
					}
					else
					{
						var pPixelData = (ushort*) p;
						for (int i = 0; i < length; ++i)
						{
							// sign-fill the unused bits by shifting it left, then shifting it back
							var value = *pPixelData;
							value <<= leftShift;
							value >>= rightShift;
							*pPixelData++ = value;
						}
					}
				}
			}
			else
			{
				// 8-bit
				var length = pixelData.Length;
				fixed (byte* p = pixelData)
				{
					if (isSigned)
					{
						var pPixelData = (sbyte*) p;
						for (int i = 0; i < length; ++i)
						{
							// sign-fill the unused bits by shifting it left, then shifting it back
							var value = *pPixelData;
							value <<= leftShift;
							value >>= rightShift;
							*pPixelData++ = value;
						}
					}
					else
					{
						var pPixelData = p;
						for (int i = 0; i < length; ++i)
						{
							// sign-fill the unused bits by shifting it left, then shifting it back
							var value = *pPixelData;
							value <<= leftShift;
							value >>= rightShift;
							*pPixelData++ = value;
						}
					}
				}
			}
		}

		#endregion

		#region Palette Color to RGB Conversion

		/// <summary>
		/// Convert Palette Color pixel data to RGB.
		/// </summary>
		/// <param name="bitsAllocated"></param>
		/// <param name="isSigned"></param>
		/// <param name="srcPixelData"></param>
		/// <param name="rgbPixelData"></param>
		/// <param name="lut"></param>
		public static unsafe void PaletteColorToRgb(
			int bitsAllocated,
			bool isSigned,
			byte[] srcPixelData,
			byte[] rgbPixelData,
			PaletteColorLut lut)
		{
			Platform.CheckTrue(bitsAllocated == 8 || bitsAllocated == 16, "Valid Bits Allocated");
			Platform.CheckForNullReference(srcPixelData, "srcPixelData");
			Platform.CheckForNullReference(rgbPixelData, "rgbPixelData");
			Platform.CheckForNullReference(lut, "lut");

			int sizeInPixels = rgbPixelData.Length/3;

			const string messageInvalidBufferSize = "Invalid destination buffer size";
			if (bitsAllocated == 8 && 3*srcPixelData.Length != rgbPixelData.Length)
				throw new ArgumentException(messageInvalidBufferSize, "rgbPixelData");
			if (bitsAllocated > 8 && srcPixelData.Length/2 != sizeInPixels)
				throw new ArgumentException(messageInvalidBufferSize, "rgbPixelData");

			int firstPixelMapped = lut.FirstMappedPixelValue;

			fixed (byte* pSrcPixelData = srcPixelData)
			{
				fixed (byte* pRgbPixelData = rgbPixelData)
				{
					int dst = 0;

					if (bitsAllocated == 8)
					{
						if (isSigned)
						{
							// 8-bit signed
							for (int i = 0; i < sizeInPixels; i++)
							{
								Color value = lut.Data[((sbyte*) pSrcPixelData)[i] - firstPixelMapped];
								pRgbPixelData[dst] = value.R;
								pRgbPixelData[dst + 1] = value.G;
								pRgbPixelData[dst + 2] = value.B;

								dst += 3;
							}
						}
						else
						{
							// 8-bit unsigned
							for (int i = 0; i < sizeInPixels; i++)
							{
								Color value = lut.Data[pSrcPixelData[i] - firstPixelMapped];
								pRgbPixelData[dst] = value.R;
								pRgbPixelData[dst + 1] = value.G;
								pRgbPixelData[dst + 2] = value.B;

								dst += 3;
							}
						}
					}
					else
					{
						if (isSigned)
						{
							// 16-bit signed
							for (int i = 0; i < sizeInPixels; i++)
							{
								Color value = lut.Data[((short*) pSrcPixelData)[i] - firstPixelMapped];
								pRgbPixelData[dst] = value.R;
								pRgbPixelData[dst + 1] = value.G;
								pRgbPixelData[dst + 2] = value.B;

								dst += 3;
							}
						}
						else
						{
							// 16-bit unsinged
							for (int i = 0; i < sizeInPixels; i++)
							{
								Color value = lut.Data[((ushort*) pSrcPixelData)[i] - firstPixelMapped];
								pRgbPixelData[dst] = value.R;
								pRgbPixelData[dst + 1] = value.G;
								pRgbPixelData[dst + 2] = value.B;

								dst += 3;
							}
						}
					}
				}
			}
		}

		#endregion

		#region Color Space Conversion

		/// <summary>
		/// Converts a YBR_FULL value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFullToRgb(int y, int b, int r)
		{
			// |r|   | 1.0000  0.0000  1.4020 | | y       |
			// |g| = | 1.0000 -0.3441 -0.7141 | | b - 128 |
			// |b|   | 1.0000  1.7720 -0.0000 | | r - 128 |

			const int alpha = 0xff;
			int red = (int) (y + 1.4020*(r - 128) + 0.5);
			int green = (int) (y - 0.3441*(b - 128) - 0.7141*(r - 128) + 0.5);
			int blue = (int) (y + 1.7720*(b - 128) + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_FULL422 value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrFull422ToRgb(int y, int b, int r)
		{
			return YbrFullToRgb(y, b, r);
		}

		/// <summary>
		/// Converts a YBR_PARTIAL422 value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrPartial422ToRgb(int y, int b, int r)
		{
			// |r|   |  1.1644  0.0000  1.5960 | | y - 16  |
			// |g| = |  1.1644 -0.3917 -0.8130 | | b - 128 |
			// |b|   |  1.1644  2.0173  0.0000 | | r - 128 |

			const int alpha = 0xff;
			int red = (int) (1.1644*(y - 16) + 1.5960*(r - 128) + 0.5);
			int green = (int) (1.1644*(y - 16) - 0.3917*(b - 128) - 0.8130*(r - 128) + 0.5);
			int blue = (int) (1.1644*(y - 16) + 2.0173*(b - 128) + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_ICT value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrIctToRgb(int y, int b, int r)
		{
			// |r|   |  1.00000  0.00000  1.40200 | | y |
			// |g| = |  1.00000 -0.34412 -0.71414 | | b |
			// |b|   |  1.00000  1.77200  0.00000 | | r |

			const int alpha = 0xff;
			int red = (int) (y + 1.40200*r + 0.5);
			int green = (int) (y - 0.34412*b - 0.71414*r + 0.5);
			int blue = (int) (y + 1.77200*b + 0.5);

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		/// <summary>
		/// Converts a YBR_RCT value to RGB.
		/// </summary>
		/// <returns>A 32-bit ARGB value.</returns>
		public static int YbrRctToRgb(int y, int b, int r)
		{
			const int alpha = 0xff;
			int green = y - (r + b)/4;
			int red = r + green;
			int blue = b + green;

			Limit(ref red);
			Limit(ref green);
			Limit(ref blue);

			int argb = (alpha << 24) | (red << 16) | (green << 8) | blue;

			return argb;
		}

		#endregion

		private static void Limit(ref int color)
		{
			if (color < 0)
				color = 0;
			else if (color > 255)
				color = 255;
		}

		#endregion

		#region FrameData Classes

		/// <summary>
		/// Container representing frame data.
		/// </summary>
		private abstract class FrameData
		{
			private readonly DicomUncompressedPixelData _owner;

			/// <summary>
			/// Initializes the frame data container.
			/// </summary>
			protected FrameData(DicomUncompressedPixelData owner)
			{
				_owner = owner;
			}

			/// <summary>
			/// Gets the size in bytes of an uncompressed frame.
			/// </summary>
			protected int FrameSize
			{
				get { return _owner.UncompressedFrameSize; }
			}

			/// <summary>
			/// Gets the frame data, returning it in a new buffer.
			/// </summary>
			public byte[] GetFrame()
			{
				var buffer = new byte[FrameSize];
				GetFrame(buffer, 0);
				return buffer;
			}

			/// <summary>
			/// Gets the frame data, copying it to a given buffer at the specified offset.
			/// </summary>
			public abstract void GetFrame(byte[] dstBuffer, int dstOffset);
		}

		/// <summary>
		/// Container representing frame data as a byte buffer.
		/// </summary>
		private class FrameDataBytes : FrameData
		{
			private readonly byte[] _buffer;

			/// <summary>
			/// Initializes the frame data container from the specified byte buffer.
			/// </summary>
			public FrameDataBytes(DicomUncompressedPixelData owner, byte[] frameData, bool copyRequired = true)
				: base(owner)
			{
				if (copyRequired)
				{
					var buffer = new byte[frameData.Length];
					Array.Copy(frameData, buffer, buffer.Length);
					_buffer = buffer;
				}
				else
				{
					_buffer = frameData;
				}
			}

			public override void GetFrame(byte[] dstBuffer, int dstOffset)
			{
				Array.Copy(_buffer, 0, dstBuffer, dstOffset, FrameSize);
			}
		}

		/// <summary>
		/// Container representing frame data in an OB Pixel Data attribute.
		/// </summary>
		private class FrameDataOB : FrameData
		{
			private readonly DicomAttributeOB _obAttrib;
			private readonly int _frameIndex;

			/// <summary>
			/// Initializes the frame data container from an OB Pixel Data attribute.
			/// </summary>
			public FrameDataOB(DicomUncompressedPixelData owner, DicomAttributeOB pixelDataAttribute, int frameIndex)
				: base(owner)
			{
				_obAttrib = pixelDataAttribute;
				_frameIndex = frameIndex;
			}

			public override void GetFrame(byte[] dstBuffer, int dstOffset)
			{
				if (_obAttrib.StreamLength < (_frameIndex + 1)*FrameSize)
					throw new DicomDataException("Requested frame exceeds available pixel data in buffer.");

				if (_obAttrib.Reference != null)
				{
					ByteBuffer bb;
					using (var fs = _obAttrib.Reference.StreamOpener.Open())
					{
						long offset = _obAttrib.Reference.Offset + _frameIndex*FrameSize;
						fs.Seek(offset, SeekOrigin.Begin);

						bb = new ByteBuffer(FrameSize);
						bb.CopyFrom(fs, FrameSize);
						bb.Endian = _obAttrib.Reference.Endian;
						fs.Close();
					}

					bb.CopyTo(dstBuffer, 0, dstOffset, FrameSize);
					return;
				}

				using (var stream = _obAttrib.Data.AsStream())
				{
					stream.Position = _frameIndex*FrameSize;
					stream.Read(dstBuffer, dstOffset, FrameSize);
				}
			}
		}

		/// <summary>
		/// Container representing frame data in an OW Pixel Data attribute.
		/// </summary>
		private class FrameDataOW : FrameData
		{
			private readonly DicomAttributeOW _owAttrib;
			private readonly int _frameIndex;

			/// <summary>
			/// Initializes the frame data container from an OW Pixel Data attribute.
			/// </summary>
			public FrameDataOW(DicomUncompressedPixelData owner, DicomAttributeOW pixelDataAttribute, int frameIndex)
				: base(owner)
			{
				_owAttrib = pixelDataAttribute;
				_frameIndex = frameIndex;
			}

			public override void GetFrame(byte[] dstBuffer, int dstOffset)
			{
				if (_owAttrib.StreamLength < (_frameIndex + 1)*FrameSize)
					throw new DicomDataException("Requested frame exceeds available pixel data in buffer.");

				if (_owAttrib.Reference != null)
				{
					ByteBuffer bb;
					if ((FrameSize%2 == 1) && (_owAttrib.Reference.Endian != ByteBuffer.LocalMachineEndian))
					{
						// When frames are odd size, and we need to byte swap, we need to get an extra byte.
						// For odd number frames, we get a byte before the frame
						// and for even frames we get a byte after the frame.

						using (var fs = _owAttrib.Reference.StreamOpener.Open())
						{
							if (_frameIndex%2 == 1)
								fs.Seek((_owAttrib.Reference.Offset + _frameIndex*FrameSize) - 1, SeekOrigin.Begin);
							else
								fs.Seek(_owAttrib.Reference.Offset + _frameIndex*FrameSize, SeekOrigin.Begin);

							bb = new ByteBuffer(FrameSize + 1);
							bb.CopyFrom(fs, FrameSize + 1);
							bb.Endian = _owAttrib.Reference.Endian;
							fs.Close();
						}

						bb.Swap(_owAttrib.Reference.Vr.UnitSize);
						bb.Endian = ByteBuffer.LocalMachineEndian;

						if (_frameIndex%2 == 1)
							bb.CopyTo(dstBuffer, 1, dstOffset, FrameSize);
						else
							bb.CopyTo(dstBuffer, 0, dstOffset, FrameSize);
						return;
					}

					using (var fs = _owAttrib.Reference.StreamOpener.Open())
					{
						fs.Seek(_owAttrib.Reference.Offset + _frameIndex*FrameSize, SeekOrigin.Begin);

						bb = new ByteBuffer(FrameSize);
						bb.CopyFrom(fs, FrameSize);
						bb.Endian = _owAttrib.Reference.Endian;
						fs.Close();
					}

					if (_owAttrib.Reference.Endian != ByteBuffer.LocalMachineEndian)
					{
						bb.Swap(_owAttrib.Reference.Vr.UnitSize);
						bb.Endian = ByteBuffer.LocalMachineEndian;
					}

					bb.CopyTo(dstBuffer, 0, dstOffset, FrameSize);
					return;
				}

				using (var stream = _owAttrib.Data.AsStream())
				{
					stream.Position = _frameIndex*FrameSize;
					stream.Read(dstBuffer, dstOffset, FrameSize);
				}
			}
		}

		#endregion
	}
}