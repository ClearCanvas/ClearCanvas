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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// An interface for objects that handle mouse wheel input.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The framework will look for this interface on <see cref="ClearCanvas.Desktop.Tools.ITool"/>s belonging to the current
	/// <see cref="IImageViewer"/> (via <see cref="IViewerShortcutManager.GetMouseWheelHandler"/>) and if an appropriate one 
	/// is found it will be given capture until a short period of time has expired.
	/// </para>
	/// </remarks>
	/// <seealso cref="IImageViewer"/>
	/// <seealso cref="ImageViewerComponent"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="TileController"/>
	public interface IMouseWheelHandler
	{
		/// <summary>
		/// Called by the framework when mouse wheel input has started.
		/// </summary>
		void StartWheel();

		/// <summary>
		/// Called by the framework each time the mouse wheel is moved.
		/// </summary>
		void Wheel(int wheelDelta);

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		void StopWheel();
	}
}
