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
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive rectangle graphic.
	/// </summary>
	[Cloneable(true)]
	[DicomSerializableGraphicAnnotation(typeof (RectangleGraphicAnnotationSerializer))]
	public class RectanglePrimitive : BoundableGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RectanglePrimitive"/>.
		/// </summary>
		public RectanglePrimitive() 
		{
		}

		/// <summary>
		/// Performs a hit test on the <see cref="RectanglePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="RectanglePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the rectangle.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			bool result = HitTest(this.SpatialTransform.ConvertToSource(point), this.Rectangle, this.SpatialTransform);
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point on the <see cref="RectanglePrimitive"/> closest to the specified point.
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
			return GetClosestPoint(point, this.BoundingBox);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is contained
		/// in the rectangle.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool Contains(PointF point)
		{
			return this.Rectangle.Contains(point);
		}

		/// <summary>
		/// Gets an object describing the region of interest on the <see cref="Graphic.ParentPresentationImage"/> selected by this <see cref="Graphic"/>.
		/// </summary>
		/// <remarks>
		/// Graphic objects that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="Roi"/> describing this region of interest, or null if the graphic does not describe a region of interest.</returns>
		public override Roi GetRoi()
		{
			return new RectangularRoi(this);
		}

		internal static bool HitTest(PointF point, RectangleF rectangle, SpatialTransform transform)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(RectangleUtilities.ConvertToPositiveRectangle(rectangle));

			Pen pen = new Pen(Brushes.White, HitTestDistance/transform.CumulativeScale);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();

			return result;
		}

		internal static PointF GetClosestPoint(PointF point, RectangleF rectangle)
		{
			double currentDistance;
			double shortestDistance = double.MaxValue;
			PointF currentPoint = new PointF(0, 0);
			PointF closestPoint = new PointF(0, 0);

			PointF ptTopLeft = new PointF(rectangle.Left, rectangle.Top);
			PointF ptBottomRight = new PointF(rectangle.Right, rectangle.Bottom);
			PointF ptTopRight = new PointF(ptBottomRight.X, ptTopLeft.Y);
			PointF ptBottomLeft = new PointF(ptTopLeft.X, ptBottomRight.Y);

			currentDistance = Vector.DistanceFromPointToLine(point, ptTopLeft, ptTopRight, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptTopRight, ptBottomRight, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptBottomRight, ptBottomLeft, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				closestPoint = currentPoint;
			}

			currentDistance = Vector.DistanceFromPointToLine(point, ptBottomLeft, ptTopLeft, ref currentPoint);

			if (currentDistance < shortestDistance)
			{
				closestPoint = currentPoint;
			}

			return closestPoint;
		}
	}
}
