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
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class Matrix3DTests
	{
		[Test]
		public void TestConstructor()
		{
			var m = new Matrix3D();
			m.SetColumn(0, 0, 0, 0);
			m.SetColumn(1, 0, 0, 0);
			m.SetColumn(2, 0, 0, 0);
			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D()));

			m.SetRow(0, 1, 2, 3);
			m.SetRow(1, 4, 5, 6);
			m.SetRow(2, 7, 8, 9);
			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}})));
			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D(new Matrix(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}}))));

			try
			{
				new Matrix3D(new float[,] {{1, 2}, {4, 5}, {6, 7}});
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				new Matrix3D(new float[,] {{1, 2, 3}, {4, 5, 6}});
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				new Matrix3D(new Matrix(4, 5));
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestFromRows()
		{
			var v1 = new Vector3D(1, 2, 3);
			var v2 = new Vector3D(4, 5, 6);
			var v3 = new Vector3D(7, 8, 9);

			var m = Matrix3D.FromRows(v1, v2, v3);

			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}})));
		}

		[Test]
		public void TestFromColumns()
		{
			var v1 = new Vector3D(1, 2, 3);
			var v2 = new Vector3D(4, 5, 6);
			var v3 = new Vector3D(7, 8, 9);

			var m = Matrix3D.FromColumns(v1, v2, v3);

			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}}).Transpose()));
		}

		[Test]
		public void TestAreEqual()
		{
			var m1 = new Matrix3D();
			m1.SetColumn(0, 1, 4, 7);
			m1.SetColumn(1, 2, 5, 8);
			m1.SetColumn(2, 3, 6, 9);

			var m2 = new Matrix3D();
			m2.SetRow(0, 1, 2, 3);
			m2.SetRow(1, 4, 5, 6);
			m2.SetRow(2, 7, 8, 9);

			Assert.IsTrue(Matrix3D.AreEqual(m1, m2));

			m2[1, 1] = 0;
			Assert.IsFalse(Matrix3D.AreEqual(m1, m2));
		}

		[Test]
		public void TestNegate()
		{
			var m = -new Matrix3D(new float[,]
			                      	{
			                      		{1, -2, 3},
			                      		{-4, 5, -6},
			                      		{7, -8, 9}
			                      	});

			Assert.IsTrue(Matrix3D.AreEqual(m, new Matrix3D(new float[,] {{-1, 2, -3}, {4, -5, 6}, {-7, 8, -9}})));
		}

		[Test]
		public void TestAdd()
		{
			var m1 = new Matrix3D();
			m1.SetColumn(0, 2.2F, -6.1F, -7.6F);
			m1.SetColumn(1, -3.4F, 7.2F, 8.7F);
			m1.SetColumn(2, 1.6F, 5.5F, -9.8F);

			var m2 = new Matrix3D();
			m2.SetRow(0, -1.1F, 2.6F, -7.1F);
			m2.SetRow(1, 4.6F, -3.7F, 9.1F);
			m2.SetRow(2, 4.1F, -3.1F, 7.7F);

			var result = new Matrix3D();
			result.SetRow(0, 1.1F, -0.8F, -5.5F);
			result.SetRow(1, -1.5F, 3.5F, 14.6F);
			result.SetRow(2, -3.5F, 5.6F, -2.1F);

			Assert.IsTrue(Matrix3D.AreEqual(m1 + m2, result));
		}

		[Test]
		public void TestSubtract()
		{
			var m1 = new Matrix3D();
			m1.SetColumn(0, 2.2F, -6.1F, -7.6F);
			m1.SetColumn(1, -3.4F, 7.2F, 8.7F);
			m1.SetColumn(2, 1.6F, 5.5F, -9.8F);

			var m2 = new Matrix3D();
			m2.SetRow(0, -1.1F, 2.6F, -7.1F);
			m2.SetRow(1, 4.6F, -3.7F, 9.1F);
			m2.SetRow(2, 4.1F, -3.1F, 7.7F);

			var result = new Matrix3D();
			result.SetRow(0, 3.3F, -6F, 8.7F);
			result.SetRow(1, -10.7F, 10.9F, -3.6F);
			result.SetRow(2, -11.7F, 11.8F, -17.5F);

			Assert.IsTrue(Matrix3D.AreEqual(m1 - m2, result));
		}

		[Test]
		public void TestMultiplyScalar()
		{
			var m = new Matrix3D();
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			var result = new Matrix3D();
			result.SetRow(0, -3.41F, 8.06F, -22.01F);
			result.SetRow(1, 14.26F, -11.47F, 28.21F);
			result.SetRow(2, 12.71F, -9.61F, 23.87F);

			Assert.IsTrue(Matrix3D.AreEqual(m*3.1F, result));
		}

		[Test]
		public void TestDivideScalar()
		{
			var m = new Matrix3D();
			m.SetRow(0, -3.41F, 8.06F, -22.01F);
			m.SetRow(1, 14.26F, -11.47F, 28.21F);
			m.SetRow(2, 12.71F, -9.61F, 23.87F);

			var result = new Matrix3D();
			result.SetRow(0, -1.1F, 2.6F, -7.1F);
			result.SetRow(1, 4.6F, -3.7F, 9.1F);
			result.SetRow(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix3D.AreEqual(m/3.1F, result));
		}

		[Test]
		public void TestMultiplyMatrix()
		{
			const float a11 = 1.1f;
			const float a12 = 2.1f;
			const float a13 = 3.1f;
			const float a21 = 1.2f;
			const float a31 = 1.3f;
			const float a22 = 2.2f;
			const float a32 = 2.3f;
			const float a23 = 3.2f;
			const float a33 = 3.3f;
			const float b11 = 9.1f;
			const float b12 = 8.1f;
			const float b13 = 7.1f;
			const float b21 = 9.2f;
			const float b22 = 8.2f;
			const float b23 = 7.2f;
			const float b31 = 9.3f;
			const float b32 = 8.3f;
			const float b33 = 7.3f;

			var m1 = new Matrix3D(new[,] {{a11, a12, a13}, {a21, a22, a23}, {a31, a32, a33}});
			var m2 = new Matrix3D(new[,] {{b11, b12, b13}, {b21, b22, b23}, {b31, b32, b33}});

			var result = new Matrix3D();
			result.SetRow(0,
			              a11*b11 + a12*b21 + a13*b31,
			              a11*b12 + a12*b22 + a13*b32,
			              a11*b13 + a12*b23 + a13*b33);
			result.SetRow(1,
			              a21*b11 + a22*b21 + a23*b31,
			              a21*b12 + a22*b22 + a23*b32,
			              a21*b13 + a22*b23 + a23*b33);
			result.SetRow(2,
			              a31*b11 + a32*b21 + a33*b31,
			              a31*b12 + a32*b22 + a33*b32,
			              a31*b13 + a32*b23 + a33*b33);

			Assert.IsTrue(Matrix3D.AreEqual(m1*m2, result));
		}

		[Test]
		public void TestMultiplyVector()
		{
			const float a11 = 1.1f;
			const float a12 = 2.1f;
			const float a13 = 3.1f;
			const float a21 = 1.2f;
			const float a31 = 1.3f;
			const float a22 = 2.2f;
			const float a32 = 2.3f;
			const float a23 = 3.2f;
			const float a33 = 3.3f;
			const float b1 = 0.1f;
			const float b2 = 0.2f;
			const float b3 = 0.3f;

			var m = new Matrix3D(new[,] {{a11, a12, a13}, {a21, a22, a23}, {a31, a32, a33}});
			var v = new Vector3D(b1, b2, b3);

			var r1 = new Vector3D(a11*b1 + a12*b2 + a13*b3,
			                      a21*b1 + a22*b2 + a23*b3,
			                      a31*b1 + a32*b2 + a33*b3);

			Assert.IsTrue(Vector3D.AreEqual(m*v, r1));

			var r2 = new Vector3D(b1*a11 + b2*a21 + b3*a31,
			                      b1*a12 + b2*a22 + b3*a32,
			                      b1*a13 + b2*a23 + b3*a33);

			Assert.IsTrue(Vector3D.AreEqual(v*m, r2));
		}

		[Test]
		public void TestIdentity()
		{
			var identity = new Matrix3D();
			identity.SetRow(0, 1F, 0F, 0F);
			identity.SetRow(1, 0F, 1F, 0F);
			identity.SetRow(2, 0F, 0F, 1F);

			Assert.IsTrue(identity.IsIdentity);
			Assert.IsTrue(Matrix3D.AreEqual(identity, Matrix3D.GetIdentity()));

			identity[0, 1] = 1F;
			Assert.IsFalse(identity.IsIdentity);
		}

		[Test]
		public void TestTranspose()
		{
			var m = new Matrix3D();
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			var result = new Matrix3D();
			result.SetColumn(0, -1.1F, 2.6F, -7.1F);
			result.SetColumn(1, 4.6F, -3.7F, 9.1F);
			result.SetColumn(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix3D.AreEqual(m.Transpose(), result));
		}

		[Test]
		public void TestDeterminant()
		{
			var m = Matrix3D.GetIdentity();

			Assert.AreEqual(1, m.GetDeterminant());

			m.SetRow(0, 1, 0, 0);
			m.SetRow(1, 0, 1, 0);
			m.SetRow(2, 0, 5, 0);

			Assert.AreEqual(0, m.GetDeterminant());

			m = new Matrix3D(new float[,]
			                 	{
			                 		{1, 2, 3},
			                 		{4, 5, 6},
			                 		{7, 8, 9}
			                 	});

			Assert.AreEqual(1*5*9 + 2*6*7 + 3*4*8 - 7*5*3 - 8*6*1 - 9*2*4, m.GetDeterminant());
		}

		[Test]
		public void TestInverse()
		{
			// test identity inverse
			var m = Matrix3D.GetIdentity();

			Assert.IsTrue(Matrix3D.AreEqual(m.Invert(), new Matrix3D(new float[,] {{1, 0, 0}, {0, 1, 0}, {0, 0, 1}})));

			// test a known inverse
			m = new Matrix3D(new[,]
			                 	{
			                 		{0.100f, 0.200f, 0.300f},
			                 		{-0.400f, 0.500f, 0.600f},
			                 		{0.700f, 0.800f, 0.900f}
			                 	});

			var r = new Matrix3D(new[,]
			                     	{
			                     		{0.62500f, -1.25000f, 0.62500f},
			                     		{-16.25000f, 2.50000f, 3.75000f},
			                     		{13.95833f, -1.25000f, -2.70833f}
			                     	});

			Assert.IsTrue(Matrix3D.AreEqual(m.Invert(), r, 0.00001f));

			// test inverse multiplied against original is the identity
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix3D.AreEqual(m*m.Invert(), Matrix3D.GetIdentity(), 0.00001f));
			Assert.IsTrue(Matrix3D.AreEqual(m.Invert()*m, Matrix3D.GetIdentity(), 0.00001f));

			// test non-invertible
			m.SetRow(0, 1, 0, 0);
			m.SetRow(1, 0, 1, 0);
			m.SetRow(2, 0, 5, 0);

			try
			{
				m.Invert();
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestAccessor()
		{
			var m = new Matrix3D();
			m.SetRow(0, 1F, 0F, 0F);
			m.SetRow(1, 0F, 2F, 0F);
			m.SetRow(2, 0F, 0F, 3F);

			Assert.AreEqual(m[0, 0], 1F);
			Assert.AreEqual(m[1, 1], 2F);
			Assert.AreEqual(m[2, 2], 3F);

			m[0, 2] = 4;
			m[2, 0] = 5;

			Assert.AreEqual(m[0, 2], 4F);
			Assert.AreEqual(m[2, 0], 5F);

			try
			{
				float outOfRange = m[0, 3];
				Assert.Fail("Expected an exception, but instead got a value of {0}", outOfRange);
			}
			catch (ArgumentOutOfRangeException) {}
		}

		[Test]
		public void TestColumnAccessors()
		{
			var m = new Matrix3D();
			m.SetColumn(0, 1F, 2F, 3F);
			m.SetColumn(1, new Vector3D(4f, 5f, 6f));

			Assert.AreEqual(new Vector3D(1, 2, 3), m.GetColumn(0));
			Assert.AreEqual(new Vector3D(4, 5, 6), m.GetColumn(1));
			Assert.AreEqual(new Vector3D(0, 0, 0), m.GetColumn(2));
		}

		[Test]
		public void TestRowAccessors()
		{
			var m = new Matrix3D();
			m.SetRow(0, 1F, 2F, 3F);
			m.SetRow(1, new Vector3D(4f, 5f, 6f));

			Assert.AreEqual(new Vector3D(1, 2, 3), m.GetRow(0));
			Assert.AreEqual(new Vector3D(4, 5, 6), m.GetRow(1));
			Assert.AreEqual(new Vector3D(0, 0, 0), m.GetRow(2));
		}

		[Test]
		public void TestClone()
		{
			var m = new Matrix3D();
			m.SetRow(0, -3.41F, 8.06F, -22.01F);
			m.SetRow(1, 14.26F, -11.47F, 28.21F);
			m.SetRow(2, 12.71F, -9.61F, 23.87F);

			var c = m.Clone();

			Assert.IsTrue(Matrix3D.AreEqual(m, c));

			c[1, 1] = 20;

			Assert.IsFalse(Matrix3D.AreEqual(m, c));
		}
	}
}

#endif