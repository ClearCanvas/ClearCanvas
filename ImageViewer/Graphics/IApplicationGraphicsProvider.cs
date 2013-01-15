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

using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Graphics {
	/// <summary>
	/// Provides access to a <see cref="GraphicCollection"/> containing application-level graphics.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Application-level graphics are rendered after any domain-level graphics and before any
	/// user-level graphics.
	/// </para>
	/// <para>
	/// If you have subclassed <see cref="PresentationImage"/> and want to expose
	/// a <see cref="GraphicCollection"/> to <see cref="ImageViewerTool"/> objects, 
	/// do so by implementing this interface in your subclass.
	/// </para>
	/// <para>
	/// In general, avoid accessing members of subclasses of <see cref="PresentationImage"/>
	/// directly.  Prefer instead to use provider interfaces such as this one.  Doing
	/// so prevents <see cref="ImageViewerTool"/> objects from having to know about specific
	/// subclasses of <see cref="PresentationImage"/>, thus allowing them to work with
	/// any type of <see cref="PresentationImage"/> that implements the provider interface.
	/// </para>
	/// </remarks>
	/// <seealso cref="IOverlayGraphicsProvider"/>
	public interface IApplicationGraphicsProvider
	{
		/// <summary>
		/// Gets a <see cref="GraphicCollection"/> of application-level graphics.
		/// </summary>
		GraphicCollection ApplicationGraphics { get; }
	}
}
