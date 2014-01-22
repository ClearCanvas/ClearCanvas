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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arrow graphic.
	/// </summary>
	[Cloneable]
	public class ArrowGraphic : CompositeGraphic, ILineSegmentGraphic
	{
		[CloneIgnore]
		private LinePrimitive _shaft;

		[CloneIgnore]
		private InvariantArrowheadGraphic _arrowhead;

		private event EventHandler<PointChangedEventArgs> _startPointChanged;
		private event EventHandler<PointChangedEventArgs> _endPointChanged;

		private bool _visible = true;
		private bool _showArrowhead = true;
		private bool _enablePointChangeEvents = true;

		/// <summary>
		/// Constructs an arrow graphic.
		/// </summary>
		public ArrowGraphic()
		{
			Initialize();
		}

		/// <summary>
		/// Constructs a arrow graphic with an optional arrowhead.
		/// </summary>
		/// <param name="showArrow">A value indicating if the arrowhead should be shown.</param>
		public ArrowGraphic(bool showArrow) : this()
		{
			_showArrowhead = showArrow;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ArrowGraphic(ArrowGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			if (_shaft == null)
			{
				base.Graphics.Add(_shaft = new LinePrimitive());
			}

			if (_arrowhead == null)
			{
				base.Graphics.Add(_arrowhead = new InvariantArrowheadGraphic());
				_arrowhead.Visible = _showArrowhead;
			}

			_shaft.Point1Changed += OnShaftPoint1Changed;
			_shaft.Point2Changed += OnShaftPoint2Changed;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_shaft = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is LinePrimitive; }) as LinePrimitive;
			_arrowhead = CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is InvariantArrowheadGraphic; }) as InvariantArrowheadGraphic;

			Initialize();
		}

		/// <summary>
		/// Gets or sets the tail endpoint of the arrow.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF StartPoint
		{
			get { return _shaft.Point1; }
			set
			{
				_shaft.Point1 = value;
				UpdateArrowheadAngle();
			}
		}

		/// <summary>
		/// Gets or sets the tip endpoint of the arrow.
		/// </summary>
		/// <remarks>
		/// <para>This point is the location to which the arrow points.</para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.</para>
		/// </remarks>
		public PointF EndPoint
		{
			get { return _shaft.Point2; }
			set
			{
				_shaft.Point2 = _arrowhead.Point = value;
				UpdateArrowheadAngle();
			}
		}

		/// <summary>
		/// Event fired when the value of <see cref="StartPoint"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> StartPointChanged
		{
			add { _startPointChanged += value; }
			remove { _startPointChanged -= value; }
		}

		/// <summary>
		/// Event fired when the value of <see cref="EndPoint"/> changes.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> EndPointChanged
		{
			add { _endPointChanged += value; }
			remove { _endPointChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the line style of the arrowhead.
		/// </summary>
		public LineStyle ArrowheadLineStyle
		{
			get { return _arrowhead.LineStyle; }
			set { _arrowhead.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the arrowhead should be visible.
		/// </summary>
		public bool ShowArrowhead
		{
			get { return _showArrowhead; }
			set
			{
				if (_showArrowhead != value)
				{
					_showArrowhead = value;
					UpdateArrowheadVisibility();
				}
			}
		}

		/// <summary>
		/// Gets or sets the colour of the arrow.
		/// </summary>
		public Color Color
		{
			get { return _shaft.Color; }
			set { _shaft.Color = _arrowhead.Color = value; }
		}

		/// <summary>
		/// Gets or sets the line style of the shaft of the arrow.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _shaft.LineStyle; }
			set { _shaft.LineStyle = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the arrow is visible.
		/// </summary>
		public override bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = _shaft.Visible = value;
				UpdateArrowheadVisibility();
			}
		}

		/// <summary>
		/// Gets or sets the tail endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point1
		{
			get { return this.StartPoint; }
			set { this.StartPoint = value; }
		}

		/// <summary>
		/// Gets or sets the tip endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF ILineSegmentGraphic.Point2
		{
			get { return this.EndPoint; }
			set { this.EndPoint = value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point1Changed
		{
			add { this.StartPointChanged += value; }
			remove { this.StartPointChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ILineSegmentGraphic.Point2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point2Changed
		{
			add { this.EndPointChanged += value; }
			remove { this.EndPointChanged -= value; }
		}

		/// <summary>
		/// Moves the <see cref="ArrowGraphic"/> by a specified delta.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.
		/// </remarks>
		/// <param name="delta">The offset by which the graphic is to be moved.</param>
		public override void Move(SizeF delta)
		{
			_enablePointChangeEvents = false;
			try
			{
				base.Move(delta);
				
				// force realignment the arrowhead
				_arrowhead.Point = _shaft.Point2;
				this.UpdateArrowheadAngle();
			}
			finally
			{
				_enablePointChangeEvents = true;
			}

			// trigger events
			_shaft.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.OnShaftPoint1Changed(this, new PointChangedEventArgs(_shaft.Point1, CoordinateSystem.Source));
				this.OnShaftPoint2Changed(this, new PointChangedEventArgs(_shaft.Point2, CoordinateSystem.Source));
			}
			finally
			{
				_shaft.ResetCoordinateSystem();
			}
		}

		private void UpdateArrowheadAngle()
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// the arrowhead is invariant, but the angle part isn't! must be computed with source coordinates!
				_arrowhead.Angle = (int)Vector.SubtendedAngle(_shaft.Point2, _shaft.Point1, _shaft.Point1 + new SizeF(1, 0));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
			UpdateArrowheadVisibility();
		}

		private void UpdateArrowheadVisibility()
		{
			_shaft.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				// if arrowhead option is true and the graphic is visible, only show arrowhead if line is long enough!
				if (_showArrowhead && _visible)
					_arrowhead.Visible = Vector.Distance(_shaft.Point1, _shaft.Point2) > _arrowhead.Length;
				else
					_arrowhead.Visible = false;
			}
			finally
			{
				_shaft.ResetCoordinateSystem();
			}
		}

		private void OnShaftPoint1Changed(object sender, PointChangedEventArgs e)
		{
			if (_enablePointChangeEvents)
				EventsHelper.Fire(_startPointChanged, this, new PointChangedEventArgs(e.Point, e.CoordinateSystem));
		}

		private void OnShaftPoint2Changed(object sender, PointChangedEventArgs e)
		{
			if (_enablePointChangeEvents)
				EventsHelper.Fire(_endPointChanged, this, new PointChangedEventArgs(e.Point, e.CoordinateSystem));
		}
	}
}