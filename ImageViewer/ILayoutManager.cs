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

namespace ClearCanvas.ImageViewer 
{
	/// <summary>
	/// Defines an interface for image layout management.
	/// </summary>
	/// <remarks>
	/// If you want to implement your own hanging protocol engine,
	/// you need to 1) implement this interface and 2) mark your class
	/// with the <code>[ExtensionOf(typeof(LayoutManagerExtensionPoint))]</code>
	/// attribute.
	/// </remarks>
	public interface ILayoutManager : IDisposable
	{
		/// <summary>
		/// Sets the owning <see cref="IImageViewer"/>.
		/// </summary>
		/// <param name="imageViewer"></param>
		void SetImageViewer(IImageViewer imageViewer);

		/// <summary>
		/// Lays out the images on the image viewer specified by <see cref="SetImageViewer"/>.
		/// </summary>
		/// <remarks>
		/// This is invoked by the <see cref="ImageViewerComponent"/> when images are
		/// first displayed, or anytime when <see cref="IImageViewer.Layout"/> is called.
		/// </remarks>
		void Layout();
	}
}