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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A <see cref="VectorGraphic3D"/> that can be described by a
	/// 3D rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangular cuboids and ellipsoids are examples of graphics that can be
	/// described by a 3D rectangular bounding box.
	/// </remarks>
	[Cloneable(true)]
	public abstract class BoundableGraphic3D : VectorGraphic3D
	{
		[CloneCopyReference]
		private Vector3D _frontTopLeft = Vector3D.Null;

		[CloneCopyReference]
		private Vector3D _backBottomRight = Vector3D.Null;

		private event PointChanged3DEventHandler _frontTopLeftChanged;
		private event PointChanged3DEventHandler _backBottomRightChanged;

		/// <summary>
		/// Gets or sets the front-top-left corner of the 3D rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public Vector3D FrontTopLeft
		{
			get
			{
				if (CoordinateSystem == CoordinateSystem.Source)
					return _frontTopLeft;
				else
				{
					Platform.CheckMemberIsSet(SpatialTransform, "SpatialTransform");
					return SpatialTransform.ConvertPointToDestination(_frontTopLeft);
				}
			}
			set
			{
				if (value == null) value = Vector3D.Null;
				if (Vector3D.AreEqual(FrontTopLeft, value)) return;

				if (CoordinateSystem == CoordinateSystem.Source)
					_frontTopLeft = value;
				else
				{
					Platform.CheckMemberIsSet(SpatialTransform, "SpatialTransform");
					_frontTopLeft = SpatialTransform.ConvertPointToSource(value);
				}

				OnFrontTopLeftChanged(new PointChanged3DEventArgs(value, CoordinateSystem));
				NotifyVisualStateChanged("FrontTopLeft");
			}
		}

		/// <summary>
		/// Gets or sets the back-bottom-right corner of the 3D rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public Vector3D BackBottomRight
		{
			get
			{
				if (CoordinateSystem == CoordinateSystem.Source)
					return _backBottomRight;
				else
				{
					Platform.CheckMemberIsSet(SpatialTransform, "SpatialTransform");
					return SpatialTransform.ConvertPointToDestination(_backBottomRight);
				}
			}
			set
			{
				if (value == null) value = Vector3D.Null;
				if (Vector3D.AreEqual(BackBottomRight, value)) return;

				if (CoordinateSystem == CoordinateSystem.Source)
					_backBottomRight = value;
				else
				{
					Platform.CheckMemberIsSet(SpatialTransform, "SpatialTransform");
					_backBottomRight = SpatialTransform.ConvertPointToSource(value);
				}

				OnBackBottomRightChanged(new PointChanged3DEventArgs(value, CoordinateSystem));
				NotifyVisualStateChanged("BackBottomRight");
			}
		}

		/// <summary>
		/// Gets or sets the diagonal vector of the 3D rectangle
		/// (from the front-top-left to the back-bottom-right corner)
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public Vector3D Diagonal
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _backBottomRight - _frontTopLeft;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					var topLeft = base.SpatialTransform.ConvertPointToDestination(_frontTopLeft);
					var bottomRight = base.SpatialTransform.ConvertPointToDestination(_backBottomRight);
					return bottomRight - topLeft;
				}
			}
		}

		/// <summary>
		/// Gets the width of the 3D rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get { return Diagonal.X; }
		}

		/// <summary>
		/// Gets the height of the 3D rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Height
		{
			get { return Diagonal.Y; }
		}

		/// <summary>
		/// Gets the depth of the 3D rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Depth
		{
			get { return Diagonal.Z; }
		}

		/// <summary>
		/// Gets the 3D rectangle that defines a <see cref="BoundableGraphic3D"/>.
		/// </summary>
		public Rectangle3D Rectangle
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return Rectangle3D.FromLTFRBB(_frontTopLeft.X, _frontTopLeft.Y, _frontTopLeft.Z, _backBottomRight.X, _backBottomRight.Y, _backBottomRight.Z);
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					var topLeft = base.SpatialTransform.ConvertPointToDestination(_frontTopLeft);
					var bottomRight = base.SpatialTransform.ConvertPointToDestination(_backBottomRight);
					return Rectangle3D.FromLTFRBB(topLeft.X, topLeft.Y, topLeft.Z, bottomRight.X, bottomRight.Y, bottomRight.Z);
				}
			}
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override Rectangle3D BoundingBox
		{
			get { return Rectangle.ToPositiveRectangle(); }
		}

		/// <summary>
		/// Occurs when the <see cref="FrontTopLeft"/> property has changed.
		/// </summary>
		public event PointChanged3DEventHandler FrontTopLeftChanged
		{
			add { _frontTopLeftChanged += value; }
			remove { _frontTopLeftChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="BackBottomRight"/> property has changed.
		/// </summary>
		public event PointChanged3DEventHandler BackBottomRightChanged
		{
			add { _backBottomRightChanged += value; }
			remove { _backBottomRightChanged -= value; }
		}

		/// <summary>
		/// Called to notify that the <see cref="FrontTopLeft"/> property has changed.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnFrontTopLeftChanged(PointChanged3DEventArgs e)
		{
			EventsHelper.Fire(_frontTopLeftChanged, this, e);
		}

		/// <summary>
		/// Called to notify that the <see cref="BackBottomRight"/> property has changed.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnBackBottomRightChanged(PointChanged3DEventArgs e)
		{
			EventsHelper.Fire(_backBottomRightChanged, this, e);
		}

		/// <summary>
		/// Moves the <see cref="BoundableGraphic3D"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(Vector3D delta)
		{
			FrontTopLeft += delta;
			BackBottomRight += delta;
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(Vector3D point);
	}
}