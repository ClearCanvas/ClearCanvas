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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	#region ReferenceLineCompositeGraphic class

	internal class ReferenceLineCompositeGraphic : CompositeGraphic
	{
		private bool _dirty = false;

		public ReferenceLineCompositeGraphic()
		{
		}

		public bool Dirty
		{
			get { return _dirty; }
		}

		public override bool Visible
		{
			//always visible so the dirty flag gets reset on draw
			get { return true; }
			set
			{
			}
		}

		public ReferenceLineGraphic this[int index]
		{
			get
			{
				for (int i = base.Graphics.Count; i <= index; ++i)
				{
					ReferenceLineGraphic newGraphic = new ReferenceLineGraphic();
					newGraphic.Changed += OnChildChanged;
					base.Graphics.Add(newGraphic);
				}

				return (ReferenceLineGraphic)base.Graphics[index];
			}
		}

		public void HideAllReferenceLines()
		{
			foreach (ReferenceLineGraphic child in base.Graphics)
				child.Visible = false;
		}

		public override void OnDrawing()
		{
			base.OnDrawing();
			_dirty = false;
		}

		private void OnChildChanged(object sender, EventArgs e)
		{
			_dirty = true;
		}
	}

	#endregion

	#region ReferenceLineGraphic class

	internal class ReferenceLineGraphic : CompositeGraphic
	{
		private readonly LinePrimitive _line;
		private readonly InvariantTextPrimitive _text;
		private PointF _point1;
		private PointF _point2;
		public event EventHandler _changed;

		public ReferenceLineGraphic()
		{
			base.Graphics.Add(_text = new InvariantTextPrimitive());
			base.Graphics.Add(_line = new LinePrimitive());

			_text.BoundingBoxChanged += OnTextBoundingBoxChanged;
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			KeepTextInsideClientRectangle();
		}

		private void KeepTextInsideClientRectangle()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			PointF startPoint, endPoint;
			float lengthOfLineThroughTextBox;
			if (!GetTextBoxAdjustmentParameters(out startPoint, out endPoint, out lengthOfLineThroughTextBox))
			{
				_line.Point1 = Point1;
				_line.Point2 = Point2;
				_text.Location = Point1;

				this.ResetCoordinateSystem();
				return;
			}

			Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

			float ratioLengthOfLineThroughTextBox = lengthOfLineThroughTextBox / lineDirection.Magnitude;

			SizeF textEdgeOffset = new SizeF(ratioLengthOfLineThroughTextBox * lineDirection.X, ratioLengthOfLineThroughTextBox * lineDirection.Y);

			SizeF textAnchorPointOffset = new SizeF(textEdgeOffset.Width / 2F, textEdgeOffset.Height / 2F);

			Vector3D lineUnit = lineDirection.Normalize();

			// extend the endpoint of the line by the distance to the outside text edge.
			endPoint = PointF.Add(endPoint, textEdgeOffset);
			// add an additional 5 pixel offset so we don't push back as far as the start point.
			endPoint = PointF.Add(endPoint, new SizeF(5F * lineUnit.X, 5F * lineUnit.Y));

			SizeF clientEdgeOffset = Size.Empty;

			// find the intersection of the extended line segment and either of the left or bottom client edge.
			PointF? intersectionPoint = GetClientRightOrBottomEdgeIntersectionPoint(startPoint, endPoint);
			if (intersectionPoint != null)
			{
				Vector3D clientEdgeOffsetVector = new Vector3D(endPoint.X - intersectionPoint.Value.X, endPoint.Y - intersectionPoint.Value.Y, 0);
				//don't allow the text to be pushed back past the start point.
				if (clientEdgeOffsetVector.Magnitude > lineDirection.Magnitude)
					clientEdgeOffsetVector = lineDirection;

				clientEdgeOffset = new SizeF(clientEdgeOffsetVector.X, clientEdgeOffsetVector.Y);
			}
			
			_line.Point1 = startPoint;

			// offset by the distance from the extended endpoint to the client rectangle edge.
			endPoint = PointF.Subtract(endPoint, clientEdgeOffset);
			// offset again by half the distance necessary to keep the text box inside the client rectangle
			endPoint = PointF.Subtract(endPoint, textAnchorPointOffset);

			// this aligns the text edge with the client edge in the case where the line intersects the client edge.
			_text.Location = endPoint;
			
			// offset the line by half again the distance necessary to keep the text box inside the client rectangle.
			_line.Point2 = PointF.Subtract(endPoint, textAnchorPointOffset);

			this.ResetCoordinateSystem();
		}

		private bool GetTextBoxAdjustmentParameters(out PointF startPoint, out PointF endPoint, out float lengthOfLineThroughTextBox)
		{
			startPoint = Point1;
			endPoint = Point2;
			lengthOfLineThroughTextBox = 0;

			Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

			if (Vector3D.AreEqual(lineDirection, Vector3D.Null))
				return false;

			Vector3D lineUnit = lineDirection.Normalize();

			Vector3D xUnit = new Vector3D(1F, 0, 0);
			Vector3D yUnit = new Vector3D(0, 1F, 0);

			float cosThetaX = Math.Abs(xUnit.Dot(lineUnit));
			float cosThetaY = Math.Abs(yUnit.Dot(lineUnit));

			float textWidth = _text.BoundingBox.Width;
			float textHeight = _text.BoundingBox.Height;

			if (cosThetaX >= cosThetaY)
			{
				// the distance along the line to where we want the outside right edge of the text to be.
				lengthOfLineThroughTextBox = cosThetaX*textWidth;
				if (lineDirection.X < 0)
				{
					startPoint = Point2;
					endPoint = Point1;
				}
			}
			else
			{
				// the distance along the line to where we want the outside bottom edge of the text to be.
				lengthOfLineThroughTextBox = cosThetaY*textHeight;
				if (lineDirection.Y < 0)
				{
					startPoint = Point2;
					endPoint = Point1;
				}
			}

			return true;
		}

		private PointF? GetClientRightOrBottomEdgeIntersectionPoint(PointF lineSegmentStartPoint, PointF lineSegmentEndPoint)
		{
			Rectangle clientRectangle = base.ParentPresentationImage.ClientRectangle;

			PointF clientTopRight = new PointF(clientRectangle.Right, clientRectangle.Top);
			PointF clientBottomLeft = new PointF(clientRectangle.Left, clientRectangle.Bottom);
			PointF clientBottomRight = new PointF(clientRectangle.Right, clientRectangle.Bottom);

			PointF intersectionPoint;
			if (Vector.IntersectLineSegments(lineSegmentStartPoint, lineSegmentEndPoint, clientTopRight, clientBottomRight, out intersectionPoint))
			{
				return intersectionPoint;
			}
			else if (Vector.IntersectLineSegments(lineSegmentStartPoint, lineSegmentEndPoint, clientBottomLeft, clientBottomRight, out intersectionPoint))
			{
				return intersectionPoint;
			}
			else
			{
				return null;
			}
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				if (base.Visible == value)
					return;

				base.Visible = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}
		}
		public PointF Point1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point1;
				else
					return base.SpatialTransform.ConvertToDestination(_point1);
			}
			set
			{
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value == _point1)
					return;

				_point1 = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}
		}

		public PointF Point2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point2;
				else
					return base.SpatialTransform.ConvertToDestination(_point2);
			}
			set
			{
				
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value == _point2)
					return;

				_point2 = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}
		}

		public string Text
		{
			get { return _text.Text; }
			set
			{
				if (value == _text.Text)
					return;

				_text.Text = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}	
		}

		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		public override void OnDrawing()
		{
			OnTextBoundingBoxChanged(this, null);
			base.OnDrawing();
		}
	}

	#endregion
}
