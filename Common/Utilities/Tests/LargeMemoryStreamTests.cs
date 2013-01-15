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

using System.IO;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class LargeMemoryStreamTests : StreamTestBase<LargeMemoryStream>
	{
		protected override LargeMemoryStream CreateStream(byte[] seedData)
		{
			var stream = new LargeMemoryStream();
			if (seedData != null)
			{
				stream.Write(seedData, 0, seedData.Length);
				stream.Position = 0;
			}
			return stream;
		}

		[Test]
		public void TestInitStream()
		{
			var seedData = new byte[(1 << 17)];
			new PseudoRandom(323679581).NextBytes(seedData);

			using (var s = new LargeMemoryStream(seedData))
			{
				AssertAreEqual(seedData, s.ToArray(), "ToArray()");
			}
		}

		[Test]
		public void TestDumpStream()
		{
			var seedData = new byte[(1 << 17)];
			new PseudoRandom(323679581).NextBytes(seedData);

			using (var s = CreateStream(seedData))
			using (var r = new MemoryStream())
			{
				s.Position = 1037; // set position to an arbitrary offset (ToArray and WriteTo should not be affected by position)

				Assert.AreEqual(seedData, s.ToArray(), "ToArray()");

				Assert.AreEqual(1037, s.Position, "Position");

				s.WriteTo(r);
				Assert.AreEqual(seedData, r.ToArray(), "WriteTo(Stream)");

				Assert.AreEqual(1037, s.Position, "Position");
			}
		}

		[Test]
		public void TestMaxLength()
		{
			// the block sizing implementation of LargeMemoryStream allows for the following:
			// * blocks 1 through 4 combine for a total capacity of 65536 bytes
			// * blocks 5+ each have a capacity of 65536
			// * limited to a total of 2^31-1 (int.MaxValue) blocks
			// thus the capacity = 65536 + 65536*((2^31-1)-4)
			Assert.AreEqual(((1L << 31) - 1 - 4)*65536L + 65536, LargeMemoryStream.MaxLength, "LargeMemoryStream.MaxLength");
		}

		[Test]
		public void TestBlockSizes()
		{
			Assert.AreEqual(1024, LargeMemoryStream.TestGetBlockSize(0), "LargeMemoryStream.GetBlockSize(0)");
			Assert.AreEqual(4096 - 1024, LargeMemoryStream.TestGetBlockSize(1), "LargeMemoryStream.GetBlockSize(1)");
			Assert.AreEqual(16384 - 4096, LargeMemoryStream.TestGetBlockSize(2), "LargeMemoryStream.GetBlockSize(2)");
			Assert.AreEqual(65536 - 16384, LargeMemoryStream.TestGetBlockSize(3), "LargeMemoryStream.GetBlockSize(3)");
			for (var n = 4; n < 10; ++n)
				Assert.AreEqual(65536, LargeMemoryStream.TestGetBlockSize(n), "LargeMemoryStream.GetBlockSize({0})", n);
		}

		[Test]
		public void TestBlockOffsets()
		{
			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(0), "LargeMemoryStream.GetBlockOffset({0})", 0);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(1), "LargeMemoryStream.GetBlockOffset({0})", 1);
			Assert.AreEqual(1023, LargeMemoryStream.TestGetBlockOffset(1023), "LargeMemoryStream.GetBlockOffset({0})", 1023);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(1024), "LargeMemoryStream.GetBlockOffset({0})", 1024);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(1025), "LargeMemoryStream.GetBlockOffset({0})", 1025);
			Assert.AreEqual(4095 - 1024, LargeMemoryStream.TestGetBlockOffset(4095), "LargeMemoryStream.GetBlockOffset({0})", 4095);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(4096), "LargeMemoryStream.GetBlockOffset({0})", 4096);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(4097), "LargeMemoryStream.GetBlockOffset({0})", 4097);
			Assert.AreEqual(16383 - 4096, LargeMemoryStream.TestGetBlockOffset(16383), "LargeMemoryStream.GetBlockOffset({0})", 16383);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(16384), "LargeMemoryStream.GetBlockOffset({0})", 16384);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(16385), "LargeMemoryStream.GetBlockOffset({0})", 16385);
			Assert.AreEqual(65535 - 16384, LargeMemoryStream.TestGetBlockOffset(65535), "LargeMemoryStream.GetBlockOffset({0})", 65535);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(65536), "LargeMemoryStream.GetBlockOffset({0})", 65536);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(65537), "LargeMemoryStream.GetBlockOffset({0})", 65537);
			Assert.AreEqual(65535, LargeMemoryStream.TestGetBlockOffset(2*65536 - 1), "LargeMemoryStream.GetBlockOffset({0})", 2*65536 - 1);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(2*65536), "LargeMemoryStream.GetBlockOffset({0})", 2*65536);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockOffset(2*65536 + 1), "LargeMemoryStream.GetBlockOffset({0})", 2*65536 + 1);
			Assert.AreEqual(65535, LargeMemoryStream.TestGetBlockOffset(3*65536 - 1), "LargeMemoryStream.GetBlockOffset({0})", 3*65536 - 1);

			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockOffset(3*65536), "LargeMemoryStream.GetBlockOffset({0})", 3*65536);
		}

		[Test]
		public void TestBlockIndices()
		{
			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockIndex(0), "LargeMemoryStream.GetBlockIndex({0})", 0);
			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockIndex(1), "LargeMemoryStream.GetBlockIndex({0})", 1);
			Assert.AreEqual(0, LargeMemoryStream.TestGetBlockIndex(1023), "LargeMemoryStream.GetBlockIndex({0})", 1023);

			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockIndex(1024), "LargeMemoryStream.GetBlockIndex({0})", 1024);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockIndex(1025), "LargeMemoryStream.GetBlockIndex({0})", 1025);
			Assert.AreEqual(1, LargeMemoryStream.TestGetBlockIndex(4095), "LargeMemoryStream.GetBlockIndex({0})", 4095);

			Assert.AreEqual(2, LargeMemoryStream.TestGetBlockIndex(4096), "LargeMemoryStream.GetBlockIndex({0})", 4096);
			Assert.AreEqual(2, LargeMemoryStream.TestGetBlockIndex(4097), "LargeMemoryStream.GetBlockIndex({0})", 4097);
			Assert.AreEqual(2, LargeMemoryStream.TestGetBlockIndex(16383), "LargeMemoryStream.GetBlockIndex({0})", 16383);

			Assert.AreEqual(3, LargeMemoryStream.TestGetBlockIndex(16384), "LargeMemoryStream.GetBlockIndex({0})", 16384);
			Assert.AreEqual(3, LargeMemoryStream.TestGetBlockIndex(16385), "LargeMemoryStream.GetBlockIndex({0})", 16385);
			Assert.AreEqual(3, LargeMemoryStream.TestGetBlockIndex(65535), "LargeMemoryStream.GetBlockIndex({0})", 65535);

			Assert.AreEqual(4, LargeMemoryStream.TestGetBlockIndex(65536), "LargeMemoryStream.GetBlockIndex({0})", 65536);
			Assert.AreEqual(4, LargeMemoryStream.TestGetBlockIndex(65537), "LargeMemoryStream.GetBlockIndex({0})", 65537);
			Assert.AreEqual(4, LargeMemoryStream.TestGetBlockIndex(2*65536 - 1), "LargeMemoryStream.GetBlockIndex({0})", 2*65536 - 1);

			Assert.AreEqual(5, LargeMemoryStream.TestGetBlockIndex(2*65536), "LargeMemoryStream.GetBlockIndex({0})", 2*65536);
			Assert.AreEqual(5, LargeMemoryStream.TestGetBlockIndex(2*65536 + 1), "LargeMemoryStream.GetBlockIndex({0})", 2*65536 + 1);
			Assert.AreEqual(5, LargeMemoryStream.TestGetBlockIndex(3*65536 - 1), "LargeMemoryStream.GetBlockIndex({0})", 3*65536 - 1);

			Assert.AreEqual(6, LargeMemoryStream.TestGetBlockIndex(3*65536), "LargeMemoryStream.GetBlockIndex({0})", 3*65536);
		}
	}
}

#endif