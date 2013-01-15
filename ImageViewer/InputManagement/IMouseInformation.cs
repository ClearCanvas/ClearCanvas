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

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Used by the framework to relay information about the mouse to domain objects.
	/// </summary>
	public interface IMouseInformation
	{
		/// <summary>
		/// Gets the <see cref="ITile"/> the mouse is currently in.
		/// </summary>
		ITile Tile { get; }

		/// <summary>
		/// Gets the mouse's current location, in terms of the <see cref="ITile"/>'s client rectangle coordinates.
		/// </summary>
		Point Location { get; }

		/// <summary>
		/// Gets the currently depressed mouse button, if any.
		/// </summary>
		XMouseButtons ActiveButton { get; }

		/// <summary>
		/// Gets the current mouse button click count.
		/// </summary>
		uint ClickCount { get; }
	}
}
