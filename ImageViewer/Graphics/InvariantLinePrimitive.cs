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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A linear <see cref="InvariantPrimitive"/>.
	/// </summary>
	/// <remarks>
	/// <para>This primitive graphic defines a line whose position can be fixed to the
	/// source coordinate system and whose length will be fixed relative to the
	/// destination coordinate system.</para>
	/// <para>The <see cref="InvariantPrimitive.Location"/> defines the point
	/// that is affixed to the source coordinate system, and the <see cref="InvariantBoundablePrimitive.InvariantTopLeft"/>
	/// and <see cref="InvariantBoundablePrimitive.InvariantBottomRight"/> properties define the length
	/// and orientation of the line.</para>
	/// </remarks>
	[Cloneable(true)]
	public class InvariantLinePrimitive : InvariantBoundablePrimitive, ILineSegmentGraphic
	{
		private event EventHandler<PointChangedEventArgs> _point1Changed;
		private event EventHandler<PointChangedEventArgs> _point2Changed;

		/// <summary>
		/// Constructs a new invariant line primitive.
		/// </summary>
		public InvariantLinePrimitive() {}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantLinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF output = new PointF();
				double distance = Vector.DistanceFromPointToLine(point, this.TopLeft, this.BottomRight, ref output);
				return distance < HitTestDistance;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
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
			RectangleF rect = this.Rectangle;
			Vector.DistanceFromPointToLine(point, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom), ref result);
			return result;
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool Contains(PointF point)
		{
			return false;
		}

		/// <summary>
		/// Called when the value of <see cref="InvariantBoundablePrimitive.TopLeft"/> or <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected override void OnTopLeftChanged()
		{
			EventsHelper.Fire(_point1Changed, this, new PointChangedEventArgs(this.TopLeft, CoordinateSystem));
			base.OnTopLeftChanged();
		}

		/// <summary>
		/// Called when the value of <see cref="InvariantBoundablePrimitive.BottomRight"/> or <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected override void OnBottomRightChanged()
		{
			EventsHelper.Fire(_point2Changed, this, new PointChangedEventArgs(this.BottomRight, CoordinateSystem));
			base.OnBottomRightChanged();
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantBoundablePrimitive.TopLeft"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point1
		{
			get { return this.TopLeft; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// The endpoint of the line as specified by <see cref="InvariantBoundablePrimitive.BottomRight"/> in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point2
		{
			get { return this.BottomRight; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point1Changed
		{
			add { _point1Changed += value; }
			remove { _point1Changed -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point2Changed
		{
			add { _point2Changed += value; }
			remove { _point2Changed -= value; }
		}
	}
}