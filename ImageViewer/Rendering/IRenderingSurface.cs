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

// ReSharper disable InconsistentNaming

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Defines a rendering surface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Unless you are implementing your own renderering surface, you should never
	/// have to interact with this interface.  The two properties on 
	/// <see cref="IRenderingSurface"/> should only ever have to be set by the 
	/// Framework.
	/// </para>
	/// </remarks>
	public interface IRenderingSurface : IDisposable
	{
		/// <summary>
		/// Gets the type of rendering surface.
		/// </summary>
		/// <remarks>
		/// Advanced <see cref="IRenderer"/> implementations may need to provide different implementations of <see cref="IRenderingSurface"/>
		/// depending on whether the intended rendering destination is an onscreen window or an offscreen bitmap.
		/// For example, the onscreen surface may use a dynamic level-of-detail rendering in order to provide an update rate suitable for user interactivity.
		/// In contrast, the offscreen surface would always render at the highest level-of-detail in order to export the image to a memory bitmap.
		/// </remarks>
		RenderingSurfaceType Type { get; }

		/// <summary>
		/// Gets or sets the window ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the window handle, or "hwnd" 
		/// of the WinForms control you will eventually render to.  This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr WindowID { get; set; }

		/// <summary>
		/// Gets or sets the context ID.
		/// </summary>
		/// <remarks>
		/// On Windows systems, this is the device context handle, or "hdc"
		/// of the WinForms control you will eventually render to. This
		/// property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		IntPtr ContextID { get; set; }

		/// <summary>
		/// Gets or sets the rectangle of the surface.
		/// </summary>
		/// <remarks>
		/// This is the rectangle of the view onto the <see cref="ITile"/>.
		/// The top-left corner is always (0,0).  This rectangle changes as the
		/// view (i.e., the hosting window) changes size. Implementor should be
		/// aware that the rectangle can have a width or height of 0, and handle
		/// that boundary case appropriately.
		/// </remarks>
		Rectangle ClientRectangle { get; set; }

		/// <summary>
		/// Gets or sets the rectangle that requires repainting.
		/// </summary>
		/// <remarks>
		/// The implementer of <see cref="IRenderer"/> should use this rectangle
		/// to intelligently perform the <see cref="DrawMode.Refresh"/> operation.
		/// This property is set by the Framework; you should never have to
		/// set this property yourself.
		/// </remarks>
		Rectangle ClipRectangle { get; set; }

		/// <summary>
		/// Fired to notify that the contents of the surface have updated independently of the contents of the <see cref="ITile"/>.
		/// </summary>
		event EventHandler Invalidated;
	}

	/// <summary>
	/// Describes the type of <see cref="IRenderingSurface"/>.
	/// </summary>
	public enum RenderingSurfaceType
	{
		/// <summary>
		/// Indicates that the <see cref="IRenderingSurface"/> is suitable for onscreen use, and may have behaviours that only function correctly onscreen.
		/// </summary>
		/// <remarks>
		/// Advanced <see cref="IRenderer"/> implementations may need to provide different implementations of <see cref="IRenderingSurface"/>
		/// depending on whether the intended rendering destination is an onscreen window or an offscreen bitmap.
		/// For example, the onscreen surface may use a dynamic level-of-detail rendering in order to provide an update rate suitable for user interactivity.
		/// In contrast, the offscreen surface would always render at the highest level-of-detail in order to export the image to a memory bitmap.
		/// </remarks>
		Onscreen,

		/// <summary>
		/// Indicates that the <see cref="IRenderingSurface"/> is suitable for general purpose (offscreen) use.
		/// </summary>
		/// <remarks>
		/// Advanced <see cref="IRenderer"/> implementations may need to provide different implementations of <see cref="IRenderingSurface"/>
		/// depending on whether the intended rendering destination is an onscreen window or an offscreen bitmap.
		/// For example, the onscreen surface may use a dynamic level-of-detail rendering in order to provide an update rate suitable for user interactivity.
		/// In contrast, the offscreen surface would always render at the highest level-of-detail in order to export the image to a memory bitmap.
		/// </remarks>
		Offscreen
	}
}

// ReSharper restore InconsistentNaming