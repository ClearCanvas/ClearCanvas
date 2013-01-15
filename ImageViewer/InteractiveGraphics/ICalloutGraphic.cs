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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that consists of some text content associated with a particular anchor point.
	/// </summary>
	public interface ICalloutGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets the callout text.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Gets or sets the font size in points used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font size is 10 points.
		/// </remarks>
		float FontSize { get; set; }

		/// <summary>
		/// Gets or sets the font name used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font is Arial.
		/// </remarks>
		string FontName { get; set; }

		/// <summary>
		/// Gets the bounding rectangle of the text portion of the callout.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		RectangleF TextBoundingBox { get; }

		/// <summary>
		/// Gets or sets the location of the center of the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF TextLocation { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="TextLocation"/> property changes.
		/// </summary>
		event EventHandler TextLocationChanged;

		/// <summary>
		/// Gets or sets the point where the callout is anchored.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF AnchorPoint { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="AnchorPoint"/> property changes.
		/// </summary>
		event EventHandler AnchorPointChanged;
	}
}