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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	[Cloneable]
	public class ProtractorGraphic : PolylineGraphic
	{
		private const int _arcRadius = 20;

		[CloneIgnore]
		private InvariantArcPrimitive _arc;

		protected ProtractorGraphic(ProtractorGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public ProtractorGraphic() : base()
		{
			base.Graphics.Add(_arc = new InvariantArcPrimitive());
			_arc.Visible = false;
		}

		protected override void OnColorChanged()
		{
			base.OnColorChanged();
			_arc.Color = base.Color;
		}

		public override void OnDrawing()
		{
			base.OnDrawing();

			if (this.Points.Count == 3)
			{
				_arc.Visible = IsArcVisible();

				if (_arc.Visible)
					CalculateArc();
			}
		}

		public override bool HitTest(Point point)
		{
			if (_arc.Visible)
			{
				return base.HitTest(point);
			}

			foreach (IGraphic graphic in this.Graphics)
			{
				if (graphic.Visible && graphic.HitTest(point))
					return true;
			}
			return false;
		}

		public override PointF GetClosestPoint(PointF point)
		{
			if (_arc.Visible)
			{
				return base.GetClosestPoint(point);
			}

			foreach (IGraphic graphic in this.Graphics)
			{
				if (graphic.Visible)
					return graphic.GetClosestPoint(point);
			}
			return PointF.Empty;
		}

		private void CalculateArc()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			_arc.CoordinateSystem = CoordinateSystem.Destination;

			// The arc center is the vertex of the protractor
			_arc.Location = this.Points[1];

			_arc.InvariantTopLeft = new PointF(-_arcRadius, -_arcRadius);
			_arc.InvariantBottomRight = new PointF(_arcRadius, _arcRadius);

			float startAngle, sweepAngle;
			CalculateAngles(out startAngle, out sweepAngle);
			_arc.StartAngle = startAngle;
			_arc.SweepAngle = sweepAngle;

			_arc.ResetCoordinateSystem();
			this.ResetCoordinateSystem();
		}

		private void CalculateAngles(out float startAngle, out float sweepAngle)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			sweepAngle = -(float) Vector.SubtendedAngle(
				this.Points[0],
				this.Points[1],
				this.Points[2]);

			// Define a horizontal ray
			PointF zeroDegreePoint = this.Points[1];
			zeroDegreePoint.X += 50;

			startAngle = (float) Vector.SubtendedAngle(
				this.Points[0],
				this.Points[1],
				zeroDegreePoint);

			this.ResetCoordinateSystem();
		}

		private bool IsArcVisible()
		{
			// Arc should only be visible if the arc radius is smaller than both of the
			// two arms of the angle
			this.CoordinateSystem = CoordinateSystem.Destination;
			Vector3D vertexPositionVector = new Vector3D(this.Points[1].X, this.Points[1].Y, 0);
			Vector3D a = new Vector3D(this.Points[0].X, this.Points[0].Y, 0) - vertexPositionVector;
			Vector3D b = new Vector3D(this.Points[2].X, this.Points[2].Y, 0) - vertexPositionVector;
			this.ResetCoordinateSystem();

			return a.Magnitude > _arcRadius && b.Magnitude > _arcRadius;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_arc = CollectionUtils.SelectFirst(base.Graphics,
			                                   delegate(IGraphic test) { return test is InvariantArcPrimitive; }) as InvariantArcPrimitive;

			Platform.CheckForNullReference(_arc, "_arc");
		}

		public override Roi GetRoi()
		{
			return new ProtractorRoi(this);
		}
	}
}