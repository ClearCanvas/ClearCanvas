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
using System.IO;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.IO.Tests
{
	//[TestFixture] // uncomment this attribute for ease of debugging, but otherwise leave it off since it doesn't actually unit test anything
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	internal class ByteBufferPerformanceTest : IApplicationRoot
	{
		public void RunApplication(string[] args)
		{
			TestByteBuffer.NoReport = true;
			TestByteBuffer.UseLegacyImplementation = (args != null && args.Any(s => s == "--legacy"));
			try
			{
				ExecuteTestFixture(this);
			}
			finally
			{
				TestByteBuffer.UseLegacyImplementation = false;
				TestByteBuffer.NoReport = false;
			}
		}

		private static void ExecuteTestFixture(object testFixture)
		{
			Platform.CheckForNullReference(testFixture, "testFixture");

			var testType = testFixture.GetType();
			var testMethods = from m in testType.GetMethods()
			                  let attributes = m.GetCustomAttributes(typeof (TestAttribute), true)
			                  where attributes != null && attributes.Length > 0
			                  select m;
			foreach (var testMethod in testMethods)
			{
				try
				{
					testMethod.Invoke(testFixture, null);
				}
				catch (Exception ex)
				{
					Console.WriteLine(testType.Name + '.' + testMethod.Name);
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			}
		}

		protected static byte[] RandomBytes(int size, int seed)
		{
			var buffer = new byte[size];
			new PseudoRandom(seed).NextBytes(buffer);
			return buffer;
		}

		[Test]
		public void TestChop()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Chop(1000000);
				stats += bb.LastStatistics;

				bb.Chop(1000000 - 1);
				stats += bb.LastStatistics;

				bb.Chop(1000000 + 1);
				stats += bb.LastStatistics;

				bb.Chop(1000000 - 2);
				stats += bb.LastStatistics;

				bb.Chop(1000000 + 2);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Chop(1000000);
				stats += bb.LastStatistics;

				bb.Chop(1000000 - 1);
				stats += bb.LastStatistics;

				bb.Chop(1000000 + 1);
				stats += bb.LastStatistics;

				bb.Chop(1000000 - 2);
				stats += bb.LastStatistics;

				bb.Chop(1000000 + 2);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestAppend()
		{
			var data0 = RandomBytes(10000000, 0);
			var data1 = RandomBytes(10000000, 1);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Append(data1, 0, 750000);
				stats += bb.LastStatistics;

				bb.Append(data1, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.Append(data1, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.Append(data1, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.Append(data1, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Append(data1, 0, 750000);
				stats += bb.LastStatistics;

				bb.Append(data1, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.Append(data1, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.Append(data1, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.Append(data1, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyFrom()
		{
			var data0 = RandomBytes(10000000, 0);
			var data1 = RandomBytes(10000000, 1);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream(TestByteBuffer.CopyBytes(data1)))
			{
				bb.CopyFrom(ms, 750000);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 500000);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 1001101);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream(TestByteBuffer.CopyBytes(data1)))
			{
				bb.InitializeStream();
				bb.CopyFrom(ms, 750000);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 500000);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyFrom(ms, 1001101);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyToBinaryWriter()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			using (var bw = new BinaryWriter(Stream.Null))
			{
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			using (var bw = new BinaryWriter(Stream.Null))
			{
				bb.InitializeStream();
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyToBinaryWriterLargeFileStream()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new TempFileStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new TempFileStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.InitializeStream();
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyToBinaryWriterLargeMemoryStream()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bb.InitializeStream();
				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;

				bb.CopyTo(bw);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyToStream()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				var ms = Stream.Null;

				bb.CopyTo(ms, 0, 750000);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				var ms = Stream.Null;

				bb.InitializeStream();
				bb.CopyTo(ms, 0, 750000);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyTo(ms, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestCopyToBytes()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = new byte[10000000];

				bb.CopyTo(dst, 0, 750000);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				var dst = new byte[10000000];

				bb.InitializeStream();
				bb.CopyTo(dst, 0, 750000);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.CopyTo(dst, 3000000, 1001101);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestGetChunk()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.GetChunk(0, 750000);
				stats += bb.LastStatistics;

				bb.GetChunk(750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.GetChunk(1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.GetChunk(2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.GetChunk(3000000, 1001101);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.GetChunk(0, 750000);
				stats += bb.LastStatistics;

				bb.GetChunk(750000, 750000 + 1);
				stats += bb.LastStatistics;

				bb.GetChunk(1500000 + 1, 500000);
				stats += bb.LastStatistics;

				bb.GetChunk(2000000 + 1, 1000000 - 1);
				stats += bb.LastStatistics;

				bb.GetChunk(3000000, 1001101);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestFromBytes()
		{
			var data0 = RandomBytes(10000000, 0);
			var data1 = RandomBytes(10000000, 1);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;

				bb.FromBytes(data1);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestToBytes()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;

				bb.ToBytes();
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestSwap2()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;

				bb.Swap(2);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestSwap4()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;

				bb.Swap(4);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestSwap8()
		{
			var data0 = RandomBytes(10000000, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;

				bb.Swap(8);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}

		[Test]
		public void TestSwapCustom()
		{
			var data0 = RandomBytes(10000004, 0);
			var stats = TestByteBufferStatistics.Empty;

			using (var bb = new TestByteBuffer(data0))
			{
				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;
			}

			using (var bb = new TestByteBuffer(data0))
			{
				bb.InitializeStream();
				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;

				bb.Swap(7);
				stats += bb.LastStatistics;
			}

			Console.WriteLine(stats);
		}
	}
}

#endif