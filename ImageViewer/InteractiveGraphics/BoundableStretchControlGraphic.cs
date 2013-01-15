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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that controls stretching of an <see cref="IBoundableGraphic"/>.
	/// </summary>
	[Cloneable]
	public sealed class BoundableStretchControlGraphic : ControlPointsGraphic
	{
		private const int _top = 0;
		private const int _bottom = 1;
		private const int _left = 2;
		private const int _right = 3;

		/// <summary>
		/// Constructs a new <see cref="BoundableStretchControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="IBoundableGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IBoundableGraphic"/>.</param>
		public BoundableStretchControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof(IBoundableGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rf = this.Subject.Rectangle;
				float halfWidth = rf.Width/2;
				float halfHeight = rf.Height/2;
				base.ControlPoints.Add(new PointF(rf.Left + halfWidth, rf.Top));
				base.ControlPoints.Add(new PointF(rf.Left + halfWidth, rf.Bottom));
				base.ControlPoints.Add(new PointF(rf.Left, rf.Top + halfHeight));
				base.ControlPoints.Add(new PointF(rf.Right, rf.Top + halfHeight));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private BoundableStretchControlGraphic(BoundableStretchControlGraphic source, ICloningContext context)
			: base(source, context)
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
			get { return SR.CommandStretch; }
		}

		private void Initialize()
		{
			this.Subject.BottomRightChanged += OnSubjectChanged;
			this.Subject.TopLeftChanged += OnSubjectChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.BottomRightChanged -= OnSubjectChanged;
			this.Subject.TopLeftChanged -= OnSubjectChanged;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Captures the current state of this <see cref="BoundableStretchControlGraphic"/>.
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
		/// Restores the state of this <see cref="BoundableStretchControlGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="BoundableStretchControlGraphic.CreateMemento"/>.</param>
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

		protected override PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation)
		{
			// this operation must be performed in source coodinates because the definition of top/left/right/bottom controls are relative to image not tile!
			var sourcePoint = SpatialTransform.ConvertToSource(cursorLocation);
			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				var rect = Subject.Rectangle;
				switch (controlPointIndex)
				{
					case _top:
					case _bottom:
						return SpatialTransform.ConvertToDestination(new PointF(rect.Left + rect.Width/2, sourcePoint.Y));
					case _left:
					case _right:
						return SpatialTransform.ConvertToDestination(new PointF(sourcePoint.X, rect.Top + rect.Height/2));
				}
			}
			finally
			{
				ResetCoordinateSystem();
			}

			return base.ConstrainControlPointLocation(controlPointIndex, cursorLocation);
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected override void OnControlPointChanged(int index, PointF point)
		{
			// this operation must be performed in source coodinates because the definition of top/left/right/bottom controls are relative to image not tile!
			var sourcePoint = CoordinateSystem == CoordinateSystem.Destination ? SpatialTransform.ConvertToSource(point) : point;
			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				var subject = Subject;
				var rect = subject.Rectangle;
				switch (index)
				{
					case _top:
						subject.TopLeft = new PointF(rect.Left, sourcePoint.Y);
						break;
					case _bottom:
						subject.BottomRight = new PointF(rect.Right, sourcePoint.Y);
						break;
					case _left:
						subject.TopLeft = new PointF(sourcePoint.X, rect.Top);
						break;
					case _right:
						subject.BottomRight = new PointF(sourcePoint.X, rect.Bottom);
						break;
				}
			}
			finally
			{
				ResetCoordinateSystem();
			}

			base.OnControlPointChanged(index, point);
		}

		private void OnSubjectChanged(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rect = this.Subject.Rectangle;
				float halfWidth = rect.Width / 2;
				float halfHeight = rect.Height / 2;
				this[_top] = new PointF(rect.Left + halfWidth, rect.Top);
				this[_bottom] = new PointF(rect.Left + halfWidth, rect.Bottom);
				this[_left] = new PointF(rect.Left, rect.Top + halfHeight);
				this[_right] = new PointF(rect.Right, rect.Top + halfHeight);
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}
	}
}