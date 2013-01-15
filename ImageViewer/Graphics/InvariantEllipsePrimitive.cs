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
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An elliptical <see cref="InvariantPrimitive"/>.
	/// </summary>
	[Cloneable(true)]
    /// TODO (CR Oct 2011): Should these actually be serializable given that presentation states won't show them invariant.
    [DicomSerializableGraphicAnnotation(typeof(EllipseGraphicAnnotationSerializer))]
	public class InvariantEllipsePrimitive : InvariantBoundablePrimitive
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InvariantEllipsePrimitive"/>.
		/// </summary>
		public InvariantEllipsePrimitive()
		{

		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantEllipsePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantEllipsePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			bool result = EllipsePrimitive.HitTest(
				this.SpatialTransform.ConvertToSource(point), 
				this.Rectangle,
				this.SpatialTransform);
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point where the ellipse intersects the line whose end points
		/// are the center of the ellipse and the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			// Semi major/minor axes
			float a = this.Width/2;
			float b = this.Height/2;

			// Center of ellipse
			RectangleF rect = this.Rectangle;
			float x1 = rect.Left + a;
			float y1 = rect.Top + b;

			PointF center = new PointF(x1, y1);

			return EllipsePrimitive.IntersectEllipseAndLine(a, b, center, point);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool Contains(PointF point)
		{
		    /// TODO (CR Oct 2011): duplicated from EllipsePrimitive - should combine.
            if (CoordinateSystem == CoordinateSystem.Destination)
                point = SpatialTransform.ConvertToSource(point);

            CoordinateSystem = CoordinateSystem.Source;
            // Semi major/minor axes
            float a = Width / 2;
            float b = Height / 2;
            RectangleF rect = BoundingBox;
            ResetCoordinateSystem();

            // Center of ellipse
            float xCenter = rect.Left + a;
            float yCenter = rect.Top + b;

            float x = point.X - xCenter;
            float y = point.Y - yCenter;

		    return (x*x)/(a*a) + (y*y)/(b*b) <= 1;
        }
	}
}
