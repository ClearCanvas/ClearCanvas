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

using ClearCanvas.Dicom.Iod;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Tests
{
	[TestFixture]
	public class DicomStringHelperTests
	{
		public DicomStringHelperTests()
		{
		}

		[Test]
		public void TestStringArrayConverter()
		{
			string input = null;
			string[] output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"the\lazy\\brown\dog";
			output = DicomStringHelper.GetStringArray(input);
			Assert.AreEqual(output[0], "the");
			Assert.AreEqual(output[1], "lazy");
            Assert.AreEqual(output[2], "");
            Assert.AreEqual(output[3], "brown");
			Assert.AreEqual(output[4], "dog");

            input = @"\the\lazy\brown\dog";
            output = DicomStringHelper.GetStringArray(input);
            Assert.AreEqual(output[0], "");
            Assert.AreEqual(output[1], "the");
            Assert.AreEqual(output[2], "lazy");
            Assert.AreEqual(output[3], "brown");
            Assert.AreEqual(output[4], "dog");

            input = @"the\lazy\brown\dog\";
            output = DicomStringHelper.GetStringArray(input);
            Assert.AreEqual(output[0], "the");
            Assert.AreEqual(output[1], "lazy");
            Assert.AreEqual(output[2], "brown");
            Assert.AreEqual(output[3], "dog");
            Assert.AreEqual(output[4], "");
        }

		[Test]
		public void TestDoubleArrayConverter()
		{
			string input = null;
			double[] output;
		    Assert.IsTrue(DicomStringHelper.TryGetDoubleArray(input, out output));
			Assert.AreEqual(output.Length, 0);

			input = "";
		    Assert.IsTrue(DicomStringHelper.TryGetDoubleArray(input, out output));
			Assert.AreEqual(output.Length, 0);

			input = @"0\1.2\2.3";
		    Assert.IsTrue(DicomStringHelper.TryGetDoubleArray(input, out output));
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1.2);
			Assert.AreEqual(output[2], 2.3);

            input = @"0\\1.2\2.3";
		    Assert.IsFalse(DicomStringHelper.TryGetDoubleArray(input, out output));
        }
		
		[Test]
		public void TestIntArrayConverter()
		{
			string input = null;
			int[] output;
		    Assert.IsTrue(DicomStringHelper.TryGetIntArray(input, out output));
			Assert.AreEqual(output.Length, 0);

			input = "";
		    Assert.IsTrue(DicomStringHelper.TryGetIntArray(input, out output));
			Assert.AreEqual(output.Length, 0);

			input = @"0\1\30";
		    Assert.IsTrue(DicomStringHelper.TryGetIntArray(input, out output));
			Assert.AreEqual(output[0], 0);
			Assert.AreEqual(output[1], 1);
			Assert.AreEqual(output[2], 30);

            input = @"\0\1\30";
            Assert.IsFalse(DicomStringHelper.TryGetIntArray(input, out output));
        }

		[Test]
		public void TestPersonNameArrayConverter()
		{
			string input = null;
			PersonName[] output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = "";
			output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output.Length, 0);

			input = @"Doe^John^^^\Doe^Jane^^^";
			output = DicomStringHelper.GetPersonNameArray(input);
			Assert.AreEqual(output[0].FirstName, "John");
			Assert.AreEqual(output[0].LastName, "Doe");

			Assert.AreEqual(output[1].FirstName, "Jane");
			Assert.AreEqual(output[1].LastName, "Doe");

            input = @"\Doe^Jane^^^";
            output = DicomStringHelper.GetPersonNameArray(input);
            Assert.AreEqual(output[0].FirstName ?? "", "");
            Assert.AreEqual(output[0].LastName ?? "", "");

            Assert.AreEqual(output[1].FirstName, "Jane");
            Assert.AreEqual(output[1].LastName, "Doe");

            input = @"Doe^Jane^^^\";
            output = DicomStringHelper.GetPersonNameArray(input);

            Assert.AreEqual(output[0].FirstName, "Jane");
            Assert.AreEqual(output[0].LastName, "Doe");

            Assert.AreEqual(output[1].FirstName ?? "", "");
            Assert.AreEqual(output[1].LastName ?? "", "");
        }
    }
}

#endif