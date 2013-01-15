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

// define thie flag to perform unit tests against a small scale (and thus useless for production) implementation of the blocking strategy
//#define SMALL_SCALE_TEST

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// An implementation of <see cref="Stream"/> whose backing store is memory but is optimized to store large streams
	/// of data in separate, smaller buffers, giving a total capacity of nearly 2^47 bytes.
	/// </summary>
	/// <remarks>
	/// This implementation is similar to <see cref="MemoryStream"/> in that it stores all data in memory.
	/// However, <see cref="MemoryStream"/> stores data in a single byte buffer, and is thus subject to the 2^31-1 maximum
	/// array length of the CLR, as well as potential <see cref="OutOfMemoryException"/>s when allocating many large
	/// streams due to the fragmentation of the large object heap.
	/// <para>
	/// <see cref="LargeMemoryStream"/> is optimized for larger streams of data by storing the data in a series of smaller
	/// buffers limited to 64KB in size, thus increasing the apparent stream length limit by nearly a factor of 2^16. The
	/// smaller buffers are allocated on the small object heap, where fragmentation is less of a concern due to the ability
	/// of the garbage collector to perform memory compaction. Furthermore, the first few buffers are of smaller size, trading
	/// off some maximum stream length capacity in order to reduce the overhead for small data streams 
	/// e.g. a stream of 256 bytes will take up only 1KB instead of a full 64KB)
	/// </para>
	/// </remarks>
	public class LargeMemoryStream : Stream
	{
		private List<byte[]> _blocks;
		private long _position;
		private long _length;
		private bool _closed;

		public LargeMemoryStream()
		{
			_blocks = new List<byte[]>();
			_position = 0;
			_length = 0;
			_closed = false;
		}

		public LargeMemoryStream(byte[] data)
			: this()
		{
			if (data != null && data.Length > 0)
			{
				// compute requested block position
				int lastBlockIndex, lastBlockOffset;
				ComputeBlockPosition(data.Length - 1, out lastBlockIndex, out lastBlockOffset);

				// loop for each of the requested blocks
				var bufferOffset = 0;
				for (var n = 0; n <= lastBlockIndex; ++n)
				{
					// get the requested block
					var block = AllocateBuffer(GetBlockSize(n));
					_blocks.Add(block);

					// compute the range of bytes that can be written to this block
					var range = n == lastBlockIndex ? lastBlockOffset + 1 : block.Length;

					// copy the range of bytes from the buffer into the requested block
					Buffer.BlockCopy(data, bufferOffset, block, 0, range);

					// increment the buffer offset
					bufferOffset += range;
				}

				// set the stream length
				_length = data.Length;
			}
		}

		protected override void Dispose(bool disposing)
		{
			_closed = true;

			if (disposing && _blocks != null)
			{
				_blocks.Clear();
				_blocks = null;
			}
		}

		public override bool CanRead
		{
			get { return !_closed; }
		}

		public override bool CanSeek
		{
			get { return !_closed; }
		}

		public override bool CanWrite
		{
			get { return !_closed; }
		}

		public override long Length
		{
			get
			{
				AssertIsStreamOpen();
				return _length;
			}
		}

		public override long Position
		{
			get
			{
				AssertIsStreamOpen();
				return _position;
			}
			set { Seek(value, SeekOrigin.Begin); }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			AssertIsStreamOpen();

			long newPosition;
			switch (origin)
			{
				case SeekOrigin.Current:
					newPosition = _position + offset;
					break;
				case SeekOrigin.End:
					newPosition = _length + offset;
					break;
				case SeekOrigin.Begin:
				default:
					newPosition = offset;
					break;
			}

			// assert that the new position is valid (according to the semantics of the Stream base class, you can move beyond the end of stream, but not before the start of it)
			if (newPosition < 0)
				throw new IOException("An attempt was made to move the position before the beginning of the stream.");

			// assert that the new position does not exceed the maximum length of the stream
			if (newPosition > MaxLength)
				throw new IOException("An attempt was made to move the position after the maximum length of the stream.");

			return _position = newPosition;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			AssertIsStreamOpen();
			const string messageNonNegative = "Non-negative number required.";
			const string messageBounds = "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.";

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", messageNonNegative);

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", messageNonNegative);

			if (offset + count > buffer.Length)
				throw new ArgumentException(messageBounds);

			// if current position is beyond end of stream, return immediately
			if (_position >= _length)
				return 0;

			// if requested byte count exceeds length, adjust requested byte count
			if (_position + count > _length)
				count = (int) (_length - _position);

			// if no bytes to read, return immediately
			if (count == 0)
				return 0;

			ReadCore(_position, buffer, offset, count);

			// increment position and return bytes read
			_position += count;
			return count;
		}

		private void ReadCore(long position, byte[] buffer, int offset, int count)
		{
			// if no bytes to read, return immediately
			if (count == 0) return;

			// compute requested block position
			int blockIndex, blockOffset, lastBlockIndex, lastBlockOffset;
			ComputeBlockPosition(position, out blockIndex, out blockOffset);
			ComputeBlockPosition(position + count - 1, out lastBlockIndex, out lastBlockOffset);

			// if blocks capacity insufficient, adjust last block to match (should have been caught by the count adjustment)
			if (_blocks.Count <= lastBlockIndex)
				lastBlockIndex = _blocks.Count - 1;

			// loop for each of the requested blocks
			var bufferOffset = offset;
			while (blockIndex <= lastBlockIndex)
			{
				// get the requested block
				var block = _blocks[blockIndex];
				var blockSize = block != null ? block.Length : GetBlockSize(blockIndex);

				// compute the range of bytes that can be read from this block
				var range = blockIndex == lastBlockIndex ? lastBlockOffset + 1 - blockOffset : blockSize - blockOffset;

				// read the range of bytes from the requested block into the buffer
				if (block != null)
				{
					// copy a range of bytes from the requested block into the buffer
					Buffer.BlockCopy(block, blockOffset, buffer, bufferOffset, range);
				}
				else
				{
					// requested block is actually a zero placeholder, simply zero the corresponding bytes in the buffer
					Array.Clear(buffer, bufferOffset, range);
				}

				// move to next block, reset the block offset, and increment the buffer offset
				++blockIndex;
				blockOffset = 0;
				bufferOffset += range;
			}
		}

		public override int ReadByte()
		{
			AssertIsStreamOpen();

			// if current position is beyond end of stream, signal EOS
			if (_position >= _length)
				return -1;

			// compute requested block position
			int blockIndex, blockOffset;
			ComputeBlockPosition(_position, out blockIndex, out blockOffset);

			// if blocks capacity insufficient, signal EOS (should have been caught by the position check)
			if (_blocks.Count <= blockIndex)
				return -1;

			// get the requested block
			var block = _blocks[blockIndex];

			// read the requested byte and increment position
			var result = block != null ? block[blockOffset] : 0; // if requested block is a zero placeholder, all bytes are zero
			++_position;
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			AssertIsStreamOpen();
			const string messageNonNegative = "Non-negative number required.";
			const string messageBounds = "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the destination collection.";

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", messageNonNegative);

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", messageNonNegative);

			if (offset + count > buffer.Length)
				throw new ArgumentException(messageBounds);

			// if no bytes to write, return immediately
			if (count == 0)
				return;

			// compute requested block position
			int blockIndex, blockOffset, lastBlockIndex, lastBlockOffset;
			ComputeBlockPosition(_position, out blockIndex, out blockOffset);
			ComputeBlockPosition(_position + count - 1, out lastBlockIndex, out lastBlockOffset);

			// expand blocks capacity as necessary
			while (_blocks.Count <= lastBlockIndex)
				_blocks.Add(null);

			// loop for each of the requested blocks
			var bufferOffset = offset;
			while (blockIndex <= lastBlockIndex)
			{
				// get the requested block
				var block = _blocks[blockIndex];

				// if requested block is a zero placeholder, allocate the block now
				if (block == null) _blocks[blockIndex] = block = AllocateBuffer(GetBlockSize(blockIndex));

				// compute the range of bytes that can be written to this block
				var range = blockIndex == lastBlockIndex ? lastBlockOffset + 1 - blockOffset : block.Length - blockOffset;

				// copy the range of bytes from the buffer into the requested block
				Buffer.BlockCopy(buffer, bufferOffset, block, blockOffset, range);

				// move to next block, reset the block offset, and increment the buffer offset
				++blockIndex;
				blockOffset = 0;
				bufferOffset += range;
			}

			// increment position
			_position += count;

			// if position is beyond end of stream, increase length to accomodate
			if (_position >= _length)
				_length = _position;
		}

		public override void WriteByte(byte value)
		{
			AssertIsStreamOpen();

			// compute requested block position
			int blockIndex, blockOffset;
			ComputeBlockPosition(_position, out blockIndex, out blockOffset);

			// expand blocks capacity as necessary
			while (_blocks.Count <= blockIndex)
				_blocks.Add(null);

			// get the requested block
			var block = _blocks[blockIndex];

			// if requested block is a zero placeholder, allocate the block now
			if (block == null) _blocks[blockIndex] = block = AllocateBuffer(GetBlockSize(blockIndex));

			// write the requested byte and increment position
			block[blockOffset] = value;
			++_position;

			// if position is beyond end of stream, increase length to accomodate
			if (_position >= _length)
				_length = _position;
		}

		public override void SetLength(long value)
		{
			AssertIsStreamOpen();
			const string message = "Stream length must be non-negative and less than 2^47 - 2^18.";

			if (value < 0 || value > MaxLength)
				throw new ArgumentOutOfRangeException("value", message);

			// compute requested last block position
			int blockIndex, blockOffset;
			ComputeBlockPosition(value, out blockIndex, out blockOffset);

			// expand or reduce blocks capacity as necessary
			if (_blocks.Count <= blockIndex)
			{
				while (_blocks.Count <= blockIndex)
					_blocks.Add(null);
			}
			else if (_blocks.Count > blockIndex + 1)
			{
				_blocks.RemoveRange(blockIndex + 1, _blocks.Count - blockIndex - 1);
			}

			// set the end of stream
			_length = value;
		}

		public override void Flush()
		{
			// no backing store, so nothing to flush
		}

		public virtual void WriteTo(Stream stream)
		{
			AssertIsStreamOpen();

			if (stream == null)
				throw new ArgumentNullException("stream");

			// compute requested block position
			int lastBlockIndex, lastBlockOffset;
			ComputeBlockPosition(_length - 1, out lastBlockIndex, out lastBlockOffset);

			byte[] zeroBlock = null;
			for (var n = 0; n <= lastBlockIndex; ++n)
			{
				var block = n < _blocks.Count ? _blocks[n] : null;
				var blockSize = block != null ? block.Length : GetBlockSize(n);

				// compute the range of bytes that can be read from this block
				var range = n == lastBlockIndex ? lastBlockOffset + 1 : blockSize;

				if (block == null)
				{
					if (zeroBlock == null || zeroBlock.Length != blockSize)
						zeroBlock = AllocateBuffer(blockSize);
					block = zeroBlock;
				}
				stream.Write(block, 0, range);
			}
		}

		public virtual byte[] ToArray()
		{
			AssertIsStreamOpen();

			var array = AllocateBuffer(_length);
			ReadCore(0, array, 0, array.Length);
			return array;
		}

		private void AssertIsStreamOpen()
		{
			if (_closed)
				throw new ObjectDisposedException(null, "Cannot access a closed Stream.");
		}

		private static byte[] AllocateBuffer(long size)
		{
			try
			{
				return new byte[size];
			}
			catch (OutOfMemoryException)
			{
				// if we couldn't even allocate a block buffer, make last ditch attempt to force GC collection
				GC.Collect();
			}
			return new byte[size];
		}

		#region Unit Test Support

#if UNIT_TESTS

		internal int BlockCount
		{
			get { return _blocks.Count(b => b != null); }
		}

		internal int BlockCapacity
		{
			get { return _blocks.Count; }
		}

		internal static int TestGetBlockIndex(long position)
		{
			int index, offset;
			ComputeBlockPosition(position, out index, out offset);
			return index;
		}

		internal static int TestGetBlockOffset(long position)
		{
			int index, offset;
			ComputeBlockPosition(position, out index, out offset);
			return offset;
		}

		internal static int TestGetBlockSize(int blockIndex)
		{
			return GetBlockSize(blockIndex);
		}

#endif

		#endregion

		#region Block Sizing Strategy

#if !SMALL_SCALE_TEST
		private const int _block0AddressSize = 10; // 1st page goes up to 2^10 = 1024
		private const int _block1AddressSize = 12; // 2nd page goes up to 2^12 = 4096
		private const int _block2AddressSize = 14; // 3rd page goes up to 2^14 = 16384
		private const int _block3AddressSize = 16; // 4th page goes up to 2^16 = 65536
#else
		private const int _block0AddressSize = 1; // 1st page goes up to 2^1 = 2
		private const int _block1AddressSize = 2; // 2nd page goes up to 2^2 = 4
		private const int _block2AddressSize = 3; // 3rd page goes up to 2^3 = 8
		private const int _block3AddressSize = 4; // 4th page goes up to 2^4 = 16
#endif

		/// <summary>
		/// The maximum length of a <see cref="LargeMemoryStream"/> instance is 2^47-2^18 = 140,737,488,093,184 bytes.
		/// </summary>
		/// <remarks>
		/// The first 4 blocks have a total capacity of 2^16 bytes. Subsequent blocks have a capacity of 2^16 bytes each. The total number of blocks is limited to 2^31-1.
		/// </remarks>
		public const long MaxLength = ((long) int.MaxValue - 4 + 1)*(1 << _block3AddressSize);

		/// <summary>
		/// Computes the index and offset within the block for the given stream position.
		/// </summary>
		private static void ComputeBlockPosition(long position, out int blockIndex, out int blockOffset)
		{
			const int block0Size = 1 << _block0AddressSize;
			const int block1Size = 1 << _block1AddressSize;
			const int block2Size = 1 << _block2AddressSize;
			const int block3Size = 1 << _block3AddressSize;
			const int blockNMask = block3Size - 1;

			if ((position >> _block3AddressSize) > 0)
			{
				position = position - block3Size;
				blockIndex = 4 + (int) (position >> _block3AddressSize);
				blockOffset = (int) (position & blockNMask);
			}
			else if ((position >> _block2AddressSize) > 0)
			{
				blockIndex = 3;
				blockOffset = (int) (position - block2Size);
			}
			else if ((position >> _block1AddressSize) > 0)
			{
				blockIndex = 2;
				blockOffset = (int) (position - block1Size);
			}
			else if ((position >> _block0AddressSize) > 0)
			{
				blockIndex = 1;
				blockOffset = (int) (position - block0Size);
			}
			else
			{
				blockIndex = 0;
				blockOffset = (int) position;
			}
		}

		/// <summary>
		/// Gets the size in bytes of the specified block.
		/// </summary>
		private static int GetBlockSize(int blockIndex)
		{
			const int block0Size = 1 << _block0AddressSize;
			const int block1Size = 1 << _block1AddressSize;
			const int block2Size = 1 << _block2AddressSize;
			const int block3Size = 1 << _block3AddressSize;

			switch (blockIndex)
			{
				case 0:
					return block0Size;
				case 1:
					return block1Size - block0Size;
				case 2:
					return block2Size - block1Size;
				case 3:
					return block3Size - block2Size;
				default:
					return block3Size;
			}
		}

		#endregion
	}
}