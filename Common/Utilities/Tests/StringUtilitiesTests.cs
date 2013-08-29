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

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class StringUtilitiesTests
	{
		public StringUtilitiesTests() {}

		[Test]
		public void TestDoubleCombine()
		{
			string expectedResult = "2.22, 2.33";
			string result = StringUtilities.Combine<double>(new double[] {2.2222, 2.333}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, NaN";
			result = StringUtilities.Combine<double>(new double[] {2.2222, 2.333, double.NaN}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, Infinity";
			result = StringUtilities.Combine<double>(new double[] {2.2222, 2.333, double.PositiveInfinity}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "2.22, 2.33, -Infinity";
			result = StringUtilities.Combine<double>(new double[] {2.2222, 2.333, double.NegativeInfinity}, ", ", "F2");
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void TestStringCombine()
		{
			#region Values/Separator Combine function

			string expectedResult = "";
			string result = StringUtilities.Combine<string>(new string[] {}, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] {""}, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR, CT, DX";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", "DX"}, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR, DX";
			result = StringUtilities.Combine<string>(new string[] {"MR", "", "DX"}, ", ");
			Assert.AreEqual(expectedResult, result);

			expectedResult = "MR";
			result = StringUtilities.Combine<string>(new string[] {null, "MR", ""}, ", ");
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/SkipEmpty=false Combine Function

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] {}, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] {""}, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", ";
			result = StringUtilities.Combine<string>(new string[] {null, ""}, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", MR, ";
			result = StringUtilities.Combine<string>(new string[] {"", "MR", null}, ", ", false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = ", , MR, ";
			result = StringUtilities.Combine<string>(new string[] {"", null, "MR", ""}, ", ", false);
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/FormattingDelegate Combine Function

			StringUtilities.FormatDelegate<string> noNullCheckFormattingDelegate = delegate(string value) { return String.Format("'{0}'", value); };

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", "DX"}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "", "DX"}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'CT', ''";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", ""}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', 'CT', ''";
			result = StringUtilities.Combine<string>(new string[] {null, "CT", ""}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {null, "", "DX"}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', ''";
			result = StringUtilities.Combine<string>(new string[] {null, "", ""}, ", ", noNullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			//now change the delegate

			StringUtilities.FormatDelegate<string> nullCheckFormattingDelegate = delegate(string value) { return String.IsNullOrEmpty(value) ? "" : String.Format("'{0}'", value); };

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", "DX"}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "", "DX"}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', 'CT'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", ""}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'CT'";
			result = StringUtilities.Combine<string>(new string[] {null, "CT", ""}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'DX'";
			result = StringUtilities.Combine<string>(new string[] {null, "", "DX"}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "";
			result = StringUtilities.Combine<string>(new string[] {null, "", ""}, ", ", nullCheckFormattingDelegate);
			Assert.AreEqual(expectedResult, result);

			#endregion

			#region Values/Separator/FormattingDelegate/SkipEmpty=false Combine Function

			expectedResult = "'MR', 'CT', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "CT", "DX"}, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'MR', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"MR", "", "DX"}, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', 'DX'";
			result = StringUtilities.Combine<string>(new string[] {"", "", "DX"}, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			expectedResult = "'', '', ''";
			result = StringUtilities.Combine<string>(new string[] {"", "", null}, ", ", noNullCheckFormattingDelegate, false);
			Assert.AreEqual(expectedResult, result);

			#endregion
		}

		[Test]
		public void TestToHexString()
		{
			var data = new byte[4096];
			new PseudoRandom().NextBytes(data);

			Assert.AreEqual(string.Empty, StringUtilities.ToHexString(new byte[0]), "Case 0");

			var expected = BitConverter.ToString(data).Replace("-", string.Empty).ToUpperInvariant();
			Assert.AreEqual(expected, StringUtilities.ToHexString(data), "Case 1");
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, false), "Case 1a");
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, 0, data.Length), "Case 1b");
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, 0, data.Length, false), "Case 1c");

			expected = BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant();
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, true), "Case 2");
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, 0, data.Length, true), "Case 2a");

			expected = BitConverter.ToString(data).Replace("-", string.Empty).ToUpperInvariant().Substring(10, 186);
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, 5, 93), "Case 3");
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, 5, 93, false), "Case 3a");
			Assert.AreEqual(expected.ToLowerInvariant(), StringUtilities.ToHexString(data, 5, 93, true), "Case 3b");

			try
			{
				StringUtilities.ToHexString(data, -1, 50);
				Assert.Fail("Case 4");
			}
			catch (ArgumentException) {}

			try
			{
				StringUtilities.ToHexString(data, 0, data.Length + 1);
				Assert.Fail("Case 4a");
			}
			catch (ArgumentException) {}

			try
			{
				StringUtilities.ToHexString(data, 100, data.Length - 100 + 1);
				Assert.Fail("Case 4b");
			}
			catch (ArgumentException) {}

			var arithmeticImplementation = new Func<byte[], string>(bytes =>
			                                                        	{
			                                                        		if (bytes == null) return string.Empty;
			                                                        		var chars = new char[bytes.Length*2];
			                                                        		for (var i = 0; i < bytes.Length; ++i)
			                                                        		{
			                                                        			var b = ((byte) (bytes[i] >> 4));
			                                                        			chars[i*2] = (char) (b > 9 ? b + 0x57 : b + 0x30);
			                                                        			b = ((byte) (bytes[i] & 0xF));
			                                                        			chars[i*2 + 1] = (char) (b > 9 ? b + 0x57 : b + 0x30);
			                                                        		}
			                                                        		return new string(chars);
			                                                        	});
			expected = arithmeticImplementation.Invoke(data);
			Assert.AreEqual(expected, StringUtilities.ToHexString(data, true), "Case 5");
		}
	}
}

#endif