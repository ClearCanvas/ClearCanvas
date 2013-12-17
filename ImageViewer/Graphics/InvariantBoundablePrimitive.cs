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
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InvariantPrimitive"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	[Cloneable(true)]
	public abstract class InvariantBoundablePrimitive : InvariantPrimitive, IBoundableGraphic
	{
		private event EventHandler<PointChangedEventArgs> _topLeftChanged;
		private event EventHandler<PointChangedEventArgs> _bottomRightChanged;

		private PointF _topLeft = new PointF(0,0);
		private PointF _bottomRight = new PointF(0,0);

		/// <summary>
		/// Constructor.
		/// </summary>
		protected InvariantBoundablePrimitive()
		{
		}

		/// <summary>
		/// Gets the top left corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF topLeft = this.SpatialTransform.ConvertToSource(this.TopLeft);
					this.ResetCoordinateSystem();

					return topLeft;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantTopLeft);
				}
			}
		}

		/// <summary>
		/// Gets or sets the top left corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantTopLeft"/> is different from <see cref="TopLeft"/> in that
		/// it is the top left corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantTopLeft
		{
			get { return _topLeft; }
			set
			{
				if (_topLeft != value)
				{
					_topLeft = value;
					this.OnTopLeftChanged();
					base.NotifyVisualStateChanged("InvariantTopLeft", VisualStatePropertyKind.Geometry);
				}
			}
		}

		/// <summary>
		/// Gets the bottom right corner of the rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF BottomRight
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					this.CoordinateSystem = CoordinateSystem.Destination;
					PointF bottomRight = this.SpatialTransform.ConvertToSource(this.BottomRight);
					this.ResetCoordinateSystem();

					return bottomRight;
				}
				else
				{
					return ConvertInvariantToDestination(InvariantBottomRight);
				}
			}
		}

		/// <summary>
		/// Gets or sets the bottom right corner of the rectangle in <i>screen</i> pixels relative to 
		/// the anchor point.
		/// </summary>
		/// <remarks>
		/// <see cref="InvariantBottomRight"/> is different from <see cref="BottomRight"/> in that
		/// it is the bottom right corner of the rectangle in screen pixels where (0,0)
		/// is the anchor point. For example, if you wanted an
		/// invariant square of size 9x9 screen pixels and the anchor point is in the middle
		/// of the square, <see cref="InvariantTopLeft"/> would be (-4,-4) and 
		/// <see cref="InvariantBottomRight"/> would be (4,4).
		/// </remarks>
		public PointF InvariantBottomRight
		{
			get { return _bottomRight; }
			set
			{
				if (_bottomRight != value)
				{
					_bottomRight = value;
					this.OnBottomRightChanged();
					base.NotifyVisualStateChanged("InvariantBottomRight", VisualStatePropertyKind.Geometry);
				}
			}
		}

		/// <summary>
		/// Gets the width of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get
			{
				return this.BottomRight.X - this.TopLeft.X;
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public float Height
		{
			get
			{
				return this.BottomRight.Y - this.TopLeft.Y;
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="TopLeft"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChanged += value; }
			remove { _topLeftChanged -= value; }
		}

		/// <summary>
		/// Occurs when the value of <see cref="BottomRight"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChanged += value; }
			remove { _bottomRightChanged -= value; }
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(PointF point);

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ConvertToPositiveRectangle(this.Rectangle); }
		}

		/// <summary>
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		public RectangleF Rectangle
		{
			get
			{
				return new RectangleF(this.TopLeft.X, this.TopLeft.Y, this.Width, this.Height);
			}
		}

		/// <summary>
		/// Called when the value of <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected override void OnLocationChanged()
		{
			this.OnTopLeftChanged();
			this.OnBottomRightChanged();
			base.OnLocationChanged();
		}

		private PointF ConvertInvariantToDestination(PointF invariantPoint)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = base.SpatialTransform.ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int)Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			Matrix m = new Matrix();
			m.Rotate(rotation);
			PointF[] pt = { invariantPoint };
			m.TransformPoints(pt);
			m.Dispose();

			return new PointF(base.Location.X + pt[0].X, base.Location.Y + pt[0].Y);
		}

		/// <summary>
		/// Called when the value of <see cref="TopLeft"/> or <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected virtual void OnTopLeftChanged()
		{
			EventsHelper.Fire(_topLeftChanged, this, new PointChangedEventArgs(this.TopLeft, CoordinateSystem));
		}

		/// <summary>
		/// Called when the value of <see cref="BottomRight"/> or <see cref="InvariantPrimitive.Location"/> changes.
		/// </summary>
		protected virtual void OnBottomRightChanged()
		{
			EventsHelper.Fire(_bottomRightChanged, this, new PointChangedEventArgs(this.BottomRight, CoordinateSystem));
		}

		#region IBoundableGraphic Members

		PointF IBoundableGraphic.TopLeft
		{
			get { return this.TopLeft; }
			set { throw new NotSupportedException(); }
		}

		PointF IBoundableGraphic.BottomRight
		{
			get { return this.BottomRight; }
			set { throw new NotSupportedException(); }
		}

		#endregion
	}
}