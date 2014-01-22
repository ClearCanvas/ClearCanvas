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
	/// An arc <see cref="InvariantPrimitive"/>.
	/// </summary>
	[Cloneable(true)]
	public class InvariantArcPrimitive : InvariantBoundablePrimitive, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantArcPrimitive"/>.
		/// </summary>
		public InvariantArcPrimitive()
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
		/// Performs a hit test on the <see cref="InvariantArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantArcPrimitive"/>,
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
			return ArcPrimitive.GetClosestPoint(point, this.Rectangle, this.StartAngle, this.SweepAngle);
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
	}
}
