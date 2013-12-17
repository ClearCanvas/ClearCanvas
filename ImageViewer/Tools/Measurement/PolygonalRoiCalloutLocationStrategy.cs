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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// A specialization of <see cref="DefaultRoiCalloutLocationStrategy"/> for
	/// polygonal ROIs implementing <see cref="IPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Like <see cref="DefaultRoiCalloutLocationStrategy"/>, this implementation
	/// uses the ROI graphic's <see cref="IGraphic.BoundingBox"/> compute a callout
	/// position that tries to minimize callout obstruction of the underlying anatomy
	/// while keeping the callout within the image tile. Additionally, the callout
	/// is hidden until the ROI graphic forms a valid polygon.
	/// </para>
	/// <para>
	/// The auto computation is disabled if the user manually positions the callout.
	/// </para>
	/// </remarks>
	[Cloneable(true)]
	public class PolygonalRoiCalloutLocationStrategy : DefaultRoiCalloutLocationStrategy
	{
		private bool _computingInitialCalloutLocation = false;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (AnnotationGraphic != null)
				{
					var pointsGraphic = AnnotationGraphic.Subject as IPointsGraphic;
					if (pointsGraphic != null)
					{
						pointsGraphic.Points.PointAdded -= OnPolygonGraphicClosed;
						pointsGraphic.Points.PointChanged -= OnPolygonGraphicClosed;
						pointsGraphic.Points.PointRemoved -= OnPolygonGraphicClosed;
						pointsGraphic.Points.PointsCleared -= OnPolygonGraphicClosed;
					}
				}
			}
			base.Dispose(disposing);
		}

		public override void OnCalloutLocationChangedExternally()
		{
			if (_computingInitialCalloutLocation)
				return;
			base.OnCalloutLocationChangedExternally();
		}

		protected override void OnAnnotationGraphicChanged(IAnnotationGraphic oldAnnotationGraphic, IAnnotationGraphic annotationGraphic)
		{
			if (oldAnnotationGraphic != null)
			{
				var pointsGraphic = oldAnnotationGraphic.Subject as IPointsGraphic;
				if (pointsGraphic != null)
				{
					pointsGraphic.Points.PointAdded -= OnPolygonGraphicClosed;
					pointsGraphic.Points.PointChanged -= OnPolygonGraphicClosed;
					pointsGraphic.Points.PointRemoved -= OnPolygonGraphicClosed;
					pointsGraphic.Points.PointsCleared -= OnPolygonGraphicClosed;
				}
			}

			base.OnAnnotationGraphicChanged(oldAnnotationGraphic, annotationGraphic);

			if (annotationGraphic != null)
			{
				var pointsGraphic = annotationGraphic.Subject as IPointsGraphic;
				if (pointsGraphic != null)
				{
					pointsGraphic.Points.PointAdded += OnPolygonGraphicClosed;
					pointsGraphic.Points.PointChanged += OnPolygonGraphicClosed;
					pointsGraphic.Points.PointRemoved += OnPolygonGraphicClosed;
					pointsGraphic.Points.PointsCleared += OnPolygonGraphicClosed;
				}
			}
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			base.Callout.Visible = !string.IsNullOrEmpty(base.Callout.Text);

			if (Callout.Visible && IsClosed(Roi))
				return base.CalculateCalloutLocation(out location, out coordinateSystem);

			location = PointF.Empty;
			coordinateSystem = CoordinateSystem.Destination;

			return false;
		}

		protected new IPointsGraphic Roi
		{
            get { return ((IPointsGraphic)AnnotationSubject); }
		}

		private void OnPolygonGraphicClosed(object sender, EventArgs e)
		{
			// sometimes the coordinate systems are mismatched, so force fix it now
			this.Roi.CoordinateSystem = CoordinateSystem.Destination;
			_computingInitialCalloutLocation = true;
			try
			{
				PointF location;
				CoordinateSystem coordinateSystem;

				// compute a nice location for the callout
				if (this.CalculateCalloutLocation(out location, out coordinateSystem))
				{
					this.Callout.CoordinateSystem = coordinateSystem;
					this.Callout.TextLocation = location;
					this.Callout.ResetCoordinateSystem();
				}
			}
			finally
			{
				_computingInitialCalloutLocation = false;
				this.Roi.ResetCoordinateSystem();
			}
		}

		private static bool IsClosed(IPointsGraphic g)
		{
			if (g.Points.Count > 2)
				return FloatComparer.AreEqual(g.Points[0], g.Points[g.Points.Count - 1]);
			return false;
		}
	}
}