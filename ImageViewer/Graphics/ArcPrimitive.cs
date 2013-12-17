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
using ClearCanvas.ImageViewer.Mathematics;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive arc graphic.
	/// </summary>
	/// <remarks>
	/// An arc is defined by a portion of the perimeter of an ellipse.
	/// The ellipse is defined by a bounding rectangle defined by the
	/// base class, <see cref="BoundableGraphic"/>.  The portion of the
	/// ellipse is defined by the <see cref="ArcPrimitive.StartAngle"/>
	/// and <see cref="ArcPrimitive.SweepAngle"/>.
	/// </remarks>
	[Cloneable(true)]
	public class ArcPrimitive : BoundableGraphic, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="ArcPrimitive"/>.
		/// </summary>
		public ArcPrimitive()
		{
			
		}

		/// <summary>
		/// Gets or sets the angle in degrees at which the arc begins.
		/// </summary>
		/// <remarks>
		/// It is good practice to set the <see cref="IArcGraphic.StartAngle"/> before the <see cref="IArcGraphic.SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="IArcGraphic.SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="IArcGraphic.StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="IArcGraphic.StartAngle"/>
		/// and <see cref="IArcGraphic.SweepAngle"/> can be set independently.
		/// </remarks>
		public float StartAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _startAngle;
				}
				else
				{
					return ArcPrimitive.ConvertStartAngle(_startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == Graphics.CoordinateSystem.Destination)
					value = ArcPrimitive.ConvertStartAngle(value, this.SpatialTransform, CoordinateSystem.Source);

				if (!FloatComparer.AreEqual(_startAngle, value))
				{
					_startAngle = value;
					base.NotifyVisualStateChanged("StartAngle", VisualStatePropertyKind.Geometry);
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle in degrees that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="IArcGraphic.StartAngle"/> for information on setting the <see cref="IArcGraphic.SweepAngle"/>.
		/// </remarks>
		public float SweepAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _sweepAngle;
				}
				else
				{
					return ArcPrimitive.ConvertSweepAngle(_sweepAngle, _startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Destination)
				{
					this.CoordinateSystem = CoordinateSystem.Destination;
					value = ArcPrimitive.ConvertSweepAngle(value, StartAngle, this.SpatialTransform, CoordinateSystem.Source);
					this.ResetCoordinateSystem();
				}

				if (!FloatComparer.AreEqual(_sweepAngle, value))
				{
					_sweepAngle = value;
					base.NotifyVisualStateChanged("SweepAngle", VisualStatePropertyKind.Geometry);
				}
			}
		}

		/// <summary>
		/// Performs a hit test on the <see cref="ArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="ArcPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;

			bool result = ArcPrimitive.HitTest(
				SpatialTransform.ConvertToSource(point), this.Rectangle,
				this.StartAngle, this.SweepAngle,
				this.SpatialTransform);
			
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point on the <see cref="ArcPrimitive"/> closest to the specified point.
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
			return GetClosestPoint(point, this.Rectangle, this.StartAngle, this.SweepAngle);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>Always returns <b>false</b>, since an arc cannot
		/// contain a point.</returns>
		public override bool Contains(PointF point)
		{
			return false;
		}

		internal static PointF GetClosestPoint(
			PointF point,
			RectangleF boundingBox,
			float startAngle,
			float sweepAngle)
		{
			// Semi major/minor axes
			float a = boundingBox.Width/2;
			float b = boundingBox.Height/2;

			// Center of ellipse
			float x1 = boundingBox.Left + a;
			float y1 = boundingBox.Top + b;

			// normalize the angles
			float normalizedSweepAngle = Math.Sign(sweepAngle) * Math.Min(360, Math.Abs(sweepAngle));

			float normalizedStartAngle = startAngle % 360;
			if (normalizedStartAngle < 0)
				normalizedStartAngle += 360;

			float normalizedEndAngle = (normalizedStartAngle + normalizedSweepAngle);
			if (normalizedSweepAngle < 0)
			{
				//swap start and end angles
				float t = normalizedStartAngle;
				normalizedStartAngle = normalizedEndAngle;
				normalizedEndAngle = t;

				//the sweep angle is now positive
				normalizedSweepAngle *= -1;
			}

			// find the closest endpoint
			const double degreesToRadians = Math.PI / 180;

			PointF center = new PointF(x1, y1);
			double normalizedStartAngleRadians = normalizedStartAngle * degreesToRadians;
			double normalizedEndAngleRadians = normalizedEndAngle * degreesToRadians;
			PointF start = new PointF(center.X + a * (float)Math.Cos(normalizedStartAngleRadians), center.Y + b * (float)Math.Sin(normalizedStartAngleRadians));
			PointF end = new PointF(center.X + a * (float)Math.Cos(normalizedEndAngleRadians), center.Y + b * (float)Math.Sin(normalizedEndAngleRadians));

			float distanceToStartX = start.X - point.X;
			float distanceToStartY = start.Y - point.Y;
			float distanceToEndX = end.X - point.X;
			float distanceToEndY = end.Y - point.Y;

			float squareDistanceToStart = distanceToStartX * distanceToStartX + distanceToStartY * distanceToStartY;
			float squareDistanceToEnd = distanceToEndX * distanceToEndX + distanceToEndY * distanceToEndY;

			PointF closestPoint = (squareDistanceToStart < squareDistanceToEnd) ? start : end;
			float minSquareDistance = Math.Min(squareDistanceToStart, squareDistanceToEnd);

			// find the intersection along the ray eminating from center towards point, and calculate its angle from the x-axis
			PointF result = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, point);

			//Check if the angle between 'start' and 'result' is positive and less than 'sweep'.
			double angleStartToPoint = Vector.SubtendedAngle(result, center, start);
			if (angleStartToPoint < normalizedSweepAngle && angleStartToPoint > 0)
			{
				float distanceToResultX = result.X - point.X;
				float distanceToResultY = result.Y - point.Y;
				float squareDistanceToResult = distanceToResultX * distanceToResultX + distanceToResultY * distanceToResultY;
				if (squareDistanceToResult < minSquareDistance)
					closestPoint = result;
			}

			return closestPoint;
		}

		internal static bool HitTest(
			PointF point, 
			RectangleF boundingBox, 
			float startAngle,
			float sweepAngle,
			SpatialTransform transform)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(RectangleUtilities.ConvertToPositiveRectangle(boundingBox), startAngle, sweepAngle);

			Pen pen = new Pen(Brushes.White, HitTestDistance / transform.CumulativeScale);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();

			return result;
		}

		internal static float ConvertStartAngle(float angle, SpatialTransform transform, CoordinateSystem targetSystem)
		{
			PointF xVector = new PointF(100, 0);

			Matrix rotation = new Matrix();
			PointF[] angleVector = new PointF[] { xVector };
			rotation.Rotate(angle);
			rotation.TransformVectors(angleVector);
			rotation.Dispose();

			SizeF xVectorTransformed, angleVectorTransformed;
			if (targetSystem == Graphics.CoordinateSystem.Destination)
			{
				xVectorTransformed = transform.ConvertToDestination(new SizeF(xVector));
				angleVectorTransformed = transform.ConvertToDestination(new SizeF(angleVector[0]));
			}
			else
			{
				xVectorTransformed = transform.ConvertToSource(new SizeF(xVector));
				angleVectorTransformed = transform.ConvertToSource(new SizeF(angleVector[0]));
			}

			float xRotationOffset =
				(int)Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));

			float angleTransformed =
				(int)Math.Round(Vector.SubtendedAngle(angleVectorTransformed.ToPointF(), PointF.Empty, xVectorTransformed.ToPointF()));

			// have to figure out where x-axis moved to and then return the difference between the angle
			// and the x-axis, where both are in 'target' coordinates.
			float returnAngle = angleTransformed + xRotationOffset;
			if (returnAngle < 0)
				returnAngle += 360;

			return returnAngle;
		}

		internal static float ConvertSweepAngle(float sweepAngle, float startAngle, SpatialTransform transform, CoordinateSystem targetSystem)
		{
			PointF x = new PointF(100, 0);

			PointF[] startVector = new PointF[] { x };
			Matrix rotation = new Matrix();
			rotation.Rotate(startAngle);
			rotation.TransformVectors(startVector);

			PointF[] sweepVector = (PointF[])startVector.Clone();
			rotation.Reset();
			rotation.Rotate(sweepAngle);
			rotation.TransformVectors(sweepVector);
			rotation.Dispose();

			SizeF startVectorTransformed, sweepVectorTransformed;
			if (targetSystem == Graphics.CoordinateSystem.Destination)
			{
				startVectorTransformed = transform.ConvertToDestination(new SizeF(startVector[0]));
				sweepVectorTransformed = transform.ConvertToDestination(new SizeF(sweepVector[0]));
			}
			else
			{
				startVectorTransformed = transform.ConvertToSource(new SizeF(startVector[0]));
				sweepVectorTransformed = transform.ConvertToSource(new SizeF(sweepVector[0]));
			}

			// simply return the angle between the start and sweep angle, in the target system.
			return (int)Math.Round(Vector.SubtendedAngle(sweepVectorTransformed.ToPointF(), PointF.Empty, startVectorTransformed.ToPointF()));
		}
	}
}
