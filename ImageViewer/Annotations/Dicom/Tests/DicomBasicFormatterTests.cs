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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Annotations.Dicom.Tests
{

#pragma warning disable 1591,0419,1574,1587

	[TestFixture]
	public class DicomBasicFormatterTests
	{
		public DicomBasicFormatterTests()
		{ }

		[Test]
		public void TestListFormatters()
		{
			string input = @"The\brown\dog\\jumped";
			string result = DicomDataFormatHelper.StringListFormat(DicomStringHelper.GetStringArray(input));
			Assert.AreEqual(result, "The,\nbrown,\ndog,\njumped");

			input = @"Doe^John^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe, John");

			input = @"^John^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "John");

			input = @"Doe^^^^";
			result = DicomDataFormatHelper.PersonNameFormatter(new PersonName(input));
			Assert.AreEqual(result, "Doe");

			input = @"Doe^John^^^\Doe^Jane^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "Doe, John,\nDoe, Jane");

			input = @"^John^^^\Doe^Jane^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe, Jane");

			input = @"^John^^^\Doe^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "John,\nDoe");

			input = @"^^^^\Doe^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "Doe");

			input = @"^^^^\^^^^";
			result = DicomDataFormatHelper.PersonNameListFormatter(DicomStringHelper.GetPersonNameArray(input));
			Assert.AreEqual(result, "");
		}
	}
}

#endif