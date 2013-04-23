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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Provides storage for the values of a <see cref="DicomAttributeBinary{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// By default, <see cref="DicomAttributeBinaryData{T}"/> implements storage with an array of <typeparamref name="T"/>.
	/// If the memory requirements for an array of <typeparamref name="T"/> would exceed 64 KB, or otherwise requested
	/// by calling code, the array will be converted into a <see cref="Stream"/>. This significantly improves performance
	/// for sequential accesses of a large array as well as reducing the contiguous memory required for such a structure,
	/// at the cost of performance for random access in a small array.
	/// </para>
	/// <para><typeparamref name="T"/> must be a primitive numeric type, otherwise a <see cref="TypeInitializationException"/> will be thrown.</para>
	/// </remarks>
	/// <typeparam name="T">The type of values stored by the attribute.</typeparam>
	public class DicomAttributeBinaryData<T> : IEnumerable<T>
		where T : struct
	{
		/// <summary>
		/// The threshold, in bytes, for the switch to using a Stream for storage (keep it under the large object threshold, currently 84000)
		/// </summary>
		private const int _thresholdBytes = 65536;

		private LargeMemoryStream _stream;
		private T[] _array;

		/// <summary>
		/// Initializes a new empty instance of <see cref="DicomAttributeBinaryData{T}"/>.
		/// </summary>
		public DicomAttributeBinaryData()
			: this(0) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomAttributeBinaryData{T}"/> filled with zero values.
		/// </summary>
		/// <param name="count">Number of initial values.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative.</exception>
		public DicomAttributeBinaryData(int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			if (count*_sizeOfT <= _thresholdBytes)
			{
				_array = new T[count];
			}
			else
			{
				_stream = new LargeMemoryStream();
				_stream.SetLength(count*_sizeOfT);
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomAttributeBinaryData{T}"/>, copying from an existing array of values.
		/// </summary>
		/// <param name="values">Source array of values.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
		public DicomAttributeBinaryData(T[] values)
			: this(values, true) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomAttributeBinaryData{T}"/> from an existing array of values.
		/// </summary>
		/// <param name="values">Source array of values.</param>
		/// <param name="copyArray">Indicates whether <paramref name="values"/> should be copied or if the array can be taken as owned by the <see cref="DicomAttributeBinaryData{T}"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
		public DicomAttributeBinaryData(T[] values, bool copyArray)
		{
			if (values == null) throw new ArgumentNullException("values");
			if (!copyArray)
			{
				// no reason to convert to a stream, since it already exists as an array
				_array = values;
			}
			else if (values.Length*_sizeOfT > _thresholdBytes)
			{
				_array = values;
				ConvertToStream();
			}
			else
			{
				_array = new T[values.Length];
				Array.Copy(values, _array, _array.Length);
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomAttributeBinaryData{T}"/> as a copy of another instance.
		/// </summary>
		/// <param name="other">Source <see cref="DicomAttributeBinaryData{T}"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="other"/> is null.</exception>
		public DicomAttributeBinaryData(DicomAttributeBinaryData<T> other)
		{
			if (other == null) throw new ArgumentNullException("other");
			if (other._array != null)
			{
				_array = new T[other._array.Length];
				other._array.CopyTo(_array, 0);
			}
			else if (other._stream != null)
			{
				_stream = new LargeMemoryStream();
				other._stream.WriteTo(_stream);
			}
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomAttributeBinaryData{T}"/> with the values from a <see cref="ByteBuffer"/>.
		/// </summary>
		/// <param name="buffer">Source <see cref="ByteBuffer"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null.</exception>
		public DicomAttributeBinaryData(ByteBuffer buffer)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (buffer.Length < _thresholdBytes)
			{
				_array = new T[buffer.Length/_sizeOfT];
				Buffer.BlockCopy(buffer.ToBytes(), 0, _array, 0, _array.Length*_sizeOfT);
			}
			else
			{
				_stream = new LargeMemoryStream();
				buffer.CopyTo(_stream, 0, buffer.Length - (buffer.Length%_sizeOfT));
			}
		}

		/// <summary>
		/// Gets the length of the stored values in bytes.
		/// </summary>
		public int Length
		{
			get { return _array != null ? _array.Length*_sizeOfT : (int) _stream.Length; }
		}

		/// <summary>
		/// Gets the number of stored values.
		/// </summary>
		public int Count
		{
			get { return _array != null ? _array.Length : (int) (_stream.Length/_sizeOfT); }
		}

		/// <summary>
		/// Gets or sets the value at the specified index.
		/// </summary>
		/// <remarks>
		/// The indexer can be used to append a value by setting the value at index <see cref="Count"/>. Getting the property at this index will throw an exception.
		/// </remarks>
		/// <param name="index">The index of the value to access.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is negative or specifies an index greater than <see cref="Count"/>.</exception>
		public T this[int index]
		{
			get { return GetValue(index); }
			set { SetValue(index, value); }
		}

		/// <summary>
		/// Gets the value at the specified index.
		/// </summary>
		/// <param name="index">The index of the value to access.</param>
		/// <returns>Returns the value at the specified index.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is negative or specifies an index greater than or equal to <see cref="Count"/>.</exception>
		public T GetValue(int index)
		{
			if (_array != null)
			{
				return _array[index];
			}
			else
			{
				if (index < 0 || index >= _stream.Length/_sizeOfT)
					throw new IndexOutOfRangeException();

				_stream.Position = index*_sizeOfT;
				return ReadValue(_stream);
			}
		}

		/// <summary>
		/// Gets the value at the specified index.
		/// </summary>
		/// <param name="index">The index of the value to access.</param>
		/// <param name="value">Returns the value at the specified index.</param>
		/// <returns>True if the index is valid and the value was read; False otherwise.</returns>
		public bool TryGetValue(int index, out T value)
		{
			if (_array != null)
			{
				if (index < 0 || index >= _array.Length)
				{
					value = default(T);
					return false;
				}
				value = _array[index];
				return true;
			}
			else
			{
				if (index < 0 || index >= _stream.Length/_sizeOfT)
				{
					value = default(T);
					return false;
				}
				_stream.Position = index*_sizeOfT;
				value = ReadValue(_stream);
				return true;
			}
		}

		/// <summary>
		/// Sets the value at the specified index.
		/// </summary>
		/// <remarks>
		/// This method can be used to append a value by setting the value at index <see cref="Count"/>.
		/// </remarks>
		/// <param name="index">The index of the value to access.</param>
		/// <param name="value">The value to set.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is negative or specifies an index greater than <see cref="Count"/>.</exception>
		public void SetValue(int index, T value)
		{
			if (index == Count)
			{
				AppendValue(value);
			}
			else if (_array != null)
			{
				_array[index] = value;
			}
			else
			{
				if (index < 0 || index >= _stream.Length/_sizeOfT)
					throw new IndexOutOfRangeException();

				_stream.Position = index*_sizeOfT;
				WriteValue(_stream, value);
			}
		}

		/// <summary>
		/// Appends a value at the end.
		/// </summary>
		/// <param name="value">The value to append.</param>
		public void AppendValue(T value)
		{
			if (_array != null && (_array.Length + 1)*_sizeOfT > _thresholdBytes) ConvertToStream();
			if (_array != null)
			{
				var temp = new T[_array.Length + 1];
				Array.Copy(_array, temp, _array.Length);
				temp[_array.Length] = value;
				_array = temp;
			}
			else
			{
				_stream.Position = _stream.Length;
				WriteValue(_stream, value);
			}
		}

		/// <summary>
		/// Creates a <see cref="ByteBuffer"/> filled with the values from this <see cref="DicomAttributeBinaryData{T}"/>.
		/// Note: The size of the returned buffer may not be even. If a buffer with even length is needed, use <see cref="CreateEvenLengthByteBuffer"/> instead.
		/// </summary>
		/// <param name="endian">The endianess of the <see cref="ByteBuffer"/>.</param>
		/// <returns>A new <see cref="ByteBuffer"/> instance containing the values of this instance.</returns>
		internal ByteBuffer CreateByteBuffer(Endian endian)
		{
			if (_array != null)
			{
				var length = _array.Length*_sizeOfT;
				var byteVal = new byte[length];
				Buffer.BlockCopy(_array, 0, byteVal, 0, length);
				return new ByteBuffer(byteVal, endian);
			}
			else
			{
				var bb = new ByteBuffer(endian, _stream.Length);
				_stream.WriteTo(bb.Stream);
				return bb;
			}
		}

        /// <summary>
        /// Creates a <see cref="ByteBuffer"/> filled with the values from this <see cref="DicomAttributeBinaryData{T}"/> 
        /// and padded with an extra byte if necessary to make it even length.
        /// </summary>
        /// <param name="endian">The endianess of the <see cref="ByteBuffer"/>.</param>
        /// <returns>A new <see cref="ByteBuffer"/> instance containing the values of this instance.</returns>
        internal ByteBuffer CreateEvenLengthByteBuffer(Endian endian)
        {
            if (_array != null)
            {
                var length = _array.Length * _sizeOfT;
                var bufferLength = length%2 == 0 ? length : length + 1;
                var byteVal = new byte[bufferLength];
                Buffer.BlockCopy(_array, 0, byteVal, 0, length);
                
                // just to be safe
                if (length % 2 == 1)
                    byteVal[bufferLength - 1] = 0;

                return new ByteBuffer(byteVal, endian);
            }
            else
            {
                var bb = new ByteBuffer(endian, _stream.Length);
                _stream.WriteTo(bb.Stream);
                
                if (_stream.Length%2==1)
                    bb.Stream.WriteByte(0x0);

                return bb;
            }
        }

		/// <summary>
		/// Gets the values of the <see cref="DicomAttributeBinaryData{T}"/> as an array.
		/// </summary>
		/// <returns>An array containing the values of this instance.</returns>
		public T[] ToArray()
		{
			if (_array != null)
			{
				return _array;
			}
			else
			{
				_stream.Position = 0;

				var valueCount = _stream.Length/_sizeOfT;
				var values = new T[valueCount];
				var buffer = new byte[_bufferSize];

				var bytesPosition = 0;
				int bytesInBuffer;
				while ((bytesInBuffer = _stream.Read(buffer, 0, _bufferSize)) > 0)
				{
					Buffer.BlockCopy(buffer, 0, values, bytesPosition, bytesInBuffer);
					bytesPosition += bytesInBuffer;
				}
				return values;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_array != null)
			{
				return ((IEnumerable<T>) _array).GetEnumerator();
			}
			else
			{
				return GetStreamEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Determines whether the values of this <see cref="DicomAttributeBinaryData{T}"/> are equal to the values of the other instance.
		/// </summary>
		/// <param name="other">The other <see cref="DicomAttributeBinaryData{T}"/> with which to compare.</param>
		/// <returns>True if the two instances have the same number of values and the values are equal pairwise; False otherwise.</returns>
		public bool CompareValues(DicomAttributeBinaryData<T> other)
		{
			var count = Count;
			if (count != other.Count)
				return false;

			for (var n = 0; n < count; ++n)
			{
				if (!Equals(GetValue(n), other.GetValue(n)))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Creates a <see cref="Stream"/> which maps to a view of this <see cref="DicomAttributeBinaryData{T}"/>, and can be used to read or write to the underlying values.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Each view <see cref="Stream"/> created by this method keeps track of position independently of all other views, although the byte stream is ultimately the same.
		/// In other words, modifying one view will cause those changes to appear in other views, but each view is still guaranteed that the <see cref="Stream.Position"/>
		/// will maintain the appropriate state regardless of external changes.
		/// </para>
		/// <para>
		/// If the values are internally stored as an array, calling this method will cause the values to be copied into a <see cref="Stream"/> and henceforth stored as such.
		/// </para>
		/// </remarks>
		/// <returns>A new instance of a <see cref="Stream"/> which maps to a view of this <see cref="DicomAttributeBinaryData{T}"/>.</returns>
		public Stream AsStream()
		{
			ConvertToStream();
			return new ViewStream(_stream);
		}

		private void ConvertToStream()
		{
			if (_array == null) return;

			var stream = new LargeMemoryStream();
			var buffer = new byte[_bufferSize];
			var streamLength = _sizeOfT*_array.Length;

			var bytesPosition = 0;
			int bytesInBuffer;
			while ((bytesInBuffer = Math.Min(_bufferSize, streamLength - bytesPosition)) > 0)
			{
				Buffer.BlockCopy(_array, bytesPosition, buffer, 0, bytesInBuffer);
				stream.Write(buffer, 0, bytesInBuffer);
				bytesPosition += bytesInBuffer;
			}

			_stream = stream;
			_array = null;
		}

		private IEnumerator<T> GetStreamEnumerator()
		{
			_stream.Position = 0;

			var values = new T[_bufferSize/_sizeOfT];
			var buffer = new byte[_bufferSize];

			int bytesInBuffer;
			while ((bytesInBuffer = _stream.Read(buffer, 0, _bufferSize)) > 0)
			{
				Buffer.BlockCopy(buffer, 0, values, 0, bytesInBuffer);
				var valuesRead = bytesInBuffer/_sizeOfT;
				for (var n = 0; n < valuesRead; ++n)
					yield return values[n];
			}
		}

		#region ViewStream Class

		/// <summary>
		/// Wraps a <see cref="Stream"/> with an independent cursor.
		/// </summary>
		private class ViewStream : Stream
		{
			private Stream _stream;
			private long _position;

			public ViewStream(Stream stream)
			{
				_stream = stream;
				_position = 0;
			}

			protected override void Dispose(bool disposing)
			{
				_stream = null;
				base.Dispose(disposing);
			}

			public override bool CanRead
			{
				get { return _stream != null; }
			}

			public override bool CanSeek
			{
				get { return _stream != null; }
			}

			public override bool CanWrite
			{
				get { return _stream != null; }
			}

			public override long Length
			{
				get
				{
					AssertIsStreamOpen();
					return _stream.Length;
				}
			}

			public override long Position
			{
				get
				{
					AssertIsStreamOpen();
					return _position;
				}
				set
				{
					AssertIsStreamOpen();

					// assert that the new position is valid (according to the semantics of the Stream base class, you can move beyond the end of stream, but not before the start of it)
					if (value < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");

					_position = value;
				}
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				AssertIsStreamOpen();

				long position;
				switch (origin)
				{
					case SeekOrigin.Current:
						position = _position + offset;
						break;
					case SeekOrigin.End:
						position = _stream.Length + offset;
						break;
					case SeekOrigin.Begin:
					default:
						position = offset;
						break;
				}

				// assert that the new position is valid (according to the semantics of the Stream base class, you can move beyond the end of stream, but not before the start of it)
				if (position < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");

				return _position = position;
			}

			public override void SetLength(long value)
			{
				AssertIsStreamOpen();
				_stream.SetLength(value);
			}

			public override int ReadByte()
			{
				AssertIsStreamOpen();
				_stream.Position = _position;
				var value = _stream.ReadByte();
				if (value >= 0) ++_position;
				return value;
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				AssertIsStreamOpen();
				_stream.Position = _position;
				var bytesRead = _stream.Read(buffer, offset, count);
				_position += bytesRead;
				return bytesRead;
			}

			public override void WriteByte(byte value)
			{
				AssertIsStreamOpen();
				_stream.Position = _position;
				_stream.WriteByte(value);
				++_position;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				AssertIsStreamOpen();
				_stream.Position = _position;
				_stream.Write(buffer, offset, count);
				_position += count;
			}

			public override void Flush() {}

			private void AssertIsStreamOpen()
			{
				if (_stream == null) throw new ObjectDisposedException(null, "Cannot access a closed Stream.");
			}
		}

		#endregion

		#region Unit Test Support

#if UNIT_TESTS

		internal static int TestStreamThresholdInValues
		{
			get { return _thresholdBytes/_sizeOfT; }
		}

		internal static int TestSizeOfT
		{
			get { return _sizeOfT; }
		}

		internal T[] TestGetArray()
		{
			return _array;
		}

		internal LargeMemoryStream TestGetStream()
		{
			return _stream;
		}

		internal void TestUseStream()
		{
			ConvertToStream();
		}

		internal static Stream TestCreateViewStream(byte[] seedData)
		{
			var stream = new MemoryStream();
			if (seedData != null) stream.Write(seedData, 0, seedData.Length);
			return new ViewStream(stream);
		}

#endif

		#endregion

		#region Static Members

// ReSharper disable StaticFieldInGenericType

		/// <summary>
		/// The size of <typeparamref name="T"/> in bytes.
		/// </summary>
		private static readonly int _sizeOfT;

		/// <summary>
		/// The size in bytes of a buffer suitable for copying values of type <typeparamref name="T"/>.
		/// </summary>
		private static readonly int _bufferSize;

// ReSharper restore StaticFieldInGenericType

		static DicomAttributeBinaryData()
		{
			if (typeof (T) == typeof (long) || typeof (T) == typeof (ulong))
			{
				_sizeOfT = sizeof (long);
			}
			else if (typeof (T) == typeof (int) || typeof (T) == typeof (uint))
			{
				_sizeOfT = sizeof (int);
			}
			else if (typeof (T) == typeof (short) || typeof (T) == typeof (ushort))
			{
				_sizeOfT = sizeof (short);
			}
			else if (typeof (T) == typeof (byte) || typeof (T) == typeof (sbyte))
			{
				_sizeOfT = sizeof (byte);
			}
			else if (typeof (T) == typeof (char))
			{
				_sizeOfT = sizeof (char);
			}
			else if (typeof (T) == typeof (float))
			{
				_sizeOfT = sizeof (float);
			}
			else if (typeof (T) == typeof (double))
			{
				_sizeOfT = sizeof (double);
			}
			else if (typeof (T) == typeof (decimal))
			{
				_sizeOfT = sizeof (decimal);
			}
			else
			{
				throw new ArgumentException("Generic type parameter T must be a primitive numeric type.");
			}

			// choose an appropriate buffer size for copying values of type T (i.e. must be a multiple of _sizeOfT)
			_bufferSize = 4096 + ((_sizeOfT - (4096%_sizeOfT))%_sizeOfT);
		}

		private static T ReadValue(Stream s)
		{
			var buffer = new byte[_sizeOfT];
			var values = new T[1];
			s.Read(buffer, 0, _sizeOfT);
			Buffer.BlockCopy(buffer, 0, values, 0, _sizeOfT);
			return values[0];
		}

		private static void WriteValue(Stream s, T value)
		{
			var buffer = new byte[_sizeOfT];
			Buffer.BlockCopy(new[] {value}, 0, buffer, 0, _sizeOfT);
			s.Write(buffer, 0, _sizeOfT);
		}

		#endregion
	}
}