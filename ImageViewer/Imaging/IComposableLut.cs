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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a lookup table that comprises part of the standard grayscale image display pipeline.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	public interface IComposableLut : IMemorable
	{
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		double MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		double MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		double MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		double MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		double this[double input] { get; }

		/// <summary>
		/// Performs bulk lookup for a set of input values.
		/// </summary>
		/// <param name="input">Source array of input values to be transformed.</param>
		/// <param name="output">Destination array where transformed output values will be written (may be same array as <paramref name="input"/>).</param>
		/// <param name="count">Number of values in the arrays to transform (contiguous entries from start of array).</param>
		void LookupValues(double[] input, double[] output, int count);

		/// <summary>
		/// Fired when the lookup table has changed in some way.
		/// </summary>
		event EventHandler LutChanged;

		/// <summary>
		/// Gets a string key that identifies this particular lookup table's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some lookup tables can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the lookup table.
		/// </summary>
		string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IComposableLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		IComposableLut Clone();
	}
}