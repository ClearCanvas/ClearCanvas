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

using System;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.IO
{
	/// <summary>
	/// Used internally by the framework.
	/// </summary>
	public class ByteBuffer : IByteBuffer, IDisposable
	{
		/// <summary>
		/// Buffer size of 4096 for bulk byte copies.
		/// </summary>
		/// <remarks>
		/// This seems to be the buffer size most commonly used internally by the .NET framework for bulk byte copies,
		/// and testing does seem to indicate that this value gives better performance than a larger value (like 4MB)
		/// </remarks>
		private const int _bufferSize = 4096;

		private const int _highCapacityModeThreshold = 84000;

		public static Endian LocalMachineEndian = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

		#region Private Members

		private readonly bool _highCapacityMode;
		private byte[] _data;
		private Stream _ms;

		private BinaryReader _br;
		private BinaryWriter _bw;
		private Endian _endian;

		#endregion

		#region Public Constructors

		public ByteBuffer(long length = 0)
			: this(null, LocalMachineEndian, length > _highCapacityModeThreshold) {}

		public ByteBuffer(Endian endian, long length = 0)
			: this(null, endian, length > _highCapacityModeThreshold) {}

		public ByteBuffer(byte[] data)
			: this(data, LocalMachineEndian, data != null && data.Length > _highCapacityModeThreshold) {}

		public ByteBuffer(byte[] data, Endian endian)
			: this(data, endian, data != null && data.Length > _highCapacityModeThreshold) {}

		private ByteBuffer(byte[] data, Endian endian, bool highCapacityMode = false)
		{
			_data = data;
			_endian = endian;
			Encoding = Encoding.ASCII;
			SpecificCharacterSet = null;
			_highCapacityMode = highCapacityMode;
		}

		#endregion

		#region Destructor and Disposal

		~ByteBuffer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_ms != null)
				{
					_ms.Dispose();
					_ms = null;
				}

				if (_br != null)
				{
					_br.Dispose();
					_br = null;
				}

				if (_bw != null)
				{
					_bw.Dispose();
					_bw = null;
				}

				_data = null;
			}
		}

		#endregion

		#region Public Properties

		public Stream Stream
		{
			get
			{
			    if (_ms != null)
			        return _ms;
			    AllocateStream();
			    return _ms;
			}
		}

	    private void AllocateStream(int lengthHint = 0)
	    {
	        if (_ms != null) return;

	        if (_highCapacityMode)
	        {
	            _ms = new LargeMemoryStream(_data);
	        }
	        else
	        {
	            if (_data != null)
	            {
	                _ms = new MemoryStream(_data.Length);// do not use overload with byte[] because that will make the stream non-expandable
	                _ms.Write(_data, 0, _data.Length);
	                _ms.Position = 0;
	            }
	            else
	            {
	                if (lengthHint <= 0)
	                    _ms = new MemoryStream();
	                else
	                    _ms = new MemoryStream(lengthHint);
	            }
	        }
	        _data = null;
	    }

	    public BinaryReader Reader
		{
			get { return _br ?? (_br = EndianBinaryReader.Create(Stream, Endian)); }
		}

		public BinaryWriter Writer
		{
			get { return _bw ?? (_bw = EndianBinaryWriter.Create(Stream, Endian)); }
		}

		public Endian Endian
		{
			get { return _endian; }
			set
			{
				_endian = value;
				_br = null;
				_bw = null;
			}
		}

		public Encoding Encoding { get; set; }

		public string SpecificCharacterSet { get; set; }

		public int Length
		{
			get
			{
				if (_ms != null)
					return (int) _ms.Length;
				return _data != null ? _data.Length : 0;
			}
		}

		#endregion

		#region Public Functions

		public void Clear()
		{
			_ms = null;
			_br = null;
			_bw = null;
			_data = null;
		}

		/// <summary>
		/// Truncates the buffer by removing the first <paramref name="count"/> bytes.
		/// </summary>
		/// <param name="count">Number of bytes to be truncated.</param>
		public void Chop(int count)
		{
			if (Length <= count)
			{
				// if chopping more bytes than we have data, just clear it all
				Clear();
				return;
			}

			if (_ms != null)
			{
				var buffer = new byte[_bufferSize];
				var position = _ms.Position = count;

				int bytesRead;
				while ((bytesRead = _ms.Read(buffer, 0, buffer.Length)) > 0)
				{
					// reset stream position back "count" bytes from where it was read from, and write the buffer back
					_ms.Position = position - count;
					_ms.Write(buffer, 0, bytesRead);

					// update the position
					position += bytesRead;
					_ms.Position = position;
				}

				// truncate the stream
				_ms.SetLength(position - count);
				_ms.Position = 0;
				return;
			}

			if (_data != null)
			{
				_data = GetChunk(count, _data.Length - count);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public void Append(byte[] buffer, int offset, int count)
		{
			// for an append, it's much easier to just use Streams since the buffer copy would be needed anyway
			var pos = Stream.Position;
			Stream.Seek(0, SeekOrigin.End);
			Stream.Write(buffer, offset, count);
			Stream.Position = pos;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public int CopyFrom(Stream s, int count)
		{
			// clear previous data so that next call to Stream property will create new stream instance
			Clear();
            AllocateStream(count);

			// execute copy
			var copied = CopyBytes(s, Stream, count);

			// JY: the original implementation here made an assumption that count would never exceed actual data length, but this is what would have happened if you did
			if (count > copied)
			{
				ZeroBytes(Stream, count - copied);
			}

			Stream.Position = 0;

			return copied;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bw"></param>
		public void CopyTo(BinaryWriter bw)
		{
			if (_ms != null)
			{
				var position = _ms.Position;
				_ms.Position = 0;
				_ms.CopyTo(bw.BaseStream);
				_ms.Position = position;
				return;
			}

			if (_data != null)
			{
				// Discovered a bug when saving to a network drive, where 
				// writing data in large chunks caused an IOException.  
				// Limiting single writes to a smaller size seems to fix the issue.
				const int maxSize = 1024*1024*4;
				if (_data.Length > maxSize && bw.BaseStream is FileStream)
				{
					int bytesLeft = _data.Length;
					int offset = 0;
					while (bytesLeft > 0)
					{
						int bytesToWrite = bytesLeft > maxSize ? maxSize : bytesLeft;
						bw.Write(_data, offset, bytesToWrite);
						bytesLeft -= bytesToWrite;
						offset += bytesToWrite;
					}
				}
				else
				{
					bw.Write(_data, 0, _data.Length);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public void CopyTo(Stream s, int offset, int count)
		{
			if (_ms != null)
			{
				var position = _ms.Position;
				_ms.Position = offset;
				var copied = CopyBytes(_ms, s, count);

				// JY: the original implementation here made an assumption that count would never exceed actual data length, but this is what would have happened if you did
				if (count > copied)
				{
					ZeroBytes(s, count - copied);
				}

				_ms.Position = position;
				return;
			}

			if (_data != null)
			{
				var copiableBytes = Math.Min(count, _data.Length);
				s.Write(_data, offset, copiableBytes);

				// JY: the original implementation here made an assumption that count would never exceed actual data length, but this is what would have happened if you did
				if (count > copiableBytes)
				{
					var zero = new byte[count - copiableBytes];
					s.Write(zero, 0, zero.Length);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public void CopyTo(byte[] buffer, int offset, int count)
		{
			CopyTo(buffer, offset, 0, count);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="srcOffset"></param>
		/// <param name="dstOffset"></param>
		/// <param name="count"></param>
		public void CopyTo(byte[] buffer, int srcOffset, int dstOffset, int count)
		{
			if (_ms != null)
			{
				var position = _ms.Position;
				_ms.Position = srcOffset;
				_ms.Read(buffer, dstOffset, count);
				_ms.Position = position;
				return;
			}

			if (_data != null)
			{
				Array.Copy(_data, srcOffset, buffer, dstOffset, count);
			}
		}

		public byte[] GetChunk(int offset, int count)
		{
			byte[] chunk = new byte[count];
			CopyTo(chunk, offset, count);
			return chunk;
		}

		public void FromBytes(byte[] bytes)
		{
			_data = bytes;
			_ms = null;
			_br = null;
			_bw = null;
		}

		public byte[] ToBytes()
		{
			// legacy behaviour here is to dump a MemoryStream into a byte array since we have to do a full array copy anyway
			ConvertToByteArray();

			return ToBytesCore();
		}

		private byte[] ToBytesCore()
		{
			if (_ms is LargeMemoryStream)
			{
				return ((LargeMemoryStream) _ms).ToArray();
			}
			else if (_ms is MemoryStream)
			{
				return ((MemoryStream) _ms).ToArray();
			}

			return _data ?? new byte[0];
		}

		public byte[] ToBytes(int offset, int count)
		{
			var buffer = new byte[count];
			CopyTo(buffer, offset, count);
			if (_ms != null) _ms.Position = 0;
			return buffer;
		}

// ReSharper disable InconsistentNaming

		public ushort[] ToUInt16s()
		{
			return ByteConverter.ToUInt16Array(ToBytes());
		}

		public short[] ToInt16s()
		{
			return ByteConverter.ToInt16Array(ToBytes());
		}

		public uint[] ToUInt32s()
		{
			return ByteConverter.ToUInt32Array(ToBytes());
		}

		public int[] ToInt32s()
		{
			return ByteConverter.ToInt32Array(ToBytes());
		}

// ReSharper restore InconsistentNaming

		public float[] ToFloats()
		{
			return ByteConverter.ToFloatArray(ToBytes());
		}

		public double[] ToDoubles()
		{
			return ByteConverter.ToDoubleArray(ToBytes());
		}

		public string GetString()
		{
			var buffer = ToBytes();
			if (SpecificCharacterSet != null)
				return DicomImplementation.CharacterParser.Decode(buffer, SpecificCharacterSet);
			return Encoding.GetString(buffer);
		}

		public void SetString(string stringValue)
		{
			Clear();

			if (string.IsNullOrEmpty(stringValue)) return;

			if (SpecificCharacterSet != null)
			{
				_data = DicomImplementation.CharacterParser.Encode(stringValue, SpecificCharacterSet);
			}
			else
			{
				_data = Encoding.GetBytes(stringValue);
			}
		}

		public void SetString(string stringValue, byte paddingByte)
		{
			Clear();

			if (string.IsNullOrEmpty(stringValue)) return;

			if (SpecificCharacterSet != null)
			{
				_data = DicomImplementation.CharacterParser.Encode(stringValue, SpecificCharacterSet);
				if (_data != null && (_data.Length & 1) == 1)
				{
					byte[] rawBytes = new byte[_data.Length + 1];
					rawBytes[_data.Length] = paddingByte;

					_data.CopyTo(rawBytes, 0);
					_data = rawBytes;
				}
			}
			else
			{
				int count = Encoding.GetByteCount(stringValue);
				if ((count & 1) == 1)
					count++;

				byte[] bytes = new byte[count];
				if (Encoding.GetBytes(stringValue, 0, stringValue.Length, bytes, 0) < count)
					bytes[count - 1] = paddingByte;

				_data = bytes;
			}
		}

		public void Swap(int bytesToSwap)
		{
			// use the more efficient methods if it's one of the common swap window sizes
			switch (bytesToSwap)
			{
				case 1:
					return; // NOP
				case 2:
					Swap2();
					return;
				case 4:
					Swap4();
					return;
			}

			if (bytesToSwap < 1)
			{
				const string message = "bytesToSwap must be a positive integer";
				throw new ArgumentOutOfRangeException("bytesToSwap", bytesToSwap, message);
			}

			// switch stream to byte array if possible, since swapping is easier that way
			ConvertToByteArray();

			if (_ms != null)
			{
				// the swap buffer must be a multiple of bytesToSwap
				var buffer = new byte[(_bufferSize/bytesToSwap + (_bufferSize%bytesToSwap != 0 ? 1 : 0))*bytesToSwap];
				var position = _ms.Position = 0;

				// N.B. we know _ms is a LargeMemoryStream that will never read an incomplete buffer unless it's actually the end of stream
				// so we can assume that we won't be throwing in a padding zero when that byte just hasn't been read yet!
				int bytesRead;
				while ((bytesRead = _ms.Read(buffer, 0, buffer.Length)) > 0)
				{
					// if the buffer is not full, zero the bytes after the data because the swapping may cause those bytes to be introduced
					if (bytesRead < buffer.Length) Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

					// perform the swap
					SwapCore(buffer, bytesToSwap);

					// reset stream position back to where the buffer was read from, and write the swapped buffer back
					_ms.Position = position;
					_ms.Write(buffer, 0, bytesRead);

					// update the position
					position += bytesRead;
				}
				_ms.Position = 0;
				return;
			}

			if (_data != null)
			{
				// perform swap on the byte array
				SwapCore(_data, bytesToSwap);
			}
		}

		public void Swap2()
		{
			// switch stream to byte array if possible, since swapping is easier that way
			ConvertToByteArray();

			if (_ms != null)
			{
				var buffer = new byte[_bufferSize];
				var position = _ms.Position = 0;

				// N.B. we know _ms is a LargeMemoryStream that will never read an incomplete buffer unless it's actually the end of stream
				// so we can assume that we won't be throwing in a padding zero when that byte just hasn't been read yet!
				int bytesRead;
				while ((bytesRead = _ms.Read(buffer, 0, buffer.Length)) > 0)
				{
					// if the buffer is not full, zero the bytes after the data because the swapping may cause those bytes to be introduced
					if (bytesRead < buffer.Length) Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

					// perform the swap
					Swap2Core(buffer);

					// reset stream position back to where the buffer was read from, and write the swapped buffer back
					_ms.Position = position;
					_ms.Write(buffer, 0, bytesRead);

					// update the position
					position += bytesRead;
				}
				_ms.Position = 0;
				return;
			}

			if (_data != null)
			{
				// perform swap on the byte array
				Swap2Core(_data);
			}
		}

		public void Swap4()
		{
			// switch stream to byte array if possible, since swapping is easier that way
			ConvertToByteArray();

			if (_ms != null)
			{
				var buffer = new byte[_bufferSize];
				var position = _ms.Position = 0;

				// N.B. we know _ms is a LargeMemoryStream that will never read an incomplete buffer unless it's actually the end of stream
				// so we can assume that we won't be throwing in a padding zero when that byte just hasn't been read yet!
				int bytesRead;
				while ((bytesRead = _ms.Read(buffer, 0, buffer.Length)) > 0)
				{
					// if the buffer is not full, zero the bytes after the data because the swapping may cause those bytes to be introduced
					if (bytesRead < buffer.Length) Array.Clear(buffer, bytesRead, buffer.Length - bytesRead);

					// perform the swap
					Swap4Core(buffer);

					// reset stream position back to where the buffer was read from, and write the swapped buffer back
					_ms.Position = position;
					_ms.Write(buffer, 0, bytesRead);

					// update the position
					position += bytesRead;
				}
				_ms.Position = 0;
				return;
			}

			if (_data != null)
			{
				// perform swap on the byte array
				Swap4Core(_data);
			}
		}

		public void Swap8()
		{
			Swap(8);
		}

		private static void Swap2Core(byte[] data)
		{
			var length = data.Length - (data.Length%2);
			for (var i = 0; i < length; i += 2)
			{
				var b = data[i + 1];
				data[i + 1] = data[i];
				data[i] = b;
			}
		}

		private static void Swap4Core(byte[] data)
		{
			var length = data.Length - (data.Length%4);
			for (var i = 0; i < length; i += 4)
			{
				var b = data[i + 3];
				data[i + 3] = data[i];
				data[i] = b;
				b = data[i + 2];
				data[i + 2] = data[i + 1];
				data[i + 1] = b;
			}
		}

		private static void SwapCore(byte[] data, int windowSize)
		{
			var length = data.Length - (data.Length%windowSize);
			for (var i = 0; i < length; i += windowSize)
			{
				Array.Reverse(data, i, windowSize);
			}
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS

		internal ByteBuffer(bool highCapacityMode)
			: this(null, LocalMachineEndian, highCapacityMode) {}

		internal ByteBuffer(byte[] data, bool highCapacityMode)
			: this(data, LocalMachineEndian, highCapacityMode) {}

		internal byte[] GetTestData()
		{
			return _data;
		}

		internal Stream GetTestStream()
		{
			return _ms;
		}

		byte[] IByteBuffer.GetTestData()
		{
			return GetTestData();
		}

		Stream IByteBuffer.GetTestStream()
		{
			return GetTestStream();
		}

#endif

		#endregion

		/// <summary>
		/// Converts internal storage to byte array if it was stored in a MemoryStream. Doesn't do anything otherwise. Costs one buffer copy, but may be useful if the next operation is better performed on an array.
		/// </summary>
		private void ConvertToByteArray()
		{
			if (_ms is MemoryStream)
			{
				_data = ((MemoryStream) _ms).ToArray();
				_br = null;
				_bw = null;
				_ms = null;
			}
		}

		private static int CopyBytes(Stream src, Stream dst, int count)
		{
			var bytesCopied = 0;
			var buffer = new byte[_bufferSize];

			int bytesRead;
			while (bytesCopied < count && (bytesRead = src.Read(buffer, 0, Math.Min(buffer.Length, count - bytesCopied))) > 0)
			{
				dst.Write(buffer, 0, bytesRead);
				bytesCopied += bytesRead;
			}

			return bytesCopied;
		}

		private static int ZeroBytes(Stream dst, int count)
		{
			var bytesZeroed = 0;
			var zeroes = new byte[_bufferSize];

			while (bytesZeroed < count)
			{
				var bytesLeft = Math.Min(_bufferSize, count - bytesZeroed);
				dst.Write(zeroes, 0, bytesLeft);
				bytesZeroed += bytesLeft;
			}

			return bytesZeroed;
		}
	}
}