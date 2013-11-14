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
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive spline graphic.
	/// </summary>
	[Cloneable]
	[Obsolete("Use SplinePrimitive instead.")]
	public class CurvePrimitive : SplinePrimitive
	{
		/// <summary>
		/// Constructs a spline graphic.
		/// </summary>
		public CurvePrimitive() {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected CurvePrimitive(CurvePrimitive source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}
	}

	/// <summary>
	/// A primitive spline graphic.
	/// </summary>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (SplineGraphicAnnotationSerializer))]
	public class SplinePrimitive : VectorGraphic, IPointsGraphic
	{
		[CloneIgnore]
		private readonly PointsList _points;

		/// <summary>
		/// Constructs a spline graphic.
		/// </summary>
		public SplinePrimitive()
		{
			_points = new PointsList(this);
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected SplinePrimitive(SplinePrimitive source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_points = new PointsList(source._points, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		/// <summary>
		/// Gets the ordered list of points that defines the spline.
		/// </summary>
		public IPointsList Points
		{
			get { return _points; }
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
			get { return RectangleUtilities.ComputeBoundingRectangle(_points); }
		}

		private void Initialize()
		{
			_points.PointAdded += OnPointsChanged;
			_points.PointChanged += OnPointsChanged;
			_points.PointRemoved += OnPointsChanged;
			_points.PointsCleared += OnPointsChanged;
		}

		private void OnPointsChanged(object sender, EventArgs e)
		{
			base.NotifyVisualStateChanged("Points", VisualStatePropertyKind.Geometry);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="Graphic"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		public override bool HitTest(Point point)
		{
			base.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				using (var gp = new GraphicsPath())
				using (var pen = new Pen(Color.Black, HitTestDistance))
				{
					PointF[] pathPoints = GetCurvePoints(_points);
					if (_points.IsClosed)
						gp.AddClosedCurve(pathPoints);
					else
						gp.AddCurve(pathPoints);
					return gp.IsOutlineVisible(point, pen);
				}
			}
			finally
			{
				base.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Gets the point on the <see cref="SplinePrimitive"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// <para>Since the interpolation between nodes of the curve is not explicitly
		/// defined, this method returns the closest node to the specified point, and
		/// ignores the individual curve segments for the purposes of this calculation.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			PointF result = PointF.Empty;
			double min = double.MaxValue;
			foreach (PointF pt in _points)
			{
				double d = Vector.Distance(point, pt);
				if (min > d)
				{
					min = d;
					result = pt;
				}
			}
			return result;
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
			for (int n = 0; n < _points.Count; n++)
			{
				_points[n] = _points[n] + delta;
			}
		}

		private static PointF[] GetCurvePoints(IPointsList points)
		{
			PointF[] result = new PointF[points.Count - (points.IsClosed ? 1 : 0)];
			for (int n = 0; n < result.Length; n++)
				result[n] = points[n];
			return result;
		}
	}
}