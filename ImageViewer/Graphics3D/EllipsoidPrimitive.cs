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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A primitive ellipsoid graphic.
	/// </summary>
	[Cloneable(true)]
	public class EllipsoidPrimitive : BoundableGraphic3D
	{
		public override bool HitTest(Vector3D point)
		{
			CoordinateSystem = CoordinateSystem.Source;
			try
			{
				return HitTest(SpatialTransform.ConvertPointToSource(point), Rectangle, SpatialTransform);
			}
			finally
			{
				ResetCoordinateSystem();
			}
		}

		public override Vector3D GetClosestPoint(Vector3D point)
		{
			// Semi principal axes
			var a = Width/2;
			var b = Height/2;
			var c = Depth/2;

			// Center of ellipse
			var rect = Rectangle;
			var x1 = rect.Left + a;
			var y1 = rect.Top + b;
			var z1 = rect.Front + c;

			return IntersectEllipsoidAndLine(a, b, c, new Vector3D(x1, y1, z1), point);
		}

		public override bool Contains(Vector3D point)
		{
			// Semi principal axes
			var a = Width/2;
			var b = Height/2;
			var c = Depth/2;

			// Center of ellipse
			var rect = Rectangle;
			var x1 = rect.Left + a;
			var y1 = rect.Top + b;
			var z1 = rect.Front + c;

			return ComputePointEllipsoidAltitude(a, b, c, new Vector3D(x1, y1, z1), point) <= 1;
		}

		internal static bool HitTest(Vector3D srcPoint, Rectangle3D srcRectangle, SpatialTransform3D transform)
		{
			// Semi principal axes
			var a = srcRectangle.Width/2;
			var b = srcRectangle.Height/2;
			var c = srcRectangle.Depth/2;

			// Center of ellipse
			var x1 = srcRectangle.Left + a;
			var y1 = srcRectangle.Top + b;
			var z1 = srcRectangle.Front + c;

			return Math.Abs(ComputePointEllipsoidAltitude(a, b, c, new Vector3D(x1, y1, z1), srcPoint)) - 1 <= HitTestDistance/transform.CumulativeScale;
		}

		internal static float ComputePointEllipsoidAltitude(float a, float b, float c, Vector3D center, Vector3D point)
		{
			var testPoint = point - center;
			return (testPoint.X*testPoint.X)/(a*a) + (testPoint.Y*testPoint.Y)/(b*b) + (testPoint.Z*testPoint.Z)/(c*c);
		}

		/// <summary>
		/// Finds the intersection between an ellipsoid and a line that starts at the
		/// center of the ellipsoid and ends at an aribtrary point.
		/// </summary>
		internal static Vector3D IntersectEllipsoidAndLine(float a, float b, float c, Vector3D center, Vector3D point)
		{
			/*
			 * The point of intersection (P) between the center of the ellipsoid and the test point (Pt)
			 * where the center of the ellipsoid is at (0, 0, 0) can be described by the vector equation:
			 * _     __ 
			 * P = m*Pt
			 * 
			 * which yields three equations:
			 * 
			 * x = m * xt (1)
			 * y = m * yt (2)
			 * z = m * zt (3)
			 * 
			 * An ellipsoid centered at (0, 0, 0) is described by the equation:
			 * 
			 * x^2/a^2 + y^2/b^2 + z^2/c^2 = 1 (4)
			 * 
			 * substituting (1), (2) and (3) into (4) gives:
			 * 
			 * m^2*xt^2/a^2 + m^2*yt^2/b^2 + m^2*zt^2/c^2 = 1
			 * m^2*(xt^2*b^2*c^2 + yt^2*a^2*c^2 + zt^2*a^2*b^2) = (a*b*c)^2
			 * 
			 * finally,
			 * 
			 * m = a*b*c/Sqrt(xt^2*b^2*c^2 + yt^2*a^2*c^2 + zt^2*a^2*b^2) (where xt^2*b^2*c^2 > 0 and/or yt^2*a^2*c^2 > 0 and/or zt^2*a^2*b^2 > 0)
			 * 
			 * which is a constant for a given ellipsoid.
			 * 
			 * The intersection point (x, y, z) can then be found by substituting m into (1), (2) and (3).
			*/

			var testPoint = point - center;
			var denominator = (float) Math.Sqrt(testPoint.X*testPoint.X*b*b*c*c +
			                                    testPoint.Y*testPoint.Y*a*a*c*c +
			                                    testPoint.Z*testPoint.Z*a*a*b*b);

			if (FloatComparer.AreEqual(denominator, 0.0F, 0.001F))
				return center;

			var m = Math.Abs(a*b*c/denominator);
			return center + m*testPoint;
		}
	}
}