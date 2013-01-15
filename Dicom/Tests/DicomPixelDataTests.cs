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

#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class DicomPixelDataTests
	{
		[Test]
		public void TestGetMinMaxPixelData()
		{
			Assert.AreEqual(0, DicomPixelData.GetMinPixelValue(7, false));
			Assert.AreEqual(0, DicomPixelData.GetMinPixelValue(8, false));
			Assert.AreEqual(0, DicomPixelData.GetMinPixelValue(12, false));
			Assert.AreEqual(0, DicomPixelData.GetMinPixelValue(16, false));

			Assert.AreEqual(127, DicomPixelData.GetMaxPixelValue(7, false));
			Assert.AreEqual(255, DicomPixelData.GetMaxPixelValue(8, false));
			Assert.AreEqual(4095, DicomPixelData.GetMaxPixelValue(12, false));
			Assert.AreEqual(65535, DicomPixelData.GetMaxPixelValue(16, false));

			Assert.AreEqual(-64, DicomPixelData.GetMinPixelValue(7, true));
			Assert.AreEqual(-128, DicomPixelData.GetMinPixelValue(8, true));
			Assert.AreEqual(-2048, DicomPixelData.GetMinPixelValue(12, true));
			Assert.AreEqual(-32768, DicomPixelData.GetMinPixelValue(16, true));

			Assert.AreEqual(63, DicomPixelData.GetMaxPixelValue(7, true));
			Assert.AreEqual(127, DicomPixelData.GetMaxPixelValue(8, true));
			Assert.AreEqual(2047, DicomPixelData.GetMaxPixelValue(12, true));
			Assert.AreEqual(32767, DicomPixelData.GetMaxPixelValue(16, true));
		}

		#region Unused Bits

		[Test]
		public void TestZeroUnusedBits_BitsStored8_NoOp()
		{
			var testValues = new byte[] { 128, 255 };
			var expectedValues = (byte[])testValues.Clone();
			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(expectedValues, 8, 7));
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored7()
		{
			// Actual: 64, 127, 127 (with extra bit at left=255)
			// Test: 0\1000000, 0\1111111, 1\1111111
			// Expected: 64, 127, 127

			var testValues = new byte[] { 64, 127, 255 };
			var expectedValues = new byte[] {64, 127, 127};
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 7, 6));
			Assert.AreEqual(expectedValues, testValues);

			//Won't have zeroed anything.
			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 7, 6));
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored4_HighBit5()
		{
			// Actual: 15, [15, 15, 15, 10, 5, 3] (all with varying extra bits at both ends)
			// Test: 00\1111\00, 11\1111\11, 01\1111\10, 10\1111\01, 10\1010\10, 01\0101\01, 11\0011\00
			// Expected: 60, 60, 60, 60, 40, 20, 12 (actual, but not right-aligned)

			var testValues = new byte[] { 60, 255, 126, 189, 170, 85, 204 };
			var expectedValues = new byte[] {60, 60, 60, 60, 40, 20, 12};
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 4, 5));
			Assert.AreEqual(expectedValues, testValues);

			//Won't have zeroed anything.
			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 4, 5));
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored16_NoOp()
		{
			var testValues = new ushort[] {65535, 32767};
			var expectedValues = (ushort[]) testValues.Clone();
			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(expectedValues, 16, 15));
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored15()
		{
			// Actual: 32767, 21845, 10922 (with extra bit at left=43690)
			// Test: 0\111111111111111, 0\101010101010101, 1\010101010101010
			// Expected: 32767, 21845, 10922

			var testValues = new ushort[] { 32767, 21845, 43690 };
			var expectedValues = new ushort[] {32767, 21845, 10922};
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 15, 14));
			Assert.AreEqual(expectedValues, testValues);

			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 15, 14));
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored10_HighBit12()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 8184, 8184, 8184, 8184, 2728, 5456

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {8184, 8184, 8184, 8184, 2728, 5456};
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 10, 12));
			Assert.AreEqual(expectedValues, testValues);

			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 10, 12));
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored10_HighBit12_AsBytes()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 8184, 8184, 8184, 8184, 2728, 5456

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {8184, 8184, 8184, 8184, 2728, 5456};

			var testValuesBytes = ByteConverter.ToByteArray(testValues); //to bytes
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValuesBytes, 16, 10, 12));

			testValues = ByteConverter.ToUInt16Array(testValuesBytes); //back to ushorts
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored10_HighBit12_OpposingEndian()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 8184, 8184, 8184, 8184, 2728, 5456

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {8184, 8184, 8184, 8184, 2728, 5456};

			var opposingEndian = (ByteBuffer.LocalMachineEndian == Endian.Big) ? Endian.Little : Endian.Big;
			ByteConverter.SwapBytes(testValues);
			
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 10, 12, opposingEndian));

			ByteConverter.SwapBytes(testValues);
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestZeroUnusedBits_BitsStored10_HighBit12_AsBytes_OpposingEndian()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 8184, 8184, 8184, 8184, 2728, 5456

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {8184, 8184, 8184, 8184, 2728, 5456};

			var opposingEndian = (ByteBuffer.LocalMachineEndian == Endian.Big) ? Endian.Little : Endian.Big;
			ByteConverter.SwapBytes(testValues); //swap
			var testValuesBytes = ByteConverter.ToByteArray(testValues); //to bytes

			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValuesBytes, 16, 10, 12, opposingEndian));
			testValues = ByteConverter.ToUInt16Array(testValuesBytes); //back to ushort
			ByteConverter.SwapBytes(testValues);//un-swap

			Assert.AreEqual(expectedValues, testValues);	

			Assert.IsFalse(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 10, 12));
		}

		#endregion

		#region Right Align

		public void TestRightAlign_BitsStored8_NoOp()
		{
			var testValues = new byte[] { 128, 255 };
			var expectedValues = (byte[])testValues.Clone();
			Assert.IsFalse(DicomUncompressedPixelData.RightAlign(expectedValues, 8, 7));
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestRightAlign_BitsStored7_NoOp()
		{
			// 64, 127
			// 0\1000000, 0\1111111
			var testValues = new byte[] { 64, 127 };
			var expectedValues = (byte[])testValues.Clone();
			Assert.IsFalse(DicomUncompressedPixelData.RightAlign(testValues, 7, 6));
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestRightAlign_BitsStored4_HighBit5()
		{
			// Actual: 15, [15, 15, 15, 10, 5, 3] (all with varying extra bits at both ends)
			// Test: 00\1111\00, 11\1111\11, 01\1111\10, 10\1111\01, 10\1010\10, 01\0101\01, 11\0011\00
			// Expected: 15, 15, 15, 15, 10, 5, 3

			var testValues = new byte[] { 60, 255, 126, 189, 170, 85, 204 };
			var expectedValues = new byte[] {15, 15, 15, 15, 10, 5, 3};
			TestRightAlign(testValues, 4, 5, expectedValues);
		}

		[Test]
		public void TestRightAlign_BitsStored16_NoOp()
		{
			var testValues = new ushort[] { 65535, 32767 };
			var expectedValues = (ushort[])testValues.Clone();
			Assert.IsFalse(DicomUncompressedPixelData.RightAlign(expectedValues, 16, 15));
			Assert.AreEqual(testValues, expectedValues);
		}

		[Test]
		public void TestRightAlign_BitsStored15_NoOp()
		{
			// Actual: 32767, 21845, 10922 (with extra bit at left=43690)
			// Test: 0\111111111111111, 0\101010101010101, 1\010101010101010
			// Expected: 32767, 21845, 10922

			var testValues = new ushort[] { 32767, 21845, 43690 };
			var expectedValues = new ushort[] {32767, 21845, 10922};

			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, 15, 14));
			Assert.IsFalse(DicomUncompressedPixelData.RightAlign(testValues, 16, 15));
			Assert.AreEqual(expectedValues, testValues);
		}

		[Test]
		public void TestRightAlign_BitsStored10_HighBit12()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 1023, 1023, 1023, 1023, 341, 682

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {1023, 1023, 1023, 1023, 341, 682};
			TestRightAlign(testValues, 10, 12, expectedValues, false, false);
		}

		[Test]
		public void TestRightAlign_BitsStored10_HighBit12_AsBytes()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 1023, 1023, 1023, 1023, 341, 682

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {1023, 1023, 1023, 1023, 341, 682};
			TestRightAlign(testValues, 10, 12, expectedValues, true, false);
		}

		[Test]
		public void TestRightAlign_BitsStored10_HighBit12_OpposingEndian()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 1023, 1023, 1023, 1023, 341, 682

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {1023, 1023, 1023, 1023, 341, 682};
			TestRightAlign(testValues, 10, 12, expectedValues, false, true);
		}

		[Test]
		public void TestRightAlign_BitsStored10_HighBit12_AsBytes_OpposingEndian()
		{
			// Actual: 1023, 1023, 1023, 1023, 341, 682 (all with varying extra bits at both ends)
			// Test: 111\1111111111\111, 010\1111111111\101, 101\1111111111\010, 000\1111111111\000, 111\0101010101\111, 010\1010101010\101
			// Expected: 1023, 1023, 1023, 1023, 341, 682

			var testValues = new ushort[] { 65535, 24573, 49146, 8184, 60079, 21845 };
			var expectedValues = new ushort[] {1023, 1023, 1023, 1023, 341, 682};
			TestRightAlign(testValues, 10, 12, expectedValues, true, true);
		}

		[Test]
		public void TestRightAlign_BitsStored6_HighBit6_Signed()
		{
			// Actual: -32, -17, -6, 2, 6, 15, 31 (all with varying extra bits at both ends)
			// Test: 1\100000\1, 1\101111\0, 1\111010\1, 0\000010\0, 1\000110\1, 0\001111\0, 1\011111\0
			// Expected: -32, -17, -6, 2, 6, 15, 31

			var testValues = new byte[] { 0xC1, 0xDE, 0xF5, 0x4, 0x8D, 0x1E, 0xBE }; //Test values in hex
			var expectedValues = new sbyte[] { -32, -17, -6, 2, 6, 15, 31 };
			TestRightAlignSigned(testValues, 6, 6, expectedValues);
		}

		[Test]
		public void TestRightAlign_BitsStored12_HighBit13_Signed()
		{
			// Actual: -2048, -1111, -107, -2, 6, 107, 1113, 2047
			// Test: 11\100000000000\10, 10\101110101001\11, 01\111110010101\00, 00\111111111110\01, 01\000000000110\00, 10\000001101011\11, 01\010001011001\00, 11\011111111111\01
			// Expected: -2048, -1111, -107, -2, 6, 107, 1113, 2047

			var testValues = new ushort[] { 0xE002, 0xAEA7, 0x7E54, 0x3FF9, 0x4018, 0x81AF, 0x5164, 0xDFFD }; //Test values in hex
			var expectedValues = new short[] { -2048, -1111, -107, -2, 6, 107, 1113, 2047 };
			TestRightAlignSigned(testValues, 12, 13, expectedValues, false, false);
		}

		[Test]
		public void TestRightAlign_BitsStored12_HighBit13_Signed_AsBytes()
		{
			// Actual: -2048, -1111, -107, -2, 6, 107, 1113, 2047
			// Test: 11\100000000000\10, 10\101110101001\11, 01\111110010101\00, 00\111111111110\01, 01\000000000110\00, 10\000001101011\11, 01\010001011001\00, 11\011111111111\01
			// Expected: -2048, -1111, -107, -2, 6, 107, 1113, 2047

			var testValues = new ushort[] { 0xE002, 0xAEA7, 0x7E54, 0x3FF9, 0x4018, 0x81AF, 0x5164, 0xDFFD }; //Test values in hex
			var expectedValues = new short[] { -2048, -1111, -107, -2, 6, 107, 1113, 2047 };
			TestRightAlignSigned(testValues, 12, 13, expectedValues, true, false);
		}

		[Test]
		public void TestRightAlign_BitsStored12_HighBit13_Signed_OpposingEndian()
		{
			// Actual: -2048, -1111, -107, -2, 6, 107, 1113, 2047
			// Test: 11\100000000000\10, 10\101110101001\11, 01\111110010101\00, 00\111111111110\01, 01\000000000110\00, 10\000001101011\11, 01\010001011001\00, 11\011111111111\01
			// Expected: -2048, -1111, -107, -2, 6, 107, 1113, 2047

			var testValues = new ushort[] { 0xE002, 0xAEA7, 0x7E54, 0x3FF9, 0x4018, 0x81AF, 0x5164, 0xDFFD }; //Test values in hex
			var expectedValues = new short[] { -2048, -1111, -107, -2, 6, 107, 1113, 2047 };
			TestRightAlignSigned(testValues, 12, 13, expectedValues, false, true);
		}

		[Test]
		public void TestRightAlign_BitsStored12_HighBit13_Signed_AsBytes_OpposingEndian()
		{
			// Actual: -2048, -1111, -107, -2, 6, 107, 1113, 2047
			// Test: 11\100000000000\10, 10\101110101001\11, 01\111110010101\00, 00\111111111110\01, 01\000000000110\00, 10\000001101011\11, 01\010001011001\00, 11\011111111111\01
			// Expected: -2048, -1111, -107, -2, 6, 107, 1113, 2047

			var testValues = new ushort[] { 0xE002, 0xAEA7, 0x7E54, 0x3FF9, 0x4018, 0x81AF, 0x5164, 0xDFFD }; //Test values in hex
			var expectedValues = new short[] { -2048, -1111, -107, -2, 6, 107, 1113, 2047 };
			TestRightAlignSigned(testValues, 12, 13, expectedValues, true, true);
		}

		private static void TestRightAlign(byte[] testValues, int bitsStored, int highBit, byte[] expectedValues)
		{
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, bitsStored, highBit));
			Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValues, bitsStored, highBit));
			Assert.AreEqual(expectedValues, testValues); //Expected values in hex
		}

		private static void TestRightAlignSigned(byte[] testValues, int bitsStored, int highBit, sbyte[] expectedValues)
		{
			Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, bitsStored, highBit));
			Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValues, bitsStored, highBit));

			Assert.AreEqual(expectedValues, DicomSignedToSigned(testValues, highBit - DicomPixelData.GetLowBit(bitsStored, highBit)));
		}

		private static void TestRightAlign(ushort[] testValues, int bitsStored, int highBit, ushort[] expectedValues, bool asBytes, bool asOpposingEndian)
		{
			TestRightAlign(ref testValues, bitsStored, highBit, asBytes, asOpposingEndian);
			Assert.AreEqual(expectedValues, testValues);
		}

		private static void TestRightAlignSigned(ushort[] testValues, int bitsStored, int highBit, short[] expectedValues, bool asBytes, bool asOpposingEndian)
		{
			TestRightAlign(ref testValues, bitsStored, highBit, asBytes, asOpposingEndian);
			var newHighBit = highBit - DicomPixelData.GetLowBit(bitsStored, highBit);
			Assert.AreEqual(expectedValues, DicomSignedToSigned(testValues, newHighBit));
		}

		private static void TestRightAlign(ref ushort[] testValues, int bitsStored, int highBit, bool asBytes, bool asOpposingEndian)
		{
			if (!asBytes)
			{
				if (asOpposingEndian)
				{
					var opposingEndian = (ByteBuffer.LocalMachineEndian == Endian.Big) ? Endian.Little : Endian.Big;
					ByteConverter.SwapBytes(testValues);
					Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, bitsStored, highBit, opposingEndian));
					Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValues, bitsStored, highBit, opposingEndian));
					ByteConverter.SwapBytes(testValues);
				}
				else
				{
					Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValues, bitsStored, highBit));
					Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValues, bitsStored, highBit));
				}
			}
			else
			{
				if (asOpposingEndian)
				{
					ByteConverter.SwapBytes(testValues);
					var testValuesBytes = ByteConverter.ToByteArray(testValues);

					var opposingEndian = (ByteBuffer.LocalMachineEndian == Endian.Big) ? Endian.Little : Endian.Big;
					Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValuesBytes, 16, bitsStored, highBit, opposingEndian));
					Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValuesBytes, 16, bitsStored, highBit, opposingEndian));
					
					testValues = ByteConverter.ToUInt16Array(testValuesBytes);
					ByteConverter.SwapBytes(testValues);
				}
				else
				{
					var testValuesBytes = ByteConverter.ToByteArray(testValues);
					Assert.IsTrue(DicomUncompressedPixelData.ZeroUnusedBits(testValuesBytes, 16, bitsStored, highBit));
					Assert.IsTrue(DicomUncompressedPixelData.RightAlign(testValuesBytes, 16, bitsStored, highBit));
					testValues = ByteConverter.ToUInt16Array(testValuesBytes);
				}
			}
		}

		#endregion

		#region Toggle Pixel Representation

		[Test]
		public void TestTogglePixelRep_BitsStored8_HighBit7()
		{
			//Test: -128, -95, -27, -1, 0, 27, 93, 127
			//Test (bin): 10000000, 10100001, 11100101, 11111111, 00000000, 00011011, 01011101, 01111111

			const int bitsAllocated = 8;
			const int bitsStored = 8;
			const int highBit = 7;

			var testValues = new byte[] {0x80, 0xA1, 0xE5, 0xFF, 0x0, 0x1B, 0x5D, 0x7F};
			var expectedValues = new byte[] { 0,33,101,127,128,155,221,255 };

			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 128);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored7_HighBit6()
		{
			//Test: -64, -31, -8, -1, 0, 13, 43, 63
			//Test (bin): 01000000, 01100001, 01111000, 01111111, 00000000, 00001101, 00101011, 00111111

			const int bitsAllocated = 8;
			const int bitsStored = 7;
			const int highBit = 6;

			var testValues = new byte[] {0x40, 0x61, 0x78, 0x7F, 0x0, 0xD, 0x2B, 0x3F};
			var expectedValues = new byte[] { 0,33,56,63,64,77,107,127};
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 64);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored7_HighBit7()
		{
			//Test: -64, -31, -8, -1, 0, 13, 43, 63
			//Test (bin): 10000000, 11000010, 11110000, 11111110, 00000000, 00011010, 01010110, 01111110

			const int bitsAllocated = 8;
			const int bitsStored = 7;
			const int highBit = 7;

			var testValues = new byte[] {0x80, 0xC2, 0xF0, 0xFE, 0x0, 0x1A, 0x56, 0x7E};
			var expectedValues = new byte[] { 0,33,56,63,64,77,107,127};
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 64);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored6_HighBit7()
		{
			//Test: -32, -16, -8, -1, 0, 7, 13, 31
			//Test(bin): 10000000, 11000000, 11100000, 11111100, 00000000, 00011100, 00110100, 01111100

			const int bitsAllocated = 8;
			const int bitsStored = 6;
			const int highBit = 7;

			var testValues = new byte[] { 0x80, 0xC0, 0xE0, 0xFC, 0x0, 0x1C, 0x34, 0x7C };
			var expectedValues = new byte[] { 0,16,24,31,32,39,45,63 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 32);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored6_HighBit6()
		{
			//Test: -32, -16, -8, -1, 0, 7, 13, 31
			//Test(bin): 01000000, 01100000, 01110000, 01111110, 00000000, 00001110, 00011010, 00111110

			const int bitsAllocated = 8;
			const int bitsStored = 6;
			const int highBit = 6;

			var testValues = new byte[] { 0x40, 0x60, 0x70, 0x7E, 0x0, 0xE, 0x1A, 0x3E};
			var expectedValues = new byte[] { 0,16,24,31,32,39,45,63};
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 32);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored5_HighBit6()
		{
			//Test: -16, -11, -7, -1, 0, 7, 11, 15
			//Test(bin): 01000000, 01010100, 01100100, 01111100, 00000000, 00011100, 00101100, 00111100

			const int bitsAllocated = 8;
			const int bitsStored = 5;
			const int highBit = 6;

			var testValues = new byte[] { 0x40, 0x54, 0x64, 0x7C, 0x0, 0x1C, 0x2C, 0x3C };
			var expectedValues = new byte[] { 0,5,9,15,16,23,27,31 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 16);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored5_HighBit4()
		{
			//Test: -16, -11, -7, -1, 0, 7, 11, 15
			//Test(bin): 00010000, 00010101, 00011001, 00011111, 00000000, 00000111, 00001011, 00001111

			const int bitsAllocated = 8;
			const int bitsStored = 5;
			const int highBit = 4;

			var testValues = new byte[] { 0x10, 0x15, 0x19, 0x1F, 0x0, 0x7, 0xB, 0xF};
			var expectedValues = new byte[] { 0,5,9,15,16,23,27,31};
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);
			Assert.AreEqual(expectedValues, testValues);
			Assert.AreEqual(rescale, 16);
		}

		[Test]
		public unsafe void TestTogglePixelRep_BitsStored16_HighBit15()
		{
			//Test: -32768, -21101, -9999, -3, 0, 3, 9787, 21397, 32767
			//Test(bin): 10000000 00000000, 10101101 10010011, 11011000 11110001, 11111111 11111101, 00000000 00000000, 00000000 00000011, 00100110 00111011, 01010011 10010101, 01111111 11111111

			const int bitsAllocated = 16;
			const int bitsStored = 16;
			const int highBit = 15;

			var testValues = ByteConverter.ToByteArray(new ushort[] {0x8000, 0xAD93, 0xD8F1, 0xFFFD, 0x00, 0x03, 0x263B, 0x5395, 0x7FFF});
			var expectedValues = new ushort[] { 0, 11667, 22769, 32765, 32768, 32771, 42555, 54165, 65535 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);

			Assert.AreEqual(expectedValues, ByteConverter.ToUInt16Array(testValues));
			Assert.AreEqual(rescale, 32768);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored15_HighBit15()
		{
			//Test: -16384, -9817, -4331, -3, 0, 3, 4331, 9817, 16383
			//Test(bin): 10000000 00000000, 10110011 01001110, 11011110 00101010, 11111111 11111010, 00000000 00000000, 00000000 00000110, 00100001 11010110, 01001100 10110010, 01111111 11111110, 

			const int bitsAllocated = 16;
			const int bitsStored = 15;
			const int highBit = 15;

			var testValues = ByteConverter.ToByteArray(new ushort[] { 0x8000, 0xB34E, 0xDE2A, 0xFFFA, 0x00, 0x06, 0x21D6, 0x4CB2, 0x7FFE });
			var expectedValues = new ushort[] { 0,6567,12053,16381,16384,16387,20715,26201,32767 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);

			Assert.AreEqual(expectedValues, ByteConverter.ToUInt16Array(testValues));
			Assert.AreEqual(rescale, 16384);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored14_HighBit15()
		{
			//Test: -8192, -4331, -2177, -3, 0, 3, 2177, 4331, 8191
			//Test(bin): 10000000 00000000, 10111100 01010100, 11011101 11111100, 11111111 11110100, 00000000 00000000, 00000000 00001100, 00100010 00000100, 01000011 10101100, 01111111 11111100, 

			const int bitsAllocated = 16;
			const int bitsStored = 14;
			const int highBit = 15;

			var testValues = ByteConverter.ToByteArray(new ushort[] { 0x8000, 0xBC54, 0xDDFC, 0xFFF4, 0x00, 0x0C, 0x2204, 0x43AC, 0x7FFC });
			var expectedValues = new ushort[] { 0,3861,6015,8189,8192,8195,10369,12523,16383 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);

			Assert.AreEqual(expectedValues, ByteConverter.ToUInt16Array(testValues));
			Assert.AreEqual(rescale, 8192);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored12_HighBit13()
		{
			//Test: -2048, -1111, -787, -3, 0, 3, 763, 1567, 2047
			//Test(bin): 00100000 00000000, 00101110 10100100, 00110011 10110100, 00111111 11110100, 00000000 00000000, 00000000 00001100, 00001011 11101100, 00011000 01111100, 00011111 11111100, 

			const int bitsAllocated = 16;
			const int bitsStored = 12;
			const int highBit = 13;

			var testValues = ByteConverter.ToByteArray(new ushort[] { 0x2000, 0x2EA4, 0x33B4, 0x3FF4, 0x00, 0x0C, 0xBEC, 0x187C, 0x1FFC});
			var expectedValues = new ushort[] { 0,937,1261,2045,2048,2051,2811,3615,4095};
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);

			Assert.AreEqual(expectedValues, ByteConverter.ToUInt16Array(testValues));
			Assert.AreEqual(rescale, 2048);
		}

		[Test]
		public void TestTogglePixelRep_BitsStored12_HighBit11()
		{
			//Test: -2048, -1111, -787, -3, 0, 3, 763, 1567, 2047
			//Test(bin): 00001000 00000000, 00001011 10101001, 00001100 11101101, 00001111 11111101, 00000000 00000000, 00000000 00000011, 00000010 11111011, 00000110 00011111, 00000111 11111111, 

			const int bitsAllocated = 16;
			const int bitsStored = 12;
			const int highBit = 11;

			var testValues = ByteConverter.ToByteArray(new ushort[] { 0x800, 0xBA9, 0xCED, 0xFFD, 0x00, 0x03, 0x2FB, 0x61F, 0x7FF });
			var expectedValues = new ushort[] { 0,937,1261,2045,2048,2051,2811,3615,4095 };
			int rescale;
			DicomUncompressedPixelData.TogglePixelRepresentation(testValues, highBit, bitsStored, bitsAllocated, out rescale);

			Assert.AreEqual(expectedValues, ByteConverter.ToUInt16Array(testValues));
			Assert.AreEqual(rescale, 2048);
		}

		#endregion

		private static sbyte[] DicomSignedToSigned(byte[] shorts, int highBit)
		{
			var shift = 7 - highBit;
			var values = new sbyte[shorts.Length];
			for (int i = 0; i < values.Length; i++)
				values[i] = (sbyte)(((sbyte)(shorts[i] << shift)) >> shift);
			return values;
		}

		private static short[] DicomSignedToSigned(ushort[] shorts, int highBit)
		{
			var shift = 15 - highBit;
			var values = new short[shorts.Length];
			for (int i = 0; i < values.Length; i++)
				values[i] = (short)(((short)(shorts[i] << shift)) >> shift);
			return values;
		}
	}
}

#endif