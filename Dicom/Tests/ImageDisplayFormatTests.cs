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

using NUnit.Framework;
using ClearCanvas.Dicom.Iod.Modules;
using System;

namespace ClearCanvas.Dicom.Tests
{
	//TODO (CR March 2011): Move to Iod.Modules.Tests
	[TestFixture]
	public class ImageDisplayFormatTests
	{
		// Type of image display format. Enumerated Values:
		// STANDARD\C,R
		// ROW\R1,R2,R3, etc.
		// COL\C1,C2,C3, etc.
		// SLIDE
		// SUPERSLIDE
		// CUSTOM\i

		[Test]
		public void Null_DicomString_Test()
		{
			var format = ImageDisplayFormat.FromDicomString(null);
			Assert.IsNull(format);
		}

		[Test]
		public void Empty_DicomString_Test()
		{
			var format = ImageDisplayFormat.FromDicomString(string.Empty);
			Assert.IsNull(format);
		}
		
		[Test]
		public void Supported_Formats_Test()
		{
			var supportedFormats = ImageDisplayFormat.StandardFormats;
			Assert.IsNotEmpty(supportedFormats);
		}

		[Test]
		public void Standard_Format_Test()
		{
			var format = ImageDisplayFormat.FromDicomString(@"STANDARD\7,9");
			Assert.AreEqual(format.DicomString, @"STANDARD\7,9");
			Assert.AreEqual(format.Format, ImageDisplayFormat.FormatEnum.STANDARD);
			Assert.AreEqual(format.MaximumImageBoxes, 7 * 9);
			Assert.AreEqual(format.Modifiers.Count, 2);
			Assert.AreEqual(format.Modifiers[0], 7);
			Assert.AreEqual(format.Modifiers[1], 9);
		}

		[Test]
		public void Row_Format_Test()
		{
			var simpleRowFormat = ImageDisplayFormat.FromDicomString(@"ROW\1,2");
			Assert.AreEqual(simpleRowFormat.DicomString, @"ROW\1,2");
			Assert.AreEqual(simpleRowFormat.Format, ImageDisplayFormat.FormatEnum.ROW);
			Assert.AreEqual(simpleRowFormat.MaximumImageBoxes, 1 + 2);
			Assert.AreEqual(simpleRowFormat.Modifiers.Count, 2);
			Assert.AreEqual(simpleRowFormat.Modifiers[0], 1);
			Assert.AreEqual(simpleRowFormat.Modifiers[1], 2);

			var complexRowformat = ImageDisplayFormat.FromDicomString(@"ROW\1,2,3,4,5");
			Assert.AreEqual(complexRowformat.DicomString, @"ROW\1,2,3,4,5");
			Assert.AreEqual(complexRowformat.Format, ImageDisplayFormat.FormatEnum.ROW);
			Assert.AreEqual(complexRowformat.MaximumImageBoxes, 1 + 2 + 3 + 4 + 5);
			Assert.AreEqual(complexRowformat.Modifiers.Count, 5);
			Assert.AreEqual(complexRowformat.Modifiers[0], 1);
			Assert.AreEqual(complexRowformat.Modifiers[1], 2);
			Assert.AreEqual(complexRowformat.Modifiers[2], 3);
			Assert.AreEqual(complexRowformat.Modifiers[3], 4);
			Assert.AreEqual(complexRowformat.Modifiers[4], 5);
		}

		[Test]
		public void Column_Format_Test()
		{
			var simpleColumnFormat = ImageDisplayFormat.FromDicomString(@"COL\1,2");
			Assert.AreEqual(simpleColumnFormat.DicomString, @"COL\1,2");
			Assert.AreEqual(simpleColumnFormat.Format, ImageDisplayFormat.FormatEnum.COL);
			Assert.AreEqual(simpleColumnFormat.MaximumImageBoxes, 1 + 2);
			Assert.AreEqual(simpleColumnFormat.Modifiers.Count, 2);
			Assert.AreEqual(simpleColumnFormat.Modifiers[0], 1);
			Assert.AreEqual(simpleColumnFormat.Modifiers[1], 2);

			var complexColumnformat = ImageDisplayFormat.FromDicomString(@"COL\1,2,3,4,5");
			Assert.AreEqual(complexColumnformat.DicomString, @"COL\1,2,3,4,5");
			Assert.AreEqual(complexColumnformat.Format, ImageDisplayFormat.FormatEnum.COL);
			Assert.AreEqual(complexColumnformat.MaximumImageBoxes, 1 + 2 + 3 + 4 + 5);
			Assert.AreEqual(complexColumnformat.Modifiers.Count, 5);
			Assert.AreEqual(complexColumnformat.Modifiers[0], 1);
			Assert.AreEqual(complexColumnformat.Modifiers[1], 2);
			Assert.AreEqual(complexColumnformat.Modifiers[2], 3);
			Assert.AreEqual(complexColumnformat.Modifiers[3], 4);
			Assert.AreEqual(complexColumnformat.Modifiers[4], 5);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void Slide_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"SLIDE");
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void SuperSlide_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"SUPERSLIDE");
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void Custom_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"CUSTOM\1");
		}

		[Test]
		[ExpectedException(typeof(ImageDisplayFormat.InvalidFormatException))]
		public void Invalid_Standard_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"STANDARD\1");
		}

		[Test]
		[ExpectedException(typeof(ImageDisplayFormat.InvalidFormatException))]
		public void Invalid_Row_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"ROW");
		}

		[Test]
		[ExpectedException(typeof(ImageDisplayFormat.InvalidFormatException))]
		public void Invalid_Column_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"COL\1");
		}

		[Test]
		[ExpectedException(typeof(ImageDisplayFormat.InvalidFormatException))]
		public void Unrecognized_Format_Test()
		{
			ImageDisplayFormat.FromDicomString(@"Test");
		}
	}
}

#endif