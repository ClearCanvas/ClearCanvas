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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class CrosshairGraphic : CompositeGraphic
	{
		private static readonly int _lineLength = 10;

		private readonly LinePrimitive _line1;
		private readonly LinePrimitive _line2;
		private readonly LinePrimitive _line3;
		private readonly LinePrimitive _line4;

		public PointF _anchor;

		public CrosshairGraphic()
		{
			base.Graphics.Add(_line1 = new LinePrimitive());
			base.Graphics.Add(_line2 = new LinePrimitive());
			base.Graphics.Add(_line3 = new LinePrimitive());
			base.Graphics.Add(_line4 = new LinePrimitive());

			this.Color = Color.LimeGreen;
		}

		
		public Color Color
		{
			get { return _line1.Color; }
			set
			{
				_line1.Color = _line2.Color = _line3.Color = _line4.Color = value;
			}
		}

		public PointF Anchor
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _anchor;
				}
				else
				{
					return base.SpatialTransform.ConvertToDestination(_anchor);
				}
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_anchor = value;
				}
				else
				{
					_anchor = base.SpatialTransform.ConvertToSource(value);
				}
			}
		}

		public override void OnDrawing()
		{
			SetCrossHairLines();

			base.OnDrawing();
		}

		private void SetCrossHairLines()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;

			SizeF offset1 = new SizeF(_lineLength + 5F, 0);
			SizeF offset2 = new SizeF(5F, 0);

			PointF anchor = Anchor;

			_line1.Point1 = PointF.Subtract(anchor, offset1);
			_line1.Point2 = PointF.Subtract(anchor, offset2);

			_line2.Point1 = PointF.Add(anchor, offset1);
			_line2.Point2 = PointF.Add(anchor, offset2);

			offset1 = new SizeF(0, _lineLength + 5F);
			offset2 = new SizeF(0, 5F);

			_line3.Point1 = PointF.Subtract(anchor, offset1);
			_line3.Point2 = PointF.Subtract(anchor, offset2);

			_line4.Point1 = PointF.Add(anchor, offset1);
			_line4.Point2 = PointF.Add(anchor, offset2);

			base.ResetCoordinateSystem();
		}
	}
}
