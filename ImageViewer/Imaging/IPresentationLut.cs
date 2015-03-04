#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
	public interface IPresentationLut : IComposableLut
	{
		/// <summary>
		/// Gets or sets the minimum output value.
		/// </summary>
		/// <remarks>This is set internally by the framework.</remarks>
		new int MinOutputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum output value.
		/// </summary>
		/// <remarks>This is set internally by the framework.</remarks>
		new int MaxOutputValue { get; set; }

		/// <summary>
		/// Gets the output value for the given input value.
		/// </summary>
		new int this[double value] { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IPresentationLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		new IPresentationLut Clone();
	}
}