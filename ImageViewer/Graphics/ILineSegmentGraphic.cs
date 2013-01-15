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
	/// Defines an <see cref="IVectorGraphic"/> that can be described by two endpoints.
	/// </summary>
	public interface ILineSegmentGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets one endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Point1 { get; set; }

		/// <summary>
		/// Gets or sets the other endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Point2 { get; set; }

		/// <summary>
		/// Occurs when the <see cref="Point1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Point1Changed;

		/// <summary>
		/// Occurs when the <see cref="Point2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Point2Changed;
	}
}