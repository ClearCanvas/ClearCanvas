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
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="VectorGraphic"/> whose size in destination coordinates is invariant
	/// under zoom.
	/// </summary>
	/// <remarks>
	/// Sometimes it is desirable to have a <see cref="VectorGraphic"/> whose
	/// <i>size</i> is invariant under zoom, but whose position is not.  A good example
	/// are the <see cref="ControlPoint"/> objects on measurement tools that allow a 
	/// user to stretch and resize the measurement.  They are anchored to a certain
	/// point on the underlying image so that when zoomed, the control point appears
	/// to "move" with the zoom of the image, but their size
	/// in screen pixels remains the same.
	/// </remarks>
	[Cloneable(true)]
	public abstract class InvariantPrimitive : VectorGraphic
	{
		private PointF _location;
		private event EventHandler<PointChangedEventArgs> _locationChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantPrimitive"/>.
		/// </summary>
		protected InvariantPrimitive()
		{
		}

		/// <summary>
		/// The location where the <see cref="InvariantPrimitive"/> is anchored.
		/// </summary>
		/// <remarks>
		/// The value of this property is in either source or destination coordinates
		/// depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _location;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Location, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_location = base.SpatialTransform.ConvertToSource(value);
				}

				this.OnLocationChanged();
				base.NotifyVisualStateChanged("Location", VisualStatePropertyKind.Geometry);
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Location"/> property has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> LocationChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// Called when the value of <see cref="Location"/> changes.
		/// </summary>
		protected virtual void OnLocationChanged()
		{
			EventsHelper.Fire(_locationChangedEvent, this, new PointChangedEventArgs(this.Location, CoordinateSystem));
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
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
			this.Location += del;
#else
			this.Location += delta;
#endif
		}

		#region Legacy Members

		/// <summary>
		/// Occurs when the <see cref="Location"/> property has changed.
		/// </summary>
		[Obsolete("Use the LocationChanged event instead.")]
		public event EventHandler<PointChangedEventArgs> AnchorPointChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// The point in the coordinate system of the parent <see cref="IGraphic"/> where the
		/// <see cref="InvariantPrimitive"/> is anchored.
		/// </summary>
		[Obsolete("Use the Location property instead.")]
		public PointF AnchorPoint
		{
			get { return this.Location; }
			set { this.Location = value; }
		}

		#endregion
	}
}
