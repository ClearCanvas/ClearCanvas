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
	/// A provider of a <see cref="CursorToken"/> that is returned based on the current mouse position within an <see cref="ITile"/>.
	/// </summary>
	/// <remarks>
	/// The framework will look for this interface on graphic objects (<see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>) 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (see <see cref="PresentationImage.SceneGraph"/>) when the
	/// mouse has moved within the current <see cref="ITile"/>.  If the object returns a <see cref="CursorToken"/>, then the
	/// corresponding cursor will be shown at the current mouse position.
	/// </remarks>
	/// <seealso cref="CursorToken"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>
	/// <seealso cref="ClearCanvas.ImageViewer.Graphics.Graphic"/>
	public interface ICursorTokenProvider
	{
		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		CursorToken GetCursorToken(Point point);
	}
}
