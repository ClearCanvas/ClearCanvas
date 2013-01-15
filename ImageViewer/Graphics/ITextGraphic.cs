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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that contains some dynamic, formattable text content.
	/// </summary>
	public interface ITextGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		string Text { get; set; }

		/// <summary>
		/// Gets or sets the size in points.
		/// </summary>
		/// <remarks>
		/// Default value is 10 points.
		/// </remarks>
		float SizeInPoints { get; set; }

		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <remarks>
		/// Default value is "Arial".
		/// </remarks>
		string Font { get; set; }

		/// <summary>
		/// Gets the dimensions of the smallest rectangle that bounds the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		SizeF Dimensions { get; }

		/// <summary>
		/// Gets or sets the location of the center of the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF Location { get; set; }
	}
}