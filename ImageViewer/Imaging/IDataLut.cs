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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines the properties and methods of a lookup table mapping input pixel values to output pixel values.
	/// </summary>
	public interface IDataLut
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the lookup table data.
		/// </summary>
		int[] Data { get; }

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lookup table at a given input index.
		/// </summary>
		int this[int index] { get; }

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
		/// Creates a deep-copy of the <see cref="IDataLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.
		/// </remarks>
		IDataLut Clone();
	}
}