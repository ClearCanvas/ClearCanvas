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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// This class implements a simple algorithm for determining which cursor on an 8 point compass to show
	/// for a particular control point in a <see cref="ControlPointsGraphic.ControlPointGroup"/>.  The class assumes that the
	/// purpose of the control points is to stretch the graphic that owns the control points (as will be
	/// the case with most ROI graphic implementations).
	/// </summary>
	[Cloneable(true)]
	internal class CompassStretchCursorTokenStrategy : StretchCursorTokenStrategy
	{
		private enum CompassPoints { NorthEast = 0, SouthEast = 1, SouthWest = 2, NorthWest = 3, North = 4, East = 5, South = 6, West = 7 };

		[CloneIgnore]
		private SortedList<CompassPoints, CursorToken> _stretchIndicatorTokens;

		public CompassStretchCursorTokenStrategy()
		{
			InstallDefaults();
		}

		private IControlPointsGraphic ControlPoints
		{
			get
			{
				if (base.TargetGraphic is IControlPointsGraphic)
					return (IControlPointsGraphic) base.TargetGraphic;
				return null;
			}
		}

		/// <summary>
		/// Gets the bounding rectangle that contains all the points in the <see cref="ControlPointsGraphic.ControlPointGroup"/>.
		/// </summary>
		private RectangleF BoundingRectangle
		{
			get
			{
				List<PointF> controlPoints = new List<PointF>();
				for (int i = 0; i < this.ControlPoints.Count; ++i)
					controlPoints.Add(this.ControlPoints[i]);

				return RectangleUtilities.ComputeBoundingRectangle(controlPoints.ToArray());
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that corresponds to a particular point on the compass.
		/// </summary>
		/// <param name="compassPoint">the compass point.</param>
		/// <returns>the <see cref="CursorToken"/> that corresponds to the specified point on the compass, or null.</returns>
		private CursorToken this[CompassPoints compassPoint]
		{
			get
			{
				if (!_stretchIndicatorTokens.ContainsKey(compassPoint))
					return null;

				return _stretchIndicatorTokens[compassPoint]; 
			}
			set
			{
				if (value != null)
				{
					_stretchIndicatorTokens[compassPoint] = value;
				}
				else
				{
					if (_stretchIndicatorTokens.ContainsKey(compassPoint))
						_stretchIndicatorTokens.Remove(compassPoint);
				}
			}
		}

		/// <summary>
		/// Installs the default set of system cursors for the compass.
		/// </summary>
		private void InstallDefaults()
		{
			_stretchIndicatorTokens = new SortedList<CompassPoints, CursorToken>();

			_stretchIndicatorTokens[CompassPoints.East] =
				_stretchIndicatorTokens[CompassPoints.West] = new CursorToken(CursorToken.SystemCursors.SizeWE);

			_stretchIndicatorTokens[CompassPoints.North] =
				_stretchIndicatorTokens[CompassPoints.South] = new CursorToken(CursorToken.SystemCursors.SizeNS);

			_stretchIndicatorTokens[CompassPoints.NorthEast] =
				_stretchIndicatorTokens[CompassPoints.SouthWest] = new CursorToken(CursorToken.SystemCursors.SizeNESW);

			_stretchIndicatorTokens[CompassPoints.NorthWest] =
				_stretchIndicatorTokens[CompassPoints.SouthEast] = new CursorToken(CursorToken.SystemCursors.SizeNWSE);
		}

		/// <summary>
		/// Computes the distance from a point to a compass point on the given rectangle.
		/// </summary>
		/// <param name="point">a point whose distance from a compass point on the rectangle is to be determined.</param>
		/// <param name="compassRectangle">the rectangle from which to determine the compass point position.</param>
		/// <param name="compassPoint">the point on the compass to find the distance to.</param>
		/// <returns></returns>
		private float DistanceToCompassPoint(PointF point, RectangleF compassRectangle, CompassPoints compassPoint)
		{
			PointF compassPointPosition = GetCompassPointPosition(compassPoint, compassRectangle);
			return (float)Vector.Distance(point, compassPointPosition);
		}

		/// <summary>
		/// Computes the position on a given rectangle that corresponds to the given compass point.
		/// </summary>
		/// <param name="compassPoint">the compass point whose position on the rectangle is to be determined.</param>
		/// <param name="rectangle">the rectangle.</param>
		/// <returns>the point on the rectangle that corresponds to the given compass point.</returns>
		private PointF GetCompassPointPosition(CompassPoints compassPoint, RectangleF rectangle)
		{
			float top = rectangle.Top;
			float left = rectangle.Left;
			float right = rectangle.Right;
			float bottom = rectangle.Bottom;
			
			float centreX = left;
			if (left != right)
				centreX = rectangle.Left + rectangle.Width / 2F;

			float centreY = top;
			if (top != bottom)
				centreY = rectangle.Top + rectangle.Height / 2F;

			switch (compassPoint)
			{
				case CompassPoints.NorthWest:
					return new PointF(left, top);
				case CompassPoints.NorthEast:
					return new PointF(right, top);
				case CompassPoints.SouthEast:
					return new PointF(right, bottom);
				case CompassPoints.SouthWest:
					return new PointF(left, bottom);
				case CompassPoints.North:
					return new PointF(centreX, top);
				case CompassPoints.East:
					return new PointF(right, centreY);
				case CompassPoints.South:
					return new PointF(centreX, bottom);
				default: //CompassPoints.West:
					return new PointF(left, centreY);
			}
		}

		/// <summary>
		/// Gets the appropriate <see cref="CursorToken"/> for a given point (in destination coordinates).
		/// </summary>
		/// <param name="point">the point (in destination coordinates).</param>
		/// <returns>a <see cref="CursorToken"/> that is appropriate for the given point, or null.</returns>
		public override CursorToken GetCursorToken(Point point)
		{
			if (_stretchIndicatorTokens.Count == 0)
				return null;

			Platform.CheckForNullReference(ControlPoints, "_controlPoints");

			int controlPointIndex = ControlPoints.HitTestControlPoint(point);
			if (controlPointIndex < 0)
				return null;

			ControlPoints.CoordinateSystem = CoordinateSystem.Destination;

			PointF controlPoint = ControlPoints[controlPointIndex];
			RectangleF containingRectangle = this.BoundingRectangle;

			CompassPoints closestCompassPoint = _stretchIndicatorTokens.Keys[0];
			float minDistance = DistanceToCompassPoint(controlPoint, containingRectangle, closestCompassPoint);

			for (int i = 1; i < _stretchIndicatorTokens.Keys.Count; ++i)
			{
				CompassPoints compassPoint = _stretchIndicatorTokens.Keys[i];
				float distance = DistanceToCompassPoint(controlPoint, containingRectangle, compassPoint);

				if (distance <= minDistance)
				{
					closestCompassPoint = compassPoint;
					minDistance = distance;
				}
			}

			ControlPoints.ResetCoordinateSystem();
		
			return _stretchIndicatorTokens[closestCompassPoint];
		}
	}
}
