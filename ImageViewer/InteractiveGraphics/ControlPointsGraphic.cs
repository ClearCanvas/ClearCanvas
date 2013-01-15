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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines an <see cref="IControlGraphic"/> that consists of a number of individually adjustable control points.
	/// </summary>
	public interface IControlPointsGraphic : IControlGraphic
	{
		/// <summary>
		/// Occurs when the location of a control point has changed.
		/// </summary>
		event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent;

		/// <summary>
		/// Gets the number of control points in the <see cref="IControlGraphic"/>.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets or sets the location of the specified control point.
		/// </summary>
		/// <param name="index">The zero-based index of the control point.</param>
		PointF this[int index] { get; set; }

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point">The screen coordinates to test for a control point.</param>
		/// <returns>The index of the control point or -1 if the hit test failed for all control points.</returns>
		int HitTestControlPoint(Point point);
	}

	/// <summary>
	/// Abstract base class for implementations of <see cref="IControlPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// The use of the <see cref="ControlPointsGraphic"/> base class is for convenience, as much of the logic behind
	/// the control point graphics, handling user input and command history is implemented by default.
	/// </remarks>
	[Cloneable]
	public abstract partial class ControlPointsGraphic : ControlGraphic, IControlPointsGraphic, IMemorable
	{
		private event EventHandler<ListEventArgs<PointF>> _controlPointChangedEvent;

		[CloneCopyReference]
		private CursorToken _stretchingToken;

		[CloneIgnore]
		private int _trackedControlPoint = -1;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		[CloneIgnore]
		private object _memorableState = null;

		[CloneIgnore]
		private ControlPointGroup _controlPoints;

		private StretchCursorTokenStrategy _stretchCursorTokenStrategy;

		/// <summary>
		/// Constructs a new <see cref="ControlPointsGraphic"/> to control the given subject graphic.
		/// </summary>
		/// <param name="subject">The graphic to control.</param>
		public ControlPointsGraphic(IGraphic subject)
			: base(subject)
		{
			_stretchingToken = new CursorToken(CursorToken.SystemCursors.Cross);

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ControlPointsGraphic(ControlPointsGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Occurs when the location of a control point has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		/// <summary>
		/// Gets or sets the cursor token to use when moving a control point.
		/// </summary>
		public CursorToken StretchingToken
		{
			get { return _stretchingToken; }
			set { _stretchingToken = value; }
		}

		/// <summary>
		/// Gets or sets a strategy for selecting a cursor to show when hovering over a control point.
		/// </summary>
		public StretchCursorTokenStrategy StretchCursorTokenStrategy
		{
			get { return _stretchCursorTokenStrategy; }
			set
			{
				if (_stretchCursorTokenStrategy != value)
				{
					_stretchCursorTokenStrategy = value;

					if (_stretchCursorTokenStrategy != null)
						_stretchCursorTokenStrategy.TargetGraphic = this;
				}
			}
		}

		/// <summary>
		/// Gets the number of control points in the <see cref="IControlGraphic"/>.
		/// </summary>
		public int Count
		{
			get { return _controlPoints.Count; }
		}

		/// <summary>
		/// Gets or sets the location of the specified control point.
		/// </summary>
		/// <param name="index">The zero-based index of the control point.</param>
		public PointF this[int index]
		{
			get { return _controlPoints[index]; }
			set { _controlPoints[index] = value; }
		}

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point">The screen coordinates to test for a control point.</param>
		/// <returns>The index of the control point or -1 if the hit test failed for all control points.</returns>
		public int HitTestControlPoint(Point point)
		{
			return _controlPoints.HitTestControlPoint(point);
		}

		/// <summary>
		/// Gets the collection of control points.
		/// </summary>
		protected ControlPointGroup ControlPoints
		{
			get { return _controlPoints; }
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				_controlPoints.ControlPointChangedEvent -= OnControlPointLocationChanged;
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets or sets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// Setting the <see cref="CompositeGraphic.CoordinateSystem"/> property will recursively set the 
		/// <see cref="CompositeGraphic.CoordinateSystem"/> property for <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </remarks>
		public override sealed CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set { base.CoordinateSystem = value; }
		}

		/// <summary>
		/// Resets the <see cref="CompositeGraphic.CoordinateSystem"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> will reset the <see cref="CompositeGraphic.CoordinateSystem"/>
		/// to what it was before the <see cref="CompositeGraphic.CoordinateSystem"/> was last set.
		/// </para>
		/// <para>
		/// Calling <see cref="CompositeGraphic.ResetCoordinateSystem"/> will recursively call
		/// <see cref="CompositeGraphic.ResetCoordinateSystem"/> on <i>all</i> <see cref="Graphic"/> 
		/// objects in the subtree.
		/// </para>
		/// </remarks>
		public override sealed void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();
		}

		/// <summary>
		/// Temporarily suspends notification of control point change events.
		/// </summary>
		protected void SuspendControlPointEvents()
		{
			_bypassControlPointChangedEvent = true;
		}

		/// <summary>
		/// Resumes notification of control point change events.
		/// </summary>
		protected void ResumeControlPointEvents()
		{
			_bypassControlPointChangedEvent = false;
		}

		private void OnControlPointLocationChanged(object sender, ListEventArgs<PointF> e)
		{
			if (!_bypassControlPointChangedEvent)
			{
				this.OnControlPointChanged(e.Index, e.Item);
				EventsHelper.Fire(_controlPointChangedEvent, this, e);
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <remarks>
		/// The <paramref name="point"/> parameter is given in the graphic's current <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected virtual void OnControlPointChanged(int index, PointF point) {}

		/// <summary>
		/// Computes a constrained control point location (in destination coordinates) given the location to which the cursor moved after starting drag on the control point.
		/// </summary>
		/// <param name="controlPointIndex">The index of the control point being dragged.</param>
		/// <param name="cursorLocation">The location to which the cursor moved.</param>
		/// <returns>The constrained control point location.</returns>
		/// <remarks>
		/// The default implementation is unconstrained. Subclasses can override this method to constrain the possible locations
		/// of a control point to a particular locus or to maintain some property such as aspect ratio.
		/// </remarks>
		protected virtual PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation)
		{
			return cursorLocation;
		}

		/// <summary>
		/// Captures the current state of this <see cref="ControlPointsGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Any overriding implementation of <see cref="ControlPointsGraphic.CreateMemento"/> should return an
		/// object containing enough state information so that, when <see cref="ControlPointsGraphic.SetMemento"/> is called,
		/// the object can be restored to the original state.
		/// </para>
		/// <para>The default implementation returns null.</para>
		/// </remarks>
		public virtual object CreateMemento()
		{
			return null;
		}

		/// <summary>
		/// Restores the state of this <see cref="ControlPointsGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="ControlPointsGraphic.CreateMemento"/>.</param>
		/// <remarks>
		/// <para>
		/// Any overriding implementation of <see cref="ControlPointsGraphic.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="ControlPointsGraphic.CreateMemento"/>.
		/// </para>
		/// <para>The default implementation does nothing.</para>
		/// </remarks>
		public virtual void SetMemento(object memento) {}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="ControlGraphic.GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		/// <returns></returns>
		protected override CursorToken GetCursorToken(Point point)
		{
			if (this.IsTracking)
				return this.StretchingToken;

			if (this.Visible && this.HitTest(point))
			{
				if (this.StretchCursorTokenStrategy != null)
					return StretchCursorTokenStrategy.GetCursorToken(point);
			}

			return base.GetCursorToken(point);
		}

		private void Initialize()
		{
			if (_stretchCursorTokenStrategy == null)
				_stretchCursorTokenStrategy = new CompassStretchCursorTokenStrategy();

			if (_controlPoints == null)
				base.Graphics.Add( _controlPoints = new ControlPointGroup());

			_stretchCursorTokenStrategy.TargetGraphic = this;
			_controlPoints.ControlPointChangedEvent += OnControlPointLocationChanged;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_controlPoints = (ControlPointGroup)CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is ControlPointGroup; });

			Initialize();
		}

		/// <summary>
		/// Called when the <see cref="ControlGraphic.Color"/> property changes.
		/// </summary>
		protected override void OnColorChanged() 
		{
			_controlPoints.Color = base.Color;
			base.OnColorChanged();
		}

		/// <summary>
		/// Called when the <see cref="ControlGraphic.Show"/> property changes.
		/// </summary>
		protected override void OnShowChanged()
		{
			_controlPoints.Visible = base.Show;
			base.OnShowChanged();
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
				_trackedControlPoint = this.ControlPoints.HitTestControlPoint(mouseInformation.Location);
				if (_trackedControlPoint >= 0)
				{
					_memorableState = this.CreateMemento();
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
				ControlPoint cpGraphic = null;
				if (_trackedControlPoint >= 0 && _trackedControlPoint < this.ControlPoints.Count)
					cpGraphic = (ControlPoint)this.ControlPoints.Graphics[_trackedControlPoint];

				if (base.IsTracking && cpGraphic != null)
				{
					cpGraphic.Location = this.ConstrainControlPointLocation(_trackedControlPoint, mouseInformation.Location);
					cpGraphic.Draw();
					return true;
				}

				if(this.ControlPoints.HitTest(mouseInformation.Location))
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
			if (_memorableState != null)
			{
				AddToCommandHistory(this, _memorableState, this.CreateMemento());
				_memorableState = null;
			}

			_trackedControlPoint = -1;
			return base.Stop(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to an attempt to cancel the current operation via <see cref="ControlGraphic.Cancel"/>.
		/// </summary>
		protected override void Cancel()
		{
			_memorableState = null;
			_trackedControlPoint = -1;
			base.Cancel();
		}
	}
}