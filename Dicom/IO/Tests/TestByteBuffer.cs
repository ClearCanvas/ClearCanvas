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
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.IO.Tests
{
	/// <summary>
	/// Wraps a <see cref="ByteBuffer"/> instance with performance measurement code, byte buffer isolation, and other unit test infrastructure.
	/// </summary>
	internal sealed class TestByteBuffer : IByteBuffer, IDisposable
	{
		private IByteBuffer _bb;

		public TestByteBuffer()
		{
			if (_useLegacyImplementation)
				_bb = new LegacyByteBuffer();
			else
				_bb = new ByteBuffer(_useHighCapacityMode);
		}

		public TestByteBuffer(byte[] buffer)
		{
			if (_useLegacyImplementation)
				_bb = new LegacyByteBuffer(CopyBytes(buffer));
			else
				_bb = new ByteBuffer(CopyBytes(buffer), _useHighCapacityMode);
		}

		public void Dispose()
		{
			var disposable = _bb as IDisposable;
			if (disposable != null) (disposable).Dispose();
			_bb = null;
		}

		public int Length
		{
			get { return _bb.Length; }
		}

		public Stream Stream
		{
			get { return _bb.Stream; }
		}

		public BinaryReader Reader
		{
			get { return _bb.Reader; }
		}

		public BinaryWriter Writer
		{
			get { return _bb.Writer; }
		}

		public Endian Endian
		{
			get { return _bb.Endian; }
			set { _bb.Endian = value; }
		}

		public Encoding Encoding
		{
			get { return _bb.Encoding; }
			set { _bb.Encoding = value; }
		}

		public long StreamPosition
		{
			get
			{
				var ms = _bb.GetTestStream();
				return ms != null ? ms.Position : 0;
			}
		}

		public string SpecificCharacterSet
		{
			get { return _bb.SpecificCharacterSet; }
			set { _bb.SpecificCharacterSet = value; }
		}

		/// <summary>
		/// Forces the Stream property to be lazily initialized, which can alter the execution path taken by various methods of ByteBuffer.
		/// </summary>
		public void InitializeStream(long? position = null)
		{
			var x = _bb.Stream;
			if (x == null)
				throw new Exception("Failed to lazy-initialize Stream");
			if (position.HasValue)
				x.Position = position.Value;
		}

		public byte[] DumpTestData()
		{
			var ms = _bb.GetTestStream();
			if (ms is MemoryStream) return ((MemoryStream) ms).ToArray();
			else if (ms is LargeMemoryStream) return ((LargeMemoryStream) ms).ToArray();
			return _bb.GetTestData() ?? new byte[0];
		}

		public void Clear()
		{
			_bb.Clear();
		}

		public void Append(byte[] buffer, int offset, int count)
		{
			// always copy the buffer to ensure isolation of the input data from the object under test
			buffer = CopyBytes(buffer);

			var cc = new CodeClock();
			cc.Start();
			_bb.Append(buffer, offset, count);
			cc.Stop();
			Report(cc, "Append", count);
		}

		public void Chop(int count)
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.Chop(count);
			cc.Stop();
			Report(cc, "Chop", length - count);
		}

		public int CopyFrom(Stream s, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.CopyFrom(s, count);
			cc.Stop();
			Report(cc, "CopyFrom", count);
			return result;
		}

		public void CopyTo(BinaryWriter bw)
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.CopyTo(bw);
			cc.Stop();
			Report(cc, "CopyTo(BinaryWriter)", length);
		}

		public void CopyTo(Stream s, int offset, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			_bb.CopyTo(s, offset, count);
			cc.Stop();
			Report(cc, "CopyTo(Stream,int,int)", count);
		}

		public void CopyTo(byte[] buffer, int offset, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			_bb.CopyTo(buffer, offset, count);
			cc.Stop();
			Report(cc, "CopyTo(byte[],int,int)", count);
		}

		public void CopyTo(byte[] buffer, int srcOffset, int dstOffset, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			_bb.CopyTo(buffer, srcOffset, dstOffset, count);
			cc.Stop();
			Report(cc, "CopyTo(byte[],int,int,int)", count);
		}

		public byte[] GetChunk(int offset, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.GetChunk(offset, count);
			cc.Stop();
			Report(cc, "GetChunk", count);
			return result;
		}

		public void FromBytes(byte[] bytes)
		{
			// always copy the buffer to ensure isolation of the input data from the object under test
			bytes = CopyBytes(bytes);

			var cc = new CodeClock();
			cc.Start();
			_bb.FromBytes(bytes);
			cc.Stop();
			Report(cc, "FromBytes", bytes.Length);
		}

		public byte[] ToBytes()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToBytes();
			cc.Stop();
			Report(cc, "ToBytes()", length);
			return result;
		}

		public byte[] ToBytes(int offset, int count)
		{
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToBytes(offset, count);
			cc.Stop();
			Report(cc, "ToBytes(int,int)", count);
			return result;
		}

		public string GetString()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.GetString();
			cc.Stop();
			Report(cc, "GetString", length);
			return result;
		}

		public void SetString(string stringValue)
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.SetString(stringValue);
			cc.Stop();
			Report(cc, "SetString(string)", length);
		}

		public void SetString(string stringValue, byte paddingByte)
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.SetString(stringValue, paddingByte);
			cc.Stop();
			Report(cc, "SetString(string,byte)", length);
		}

		public void Swap(int bytesToSwap)
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.Swap(bytesToSwap);
			cc.Stop();
			Report(cc, "Swap", length);
		}

		public void Swap2()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.Swap2();
			cc.Stop();
			Report(cc, "Swap2", length);
		}

		public void Swap4()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.Swap4();
			cc.Stop();
			Report(cc, "Swap4", length);
		}

		public void Swap8()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			_bb.Swap8();
			cc.Stop();
			Report(cc, "Swap8", length);
		}

		public ushort[] ToUInt16s()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToUInt16s();
			cc.Stop();
			Report(cc, "ToUInt16s", length);
			return result;
		}

		public short[] ToInt16s()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToInt16s();
			cc.Stop();
			Report(cc, "ToInt16s", length);
			return result;
		}

		public uint[] ToUInt32s()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToUInt32s();
			cc.Stop();
			Report(cc, "ToUInt32s", length);
			return result;
		}

		public int[] ToInt32s()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToInt32s();
			cc.Stop();
			Report(cc, "ToInt32s", length);
			return result;
		}

		public float[] ToFloats()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToFloats();
			cc.Stop();
			Report(cc, "ToFloats", length);
			return result;
		}

		public double[] ToDoubles()
		{
			var length = Length;
			var cc = new CodeClock();
			cc.Start();
			var result = _bb.ToDoubles();
			cc.Stop();
			Report(cc, "ToDoubles", length);
			return result;
		}

		byte[] IByteBuffer.GetTestData()
		{
			return _bb.GetTestData();
		}

		Stream IByteBuffer.GetTestStream()
		{
			return _bb.GetTestStream();
		}

		private void Report(CodeClock cc, string methodName, int bytesProcessed = 0)
		{
			var elapsedMilliseconds = cc.Seconds*1000;
			if (bytesProcessed < 0) bytesProcessed = 0;

			LastStatistics = new TestByteBufferStatistics {Method = methodName, TimeMilliseconds = elapsedMilliseconds, BytesProcessed = bytesProcessed};

			if (NoReport) return;

			if (bytesProcessed > 0)
			{
				const string message = "{0}: processed {1} bytes in {2:f0} ms ({3:f1} kB/s average)";
				Console.WriteLine(message, methodName, bytesProcessed, elapsedMilliseconds, bytesProcessed/elapsedMilliseconds);
			}
			else
			{
				const string message = "{0}: completed in {1:f0} ms";
				Console.WriteLine(message, methodName, elapsedMilliseconds);
			}
		}

		public TestByteBufferStatistics LastStatistics { get; private set; }

		internal static byte[] CopyBytes(byte[] data)
		{
			// creates a second copy, so that we never directly pass control of the buffer to object under test in case it modifies the buffer
			var buffer = new byte[data.Length];
			Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
			return buffer;
		}

		private static bool _useHighCapacityMode = false;

		public static bool UseHighCapacityMode
		{
			get { return _useHighCapacityMode; }
			set
			{
				if (_useHighCapacityMode == value) return;
				_useHighCapacityMode = value;

				const string message = "ByteBufferTests - HighCapacityMode {0}";
				Console.WriteLine(message, value ? "ENABLED" : "DISABLED");
			}
		}

		private static bool _useLegacyImplementation = false;

		public static bool UseLegacyImplementation
		{
			get { return _useLegacyImplementation; }
			set
			{
				if (_useLegacyImplementation == value) return;
				_useLegacyImplementation = value;

				const string message = "ByteBufferTests - LegacyImplementation {0}";
				Console.WriteLine(message, value ? "ENABLED" : "DISABLED");
			}
		}

		public static bool NoReport { get; set; }
	}

	internal struct TestByteBufferStatistics
	{
		public string Method { get; set; }
		public float TimeMilliseconds { get; set; }
		public int BytesProcessed { get; set; }

		public float AverageThroughput
		{
			get { return BytesProcessed/TimeMilliseconds; }
		}

		public override string ToString()
		{
			return string.Format("\"{0}\",{1:f0},{2},{3:f0}", !string.IsNullOrEmpty(Method) ? Method.Replace("\"", "\"\"") : string.Empty, TimeMilliseconds, BytesProcessed, AverageThroughput);
		}

		public static readonly TestByteBufferStatistics Empty = new TestByteBufferStatistics();

		public static TestByteBufferStatistics operator +(TestByteBufferStatistics a, TestByteBufferStatistics b)
		{
			if (!string.IsNullOrEmpty(a.Method) && !string.IsNullOrEmpty(b.Method) && a.Method != b.Method)
				throw new ArgumentException();
			return new TestByteBufferStatistics
			       	{
			       		Method = !string.IsNullOrEmpty(a.Method) ? a.Method : b.Method,
			       		BytesProcessed = a.BytesProcessed + b.BytesProcessed,
			       		TimeMilliseconds = a.TimeMilliseconds + b.TimeMilliseconds
			       	};
		}
	}
}

#endif