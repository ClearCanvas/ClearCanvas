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
using System.Diagnostics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A grayscale pixel data wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="GrayscalePixelData"/> provides a number of convenience methods
	/// to make accessing and changing grayscale pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="PixelData.SetPixel(int, int, int)"/> is not recommended if you want
	/// good performance.  Instead, use the <see cref="PixelData.Raw"/> property 
	/// to get the raw byte array, then use unsafe code to do your processing.
	/// </remarks>
	/// <seealso cref="PixelData"/>
	public class GrayscalePixelData : PixelData
	{
		#region Private fields

		private bool _isSigned;
		private int _bitsStored;
		private int _highBit;
		private int _absoluteMinPixelValue;
		private int _absoluteMaxPixelValue;

		#endregion

		#region Public constructor

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscalePixelData"/> with the
		/// specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="bitsAllocated">The number of bits allocated in the <paramref name="pixelData"/>.</param>
		/// <param name="bitsStored">The number of bits stored in the <paramref name="pixelData"/>.</param>
		/// <param name="highBit">The high bit of the <paramref name="pixelData"/>.</param>
		/// <param name="isSigned">Indicates whether or not <paramref name="pixelData"/> contains signed data.</param>
		/// <param name="pixelData">The pixel data to be wrapped.</param>
		public GrayscalePixelData(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			byte[] pixelData)
			: base(rows, columns, bitsAllocated, pixelData)
		{
			Initialize(bitsStored, highBit, isSigned);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscalePixelData"/> with the
		/// specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="bitsAllocated">The number of bits allocated in the pixel data returned by <paramref name="pixelDataGetter"/>.</param>
		/// <param name="bitsStored">The number of bits stored in the pixel data returned by <paramref name="pixelDataGetter"/>.</param>
		/// <param name="highBit">The high bit of the pixel data returned by <paramref name="pixelDataGetter"/>.</param>
		/// <param name="isSigned">Indicates whether or not the pixel data returned by <paramref name="pixelDataGetter"/> contains signed data.</param>
		/// <param name="pixelDataGetter">A delegate that returns the pixel data.</param>
		public GrayscalePixelData(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			PixelDataGetter pixelDataGetter)
			: base(rows, columns, bitsAllocated, pixelDataGetter)
		{
			Initialize(bitsStored, highBit, isSigned);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Returns the absolute minimum possible pixel value for the image based on PixelRepresentation and BitsAllocated.
		/// </summary>
		public int AbsoluteMinPixelValue
		{
			get { return _absoluteMinPixelValue; }
		}

		/// <summary>
		/// Returns the absolute maximum possible pixel value for the image based on PixelRepresentation and BitsAllocated.
		/// </summary>
		public int AbsoluteMaxPixelValue
		{
			get { return _absoluteMaxPixelValue; }
		}

		/// <summary>
		/// Gets the number of bits stored per pixel in the pixel data.
		/// </summary>
		/// <remarks>
		/// In contrast to <see cref="PixelData.BitsAllocated"/> which indicates the number of bits that have been
		/// allocated for each pixel, the <see cref="BitsStored"/> describes the actual number of bits used
		/// to encode the data.
		/// </remarks>
		public int BitsStored
		{
			get { return _bitsStored; }
		}

		/// <summary>
		/// Gets the 0-based position of the most significant bit within the bits allocated for each pixel in the pixel data.
		/// </summary>
		public int HighBit
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Gets a value indicating if the pixel data is signed.
		/// </summary>
		/// <remarks>
		/// If the pixel data is signed, then each pixel is stored in 2's complement form with the sign bit starting at <see cref="HighBit"/>.
		/// </remarks>
		public bool IsSigned
		{
			get { return _isSigned; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a copy of the object, including the pixel data.
		/// </summary>
		/// <returns></returns>
		public new GrayscalePixelData Clone()
		{
			return base.Clone() as GrayscalePixelData;
		}

		/// <summary>
		/// Calculates the Minimum and Maximum pixel values from the pixel data efficiently, using unsafe code.
		/// </summary>
		/// <param name="minPixelValue">Returns the minimum pixel value.</param>
		/// <param name="maxPixelValue">Returns the maximum pixel value.</param>
		unsafe public void CalculateMinMaxPixelValue(out int minPixelValue, out int maxPixelValue)
		{
			byte[] pixelData = GetPixelData();

#if DEBUG
			CodeClock clock = new CodeClock();
			clock.Start();
#endif
			if (_isSigned)
			{
				if (_bitsAllocated == 8)
				{
					fixed (byte* ptr = pixelData)
					{
						byte* pixel = (byte*)ptr;

						byte signMask = (byte)(1 << (_bitsStored - 1));

						sbyte max, min;
						max = sbyte.MinValue;
						min = sbyte.MaxValue;

						for (int i = 0; i < _rows * _columns; ++i)
						{
							sbyte result;
							if (0 == ((*pixel) & signMask))
							{
								result = (sbyte)(*pixel);
							}
							else
							{
								byte inverted = (byte)(~(*pixel));
								// Need to mask out the bits greater above the high bit, since they're irrelevant
								byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
								byte maskedInverted = (byte)(inverted & mask);
								result = (sbyte)(-(maskedInverted + 1));
							}

							if (result > max)
								max = result;
							else if (result < min)
								min = result;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
				else
				{
					fixed (byte* ptr = pixelData)
					{
						UInt16* pixel = (UInt16*)ptr;

						UInt16 signMask = (UInt16)(1 << (_bitsStored - 1));

						Int16 max, min;
						max = Int16.MinValue;
						min = Int16.MaxValue;

						for (int i = 0; i < _rows * _columns; ++i)
						{
							Int16 result;
							if (0 == ((*pixel) & signMask))
							{
								result = (Int16)(*pixel);
							}
							else
							{
								UInt16 inverted = (UInt16)(~(*pixel));
								// Need to mask out the bits greater above the high bit, since they're irrelevant
								UInt16 mask = (UInt16)(UInt16.MaxValue >> (_bitsAllocated - _bitsStored));
								UInt16 maskedInverted = (UInt16)(inverted & mask);
								result = (Int16)(-(maskedInverted + 1));
							}

							if (result > max)
								max = result;
							else if (result < min)
								min = result;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
			}
			else
			{
				if (_bitsAllocated == 8)
				{
					fixed (byte* ptr = pixelData)
					{
						byte* pixel = ptr;

						byte max, min;
						max = min = *pixel;

						for (int i = 1; i < _rows * _columns; ++i)
						{
							if (*pixel > max)
								max = *pixel;
							else if (*pixel < min)
								min = *pixel;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
				else
				{
					fixed (byte* ptr = pixelData)
					{
						UInt16* pixel = (UInt16*)ptr;

						UInt16 max, min;
						max = min = *pixel;

						for (int i = 1; i < _rows * _columns; ++i)
						{
							if (*pixel > max)
								max = *pixel;
							else if (*pixel < min)
								min = *pixel;

							++pixel;
						}

						maxPixelValue = (int)max;
						minPixelValue = (int)min;
					}
				}
			}

#if DEBUG
			clock.Stop();
			Trace.WriteLine(String.Format("Min/Max pixel value calculation took {0:F3} seconds (rows = {1}, columns = {2})", clock.Seconds, _rows, _columns));
#endif
		}

		#endregion

		#region Overrides

		/// <summary>
		/// This method overrides <see cref="PixelData.CloneInternal"/>.
		/// </summary>
		protected override PixelData CloneInternal()
		{
			return new GrayscalePixelData(
				_rows,
				_columns,
				_bitsAllocated,
				_bitsStored,
				_highBit,
				_isSigned,
				(byte[])GetPixelData().Clone());
		}

		/// <summary>
		/// This method overrides <see cref="PixelData.GetPixelInternal"/>.
		/// </summary>
		protected override int GetPixelInternal(int i)
		{
			if (_bytesPerPixel == 1) // 8 bit
			{
				if (_isSigned)
					return (int)GetPixelSigned8(i);
				else
					return (int)GetPixelUnsigned8(i);
			}
			else // 16 bit
			{
				if (_isSigned)
					return (int)GetPixelSigned16(i);
				else
					return (int)GetPixelUnsigned16(i);
			}
		}

		/// <summary>
		/// This method overrides <see cref="PixelData.SetPixelInternal"/>
		/// </summary>
		protected override void SetPixelInternal(int i, int value)
		{
			if (_bytesPerPixel == 1)
			{
				if (_isSigned)
					SetPixelSigned8(i, value);
				else
					SetPixelUnsigned8(i, value);
			}
			else
			{
				if (_isSigned)
					SetPixelSigned16(i, value);
				else
					SetPixelUnsigned16(i, value);
			}
		}

		#endregion

		#region Private get methods

		private byte GetPixelUnsigned8(int i)
		{
			return GetPixelData()[i];
		}

		private sbyte GetPixelSigned8(int i)
		{
			byte raw = GetPixelUnsigned8(i);
			sbyte result = ConvertToSigned8(raw);

			return result;
		}

		private sbyte ConvertToSigned8(byte raw)
		{
			// Create a mask that will pick out the sign bit, which is the high bit
			byte signMask = (byte)(1 << (_bitsStored - 1));
			sbyte result;

			// If the sign bit is 0, then just return the raw value,
			// since it's like an unsigned value
			if ((raw & signMask) == 0)
			{
				result = (sbyte)raw;
			}
			// If the sign bit is 1, then the value is in 2's complement, which
			// means we have to compute the corresponding positive number by
			// inverting the bits and adding 1
			else
			{
				byte inverted = (byte)(~raw);
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
				byte maskedInverted = (byte)(inverted & mask);
				result = (sbyte)(-(maskedInverted + 1));
			}

			return result;
		}

		private ushort GetPixelUnsigned16(int i)
		{
			ushort lowbyte, highbyte;
			byte[] pixelData = GetPixelData();

			lowbyte = (ushort)pixelData[i];
			highbyte = (ushort)pixelData[i + 1];
			return (ushort)(lowbyte | (highbyte << 8));
		}

		private short GetPixelSigned16(int i)
		{
			ushort raw = GetPixelUnsigned16(i);
			short result = ConvertToSigned16(raw);

			return result;
		}

		private short ConvertToSigned16(ushort raw)
		{
			// Create a mask that will pick out the sign bit, which is the high bit
			ushort signMask = (ushort)(1 << (_bitsStored - 1));
			short result;

			// If the sign bit is 0, then just return the raw value,
			// since it's like an unsigned value
			if ((raw & signMask) == 0)
			{
				result = (short)raw;
			}
			// If the sign bit is 1, then the value is in 2's complement, which
			// means we have to compute the corresponding positive number by
			// inverting the bits and adding 1
			else
			{
				ushort inverted = (ushort)(~raw);
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				ushort mask = (ushort)(ushort.MaxValue >> (_bitsAllocated - _bitsStored));
				ushort maskedInverted = (ushort)(inverted & mask);
				result = (short)(-(maskedInverted + 1));
			}

			return result;
		}


		#endregion

		#region Private set methods

		private void SetPixelUnsigned8(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			GetPixelData()[i] = (byte)value;
		}


		private void SetPixelSigned8(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel8(i, (byte)value);
			else
			{
				byte raw = (byte)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				byte mask = (byte)(byte.MaxValue >> (_bitsAllocated - _bitsStored));
				byte maskedRaw = (byte)(raw & mask);
				SetPixel8(i, maskedRaw);
			}
		}

		private void SetPixel8(int i, byte value)
		{
			GetPixelData()[i] = value;
		}

		private void SetPixelUnsigned16(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			SetPixel16(i, (ushort)value);
		}

		private void SetPixelSigned16(int i, int value)
		{
			if (value < _absoluteMinPixelValue || value > _absoluteMaxPixelValue)
				throw new ArgumentException("Value is out of range");

			if (value >= 0)
				SetPixel16(i, (ushort)value);
			else
			{
				ushort raw = (ushort)value;
				// Need to mask out the bits greater above the high bit, since they're irrelevant
				ushort mask = (ushort)(ushort.MaxValue >> (_bitsAllocated - _bitsStored));
				ushort maskedRaw = (ushort)(raw & mask);
				SetPixel16(i, maskedRaw);
			}
		}

		private void SetPixel16(int i, ushort value)
		{
			byte[] pixelData = GetPixelData();

			pixelData[i] = (byte)(value & 0x00ff); // low-byte first (little endian)
			pixelData[i + 1] = (byte)((value & 0xff00) >> 8); // high-byte last
		}

		#endregion

		#region Other private methods

		private void Initialize(int bitsStored, int highBit, bool isSigned)
		{
			DicomValidator.ValidateBitsStored(bitsStored);
			DicomValidator.ValidateHighBit(highBit);

			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;

			CalculateAbsolutePixelValueRange();
		}

		private void CalculateAbsolutePixelValueRange()
		{
			if (_isSigned)
			{
				_absoluteMinPixelValue = -(1 << (_bitsStored - 1));
				_absoluteMaxPixelValue = (1 << (_bitsStored - 1)) - 1;
			}
			else
			{
				_absoluteMinPixelValue = 0;
				_absoluteMaxPixelValue = (1 << _bitsStored) - 1;
			}
		}

		#endregion
	}
}
