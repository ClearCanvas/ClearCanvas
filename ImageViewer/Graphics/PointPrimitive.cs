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
	/// A primitive point graphic.
	/// </summary>
	[Cloneable(true)]
	public class PointPrimitive : VectorGraphic, IPointGraphic
	{
		private event EventHandler _pointChanged;
		private PointF _point;

		/// <summary>
		/// Initializes a new instance of <see cref="PointPrimitive"/>.
		/// </summary>
		public PointPrimitive()
		{
		}

		/// <summary>
		/// Gets or sets the location of the point in either source
		/// or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Point
		{
			get 
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point);
				}
			}
			set 
			{
				if (FloatComparer.AreEqual(this.Point, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point = base.SpatialTransform.ConvertToSource(value);
				}

				this.NotifyPointChanged();
				base.NotifyVisualStateChanged("Point", VisualStatePropertyKind.Geometry);
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="IPointGraphic.Point"/> changes.
		/// </summary>
		public event EventHandler PointChanged
		{
			add { _pointChanged += value; }
			remove { _pointChanged -= value; }
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
			get { return new RectangleF(this.Point, SizeF.Empty); }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="PointPrimitive"/>.
		/// </summary>
		/// <param name="point">The test point in destination coordinates.</param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return FloatComparer.AreEqual(base.SpatialTransform.ConvertToDestination(_point), point);
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
			return this.Point;
		}

		/// <summary>
		/// Move the <see cref="PointPrimitive"/> by a specified delta.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
			this.Point += delta;
		}

		private void NotifyPointChanged()
		{
			EventsHelper.Fire(_pointChanged, this, EventArgs.Empty);
		}
	}
}
