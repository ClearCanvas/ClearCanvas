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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of a color map, accessed and manipulated via the <see cref="ColorMapManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ColorMapManager"/> property allows access to and manipulation of the installed color map.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="ColorMapManager"/> property.
	/// </para>
	/// </remarks>
	/// <seealso cref="IColorMapManager"/>
	public interface IColorMapProvider : IDrawable
	{
		/// <summary>
		/// Gets the <see cref="IColorMapManager"/> associated with the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IColorMapManager ColorMapManager { get; }
	}
}
