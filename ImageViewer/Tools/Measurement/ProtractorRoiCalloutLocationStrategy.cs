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

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[Cloneable(true)]
	public class ProtractorRoiCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _firstCalculation;
		private PointF _lastVertexLocationSource;
		private bool _userMovedCallout;

		public ProtractorRoiCalloutLocationStrategy()
		{
			_firstCalculation = true;
			_userMovedCallout = false;
		}

		public new IPointsGraphic Roi
		{
            get { return (IPointsGraphic)AnnotationSubject; }
		}

		protected override void OnAnnotationGraphicChanged(IAnnotationGraphic oldAnnotationGraphic, IAnnotationGraphic annotationGraphic)
		{
			base.OnAnnotationGraphicChanged(oldAnnotationGraphic, annotationGraphic);
			if (_firstCalculation)
				base.Callout.Visible = false;
		}

		public override void OnCalloutLocationChangedExternally()
		{
			_userMovedCallout = true;
		}

		public override void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem)
		{
			if (_userMovedCallout)
			{
				base.CalculateCalloutEndPoint(out endPoint, out coordinateSystem);
			}
			else
			{
                coordinateSystem = AnnotationSubject.CoordinateSystem;

				if (this.Roi.Points.Count < 3)
					endPoint = this.Roi.Points[0];
				else
					endPoint = this.Roi.Points[1];
			}
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			if (this.Roi.Points.Count < 3 || string.IsNullOrEmpty(this.Callout.Text))
				base.Callout.Visible = false;
			else
				base.Callout.Visible = true;

			if (!base.Callout.Visible || _userMovedCallout)
				return base.CalculateCalloutLocation(out location, out coordinateSystem);

			SizeF calloutOffsetDestination = GetCalloutOffsetDestination();

			coordinateSystem = CoordinateSystem.Destination;
			base.AnnotationGraphic.CoordinateSystem = coordinateSystem;

			// first, move the callout by the same amount the vertex moved (if it moved at all).
			location = base.Callout.TextLocation + calloutOffsetDestination;

			PointF start = this.Roi.Points[0];
			PointF vertex = this.Roi.Points[1];
			PointF end = this.Roi.Points[2];

			base.AnnotationGraphic.ResetCoordinateSystem();

			double vectorAngle = -Vector.SubtendedAngle(start, vertex, end) / 2 + 180;

			PointF[] points = new PointF[] { start, end };

			using (Matrix rotation = new Matrix())
			{
				rotation.Rotate((float) vectorAngle);
				rotation.Translate(-vertex.X, -vertex.Y);
				rotation.TransformPoints(points);
			}

			float calloutMagnitude = new Vector3D(location.X - vertex.X, location.Y - vertex.Y, 0).Magnitude;

			Vector3D startVector = new Vector3D(points[0].X, points[0].Y, 0);
			if (FloatComparer.AreEqual(startVector.Magnitude, 0F, 0.01F))
				startVector = new Vector3D(-1, 0, 0);

			startVector = startVector / startVector.Magnitude * calloutMagnitude;

			location = new PointF(startVector.X + vertex.X, startVector.Y + vertex.Y);

			return true;
		}

		private SizeF GetCalloutOffsetDestination()
		{
			base.AnnotationGraphic.CoordinateSystem = CoordinateSystem.Source;

			SizeF calloutOffsetDestination = SizeF.Empty;

			if (_firstCalculation)
			{
				//on first calculation, move the callout to the vertex.
				_firstCalculation = false;
				_lastVertexLocationSource = base.Callout.AnchorPoint;
			}

			PointF currentVertexLocationSource = this.Roi.Points[1];

			if (_lastVertexLocationSource != currentVertexLocationSource)
			{
				PointF currentVertexLocationDestination = Roi.SpatialTransform.ConvertToDestination(currentVertexLocationSource);
				PointF lastVertexLocationDestination = Roi.SpatialTransform.ConvertToDestination(_lastVertexLocationSource);

				calloutOffsetDestination = new SizeF(
					currentVertexLocationDestination.X - lastVertexLocationDestination.X,
					currentVertexLocationDestination.Y - lastVertexLocationDestination.Y);
			}

			_lastVertexLocationSource = currentVertexLocationSource;

			base.AnnotationGraphic.ResetCoordinateSystem();

			return calloutOffsetDestination;
		}
	}
}
