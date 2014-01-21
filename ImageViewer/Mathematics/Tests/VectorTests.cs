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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class VectorTests
	{
		[Test]
		public void TestSubtendedAngle()
		{
			double angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(0, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(0, 1));
			Assert.AreEqual(-90, angle);

			angle = Vector.SubtendedAngle(new PointF(0, 10), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(90, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(-1, 0));
			Assert.AreEqual(180, angle);
		}

		[Test]
		public void TestUnitVector()
		{
			float oneOverSqrt2 = 1/(float) Math.Sqrt(2);
			SizeF unitVector;
			PointF origin;

			origin = new PointF(0, 0);
			unitVector = new SizeF(Vector.GetUnitVector(origin, origin + new SizeF(-1, 1)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(-1, 1);
			unitVector = new SizeF(Vector.GetUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, -1);
			unitVector = new SizeF(Vector.GetUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, 1);
			unitVector = new SizeF(Vector.GetUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");
		}

		[Test]
		public void TestDistance()
		{
			var root2 = Math.Sqrt(2);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, 1);
			var p2 = new PointF(1, -1);
			var p3 = new PointF(-1, -1);
			var p4 = new PointF(-2, -2);

			// exercise zero computations
			Assert.AreEqual(0, Vector.Distance(p0, p0), "P0 to P0 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p1, p1), "P1 to P1 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p2, p2), "P2 to P2 distance should be 0");
			Assert.AreEqual(0, Vector.Distance(p3, p3), "P3 to P3 distance should be 0");

			// exercise computation with origin
			Assert.AreEqual(root2, Vector.Distance(p0, p1), "P0 to P1 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p1, p0), "P1 to P0 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p0, p2), "P0 to P2 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p2, p0), "P2 to P0 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p0, p3), "P0 to P3 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p3, p0), "P3 to P0 distance should be \u221A2");

			// exercise cross-quadrant computations
			Assert.AreEqual(2*root2, Vector.Distance(p1, p2), "P1 to P2 distance should be 2\u221A2");
			Assert.AreEqual(2*root2, Vector.Distance(p2, p1), "P2 to P1 distance should be 2\u221A2");
			Assert.AreEqual(2, Vector.Distance(p1, p3), "P1 to P3 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p3, p1), "P3 to P1 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p2, p3), "P2 to P3 distance should be 2");
			Assert.AreEqual(2, Vector.Distance(p3, p2), "P3 to P2 distance should be 2");

			// exercise same-quadrant computation
			Assert.AreEqual(root2, Vector.Distance(p3, p4), "P3 to P4 distance should be \u221A2");
			Assert.AreEqual(root2, Vector.Distance(p4, p3), "P4 to P3 distance should be \u221A2");
		}

		[Test]
		public void TestMidpoint()
		{
			var root2F = (float) Math.Sqrt(2);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, 1);
			var p2 = new PointF(1, -1);
			var p3 = new PointF(-1, -1);
			var p4 = new PointF(-2, -2);

			// exercise zero computations
			Assert.AreEqual(p0, Vector.Midpoint(p0, p0), "P0 to P0 midpoint should be P0");
			Assert.AreEqual(p1, Vector.Midpoint(p1, p1), "P1 to P1 midpoint should be P1");
			Assert.AreEqual(p2, Vector.Midpoint(p2, p2), "P2 to P2 midpoint should be P2");
			Assert.AreEqual(p3, Vector.Midpoint(p3, p3), "P3 to P3 midpoint should be P3");

			// exercise computation with origin
			Assert.AreEqual(new PointF(-1/2f, 1/2f), Vector.Midpoint(p0, p1), "P0 to P1 midpoint should be (-\u00BD,\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, 1/2f), Vector.Midpoint(p1, p0), "P1 to P0 midpoint should be (-\u00BD,\u00BD)");
			Assert.AreEqual(new PointF(1/2f, -1/2f), Vector.Midpoint(p0, p2), "P0 to P2 midpoint should be (\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(1/2f, -1/2f), Vector.Midpoint(p2, p0), "P2 to P0 midpoint should be (\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, -1/2f), Vector.Midpoint(p0, p3), "P0 to P3 midpoint should be (-\u00BD,-\u00BD)");
			Assert.AreEqual(new PointF(-1/2f, -1/2f), Vector.Midpoint(p3, p0), "P3 to P0 midpoint should be (-\u00BD,-\u00BD)");

			// exercise cross-quadrant computations
			Assert.AreEqual(new PointF(0, 0), Vector.Midpoint(p1, p2), "P1 to P2 midpoint should be (0,0)");
			Assert.AreEqual(new PointF(0, 0), Vector.Midpoint(p2, p1), "P2 to P1 midpoint should be (0,0)");
			Assert.AreEqual(new PointF(-1, 0), Vector.Midpoint(p1, p3), "P1 to P3 midpoint should be (-1,0)");
			Assert.AreEqual(new PointF(-1, 0), Vector.Midpoint(p3, p1), "P3 to P1 midpoint should be (-1,0)");
			Assert.AreEqual(new PointF(0, -1), Vector.Midpoint(p2, p3), "P2 to P3 midpoint should be (0,-1)");
			Assert.AreEqual(new PointF(0, -1), Vector.Midpoint(p3, p2), "P3 to P2 midpoint should be (0,-1)");

			// exercise same-quadrant computation
			Assert.AreEqual(new PointF(root2F/2, -root2F/2), Vector.Midpoint(p0, new PointF(root2F, -root2F)), "P0 to (\u221A2,-\u221A2) midpoint should be (-\u221A2/2,-\u221A2/2)");
			Assert.AreEqual(new PointF(root2F/2, -root2F/2), Vector.Midpoint(new PointF(root2F, -root2F), p0), "(\u221A2,-\u221A2) to P0 midpoint should be (-\u221A2/2,-\u221A2/2)");
			Assert.AreEqual(new PointF(-1.5f, -1.5f), Vector.Midpoint(p3, p4), "P3 to P4 midpoint should be (-1\u00BD,-1\u00BD)");
			Assert.AreEqual(new PointF(-1.5f, -1.5f), Vector.Midpoint(p4, p3), "P4 to P3 midpoint should be (-1\u00BD,-1\u00BD)");
		}

		[Test]
		public void TestPointToLine()
		{
			var root2 = Math.Sqrt(2);
			var root3 = Math.Sqrt(3);
			var root5 = Math.Sqrt(5);
			var p0 = new PointF(0, 0);
			var p1 = new PointF(-1, -1);
			var p2 = new PointF(2, -1);
			var p3 = new PointF(1, 1);
			var p4 = new PointF(1, (float) root3);

			// exercise coincident point-point computations
			AssertPointToLine(0, p0, p0, p0, p0, "Point P0 and line P0P0 (coincident points))");
			AssertPointToLine(0, p1, p1, p1, p1, "Point P1 and line P1P1 (coincident points)");
			AssertPointToLine(0, p2, p2, p2, p2, "Point P2 and line P2P2 (coincident points)");
			AssertPointToLine(0, p3, p3, p3, p3, "Point P3 and line P3P3 (coincident points)");

			// exercise point-point computations
			AssertPointToLine(root2, p1, p0, p1, p1, "Point P0 and line P1P1 (point to point)");
			AssertPointToLine(3, p2, p1, p2, p2, "Point P1 and line P2P2 (point to point)");
			AssertPointToLine(root5, p3, p2, p3, p3, "Point P2 and line P3P3 (point to point)");
			AssertPointToLine(root2, p0, p3, p0, p0, "Point P3 and line P0P0 (point to point)");

			// exercise colinear point-line computations
			AssertPointToLine(0, p0, p0, p1, p3, "Point P0 and line P1P3 (colinear point to line segment)");
			AssertPointToLine(0, p0, p0, p3, p1, "Point P0 and line P3P1 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p1, p0, p3, "Point P1 and line P0P3 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p1, p3, p0, "Point P1 and line P3P0 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p3, p0, p1, "Point P3 and line P0P1 (colinear point to line segment)");
			AssertPointToLine(root2, p0, p3, p1, p0, "Point P3 and line P1P0 (colinear point to line segment)");

			// exercise point-line computations
			AssertPointToLine(root2, p0, p1, p0, p2, "Point P1 and line P0P2 (point to line segment)");
			AssertPointToLine(root2, p0, p1, p2, p0, "Point P1 and line P2P0 (point to line segment)");
			AssertPointToLine(2, new PointF(1, -1), p3, p1, p2, "Point P3 and line P1P2 (point to line segment)");
			AssertPointToLine(2, new PointF(1, -1), p3, p2, p1, "Point P3 and line P2P1 (point to line segment)");
			AssertPointToLine(2*root3, 1e-6, p4, new PointF(4, 0), p0, p4, "Point (4,0) and line P0P4 (point to line segment)");
			AssertPointToLine(2*root3, 1e-6, p4, new PointF(4, 0), p4, p0, "Point (4,0) and line P4P0 (point to line segment)");
			// distance is off by that much here even when using Vector.Distance as a reference
		}

		private static void AssertPointToLine(double expectedDistance, PointF expectedPoint, PointF pT, PointF p1, PointF p2, string message, params object[] args)
		{
			var actualPoint = new PointF();
			var actualDistance = Vector.DistanceFromPointToLine(pT, p1, p2, ref actualPoint);
			Assert.AreEqual(expectedDistance, actualDistance, message, args);
			Assert.AreEqual(expectedPoint, actualPoint, message, args);
		}

		private static void AssertPointToLine(double expectedDistance, double tolerance, PointF expectedPoint, PointF pT, PointF p1, PointF p2, string message, params object[] args)
		{
			var actualPoint = new PointF();
			var actualDistance = Vector.DistanceFromPointToLine(pT, p1, p2, ref actualPoint);
			Assert.AreEqual(expectedDistance, actualDistance, tolerance, message, args);
			Assert.AreEqual(expectedPoint, actualPoint, message, args);
		}

		[Test]
		[Obsolete("Function under test is deprecated.")]
		public void TestLineSegmentIntersection()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate intersection computations
			AssertLineSegmentIntersection(p0, p0, p0, p0, "P0P0 and P0P0 (point and point)");
			AssertLineSegmentIntersection(p1, p1, p1, p1, "P1P1 and P1P1 (point and point)");
			AssertLineSegmentIntersection(p0, p0, p1, p1, "P0P0 and P1P1 (point and point)");
			AssertLineSegmentIntersection(p1, p1, p0, p0, "P1P1 and P0P0 (point and point)");

			// exercise single degenerate intersection computations
			AssertLineSegmentIntersection(p2, p2, p0, p1, "P2P2 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(p2, p2, p1, p0, "P2P2 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(p3, p3, p0, p1, "P3P3 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(p3, p3, p1, p0, "P3P3 and P1P0 (point and line segment)");
			AssertLineSegmentIntersection(p5, p5, p0, p1, "P5P5 and P0P1 (point and line segment)");
			AssertLineSegmentIntersection(p5, p5, p1, p0, "P5P5 and P1P0 (point and line segment)");

			// exercise two non-colinear line segments
			AssertLineSegmentIntersection(p2, p4, p0, p1, "P2P4 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p4, p2, p0, p1, "P4P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p2, p6, p0, p1, "P2P6 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p6, p2, p0, p1, "P6P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p2, p7, p0, p1, "P2P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p7, p2, p0, p1, "P7P2 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p6, p7, p0, p1, "P6P7 and P0P1 (distinct line segments)");
			AssertLineSegmentIntersection(p7, p6, p0, p1, "P7P6 and P0P1 (distinct line segments)");

			// exercise two colinear line segments
			AssertLineSegmentIntersection(p0, p1, p3, p5, "P0P1 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(p0, p1, p5, p3, "P0P1 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(p1, p0, p3, p5, "P1P0 and P3P5 (partially overlapping line segments)");
			AssertLineSegmentIntersection(p1, p0, p5, p3, "P1P0 and P5P3 (partially overlapping line segments)");
			AssertLineSegmentIntersection(p0, p5, p3, p1, "P0P5 and P3P1 (overlapping line segments)");
			AssertLineSegmentIntersection(p0, p5, p1, p3, "P0P5 and P1P3 (overlapping line segments)");
			AssertLineSegmentIntersection(p3, p1, p5, p0, "P3P1 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(p1, p3, p5, p0, "P1P3 and P5P0 (overlapping line segments)");
			AssertLineSegmentIntersection(p0, p3, p1, p5, "P0P3 and P1P5 (distinct line segments)");
			AssertLineSegmentIntersection(p3, p0, p5, p1, "P3P0 and P5P1 (distinct line segments)");
			AssertLineSegmentIntersection(p2, p4, p2, p6, "P2P4 and P2P6 (concurrent line segments)");
			AssertLineSegmentIntersection(p2, p4, p6, p2, "P2P4 and P6P2 (concurrent line segments)");
			AssertLineSegmentIntersection(p4, p2, p2, p6, "P4P2 and P2P6 (concurrent line segments)");
			AssertLineSegmentIntersection(p4, p2, p6, p2, "P4P2 and P6P2 (concurrent line segments)");
		}

		// TODO CR (Nov 11): Need to actually write an intersection test from scratch, as we've seen that the legacy "reference" function has a lot of precision loss on a x64 cpu

		[Obsolete("Function under test is deprecated.")]
		private static void AssertLineSegmentIntersection(PointF p1, PointF p2, PointF q1, PointF q2, string message, params object[] args)
		{
			// the legacy function had an error of up to +/- (0.5,0.5)
			PointF expectedIntersection;
			var expectedResult = LegacyLineSegmentIntersection(p1, p2, q1, q2, out expectedIntersection);

			PointF actualIntersection;
			var actualResult = Vector.LineSegmentIntersection(p1, p2, q1, q2, out actualIntersection);

			Assert.AreEqual(expectedResult, actualResult, message, args);
			Assert.Less(Math.Abs(expectedIntersection.X - actualIntersection.X), 0.5000001, message, args);
			Assert.Less(Math.Abs(expectedIntersection.Y - actualIntersection.Y), 0.5000001, message, args);
		}

		[Test]
		[Obsolete("Function under test is deprecated.")]
		public void TestLineSegmentIntersectionBrutal()
		{
			var values = new[] {-100.253f, -10, -1, 0, 1, 10, 99.3435f};
			var mod = values.Length;
			var permutations = (int) Math.Pow(mod, 8);
			for (int n = 0; n < permutations; n++)
			{
				var log = new StringBuilder();
				var printLog = false;

				var p1 = new PointF(values[(n)%mod], values[(n/mod/mod/mod/mod/mod/mod/mod)%mod]);
				var p2 = new PointF(values[(n/mod/mod)%mod], values[(n/mod/mod/mod/mod/mod)%mod]);
				var q1 = new PointF(values[(n/mod/mod/mod/mod)%mod], values[(n/mod/mod/mod)%mod]);
				var q2 = new PointF(values[(n/mod/mod/mod/mod/mod/mod)%mod], values[(n/mod)%mod]);

				PointF expectedIntersection;
				var expectedResult = LegacyLineSegmentIntersection(p1, p2, q1, q2, out expectedIntersection);

				PointF actualIntersection;
				var actualResult = Vector.LineSegmentIntersection(p1, p2, q1, q2, out actualIntersection);

				try
				{
					log.AppendLine(string.Format("Under Test <{0}{1}> :: <{2}{3}>", p1, p2, q1, q2));

					if (expectedResult == actualResult)
					{
						log.AppendLine(string.Format("  Expected intersection at {0}", expectedIntersection));
						log.AppendLine(string.Format("  Computed intersection at {0}", actualIntersection));

						// use 0.51 error as we've seen that the legacy function adds up to 0.5 in each dimension for rounding
						Assert.Less(Math.Abs(expectedIntersection.X - actualIntersection.X), 0.55, "Intersection has excessive difference in Y dimension");
						Assert.Less(Math.Abs(expectedIntersection.Y - actualIntersection.Y), 0.55, "Intersection has excessive difference in Y dimension");
					}
					else
					{
						log.AppendLine(string.Format("  Expected {0} but got {1}", expectedResult, actualResult));
						if (expectedResult == Vector.LineSegments.Intersect)
						{
							log.AppendLine(string.Format("  Expected intersection at {0}", expectedIntersection));

							PointF lineIntersection;
							if (Vector.IntersectLines(p1, p2, q1, q2, out lineIntersection))
							{
								var error = Vector.Distance(expectedIntersection, lineIntersection);
								log.AppendLine(string.Format("  Theoretical intersection at {0} (delta magnitude of {1})", lineIntersection, error));

								// use 0.71 error as we've seen that the legacy function adds up to 0.5 in each dimension for rounding
								// and 0.5 in each dimension is 0.707 in magnitude form
								Assert.Less(error, 0.8, "Intersection error exceeds threshold for floating point error while restricting intersection to line segment");
								//printLog = true;
							}
							else
							{
								log.AppendLine(string.Format("  Theoretical intersection not found"));
								Assert.Fail("Legacy function found intersection at {0}, but computation did not produce any near results", expectedIntersection);
								printLog = true;
							}
						}
						else if (actualResult == Vector.LineSegments.Intersect)
						{
							log.AppendLine(string.Format("  Computed intersection at {0}", actualIntersection));
							log.AppendLine(string.Format("  Explained as a corner case that the legacy function does not handle well."));
							//printLog = true;
						}
						else
						{
							// the functions disagree on whether two non-intersecting lines are or are not colinear
							// we shall allow these to pass, since we already know that the legacy function does not
							// handle degenerate inputs particularly well or consistently
							log.AppendLine(string.Format("  Explained as a corner case that the legacy function does not handle well."));
							//printLog = true;
						}
					}
				}
				catch (Exception)
				{
					printLog = true;
					throw;
				}
				finally
				{
					if (printLog)
						Console.WriteLine(log.ToString());
				}
			}
		}

		[Test]
		public void TestIntersectLineSegments()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate intersection computations
			AssertIntersectLineSegments(null, p0, p0, p0, p0, "P0P0 and P0P0 (point and point)");
			AssertIntersectLineSegments(null, p1, p1, p1, p1, "P1P1 and P1P1 (point and point)");
			AssertIntersectLineSegments(null, p0, p0, p1, p1, "P0P0 and P1P1 (point and point)");
			AssertIntersectLineSegments(null, p1, p1, p0, p0, "P1P1 and P0P0 (point and point)");

			// exercise single degenerate intersection computations
			AssertIntersectLineSegments(null, p2, p2, p0, p1, "P2P2 and P0P1 (point and line segment)");
			AssertIntersectLineSegments(null, p2, p2, p1, p0, "P2P2 and P1P0 (point and line segment)");
			AssertIntersectLineSegments(null, p3, p3, p0, p1, "P3P3 and P0P1 (point and line segment)");
			AssertIntersectLineSegments(null, p3, p3, p1, p0, "P3P3 and P1P0 (point and line segment)");
			AssertIntersectLineSegments(null, p5, p5, p0, p1, "P5P5 and P0P1 (point and line segment)");
			AssertIntersectLineSegments(null, p5, p5, p1, p0, "P5P5 and P1P0 (point and line segment)");

			// exercise two non-colinear line segments
			AssertIntersectLineSegments(p3, p2, p4, p0, p1, "P2P4 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(p3, p4, p2, p0, p1, "P4P2 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p2, p6, p0, p1, "P2P6 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p6, p2, p0, p1, "P6P2 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p2, p7, p0, p1, "P2P7 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p7, p2, p0, p1, "P7P2 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p6, p7, p0, p1, "P6P7 and P0P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p7, p6, p0, p1, "P7P6 and P0P1 (distinct line segments)");

			// exercise two colinear line segments
			AssertIntersectLineSegments(null, p0, p1, p3, p5, "P0P1 and P3P5 (partially overlapping line segments)");
			AssertIntersectLineSegments(null, p0, p1, p5, p3, "P0P1 and P5P3 (partially overlapping line segments)");
			AssertIntersectLineSegments(null, p1, p0, p3, p5, "P1P0 and P3P5 (partially overlapping line segments)");
			AssertIntersectLineSegments(null, p1, p0, p5, p3, "P1P0 and P5P3 (partially overlapping line segments)");
			AssertIntersectLineSegments(null, p0, p5, p3, p1, "P0P5 and P3P1 (overlapping line segments)");
			AssertIntersectLineSegments(null, p0, p5, p1, p3, "P0P5 and P1P3 (overlapping line segments)");
			AssertIntersectLineSegments(null, p3, p1, p5, p0, "P3P1 and P5P0 (overlapping line segments)");
			AssertIntersectLineSegments(null, p1, p3, p5, p0, "P1P3 and P5P0 (overlapping line segments)");
			AssertIntersectLineSegments(null, p0, p3, p1, p5, "P0P3 and P1P5 (distinct line segments)");
			AssertIntersectLineSegments(null, p3, p0, p5, p1, "P3P0 and P5P1 (distinct line segments)");
			AssertIntersectLineSegments(null, p2, p4, p2, p6, "P2P4 and P2P6 (concurrent line segments)");
			AssertIntersectLineSegments(null, p2, p4, p6, p2, "P2P4 and P6P2 (concurrent line segments)");
			AssertIntersectLineSegments(null, p4, p2, p2, p6, "P4P2 and P2P6 (concurrent line segments)");
			AssertIntersectLineSegments(null, p4, p2, p6, p2, "P4P2 and P6P2 (concurrent line segments)");
		}

		private static void AssertIntersectLineSegments(PointF? expectedIntersection, PointF p1, PointF p2, PointF q1, PointF q2, string message, params object[] args)
		{
			PointF actualIntersection;
			var result = Vector.IntersectLineSegments(p1, p2, q1, q2, out actualIntersection);
			if (expectedIntersection.HasValue)
			{
				Assert.IsTrue(result, message, args);
				Assert.AreEqual(expectedIntersection.Value, actualIntersection, message, args);
			}
			else
			{
				Assert.IsFalse(result, message, args);
				Assert.AreEqual(PointF.Empty, actualIntersection, message, args);
			}
		}

		[Test]
		public void TestAreColinear()
		{
			var p0 = new PointF(0, 0);
			var p1 = new PointF(3, 0);
			var p2 = new PointF(2, 1);
			var p3 = new PointF(1.5f, 0);
			var p4 = new PointF(1, -1);
			var p5 = new PointF(4, 0);
			var p6 = new PointF(2.5f, 2);
			var p7 = new PointF(5, 1);

			// exercise dual degenerate inputs
			Assert.AreEqual(true, Vector.AreColinear(p0, p0, p0, p0), "P0P0 and P0P0 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p1, p1, p1), "P1P1 and P1P1 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p0, p1, p1), "P0P0 and P1P1 (point and point)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p1, p0, p0), "P1P1 and P0P0 (point and point)");

			// exercise single degenerate inputs
			Assert.AreEqual(false, Vector.AreColinear(p2, p2, p0, p1), "P2P2 and P0P1 (point and line)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p2, p1, p0), "P2P2 and P1P0 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p3, p0, p1), "P3P3 and P0P1 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p3, p1, p0), "P3P3 and P1P0 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p5, p5, p0, p1), "P5P5 and P0P1 (point and line)");
			Assert.AreEqual(true, Vector.AreColinear(p5, p5, p1, p0), "P5P5 and P1P0 (point and line)");

			// exercise two non-colinear lines
			Assert.AreEqual(false, Vector.AreColinear(p2, p4, p0, p1), "P2P4 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p4, p2, p0, p1), "P4P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p6, p0, p1), "P2P6 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p6, p2, p0, p1), "P6P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p2, p7, p0, p1), "P2P7 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p7, p2, p0, p1), "P7P2 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p6, p7, p0, p1), "P6P7 and P0P1 (distinct lines)");
			Assert.AreEqual(false, Vector.AreColinear(p7, p6, p0, p1), "P7P6 and P0P1 (distinct lines)");

			// exercise two colinear lines
			Assert.AreEqual(true, Vector.AreColinear(p0, p1, p3, p5), "P0P1 and P3P5 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p1, p5, p3), "P0P1 and P5P3 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p0, p3, p5), "P1P0 and P3P5 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p0, p5, p3), "P1P0 and P5P3 (partially overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p5, p3, p1), "P0P5 and P3P1 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p5, p1, p3), "P0P5 and P1P3 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p1, p5, p0), "P3P1 and P5P0 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p1, p3, p5, p0), "P1P3 and P5P0 (overlapping lines)");
			Assert.AreEqual(true, Vector.AreColinear(p0, p3, p1, p5), "P0P3 and P1P5 (distinct lines)");
			Assert.AreEqual(true, Vector.AreColinear(p3, p0, p5, p1), "P3P0 and P5P1 (distinct lines)");
			Assert.AreEqual(true, Vector.AreColinear(p2, p4, p2, p6), "P2P4 and P2P6 (concurrent lines)");
			Assert.AreEqual(true, Vector.AreColinear(p2, p4, p6, p2), "P2P4 and P6P2 (concurrent lines)");
			Assert.AreEqual(true, Vector.AreColinear(p4, p2, p2, p6), "P4P2 and P2P6 (concurrent lines)");
			Assert.AreEqual(true, Vector.AreColinear(p4, p2, p6, p2), "P4P2 and P6P2 (concurrent lines)");
		}

		[Test]
		public void ComparePerformance()
		{
			PointF dummy;
			Trace.WriteLine(string.Format(" Legacy      : {0:f8} [\u00B5s]", ExecutePerformanceTest((p, q, r, s) => LegacyLineSegmentIntersection(p, q, r, s, out dummy))));
			Trace.WriteLine(string.Format(" Intersect   : {0:f8} [\u00B5s]", ExecutePerformanceTest((p, q, r, s) => Vector.IntersectLineSegments(p, q, r, s, out dummy))));
			Trace.WriteLine(string.Format(" AreColinear : {0:f8} [\u00B5s]", ExecutePerformanceTest((p, q, r, s) => Vector.AreColinear(p, q, r, s))));
		}

		#region Performance Testing Helpers

		private delegate void PerformanceTestCallback(PointF p1, PointF p2, PointF p3, PointF p4);

		private static float ExecutePerformanceTest(PerformanceTestCallback callback)
		{
			var values = new[] {-100300.43245325f, -1, -float.Epsilon, 0, float.Epsilon, 1, 90952.343542f};
			var mod = values.Length;
			var permutations = (int) Math.Pow(mod, 8);

			var cc = new CodeClock();
			cc.Start();
			for (int n = 0; n < permutations; n++)
			{
				callback.Invoke(
					new PointF(values[(n)%mod], values[(n/mod/mod/mod/mod/mod/mod/mod)%mod]),
					new PointF(values[(n/mod/mod)%mod], values[(n/mod/mod/mod/mod/mod)%mod]),
					new PointF(values[(n/mod/mod/mod/mod)%mod], values[(n/mod/mod/mod)%mod]),
					new PointF(values[(n/mod/mod/mod/mod/mod/mod)%mod], values[(n/mod)%mod])
					);
			}
			cc.Stop();
			return cc.Seconds*1000000/permutations;
		}

		#endregion

		#region Reference Implementations

		/// <summary>
		/// Legacy implementation of computing line segment intersection and colinearity for baseline reference.
		/// </summary>
		private static Vector.LineSegments LegacyLineSegmentIntersection(PointF p1, PointF p2, PointF q1, PointF q2, out PointF intersectionPoint)
		{
			float x1 = p1.X;
			float y1 = p1.Y;
			float x2 = p2.X;
			float y2 = p2.Y;
			float x3 = q1.X;
			float y3 = q1.Y;
			float x4 = q2.X;
			float y4 = q2.Y;

			float a1, a2, b1, b2, c1, c2; /* Coefficients of line eqns. */
			float r1, r2, r3, r4; /* 'Sign' values */
			float denom, offset, num; /* Intermediate values */

			/* Compute a1, b1, c1, where line joining points 1 and 2
			 * is "a1 x  +  b1 y  +  c1  =  0".
			 */

			a1 = y2 - y1;
			b1 = x1 - x2;
			c1 = x2*y1 - x1*y2;

			/* Compute r3 and r4.
			 */

			r3 = a1*x3 + b1*y3 + c1;
			r4 = a1*x4 + b1*y4 + c1;

			/* Check signs of r3 and r4.  If both point 3 and point 4 lie on
			 * same side of line 1, the line segments do not intersect.
			 */

			if (r3 != 0 &&
			    r4 != 0 &&
			    Math.Sign(r3) == Math.Sign(r4))
			{
				intersectionPoint = new PointF(0, 0);
				return Vector.LineSegments.DoNotIntersect;
			}

			/* Compute a2, b2, c2 */

			a2 = y4 - y3;
			b2 = x3 - x4;
			c2 = x4*y3 - x3*y4;

			/* Compute r1 and r2 */

			r1 = a2*x1 + b2*y1 + c2;
			r2 = a2*x2 + b2*y2 + c2;

			/* Check signs of r1 and r2.  If both point 1 and point 2 lie
			 * on same side of second line segment, the line segments do
			 * not intersect.
			 */

			if (r1 != 0 &&
			    r2 != 0 &&
			    Math.Sign(r1) == Math.Sign(r2))
			{
				intersectionPoint = new PointF(0, 0);
				return Vector.LineSegments.DoNotIntersect;
			}

			/* Line segments intersect: compute intersection point. 
			 */

			denom = a1*b2 - a2*b1;
			if (denom == 0)
			{
				intersectionPoint = new PointF(0, 0);
				return Vector.LineSegments.Colinear;
			}

			offset = denom < 0 ? -denom/2 : denom/2;

			/* The denom/2 is to get rounding instead of truncating.  It
			 * is added or subtracted to the numerator, depending upon the
			 * sign of the numerator.
			 */

			num = b1*c2 - b2*c1;
			float x = (num < 0 ? num - offset : num + offset)/denom;

			num = a2*c1 - a1*c2;
			float y = (num < 0 ? num - offset : num + offset)/denom;

			intersectionPoint = new PointF(x, y);

			return Vector.LineSegments.Intersect;
		}

		#endregion
	}
}

#endif