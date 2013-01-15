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
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class OverlayDataTest
	{
		#region OverlayData Tests

		[Test]
		public void TestCreateOverlayData_8BitsAllocated()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001

			const string expectedResult = "111111001000000";
			var overlayPixels = new byte[]
			                    	{
			                    		0x10, 0x10, 0x10, 0x10, 0x10,
			                    		0x10, 0x00, 0x00, 0x10, 0x00,
			                    		0x00, 0x00, 0x00, 0x00, 0x00
			                    	};

			// little endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, false, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, false).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 8-bit case (little endian)");
			}

			// big endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, true, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, true).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 8-bit case (big endian)");
			}
		}

		[Test]
		public void TestCreateOverlayData_16BitsAllocated()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001

			const string expectedResult = "111111001000000";
			var overlayPixels = SwapBytes(new byte[]
			                              	{
			                              		// column1 |   column2 |   column3 |   column4 |   column5
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
			                              		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
			                              	}); // written here in big endian for easy reading

			// little endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, 12, 16, 15, false, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, false).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit case (little endian)");
			}

			// big endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, 12, 16, 15, true, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, true).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit case (big endian)");
			}
		}

		[Test]
		public void TestCreateOverlayData_16BitsAllocatedWithJunk()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001

			const string expectedResult = "111111001000000";

			// simulating the junk by specifying that we're using the middle 8 bits and the first and last 4 bits are to be ignored
			var overlayPixels = SwapBytes(new byte[]
			                              	{
			                              		// column1 |   column2 |   column3 |   column4 |   column5
			                              		0x09, 0x10, 0x09, 0x00, 0x03, 0x00, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x80, 0x03, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
			                              		0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x0F, 0x00, 0x00
			                              	}); // written here in big endian for easy reading

			// little endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, 8, 16, 11, false, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, false).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit w/junk case (little endian)");
			}

			// big endian test
			{
				var packedBits = OverlayData.CreateOverlayData(3, 5, 8, 16, 11, true, overlayPixels).Raw;
				var actualResult = FormatBits(packedBits, true).Substring(0, 15);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit w/junk case (big endian)");
			}
		}

		[Test]
		public void TestUnpack_SingleFrameA()
		{
			// 1 frame, 7x3
			// 1111111
			// 1000101
			// 1111111
			// continuous bit stream: 11111111 00010111 11111xxx xxxxxxxx
			// continuous LE byte stream: 11111111 11101000 xxx11111 xxxxxxxx
			// continuous BE word stream: 11101000 11111111 xxxxxxxx xxx11111

			const string expectedResult = "111111110001011111111";
			var packedBits = new byte[] {0xff, 0xe8, 0x9f, 0xf9};

			// little endian test
			{
				var unpackedBits = new OverlayData(3, 7, false, packedBits).Unpack();
				var actualResult = FormatNonZeroBytes(unpackedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in unpacked bits for single frame case A (little endian)");
			}

			// big endian test
			{
				var unpackedBits = new OverlayData(3, 7, true, SwapBytes(packedBits)).Unpack();
				var actualResult = FormatNonZeroBytes(unpackedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in unpacked bits for single frame case A (big endian)");
			}
		}

		[Test]
		public void TestUnpack_SingleFrameB()
		{
			// 1 frame, 7x3
			// 1111111
			// 1001011
			// 0000011
			// continuous bit stream: 11111111 00101100 00011xxx xxxxxxxx
			// continuous LE byte stream: 11111111 00110100 xxx11000 xxxxxxxx

			const string expectedResult = "111111110010110000011";
			var packedBits = new byte[] {0xff, 0x34, 0x78, 0xf9};

			// little endian test
			{
				var unpackedBits = new OverlayData(3, 7, false, packedBits).Unpack();
				var actualResult = FormatNonZeroBytes(unpackedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in unpacked bits for single frame case B (little endian)");
			}

			// big endian test
			{
				var unpackedBits = new OverlayData(3, 7, true, SwapBytes(packedBits)).Unpack();
				var actualResult = FormatNonZeroBytes(unpackedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in unpacked bits for single frame case B (big endian)");
			}
		}

		[Test]
		public void TestUnpack_Multiframe()
		{
			// 7 frames, each 3x2 (cols, rows) = 42 bits = 6 bytes
			// 111 111 111 111 111 111 111
			// 000 001 010 011 100 101 110
			// continuous bit stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx
			// continuous LE byte stream: 11000111 01111001 11011101 11001111 11111011 xxxxxx01
			var expectedResult = new[] {"111000", "111001", "111010", "111011", "111100", "111101", "111110"};
			var packedBits = new byte[] {0xc7, 0x79, 0xdd, 0xcf, 0xfb, 0xf1};

			for (int frameIndex = 0; frameIndex < 7; frameIndex++)
			{
				var offset = 3*2*frameIndex;

				// little endian test
				{
					var unpackedBits = new OverlayData(offset, 2, 3, false, packedBits).Unpack();
					var actualResult = FormatNonZeroBytes(unpackedBits);
					Assert.AreEqual(expectedResult[frameIndex], actualResult, "Error in unpacked bits for multiframe (fr#{0}) (little endian)", frameIndex);
				}

				// big endian test
				{
					var unpackedBits = new OverlayData(offset, 2, 3, true, SwapBytes(packedBits)).Unpack();
					var actualResult = FormatNonZeroBytes(unpackedBits);
					Assert.AreEqual(expectedResult[frameIndex], actualResult, "Error in unpacked bits for multiframe (fr#{0}) (big endian)", frameIndex);
				}
			}
		}

		[Test]
		public void TestUnpackFromPixelData_8BitsAllocated()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001
			const string expectedResult = "111111001000000";

			// 8 bits allocated

			// some pixel data: 1st col is a random "pixel", 2nd col creates room for overlay bit, 3rd col is the overlay
			var pixelData = new byte[]
			                	{
			                		(0xC5 & 0xFE) | 1,
			                		(0x2D & 0xFE) | 1,
			                		(0x5B & 0xFE) | 1,
			                		(0xB3 & 0xFE) | 1,
			                		(0xFC & 0xFE) | 1,
			                		(0xBC & 0xFE) | 1,
			                		(0x4d & 0xFE) | 0,
			                		(0xbf & 0xFE) | 0,
			                		(0x86 & 0xFE) | 1,
			                		(0x75 & 0xFE) | 0,
			                		(0xA8 & 0xFE) | 0,
			                		(0x19 & 0xFE) | 0,
			                		(0xAC & 0xFE) | 0,
			                		(0xD4 & 0xFE) | 0,
			                		(0x79 & 0xFE) | 0
			                	};

			var extractedBits = OverlayData.UnpackFromPixelData(0, 8, false, pixelData);
			var actualResult = FormatNonZeroBytes(extractedBits);
			Assert.AreEqual(expectedResult, actualResult, "Error in extracted bits from pixel data (8-bit pixels)");
		}

		[Test]
		public void TestUnpackFromPixelData_16BitsAllocated()
		{
			// 1 frame, 7x3
			// 1111111
			// 1001011
			// 0000011
			// continuous bit stream: 11111111 00101100 00011xxx xxxxxxxx
			const string expectedResult = "111111110010110000011";

			// 16 bits allocated

			// some pixel data: 1st col is a random "pixel", 2nd col creates room for overlay bit, 3rd col is the overlay
			//                  4th col moves the overlay to bit position 14, 5th col is the lower byte of the random pixel
			var pixelData = SwapBytes(new byte[] // written in big endian form for ease of reading :P
			                          	{
			                          		(0xC5 & 0x0F) | (1 << 6), 0x93,
			                          		(0x2D & 0x0F) | (1 << 6), 0x9C,
			                          		(0x5B & 0x0F) | (1 << 6), 0x78,
			                          		(0xB3 & 0x0F) | (1 << 6), 0x17,
			                          		(0xFC & 0x0F) | (1 << 6), 0x0c,
			                          		(0xBC & 0x0F) | (1 << 6), 0xc4,
			                          		(0x0B & 0x0F) | (1 << 6), 0x4c,
			                          		(0x74 & 0x0F) | (1 << 6), 0xe5,
			                          		(0x4d & 0x0F) | (0 << 6), 0x45,
			                          		(0xbf & 0x0F) | (0 << 6), 0xcd,
			                          		(0x86 & 0x0F) | (1 << 6), 0xAE,
			                          		(0x75 & 0x0F) | (0 << 6), 0xA9,
			                          		(0x51 & 0x0F) | (1 << 6), 0xbe,
			                          		(0x71 & 0x0F) | (1 << 6), 0x7e,
			                          		(0xA8 & 0x0F) | (0 << 6), 0x29,
			                          		(0x19 & 0x0F) | (0 << 6), 0x11,
			                          		(0xAC & 0x0F) | (0 << 6), 0xDD,
			                          		(0xD4 & 0x0F) | (0 << 6), 0x01,
			                          		(0x79 & 0x0F) | (0 << 6), 0xF0,
			                          		(0xA0 & 0x0F) | (1 << 6), 0xe2,
			                          		(0xBA & 0x0F) | (1 << 6), 0x98
			                          	});

			// little endian
			{
				var extractedBits = OverlayData.UnpackFromPixelData(14, 16, false, pixelData);
				var actualResult = FormatNonZeroBytes(extractedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in extracted bits from pixel data (16-bit pixels, little endian)");
			}

			// big endian
			{
				var extractedBits = OverlayData.UnpackFromPixelData(14, 16, true, SwapBytes(pixelData));
				var actualResult = FormatNonZeroBytes(extractedBits);
				Assert.AreEqual(expectedResult, actualResult, "Error in extracted bits from pixel data (16-bit pixels, big endian)");
			}
		}

		#endregion

		#region Algorithm Tests

		[Test]
		public void TestBitPacking_Single8BitFrame()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001

			const string expectedResult = "111111001000000";
			var overlayPixels = new byte[]
			                    	{
			                    		0x10, 0x10, 0x10, 0x10, 0x10,
			                    		0x10, 0x00, 0x00, 0x10, 0x00,
			                    		0x00, 0x00, 0x00, 0x00, 0x00
			                    	};

			// little endian test
			{
				const int pixelCount = 3*5;
				byte[] packedBits = new byte[(int) (Math.Ceiling(pixelCount/8.0))];
				OverlayData.TestPack(overlayPixels, packedBits, 0, pixelCount, 0xFF, false);
				var actualResult = FormatBits(packedBits, false).Substring(0, pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 8-bit case (little endian)");
			}

			// big endian test
			{
				const int pixelCount = 3*5;
				byte[] packedBits = new byte[(int) (Math.Ceiling(pixelCount/8.0))];
				OverlayData.TestPack(overlayPixels, packedBits, 0, pixelCount, 0xFF, true);
				var actualResult = FormatBits(packedBits, true).Substring(0, pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 8-bit case (big endian)");
			}
		}

		[Test]
		public void TestBitPacking_Single16BitFrame()
		{
			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001

			const string expectedResult = "111111001000000";
			var overlayPixels = SwapBytes(new byte[] // written in big endian for ease of reading
			                              	{
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
			                              		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
			                              	});

			// little endian test
			{
				const int pixelCount = 3*5;
				byte[] packedBits = new byte[(int) (Math.Ceiling(pixelCount/8.0))];
				OverlayData.TestPack(overlayPixels, packedBits, 0, pixelCount, (ushort) 0x00FF, false);
				var actualResult = FormatBits(packedBits, false).Substring(0, pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit case (little endian)");
			}

			// big endian test
			{
				const int pixelCount = 3*5;
				byte[] packedBits = new byte[(int) (Math.Ceiling(pixelCount/8.0))];
				OverlayData.TestPack(overlayPixels, packedBits, 0, pixelCount, (ushort) 0x00FF, true);
				var actualResult = FormatBits(packedBits, true).Substring(0, pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for 16-bit case (big endian)");
			}
		}

		[Test]
		public void TestBitPacking_Multiple8BitFrames()
		{
			// 7 frames, each 3x2 (cols, rows) = 42 bits = 6 bytes
			// 111 111 111 111 111 111 111
			// 000 001 010 011 100 101 110
			// continuous bit stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx
			// continuous LE byte stream: 11000111 01111001 11011101 11001111 11111011 xxxxxx01

			const string expectedResult = "111000111001111010111011111100111101111110";
			var overlayPixels = new byte[]
			                    	{
			                    		// frame 1
			                    		0x10, 0x10, 0x10,
			                    		0x00, 0x00, 0x00,
			                    		// frame 2
			                    		0x10, 0x10, 0x10,
			                    		0x00, 0x00, 0x10,
			                    		// frame 3
			                    		0x10, 0x10, 0x10,
			                    		0x00, 0x10, 0x00,
			                    		// frame 4
			                    		0x10, 0x10, 0x10,
			                    		0x00, 0x10, 0x10,
			                    		// frame 5
			                    		0x10, 0x10, 0x10,
			                    		0x10, 0x00, 0x00,
			                    		// frame 6
			                    		0x10, 0x10, 0x10,
			                    		0x10, 0x00, 0x10,
			                    		// frame 7
			                    		0x10, 0x10, 0x10,
			                    		0x10, 0x10, 0x00,
			                    	};

			// little endian test
			{
				const bool bigEndian = false;
				const int frames = 7;
				const int pixelCount = 2*3;
				byte[] packedBits = new byte[(int) (Math.Ceiling(frames*pixelCount/8.0))];
				// pack the individual frames in no specific order
				OverlayData.TestPack(Subarray(overlayPixels, 0*pixelCount, pixelCount), packedBits, 0*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 4*pixelCount, pixelCount), packedBits, 4*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 1*pixelCount, pixelCount), packedBits, 1*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 5*pixelCount, pixelCount), packedBits, 5*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*pixelCount, pixelCount), packedBits, 2*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 6*pixelCount, pixelCount), packedBits, 6*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 3*pixelCount, pixelCount), packedBits, 3*pixelCount, pixelCount, 0xFF, bigEndian);
				var actualResult = FormatBits(packedBits, bigEndian).Substring(0, frames*pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for multiframe 8-bit case (little endian)");
			}

			// big endian test
			{
				const bool bigEndian = true;
				const int frames = 7;
				const int pixelCount = 2*3;
				byte[] packedBits = new byte[(int) (Math.Ceiling(frames*pixelCount/8.0))];
				// pack the individual frames in no specific order
				OverlayData.TestPack(Subarray(overlayPixels, 0*pixelCount, pixelCount), packedBits, 0*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 4*pixelCount, pixelCount), packedBits, 4*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 1*pixelCount, pixelCount), packedBits, 1*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 5*pixelCount, pixelCount), packedBits, 5*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*pixelCount, pixelCount), packedBits, 2*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 6*pixelCount, pixelCount), packedBits, 6*pixelCount, pixelCount, 0xFF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 3*pixelCount, pixelCount), packedBits, 3*pixelCount, pixelCount, 0xFF, bigEndian);
				var actualResult = FormatBits(packedBits, bigEndian).Substring(0, frames*pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for multiframe 8-bit case (big endian)");
			}
		}

		[Test]
		public void TestBitPacking_Multiple16BitFrames()
		{
			// 7 frames, each 3x2 (cols, rows) = 42 bits = 6 bytes
			// 111 111 111 111 111 111 111
			// 000 001 010 011 100 101 110
			// continuous bit stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx
			// continuous LE byte stream: 11000111 01111001 11011101 11001111 11111011 xxxxxx01

			const string expectedResult = "111000111001111010111011111100111101111110";
			var overlayPixels = SwapBytes(new byte[] // written in big endian for convenience
			                              	{
			                              		// frame 1
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			                              		// frame 2
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
			                              		// frame 3
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
			                              		// frame 4
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x00, 0x00, 0x10, 0x00, 0x10,
			                              		// frame 5
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x00, 0x00, 0x00, 0x00,
			                              		// frame 6
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x00, 0x00, 0x00, 0x10,
			                              		// frame 7
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x10,
			                              		0x00, 0x10, 0x00, 0x10, 0x00, 0x00,
			                              	});

			// little endian test
			{
				const bool bigEndian = false;
				const int frames = 7;
				const int pixelCount = 2*3;
				byte[] packedBits = new byte[(int) (Math.Ceiling(frames*pixelCount/8.0))];
				// pack the individual frames in no specific order
				OverlayData.TestPack(Subarray(overlayPixels, 2*0*pixelCount, 2*pixelCount), packedBits, 0*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*4*pixelCount, 2*pixelCount), packedBits, 4*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*1*pixelCount, 2*pixelCount), packedBits, 1*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*5*pixelCount, 2*pixelCount), packedBits, 5*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*2*pixelCount, 2*pixelCount), packedBits, 2*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*6*pixelCount, 2*pixelCount), packedBits, 6*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*3*pixelCount, 2*pixelCount), packedBits, 3*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				var actualResult = FormatBits(packedBits, bigEndian).Substring(0, frames*pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for multiframe 8-bit case (little endian)");
			}

			// big endian test
			{
				const bool bigEndian = true;
				const int frames = 7;
				const int pixelCount = 2*3;
				byte[] packedBits = new byte[(int) (Math.Ceiling(frames*pixelCount/8.0))];
				// pack the individual frames in no specific order
				OverlayData.TestPack(Subarray(overlayPixels, 2*0*pixelCount, 2*pixelCount), packedBits, 0*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*4*pixelCount, 2*pixelCount), packedBits, 4*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*1*pixelCount, 2*pixelCount), packedBits, 1*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*5*pixelCount, 2*pixelCount), packedBits, 5*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*2*pixelCount, 2*pixelCount), packedBits, 2*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*6*pixelCount, 2*pixelCount), packedBits, 6*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				OverlayData.TestPack(Subarray(overlayPixels, 2*3*pixelCount, 2*pixelCount), packedBits, 3*pixelCount, pixelCount, (ushort) 0x00FF, bigEndian);
				var actualResult = FormatBits(packedBits, bigEndian).Substring(0, frames*pixelCount);
				Assert.AreEqual(expectedResult, actualResult, "Error in packed bits for multiframe 8-bit case (big endian)");
			}
		}

		[Test]
		public void TestBitUnpacking_SingleFrame()
		{
			byte[] testData;

			// 1 frame, 5x3
			// 11111
			// 10001
			// 11111
			// continuous bit stream: 11111100 0111111x
			// continuous LE byte stream: 00111111 x1111110
			// continuous BE word stream: x1111110 00111111
			testData = new byte[] {0x3f, 0x7e};
			TestFrame(testData, 5*3, 0, "111111000111111", "continuous stream: 11111100 0111111x");

			// 1 frame, 5x3
			// 11111
			// 10010
			// 00000
			// continuous bit stream: 11111100 1000000x
			// continuous LE byte stream: 00111111 x0000001
			testData = new byte[] {0x3f, 0x81};
			TestFrame(testData, 5*3, 0, "111111001000000", "continuous stream: 11111100 1000000x");
		}

		[Test]
		public void TestBitUnpacking_MultiFrame()
		{
			byte[] testData;

			// 7 frames, each 3x2 (cols, rows) = 42 bits = 6 bytes
			// 111 111 111 111 111 111 111
			// 000 001 010 011 100 101 110
			// continuous bit stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx
			// continuous LE byte stream: 11000111 01111001 11011101 11001111 11111011 xxxxxx01
			testData = new byte[] {0xc7, 0x79, 0xdd, 0xcf, 0xfb, 0xf1};
			TestFrame(testData, 3*2, 0, "111000", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 1, "111001", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 2, "111010", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 3, "111011", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 4, "111100", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 5, "111101", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
			TestFrame(testData, 3*2, 6, "111110", "continuous stream: 11100011 10011110 10111011 11110011 11011111 10xxxxxx");
		}

		private static void TestFrame(byte[] packedData, int outBufferSize, int frameNum, string expectedConcat, string datamsg)
		{
			byte[] result = new byte[outBufferSize];
			OverlayData.TestUnpack(packedData, result, frameNum*outBufferSize, outBufferSize, false);
			Assert.AreEqual(expectedConcat, FormatNonZeroBytes(result), "LittleEndianWords Frame {0} of {1}", frameNum, datamsg);

			// you should get the exact same frame data (scanning horizontally from top left to bottom right) if you had packed data in little endian or big endian
			byte[] swappedPackedData = SwapBytes(packedData);
			result = new byte[outBufferSize];
			OverlayData.TestUnpack(swappedPackedData, result, frameNum*outBufferSize, outBufferSize, true);
			Assert.AreEqual(expectedConcat, FormatNonZeroBytes(result), "BigEndianWords Frame {0} of {1}", frameNum, datamsg);
		}

		#endregion

		private static byte[] Subarray(byte[] array, int start, int length)
		{
			var subarray = new byte[length];
			Array.Copy(array, start, subarray, 0, length);
			return subarray;
		}

		private static byte[] SwapBytes(byte[] swapBytes)
		{
			byte[] output = new byte[swapBytes.Length];
			for (int n = 0; n < swapBytes.Length; n += 2)
			{
				output[n + 1] = swapBytes[n];
				output[n] = swapBytes[n + 1];
			}
			return output;
		}

		/// <summary>
		/// Formats the bits of a byte array as a sequence of 1s and 0s.
		/// </summary>
		private static string FormatBits(byte[] data, bool bigEndianWords)
		{
			var sb = new StringBuilder(data.Length*8);
			if (bigEndianWords)
			{
				for (int n = 0; n < data.Length; n += 2)
				{
					// the lsb is the 2nd byte in each big endian word
					// list each bit of the byte from lsb to msb
					for (int bit = 0; bit < 8; bit++)
						sb.Append((data[n + 1] >> bit)%2 == 1 ? '1' : '0');
					for (int bit = 0; bit < 8; bit++)
						sb.Append((data[n] >> bit)%2 == 1 ? '1' : '0');
				}
			}
			else
			{
				for (int n = 0; n < data.Length; n++)
				{
					// list each bit of the byte from lsb to msb
					for (int bit = 0; bit < 8; bit++)
						sb.Append((data[n] >> bit)%2 == 1 ? '1' : '0');
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Formats a byte array as a sequence of 1s and 0s representing non-zero and zero bytes respectively.
		/// </summary>
		private static string FormatNonZeroBytes(byte[] data)
		{
			var sb = new StringBuilder(data.Length);
			for (int n = 0; n < data.Length; n++)
				sb.Append(data[n] > 0 ? '1' : '0');
			return sb.ToString();
		}
	}
}

#endif