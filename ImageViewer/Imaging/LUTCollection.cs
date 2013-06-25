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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A collection of <see cref="IComposableLut"/> objects.
	/// </summary>
	public class LutCollection : ObservableList<IComposableLut>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		internal LutCollection()
		{
		}

		internal void Validate()
		{
			int count = 0;

			// Check for null LUTs
			foreach (IComposableLut lut in this)
			{
				++count;
				if (lut == null)
					throw new InvalidOperationException(SR.ExceptionLUTNotAdded);
			}

			if (count == 0) //do this instead of accessing Count b/c it can be expensive.
				throw new InvalidOperationException(SR.ExceptionLUTNotAdded);

			// Verify that the input range of the nth LUT is equal to the output
			// range of the n-1th LUT.
			for (int i = 1; i < count; i++)
			{
				IComposableLut curLut = this[i];
				IComposableLut prevLut = this[i - 1];

				if (!FloatComparer.AreEqual(prevLut.MinOutputValue, curLut.MinInputValue) ||
					!FloatComparer.AreEqual(prevLut.MaxOutputValue, curLut.MaxInputValue))
					throw new InvalidOperationException(SR.ExceptionLUTInputOutputRange);
			}
		}

		internal void SyncMinMaxValues()
		{
			int count = Count;
			for (int i = 1; i < count; ++i)
			{
				IComposableLut curLut = this[i];
				IComposableLut prevLut = this[i - 1];

				curLut.MinInputValue = prevLut.MinOutputValue;
				curLut.MaxInputValue = prevLut.MaxOutputValue;
			}
		}
	}
}
