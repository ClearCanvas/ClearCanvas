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
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a pixel data prefetching strategy.
	/// </summary>
	public interface IPrefetchingStrategy
	{
		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the friendly description of the prefetching strategy.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Starts prefetching pixel data in the background for the images
		/// that have already been added into <paramref name="imageViewer"/>.
		/// </summary>
		void Start(IImageViewer imageViewer);

		/// <summary>
		/// Stops prefetching of pixel data in the background.
		/// </summary>
		/// <remarks>
		/// Implementers should ensure that all background threads have terminated
		/// before this method returns.
		/// </remarks>
		void Stop();
	}
}
