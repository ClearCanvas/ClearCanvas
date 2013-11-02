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
	/// A strategy for automatically calculating the location of a <see cref="IAnnotationGraphic"/>'s callout.
	/// </summary>
	public interface IAnnotationCalloutLocationStrategy : IDisposable
	{
		/// <summary>
		/// Sets the <see cref="IAnnotationGraphic"/> that owns this strategy.
		/// </summary>
		void SetAnnotationGraphic(IAnnotationGraphic annotationGraphic);

		/// <summary>
		/// Called when the <see cref="IAnnotationGraphic"/>'s callout location has been changed externally; for example, by the user.
		/// </summary>
		void OnCalloutLocationChangedExternally();

		/// <summary>
		/// Called by the owning <see cref="IAnnotationGraphic"/> to get the callout's new location.
		/// </summary>
		/// <param name="location">The new location of the callout.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="location"/>.</param>
		/// <returns>True if the callout location needs to change, false otherwise.</returns>
		bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Called by the owning <see cref="AnnotationGraphic"/> to get the callout's end point.
		/// </summary>
		/// <param name="endPoint">The callout end point.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="endPoint"/>.</param>
		void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// <see cref="IAnnotationCalloutLocationStrategy"/>s should not return null from this method.
		/// </remarks>
		IAnnotationCalloutLocationStrategy Clone();
	}
}