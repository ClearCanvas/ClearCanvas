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
	/// A <see cref="VectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	[Cloneable(true)]
	public abstract class BoundableGraphic : VectorGraphic, IBoundableGraphic
	{
		private PointF _topLeft = new PointF(0, 0);
		private PointF _bottomRight = new PointF(0, 0);
		private event EventHandler<PointChangedEventArgs> _topLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> _bottomRightChangedEvent;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected BoundableGraphic() {}

		/// <summary>
		/// Gets or sets the top-left corner of the rectangle
		/// in either source or destination coordinates.
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
					return _topLeft;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_topLeft);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_topLeft = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_topLeft = base.SpatialTransform.ConvertToSource(value);
				}

				OnTopLeftChanged(new PointChangedEventArgs(TopLeft, CoordinateSystem));
				NotifyVisualStateChanged("TopLeft", VisualStatePropertyKind.Geometry);
			}
		}

		/// <summary>
		/// Gets or sets the bottom-right corner of the rectangle
		/// in either source or destination coordinates.
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
					return _bottomRight;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_bottomRight);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(BottomRight, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_bottomRight = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_bottomRight = base.SpatialTransform.ConvertToSource(value);
				}

				OnBottomRightChanged(new PointChangedEventArgs(BottomRight, CoordinateSystem));
				NotifyVisualStateChanged("BottomRight", VisualStatePropertyKind.Geometry);
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
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _bottomRight.X - _topLeft.X;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return bottomRight.X - topLeft.X;
				}
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Height
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _bottomRight.Y - _topLeft.Y;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		/// <summary>
		/// Gets the rectangle that defines a <see cref="BoundableGraphic"/>.
		/// </summary>
		public RectangleF Rectangle
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return RectangleF.FromLTRB(_topLeft.X, _topLeft.Y, _bottomRight.X, _bottomRight.Y);
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF topLeft = base.SpatialTransform.ConvertToDestination(_topLeft);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(_bottomRight);

					return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
				}
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
			get { return RectangleUtilities.ConvertToPositiveRectangle(Rectangle); }
		}

		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChangedEvent += value; }
			remove { _topLeftChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChangedEvent += value; }
			remove { _bottomRightChangedEvent -= value; }
		}

		/// <summary>
		/// Called to notify that the <see cref="TopLeft"/> property has changed.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnTopLeftChanged(PointChangedEventArgs e)
		{
			EventsHelper.Fire(_topLeftChangedEvent, this, e);
		}

		/// <summary>
		/// Called to notify that the <see cref="BottomRight"/> property has changed.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnBottomRightChanged(PointChangedEventArgs e)
		{
			EventsHelper.Fire(_bottomRightChangedEvent, this, e);
		}

		/// <summary>
		/// Moves the <see cref="BoundableGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			TopLeft += delta;
			BottomRight += delta;
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(PointF point);
	}
}