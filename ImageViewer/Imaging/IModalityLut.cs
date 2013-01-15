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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	public interface IModalityLut : IComposableLut
	{
		/// <summary>
		/// Gets or sets the minimum input stored pixel value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		new int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input stored pixel value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		new int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input stored pixel value.
		/// </summary>
		double this[int input] { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IModalityLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		new IModalityLut Clone();
	}
}