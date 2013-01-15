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

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Utilities.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.IO.Tests
{
	/// <summary>
	/// Unit tests for <see cref="ByteBuffer"/>.
	/// </summary>
	/// <remarks>
	/// The unit tests here are implemented to match the original ByteBuffer's functionality.
	/// The API of the class is so highly inconsistent in terms of behaviour that it makes little sense to define a logical "expected" behavioural
	/// specification against which to test... instead we'll just test against what the ByteBuffer is expected to do, because it's so tightly integrated
	/// in some of the core parts of the DICOM I/O code that we really don't want to revisit it anytime soon.
	/// </remarks>
	[TestFixture]
	public class ByteBufferTests
	{
		[TestFixtureSetUp]
		public virtual void Initialize()
		{
			const string message = "Note: Performance statistics may not be precise when executed inside the unit test runner";
			Console.WriteLine(message);

			TestByteBuffer.UseHighCapacityMode = false;
		}

		[TestFixtureTearDown]
		public virtual void Deinitialize() {}

		[Test]
		public void TestChop()
		{
			var data0 = RandomBytes(6000, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Chop(6000);
				Assert.AreEqual(0, bb.Length, "Chop(6000) - Length");
				Assert.AreEqual(0, bb.StreamPosition, "Chop(6000) - StreamPosition");
				AssertAreEqual(new byte[0], bb.DumpTestData(), "Chop(6000)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Chop(401);
				Assert.AreEqual(6000 - 401, bb.Length, "Chop(401) - Length");
				Assert.AreEqual(0, bb.StreamPosition, "Chop(401) - StreamPosition");
				AssertAreEqual(data0, 401, bb.DumpTestData(), 0, 5599, "Chop(401)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Chop(6000);
				Assert.AreEqual(0, bb.Length, "Chop(6000) - Length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "Chop(6000) - StreamPosition - after Stream initialized");
				AssertAreEqual(new byte[0], bb.DumpTestData(), "Chop(6000) - after Stream initialized");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Chop(401);
				Assert.AreEqual(6000 - 401, bb.Length, "Chop(401) - Length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "Chop(401) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, 401, bb.DumpTestData(), 0, 5599, "Chop(401) - after Stream initialized");
			}
		}

		[Test]
		public void TestAppend()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);
			var data1 = RandomBytes(8472, -0x7BFC1702);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Append(data1, 4195, 4194);
				Assert.AreEqual(8472 + 4194, bb.Length, "Append - length");
				Assert.AreEqual(0, bb.StreamPosition, "Append - StreamPosition");
				AssertAreEqual(data0, 0, bb.DumpTestData(), 0, 8472, "Append - original data");
				AssertAreEqual(data1, 4195, bb.DumpTestData(), 8472, 4194, "Append - appended data");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Append(data1, 4195, 4194);
				Assert.AreEqual(8472 + 4194, bb.Length, "Append - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "Append - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, 0, bb.DumpTestData(), 0, 8472, "Append - original data - after Stream initialized");
				AssertAreEqual(data1, 4195, bb.DumpTestData(), 8472, 4194, "Append - appended data - after Stream initialized");
			}
		}

		[Test]
		public void TestCopyFrom()
		{
			var data0 = RandomBytes(9432, -0x02F0B4EF);
			var data1 = RandomBytes(8472, -0x7BFC1702);

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream(TestByteBuffer.CopyBytes(data1)))
			{
				var result = bb.CopyFrom(ms, 4195);
				Assert.AreEqual(4195, bb.Length, "CopyFrom - length");
				Assert.AreEqual(0, bb.StreamPosition, "CopyFrom - StreamPosition");
				Assert.AreEqual(4195, result, "CopyFrom - first batch return value");
				AssertAreEqual(data1, 0, bb.DumpTestData(), 0, 4195, "CopyFrom - first batch data");

				result = bb.CopyFrom(ms, 9000);
				Assert.AreEqual(9000, bb.Length, "CopyFrom - length"); // apparently the length of the buffer after CopyFrom is the bytes requested, and not actual bytes read
				Assert.AreEqual(0, bb.StreamPosition, "CopyFrom - StreamPosition");
				Assert.AreEqual(4277, result, "CopyFrom - second batch return value");
				AssertAreEqual(data1, 4195, bb.DumpTestData(), 0, 4277, "CopyFrom - second batch data");
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream(TestByteBuffer.CopyBytes(data1)))
			{
				bb.InitializeStream(position : 42);

				var result = bb.CopyFrom(ms, 4195);
				Assert.AreEqual(4195, bb.Length, "CopyFrom - length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "CopyFrom - StreamPosition - after Stream initialized");
				Assert.AreEqual(4195, result, "CopyFrom - first batch return value - after Stream initialized");
				AssertAreEqual(data1, 0, bb.DumpTestData(), 0, 4195, "CopyFrom - first batch data - after Stream initialized");

				bb.InitializeStream(position : 42);

				result = bb.CopyFrom(ms, 9000);
				Assert.AreEqual(9000, bb.Length, "CopyFrom - length - after Stream initialized"); // apparently the length of the buffer after CopyFrom is the bytes requested, and not actual bytes read
				Assert.AreEqual(0, bb.StreamPosition, "CopyFrom - StreamPosition - after Stream initialized");
				Assert.AreEqual(4277, result, "CopyFrom - second batch return value - after Stream initialized");
				AssertAreEqual(data1, 4195, bb.DumpTestData(), 0, 4277, "CopyFrom - second batch data - after Stream initialized");
			}
		}

		[Test]
		public void TestCopyToBinaryWriter()
		{
			var data0 = RandomBytes(6001, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.CopyTo(bw);

				Assert.AreEqual(6001, ms.Length, "CopyTo(BinaryWriter) - length");
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(BinaryWriter) - StreamPosition");
				AssertAreEqual(data0, ToArray(ms), "CopyTo(BinaryWriter) - destination data");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter) - source data");
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.InitializeStream(position : 42);
				bb.CopyTo(bw);

				Assert.AreEqual(6001, ms.Length, "CopyTo(BinaryWriter) - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(BinaryWriter) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, ToArray(ms), "CopyTo(BinaryWriter) - destination data after Stream initialized");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter) - source data after Stream initialized");
			}
		}

		[Test]
		public void TestCopyToBinaryWriterLargeFileStream()
		{
			var data0 = RandomBytes(6001001, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			using (var fs = new TempFileStream())
			using (var bw = new BinaryWriter(fs))
			{
				bb.CopyTo(bw);

				Assert.AreEqual(6001001, fs.Length, "CopyTo(BinaryWriter(FileStream)) - length");
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(BinaryWriter(FileStream)) - StreamPosition");
				AssertAreEqual(data0, ToArray(fs), "CopyTo(BinaryWriter(FileStream)) - destination data");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter(FileStream)) - source data");
			}

			using (var bb = new TestByteBuffer(data0))
			using (var fs = new TempFileStream())
			using (var bw = new BinaryWriter(fs))
			{
				bb.InitializeStream(position : 42);
				bb.CopyTo(bw);

				Assert.AreEqual(6001001, fs.Length, "CopyTo(BinaryWriter(FileStream)) - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(BinaryWriter(FileStream)) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, ToArray(fs), "CopyTo(BinaryWriter(FileStream)) - destination data after Stream initialized");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter(FileStream)) - source data after Stream initialized");
			}
		}

		[Test]
		public void TestCopyToBinaryWriterLargeMemoryStream()
		{
			var data0 = RandomBytes(6001001, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.CopyTo(bw);

				Assert.AreEqual(6001001, ms.Length, "CopyTo(BinaryWriter(MemoryStream)) - length");
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(BinaryWriter(MemoryStream)) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, ToArray(ms), "CopyTo(BinaryWriter(MemoryStream)) - destination data");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter(MemoryStream)) - source data");
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.InitializeStream(position : 42);
				bb.CopyTo(bw);

				Assert.AreEqual(6001001, ms.Length, "CopyTo(BinaryWriter(MemoryStream)) - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(BinaryWriter(MemoryStream)) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, ToArray(ms), "CopyTo(BinaryWriter(MemoryStream)) - destination data after Stream initialized");
				AssertAreEqual(data0, bb.DumpTestData(), "CopyTo(BinaryWriter(MemoryStream)) - source data after Stream initialized");
			}
		}

		[Test]
		public void TestCopyToStream()
		{
			var data0 = RandomBytes(7101, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			{
				bb.CopyTo(ms, 1237, 4193);

				Assert.AreEqual(4193, ms.Length, "CopyTo(Stream,int,int) - length");
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(Stream,int,int) - StreamPosition");
				AssertAreEqual(data0, 1237, ToArray(ms), 0, 4193, "CopyTo(Stream,int,int)");

				bb.CopyTo(ms, 642, 942);

				Assert.AreEqual(4193 + 942, ms.Length, "CopyTo(Stream,int,int) - length after append");
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(Stream,int,int) - StreamPosition after append");
				AssertAreEqual(data0, 1237, ToArray(ms), 0, 4193, "CopyTo(Stream,int,int) - existing data");
				AssertAreEqual(data0, 642, ToArray(ms), 4193, 942, "CopyTo(Stream,int,int) - new appended data");
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			{
				bb.InitializeStream(position : 42);
				bb.CopyTo(ms, 1237, 4193);

				Assert.AreEqual(4193, ms.Length, "CopyTo(Stream,int,int) - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(Stream,int,int) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, 1237, ToArray(ms), 0, 4193, "CopyTo(Stream,int,int) - after Stream initialized");

				bb.CopyTo(ms, 642, 942);

				Assert.AreEqual(4193 + 942, ms.Length, "CopyTo(Stream,int,int) - length after append - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(Stream,int,int) - StreamPosition after append - after Stream initialized");
				AssertAreEqual(data0, 1237, ToArray(ms), 0, 4193, "CopyTo(Stream,int,int) - existing data - after Stream initialized");
				AssertAreEqual(data0, 642, ToArray(ms), 4193, 942, "CopyTo(Stream,int,int) - new appended data - after Stream initialized");
			}
		}

		[Test]
		public void TestCopyToBytes()
		{
			var data0 = RandomBytes(7101, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = new byte[6731];
				bb.CopyTo(dst, 1007, 4361);
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(byte[],int,int) - StreamPosition");
				AssertAreEqual(data0, 1007, dst, 0, 4361, "CopyTo(byte[],int,int) - actual data");
				AssertAreEqual(new byte[6731 - 4361], 0, dst, 4361, 6731 - 4361, "CopyTo(byte[],int,int) - zero data");

				dst = new byte[6731];
				bb.InitializeStream(position : 42);
				bb.CopyTo(dst, 1007, 4361);
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(byte[],int,int) - StreamPosition after Stream initialized");
				AssertAreEqual(data0, 1007, dst, 0, 4361, "CopyTo(byte[],int,int) - actual data after Stream initialized");
				AssertAreEqual(new byte[6731 - 4361], 0, dst, 4361, 6731 - 4361, "CopyTo(byte[],int,int) - zero data after Stream initialized");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = new byte[6731];
				bb.CopyTo(dst, 1007, 723, 4361);
				Assert.AreEqual(0, bb.StreamPosition, "CopyTo(byte[],int,int,int) - StreamPosition");
				AssertAreEqual(data0, 1007, dst, 723, 4361, "CopyTo(byte[],int,int,int) - actual data");
				AssertAreEqual(new byte[723], 0, dst, 0, 723, "CopyTo(byte[],int,int,int) - zero data before actual");
				AssertAreEqual(new byte[6731 - (4361 + 723)], 0, dst, 723 + 4361, 6731 - (4361 + 723), "CopyTo(byte[],int,int,int) - zero data after actual");

				dst = new byte[6731];
				bb.InitializeStream(position : 42);
				bb.CopyTo(dst, 1007, 723, 4361);
				Assert.AreEqual(42, bb.StreamPosition, "CopyTo(byte[],int,int,int) - StreamPosition after Stream initialized");
				AssertAreEqual(data0, 1007, dst, 723, 4361, "CopyTo(byte[],int,int,int) - actual data after Stream initialized");
				AssertAreEqual(new byte[723], 0, dst, 0, 723, "CopyTo(byte[],int,int,int) - zero data before actual after Stream initialized");
				AssertAreEqual(new byte[6731 - (4361 + 723)], 0, dst, 723 + 4361, 6731 - (4361 + 723), "CopyTo(byte[],int,int,int) - zero data after actual after Stream initialized");
			}
		}

		[Test]
		public void TestGetChunk()
		{
			var data0 = RandomBytes(7101, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = bb.GetChunk(1007, 4361);
				Assert.AreEqual(4361, dst.Length, "GetChunk(int,int) - length");
				Assert.AreEqual(0, bb.StreamPosition, "GetChunk(int,int) - StreamPosition");
				AssertAreEqual(data0, 1007, dst, 0, 4361, "GetChunk(int,int) - actual data");

				bb.InitializeStream(position : 42);
				dst = bb.GetChunk(1007, 4361);
				Assert.AreEqual(4361, dst.Length, "GetChunk(int,int) - length - after Stream initialized");
				Assert.AreEqual(42, bb.StreamPosition, "GetChunk(int,int) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, 1007, dst, 0, 4361, "GetChunk(int,int) - actual data after Stream initialized");
			}
		}

		[Test]
		public void TestFromBytes()
		{
			var data0 = RandomBytes(7101, -0x02F0B4EF);
			var data1 = RandomBytes(9431, -0x7BFC1702);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.FromBytes(data1);

				Assert.AreEqual(9431, bb.Length, "FromBytes - length");
				Assert.AreEqual(0, bb.StreamPosition, "FromBytes - StreamPosition");
				AssertAreEqual(data1, bb.DumpTestData(), "FromBytes");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.FromBytes(data1);

				Assert.AreEqual(9431, bb.Length, "FromBytes - length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "FromBytes - StreamPosition - after Stream initialized");
				AssertAreEqual(data1, bb.DumpTestData(), "FromBytes - after Stream initialized");
			}
		}

		[Test]
		public void TestToBytes()
		{
			var data0 = RandomBytes(7101, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = bb.ToBytes();
				Assert.AreEqual(7101, dst.Length, "ToBytes() - length");
				Assert.AreEqual(0, bb.StreamPosition, "ToBytes() - StreamPosition");
				AssertAreEqual(data0, dst, "ToBytes()");

				bb.InitializeStream();
				dst = bb.ToBytes();
				Assert.AreEqual(7101, dst.Length, "ToBytes() - length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "ToBytes() - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, dst, "ToBytes() - after Stream initialized");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = bb.ToBytes(389, 5803);
				Assert.AreEqual(5803, dst.Length, "ToBytes(int,int) - length");
				Assert.AreEqual(0, bb.StreamPosition, "ToBytes(int,int) - StreamPosition");
				AssertAreEqual(data0, 389, dst, 0, 5803, "ToBytes(int,int)");

				bb.InitializeStream();
				dst = bb.ToBytes(389, 5803);
				Assert.AreEqual(5803, dst.Length, "ToBytes(int,int) - length - after Stream initialized");
				Assert.AreEqual(0, bb.StreamPosition, "ToBytes(int,int) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, 389, dst, 0, 5803, "ToBytes(int,int) - after Stream initialized");
			}
		}

		[Test]
		public void TestSwap1()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(1);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(1) - StreamPosition");
				AssertAreEqual(data0, bb.DumpTestData(), "Swap(1)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Swap(1);
				Assert.AreEqual(42, bb.StreamPosition, "Swap(1) - StreamPosition - after Stream initialized");
				AssertAreEqual(data0, bb.DumpTestData(), "Swap(1) - after Stream initialized");
			}
		}

		[Test]
		public void TestSwap2()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);
			var swapped0 = SwapBytes(data0, 2);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(2);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(2) - StreamPosition");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(2)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Swap(2);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(2) - StreamPosition - after Stream initialized");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(2) - after Stream initialized");
			}
		}

		[Test]
		public void TestSwap4()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);
			var swapped0 = SwapBytes(data0, 4);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(4);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(4) - StreamPosition");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(4)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Swap(4);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(4) - StreamPosition - after Stream initialized");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(4) - after Stream initialized");
			}
		}

		[Test]
		public void TestSwap8()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);
			var swapped0 = SwapBytes(data0, 8);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(8);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(8) - StreamPosition");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(8)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Swap(8);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(8) - StreamPosition - after Stream initialized");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(8) - after Stream initialized");
			}
		}

		[Test]
		public void TestSwapCustom()
		{
			var data0 = RandomBytes(8472, -0x02F0B4EF);
			var swapped0 = SwapBytes(data0, 3);

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(3);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(3) - StreamPosition");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(3)");
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream(position : 42);
				bb.Swap(3);
				Assert.AreEqual(0, bb.StreamPosition, "Swap(3) - StreamPosition - after Stream initialized");
				AssertAreEqual(swapped0, bb.DumpTestData(), "Swap(3) - after Stream initialized");
			}
		}

		[Test]
		public void TestGetString()
		{
			var scsDefault = new byte[]
			                 	{
			                 		0x4D, 0x4F, 0x4E, 0x4F, 0x43, 0x48, 0x52, 0x4F,
			                 		0x4D, 0x45, 0x32
			                 	};

			var scsH31 = new byte[]
			             	{
			             		0x59, 0x61, 0x6D, 0x61, 0x64, 0x61, 0x5E, 0x54,
			             		0x61, 0x72, 0x6F, 0x75, 0x3D, 0x1B, 0x24, 0x42,
			             		0x3B, 0x33, 0x45, 0x44, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x42, 0x40, 0x4F, 0x3A, 0x1B,
			             		0x28, 0x42, 0x3D, 0x1B, 0x24, 0x42, 0x24, 0x64,
			             		0x24, 0x5E, 0x24, 0x40, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x24, 0x3F, 0x24, 0x6D, 0x24,
			             		0x26, 0x1B, 0x28, 0x42
			             	};

			using (var bb = new TestByteBuffer(scsDefault))
			{
				Assert.AreEqual(@"MONOCHROME2", bb.GetString(), @"GetString - Default Character Set");
				Assert.AreEqual(0, bb.StreamPosition, @"GetString - StreamPosition - Default Character Set");
			}

			using (var bb = new TestByteBuffer(scsH31) {SpecificCharacterSet = @"\ISO 2022 IR 87"})
			{
				Assert.AreEqual(@"Yamada^Tarou=山田^太郎=やまだ^たろう", bb.GetString(), @"GetString - H31 Example (\ISO 2022 IR 87)");
				Assert.AreEqual(0, bb.StreamPosition, @"GetString - StreamPosition - H31 Example (\ISO 2022 IR 87)");
			}
		}

		[Test]
		public void TestSetString()
		{
			var defCharSet = new byte[]
			                 	{
			                 		0x4D, 0x4F, 0x4E, 0x4F, 0x43, 0x48, 0x52, 0x4F,
			                 		0x4D, 0x45, 0x32
			                 	};

			var scsH31 = new byte[]
			             	{
			             		0x59, 0x61, 0x6D, 0x61, 0x64, 0x61, 0x5E, 0x54,
			             		0x61, 0x72, 0x6F, 0x75, 0x3D, 0x1B, 0x24, 0x42,
			             		0x3B, 0x33, 0x45, 0x44, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x42, 0x40, 0x4F, 0x3A, 0x1B,
			             		0x28, 0x42, 0x3D, 0x1B, 0x24, 0x42, 0x24, 0x64,
			             		0x24, 0x5E, 0x24, 0x40, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x24, 0x3F, 0x24, 0x6D, 0x24,
			             		0x26, 0x1B, 0x28, 0x42
			             	};

			using (var bb = new TestByteBuffer())
			{
				bb.SetString(@"MONOCHROME2");
				Assert.AreEqual(0, bb.StreamPosition, @"SetString - StreamPosition - Default Character Set");
				AssertAreEqual(defCharSet, bb.DumpTestData(), @"SetString - Default Character Set");
			}

			using (var bb = new TestByteBuffer {SpecificCharacterSet = @"\ISO 2022 IR 87"})
			{
				bb.SetString(@"Yamada^Tarou=山田^太郎=やまだ^たろう");
				Assert.AreEqual(0, bb.StreamPosition, @"SetString - StreamPosition - H31 Example (\ISO 2022 IR 87)");
				AssertAreEqual(scsH31, bb.DumpTestData(), @"SetString - H31 Example (\ISO 2022 IR 87)");
			}
		}

		[Test]
		public void TestSetStringEvenLengthPadding()
		{
			var defCharSet = new byte[]
			                 	{
			                 		0x4D, 0x4F, 0x4E, 0x4F, 0x43, 0x48, 0x52, 0x4F,
			                 		0x4D, 0x45, 0x32
			                 	};

			var scsH31 = new byte[]
			             	{
			             		65,
			             		0x59, 0x61, 0x6D, 0x61, 0x64, 0x61, 0x5E, 0x54,
			             		0x61, 0x72, 0x6F, 0x75, 0x3D, 0x1B, 0x24, 0x42,
			             		0x3B, 0x33, 0x45, 0x44, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x42, 0x40, 0x4F, 0x3A, 0x1B,
			             		0x28, 0x42, 0x3D, 0x1B, 0x24, 0x42, 0x24, 0x64,
			             		0x24, 0x5E, 0x24, 0x40, 0x1B, 0x28, 0x42, 0x5E,
			             		0x1B, 0x24, 0x42, 0x24, 0x3F, 0x24, 0x6D, 0x24,
			             		0x26, 0x1B, 0x28, 0x42
			             	};

			using (var bb = new TestByteBuffer())
			{
				bb.SetString(@"MONOCHROME2", 0x20);
				Assert.AreEqual(0, bb.StreamPosition, @"SetString - StreamPosition - Default Character Set");
				AssertAreEqual(defCharSet, 0, bb.DumpTestData(), 0, defCharSet.Length, @"SetString - Default Character Set");
				AssertAreEqual(new byte[] {0x20}, 0, bb.DumpTestData(), defCharSet.Length, 1, @"SetString - Default Character Set - padding");
			}

			using (var bb = new TestByteBuffer {SpecificCharacterSet = @"\ISO 2022 IR 87"})
			{
				bb.SetString(@"AYamada^Tarou=山田^太郎=やまだ^たろう", 0x20);
				Assert.AreEqual(0, bb.StreamPosition, @"SetString - StreamPosition - H31 Example (\ISO 2022 IR 87)");
				AssertAreEqual(scsH31, 0, bb.DumpTestData(), 0, scsH31.Length, @"SetString - H31 Example (\ISO 2022 IR 87)");
				AssertAreEqual(new byte[] {0x20}, 0, bb.DumpTestData(), scsH31.Length, 1, @"SetString - H31 Example (\ISO 2022 IR 87) - padding");
			}
		}

		protected static byte[] ToArray(Stream stream)
		{
			if (stream is MemoryStream)
				return ((MemoryStream) stream).ToArray();
			else if (stream is LargeMemoryStream)
				return ((LargeMemoryStream) stream).ToArray();

			var pos = stream.Position;
			try
			{
				var buffer = new byte[stream.Length];
				stream.Position = 0;
				stream.Read(buffer, 0, buffer.Length);
				return buffer;
			}
			finally
			{
				stream.Position = pos;
			}
		}

		protected static byte[] RandomBytes(int size, int seed)
		{
			var buffer = new byte[size];
			new PseudoRandom(seed).NextBytes(buffer);
			return buffer;
		}

		protected static byte[] SwapBytes(byte[] data, int windowSize)
		{
			const string message = "windowSize must be positive.";
			if (windowSize < 1) throw new ArgumentOutOfRangeException("windowSize", windowSize, message);
			var buffer = new byte[(data.Length/windowSize + (data.Length%windowSize != 0 ? 1 : 0))*windowSize];
			Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
			for (var n = 0; n < buffer.Length; n += windowSize)
				Array.Reverse(buffer, n, windowSize);
			return buffer;
		}

		protected static void AssertAreEqual<TElement>(IList<TElement> expectedValues, IList<TElement> actualValues, string message, params object[] args)
		{
			Assert.AreEqual(expectedValues.Count, actualValues.Count, "Length does not match. {0}", string.Format(message, args));
			for (var n = 0; n < expectedValues.Count; ++n)
				Assert.AreEqual(expectedValues[n], actualValues[n], "Element @ index {1} does not match. {0}", string.Format(message, args), n);
		}

		protected static void AssertAreEqual<TElement>(IList<TElement> expectedValues, int expectedValuesOffset, IList<TElement> actualValues, int actualValuesOffset, int compareCount, string message, params object[] args)
		{
			if (!(compareCount <= expectedValues.Count - expectedValuesOffset)) throw new ArgumentException(string.Format("Expected array does not contain {1} elements starting at offset {2}. {0}", string.Format(message, args), compareCount, expectedValuesOffset));
			Assert.IsTrue(compareCount <= actualValues.Count - actualValuesOffset, "Actual array does not contain {1} elements starting at offset {2}. {0}", string.Format(message, args), compareCount, actualValuesOffset);
			for (var n = 0; n < compareCount; ++n)
				Assert.AreEqual(expectedValues[expectedValuesOffset + n], actualValues[actualValuesOffset + n], "Element @ actual index {1} does not match @ expected index {2}. {0}", string.Format(message, args), actualValuesOffset + n, expectedValuesOffset + n);
		}

		protected static void AssertThrowsException<TState>(TState state, Action<TState> action, Action<TState, Exception> assertOnException = null)
		{
			try
			{
				action.Invoke(state);
				Assert.Fail("Expected an exception");
			}
			catch (Exception ex)
			{
				const string format = "{0}: {1}";
				Console.WriteLine(format, ex.GetType().FullName, ex.Message);

				if (assertOnException != null)
					assertOnException.Invoke(state, ex);
			}
		}
	}
}

#endif