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

using System.ComponentModel;
using System;
namespace ClearCanvas.Desktop
{
    /// <summary>
	/// An item for display in a gallery-style view.
	/// </summary>
	public interface IGalleryItem : INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// The image/icon to display.
		/// </summary>
		object Image { get; }

		/// <summary>
		/// The name of the object.
		/// </summary>
        string Name { get; set; }

		/// <summary>
		/// A brief description of the object.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// The actual object that is being visually represented in the gallery.
		/// </summary>
		object Item { get; }
	}
}
