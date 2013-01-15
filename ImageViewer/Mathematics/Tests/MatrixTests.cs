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

using NUnit.Framework;
using System;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class MatrixTests
	{
		public MatrixTests()
		{
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
		public void TestMultiply()
		{
			Matrix m = new Matrix(3, 3);
			m.SetRow(0, -1.1F, 2.6F, -7.1F);
			m.SetRow(1, 4.6F, -3.7F, 9.1F);
			m.SetRow(2, 4.1F, -3.1F, 7.7F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, -3.41F, 8.06F, -22.01F);
			result.SetRow(1, 14.26F, -11.47F, 28.21F);
			result.SetRow(2, 12.71F, -9.61F, 23.87F);

			Assert.IsTrue(Matrix.AreEqual(m * 3.1F, result));
		}

		[Test]
		public void TestDivide()
		{
			Matrix m = new Matrix(3, 3);
			m.SetRow(0, -3.41F, 8.06F, -22.01F);
			m.SetRow(1, 14.26F, -11.47F, 28.21F);
			m.SetRow(2, 12.71F, -9.61F, 23.87F);

			Matrix result = new Matrix(3, 3);
			result.SetRow(0, -1.1F, 2.6F, -7.1F);
			result.SetRow(1, 4.6F, -3.7F, 9.1F);
			result.SetRow(2, 4.1F, -3.1F, 7.7F);

			Assert.IsTrue(Matrix.AreEqual(m / 3.1F, result));
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
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
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

			float outOfRange = m[0, 4];
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestColumnSetter()
		{
			Matrix m = new Matrix(3, 1);
			m.SetColumn(0, 1F, 2F, 3F);

			//too many.
			m.SetColumn(0, 1F, 2F, 3F, 4F);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestRowSetter()
		{
			Matrix m = new Matrix(1, 3);
			m.SetRow(0, 1F, 2F, 3F);

			//not enough.
			m.SetRow(0, 1F, 2F);
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

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestZeroSize()
		{
			Matrix m = new Matrix(0, 0);
		}
	}
}

#endif