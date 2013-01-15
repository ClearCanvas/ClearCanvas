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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Represents DICOM overlay data.
	/// </summary>
	public class OverlayData
	{
		private readonly int _offset;
		private readonly int _rows;
		private readonly int _columns;
		private readonly bool _bigEndianWords;
		private readonly byte[] _rawOverlayData;

		/// <summary>
		/// Constructs a new overlay data object.
		/// </summary>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		/// <param name="bigEndianWords">A value indicating if the <paramref name="overlayData"/> is stored as 16-bit words with big endian byte ordering</param>
		/// <param name="overlayData">The packed bits overlay data buffer.</param>
		public OverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayData)
			: this(0, rows, columns, bigEndianWords, overlayData) {}

		/// <summary>
		/// Constructs a new overlay data object.
		/// </summary>
		/// <param name="offset">The initial offset of the first bit within the <paramref name="overlayData"/>.</param>
		/// <param name="rows">The number of rows in the overlay.</param>
		/// <param name="columns">The number of columns in the overlay.</param>
		/// <param name="bigEndianWords">A value indicating if the <paramref name="overlayData"/> is stored as 16-bit words with big endian byte ordering</param>
		/// <param name="overlayData">The packed bits overlay data buffer.</param>
		public OverlayData(int offset, int rows, int columns, bool bigEndianWords, byte[] overlayData)
		{
			Platform.CheckNonNegative(offset, "offset");

			if (overlayData == null)
				overlayData = new byte[0];

			_offset = offset;
			_rows = rows;
			_columns = columns;
			_bigEndianWords = bigEndianWords;
			_rawOverlayData = overlayData;
		}

		/// <summary>
		/// Gets the raw, packed bits overlay data buffer.
		/// </summary>
		public byte[] Raw
		{
			get { return _rawOverlayData; }
		}

		/// <summary>
		/// Unpacks the overlay data into an 8-bit overlay pixel data buffer.
		/// </summary>
		/// <returns>The unpacked, 8-bit overlay pixel data buffer.</returns>
		public byte[] Unpack()
		{
			byte[] unpackedPixelData = MemoryManager.Allocate<byte>(_rows*_columns);
			Unpack(_rawOverlayData, unpackedPixelData, _offset, unpackedPixelData.Length, _bigEndianWords);
			return unpackedPixelData;
		}

		/// <summary>
		/// Extracts an overlay embedded in a DICOM Pixel Data buffer and unpacks it to form an 8-bit overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Embedded overlays were last defined in the 2004 DICOM Standard. Since then, their usage has been retired.
		/// As such, there is no mechanism to directly read or encode embedded overlays. This method may be used as a
		/// helper to extract overlays in images encoded with a previous version of the standard for display in compatibility
		/// mode or storage as packed bit data.
		/// </para>
		/// <para>
		/// This overload processes all pixel allocations in the pixel data buffer. If the pixel data contains mutliple image frames,
		/// the unpacked overlay data buffer will also be multi-frame (with the key difference that the overlay data will always
		/// be 8 bits per pixel, rather than potentially 16 bits per pixel).
		/// </para>
		/// </remarks>
		/// <param name="bitPosition">The bit position of the overlay plane within the pixel data buffer.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel. Must be 8 or 16.</param>
		/// <param name="bigEndianWords">A value indicating if the pixel data buffer is stored as 16-bit words with big endian byte ordering.</param>
		/// <param name="pixelData">The DICOM Pixel Data buffer containing an embedded overlay.</param>
		/// <returns>The unpacked, 8-bit overlay pixel data buffer.</returns>
		public static byte[] UnpackFromPixelData(int bitPosition, int bitsAllocated, bool bigEndianWords, byte[] pixelData)
		{
			return UnpackFromPixelData(bitPosition, bitsAllocated, 0, pixelData.Length, bigEndianWords, pixelData);
		}

		/// <summary>
		/// Extracts an overlay embedded in a DICOM Pixel Data buffer and unpacks it to form an 8-bit overlay pixel data buffer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Embedded overlays were last defined in the 2004 DICOM Standard. Since then, their usage has been retired.
		/// As such, there is no mechanism to directly read or encode embedded overlays. This method may be used as a
		/// helper to extract overlays in images encoded with a previous version of the standard for display in compatibility
		/// mode or storage as packed bit data.
		/// </para>
		/// </remarks>
		/// <param name="bitPosition">The bit position of the overlay plane within the pixel data buffer.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel. Must be 8 or 16.</param>
		/// <param name="frameIndex">The 0-based index of the image frame from which an overlay plane is to be extracted.</param>
		/// <param name="frameLength">The size of an image frame in bytes.</param>
		/// <param name="bigEndianWords">A value indicating if the pixel data buffer is stored as 16-bit words with big endian byte ordering.</param>
		/// <param name="pixelData">The DICOM Pixel Data buffer containing an embedded overlay.</param>
		/// <returns>The unpacked, 8-bit overlay pixel data buffer.</returns>
		public static byte[] UnpackFromPixelData(int bitPosition, int bitsAllocated, int frameIndex, int frameLength, bool bigEndianWords, byte[] pixelData)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("bitsAllocated must be either 8 or 16.", "bitsAllocated");

			var bytesNeeded = frameLength/(bitsAllocated/8);
			var extractedOverlay = MemoryManager.Allocate<byte>(bytesNeeded);
			Extract(pixelData, extractedOverlay, frameIndex*frameLength, bytesNeeded, bitsAllocated, bitPosition, bigEndianWords);
			return extractedOverlay;
		}

		/// <summary>
		/// Creates a packed overlay data object using the specified 8-bit pixel data buffer.
		/// </summary>
		/// <remarks>
		/// This function creates an overlay from the non-zero pixels of the specified pixel data.
		/// </remarks>
		/// <param name="rows">The number of rows of pixels.</param>
		/// <param name="columns">The number of columns of pixels.</param>
		/// <param name="bigEndianWords">A value indicating if the output overlay data should be encoded as 16-bit words with big endian byte ordering.</param>
		/// <param name="overlayPixelData">The pixel data buffer containing the overlay contents.</param>
		/// <returns>A packed overlay data object representing the overlay contents.</returns>
		public static OverlayData CreateOverlayData(int rows, int columns, bool bigEndianWords, byte[] overlayPixelData)
		{
			return CreateOverlayData(rows, columns, 8, 8, 7, bigEndianWords, overlayPixelData);
		}

		/// <summary>
		/// Creates a packed overlay data object using the specified pixel data buffer.
		/// </summary>
		/// <remarks>
		/// This function creates an overlay from the non-zero pixels of the specified pixel data. Only the bits
		/// specified by <paramref name="bitsStored"/> and <paramref name="highBit"/> are considered.
		/// </remarks>
		/// <param name="rows">The number of rows of pixels.</param>
		/// <param name="columns">The number of columns of pixels.</param>
		/// <param name="bitsStored">The number of bits stored per pixel.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel. Must be 8 or 16.</param>
		/// <param name="highBit">The bit position of the most significant bit of a pixel.</param>
		/// <param name="bigEndianWords">A value indicating if the output overlay data should be encoded as 16-bit words with big endian byte ordering.</param>
		/// <param name="overlayPixelData">The pixel data buffer containing the overlay contents.</param>
		/// <returns>A packed overlay data object representing the overlay contents.</returns>
		public static OverlayData CreateOverlayData(int rows, int columns, int bitsStored, int bitsAllocated, int highBit, bool bigEndianWords, byte[] overlayPixelData)
		{
			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("bitsAllocated must be either 8 or 16.", "bitsAllocated");

			var bytesNeeded = (int) Math.Ceiling(rows*columns/8d);
			var packedOverlayData = MemoryManager.Allocate<byte>(bytesNeeded + (bytesNeeded%2)); // the %2 term rounds up to even length
			var inputMask = ((1 << bitsStored) - 1) << (highBit - bitsStored + 1);
			if (bitsAllocated == 8)
				Pack(overlayPixelData, packedOverlayData, 0, rows*columns, (byte) inputMask, bigEndianWords);
			else if (bitsAllocated == 16)
				Pack(overlayPixelData, packedOverlayData, 0, rows*columns, (ushort) inputMask, bigEndianWords);
			return new OverlayData(rows, columns, bigEndianWords, packedOverlayData);
		}

		#region Overlay Data Algorithms

		/// <summary>
		/// Converts an 8-bit byte buffer into a packed bit stream (zero and non-zero bytes mapping to bits 0 and 1 respectively).
		/// </summary>
		/// <param name="unpackedData">The buffer containing the unpacked data.</param>
		/// <param name="packedBits">A buffer to which the bits will be packed.</param>
		/// <param name="offset">The bit offset in <paramref name="packedBits"/> to which conversion will start.</param>
		/// <param name="length">The number of bits to be converted.</param>
		/// <param name="inputMask">The input mask for each 8-bit input window of the <paramref name="unpackedData"/>.</param>
		/// <param name="bigEndianWords">Whether or not <paramref name="packedBits"/> is in 16-bit big endian words.</param>
		private static void Pack(byte[] unpackedData, byte[] packedBits, int offset, int length, byte inputMask, bool bigEndianWords)
		{
			if (inputMask == 0)
				throw new ArgumentException("Input mask was not specified.", "inputMask");
			if (unpackedData.Length < length)
				throw new ArgumentException("Input byte array does not contain sufficient information.", "unpackedData");
			if (bigEndianWords && packedBits.Length%2 == 1)
				throw new ArgumentException("Output byte array must be even-length.", "packedBits");
			if (packedBits.Length*8 < offset + length)
				throw new ArgumentException("Output byte array does not have sufficient room to store results.", "packedBits");

			var outLen = packedBits.Length;
			var inPos = 0;
			unsafe
			{
				// buffer overrun declaration: <input> is indexed using <inPos> which is checked against <length>
				fixed (byte* input = unpackedData)
				{
					// buffer overrun declaration: <output> is indexed using <outPos> which is checked against <outLen>
					fixed (byte* output = packedBits)
					{
						if (bigEndianWords) // when the output is 16-bit big endian, we must write 16 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each output window)
							var bitMask = (ushort) (1 << (offset%16));

							// compute the starting write cursor based on byte offset
							var outPos = 2*(offset/16);

							// write until we've converted the requested bits
							while (inPos < length && outPos < outLen - 1)
							{
								// initialize the output window from the bytes at the write cursor
								var window = (ushort) ((output[outPos] << 8) + output[outPos + 1]);

								// loop until we've set all bits in the current output window or we've set all we needed
								while (bitMask > 0 && inPos < length)
								{
									// convert the input in question and either set or clear the bit
									window = (ushort) ((input[inPos++] & inputMask) > 0 ? window | bitMask : window & ~bitMask);

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (ushort) (bitMask << 1);
								}

								// write the output window back to the buffer
								output[outPos] = (byte) ((window >> 8) & 0x00FF);
								output[outPos + 1] = (byte) (window & 0x00FF);

								// reset the bit mask
								bitMask = 0x01;

								// advance the write cursor
								outPos += 2;
							}
						}
						else // when output is 8-bit or 16-bit little endian, we can write 8 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each output window)
							var bitMask = (byte) (1 << (offset%8));

							// compute the starting write cursor based on byte offset
							var outPos = (offset/8);

							// write until we've converted the requested bits
							while (inPos < length && outPos < outLen)
							{
								// initialize the output window from the bytes at the write cursor
								var window = output[outPos];

								// loop until we've set all bits in the current output window or we've set all we needed
								while (bitMask > 0 && inPos < length)
								{
									// convert the input in question and either set or clear the bit
									window = (byte) ((input[inPos++] & inputMask) > 0 ? window | bitMask : window & ~bitMask);

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (byte) (bitMask << 1);
								}

								// write the output window back to the buffer
								output[outPos] = window;

								// reset the bit mask
								bitMask = 0x01;

								// advance the write cursor
								++outPos;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts a 16-bit byte buffer into a packed bit stream (zero and non-zero words mapping to bits 0 and 1 respectively).
		/// </summary>
		/// <param name="unpackedData">The buffer containing the unpacked data in local machine endianess.</param>
		/// <param name="packedBits">A buffer to which the bits will be packed.</param>
		/// <param name="offset">The bit offset in <paramref name="packedBits"/> to which conversion will start.</param>
		/// <param name="length">The number of bits to be converted.</param>
		/// <param name="inputMask">The input mask for each 16-bit input window of the <paramref name="unpackedData"/>.</param>
		/// <param name="bigEndianWords">Whether or not <paramref name="packedBits"/> is in 16-bit big endian words.</param>
		private static void Pack(byte[] unpackedData, byte[] packedBits, int offset, int length, ushort inputMask, bool bigEndianWords)
		{
			if (inputMask == 0)
				throw new ArgumentException("Input mask was not specified.", "inputMask");
			if (unpackedData.Length < length*2)
				throw new ArgumentException("Input byte array does not contain sufficient information.", "unpackedData");
			if (bigEndianWords && packedBits.Length%2 == 1)
				throw new ArgumentException("Output byte array must be even-length.", "packedBits");
			if (packedBits.Length*8 < offset + length)
				throw new ArgumentException("Output byte array does not have sufficient room to store results.", "packedBits");

			var outLen = packedBits.Length;
			var inPos = 0;
			unsafe
			{
				// buffer overrun declaraction: <input> is indexed using <inPos> which is checked against <length>
				fixed (byte* inputBytes = unpackedData)
				{
					ushort* input = (ushort*) inputBytes;

					// buffer overrun declaration: <output> is indexed using <outPos> which is checked against <outLen>
					fixed (byte* output = packedBits)
					{
						if (bigEndianWords) // when the output is 16-bit big endian, we must write 16 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each output window)
							var bitMask = (ushort) (1 << (offset%16));

							// compute the starting write cursor based on byte offset
							var outPos = 2*(offset/16);

							// write until we've converted the requested bits
							while (inPos < length && outPos < outLen - 1)
							{
								// initialize the output window from the bytes at the write cursor
								var window = (ushort) ((output[outPos] << 8) + output[outPos + 1]);

								// loop until we've set all bits in the current output window or we've set all we needed
								while (bitMask > 0 && inPos < length)
								{
									// convert the input in question and either set or clear the bit
									window = (ushort) ((input[inPos++] & inputMask) > 0 ? window | bitMask : window & ~bitMask);

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (ushort) (bitMask << 1);
								}

								// write the output window back to the buffer
								output[outPos] = (byte) ((window >> 8) & 0x00FF);
								output[outPos + 1] = (byte) (window & 0x00FF);

								// reset the bit mask
								bitMask = 0x01;

								// advance the write cursor
								outPos += 2;
							}
						}
						else // when output is 8-bit or 16-bit little endian, we can write 8 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each output window)
							var bitMask = (byte) (1 << (offset%8));

							// compute the starting write cursor based on byte offset
							var outPos = (offset/8);

							// write until we've converted the requested bits
							while (inPos < length && outPos < outLen)
							{
								// initialize the output window from the bytes at the write cursor
								var window = output[outPos];

								// loop until we've set all bits in the current output window or we've set all we needed
								while (bitMask > 0 && inPos < length)
								{
									// convert the input in question and either set or clear the bit
									window = (byte) ((input[inPos++] & inputMask) > 0 ? window | bitMask : window & ~bitMask);

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (byte) (bitMask << 1);
								}

								// write the output window back to the buffer
								output[outPos] = window;

								// reset the bit mask
								bitMask = 0x01;

								// advance the write cursor
								++outPos;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts a packed bit stream into an 8-bit byte buffer (bits 0 and 1 mapping to bytes 0x00 and 0xFF respectively).
		/// </summary>
		/// <param name="packedBits">The buffer containing the packed bits.</param>
		/// <param name="unpackedData">A buffer to which the bits will be unpacked.</param>
		/// <param name="offset">The bit offset in <paramref name="packedBits"/> from which conversion will start.</param>
		/// <param name="length">The number of bits to be converted.</param>
		/// <param name="bigEndianWords">Whether or not the <paramref name="packedBits"/> is in 16-bit big endian words.</param>
		private static void Unpack(byte[] packedBits, byte[] unpackedData, int offset, int length, bool bigEndianWords)
		{
			const byte ONE = 0xff;
			const byte ZERO = 0x00;

			if (bigEndianWords && packedBits.Length%2 == 1)
				throw new ArgumentException("Input byte array must be even-length.", "packedBits");
			if (packedBits.Length*8 < offset + length)
				throw new ArgumentException("Input byte array does not contain sufficient information.", "packedBits");
			if (unpackedData.Length < length)
				throw new ArgumentException("Output byte array does not have sufficient room to store results.", "unpackedData");

			var inLen = packedBits.Length;
			var outPos = 0;
			unsafe
			{
				// buffer overrun declaration: <input> is indexed using <inPos> which is checked against <inLen>
				fixed (byte* input = packedBits)
				{
					// buffer overrun declaration: <output> is indexed using <outPos> which is checked against <length>
					fixed (byte* output = unpackedData)
					{
						if (bigEndianWords) // when the input is 16-bit big endian, we must read 16 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each input window)
							var bitMask = (ushort) (1 << (offset%16));

							// compute the starting read cursor based on byte offset
							var inPos = 2*(offset/16);

							// read until we've converted the requested bits
							while (outPos < length && inPos < inLen - 1)
							{
								// create the input window from the bytes at the read cursor
								var window = (ushort) ((input[inPos] << 8) + input[inPos + 1]);

								// loop until we've got all bits in the current input window or we've got all we came for
								while (bitMask > 0 && outPos < length)
								{
									// convert the bit in question into a byte
									output[outPos++] = (window & bitMask) > 0 ? ONE : ZERO;

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (ushort) (bitMask << 1);
								}

								// reset the bit mask
								bitMask = 0x01;

								// advance the read cursor
								inPos += 2;
							}
						}
						else // when input is 8-bit or 16-bit little endian, we can read 8 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each input window)
							var bitMask = (byte) (1 << (offset%8));

							// compute the starting read cursor based on byte offset
							var inPos = (offset/8);

							// read until we've converted the requested bits
							while (outPos < length && inPos < inLen)
							{
								// create the input window from the bytes at the read cursor
								var window = input[inPos];

								// loop until we've got all bits in the current input window or we've got all we came for
								while (bitMask > 0 && outPos < length)
								{
									// convert the bit in question into a byte
									output[outPos++] = (window & bitMask) > 0 ? ONE : ZERO;

									// shift the bit mask (eventually, the mask rolls over to 0, which will break the loop)
									bitMask = (byte) (bitMask << 1);
								}

								// reset the bit mask
								bitMask = 0x01;

								// advance the read cursor
								++inPos;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Extracts a specified bit from a pixel data buffer into an 8-bit byte buffer (bits 0 and 1 mapping to bytes 0x00 and 0xFF respectively).
		/// </summary>
		/// <param name="pixelData">The buffer containing the pixel data.</param>
		/// <param name="extractedData">A buffer to which the bits will be extracted.</param>
		/// <param name="offset">The byte offset in <paramref name="pixelData"/> from which extraction will start.</param>
		/// <param name="length">The number of pixels from which a bit will be extracted.</param>
		/// <param name="bitsAllocated">The number of bits allocated per pixel in the <paramref name="pixelData"/>.</param>
		/// <param name="bitPosition">The 0-based bit position within a pixel allocation which is to be extracted.</param>
		/// <param name="bigEndianWords">Whether or not the <paramref name="pixelData"/> is in 16-bit big endian words.</param>
		private static void Extract(byte[] pixelData, byte[] extractedData, int offset, int length, int bitsAllocated, int bitPosition, bool bigEndianWords)
		{
			const byte ONE = 0xff;
			const byte ZERO = 0x00;

			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("Bits Allocated must be either 8 or 16 bits.", "bitsAllocated");
			if (bitPosition < 0 || bitPosition >= bitsAllocated)
				throw new ArgumentOutOfRangeException("bitPosition", "Bit Position must be between 0 and Bits Allocated - 1, inclusive.");
			if (bitsAllocated == 16 && bigEndianWords && pixelData.Length%2 == 1)
				throw new ArgumentException("Input byte array must be even-length.", "pixelData");
			if (pixelData.Length < offset + length/(bitsAllocated/8))
				throw new ArgumentException("Input byte array does not contain sufficient information.", "pixelData");
			if (extractedData.Length < length)
				throw new ArgumentException("Output byte array does not have sufficient room to store results.", "extractedData");

			var inLen = pixelData.Length;
			var outPos = 0;
			unsafe
			{
				// buffer overrun declaration: <input> is indexed using <inPos> which is checked against <inLen>
				fixed (byte* input = pixelData)
				{
					// buffer overrun declaration: <output> is indexed using <outPos> which is checked against <length>
					fixed (byte* output = extractedData)
					{
						if (bitsAllocated == 16) // when the pixel data is in 16-bit word allocations, we must read 16 bits at a time
						{
							// initialize the bit mask to the correct bit offset (relative to each input window)
							var bitMask = (ushort) (1 << bitPosition);

							// compute the starting read cursor based on byte offset
							var inPos = offset;

							// in a big endian word, the most significant byte is the first one
							var msb = bigEndianWords ? 0 : 1;

							// read until we've extracted the requested bits
							while (outPos < length && inPos < inLen - 1)
							{
								// create the input window from the bytes at the read cursor
								var window = (ushort) ((input[inPos + msb] << 8) + input[inPos + 1 - msb]);

								// convert the bit in question into a byte
								output[outPos++] = (window & bitMask) > 0 ? ONE : ZERO;

								// advance the read cursor
								inPos += 2;
							}
						}
						else
						{
							// initialize the bit mask to the correct bit offset (relative to each input window)
							var bitMask = (byte) (1 << bitPosition);

							// compute the starting read cursor based on byte offset
							var inPos = offset;

							// read until we've extracted the requested bits
							while (outPos < length && inPos < inLen)
							{
								// create the input window from the bytes at the read cursor
								var window = input[inPos];

								// convert the bit in question into a byte
								output[outPos++] = (window & bitMask) > 0 ? ONE : ZERO;

								// advance the read cursor
								inPos++;
							}
						}
					}
				}
			}
		}

		#region Unit Test Entry Points

#if UNIT_TESTS

		/// <summary>
		/// bigEndianWords affects only <paramref name="packedBits"/>, *NOT* unpackedData, which is in machine endian
		/// </summary>
		internal static void TestUnpack(byte[] packedBits, byte[] unpackedData, int offset, int length, bool bigEndianWords)
		{
			Unpack(packedBits, unpackedData, offset, length, bigEndianWords);
		}

		/// <summary>
		/// bigEndianWords affects only <paramref name="packedBits"/>, *NOT* unpackedData, which is in machine endian
		/// </summary>
		internal static void TestPack(byte[] unpackedData, byte[] packedBits, int offset, int length, byte inputMask, bool bigEndianWords)
		{
			Pack(unpackedData, packedBits, offset, length, inputMask, bigEndianWords);
		}

		/// <summary>
		/// bigEndianWords affects only <paramref name="packedBits"/>, *NOT* unpackedData, which is in machine endian
		/// </summary>
		internal static void TestPack(byte[] unpackedData, byte[] packedBits, int offset, int length, ushort inputMask, bool bigEndianWords)
		{
			Pack(unpackedData, packedBits, offset, length, inputMask, bigEndianWords);
		}

		/// <summary>
		/// bigEndianWords affects only <paramref name="pixelData"/>, *NOT* extractedData, which is in machine endian
		/// </summary>
		internal static void TestExtract(byte[] pixelData, byte[] extractedData, int offset, int length, int bitsAllocated, int bitPosition, bool bigEndianWords)
		{
			Extract(pixelData, extractedData, offset, length, bitsAllocated, bitPosition, bigEndianWords);
		}

#endif

		#endregion

		#endregion
	}
}