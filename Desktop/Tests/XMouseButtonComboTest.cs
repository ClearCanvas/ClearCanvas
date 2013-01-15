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
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class XMouseButtonComboTest
	{
		[Test]
		public void TestTypeConverterAttribute()
		{
			// tests that a TypeConverter is properly defined on the XMouseButtonCombo type
			TypeConverter converter = TypeDescriptor.GetConverter(typeof (XMouseButtonCombo));
			Assert.IsNotNull(converter, "XMouseButtonCombo should be marked with a TypeConverterAttribute.");
			Assert.IsTrue(converter.CanConvertFrom(typeof (string)), "XMouseButtonCombo should have a TypeConverter that supports ConvertFromString");
			Assert.IsTrue(converter.CanConvertTo(typeof (string)), "XMouseButtonCombo should have a TypeConverter that supports ConvertToString");
		}

		[Test]
		public void TestEmptyStringParse()
		{
			// empty strings should parse to XMouseButtonCombo.None
			XMouseButtonCombo mouseButtonCombo = XMouseButtonCombo.Parse(string.Empty);
			Assert.AreEqual(XMouseButtonCombo.None, mouseButtonCombo, "empty strings should parse to XMouseButtonCombo.None");
		}

		[Test]
		public void TestInvalidStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("\n");
				Assert.Fail("Expected an exception because the parsed string has an invalid character");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidModifierOnlyStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("Ctrl");
				Assert.Fail("Expected an exception because the parsed string has no mouse button, only a single modifier");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidModifiersOnlyStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("Ctrl+Shift");
				Assert.Fail("Expected an exception because the parsed string has no mouse button, only modifiers");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidElementStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("NonExistentCombo");
				Assert.Fail("Expected an exception because the parsed string has an invalid element name");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidElementWithNonElementStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("Ctrl+NonExistentCombo");
				Assert.Fail("Expected an exception because the parsed string has an invalid element name");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidNonElementWithElementStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("NonExistentCombo+Left Mouse Button");
				Assert.Fail("Expected an exception because the parsed string has an invalid element name");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidElementWithElementsStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("Shift+NonExistentCombo+Left Mouse Button");
				Assert.Fail("Expected an exception because the parsed string has an invalid element name");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidElementTrailingSeparatorStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("Shift+");
				Assert.Fail("Expected an exception because of a trailing separator in the parse string");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestInvalidElementLeadingSeparatorStringParse()
		{
			try
			{
				// on the other hand, invalid combo strings should throw exceptions during conversion
				XMouseButtonCombo.Parse("+Right Mouse Button");
				Assert.Fail("Expected an exception because of a leading separator in the parse string");
			}
			catch (FormatException) {}
		}

		[Test]
		public void TestEmptyEquality()
		{
			Assert.AreEqual(XMouseButtonCombo.None, XMouseButtonCombo.None, "None should be equal to None.");
			Assert.AreEqual(XMouseButtonCombo.None, new XMouseButtonCombo(), "None should be equal to value constructed with default ctor.");
			Assert.AreEqual(XMouseButtonCombo.None, new XMouseButtonCombo(XMouseButtons.None), "None should be equal to value constructed with no mouse buttons.");
			Assert.AreEqual(XMouseButtonCombo.None, new XMouseButtonCombo(XMouseButtons.None, ModifierFlags.None), "None should be equal to value constructed with no mouse buttons and no modifiers.");
			Assert.AreEqual(XMouseButtonCombo.None, new XMouseButtonCombo(XMouseButtons.None, ModifierFlags.Shift), "None should be equal to value constructed with no mouse buttons and non-zero modifiers.");
			Assert.AreEqual(XMouseButtonCombo.None, (XMouseButtonCombo) XMouseButtons.None, "None should be equal to value cast from no mouse buttons.");
			Assert.AreEqual(XMouseButtonCombo.None, (XMouseButtonCombo) XKeys.None, "None should be equal to value cast from no XKeys.");
			Assert.AreEqual(XMouseButtonCombo.None, (XMouseButtonCombo) (XKeys.Control | XKeys.None), "None should be equal to value cast from modified XKeys.None.");

			XMouseButtonCombo value = new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Shift);
			value.MouseButtons = XMouseButtons.None;
			Assert.AreEqual(XMouseButtonCombo.None, value, "None should be equal to value which had a modifier and has had its mouse button field modified to be empty.");

			XMouseButtonCombo value2 = new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.None);
			value2.MouseButtons = XMouseButtons.None;
			Assert.AreEqual(XMouseButtonCombo.None, value2, "None should be equal to value which has had its mouse button field modified to be empty.");

			XMouseButtonCombo value3 = new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Shift);
			value3.Modifiers = ModifierFlags.None;
			value3.MouseButtons = XMouseButtons.None;
			Assert.AreEqual(XMouseButtonCombo.None, value3, "None should be equal to value which has had its fields modified to be empty.");
		}

		[Test]
		public void TestEquality()
		{
			XMouseButtonCombo value = new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Shift);
			value.MouseButtons = XMouseButtons.XButton2;
			value.Modifiers = ModifierFlags.None;
			Assert.AreEqual(new XMouseButtonCombo(XMouseButtons.XButton2), new XMouseButtonCombo(XMouseButtons.XButton2, ModifierFlags.None));
			Assert.AreEqual(new XMouseButtonCombo(XMouseButtons.XButton2), new XMouseButtonCombo(XMouseButtons.XButton2, ModifierFlags.None));
			Assert.AreEqual(new XMouseButtonCombo(XMouseButtons.XButton2), value);
		}

		[Test]
		public void TestCastXMouseButtons()
		{
			Assert.AreEqual((XMouseButtonCombo) XMouseButtons.Left, new XMouseButtonCombo(XMouseButtons.Left), "XMouseButtons should be equal to equivalent XMouseButtonCombo");
			Assert.AreEqual(XMouseButtons.Left, new XMouseButtonCombo(XMouseButtons.Left).MouseButtons, "XMouseButtons should be equal to equivalent XMouseButtonCombo");
		}

		[Test]
		public void TestCastXKeys()
		{
			Assert.AreNotEqual((XKeys.Control | XKeys.LeftMouseButton), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control), "cast XKeys to XMouseButtonCombo should be explicit");
			Assert.AreEqual((XMouseButtonCombo) (XKeys.Control | XKeys.LeftMouseButton), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control), "explicit XKeys should be equal to equivalent XMouseButtonCombo");
			Assert.AreNotEqual(XKeys.Control | XKeys.LeftMouseButton, new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control), "cast XMouseButtonCombo to XKeys should be explicit");
			Assert.AreEqual(XKeys.Control | XKeys.LeftMouseButton, (XKeys) new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control), "explicit XKeys should be equal to equivalent XMouseButtonCombo");
		}

		[Test]
		public void TestInvalidCastFromXKeys()
		{
			try
			{
				XMouseButtonCombo combo = (XMouseButtonCombo) (XKeys.Shift | XKeys.Attn);
				Assert.Fail("Casting a key stroke as an XMouseButtonCombo should be invalid");
			}
			catch (InvalidCastException) {}
		}

		[Test]
		public void TestInvalidCastToXKeys()
		{
			try
			{
				XKeys keys = (XKeys) new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.None);
				Assert.Fail("Casting a multi-button combo as an XKeys should be invalid");
			}
			catch (InvalidCastException) {}
		}

		[Test]
		public void TestInvariantCombos()
		{
			// test combining buttons with modifiers in the invariant case
			const string message = "Invariant Combinations";
			CultureInfo culture = CultureInfo.InvariantCulture;

			AssertEquivalency(string.Format("Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left), culture, message);
			AssertEquivalency(string.Format("Ctrl+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control), culture, message);
			AssertEquivalency(string.Format("Alt+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Alt), culture, message);
			AssertEquivalency(string.Format("Shift+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control | ModifierFlags.Alt), culture, message);
			AssertEquivalency(string.Format("Alt+Shift+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Alt | ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Shift+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control | ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt+Shift+Left Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left, ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift), culture, message);
		}

		[Test]
		public void TestInvariantMultiCombos()
		{
			// test combining multiple buttons with modifiers in the invariant case
			const string message = "Invariant Multi-Button Combinations";
			CultureInfo culture = CultureInfo.InvariantCulture;

			AssertEquivalency(string.Format("Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right), culture, message);
			AssertEquivalency(string.Format("Ctrl+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Control), culture, message);
			AssertEquivalency(string.Format("Alt+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Alt), culture, message);
			AssertEquivalency(string.Format("Shift+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Control | ModifierFlags.Alt), culture, message);
			AssertEquivalency(string.Format("Alt+Shift+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Alt | ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Shift+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Control | ModifierFlags.Shift), culture, message);
			AssertEquivalency(string.Format("Ctrl+Alt+Shift+Left Mouse Button+Right Mouse Button"), new XMouseButtonCombo(XMouseButtons.Left | XMouseButtons.Right, ModifierFlags.Control | ModifierFlags.Alt | ModifierFlags.Shift), culture, message);
		}

		private static void AssertEquivalency(string sMouseButtonCombo, XMouseButtonCombo vMouseButtonCombo, CultureInfo culture, string message)
		{
			AssertStringFormat(sMouseButtonCombo, vMouseButtonCombo, culture, message);
			AssertStringParse(sMouseButtonCombo, vMouseButtonCombo, culture, message);
		}

		private static void AssertStringFormat(string sMouseButtonCombo, XMouseButtonCombo vMouseButtonCombo, CultureInfo culture, string message)
		{
			string actualString = vMouseButtonCombo.ToString(culture);
			//System.Diagnostics.Trace.WriteLine(actualString);
			Assert.AreEqual(sMouseButtonCombo, actualString, "{0}: converting [{1},{2}] which is {3}", message, vMouseButtonCombo.MouseButtons, vMouseButtonCombo.Modifiers, vMouseButtonCombo);
		}

		private static void AssertStringParse(string sMouseButtonCombo, XMouseButtonCombo vMouseButtonCombo, CultureInfo culture, string message)
		{
			XMouseButtonCombo actualValue = XMouseButtonCombo.Parse(sMouseButtonCombo, culture);
			//System.Diagnostics.Trace.WriteLine(actualValue);
			Assert.AreEqual(vMouseButtonCombo, actualValue, "{0}: converting {1} which is {2}", message, sMouseButtonCombo, actualValue);
		}
	}
}

#endif