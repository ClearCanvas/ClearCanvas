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
using ClearCanvas.Common.Utilities.Tests;
using ClearCanvas.Dicom.IO;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	/// <summary>
	/// Base test implementation for <see cref="DicomAttributeBinaryData{T}"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DicomAttributeBinaryDataTestBase<T>
		where T : struct
	{
		/// <summary>
		/// Implement to provide the expected size of T in bytes.
		/// </summary>
		protected abstract int SizeOfT { get; }

		/// <summary>
		/// Implement to generate a number of T values in an array.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		protected abstract T[] GenerateValues(int count);

		[Test]
		public void TestSizeOfT()
		{
			Assert.AreEqual(SizeOfT, DicomAttributeBinaryData<T>.TestSizeOfT);
		}

		[Test]
		public void TestConstructor()
		{
			Assert.AreEqual(0, new DicomAttributeBinaryData<T>().Count, ".ctor(0)");
			Assert.AreEqual(10, new DicomAttributeBinaryData<T>(10).Count, ".ctor(10)");

			var d = new DicomAttributeBinaryData<T>(DicomAttributeBinaryData<T>.TestStreamThresholdInValues);
			Assert.IsNotNull(d.TestGetArray(), "array in .ctor(threshold)");
			Assert.IsNull(d.TestGetStream(), "stream in .ctor(threshold)");

			d = new DicomAttributeBinaryData<T>(DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1);
			Assert.IsNull(d.TestGetArray(), "array in .ctor(threshold+1)");
			Assert.IsNotNull(d.TestGetStream(), "stream in .ctor(threshold+1)");
		}

		[Test]
		public void TestConstructorArray()
		{
			var array0 = GenerateValues(100);
			var data0 = new DicomAttributeBinaryData<T>(array0, false);

			Assert.AreSame(array0, data0.TestGetArray(), "no copy");

			data0 = new DicomAttributeBinaryData<T>(array0, true);
			Assert.AreNotSame(array0, data0.TestGetArray(), "copy (reference)");
			Assert.AreEqual(array0, data0.TestGetArray(), "copy (contents)");

			array0 = GenerateValues(DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1);
			data0 = new DicomAttributeBinaryData<T>(array0, true);
			Assert.IsNull(data0.TestGetArray(), "copy to stream (array)");
			Assert.AreEqual(array0, ConvertToTArray(data0.TestGetStream().ToArray()), "copy to stream (contents)");
		}

		[Test]
		public void TestConstructorCopy()
		{
			var array0 = GenerateValues(100);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var data1 = new DicomAttributeBinaryData<T>(data0);

			Assert.AreEqual(array0, data1.TestGetArray(), "arrays");
		}

		[Test]
		public void TestConstructorCopy2()
		{
			var array0 = GenerateValues(10000);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var data1 = new DicomAttributeBinaryData<T>(data0);
			var buffer1 = data1.TestGetStream().ToArray();
			var array1 = new T[10000];
			Buffer.BlockCopy(buffer1, 0, array1, 0, buffer1.Length);

			Assert.AreEqual(array0, array1, "arrays");
		}

		[Test]
		public void TestByteLength()
		{
			Assert.AreEqual(0, new DicomAttributeBinaryData<T>(0).Length);
			Assert.AreEqual(101*SizeOfT, new DicomAttributeBinaryData<T>(101).Length);
			Assert.AreEqual((DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1)*SizeOfT, new DicomAttributeBinaryData<T>(DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1).Length);
		}

		[Test]
		public void TestValueCount()
		{
			Assert.AreEqual(0, new DicomAttributeBinaryData<T>(0).Count);
			Assert.AreEqual(101, new DicomAttributeBinaryData<T>(101).Count);
			Assert.AreEqual((DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1), new DicomAttributeBinaryData<T>(DicomAttributeBinaryData<T>.TestStreamThresholdInValues + 1).Count);
		}

		[Test]
		public void TestIndexer()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "get array[{0}]", n);

			for (var n = 0; n < testSize; ++n)
				data0[n] = array0[n] = array0[(n + 25)%testSize];

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "set array[{0}]", n);

			var dummy = default(T);
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[-1], "Getting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[testSize], "Getting a value @100");
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[testSize + 1], "Getting a value @101");
			Assert.Throws<IndexOutOfRangeException>(() => data0[-1] = default(T), "Setting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0[testSize + 1] = default(T), "Setting a value @101");
			Assert.DoesNotThrow(() => data0[testSize] = default(T), "Setting a value @100");
			Assert.AreEqual(default(T), dummy);
		}

		[Test]
		public void TestIndexer2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "get array[{0}]", n);

			for (var n = 0; n < testSize; ++n)
				data0[n] = array0[n] = array0[(n + 25)%testSize];

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "set array[{0}]", n);

			var dummy = default(T);
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[-1], "Getting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[testSize], "Getting a value @10000");
			Assert.Throws<IndexOutOfRangeException>(() => dummy = data0[testSize + 1], "Getting a value @10001");
			Assert.Throws<IndexOutOfRangeException>(() => data0[-1] = default(T), "Setting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0[testSize + 1] = default(T), "Setting a value @10001");
			Assert.DoesNotThrow(() => data0[testSize] = default(T), "Setting a value @10000");
			Assert.AreEqual(default(T), dummy);
		}

		[Test]
		public void TestGetSetValue()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0.GetValue(n), "get array[{0}]", n);

			for (var n = 0; n < testSize; ++n)
				data0.SetValue(n, array0[n] = array0[(n + 50)%testSize]);

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0.GetValue(n), "set array[{0}]", n);

			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(-1), "Getting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(testSize), "Getting a value @100");
			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(testSize + 1), "Getting a value @101");
			Assert.Throws<IndexOutOfRangeException>(() => data0.SetValue(-1, default(T)), "Setting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0.SetValue(testSize + 1, default(T)), "Setting a value @101");
			Assert.DoesNotThrow(() => data0.SetValue(testSize, default(T)), "Setting a value @100");
		}

		[Test]
		public void TestGetSetValue2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0.GetValue(n), "get array[{0}]", n);

			for (var n = 0; n < testSize; ++n)
				data0.SetValue(n, array0[n] = array0[(n + 500)%testSize]);

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0.GetValue(n), "set array[{0}]", n);

			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(-1), "Getting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(testSize), "Getting a value @10000");
			Assert.Throws<IndexOutOfRangeException>(() => data0.GetValue(testSize + 1), "Getting a value @10001");
			Assert.Throws<IndexOutOfRangeException>(() => data0.SetValue(-1, default(T)), "Setting a value @-1");
			Assert.Throws<IndexOutOfRangeException>(() => data0.SetValue(testSize + 1, default(T)), "Setting a value @10001");
			Assert.DoesNotThrow(() => data0.SetValue(testSize, default(T)), "Setting a value @10000");
		}

		[Test]
		public void TestTryGetValue()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			T value;

			for (var n = -25; n < 0; ++n)
				Assert.AreEqual(false, data0.TryGetValue(n, out value), "TryGet[{0}]", n);

			for (var n = 0; n < testSize; ++n)
			{
				Assert.AreEqual(true, data0.TryGetValue(n, out value), "TryGet[{0}]", n);
				Assert.AreEqual(array0[n], value, "TryGet[{0}]", n);
			}

			for (var n = testSize; n < testSize + 25; ++n)
				Assert.AreEqual(false, data0.TryGetValue(n, out value), "TryGet[{0}]", n);
		}

		[Test]
		public void TestTryGetValue2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			T value;

			for (var n = -25; n < 0; ++n)
				Assert.AreEqual(false, data0.TryGetValue(n, out value), "TryGet[{0}]", n);

			for (var n = 0; n < testSize; ++n)
			{
				Assert.AreEqual(true, data0.TryGetValue(n, out value), "TryGet[{0}]", n);
				Assert.AreEqual(array0[n], value, "TryGet[{0}]", n);
			}

			for (var n = testSize; n < testSize + 25; ++n)
				Assert.AreEqual(false, data0.TryGetValue(n, out value), "TryGet[{0}]", n);
		}

		[Test]
		public void TestAppendValue()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var array1 = GenerateValues(25);
			for (var n = 0; n < 25; ++n)
				data0.AppendValue(array1[n]);
			for (var n = 0; n < 25; ++n)
				data0[125 + n] = array1[n];

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "@[{0}]", n);

			for (var n = testSize; n < testSize + 25; ++n)
				Assert.AreEqual(array1[n - testSize], data0[n], "@[{0}]", n);

			for (var n = testSize + 25; n < testSize + 50; ++n)
				Assert.AreEqual(array1[n - testSize - 25], data0[n], "@[{0}]", n);
		}

		[Test]
		public void TestAppendValue2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var array1 = GenerateValues(250);
			for (var n = 0; n < 250; ++n)
				data0.AppendValue(array1[n]);
			for (var n = 0; n < 250; ++n)
				data0[10250 + n] = array1[n];

			for (var n = 0; n < testSize; ++n)
				Assert.AreEqual(array0[n], data0[n], "@[{0}]", n);

			for (var n = testSize; n < testSize + 250; ++n)
				Assert.AreEqual(array1[n - testSize], data0[n], "@[{0}]", n);

			for (var n = testSize + 250; n < testSize + 500; ++n)
				Assert.AreEqual(array1[n - testSize - 250], data0[n], "@[{0}]", n);
		}

		[Test]
		public void TestToArray()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var array = data0.ToArray();
			Assert.AreEqual(array0, array);
		}

		[Test]
		public void TestToArray2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var array = data0.ToArray();
			Assert.AreEqual(array0, array);
		}

		[Test]
		public void TestGetEnumerator()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var enumerator = data0.GetEnumerator();
			for (var n = 0; n < testSize; ++n)
			{
				Assert.AreEqual(true, enumerator.MoveNext(), "MoveNext @{0}", n);
				Assert.AreEqual(array0[n], enumerator.Current, "Current @{0}", n);
			}
			Assert.AreEqual(false, enumerator.MoveNext(), "MoveNext @END");
		}

		[Test]
		public void TestGetEnumerator2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var enumerator = data0.GetEnumerator();
			for (var n = 0; n < testSize; ++n)
			{
				Assert.AreEqual(true, enumerator.MoveNext(), "MoveNext @{0}", n);
				Assert.AreEqual(array0[n], enumerator.Current, "Current @{0}", n);
			}
			Assert.AreEqual(false, enumerator.MoveNext(), "MoveNext @END");
		}

		[Test]
		public void TestCompareValues()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			var data1 = new DicomAttributeBinaryData<T>(array0, true);

			AssertCompareValues(true, data0, data1, "initial");

			data1.SetValue(0, default(T));
			AssertCompareValues(false, data0, data1, "after setting 0 on one");

			data0.SetValue(0, default(T));
			AssertCompareValues(true, data0, data1, "after setting 0 on both");

			data1.SetValue(testSize/2, default(T));
			AssertCompareValues(false, data0, data1, "after setting {0} on one", testSize/2);

			data0.SetValue(testSize/2, default(T));
			AssertCompareValues(true, data0, data1, "after setting {0} on both", testSize/2);

			data1.SetValue(testSize, default(T));
			AssertCompareValues(false, data0, data1, "after setting {0} on one", testSize);

			data0.SetValue(testSize, default(T));
			AssertCompareValues(true, data0, data1, "after setting {0} on both", testSize);

			data1.AppendValue(default(T));
			AssertCompareValues(false, data0, data1, "after appending on one");

			data0.AppendValue(default(T));
			AssertCompareValues(true, data0, data1, "after appending on both");
		}

		[Test]
		public void TestCompareValues2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var data1 = new DicomAttributeBinaryData<T>(array0, true);

			AssertCompareValues(true, data0, data1, "initial");

			data1.SetValue(0, default(T));
			AssertCompareValues(false, data0, data1, "after setting 0 on one");

			data0.SetValue(0, default(T));
			AssertCompareValues(true, data0, data1, "after setting 0 on both");

			data1.SetValue(testSize/2, default(T));
			AssertCompareValues(false, data0, data1, "after setting {0} on one", testSize/2);

			data0.SetValue(testSize/2, default(T));
			AssertCompareValues(true, data0, data1, "after setting {0} on both", testSize/2);

			data1.SetValue(testSize, default(T));
			AssertCompareValues(false, data0, data1, "after setting {0} on one", testSize);

			data0.SetValue(testSize, default(T));
			AssertCompareValues(true, data0, data1, "after setting {0} on both", testSize);

			data1.AppendValue(default(T));
			AssertCompareValues(false, data0, data1, "after appending on one");

			data0.AppendValue(default(T));
			AssertCompareValues(true, data0, data1, "after appending on both");
		}

		[Test]
		public void TestAsStream()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var bufferT = new byte[SizeOfT];

			// just assert that different stream instances act as independent views
			// the semantics of the stream implementation is more rigourously asserted by DicomAttributeBinaryDataViewStreamTests
			using (var stream0 = data0.AsStream())
			{
				using (var stream1 = data0.AsStream())
				using (var stream2 = data0.AsStream())
				{
					for (var n = 0; n < testSize; ++n)
					{
						// sequential reading, no seek
						stream0.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[n], ConvertToT(bufferT), "stream0, @{0}", n);

						// sequential reading, with seek
						var index1 = (n + 43)%testSize;
						stream1.Position = index1*SizeOfT;
						stream1.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[index1], ConvertToT(bufferT), "stream1, @{1} (n = {0})", n, index1);

						// random access reading
						var index2 = n%2 == 0 ? (n + 7)%testSize : (n + 7 + testSize/2)%testSize;
						stream2.Position = index2*SizeOfT;
						stream2.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[index2], ConvertToT(bufferT), "stream2, @{1} (n = {0})", n, index2);
					}
				}

				stream0.Position = 0;
				for (var n = 0; n < testSize; ++n)
				{
					// sequential reading, no seek
					stream0.Read(bufferT, 0, SizeOfT);
					Assert.AreEqual(array0[n], ConvertToT(bufferT), "stream0 after other streams disposed, @{0}", n);
				}
			}
		}

		[Test]
		public void TestAsStream2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var bufferT = new byte[SizeOfT];

			// just assert that different stream instances act as independent views
			// the semantics of the stream implementation is more rigourously asserted by DicomAttributeBinaryDataViewStreamTests
			using (var stream0 = data0.AsStream())
			{
				using (var stream1 = data0.AsStream())
				using (var stream2 = data0.AsStream())
				{
					for (var n = 0; n < testSize; ++n)
					{
						// sequential reading, no seek
						stream0.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[n], ConvertToT(bufferT), "stream0, @{0}", n);

						// sequential reading, with seek
						var index1 = (n + 43)%testSize;
						stream1.Position = index1*SizeOfT;
						stream1.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[index1], ConvertToT(bufferT), "stream1, @{1} (n = {0})", n, index1);

						// random access reading
						var index2 = n%2 == 0 ? (n + 7)%testSize : (n + 7 + testSize/2)%testSize;
						stream2.Position = index2*SizeOfT;
						stream2.Read(bufferT, 0, SizeOfT);
						Assert.AreEqual(array0[index2], ConvertToT(bufferT), "stream2, @{1} (n = {0})", n, index2);
					}
				}

				stream0.Position = 0;
				for (var n = 0; n < testSize; ++n)
				{
					// sequential reading, no seek
					stream0.Read(bufferT, 0, SizeOfT);
					Assert.AreEqual(array0[n], ConvertToT(bufferT), "stream0 after other streams disposed, @{0}", n);
				}
			}
		}

		[Test]
		public void TestAppendValueConvertToStream()
		{
			var testSize = DicomAttributeBinaryData<T>.TestStreamThresholdInValues;
			var array0 = GenerateValues(testSize);
			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			Assert.IsNotNull(data0.TestGetArray(), "array before switch");
			Assert.IsNull(data0.TestGetStream(), "stream before switch");

			data0.AppendValue(default(T));

			Assert.IsNull(data0.TestGetArray(), "array after switch");
			Assert.IsNotNull(data0.TestGetStream(), "stream after switch");
		}

		[Test]
		public void TestCreateByteBuffer()
		{
			const int testSize = 100;
			var array0 = GenerateValues(testSize);
			var buffer0 = new byte[testSize*SizeOfT];
			Buffer.BlockCopy(array0, 0, buffer0, 0, buffer0.Length);

			var data0 = new DicomAttributeBinaryData<T>(array0, true);

			var bb = data0.CreateByteBuffer(ByteBuffer.LocalMachineEndian);
			var buffer1 = bb.GetChunk(0, bb.Length);

			Assert.AreEqual(buffer0, buffer1);
		}

		[Test]
		public void TestCreateByteBuffer2()
		{
			const int testSize = 10000;
			var array0 = GenerateValues(testSize);
			var buffer0 = new byte[testSize*SizeOfT];
			Buffer.BlockCopy(array0, 0, buffer0, 0, buffer0.Length);

			var data0 = new DicomAttributeBinaryData<T>(array0, true);
			data0.TestUseStream();

			var bb = data0.CreateByteBuffer(ByteBuffer.LocalMachineEndian);
			var buffer1 = bb.GetChunk(0, bb.Length);

			Assert.AreEqual(buffer0, buffer1);
		}

		private static T ConvertToT(byte[] bytes)
		{
			var t = new T[1];
			Buffer.BlockCopy(bytes, 0, t, 0, bytes.Length);
			return t[0];
		}

		private T[] ConvertToTArray(byte[] bytes)
		{
			var t = new T[bytes.Length/SizeOfT];
			Buffer.BlockCopy(bytes, 0, t, 0, bytes.Length);
			return t;
		}

		private static void AssertCompareValues(bool expectedResult, DicomAttributeBinaryData<T> a, DicomAttributeBinaryData<T> b, string message, params object[] args)
		{
			var msg = string.Format(message, args);
			if (expectedResult)
			{
				Assert.IsTrue(a.CompareValues(b), msg);
				Assert.IsTrue(b.CompareValues(a), msg + " (Reversed)");
			}
			else
			{
				Assert.IsFalse(a.CompareValues(b), msg, args);
				Assert.IsFalse(b.CompareValues(a), msg + " (Reversed)");
			}
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataViewStreamTests : StreamTestBase<Stream>
	{
		protected override Stream CreateStream(byte[] seedData)
		{
			// the view stream implementation does not depend on the type parameter
			return DicomAttributeBinaryData<int>.TestCreateViewStream(seedData);
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestByte : DicomAttributeBinaryDataTestBase<byte>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 1; }
		}

		protected override byte[] GenerateValues(int count)
		{
			var bytes = new byte[count];
			_rng.NextBytes(bytes);
			return bytes;
		}

        [Test]
        public void TestCreateByteBuffer_OddLength()
        {
            const int testSize = 101;
            var array0 = GenerateValues(testSize);
            var buffer0 = new byte[testSize * SizeOfT];
            Buffer.BlockCopy(array0, 0, buffer0, 0, buffer0.Length);

            var data0 = new DicomAttributeBinaryData<byte>(array0, true);

            var bb = data0.CreateEvenLengthByteBuffer(ByteBuffer.LocalMachineEndian);
            Assert.AreEqual(102, bb.Length);

            var buffer1 = bb.GetChunk(0, testSize);
            Assert.AreEqual(buffer0, buffer1);
            
        }
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestInt16 : DicomAttributeBinaryDataTestBase<short>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 2; }
		}

		protected override short[] GenerateValues(int count)
		{
			var values = new short[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestUInt16 : DicomAttributeBinaryDataTestBase<ushort>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 2; }
		}

		protected override ushort[] GenerateValues(int count)
		{
			var values = new ushort[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestInt32 : DicomAttributeBinaryDataTestBase<int>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 4; }
		}

		protected override int[] GenerateValues(int count)
		{
			var values = new int[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestUInt32 : DicomAttributeBinaryDataTestBase<uint>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 4; }
		}

		protected override uint[] GenerateValues(int count)
		{
			var values = new uint[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestFloat32 : DicomAttributeBinaryDataTestBase<float>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 4; }
		}

		protected override float[] GenerateValues(int count)
		{
			var values = new float[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}

	[TestFixture]
	public class DicomAttributeBinaryDataTestFloat64 : DicomAttributeBinaryDataTestBase<double>
	{
		private readonly PseudoRandom _rng = new PseudoRandom(-0x1BCDFEB1);

		protected override int SizeOfT
		{
			get { return 8; }
		}

		protected override double[] GenerateValues(int count)
		{
			var values = new double[count];
			var bytes = new byte[count*SizeOfT];
			_rng.NextBytes(bytes);
			Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
			return values;
		}
	}
}

#endif