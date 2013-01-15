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
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	public interface IBoundableGraphic : IVectorGraphic 
	{
		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> TopLeftChanged;

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> BottomRightChanged;

		/// <summary>
		/// Gets or sets the top left corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF TopLeft { get; set; }

		/// <summary>
		/// Gets or sets the bottom right corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF BottomRight { get; set; }

		//TODO (CR Sept 2010): not sure about the name.

		/// <summary>
		/// Gets the bounding rectangle of the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property gives the orientation-sensitive rectangle that bounds the graphic, whereas
		/// the <see cref="IGraphic.BoundingBox"/> property gives the normalized rectangle with positive
		/// width and height.
		/// </para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		/// <seealso cref="IGraphic.BoundingBox"/>
		RectangleF Rectangle { get; }

		/// <summary>
		/// Gets the width of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Width { get; }

		/// <summary>
		/// Gets the height of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Height { get; }

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		bool Contains(PointF point);
	}
}