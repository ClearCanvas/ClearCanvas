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
	public class MatrixTests
	{
		private const float _tolerance = 0.00005f;

		[Test]
		public void TestConstructor()
		{
			var m = new Matrix(3, 3);
			m.SetColumn(0, 0, 0, 0);
			m.SetColumn(1, 0, 0, 0);
			m.SetColumn(2, 0, 0, 0);
			Assert.IsTrue(Matrix.AreEqual(m, new Matrix(3, 3)));

			m.SetRow(0, 1, 2, 3);
			m.SetRow(1, 4, 5, 6);
			m.SetRow(2, 7, 8, 9);
			Assert.IsTrue(Matrix.AreEqual(m, new Matrix(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}})));
			Assert.IsTrue(Matrix.AreEqual(m, new Matrix(new Matrix(new float[,] {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}}))));

			try
			{
				new Matrix(0, 0);
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				new Matrix(new Matrix(4, -5));
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				new Matrix((Matrix) null);
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentNullException) {}

			try
			{
				new Matrix((float[,]) null);
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentNullException) {}
		}

		[Test]
		public void TestAreEqual()
		{
			var m1 = new Matrix(3, 3);
			m1.SetColumn(0, 1, 4, 7);
			m1.SetColumn(1, 2, 5, 8);
			m1.SetColumn(2, 3, 6, 9);

			var m2 = new Matrix(3, 3);
			m2.SetRow(0, 1, 2, 3);
			m2.SetRow(1, 4, 5, 6);
			m2.SetRow(2, 7, 8, 9);

			Assert.IsTrue(Matrix.AreEqual(m1, m2));

			m2[1, 1] = 0;
			Assert.IsFalse(Matrix.AreEqual(m1, m2));
		}

		[Test]
		public void TestNegate()
		{
			var m = -new Matrix(new float[,]
			                    	{
			                    		{1, -2, 3},
			                    		{-4, 5, -6},
			                    		{7, -8, 9}
			                    	});

			Assert.IsTrue(Matrix.AreEqual(m, new Matrix(new float[,] {{-1, 2, -3}, {4, -5, 6}, {-7, 8, -9}})));
		}

		[Test]
		public void TestAdd()
		{
			Matrix m1 = new Matrix(3, 3);
			m1.SetColumn(0, 2.2F, -6.1F, -7.6F);
			m1.SetColumn(1, -3.4F, 7.2F, 8.7F);
			m1.SetColumn(2, 1.6F, 5.5F, -9.8F);

			Matrix m2 = new Matrix(3, 3);
			m2.SetRow(0, -1.1F, 2.6F, -7.1F);
			m2.SetRow(1, 4.6F, -3.7F, 9.1F);
			m2.SetRow(2, 4.1F, -3.1F, 7.7F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, 1.1F, -0.8F, -5.5F);
			result.SetRow(1, -1.5F, 3.5F, 14.6F);
			result.SetRow(2, -3.5F, 5.6F, -2.1F);

			Assert.IsTrue(Matrix.AreEqual(m1 + m2, result));
		}

		[Test]
		public void TestSubtract()
		{
			Matrix m1 = new Matrix(3, 3);
			m1.SetColumn(0, 2.2F, -6.1F, -7.6F);
			m1.SetColumn(1, -3.4F, 7.2F, 8.7F);
			m1.SetColumn(2, 1.6F, 5.5F, -9.8F);

			Matrix m2 = new Matrix(3, 3);
			m2.SetRow(0, -1.1F, 2.6F, -7.1F);
			m2.SetRow(1, 4.6F, -3.7F, 9.1F);
			m2.SetRow(2, 4.1F, -3.1F, 7.7F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, 3.3F, -6F, 8.7F);
			result.SetRow(1, -10.7F, 10.9F, -3.6F);
			result.SetRow(2, -11.7F, 11.8F, -17.5F);

			Assert.IsTrue(Matrix.AreEqual(m1 - m2, result));
		}

		[Test]
		public void TestMultiplyScalar()
		{
			Matrix m = new Matrix(3, 3);
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, -3.41F, 8.06F, -22.01F);
			result.SetRow(1, 14.26F, -11.47F, 28.21F);
			result.SetRow(2, 12.71F, -9.61F, 23.87F);

			Assert.IsTrue(Matrix.AreEqual(m*3.1F, result));
		}

		[Test]
		public void TestDivideScalar()
		{
			Matrix m = new Matrix(3, 3);
			m.SetRow(0, -3.41F, 8.06F, -22.01F);
			m.SetRow(1, 14.26F, -11.47F, 28.21F);
			m.SetRow(2, 12.71F, -9.61F, 23.87F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, -1.1F, 2.6F, -7.1F);
			result.SetRow(1, 4.6F, -3.7F, 9.1F);
			result.SetRow(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix.AreEqual(m/3.1F, result));
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

			var m1 = new Matrix(new[,] {{a11, a12, a13}, {a21, a22, a23}, {a31, a32, a33}});
			var m2 = new Matrix(new[,] {{b11, b12, b13}, {b21, b22, b23}, {b31, b32, b33}});

			var result = new Matrix(3, 3);
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

			Assert.IsTrue(Matrix.AreEqual(m1*m2, result));
		}

		[Test]
		public void TestIdentity()
		{
			Matrix identity = new Matrix(4, 4);
			identity.SetRow(0, 1F, 0F, 0F, 0F);
			identity.SetRow(1, 0F, 1F, 0F, 0F);
			identity.SetRow(2, 0F, 0F, 1F, 0F);
			identity.SetRow(3, 0F, 0F, 0F, 1F);

			Assert.IsTrue(identity.IsSquare);
			Assert.IsTrue(identity.IsIdentity);
			Assert.IsTrue(Matrix.AreEqual(identity, Matrix.GetIdentity(4)));

			identity[0, 1] = 1F;
			Assert.IsFalse(identity.IsIdentity);
		}

		public void TestTranspose()
		{
			Matrix m = new Matrix(2, 3);
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);

			Matrix result = new Matrix(3, 2);
			result.SetColumn(0, -1.1F, 2.6F, -7.1F);
			result.SetColumn(1, 4.6F, -3.7F, 9.1F);

			Assert.IsTrue(Matrix.AreEqual(m.Transpose(), result));
		}

		[Test]
		public void TestDeterminant()
		{
			try
			{
				var m = new Matrix(3, 2);
				m.GetDeterminant();
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				var m = Matrix.GetIdentity(5);
				m.GetDeterminant();
				Assert.Fail("Expected an exception");
			}
			catch (NotImplementedException) {}
		}

		[Test]
		public void TestDeterminant1()
		{
			var m = Matrix.GetIdentity(1);

			Assert.AreEqual(1, m.GetDeterminant());

			m.SetRow(0, 0);

			Assert.AreEqual(0, m.GetDeterminant());

			m = new Matrix(new float[,]
			               	{
			               		{2}
			               	});

			Assert.AreEqual(2, m.GetDeterminant());
		}

		[Test]
		public void TestDeterminant2()
		{
			var m = Matrix.GetIdentity(2);

			Assert.AreEqual(1, m.GetDeterminant());

			m.SetRow(0, 1, 0);
			m.SetRow(1, 5, 0);

			Assert.AreEqual(0, m.GetDeterminant());

			m = new Matrix(new float[,]
			               	{
			               		{1, 2},
			               		{4, 3}
			               	});

			Assert.AreEqual(1*3 - 2*4, m.GetDeterminant());
		}

		[Test]
		public void TestDeterminant3()
		{
			var m = Matrix.GetIdentity(3);

			Assert.AreEqual(1, m.GetDeterminant());

			m.SetRow(0, 1, 0, 0);
			m.SetRow(1, 0, 1, 0);
			m.SetRow(2, 0, 5, 0);

			Assert.AreEqual(0, m.GetDeterminant());

			m = new Matrix(new float[,]
			               	{
			               		{1, 2, 3},
			               		{6, 5, 4},
			               		{8, 7, 9}
			               	});

			Assert.AreEqual(1*5*9 + 2*4*8 + 3*7*6 - 8*5*3 - 7*4*1 - 9*2*6, m.GetDeterminant());
		}

		[Test]
		public void TestDeterminant4()
		{
			var m = Matrix.GetIdentity(4);

			Assert.AreEqual(1, m.GetDeterminant());

			m.SetRow(0, 1, 0, 0, 0);
			m.SetRow(1, 0, 1, 0, 0);
			m.SetRow(2, 0, 0, 1, 0);
			m.SetRow(3, 0, 0, 5, 0);

			Assert.AreEqual(0, m.GetDeterminant());

			m = new Matrix(new float[,]
			               	{
			               		{1, 2, 3, 4},
			               		{8, 7, 6, 5},
			               		{10, 9, 11, 12},
			               		{14, 15, 13, 16}
			               	});

			Assert.AreEqual(-108, m.GetDeterminant());
		}

		[Test]
		public void TestInverse()
		{
			try
			{
				var m = new Matrix(3, 2);
				m.Invert();
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}

			try
			{
				var m = Matrix.GetIdentity(5);
				m.Invert();
				Assert.Fail("Expected an exception");
			}
			catch (NotImplementedException) {}
		}

		[Test]
		public void TestInverse1()
		{
			// test identity inverse
			var m = Matrix.GetIdentity(1);

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), new Matrix(new float[,] {{1}})));

			// test a known inverse
			m = new Matrix(new[,]
			               	{
			               		{0.100f}
			               	});

			var r = new Matrix(new[,]
			                   	{
			                   		{10.00f}
			                   	});

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), r, _tolerance));

			// test inverse multiplied against original is the identity
			m.SetRow(0, -1.1F);

			Assert.IsTrue(Matrix.AreEqual(m*m.Invert(), Matrix.GetIdentity(1), _tolerance));
			Assert.IsTrue(Matrix.AreEqual(m.Invert()*m, Matrix.GetIdentity(1), _tolerance));

			// test non-invertible
			m.SetRow(0, 0);

			try
			{
				m.Invert();
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestInverse2()
		{
			// test identity inverse
			var m = Matrix.GetIdentity(2);

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), new Matrix(new float[,] {{1, 0}, {0, 1}})));

			// test a known inverse
			m = new Matrix(new[,]
			               	{
			               		{0.100f, 0.200f},
			               		{-0.400f, 0.500f}
			               	});

			var r = new Matrix(new[,]
			                   	{
			                   		{3.84615f, -1.53846f},
			                   		{3.07692f, 0.76923f}
			                   	});

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), r, _tolerance));

			// test inverse multiplied against original is the identity
			m.SetRow(0, -1.1F, 2.6F);
			m.SetRow(1, 4.6F, -3.7F);

			Assert.IsTrue(Matrix.AreEqual(m*m.Invert(), Matrix.GetIdentity(2), _tolerance));
			Assert.IsTrue(Matrix.AreEqual(m.Invert()*m, Matrix.GetIdentity(2), _tolerance));

			// test non-invertible
			m.SetRow(0, 1, 0);
			m.SetRow(1, 5, 0);

			try
			{
				m.Invert();
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestInverse3()
		{
			// test identity inverse
			var m = Matrix.GetIdentity(3);

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), new Matrix(new float[,] {{1, 0, 0}, {0, 1, 0}, {0, 0, 1}})));

			// test a known inverse
			m = new Matrix(new[,]
			               	{
			               		{0.100f, 0.200f, 0.300f},
			               		{-0.400f, 0.500f, 0.600f},
			               		{0.700f, 0.800f, 0.900f}
			               	});

			var r = new Matrix(new[,]
			                   	{
			                   		{0.62500f, -1.25000f, 0.62500f},
			                   		{-16.25000f, 2.50000f, 3.75000f},
			                   		{13.95833f, -1.25000f, -2.70833f}
			                   	});

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), r, _tolerance));

			// test inverse multiplied against original is the identity
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix.AreEqual(m*m.Invert(), Matrix.GetIdentity(3), _tolerance));
			Assert.IsTrue(Matrix.AreEqual(m.Invert()*m, Matrix.GetIdentity(3), _tolerance));

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
		public void TestInverse4()
		{
			// test identity inverse
			var m = Matrix.GetIdentity(4);

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), new Matrix(new float[,] {{1, 0, 0, 0}, {0, 1, 0, 0}, {0, 0, 1, 0}, {0, 0, 0, 1}})));

			// test a known inverse
			m = new Matrix(new[,]
			               	{
			               		{0.100f, 0.200f, 0.300f, 1.300f},
			               		{-0.400f, 0.500f, 0.600f, 1.400f},
			               		{0.700f, 0.800f, 0.900f, 1.500f},
			               		{1.000f, 1.100f, 1.200f, -1.600f}
			               	});

			var r = new Matrix(new[,]
			                   	{
			                   		{0.62500f, -1.25000f, 0.62500f, 0.00000f},
			                   		{-18.12500f, 2.50000f, 9.37500f, -3.75000f},
			                   		{15.8854167f, -1.25000f, -8.4895833f, 3.854167f},
			                   		{-0.15625f, 0.00000f, 0.46875f, -0.31250f}
			                   	});

			Assert.IsTrue(Matrix.AreEqual(m.Invert(), r, _tolerance));

			// test inverse multiplied against original is the identity
			m.SetRow(0, -1.1F, 2.6F, -7.1F, 2.2F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F, 6.9F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F, 7.1F);
			m.SetRow(3, -9.9F, 0.2F, 4.3F, 5.5F);

			Assert.IsTrue(Matrix.AreEqual(m*m.Invert(), Matrix.GetIdentity(4), _tolerance));
			Assert.IsTrue(Matrix.AreEqual(m.Invert()*m, Matrix.GetIdentity(4), _tolerance));

			// test non-invertible
			m.SetRow(0, 1, 0, 0, 0);
			m.SetRow(1, 0, 1, 0, 0);
			m.SetRow(2, 0, 0, 1, 0);
			m.SetRow(3, 0, 0, 5, 0);

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
			Matrix m = new Matrix(4, 4);
			m.SetRow(0, 1F, 0F, 0F, 0F);
			m.SetRow(1, 0F, 2F, 0F, 0F);
			m.SetRow(2, 0F, 0F, 3F, 0F);
			m.SetRow(3, 0F, 0F, 0F, 4F);

			Assert.AreEqual(m[0, 0], 1F);
			Assert.AreEqual(m[1, 1], 2F);
			Assert.AreEqual(m[2, 2], 3F);
			Assert.AreEqual(m[3, 3], 4F);

			try
			{
				float outOfRange = m[0, 4];
				Assert.Fail("Expected an exception, but instead for a value of {0}", outOfRange);
			}
			catch (ArgumentOutOfRangeException) {}
		}

		[Test]
		public void TestColumnSetter()
		{
			Matrix m = new Matrix(3, 1);
			m.SetColumn(0, 1F, 2F, 3F);

			try
			{
				//too many.
				m.SetColumn(0, 1F, 2F, 3F, 4F);
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestRowSetter()
		{
			Matrix m = new Matrix(1, 3);
			m.SetRow(0, 1F, 2F, 3F);

			try
			{
				//not enough.
				m.SetRow(0, 1F, 2F);
				Assert.Fail("Expected an exception");
			}
			catch (ArgumentException) {}
		}

		[Test]
		public void TestClone()
		{
			Matrix m = new Matrix(3, 3);
			m.SetRow(0, -3.41F, 8.06F, -22.01F);
			m.SetRow(1, 14.26F, -11.47F, 28.21F);
			m.SetRow(2, 12.71F, -9.61F, 23.87F);

			Assert.IsTrue(Matrix.AreEqual(m, m.Clone()));
		}
	}
}

#endif