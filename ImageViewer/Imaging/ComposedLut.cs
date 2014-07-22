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
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public sealed class ComposedLut : IComposedLut
	{
		private readonly int _minInputValue;
		private readonly int _maxInputValue;
		private readonly int _minOutputValue;
		private readonly int _maxOutputValue;
		private readonly int _length;
		private readonly int[] _data;

		public ComposedLut(IComposableLut[] luts)
			: this(luts, null, null) {}

		public unsafe ComposedLut(IComposableLut[] luts, BufferCache<int> cache, BufferCache<double> doubleCache)
		{
			//luts.Validate();
			int lutCount;
			IComposableLut firstLut, lastLut;
			GetFirstAndLastLut(luts, out firstLut, out lastLut, out lutCount);

			_minInputValue = (int) Math.Round(firstLut.MinInputValue);
			_maxInputValue = (int) Math.Round(firstLut.MaxInputValue);
			_minOutputValue = (int) Math.Round(lastLut.MinOutputValue);
			_maxOutputValue = (int) Math.Round(lastLut.MaxOutputValue);

			_length = _maxInputValue - _minInputValue + 1;
			_data = cache != null ? cache.Allocate(_length) : MemoryManager.Allocate<int>(_length);

			const int intermediateDataSize = 8192; // each double entry is 8 bytes so the entire array is 64kB - just small enough to stay off the large object heap
			var intermediateData = doubleCache != null ? doubleCache.Allocate(intermediateDataSize) : MemoryManager.Allocate<double>(intermediateDataSize);
			try
			{
				fixed (double* intermediateLutData = intermediateData)
				fixed (int* composedLutData = _data)
				{
					var min = _minInputValue;
					var max = _maxInputValue + 1;
					var pComposed = composedLutData;

					// performs the bulk lookups in 64kB chunks (8k entries @ 8 bytes per) in order to keep the intermediate buffer off the large object heap
					for (var start = min; start < max; start += intermediateDataSize)
					{
						var stop = Math.Min(max, start + intermediateDataSize);
						var count = stop - start;

						var pIntermediate = intermediateLutData;
						for (var i = start; i < stop; ++i)
							*pIntermediate++ = i;

						for (var j = 0; j < lutCount; ++j)
							luts[j].LookupValues(intermediateData, intermediateData, count);

						pIntermediate = intermediateLutData;
						for (var i = 0; i < count; ++i)
							*pComposed++ = (int) Math.Round(*pIntermediate++);
					}
				}
			}
			finally
			{
				if (doubleCache != null)
					doubleCache.Return(intermediateData);
			}
		}

		#region IComposedLut Members

		public int[] Data
		{
			get { return _data; }
		}

		#endregion

		#region ILut Members

		public int MinInputValue
		{
			get { return _minInputValue; }
		}

		public int MaxInputValue
		{
			get { return _maxInputValue; }
		}

		public int MinOutputValue
		{
			get { return _minOutputValue; }
		}

		public int MaxOutputValue
		{
			get { return _maxOutputValue; }
		}

		public int this[int index]
		{
			get
			{
				if (index <= _minInputValue)
					return _data[0];
				else if (index >= _maxInputValue)
					return Data[_length - 1];

				return Data[index - _minInputValue];
			}
		}

		#endregion

		#region Private Static

		private static void GetFirstAndLastLut(IEnumerable<IComposableLut> luts, out IComposableLut firstLut, out IComposableLut lastLut, out int count)
		{
			count = 0;
			firstLut = lastLut = null;

			foreach (IComposableLut lut in luts)
			{
				if (firstLut == null)
					firstLut = lut;

				lastLut = lut;
				++count;
			}

			if (count == 0)
				throw new ArgumentException("There are no LUTs in the collection.");
		}

		#endregion
	}
}