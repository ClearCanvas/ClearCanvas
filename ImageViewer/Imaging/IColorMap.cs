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
	/// Defines the properties and methods to access a color map that defines the mapping of single-channel input pixel values to ARGB color values.
	/// </summary>
	public interface IColorMap : IMemorable
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the color map data as a lookup table.
		/// </summary>
		int[] Data { get; }

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the output ARGB color at a given input index.
		/// </summary>
		int this[int index] { get; }

		/// <summary>
		/// Fired when the color map has changed in some way.
		/// </summary>
		event EventHandler LutChanged;

		/// <summary>
		/// Gets a string key that identifies this particular color map's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some color maps can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the color map.
		/// </summary>
		string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IColorMap"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.	
		/// </remarks>
		IColorMap Clone();
	}
}