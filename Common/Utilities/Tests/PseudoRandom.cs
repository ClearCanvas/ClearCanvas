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
using System.Runtime.InteropServices;

namespace ClearCanvas.Common.Utilities.Tests
{
	/// <summary>
	/// A pseudorandom number generator implementation based on a 32-bit linear feedback shift register with taps at bits 31, 21, 1 and 0.
	/// </summary>
	/// <remarks>
	/// Unlike <see cref="Random"/>, this implementation will produce deterministic, platform-agnostic results for a given seed.
	/// </remarks>
	public sealed class PseudoRandom
	{
		private int _currentValue;

		/// <summary>
		/// Initializes a new pseudorandom number sequence using the current time as the seed.
		/// </summary>
		public PseudoRandom()
			: this(Environment.TickCount) {}

		/// <summary>
		///  Initializes a new pseudorandom number sequence using the specified sequence seed.
		/// </summary>
		/// <param name="seed">A non-zero number to use as the sequence seed. Typically, this should be constant for any given test, in order to ensure reproducibility between runs.</param>
		public PseudoRandom(int seed)
		{
			_currentValue = seed != 0 ? seed : 1;
		}

		/// <summary>
		/// Gets the next 32-bit signed integer in the sequence.
		/// </summary>
		/// <returns>A pseudorandom 32-bit signed integer.</returns>
		public int Next()
		{
			// a 32-bit linear feedback shift register implementation - taps at 31, 21, 1, and 0 will produce a maximal length sequence
			var bit = ((_currentValue >> 31) ^ (_currentValue >> 21) ^ (_currentValue >> 1) ^ (_currentValue >> 0)) & 1;
			return _currentValue = ((_currentValue >> 1) & int.MaxValue) | (bit << 31);
		}

		/// <summary>
		/// Gets the next integer in the sequence rescaled to the specified range.
		/// </summary>
		/// <param name="minValue">The minimum possible value that this method should return.</param>
		/// <param name="maxValue">The maximum possible value that this method should return.</param>
		/// <returns>A pseudorandom 32-bit signed integer between <paramref name="minValue"/> and <paramref name="maxValue"/>, inclusive.</returns>
		public int Next(int minValue, int maxValue)
		{
			const string message = "minValue must be less than or equal to maxValue.";
			if (minValue > maxValue)
				throw new ArgumentOutOfRangeException("minValue", message);
			var range = (long) maxValue - minValue;
			return (int) (NextDouble()*range) + minValue;
		}

		/// <summary>
		/// Gets the next non-negative integer in the sequence rescaled to the specified range.
		/// </summary>
		/// <param name="maxValue">The maximum possible value that this method should return.</param>
		/// <returns>A pseudorandom 32-bit signed integer between 0 and <paramref name="maxValue"/>, inclusive.</returns>
		public int Next(int maxValue)
		{
			const string message = "maxValue must be greater than or equal to zero.";
			if (maxValue < 0)
				throw new ArgumentOutOfRangeException("maxValue", message);
			return (int) (NextDouble()*maxValue);
		}

		/// <summary>
		/// Fills the given buffer with random bytes from the sequence.
		/// </summary>
		/// <param name="buffer">The buffer to be filled with random bytes.</param>
		public void NextBytes(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			for (var n = 0; n < buffer.Length; n++)
				buffer[n] = (byte) (Next()%256);
		}

		/// <summary>
		/// Fills the given buffer with random bytes from the sequence.
		/// </summary>
		/// <param name="buffer">Pointer to the buffer to be filled with random bytes.</param>
		/// <param name="bufferSize">Size of the buffer in bytes.</param>
		public void NextBytes(IntPtr buffer, int bufferSize)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentNullException("buffer");
			if (bufferSize < 0)
				throw new ArgumentOutOfRangeException("bufferSize");

			var byteBuffer = new byte[bufferSize];
			NextBytes(byteBuffer);
			Marshal.Copy(byteBuffer, 0, buffer, bufferSize);
		}

		/// <summary>
		/// Gets the next 64-bit floating-point number in the sequence.
		/// </summary>
		/// <returns>A pseudorandom 64-bit floating-point number.</returns>
		public double NextDouble()
		{
			return ((uint) Next())/(double) (1L + uint.MaxValue);
		}
	}
}

#endif