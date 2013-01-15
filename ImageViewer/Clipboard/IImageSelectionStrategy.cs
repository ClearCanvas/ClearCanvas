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

using System.Collections.Generic;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Defines a strategy for image selection.
	/// </summary>
	/// <remarks>
	/// This interface can be used to implement different strategies for
	/// what subset of images in a display set should be sent to the clipboard.
	/// For example, you might have a "ImageRangeStrategy", which would accept
	/// a beginning and ending image.  When the clipboard framework calls
	/// <see cref="GetImages"/>, that range of images is returned.
	/// </remarks>
	public interface IImageSelectionStrategy
	{
		/// <summary>
		/// Gets a description of the image selection strategy.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the subset of <see cref="IPresentationImage"/>s from the specified
		/// <see cref="IDisplaySet"/> that will be sent to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <returns></returns>
		IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet);
	}
}
