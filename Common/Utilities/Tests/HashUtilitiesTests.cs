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

// ReSharper disable LocalizableElement

using System;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	internal class HashUtilitiesTests
	{
		[Test]
		public void TestHashGuid()
		{
			var result0 = Guid.Empty;

			var result1 = HashUtilities.ComputeHashGuid(new byte[0]);
			Console.WriteLine("Hashed to: {0}", result1);
			Assert.AreNotEqual(result0, result1);

			var result1B = HashUtilities.ComputeHashGuid(new byte[0]);
			Console.WriteLine("Hashed to: {0}", result1B);
			Assert.AreEqual(result1, result1B);

			var result2 = HashUtilities.ComputeHashGuid(Encoding.ASCII.GetBytes("ClearCanvas.Common.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", result2);
			Assert.AreNotEqual(result0, result2);
			Assert.AreNotEqual(result1, result2);

			var result3 = HashUtilities.ComputeHashGuid(Encoding.ASCII.GetBytes("ClearCanvas.Commom.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", result3);
			Assert.AreNotEqual(result0, result3);
			Assert.AreNotEqual(result1, result3);
			Assert.AreNotEqual(result2, result3);
		}

		[Test]
		public void TestHash128()
		{
			const int minBitDelta = 32; // on average, the bit delta will be 64
			var result0 = new byte[128/8];

			var result1 = HashUtilities.ComputeHash128(new byte[0]);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result1));
			Assert.GreaterOrEqual(GetBitDelta(result0, result1), minBitDelta);

			var result1B = HashUtilities.ComputeHash128(new byte[0]);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result1B));
			Assert.AreEqual(0, GetBitDelta(result1, result1B));

			var result2 = HashUtilities.ComputeHash128(Encoding.ASCII.GetBytes("ClearCanvas.Common.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result2));
			Assert.GreaterOrEqual(GetBitDelta(result0, result2), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result2), minBitDelta);

			var result3 = HashUtilities.ComputeHash128(Encoding.ASCII.GetBytes("ClearCanvas.Commom.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result3));
			Assert.GreaterOrEqual(GetBitDelta(result0, result3), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result3), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result3), minBitDelta);

			var result4 = HashUtilities.ComputeHash128(Encoding.ASCII.GetBytes("ClearCanvas.Common.Utilities.Tests"), 18, 10);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result4));
			Assert.GreaterOrEqual(GetBitDelta(result0, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result3, result4), minBitDelta);

			var result5 = HashUtilities.ComputeHash128(Encoding.ASCII.GetBytes("ClearCanvas.Commom.Utilities.Tests"), 18, 10);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result5));
			Assert.GreaterOrEqual(GetBitDelta(result0, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result3, result5), minBitDelta);
			Assert.AreEqual(0, GetBitDelta(result4, result5), minBitDelta);
		}

		[Test]
		public void TestHash256()
		{
			const int minBitDelta = 64; // on average, the bit delta will be 128
			var result0 = new byte[256/8];

			var result1 = HashUtilities.ComputeHash256(new byte[0]);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result1));
			Assert.GreaterOrEqual(GetBitDelta(result0, result1), minBitDelta);

			var result1B = HashUtilities.ComputeHash256(new byte[0]);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result1B));
			Assert.AreEqual(GetBitDelta(result1, result1B), 0);

			var result2 = HashUtilities.ComputeHash256(Encoding.ASCII.GetBytes("ClearCanvas.Common.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result2));
			Assert.GreaterOrEqual(GetBitDelta(result0, result2), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result2), minBitDelta);

			var result3 = HashUtilities.ComputeHash256(Encoding.ASCII.GetBytes("ClearCanvas.Commom.Utilities.Tests"));
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result3));
			Assert.GreaterOrEqual(GetBitDelta(result0, result3), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result3), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result3), minBitDelta);

			var result4 = HashUtilities.ComputeHash256(Encoding.ASCII.GetBytes("ClearCanvas.Common.Utilities.Tests"), 18, 10);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result4));
			Assert.GreaterOrEqual(GetBitDelta(result0, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result4), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result3, result4), minBitDelta);

			var result5 = HashUtilities.ComputeHash256(Encoding.ASCII.GetBytes("ClearCanvas.Commom.Utilities.Tests"), 18, 10);
			Console.WriteLine("Hashed to: {0}", StringUtilities.ToHexString(result5));
			Assert.GreaterOrEqual(GetBitDelta(result0, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result1, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result2, result5), minBitDelta);
			Assert.GreaterOrEqual(GetBitDelta(result3, result5), minBitDelta);
			Assert.AreEqual(0, GetBitDelta(result4, result5), minBitDelta);
		}

		private static int GetBitDelta(byte[] a, byte[] b)
		{
			if (a.Length != b.Length) throw new ArgumentException("arrays must have same length!");

			// basically just counts all the bits that are different
			int d = 0;
			for (var n = 0; n < a.Length; ++n)
			{
				var v = (byte) (a[n] ^ b[n]);
				for (; v != 0; v >>= 1)
					d += v & 1;
			}
			return d;
		}
	}
}

// ReSharper restore LocalizableElement

#endif