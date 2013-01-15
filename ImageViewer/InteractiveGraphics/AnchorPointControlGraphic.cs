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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that controls the single point of an <see cref="IPointGraphic"/>.
	/// </summary>
	[Cloneable]
	public class AnchorPointControlGraphic : ControlPointsGraphic
	{
		[CloneIgnore]
		private bool _suspendSubjectPointChangeEvents = false;

		[CloneCopyReference]
		private CursorToken _moveCursor;

		/// <summary>
		/// Constructs a new <see cref="AnchorPointControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="IPointGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IPointGraphic"/>.</param>
		public AnchorPointControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (IPointGraphic));

			_moveCursor = new CursorToken(CursorToken.SystemCursors.SizeAll);

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Add(this.Subject.Point);
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
		protected AnchorPointControlGraphic(AnchorPointControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		public new IPointGraphic Subject
		{
			get { return base.Subject as IPointGraphic; }
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandChange; }
		}

		/// <summary>
		/// Gets or sets the cursor token to show when the mouse is hovering over the control point.
		/// </summary>
		public CursorToken MoveCursor
		{
			get { return _moveCursor; }
			set { _moveCursor = value; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.PointChanged += OnSubjectPointChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.PointChanged -= OnSubjectPointChanged;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="ControlGraphic.GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		protected override CursorToken GetCursorToken(Point point)
		{
			if (!this.IsTracking && this.Visible && this.HitTest(point))
				return _moveCursor;

			return base.GetCursorToken(point);
		}

		/// <summary>
		/// Captures the current state of this <see cref="AnchorPointControlGraphic"/>.
		/// </summary>
		public override object CreateMemento()
		{
			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				return new PointMemento(this.Subject.Point);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Restores the state of this <see cref="AnchorPointControlGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="AnchorPointControlGraphic.CreateMemento"/>.</param>
		public override void SetMemento(object memento)
		{
			PointMemento pointMemento = memento as PointMemento;
			if (pointMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			_suspendSubjectPointChangeEvents = true;
			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Point = pointMemento.Point;
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
				_suspendSubjectPointChangeEvents = false;
				this.OnSubjectPointChanged(this, EventArgs.Empty);
			}
		}

		private void OnSubjectPointChanged(object sender, EventArgs e)
		{
			if (_suspendSubjectPointChangeEvents)
				return;

			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Point;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected override void OnControlPointChanged(int index, PointF point)
		{
			this.Subject.Point = point;
			base.OnControlPointChanged(index, point);
		}
	}
}