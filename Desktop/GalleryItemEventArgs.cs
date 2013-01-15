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

namespace ClearCanvas.Desktop {
	/// <summary>
	/// Represents the method that handles an event involving a single <see cref="IGalleryItem"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="GalleryItemEventArgs"/> that contains the event data.</param>
	public delegate void GalleryItemEventHandler(object sender, GalleryItemEventArgs e);

	/// <summary>
	/// Provides data for an event involving a single <see cref="IGalleryItem"/>.
	/// </summary>
	public class GalleryItemEventArgs : EventArgs {
		private readonly IGalleryItem _item;
		
		/// <summary>
		/// Constructs a new <see cref="GalleryItemEventArgs"/>.
		/// </summary>
		/// <param name="item">The <see cref="IGalleryItem"/>.</param>
		public GalleryItemEventArgs(IGalleryItem item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets the <see cref="IGalleryItem"/>.
		/// </summary>
		public IGalleryItem Item
		{
			get { return _item; }
		}
	}
}
