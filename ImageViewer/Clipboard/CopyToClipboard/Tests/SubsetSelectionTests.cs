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

#pragma warning disable 0419,1574,1587,1591

using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Clipboard.CopyToClipboard.Tests
{
	using Range = CopySubsetToClipboardComponent.Range;

	[TestFixture]
	public class SubsetSelectionTests
	{
		public SubsetSelectionTests()
		{
		}

		[Test]
		public void TestSuccess1()
		{
			string test = "1, 3, 5, 7-9, 12-";
			List<Range> expected = new List<Range>(new Range[] { 
				new Range(1, 1), 
				new Range(3, 3), 
				new Range(5, 5), 
				new Range(7, 9), 
				new Range(12, 20)});

			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestSuccess2()
		{
			string test = "1, 3, 5, 7-";
			List<Range> expected = new List<Range>(new Range[]
			                                       	{
			                                       		new Range(1, 1), 
														new Range(3, 3), 
														new Range(5, 5), 
														new Range(7, 20)});

			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestSuccess3()
		{
			string test = "-7, 10, 15, 20";
			List<Range> expected = new List<Range>(new Range[]
			                                       	{
			                                       		new Range(1, 7), 
														new Range(10, 10), 
														new Range(15, 15), 
														new Range(20, 20)});

			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsTrue(result, "Parse failed.");

			CompareLists(expected, values);
		}

		[Test]
		public void TestFail1()
		{
			string test = "-7, 10, 15, 2a";
			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail2()
		{
			string test = "3, 5,6,-";
			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail3()
		{
			string test = "3,, 5,6";
			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		[Test]
		public void TestFail4()
		{
			string test = "3, 5,6,12-a";
			List<Range> values;
			bool result = CopySubsetToClipboardComponent.CustomImageSelectionStrategy.Parse(test, 1, 20, out values);
			Assert.IsFalse(result, "Parse succeeded when it should have failed.");
		}

		private void CompareLists(List<Range> expected, List<Range> values)
		{
			Assert.AreEqual(expected.Count, values.Count, "The two lists are not the same size.");
			for(int i = 0; i < expected.Count; ++i)
			{
				if (!expected[i].Equals(values[i]))
					Assert.Fail("The two lists are not identical.");
			}
		}
	}
}

#endif