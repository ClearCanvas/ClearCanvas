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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Base implementation of a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values whose data is purely generated.
	/// </summary>
	/// <remarks>
	/// Often, linear functions are created by deriving from this class to improve performance so that
	/// the calculation is only performed once.  For an example, see <see cref="ModalityLutLinear"/>.
	/// </remarks>
	/// <seealso cref="DataModalityLut"/>
	[Cloneable(true)]
	public abstract class GeneratedDataModalityLut : DataModalityLut, IModalityLut
	{
		[CloneIgnore]
		private double[] _data; // data will be re-generated.

		/// <summary>
		/// Since the data table is generated, simply returns <see cref="IModalityLut.MinInputValue"/>.
		/// </summary>
		public override sealed int FirstMappedPixelValue
		{
			get { return MinInputValue; }
		}

		/// <summary>
		/// Since the data table is generated, simply returns <see cref="IModalityLut.MaxInputValue"/>.
		/// </summary>
		public override sealed int LastMappedPixelValue
		{
			get { return MaxInputValue; }
		}

		/// <summary>
		/// Gets the lookup table data, lazily created.
		/// </summary>
		public override sealed double[] Data
		{
			get
			{
				if (_data == null)
					Fill(_data = MemoryManager.Allocate<double>(Length), FirstMappedPixelValue, LastMappedPixelValue);
				return _data;
			}
		}

		/// <summary>
		/// Looks up and returns a value at a particular index in the lookup table.
		/// </summary>
		public override sealed double this[int index]
		{
			get { return base[index]; }
			protected set { base[index] = value; }
		}

		double IModalityLut.this[int input]
		{
			get { return this[input]; }
		}

		double IComposableLut.this[double input]
		{
			get { return this[(int) Math.Round(input)]; }
		}

		/// <summary>
		/// Called to populate the lookup table using an algorithm.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The data table starts with the mapped output value for an input of <see cref="FirstMappedPixelValue"/> at index 0, and ends with the value for an input of <see cref="LastMappedPixelValue"/> for a total of (<see cref="LastMappedPixelValue"/> - <see cref="FirstMappedPixelValue"/> + 1) entries.
		/// </para>
		/// <para>
		/// Implementations can fill the lookup table using an iterator such as the following:
		/// <code><![CDATA[
		/// for (int i = 0; i < data.Length; ++i)
		///     data[i] = MapPixel(i + firstMappedPixelValue);
		/// ]]></code>
		/// </para>
		/// </remarks>
		/// <param name="data">The lookup table to be populated.</param>
		/// <param name="firstMappedPixelValue">The value of the first pixel in the lookup table (and hence at index 0). This value is provided for convenience, and has the same value as <see cref="FirstMappedPixelValue"/>.</param>
		/// <param name="lastMappedPixelValue">The value of the last pixel in the lookup table (and hence at index data.Length - 1). This value is provided for convenience, and has the same value as <see cref="LastMappedPixelValue"/>.</param>
		protected abstract void Fill(double[] data, int firstMappedPixelValue, int lastMappedPixelValue);

		/// <summary>
		/// Fires the <see cref="DataModalityLut.LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the lookup table has changed.
		/// </remarks>
		protected override void OnLutChanged()
		{
			Clear();
		}

		/// <summary>
		/// Clears the data in the lookup table; the data will be recreated on the next attempt to access the lookup table data.
		/// </summary>
		public void Clear()
		{
			_data = null;
		}
	}
}