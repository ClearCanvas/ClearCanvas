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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	/// <summary>
	/// An interactive graphic that allows a subject graphic to be moved.
	/// </summary>
	[Cloneable]
	internal sealed class MprMoveControlGraphic : ControlGraphic
	{
		public event EventHandler UndoableOperationStart;
		public event EventHandler UndoableOperationStop;
		public event EventHandler UndoableOperationCancel;

		[CloneCopyReference]
		private CursorToken _cursor;

		private PointF _anchor = PointF.Empty;

		/// <summary>
		/// Constructs a new instance of <see cref="MprMoveControlGraphic"/>.
		/// </summary>
		/// <param name="subject">The subject graphic.</param>
		public MprMoveControlGraphic(IGraphic subject) : base(subject)
		{
			_cursor = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private MprMoveControlGraphic(MprMoveControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandMprMove; }
		}

		/// <summary>
		/// Gets a point that can be used as a landmark to quantify the difference in position of the controlled
		/// graphic when moved by this <see cref="ControlGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <para>This point has no meaning outside of the same instance of a <see cref="MprMoveControlGraphic"/>.
		/// Specifically, there is no defined relationship between the <see cref="Anchor"/> and the underlying
		/// <see cref="ControlGraphic.Subject"/>. The typical usage is to take the difference between the current
		/// value and a previous reading of the value to determine the position delta between the two readings.</para>
		/// <para>This point is given in either source or destination coordinates depending on the current value
		/// of <see cref="Graphic.CoordinateSystem"/>.</para>
		/// </remarks>
		private PointF Anchor
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Destination)
					return this.SpatialTransform.ConvertToDestination(_anchor);
				return _anchor;
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Destination)
					value = this.SpatialTransform.ConvertToSource(value);
				_anchor = value;
			}
		}

		/// <summary>
		/// Gets or sets the cursor to show when hovering over the graphic.
		/// </summary>
		public CursorToken Cursor
		{
			get { return _cursor; }
			set { _cursor = value; }
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="ControlGraphic.GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		/// <returns></returns>
		protected override CursorToken GetCursorToken(Point point)
		{
			if (this.HitTest(point))
				return this.Cursor;
			return base.GetCursorToken(point);
		}

		/// <summary>
		/// Performs a hit test on the <see cref="CompositeGraphic"/> at given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" any <see cref="Graphic"/>
		/// in the subtree, <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// Calling <see cref="CompositeGraphic.HitTest"/> will recursively call <see cref="CompositeGraphic.HitTest"/> on
		/// <see cref="Graphic"/> objects in the subtree.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			return base.DecoratedGraphic.HitTest(point);
		}

		/// <summary>
		/// Moves the <see cref="CompositeGraphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			this.Anchor += delta;
			this.DecoratedGraphic.Move(delta);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to a mouse button click via <see cref="ControlGraphic.Start"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		protected override bool Start(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				if (this.HitTest(mouseInformation.Location))
				{
					EventsHelper.Fire(this.UndoableOperationStart, this, EventArgs.Empty);
					return true;
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return base.Start(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework tracking mouse input via <see cref="ControlGraphic.Track"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		protected override bool Track(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				if (base.IsTracking)
				{
					this.Move(Vector.CalculatePositionDelta(base.LastTrackedPosition, mouseInformation.Location));
					this.Draw();
					return true;
				}

				if (this.HitTest(mouseInformation.Location))
				{
					return true;
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return base.Track(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response a mouse button release via <see cref="ControlGraphic.Stop"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		protected override bool Stop(IMouseInformation mouseInformation)
		{
			EventsHelper.Fire(this.UndoableOperationStop, this, EventArgs.Empty);
			return base.Stop(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to an attempt to cancel the current operation via <see cref="ControlGraphic.Cancel"/>.
		/// </summary>
		protected override void Cancel()
		{
			EventsHelper.Fire(this.UndoableOperationCancel, this, EventArgs.Empty);
			base.Cancel();
		}
	}
}