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
		private readonly int _length;
		private readonly int[] _data;

		public ComposedLut(LutCollection luts): this(luts, null)
		{
		}

		public ComposedLut(LutCollection luts, BufferCache<int> cache)
		{
			//luts.Validate();

			int lutCount;
			IComposableLut firstLut, lastLut;
			GetFirstAndLastLut(luts, out firstLut, out lastLut, out lutCount);

			_minInputValue = (int) Math.Round(firstLut.MinInputValue);
			_maxInputValue = (int) Math.Round(firstLut.MaxInputValue);
			_length = _maxInputValue - _minInputValue + 1;
			_data = cache != null ? cache.Allocate(_length) : MemoryManager.Allocate<int>(_length);

			//copy to array because accessing ObservableList's indexer in a tight loop is very expensive
			IComposableLut[] lutArray = new IComposableLut[lutCount];
			luts.CopyTo(lutArray, 0);

			unsafe
			{
				fixed (int* composedLutData = _data)
				{
					int* pLutData = composedLutData;
					int min = _minInputValue;
					int max = _maxInputValue + 1;

					for (int i = min; i < max; ++i)
					{
						double val = i;

						for (int j = 0; j < lutCount; ++j)
							val = lutArray[j][val];

						*pLutData = (int) Math.Round(val);
						++pLutData;
					}
				}
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
        
		//never guaranteed because the output could be colour.
		public int MinOutputValue
		{
			get { throw new InvalidOperationException("Composed Luts cannot have a MinOutputValue."); }
		}
		public int MaxOutputValue
		{
			get { throw new InvalidOperationException("Composed Luts cannot have a MaxOutputValue."); }
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