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
using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.Dicom.IO
{
	public class ByteArrayChunk
	{
		public byte[] Array { get; set; }
		public int Index { get; set; }
		public int Count { get; set; }
	}

	internal class ChunkStream : Stream
	{
		#region Private Members
		private long _position;
		private long _length;
		private int _current;
		private int _offset;
		private readonly List<ByteArrayChunk> _chunks;
		#endregion

		#region Public Constructors
		public ChunkStream()
		{
			_current = 0;
			_offset = 0;
			_position = 0;
			_length = 0;
			_chunks = new List<ByteArrayChunk>();
		}
		#endregion

		#region Public Members
		public void AddChunk(byte[] chunk)
		{
			_chunks.Add(new ByteArrayChunk { Array = chunk, Count = chunk.Length, Index = 0 });
			_length += chunk.Length;
		}

		public void AddChunk(ByteArrayChunk chunk)
		{
			_chunks.Add(chunk);
			_length += chunk.Count;
		}

		public void Clear()
		{
			_current = 0;
			_offset = 0;
			_position = 0;
			_length = 0;
			_chunks.Clear();
		}
		#endregion

		#region Stream Members
		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { return _length; }
		}

		public override long Position
		{
			get { return _position; }
			set { Seek(value, SeekOrigin.Begin); }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0, dstOffset = 0;
			for (int i = _current; i < _chunks.Count; i++)
			{
				ByteArrayChunk chunk = _chunks[i];
				int bytesInChunk = chunk.Count - _offset;

				if (bytesInChunk > count)
				{
					Array.Copy(chunk.Array, chunk.Index + _offset, buffer, dstOffset, count);
					read += count;
					_offset += count;
					_position += count;
					return read;
				}

				Array.Copy(chunk.Array, chunk.Index + _offset, buffer, dstOffset, bytesInChunk);

				read += bytesInChunk;
				count -= bytesInChunk;
				dstOffset += bytesInChunk;
				_position += bytesInChunk;

				// Allow garbage collection for the chunk, its already been read
				_chunks[_current] = null;

				_current++;
				_offset = 0;
			}
			return read;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
			{
				_current = 0;
				_offset = 0;
				_position = 0;
			}
			if (origin == SeekOrigin.End)
			{
				_current = _chunks.Count - 1;
				_offset = _chunks[_current].Count - 1;
				_position = _length - 1;
			}

			_position += offset;
			if (_position < 0)
				_position = 0;
			else if (_position >= _length)
				_position = _length - 1;

			_current = 0;
			_offset = 0;
			long remain = _position;

			for (int i = 0; i < _chunks.Count; i++)
			{
				ByteArrayChunk chunk = _chunks[i];
				if (chunk == null)
					throw new NotSupportedException("Seek not supported at this time");
				if (remain > chunk.Count)
				{
					remain -= chunk.Count;
				}
				else
				{
					_offset = (int)remain;
					return _position;
				}
				_current++;
			}

			_position -= remain;
			return _position;
		}

		public override void SetLength(long value)
		{
			throw new Exception("Unable to set length of unwritable stream!");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new Exception("Stream not writable!");
		}
		#endregion
	}
}
