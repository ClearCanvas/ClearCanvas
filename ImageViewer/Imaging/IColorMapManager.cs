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

using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: change how this manager/provider relationship works ... it just doesn't feel right.

	/// <summary>
	/// A Color Map Manager, which is responsible for managing installation and restoration
	/// of color maps via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementors can maintain the named color maps any way they choose.
	/// However, the <see cref="ColorMapFactoryExtensionPoint"/> is the preferred method of 
	/// creating new color maps.
	/// </para>
	/// <para>
	/// Implementors must not return null from the <see cref="IColorMapInstaller.ColorMap"/> property.
	/// </para>
	/// </remarks>
	public interface IColorMapManager : IColorMapInstaller, IMemorable
	{
		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		[Obsolete("Use the ColorMap property instead.")]
		IColorMap GetColorMap();
	}
}
