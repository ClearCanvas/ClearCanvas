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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	partial class ControlPointsGraphic
	{
		/// <summary>
		/// A group of <see cref="ControlPoint"/>s. 
		/// </summary>
		[Cloneable(true)]
		protected class ControlPointGroup : CompositeGraphic
		{
			private Color _color = Color.Yellow;
			private event EventHandler<ListEventArgs<PointF>> _controlPointChangedEvent;

			/// <summary>
			/// Initializes a new instance of <see cref="ControlPointGroup"/>.
			/// </summary>
			public ControlPointGroup() {}

			/// <summary>
			/// Occurs when the location of a <see cref="ControlPoint"/> has changed.
			/// </summary>
			public event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent
			{
				add { _controlPointChangedEvent += value; }
				remove { _controlPointChangedEvent -= value; }
			}

			/// <summary>
			/// Gets or sets the colour of the <see cref="ControlPointGroup"/>.
			/// </summary>
			public Color Color
			{
				get { return _color; }
				set
				{
					_color = value;

					foreach (ControlPoint controlPoint in this.Graphics)
						controlPoint.Color = _color;
				}
			}

			/// <summary>
			/// Returns the number of <see cref="ControlPoint"/>s in the
			/// <see cref="ControlPointGroup"/>.
			/// </summary>
			public int Count
			{
				get { return this.Graphics.Count; }
			}

			/// <summary>
			/// Gets or sets the location of the specified <see cref="ControlPoint"/>.
			/// </summary>
			/// <param name="index">The zero-based index of the <see cref="ControlPoint"/>.</param>
			/// <returns></returns>
			public PointF this[int index]
			{
				get { return ((ControlPoint) this.Graphics[index]).Location; }
				set { ((ControlPoint) this.Graphics[index]).Location = value; }
			}

			/// <summary>
			/// Adds a new <see cref="ControlPoint"/> to the
			/// <see cref="ControlPointGroup"/>.
			/// </summary>
			/// <param name="point"></param>
			public void Add(PointF point)
			{
				ControlPoint controlPoint = new ControlPoint();
				this.Graphics.Add(controlPoint);
				controlPoint.Location = point;
				controlPoint.Color = this.Color;
				controlPoint.LocationChanged += OnControlPointChanged;
			}

			/// <summary>
			/// Removes a <see cref="ControlPoint"/> from the group.
			/// </summary>
			public void RemoveAt(int index)
			{
				if (index < Count)
				{
					ControlPoint point = base.Graphics[index] as ControlPoint;
					if (point != null)
						point.LocationChanged -= OnControlPointChanged;

					base.Graphics.RemoveAt(index);
				}
			}

			/// <summary>
			/// Removes all <see cref="ControlPoint"/>s from the <see cref="ControlPointGroup"/>.
			/// </summary>
			public void Clear()
			{
				for (int i = this.Count - 1; i >= 0; --i)
					RemoveAt(i);
			}

			/// <summary>
			/// Performs a hit test on the <see cref="ControlPoint"/>s
			/// in the <see cref="ControlPointGroup"/>.
			/// </summary>
			/// <param name="point"></param>
			/// <returns></returns>
			public override bool HitTest(Point point)
			{
				foreach (ControlPoint controlPoint in this.Graphics)
				{
					if (controlPoint != null)
						if (controlPoint.HitTest(point))
							return true;
				}

				return false;
			}

			/// <summary>
			/// Performs a hit test on each <see cref="ControlPoint"/> and returns
			/// the index of the <see cref="ControlPoint"/> for which the test is true.
			/// </summary>
			/// <param name="point"></param>
			/// <returns>The index of the <see cref="ControlPoint"/> or
			/// -1 if the hit test failed for all <see cref="ControlPoint"/>s.</returns>
			public int HitTestControlPoint(Point point)
			{
				int controlPointIndex = 0;

				// Check if mouse is over a control point
				foreach (ControlPoint controlPoint in this.Graphics)
				{
					if (controlPoint.HitTest(point))
						return controlPointIndex;

					controlPointIndex++;
				}

				return -1;
			}

			/// <summary>
			/// Releases all resources used by this <see cref="ControlPointGroup"/>.
			/// </summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					foreach (ControlPoint controlPoint in this.Graphics)
						controlPoint.LocationChanged -= OnControlPointChanged;
				}

				base.Dispose(disposing);
			}

			private void OnControlPointChanged(object sender, EventArgs e)
			{
				ControlPoint controlPoint = (ControlPoint) sender;
				EventsHelper.Fire(_controlPointChangedEvent, this, new ListEventArgs<PointF>(controlPoint.Location, this.Graphics.IndexOf(controlPoint)));
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				foreach (ControlPoint controlPoint in this.Graphics)
					controlPoint.LocationChanged += OnControlPointChanged;
			}
		}
	}
}