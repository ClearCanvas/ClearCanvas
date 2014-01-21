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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive line graphic.
	/// </summary>
	[Cloneable(true)]
	public class LinePrimitive : VectorGraphic, ILineSegmentGraphic
	{
		#region Private fields
		
		private PointF _point1;
		private PointF _point2;
		private event EventHandler<PointChangedEventArgs> _point1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> _point2ChangedEvent;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LinePrimitive"/>.
		/// </summary>
		public LinePrimitive()
		{
		}

		/// <summary>
		/// Gets or sets one endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Point1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point1;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point1);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Point1, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point1 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point1 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_point1ChangedEvent, this, new PointChangedEventArgs(this.Point1, CoordinateSystem));
				base.NotifyVisualStateChanged("Point1", VisualStatePropertyKind.Geometry);
			}
		}

		/// <summary>
		/// Gets or sets the other endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Point2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point2;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point2);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Point2, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point2 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point2 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_point2ChangedEvent, this, new PointChangedEventArgs(this.Point2, CoordinateSystem));
				base.NotifyVisualStateChanged("Point2", VisualStatePropertyKind.Geometry);
			}
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ComputeBoundingRectangle(this.Point1, this.Point2); }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point1"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Point1Changed
		{
			add { _point1ChangedEvent += value; }
			remove { _point1ChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point2"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Point2Changed
		{
			add { _point2ChangedEvent += value; }
			remove { _point2ChangedEvent -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="LinePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="LinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the line.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);

			// Always do the hit test in destination coordinates since we want the
			// "activation distance" to be the same irrespective of the zoom
			this.CoordinateSystem = CoordinateSystem.Destination;
			distance = Vector.DistanceFromPointToLine(point, this.Point1, this.Point2, ref ptNearest);
			this.ResetCoordinateSystem();

			if (distance < HitTestDistance)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the point on the <see cref="Graphic"/> closest to the specified point.
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
			PointF result = PointF.Empty;
			Vector.DistanceFromPointToLine(point, this.Point1, this.Point2, ref result);
			return result;
		}

		/// <summary>
		/// Moves the <see cref="LinePrimitive"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.Point1 += del;
			this.Point2 += del;
#else
			this.Point1 += delta;
			this.Point2 += delta;
#endif
		}

		#region Legacy Members

		/// <summary>
		/// This member has been deprecated in favour of <see cref="Point1"/>.
		/// </summary>
		[Obsolete("Use LinePrimitive.Point1 instead.")]
		public PointF Pt1
		{
			get { return this.Point1; }
			set { this.Point1 = value; }
		}

		/// <summary>
		/// This member has been deprecated in favour of <see cref="Point2"/>.
		/// </summary>
		[Obsolete("Use LinePrimitive.Point2 instead.")]
		public PointF Pt2
		{
			get { return this.Point2; }
			set { this.Point2 = value; }
		}

		/// <summary>
		/// This member has been deprecated in favour of <see cref="Point1Changed"/>.
		/// </summary>
		[Obsolete("Use LinePrimitive.Point1Changed instead.")]
		public event EventHandler<PointChangedEventArgs> Pt1Changed
		{
			add { _point1ChangedEvent += value; }
			remove { _point1ChangedEvent -= value; }
		}

		/// <summary>
		/// This member has been deprecated in favour of <see cref="Point2Changed"/>.
		/// </summary>
		[Obsolete("Use LinePrimitive.Point2Changed instead.")]
		public event EventHandler<PointChangedEventArgs> Pt2Changed
		{
			add { _point2ChangedEvent += value; }
			remove { _point2ChangedEvent -= value; }
		}

		#endregion
	}
}
