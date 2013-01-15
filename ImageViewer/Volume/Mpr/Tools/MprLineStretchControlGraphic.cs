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
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	/// <summary>
	/// An interactive graphic that controls the end points of an <see cref="ILineSegmentGraphic"/>.
	/// </summary>
	[Cloneable]
	internal sealed class MprLineStretchControlGraphic : ControlPointsGraphic
	{
		public event EventHandler UndoableOperationStart;
		public event EventHandler UndoableOperationStop;
		public event EventHandler UndoableOperationCancel;

		/// <summary>
		/// Constructs a new <see cref="MprLineStretchControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="ILineSegmentGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="ILineSegmentGraphic"/>.</param>
		public MprLineStretchControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ILineSegmentGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Add(this.Subject.Point1);
				base.ControlPoints.Add(this.Subject.Point2);
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
		private MprLineStretchControlGraphic(MprLineStretchControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		public new ILineSegmentGraphic Subject
		{
			get { return base.Subject as ILineSegmentGraphic; }
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandMprReslice; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.Point1Changed += OnSubjectPoint1Changed;
			this.Subject.Point2Changed += OnSubjectPoint2Changed;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.Point1Changed -= OnSubjectPoint1Changed;
			this.Subject.Point2Changed -= OnSubjectPoint2Changed;
			base.Dispose(disposing);
		}

		private void OnSubjectPoint1Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Point1;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		private void OnSubjectPoint2Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[1] = this.Subject.Point2;
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
			if (index == 0)
				this.Subject.Point1 = point;
			else if (index == 1)
				this.Subject.Point2 = point;
			base.OnControlPointChanged(index, point);
		}

		protected override bool Start(IMouseInformation mouseInformation)
		{
			bool result = base.Start(mouseInformation);
			if (result)
				EventsHelper.Fire(this.UndoableOperationStart, this, EventArgs.Empty);
			return result;
		}

		protected override bool Stop(IMouseInformation mouseInformation)
		{
			bool result = base.Stop(mouseInformation);
			EventsHelper.Fire(this.UndoableOperationStop, this, EventArgs.Empty);
			return result;
		}

		protected override void Cancel()
		{
			EventsHelper.Fire(this.UndoableOperationCancel, this, EventArgs.Empty);
			base.Cancel();
		}
	}
}