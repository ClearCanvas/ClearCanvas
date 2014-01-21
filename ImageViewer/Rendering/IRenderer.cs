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

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines an <see cref="IPresentationImage"/> renderer.
	/// </summary>
	/// <remarks>
	/// Unless you are implementing your own renderer, you should never
	/// have to interact with this interface.  The two methods on <see cref="IRenderer"/>
	/// should only ever have to be called by the Framework, and thus
	/// should be treated as internal.
	/// </remarks>
	public interface IRenderer : IDisposable
	{
		/// <summary>
		/// Creates an instance of <see cref="IRenderingSurface"/> suitable for this renderer.
		/// </summary>
		/// <param name="windowId">The window ID.  On Windows systems, this is the window handle
		/// or "HWND".</param>
		/// <param name="width">The width of the surface.</param>
		/// <param name="height">The height of the surface.</param>
		/// <param name="type">A value indicating the type of rendering surface requested.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method is called by <b>TileControl</b> (i.e., the <see cref="ITile"/> view)
		/// whenever it is resized, which includes when the control is first created.
		/// Once <b>TileControl</b> has obtained the surface, it just holds onto it.
		/// Your implementation of <see cref="Draw"/> should just render to the
		/// that same surface (passed in via the <see cref="DrawArgs"/>) irrespective 
		/// of the <see cref="IPresentationImage"/> being rendered.
		/// </remarks>
		IRenderingSurface CreateRenderingSurface(IntPtr windowId, int width, int height, RenderingSurfaceType type);

		/// <summary>
		/// Renders the specified scene graph to the graphics surface.
		/// </summary>
		/// <remarks>
		/// Calling code should take care to handle any exceptions in a manner suitable to the context of
		/// the rendering operation. For example, the view control for an
		/// <see cref="ITile"/> may wish to display the error message in the tile's client area <i>without
		/// crashing the control</i>, whereas an image export routine may wish to notify the user via an error
		/// dialog and have the export output <i>fail to be created</i>. Automated routines (such as unit
		/// tests) may even wish that the exception bubble all the way to the top for debugging purposes.
		/// </remarks>
		/// <param name="args">A <see cref="DrawArgs"/> object that specifies the graphics surface and the scene graph to be rendered.</param>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered in the rendering pipeline.</exception>
		void Draw(DrawArgs args);
	}
}