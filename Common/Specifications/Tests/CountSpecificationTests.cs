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

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class CountSpecificationTests : TestsBase
	{
		[Test]
		public void Test_MinMaxBothZero()
		{
			CountSpecification s = new CountSpecification(0, 0, null);
			Assert.IsTrue(s.Test(new int[] {}).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		public void Test_MinMaxBothLargest()
		{
			// this should always be false unless someone has fills an array with MaxValue items
			CountSpecification s = new CountSpecification(int.MaxValue, int.MaxValue, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		public void Test_MinMaxBothEqualAndNonZero()
		{
			CountSpecification s = new CountSpecification(2, 2, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsFalse(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		public void Test_MinMaxRange1()
		{
			CountSpecification s = new CountSpecification(1, 2, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsTrue(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		public void Test_MinMaxRange()
		{
			CountSpecification s = new CountSpecification(1, 4, null);
			Assert.IsFalse(s.Test(new int[] { }).Success);
			Assert.IsTrue(s.Test(new int[] { 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3, 4 }).Success);
			Assert.IsFalse(s.Test(new int[] { 1, 2, 3, 4, 5 }).Success);
		}

		[Test]
		public void Test_Filter()
		{
			CountSpecification s1 = new CountSpecification(1, 1, AlwaysTrue);
			Assert.IsFalse(s1.Test(new int[] { }).Success);
			Assert.IsTrue(s1.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s1.Test(new int[] { 1, 2 }).Success);

			CountSpecification s2 = new CountSpecification(1, 1, AlwaysFalse);
			Assert.IsFalse(s2.Test(new int[] { }).Success);
			Assert.IsFalse(s2.Test(new int[] { 1 }).Success);
			Assert.IsFalse(s2.Test(new int[] { 1, 2 }).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_InvalidRange()
		{
			CountSpecification s = new CountSpecification(1, 0, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_NegativeRange()
		{
			CountSpecification s = new CountSpecification(-1, 0, null);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_NonEnumerable()
		{
			CountSpecification s = new CountSpecification(1, 1, null);

			// testing an object that does not implement IEnumerable should throw
			Assert.IsFalse(s.Test(new object()).Success);
		}
	}
}

#endif
