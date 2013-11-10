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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Interactive builder class that interprets mouse clicks as ordered
	/// vertices to setup a closed <see cref="IPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// This builder takes input until the the maximum number of
	/// vertices is reached, the user clicks near the first point
	/// or the user double clicks anywhere, after which the graphic is complete
	/// and control is released. A visual cue is shown when the cursor is close
	/// enough to the first point to snap and close the polygon.
	/// </remarks>
	public class InteractivePolygonGraphicBuilder : InteractiveGraphicBuilder
	{
		private readonly int _maximumVertices;
		private float _snapRadius = 15f;
		private int _numberOfPointsAnchored = 0;

		private SnapPointGraphic _snapPoint;

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractivePolygonGraphicBuilder(IPointsGraphic pointsGraphic)
			: this(int.MaxValue, pointsGraphic) {}

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="maximumVertices">The maximum number of vertices to accept.</param>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maximumVertices"/> is less than 3.</exception>
		public InteractivePolygonGraphicBuilder(int maximumVertices, IPointsGraphic pointsGraphic)
			: base(pointsGraphic)
		{
			Platform.CheckArgumentRange(maximumVertices, 3, int.MaxValue, "maximumVertices");
			_maximumVertices = maximumVertices;
		}

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IPointsGraphic Graphic
		{
			get { return (IPointsGraphic) base.Graphic; }
		}

		/// <summary>
		/// Gets or the sets the radius of the snap-to feature in destination pixels.
		/// </summary>
		public float SnapRadius
		{
			get { return _snapRadius; }
			set { _snapRadius = value; }
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public override void Reset()
		{
			InstallSnapPointGraphic(false);
			_numberOfPointsAnchored = 0;
			base.Reset();
		}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected override void Rollback()
		{
			InstallSnapPointGraphic(false);
			_numberOfPointsAnchored = Math.Max(_numberOfPointsAnchored - 1, 0);
		}

		/// <summary>
		/// Called when the builder is cancelling building the <see cref="InteractiveGraphicBuilder.Graphic"/> due to user cancellation.
		/// </summary>
		protected override void OnGraphicCancelled()
		{
			base.OnGraphicCancelled();
			InstallSnapPointGraphic(false);
		}

		/// <summary>
		/// Called when the builder is done building the <see cref="InteractiveGraphicBuilder.Graphic"/>.
		/// </summary>
		protected override void OnGraphicComplete()
		{
			InstallSnapPointGraphic(false);
			base.OnGraphicComplete();
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			_numberOfPointsAnchored++;

			if (_numberOfPointsAnchored == 1) // We just started creating
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}
			else if (_numberOfPointsAnchored >= 5 && mouseInformation.ClickCount >= 2) // We're done creating
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					this.Graphic.Points.RemoveAt(--_numberOfPointsAnchored);
					this.Graphic.Points[--_numberOfPointsAnchored] = this.Graphic.Points[0];
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}

				this.NotifyGraphicComplete();
			}
			else if (_numberOfPointsAnchored >= 4 && AtOrigin(mouseInformation.Location) && mouseInformation.ClickCount == 1) // We're done creating
			{
				this.NotifyGraphicComplete();
			}
			else if (_numberOfPointsAnchored >= 3 && _numberOfPointsAnchored >= _maximumVertices && mouseInformation.ClickCount == 1) // We're done creating
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					this.Graphic.Points.Add(this.Graphic.Points[0]);
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}
				this.NotifyGraphicComplete();
			}
			else if (_numberOfPointsAnchored >= 2 && mouseInformation.ClickCount == 1) // We're in the middle of creating
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					// ensures that the last anchored point is updated to the current point, in case the last tracked point wasn't equal to current location
					this.Graphic.Points[Graphic.Points.Count - 1] = mouseInformation.Location;
					this.Graphic.Points.Add(mouseInformation.Location);
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}
			}
			else if (mouseInformation.ClickCount > 1)
			{
				// removes the extra click if the user multi-clicks and it doesn't otherwise have any meaning
				_numberOfPointsAnchored--;
			}

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;

			if (_numberOfPointsAnchored >= 3 && AtOrigin(mouseInformation.Location))
			{
				this.Graphic.Points[_numberOfPointsAnchored] = this.Graphic.Points[0];
				InstallSnapPointGraphic(true);
			}
			else
			{
				this.Graphic.Points[_numberOfPointsAnchored] = mouseInformation.Location;
				InstallSnapPointGraphic(false);
			}

			this.Graphic.ResetCoordinateSystem();
			this.Graphic.Draw();

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		protected new void NotifyGraphicComplete()
		{
			// ensures that the last point is exactly equal to the first point, in case the last tracked point wasn't precisely at the origin
			Graphic.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				var firstPoint = Graphic.Points[0];
				Graphic.Points[Graphic.Points.Count - 1] = firstPoint;
			}
			finally
			{
				Graphic.ResetCoordinateSystem();
			}
			base.NotifyGraphicComplete();
		}

		private bool AtOrigin(PointF point)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			double distanceToOrigin = Vector.Distance(this.Graphic.Points[0], point);
			this.Graphic.ResetCoordinateSystem();
			return distanceToOrigin < _snapRadius;
		}

		private void InstallSnapPointGraphic(bool install)
		{
			IPointsGraphic subject = this.Graphic;
			CompositeGraphic parent = subject.ParentGraphic as CompositeGraphic;
			if (parent == null)
				return;

			if (install && subject.Points.Count > 0)
			{
				if (_snapPoint == null)
					_snapPoint = new SnapPointGraphic();

				if (!parent.Graphics.Contains(_snapPoint))
					parent.Graphics.Add(_snapPoint);

				_snapPoint.CoordinateSystem = subject.CoordinateSystem;
				_snapPoint.Location = subject.Points[0];
				_snapPoint.ResetCoordinateSystem();

				parent.Draw();
			}
			else if (_snapPoint != null && parent.Graphics.Contains(_snapPoint))
			{
				parent.Graphics.Remove(_snapPoint);
				parent.Draw();
			}
		}

		/// <summary>
		/// A graphic for indicating that the cursor is close enough to an existing point to snap to it.
		/// </summary>
		private class SnapPointGraphic : CompositeGraphic
		{
			private readonly InvariantEllipsePrimitive _circle;
			private PointF _location;

			internal SnapPointGraphic()
			{
				_circle = new InvariantEllipsePrimitive();
				_circle.Color = Color.Tomato;
				_circle.InvariantTopLeft = new PointF(-6, -6);
				_circle.InvariantBottomRight = new PointF(6, 6);

				this.Graphics.Add(_circle);
			}

			protected override void Dispose(bool disposing)
			{
				_circle.Dispose();
				base.Dispose(disposing);
			}

			/// <summary>
			/// Gets the coordinates of the point.
			/// </summary>
			public PointF Location
			{
				get
				{
					if (base.CoordinateSystem == CoordinateSystem.Source)
						return _location;
					else
						return base.SpatialTransform.ConvertToDestination(_location);
				}
				internal set
				{
					if (!FloatComparer.AreEqual(this.Location, value))
					{
						Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

						if (base.CoordinateSystem == CoordinateSystem.Source)
							_location = value;
						else
							_location = base.SpatialTransform.ConvertToSource(value);

						_circle.Location = this.Location;
					}
				}
			}

			/// <summary>
			/// Gets or sets the radius of the snap graphic.
			/// </summary>
			public float Radius
			{
				get { return _circle.InvariantBottomRight.X; }
				set
				{
					_circle.InvariantTopLeft = new PointF(-value, -value);
					_circle.InvariantBottomRight = new PointF(value, value);
				}
			}

			/// <summary>
			/// Gets or sets the colour of the snap graphic.
			/// </summary>
			public Color Color
			{
				get { return _circle.Color; }
				set { _circle.Color = value; }
			}

			/// <summary>
			/// Performs a hit test on the snap graphic at given point.
			/// </summary>
			/// <param name="point">The mouse position in destination coordinates.</param>
			/// <returns>
			/// <b>True</b> if <paramref name="point"/> is inside the snap graphic's radius, <b>false</b> otherwise.
			/// </returns>
			public override bool HitTest(Point point)
			{
				base.CoordinateSystem = CoordinateSystem.Source;
				bool result = FloatComparer.IsLessThan((float) Vector.Distance(base.SpatialTransform.ConvertToSource(point), this.Location), this.Radius);
				base.ResetCoordinateSystem();

				return result;
			}
		}
	}
}