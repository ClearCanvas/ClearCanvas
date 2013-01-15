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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class RotateControlGraphic : ControlPointsGraphic
	{
		private PointF _centre;
		private PointF _reference;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		public RotateControlGraphic(IGraphic subject)
			: base(subject)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF bounds = subject.BoundingBox;
				_centre = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
				_reference = _centre + new SizeF(0, bounds.Height/5);
				base.ControlPoints.Add(_reference);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		private RotateControlGraphic(RotateControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation) {
			return base.ConstrainControlPointLocation(controlPointIndex, cursorLocation);
		}

		//protected override void OnMoved() {
		//    base.OnMoved();

		//    this.CoordinateSystem = CoordinateSystem.Source;
		//    try {
		//        RectangleF bounds = this.Subject.BoundingBox;
		//        PointF newCentre = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
		//        SizeF offset = new SizeF(newCentre.X - _centre.X, newCentre.Y - _centre.Y);
		//        _centre += offset;
		//        _reference += offset;
		//        base.ControlPoints[0] += offset;
		//    } finally {
		//        this.ResetCoordinateSystem();
		//    }
		//}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				PointF centre = _centre;
				PointF reference = _reference;

				if(this.CoordinateSystem ==  CoordinateSystem.Destination)
				{
					_centre = this.SpatialTransform.ConvertToSource(_centre);
					_reference = this.SpatialTransform.ConvertToSource(_reference);
				}

				this.SpatialTransform.RotationXY = (int) Vector.SubtendedAngle(reference, centre, point);
			}
			base.OnControlPointChanged(index, point);
		}
	}
}