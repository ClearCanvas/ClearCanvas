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
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	/// <summary>
	/// A base unit test for validating basic functionality and semantics of a <see cref="Stream"/> implementation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StreamTestBase<T>
		where T : Stream
	{
		protected abstract T CreateStream(byte[] seedData);

		protected virtual T CreateStream()
		{
			return CreateStream(null);
		}

		[Test]
		public virtual void TestSeek()
		{
			using (var s = CreateStream())
			{
				if (!s.CanSeek)
				{
					Console.WriteLine("Test skipped because {0} doesn't support Seek", s.GetType().FullName);
					return;
				}

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Seek(s, 0, SeekOrigin.Begin);
				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Seek(s, 5, SeekOrigin.Begin);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Seek(s, 3, SeekOrigin.Current);
				Assert.AreEqual(8, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Seek(s, -4, SeekOrigin.Current);
				Assert.AreEqual(4, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Seek(s, 3, SeekOrigin.End);
				Assert.AreEqual(3, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				AssertThrowsException(s, x => Seek(x, -3, SeekOrigin.End));
				Assert.AreEqual(3, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				AssertThrowsException(s, x => Seek(x, -9, SeekOrigin.Current));
				Assert.AreEqual(3, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				AssertThrowsException(s, x => Seek(x, -1, SeekOrigin.Begin));
				Assert.AreEqual(3, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");
			}
		}

		[Test]
		public virtual void TestRead()
		{
			var seedData = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF};
			using (var s = CreateStream(seedData))
			{
				if (!s.CanRead)
				{
					Console.WriteLine("Test skipped because {0} doesn't support Read", s.GetType().FullName);
					return;
				}

				byte[] buffer;
				int readByteResult;

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(5, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0x0, 0x1, 0x2, 0x3, 0x4}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(3, Read(s, buffer, 0, 3), "Bytes Read");
				Assert.AreEqual(8, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0x5, 0x6, 0x7, 0xFF, 0xFF}, buffer, "Buffer");

				readByteResult = ReadByte(s);
				Assert.AreEqual(9, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(0x8, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(2, Read(s, buffer, 2, 2), "Bytes Read");
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0x9, 0xA, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(0, Read(s, buffer, 2, 0), "Bytes Read");
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				AssertThrowsException(s, x => Read(x, null, 0, 0));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

// ReSharper disable AccessToModifiedClosure

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				AssertThrowsException(s, x => Read(x, buffer, -9, 0));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				AssertThrowsException(s, x => Read(x, buffer, 0, -3));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				AssertThrowsException(s, x => Read(x, buffer, 3, 6));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				AssertThrowsException(s, x => Read(x, buffer, 5, 1));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

// ReSharper restore AccessToModifiedClosure

				readByteResult = ReadByte(s);
				Assert.AreEqual(12, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(0xB, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(4, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xC, 0xD, 0xE, 0xF, 0xFF}, buffer, "Buffer");

				readByteResult = ReadByte(s);
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(-1, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(0, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");
			}
		}

		[Test]
		public virtual void TestWrite()
		{
			using (var s = CreateStream())
			{
				if (!s.CanWrite)
				{
					Console.WriteLine("Test skipped because {0} doesn't support Write", s.GetType().FullName);
					return;
				}

				var referenceBuffer = new byte[] {0xF1, 0xF2, 0xF3, 0xF4, 0xF5};
				var buffer = new byte[] {0xF1, 0xF2, 0xF3, 0xF4, 0xF5};

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Write(s, buffer, 0, 5);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(5, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				Write(s, buffer, 0, 3);
				Assert.AreEqual(8, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				WriteByte(s, 0x7F);
				Assert.AreEqual(9, s.Position, "Position");
				Assert.AreEqual(9, s.Length, "Length");

				Write(s, buffer, 2, 2);
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				Write(s, buffer, 2, 0);
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				AssertThrowsException(s, x => Write(x, null, 0, 0));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				AssertThrowsException(s, x => Write(x, buffer, -9, 0));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				AssertThrowsException(s, x => Write(x, buffer, 0, -3));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				AssertThrowsException(s, x => Write(x, buffer, 3, 6));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				AssertThrowsException(s, x => Write(x, buffer, 5, 1));
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");

				WriteByte(s, 0x7F);
				Assert.AreEqual(12, s.Position, "Position");
				Assert.AreEqual(12, s.Length, "Length");

				Write(s, buffer, 3, 1);
				Assert.AreEqual(13, s.Position, "Position");
				Assert.AreEqual(13, s.Length, "Length");
				AssertAreEqual(referenceBuffer, buffer, "Buffer");
			}
		}

		[Test]
		public virtual void TestReadAndSeek()
		{
			var seedData = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF};
			using (var s = CreateStream(seedData))
			{
				if (!(s.CanRead && s.CanSeek))
				{
					Console.WriteLine("Test skipped because {0} doesn't support Read and Seek", s.GetType().FullName);
					return;
				}

				byte[] buffer;
				int readByteResult;

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Seek(s, 5, SeekOrigin.Begin);
				Assert.AreEqual(5, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(10, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0x5, 0x6, 0x7, 0x8, 0x9}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(3, Read(s, buffer, 0, 3), "Bytes Read");
				Assert.AreEqual(13, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xA, 0xB, 0xC, 0xFF, 0xFF}, buffer, "Buffer");

				readByteResult = ReadByte(s);
				Assert.AreEqual(14, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(0xD, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(2, Read(s, buffer, 2, 2), "Bytes Read");
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xE, 0xF, 0xFF}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Seek(s, 3, SeekOrigin.Current);
				Assert.AreEqual(0, Read(s, buffer, 2, 0), "Bytes Read");
				Assert.AreEqual(19, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");

				Seek(s, -12, SeekOrigin.End);
				readByteResult = ReadByte(s);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(0x4, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Seek(s, -4, SeekOrigin.End);
				Assert.AreEqual(4, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xC, 0xD, 0xE, 0xF, 0xFF}, buffer, "Buffer");

				Seek(s, 2, SeekOrigin.End);
				readByteResult = ReadByte(s);
				Assert.AreEqual(18, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(-1, readByteResult, "Read Byte Result");
			}
		}

		[Test]
		public virtual void TestWriteAndSeek()
		{
			using (var s = CreateStream())
			{
				if (!(s.CanWrite && s.CanSeek))
				{
					Console.WriteLine("Test skipped because {0} doesn't support Write and Seek", s.GetType().FullName);
					return;
				}

				var buffer = new byte[5];

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(0, s.Length, "Length");

				Write(s, buffer, 0, 5);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(5, s.Length, "Length");

				Seek(s, 2, SeekOrigin.Begin);
				Write(s, buffer, 0, 3);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(5, s.Length, "Length");

				WriteByte(s, 0x7F);
				Assert.AreEqual(6, s.Position, "Position");
				Assert.AreEqual(6, s.Length, "Length");

				Seek(s, 9, SeekOrigin.Begin);
				Write(s, buffer, 2, 2);
				Assert.AreEqual(11, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");

				Seek(s, 2, SeekOrigin.Begin);
				Write(s, buffer, 2, 0);
				Assert.AreEqual(2, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");

				WriteByte(s, 0x7F);
				Assert.AreEqual(3, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");

				Write(s, buffer, 3, 1);
				Assert.AreEqual(4, s.Position, "Position");
				Assert.AreEqual(11, s.Length, "Length");
			}
		}

		[Test]
		public virtual void TestReadWriteAndSeek()
		{
			var seedData = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF};
			using (var s = CreateStream(seedData))
			{
				if (!(s.CanRead && s.CanWrite && s.CanSeek))
				{
					Console.WriteLine("Test skipped because {0} doesn't support Read, Write and Seek", s.GetType().FullName);
					return;
				}

				byte[] buffer;
				int readByteResult;

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				Write(s, new byte[] {0x20, 0x21, 0x22, 0x23, 0x24}, 0, 5);
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				Seek(s, 0, SeekOrigin.Begin);
				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(5, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(5, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0x20, 0x21, 0x22, 0x23, 0x24}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(5, Read(s, buffer, 0, 5), "Bytes Read");
				Assert.AreEqual(10, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0x5, 0x6, 0x7, 0x8, 0x9}, buffer, "Buffer");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(3, Read(s, buffer, 0, 3), "Bytes Read");
				Assert.AreEqual(13, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xA, 0xB, 0xC, 0xFF, 0xFF}, buffer, "Buffer");

				Write(s, buffer, 2, 2);
				Assert.AreEqual(15, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				WriteByte(s, 0xFE);
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				Seek(s, -3, SeekOrigin.End);
				Assert.AreEqual(13, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				readByteResult = ReadByte(s);
				Assert.AreEqual(14, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				Assert.AreEqual(0xC, readByteResult, "Read Byte Result");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(2, Read(s, buffer, 1, 4), "Bytes Read");
				Assert.AreEqual(16, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");
				AssertAreEqual(new byte[] {0xFF, 0xFF, 0xFE, 0xFF, 0xFF}, buffer, "Buffer");

				Seek(s, 2, SeekOrigin.End);
				Assert.AreEqual(18, s.Position, "Position");
				Assert.AreEqual(16, s.Length, "Length");

				Write(s, new byte[] {0x1, 0x2, 0x3, 0x4, 0x5}, 0, 3);
				Assert.AreEqual(21, s.Position, "Position");
				Assert.AreEqual(21, s.Length, "Length");

				Seek(s, -8, SeekOrigin.Current);
				Assert.AreEqual(13, s.Position, "Position");
				Assert.AreEqual(21, s.Length, "Length");

				buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
				Assert.AreEqual(8, Read(s, buffer, 0, 10), "Bytes Read");
				Assert.AreEqual(21, s.Position, "Position");
				Assert.AreEqual(21, s.Length, "Length");
				AssertAreEqual(new byte[] {0xC, 0xFF, 0xFE}, buffer, 0, "Buffer");
				AssertAreEqual(new byte[] {0x1, 0x2, 0x3}, buffer, 5, "Buffer");
			}
		}

		[Test]
		public virtual void TestSetLength()
		{
			var seedData = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7};
			using (var s = CreateStream(seedData))
			{
				if (!(s.CanWrite && s.CanSeek))
				{
					Console.WriteLine("Test skipped because {0} doesn't support Write and Seek", s.GetType().FullName);
					return;
				}

				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");

				Seek(s, 0, SeekOrigin.Begin);
				Assert.AreEqual(0, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");

				Seek(s, 2, SeekOrigin.Begin);
				Assert.AreEqual(2, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");

				SetLength(s, 5);
				// expected behaviour of Position after a SetLength call is not explicitly defined by Stream
				Assert.AreEqual(5, s.Length, "Length");

				if (s.CanRead)
				{
					var buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
					Seek(s, 0, SeekOrigin.Begin);
					Read(s, buffer, 0, 8);
					AssertAreEqual(new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");
				}

				Seek(s, 3, SeekOrigin.End);
				Assert.AreEqual(8, s.Position, "Position");
				Assert.AreEqual(5, s.Length, "Length");

				SetLength(s, 5);
				// expected behaviour of Position after a SetLength call is not explicitly defined by Stream
				Assert.AreEqual(5, s.Length, "Length");

				if (s.CanRead)
				{
					var buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
					Seek(s, 0, SeekOrigin.Begin);
					Read(s, buffer, 0, 8);
					AssertAreEqual(new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0xFF, 0xFF, 0xFF}, buffer, "Buffer");
				}

				SetLength(s, 8);
				// expected behaviour of Position after a SetLength call is not explicitly defined by Stream
				Assert.AreEqual(8, s.Length, "Length");

				if (s.CanRead)
				{
					var buffer = new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
					Seek(s, 0, SeekOrigin.Begin);
					Read(s, buffer, 0, 8);
					// contents in the expanded area after a SetLength call are not defined
					AssertAreEqual(new byte[] {0x0, 0x1, 0x2, 0x3, 0x4}, buffer, 0, "Buffer");
				}

				Seek(s, -4, SeekOrigin.End);
				Assert.AreEqual(4, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");

				AssertThrowsException(s, x => SetLength(x, -3));
				Assert.AreEqual(4, s.Position, "Position");
				Assert.AreEqual(8, s.Length, "Length");

				SetLength(s, 0);
				// expected behaviour of Position after a SetLength call is not explicitly defined by Stream
				Assert.AreEqual(0, s.Length, "Length");
			}
		}

		[Test]
		public void TestRandomAccess()
		{
			const int operationCount = 2500;

			var rng = new PseudoRandom(-0x522C5EF5);
			var seedData = new byte[1 << 17];
			using (var s = CreateStream(seedData))
			using (var r = new MemoryStream())
			{
				if (!(s.CanRead && s.CanWrite && s.CanSeek))
				{
					Console.WriteLine("Test skipped because {0} doesn't support Read, Write and Seek", s.GetType().FullName);
					return;
				}

				r.Write(seedData, 0, seedData.Length);
				r.Position = 0;

				Console.WriteLine("Preparing to execute {0} randomized operations", operationCount);
				for (var k = 0; k < operationCount; ++k)
				{
					var opcode = rng.Next(0, 12); // slightly biased towards write operations, in order to ensure "interesting" data
					switch (opcode)
					{
						case 0: // Read
						case 6:
							{
								var size = rng.Next(1024, 32768);
								var offset = rng.Next(0, 1024);
								var count = rng.Next(0, size - offset);

								var sBuffer = new byte[size];
								var rBuffer = new byte[size];
								rng.NextBytes(sBuffer);
								Buffer.BlockCopy(sBuffer, 0, rBuffer, 0, size);

								var sResult = Read(s, sBuffer, offset, count);
								var rResult = r.Read(rBuffer, offset, count);

								Assert.AreEqual(rResult, sResult, "Function return from Read at step k={0}", k);
								Assert.AreEqual(r.Position, s.Position, "Position after Read at step k={0}", k);
								Assert.AreEqual(r.Length, s.Length, "Length after Read at step k={0}", k);
								AssertAreEqual(rBuffer, sBuffer, "Buffer after Read at step k={0}", k);
							}
							break;
						case 1: // ReadByte
							{
								var sResult = ReadByte(s);
								var rResult = r.ReadByte();

								Assert.AreEqual(rResult, sResult, "Function return from ReadByte at step k={0}", k);
								Assert.AreEqual(r.Position, s.Position, "Position after ReadByte at step k={0}", k);
								Assert.AreEqual(r.Length, s.Length, "Length after ReadByte at step k={0}", k);
							}
							break;
						case 2: // Write
						case 7:
						case 8:
						case 9:
							{
								var oldLength = s.Length;
								var size = rng.Next(1024, 32768);
								var offset = rng.Next(0, 1024);
								var count = rng.Next(0, size - offset);

								var sBuffer = new byte[size];
								var rBuffer = new byte[size];
								rng.NextBytes(sBuffer);
								Buffer.BlockCopy(sBuffer, 0, rBuffer, 0, size);

								Write(s, sBuffer, offset, count);
								r.Write(rBuffer, offset, count);

								Assert.AreEqual(r.Position, s.Position, "Position after Write at step k={0}", k);
								Assert.AreEqual(r.Length, s.Length, "Length after Write at step k={0}", k);
								AssertAreEqual(rBuffer, sBuffer, "Buffer after Write at step k={0}", k);

								// because the behaviour of uninitialized bytes caused by buffer expansion is not explicitly defined,
								// we explicitly initialize those bytes in order to continue the test deterministically
								if (s.Length > oldLength)
								{
									var pos = s.Position;
									var zero = new byte[s.Length - oldLength];
									Seek(s, oldLength, SeekOrigin.Begin);
									r.Seek(oldLength, SeekOrigin.Begin);
									Write(s, zero, 0, zero.Length);
									r.Write(zero, 0, zero.Length);
									Seek(s, pos, SeekOrigin.Begin);
									r.Seek(pos, SeekOrigin.Begin);
								}
							}
							break;
						case 3: // WriteByte
						case 11:
							{
								var oldLength = s.Length;
								var value = (byte) rng.Next(0, 256);

								WriteByte(s, value);
								r.WriteByte(value);

								Assert.AreEqual(r.Position, s.Position, "Position after WriteByte at step k={0}", k);
								Assert.AreEqual(r.Length, s.Length, "Length after WriteByte at step k={0}", k);

								// because the behaviour of uninitialized bytes caused by buffer expansion is not explicitly defined,
								// we explicitly initialize those bytes in order to continue the test deterministically
								if (s.Length > oldLength)
								{
									var pos = s.Position;
									var zero = new byte[s.Length - oldLength];
									Seek(s, oldLength, SeekOrigin.Begin);
									r.Seek(oldLength, SeekOrigin.Begin);
									Write(s, zero, 0, zero.Length);
									r.Write(zero, 0, zero.Length);
									Seek(s, pos, SeekOrigin.Begin);
									r.Seek(pos, SeekOrigin.Begin);
								}
							}
							break;
						case 4: // Seek
						case 10:
							{
								int offset;
								SeekOrigin origin;

								switch (rng.Next(0, 3))
								{
									case 0:
										offset = rng.Next(0, (int) s.Length + 1024);
										origin = SeekOrigin.Begin;
										break;
									case 1:
										offset = rng.Next(-(int) s.Position, (int) s.Length - (int) s.Position + 1024);
										origin = SeekOrigin.Current;
										break;
									case 2:
									default:
										offset = rng.Next(-(int) s.Length, 1024);
										origin = SeekOrigin.End;
										break;
								}

								Seek(s, offset, origin);
								r.Seek(offset, origin);

								Assert.AreEqual(r.Position, s.Position, "Position after Seek at step k={0}", k);
								Assert.AreEqual(r.Length, s.Length, "Length after Seek at step k={0}", k);
							}
							break;
						case 5: // SetLength
							{
								var oldLength = s.Length;
								var length = (int) s.Length + rng.Next(-4096, 4096);

								// because the behaviour of Position after a SetLength call is not explicitly defined,
								// we explicitly set it here in order to continue the test deterministically
								var newPosition = rng.Next(0, (int) s.Length + 1024);

								SetLength(s, length);
								r.SetLength(length);

								Assert.AreEqual(r.Length, s.Length, "Length after SetLength at step k={0}", k);

								// because the behaviour of uninitialized bytes caused by buffer expansion is not explicitly defined,
								// we explicitly initialize those bytes in order to continue the test deterministically
								if (s.Length > oldLength)
								{
									var zero = new byte[s.Length - oldLength];
									Seek(s, oldLength, SeekOrigin.Begin);
									r.Seek(oldLength, SeekOrigin.Begin);
									Write(s, zero, 0, zero.Length);
									r.Write(zero, 0, zero.Length);
								}

								Seek(s, newPosition, SeekOrigin.Begin);
								r.Seek(newPosition, SeekOrigin.Begin);
							}
							break;
						default:
							throw new InvalidOperationException("Invalid OP code generated");
					}
				}
				Console.WriteLine("Completed executing {0} randomized operations", operationCount);

				var rArray = r.ToArray();
				var sArray = new byte[s.Length];
				Seek(s, 0, SeekOrigin.Begin);
				Assert.AreEqual(sArray.Length, Read(s, sArray, 0, sArray.Length), "Bytes Read while dumping stream contents");
				//AssertAreEqual(rArray, sArray, "Dump of Stream Contents");

				using (var hashProvider = new SHA256CryptoServiceProvider2())
				{
					var rHash = hashProvider.ComputeHash(rArray);
					var sHash = hashProvider.ComputeHash(sArray);
					Assert.AreEqual(rHash, sHash, "Hash of Stream Contents");

					var hashString = StringUtilities.ToHexString(sHash);
					Console.WriteLine("Final stream has a hash value of {0}", hashString);
				}
			}
		}

		protected virtual long Seek(T s, long offset, SeekOrigin origin)
		{
			object displayResult = null;
			try
			{
				var result = s.Seek(offset, origin);
				displayResult = result;
				return result;
			}
			catch (Exception ex)
			{
				displayResult = ex.GetType().FullName;
				throw;
			}
			finally
			{
				Console.WriteLine("Seek(offset:{0}, origin:{1}) => {2}", offset, origin, displayResult);
			}
		}

		protected virtual int Read(T s, byte[] buffer, int offset, int count)
		{
			object displayResult = null;
			try
			{
				var result = s.Read(buffer, offset, count);
				displayResult = result;
				return result;
			}
			catch (Exception ex)
			{
				displayResult = ex.GetType().FullName;
				throw;
			}
			finally
			{
				Console.WriteLine("Read(buffer:byte[{0}], offset:{1}, count:{2}) => {3}", buffer.Length, offset, count, displayResult);
			}
		}

		protected virtual int ReadByte(T s)
		{
			object displayResult = null;
			try
			{
				var result = s.ReadByte();
				displayResult = result;
				return result;
			}
			catch (Exception ex)
			{
				displayResult = ex.GetType().FullName;
				throw;
			}
			finally
			{
				Console.WriteLine("ReadByte() => {0}", displayResult);
			}
		}

		protected virtual void SetLength(T s, long length)
		{
			Console.WriteLine("SetLength(length:{0})", length);
			s.SetLength(length);
		}

		protected virtual void Write(T s, byte[] buffer, int offset, int count)
		{
			Console.WriteLine("Write(buffer:byte[{0}], offset:{1}, count:{2})", buffer.Length, offset, count);
			s.Write(buffer, offset, count);
		}

		protected virtual void WriteByte(T s, byte value)
		{
			Console.WriteLine("WriteByte(value:0x{0:X2})", value);
			s.WriteByte(value);
		}

		protected static void AssertAreEqual<TElement>(IList<TElement> expectedValues, IList<TElement> actualValues, string message, params object[] args)
		{
			Assert.AreEqual(expectedValues.Count, actualValues.Count, "Length does not match. {0}", string.Format(message, args));
			for (var n = 0; n < expectedValues.Count; ++n)
				Assert.AreEqual(expectedValues[n], actualValues[n], "Element @ index {1} does not match. {0}", string.Format(message, args), n);
		}

		protected static void AssertAreEqual<TElement>(IList<TElement> expectedValues, IList<TElement> actualValues, int actualValuesOffset, string message, params object[] args)
		{
			Assert.IsTrue(expectedValues.Count <= actualValues.Count - actualValuesOffset, "Actual array does not contain {1} elements starting at offset {2}. {0}", string.Format(message, args), expectedValues.Count, actualValuesOffset);
			for (var n = 0; n < expectedValues.Count; ++n)
				Assert.AreEqual(expectedValues[n], actualValues[actualValuesOffset + n], "Element @ actual index {1} does not match. {0}", string.Format(message, args), actualValuesOffset + n);
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
				Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);

				if (assertOnException != null)
					assertOnException.Invoke(state, ex);
			}
		}
	}
}

// ReSharper restore LocalizableElement
#endif