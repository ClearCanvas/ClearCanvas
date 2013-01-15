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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class PolygonFTests
	{
		public PolygonFTests() {}

		[TestFixtureSetUp]
		public void Init() {}

		[TestFixtureTearDown]
		public void Cleanup() {}

		[Test]
		public void TestBasics()
		{
			PolygonF p1 = new PolygonF(PointF.Empty, PointF.Empty, PointF.Empty);
			Assert.AreEqual(3, p1.CountVertices);
			PolygonF p2 = new PolygonF(new PointF(0, 0), new PointF(1, 0), new PointF(1, 1));
			Assert.AreEqual(3, p2.CountVertices);
			Assert.IsFalse(p2.IsComplex);
			PolygonF p3 = new PolygonF(new PointF(0, 0), new PointF(1, 0), new PointF(1, 1), new PointF(0, 1));
			Assert.AreEqual(4, p3.CountVertices);
			Assert.IsFalse(p3.IsComplex);
			PolygonF p4 = new PolygonF(new PointF(0, 0), new PointF(1, 0), new PointF(0, 1), new PointF(1, 1));
			Assert.AreEqual(4, p4.CountVertices);
			Assert.IsTrue(p4.IsComplex);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstruction1()
		{
			List<PointF> list = new List<PointF>();
			new PolygonF(list);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstruction2()
		{
			List<PointF> list = new List<PointF>();
			list.Add(PointF.Empty);
			new PolygonF(list);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void TestInvalidConstruction3()
		{
			List<PointF> list = new List<PointF>();
			list.Add(PointF.Empty);
			list.Add(PointF.Empty);
			new PolygonF(list);
		}

		[Test]
		public void TestContainsPoint()
		{
			PolygonF polygon = new PolygonF(new PointF[] {new PointF(0, 0), new PointF(1, 0), new PointF(1, 1), new PointF(0, 1)});
			Assert.IsTrue(polygon.Contains(new PointF(0.5f, 0.5f))); // inside
			Assert.IsFalse(polygon.Contains(new PointF(0.5f, 1.5f))); // above
			Assert.IsFalse(polygon.Contains(new PointF(0f, 1.5f))); // above

			Assert.IsTrue(polygon.Contains(new PointF(0f, 0.5f))); // left edge
			Assert.IsFalse(polygon.Contains(new PointF(1f, 0.5f))); // right edge
			Assert.IsTrue(polygon.Contains(new PointF(0.5f, 0f))); // bottom edge
			Assert.IsFalse(polygon.Contains(new PointF(0.5f, 1f))); // top edge

			Assert.IsTrue(polygon.Contains(new PointF(0f, 0f))); // bottom left corner
			Assert.IsFalse(polygon.Contains(new PointF(0f, 1f))); // top left corner
			Assert.IsFalse(polygon.Contains(new PointF(1f, 0f))); // bottom right corner
			Assert.IsFalse(polygon.Contains(new PointF(1f, 1f))); // top right corner
		}

		[Test]
		public void TestContainsPoint2()
		{
			PolygonF polygon = new PolygonF(
				new PointF(250, 208),
				new PointF(247, 201),
				new PointF(245, 208),
				new PointF(245, 213),
				new PointF(249, 215),
				new PointF(251, 215),
				new PointF(254, 215),
				new PointF(251, 210),
				new PointF(255, 209),
				new PointF(254, 201));
			Assert.IsTrue(polygon.Contains(new PointF(249, 209)));
		}

		[Test]
		public void TestContainsPointNoBufferOverrun()
		{
			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(1, 1);
			data[3] = new PointF(0, 1);

			var offset = PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						int vertexCount = data.Length - 1;
						Assert.IsTrue(PolygonF.TestContains(new PointF(0.5f, 0.5f) - offset, points, vertexCount)); // inside
						Assert.IsFalse(PolygonF.TestContains(new PointF(0.5f, 1.5f) - offset, points, vertexCount)); // above
						Assert.IsFalse(PolygonF.TestContains(new PointF(0f, 1.5f) - offset, points, vertexCount)); // above

						Assert.IsTrue(PolygonF.TestContains(new PointF(0f, 0.5f) - offset, points, vertexCount)); // left edge
						Assert.IsFalse(PolygonF.TestContains(new PointF(1f, 0.5f) - offset, points, vertexCount)); // right edge
						Assert.IsTrue(PolygonF.TestContains(new PointF(0.5f, 0f) - offset, points, vertexCount)); // bottom edge
						Assert.IsFalse(PolygonF.TestContains(new PointF(0.5f, 1f) - offset, points, vertexCount)); // top edge

						Assert.IsTrue(PolygonF.TestContains(new PointF(0f, 0f) - offset, points, vertexCount)); // bottom left corner
						Assert.IsFalse(PolygonF.TestContains(new PointF(0f, 1f) - offset, points, vertexCount)); // top left corner
						Assert.IsFalse(PolygonF.TestContains(new PointF(1f, 0f) - offset, points, vertexCount)); // bottom right corner
						Assert.IsFalse(PolygonF.TestContains(new PointF(1f, 1f) - offset, points, vertexCount)); // top right corner
					}
				}
			}
		}

		[Test]
		public void TestContainsPointNoBufferOverrun2()
		{
			PointF[] data = new PointF[11];
			data[0] = new PointF(250, 208);
			data[1] = new PointF(247, 201);
			data[2] = new PointF(245, 208);
			data[3] = new PointF(245, 213);
			data[4] = new PointF(249, 215);
			data[5] = new PointF(251, 215);
			data[6] = new PointF(254, 215);
			data[7] = new PointF(251, 210);
			data[8] = new PointF(255, 209);
			data[9] = new PointF(254, 201);

			var offset = PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 10 (the 11th point simulates garbage data beyond the array)
						int vertexCount = data.Length - 1;
						Assert.IsTrue(PolygonF.TestContains(new PointF(249, 209) - offset, points, vertexCount));
					}
				}
			}
		}

		[Test]
		public void TestSimplePolygonIsComplexNoBufferOverrun()
		{
			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(1, 1);
			data[3] = new PointF(0, 1);

			PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						int vertexCount = data.Length - 1;
						Assert.IsFalse(PolygonF.TestIsComplex(points, vertexCount));
					}
				}
			}
		}

		[Test]
		public void TestComplexPolygonIsComplexNoBufferOverrun()
		{
			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(0, 1);
			data[3] = new PointF(1, 1);

			PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						int vertexCount = data.Length - 1;
						Assert.IsTrue(PolygonF.TestIsComplex(points, vertexCount));
					}
				}
			}
		}

		[Test]
		public void TestSimplePolygonAreaComputation()
		{
			PointF[] data = new PointF[4];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(1, 1);
			data[3] = new PointF(0, 1);
			Assert.AreEqual(1, new PolygonF(data).ComputeArea());

			PointF[] data2 = new PointF[4];
			data2[0] = new PointF(0, 0);
			data2[1] = new PointF(1, 0);
			data2[2] = new PointF(1, 1);
			data2[3] = new PointF(0, 1);
			Assert.AreEqual(1, new PolygonF(data2).ComputeArea());
		}

		[Test]
		public void TestComplexPolygonAreaComputation()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = side*side/2;

			PointF[] data = new PointF[4];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(side, 0);
			data[2] = new PointF(0, side);
			data[3] = new PointF(side, side);

			double result = new PolygonF(data).ComputeArea();
			Trace.WriteLine(string.Format("Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
			Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
		}

		[Test]
		public void TestComplexPolygonAreaComputation2()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = 5*side*side;

			List<PointF> data = new List<PointF>();
			data.Add(new PointF(0, 0));
			data.Add(new PointF(side, 0));
			data.Add(new PointF(side, 3*side));
			data.Add(new PointF(0, 3*side));
			data.Add(new PointF(0, 2*side));
			data.Add(new PointF(3*side, 2*side));
			data.Add(new PointF(3*side, 3*side));
			data.Add(new PointF(2*side, 3*side));
			data.Add(new PointF(2*side, 0));
			data.Add(new PointF(3*side, 0));
			data.Add(new PointF(3*side, side));
			data.Add(new PointF(0, side));

			double result = new PolygonF(data).ComputeArea();
			Trace.WriteLine(string.Format("Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
			Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
		}

		[Test]
		public void TestSimpleAreaAlgorithm()
		{
			// this test differs from TestSimplePolygonAreaComputation in that here we specifically call the simple area algorithm
			PointF[] data = new PointF[4];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(1, 1);
			data[3] = new PointF(0, 1);

			PolygonF.TestVertexNormalization(data);

			unsafe
			{
				fixed (PointF* points = data)
				{
					// inside PolygonF, the vertexCount is always exactly the expected array length
					Assert.AreEqual(1, PolygonF.TestSimpleAreaComputation(points, data.Length));
				}
			}
		}

		[Test]
		public void TestSimpleAreaAlgorithmNoBufferOverrun()
		{
			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(1, 0);
			data[2] = new PointF(1, 1);
			data[3] = new PointF(0, 1);

			PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						Assert.AreEqual(1, PolygonF.TestSimpleAreaComputation(points, data.Length - 1));
					}
				}
			}
		}

		[Test]
		public void TestSimpleAreaAlgorithmNoBufferOverrun2()
		{
			PointF[] data = new PointF[4];
			data[0] = new PointF(-10001, 10000);
			data[1] = new PointF(-10001, 10002);
			data[2] = new PointF(-10000, 10002);

			PolygonF.TestVertexNormalization(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 3 (the 4th point simulates garbage data beyond the array)
						Assert.AreEqual(1, PolygonF.TestSimpleAreaComputation(points, data.Length - 1));
					}
				}
			}
		}

		[Test]
		public void TestComplexAreaAlgorithmWithSimplePolygon()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = side*side;

			PointF[] data = new PointF[4];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(side, 0);
			data[2] = new PointF(side, side);
			data[3] = new PointF(0, side);

			PolygonF.TestVertexNormalization(data);

			RectangleF bounding = RectangleUtilities.ComputeBoundingRectangle(data);

			unsafe
			{
				fixed (PointF* points = data)
				{
					// inside PolygonF, the vertexCount is always exactly the expected array length
					double result = PolygonF.TestComplexAreaComputation(bounding, points, data.Length);
					Trace.WriteLine(string.Format("Complex Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
					Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
				}
			}
		}

		[Test]
		public void TestComplexAreaAlgorithmWithComplexPolygon()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = side*side/2;

			PointF[] data = new PointF[4];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(side, 0);
			data[2] = new PointF(0, side);
			data[3] = new PointF(side, side);

			PolygonF.TestVertexNormalization(data);

			RectangleF bounding = RectangleUtilities.ComputeBoundingRectangle(data);

			unsafe
			{
				fixed (PointF* points = data)
				{
					// inside PolygonF, the vertexCount is always exactly the expected array length
					double result = PolygonF.TestComplexAreaComputation(bounding, points, data.Length);
					Trace.WriteLine(string.Format("Complex Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
					Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
				}
			}
		}

		[Test]
		public void TestComplexAreaAlgorithmWithSimplePolygonNoBufferOverrun()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = side*side;

			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(side, 0);
			data[2] = new PointF(side, side);
			data[3] = new PointF(0, side);
			data[4] = new PointF(side/2f, side/2f);

			PolygonF.TestVertexNormalization(data);

			RectangleF bounding = RectangleUtilities.ComputeBoundingRectangle(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						double result = PolygonF.TestComplexAreaComputation(bounding, points, data.Length - 1);
						Trace.WriteLine(string.Format("Complex Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
						Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
					}
				}
			}
		}

		[Test]
		public void TestComplexAreaAlgorithmWithComplexPolygonNoBufferOverrun()
		{
			// the complex area algorithm is very low resolution compared to simple area, so the error magnitude is higher
			const int side = 1000;
			const int expected = side*side/2;

			PointF[] data = new PointF[5];
			data[0] = new PointF(0, 0);
			data[1] = new PointF(side, 0);
			data[2] = new PointF(0, side);
			data[3] = new PointF(side, side);
			data[4] = new PointF(side/2f, side/2f);

			PolygonF.TestVertexNormalization(data);

			RectangleF bounding = RectangleUtilities.ComputeBoundingRectangle(data);

			// test for a possible buffer overrun in the computation
			foreach (PointF garbagePoint in SimulatedBufferOverrunPoints)
			{
				data[data.Length - 1] = garbagePoint;
				unsafe
				{
					fixed (PointF* points = data)
					{
						// inside PolygonF, the vertexCount is always exactly the expected array length
						// which is 4 (the 5th point simulates garbage data beyond the array)
						double result = PolygonF.TestComplexAreaComputation(bounding, points, data.Length - 1);
						Trace.WriteLine(string.Format("Complex Area: Expected {0:f5} Got {1:f5}", expected, result), "UNIT_TESTS");
						Assert.IsTrue(Math.Abs(expected - result) < expected/100f);
					}
				}
			}
		}

		[Test]
		public void TestVertexNormalization()
		{
			var data = new[]
			           	{
			           		new PointF(8000, 8000),
			           		new PointF(10000, 10000),
			           		new PointF(9503, 9763),
			           		new PointF(9273, 8000),
			           		new PointF(10000, 9939),
			           		new PointF(8799, 8103)
			           	};

			var offset = PolygonF.TestVertexNormalization(data);
			Assert.AreEqual(new SizeF(9000, 9000), offset);
			Assert.AreEqual(new PointF(-1000, -1000), data[0]);
			Assert.AreEqual(new PointF(1000, 1000), data[1]);
			Assert.AreEqual(new PointF(503, 763), data[2]);
			Assert.AreEqual(new PointF(273, -1000), data[3]);
			Assert.AreEqual(new PointF(1000, 939), data[4]);
			Assert.AreEqual(new PointF(-201, -897), data[5]);
		}

		/// <summary>
		/// Simulated garbage data that might be found if you overrun a vertex buffer
		/// </summary>
		private static IEnumerable<PointF> SimulatedBufferOverrunPoints
		{
			get
			{
				// iterate all possible combinations of normal values, valid boundary values, infinities and NaN
				float[] data = new float[] {0, 1, float.MinValue, float.MaxValue, float.NegativeInfinity, float.PositiveInfinity, float.NaN};
				for (int i = 0; i < data.Length; i++)
					for (int j = 0; j < data.Length; j++)
						yield return new PointF(data[i], data[j]);
			}
		}
	}
}

#endif