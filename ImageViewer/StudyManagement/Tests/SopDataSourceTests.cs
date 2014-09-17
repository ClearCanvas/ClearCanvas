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
#pragma warning disable 1591,0419,1574,1587

using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{
	[TestFixture]
	public class SopDataSourceTests : AbstractTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			GC.Collect();
		}

		[Test]
		public void TestGrayscalePixelNormalizationUnsigned8()
		{
			// test 8-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				7, 8, 8, false);

			// test 8-bit, lower nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0f, 0x0b, 0x0c, 0x09, 0x07, 0x08, 0x04, 0x0f, 0x05, 0x01},
				3, 4, 8, false);

			// test 8-bit, middle nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0b, 0x0e, 0x07, 0x06, 0x09, 0x06, 0x0d, 0x0f, 0x09, 0x00},
				5, 4, 8, false);

			// test 8-bit, upper nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0a, 0x0b, 0x09, 0x0d, 0x02, 0x01, 0x03, 0x0f, 0x06, 0x08},
				7, 4, 8, false);
		}

		[Test]
		public void TestGrayscalePixelNormalizationSigned8()
		{
			// test 8-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				7, 8, 8, true);

			// test 8-bit, lower nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xff, 0xfb, 0xfc, 0xf9, 0x07, 0xf8, 0x04, 0xff, 0x05, 0x01},
				3, 4, 8, true);

			// test 8-bit, middle nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xfb, 0xfe, 0x07, 0x06, 0xf9, 0x06, 0xfd, 0xff, 0xf9, 0x00},
				5, 4, 8, true);

			// test 8-bit, upper nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xfa, 0xfb, 0xf9, 0xfd, 0x02, 0x01, 0x03, 0xff, 0x06, 0xf8},
				7, 4, 8, true);
		}

		[Test]
		public void TestGrayscalePixelNormalizationUnsigned16()
		{
			// test 16-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, false);

			// test 16-bit, upper byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xbb, 0x00, 0xd9, 0x00, 0x18, 0x00, 0xff, 0x00, 0x81, 0x00},
				15, 8, 16, false);

			// test 16-bit, lower byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0x00, 0x9c, 0x00, 0x27, 0x00, 0x34, 0x00, 0x65, 0x00},
				7, 8, 16, false);

			// test 16-bit, middle byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xba, 0x00, 0x99, 0x00, 0x82, 0x00, 0xf3, 0x00, 0x16, 0x00},
				11, 8, 16, false);
		}

		[Test]
		public void TestGrayscalePixelNormalizationSigned16()
		{
			// test 16-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, true);

			// test 16-bit, upper byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xbb, 0xff, 0xd9, 0xff, 0x18, 0x00, 0xff, 0xff, 0x81, 0xff},
				15, 8, 16, true);

			// test 16-bit, lower byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xff, 0x9c, 0xff, 0x27, 0x00, 0x34, 0x00, 0x65, 0x00},
				7, 8, 16, true);

			// test 16-bit, middle byte
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xba, 0xff, 0x99, 0xff, 0x82, 0xff, 0xf3, 0xff, 0x16, 0x00},
				11, 8, 16, true);
		}

		private static void TryGrayscalePixelNormalization(byte[] input, byte[] expected, int highBit, int bitsStored, int bitsAllocated, bool signed)
		{
			DicomAttributeCollection coll = new DicomAttributeCollection();
			coll[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
			coll[DicomTags.BitsStored].SetInt32(0, bitsStored);
			coll[DicomTags.HighBit].SetInt32(0, highBit);
			coll[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);

			byte[] output = new byte[input.Length];
			input.CopyTo(output, 0);

			var bigEndian = ByteBuffer.LocalMachineEndian == Endian.Big;
			if (bigEndian)
			{
				// if the local machine is big endian, swap the bytes of input and expected around, as the tests were written for little endian

				var bb = new ByteBuffer(output, Endian.Little);
				bb.Swap2();

				var swappedExpected = new byte[expected.Length];
				input.CopyTo(swappedExpected, 0);
				bb = new ByteBuffer(swappedExpected, Endian.Little);
				bb.Swap2();
				expected = swappedExpected;
			}

			DicomMessageSopDataSource.NormalizeGrayscalePixels(coll, output);
			AssertArrayEquals(expected, output, string.Format("{0} Stored, {1} Allocated, High={2}, BigEndian={3}", bitsStored, bitsAllocated, highBit, bigEndian));
		}

		private static void AssertArrayEquals<T>(T[] expected, T[] actual, string message)
		{
			Assert.IsTrue(!(expected == null ^ actual == null), "Either both are null, or neither are null : {0}", message);
			if (expected == null)
				return;
			Assert.AreEqual(expected.Length, actual.Length, "Lengths differ : {0}", message);
			for (int n = 0; n < expected.Length; n++)
			{
				Assert.AreEqual(expected[n], actual[n], "Data differs at index {0} : {1}", n, message);
			}
		}
	}
}

#endif