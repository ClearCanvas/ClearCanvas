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
	/// <summary>
	/// An interactive graphic that controls resizing of an <see cref="IBoundableGraphic"/>.
	/// </summary>
	[Cloneable]
	public sealed class BoundableResizeControlGraphic : ControlPointsGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;

		private float? _fixedAspectRatio = null;

		/// <summary>
		/// Constructs a new <see cref="BoundableResizeControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="IBoundableGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IBoundableGraphic"/>.</param>
		public BoundableResizeControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof(IBoundableGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rf = this.Subject.Rectangle;
				base.ControlPoints.Add(new PointF(rf.Left, rf.Top));
				base.ControlPoints.Add(new PointF(rf.Right, rf.Top));
				base.ControlPoints.Add(new PointF(rf.Left, rf.Bottom));
				base.ControlPoints.Add(new PointF(rf.Right, rf.Bottom));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		/// <summary>
		/// Constructs a new <see cref="BoundableResizeControlGraphic"/>.
		/// </summary>
		/// <param name="aspectRatio">The width to height aspect ratio that constrains the movement of the resize control points.</param>
		/// <param name="subject">An <see cref="IBoundableGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IBoundableGraphic"/>.</param>
		public BoundableResizeControlGraphic(float aspectRatio, IGraphic subject)
			: this(subject)
		{
			Platform.CheckPositive(aspectRatio, "aspectRatio");
			_fixedAspectRatio = aspectRatio;
		}

		/// <summary>
		/// Constructs a new <see cref="BoundableResizeControlGraphic"/>.
		/// </summary>
		/// <param name="fixedAspectRatio">The width to height aspect ratio that constrains the movement of the resize control points, or null if the movement should not be constrained.</param>
		/// <param name="subject">An <see cref="IBoundableGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IBoundableGraphic"/>.</param>
		public BoundableResizeControlGraphic(float? fixedAspectRatio, IGraphic subject)
			: this(subject)
		{
			if (fixedAspectRatio.HasValue)
				Platform.CheckPositive(fixedAspectRatio.Value, "fixedAspectRatio");
			_fixedAspectRatio = fixedAspectRatio;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private BoundableResizeControlGraphic(BoundableResizeControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		public new IBoundableGraphic Subject
		{
			get { return base.Subject as IBoundableGraphic; }
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandResize; }
		}

		private void Initialize()
		{
			this.Subject.BottomRightChanged += OnSubjectBottomRightChanged;
			this.Subject.TopLeftChanged += OnSubjectTopLeftChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.BottomRightChanged -= OnSubjectBottomRightChanged;
			this.Subject.TopLeftChanged -= OnSubjectTopLeftChanged;

			base.Dispose(disposing);
		}

		#region Fixed Ratio Resizing

		/// <summary>
		/// Gets or sets the width to height aspect ratio that constrains the movement of
		/// the resize control points, or null if the movement should not be constrained.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the aspect ratio is zero or negative.</exception>
		public float? FixedAspectRatio
		{
			get { return _fixedAspectRatio; }
			set
			{
				if (value.HasValue)
					Platform.CheckPositive(value.Value, "FixedAspectRatio");
				_fixedAspectRatio = value;
			}
		}

		/// <summary>
		/// Computes a constrained control point location (in destination coordinates) given the location to which the cursor moved after starting drag on the control point.
		/// </summary>
		/// <param name="controlPointIndex">The index of the control point being dragged.</param>
		/// <param name="cursorLocation">The location to which the cursor moved.</param>
		/// <returns>The constrained control point location.</returns>
		protected override PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation) {
			if (!_fixedAspectRatio.HasValue)
				return cursorLocation;

			float targetRatio = _fixedAspectRatio.Value;
			PointF result = cursorLocation;
			RectangleF originalRect;

			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				originalRect = this.Subject.Rectangle;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			float rectLeft = originalRect.Left;
			float rectRight = originalRect.Right;
			float rectTop = originalRect.Top;
			float rectBottom = originalRect.Bottom;

			switch (controlPointIndex)
			{
				case _topLeft:
					rectLeft = cursorLocation.X;
					rectTop = cursorLocation.Y;
					break;
				case _bottomRight:
					rectRight = cursorLocation.X;
					rectBottom = cursorLocation.Y;
					break;
				case _topRight:
					rectRight = cursorLocation.X;
					rectTop = cursorLocation.Y;
					break;
				case _bottomLeft:
					rectLeft = cursorLocation.X;
					rectBottom = cursorLocation.Y;
					break;
			}

			if(!FloatComparer.AreEqual(rectLeft, rectRight) && !FloatComparer.AreEqual(rectTop, rectBottom))
			{
				float cursorRatio = (rectRight - rectLeft)/(rectBottom - rectTop);
				int ratioSgn = Math.Sign(cursorRatio);
				cursorRatio = Math.Abs(cursorRatio);
				if(!FloatComparer.AreEqual(cursorRatio, targetRatio))
				{
					SizeF offset;
					if(cursorRatio > targetRatio)
					{
						float height = rectBottom - rectTop;
						float width = height*targetRatio;
						float delta = rectRight - rectLeft - ratioSgn*width;

						if (controlPointIndex == _topLeft || controlPointIndex == _bottomLeft)
							offset = new SizeF(delta, 0);
						else
							offset = new SizeF(-delta, 0);
					}
					else
					{
						float width = rectRight - rectLeft;
						float height = width / targetRatio;
						float delta = rectBottom - rectTop - ratioSgn*height;

						if (controlPointIndex == _topLeft || controlPointIndex == _topRight)
							offset = new SizeF(0, delta);
						else
							offset = new SizeF(0, -delta);
					}
					result += offset;
				}
			}
			return result;
		}

		#endregion

		/// <summary>
		/// Captures the current state of this <see cref="BoundableResizeControlGraphic"/>.
		/// </summary>
		public override object CreateMemento()
		{
			PointsMemento pointsMemento = new PointsMemento();

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointsMemento.Add(this.Subject.TopLeft);
				pointsMemento.Add(this.Subject.BottomRight);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointsMemento;
		}

		/// <summary>
		/// Restores the state of this <see cref="BoundableResizeControlGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="BoundableResizeControlGraphic.CreateMemento"/>.</param>
		public override void SetMemento(object memento)
		{
			PointsMemento pointsMemento = memento as PointsMemento;
			if (pointsMemento == null || pointsMemento.Count != 2)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.TopLeft = pointsMemento[0];
				this.Subject.BottomRight = pointsMemento[1];
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected override void OnControlPointChanged(int index, PointF point)
		{
			// this operation must be performed in source coodinates because the definition of top-left/bottom-left/top-right/bottom-right controls are relative to image not tile!
			var sourcePoint = CoordinateSystem == CoordinateSystem.Destination ? SpatialTransform.ConvertToSource(point) : point;
			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				var subject = Subject;
				var rect = subject.Rectangle;
				switch (index)
				{
					case _topLeft:
						subject.TopLeft = sourcePoint;
						break;
					case _bottomRight:
						subject.BottomRight = sourcePoint;
						break;
					case _topRight:
						subject.TopLeft = new PointF(rect.Left, sourcePoint.Y);
						subject.BottomRight = new PointF(sourcePoint.X, rect.Bottom);
						break;
					case _bottomLeft:
						subject.TopLeft = new PointF(sourcePoint.X, rect.Top);
						subject.BottomRight = new PointF(rect.Right, sourcePoint.Y);
						break;
				}
			}
			finally
			{
				ResetCoordinateSystem();
			}

			base.OnControlPointChanged(index, point);
		}

		private void OnSubjectTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rect = this.Subject.Rectangle;
				this[_topRight] = new PointF(rect.Right, rect.Top);
				this[_topLeft] = new PointF(rect.Left, rect.Top);
				this[_bottomLeft] = new PointF(rect.Left, rect.Bottom);
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		private void OnSubjectBottomRightChanged(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rect = this.Subject.Rectangle;
				this[_topRight] = new PointF(rect.Right, rect.Top);
				this[_bottomRight] = new PointF(rect.Right, rect.Bottom);
				this[_bottomLeft] = new PointF(rect.Left, rect.Bottom);
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}
	}
}