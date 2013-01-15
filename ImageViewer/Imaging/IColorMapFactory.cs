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

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An extension point for <see cref="IColorMapFactory"/>s.
	/// </summary>
	/// <seealso cref="IColorMapFactory"/>
	public sealed class ColorMapFactoryExtensionPoint : ExtensionPoint<IColorMapFactory>
	{
	}

	/// <summary>
	/// A factory for color maps.
	/// </summary>
	/// <seealso cref="ColorMapFactoryExtensionPoint"/>
	/// <seealso cref="ColorMap"/>
	public interface IColorMapFactory
	{
		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should <b>not</b> be a resource string, as it should be language-independent.
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Creates a color map.
		/// </summary>
		IColorMap Create();
	}
}
