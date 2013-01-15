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

using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Iod.Modules.Tests
{
	[TestFixture]
	public class FilmSizeTests
	{
		[Test]
		public void Null_DicomString_Test()
		{
			var filmSize = FilmSize.FromDicomString(null);
			Assert.IsNull(filmSize);
		}

		[Test]
		public void Empty_DicomString_Test()
		{
			var filmSize = FilmSize.FromDicomString(string.Empty);
			Assert.IsNull(filmSize);
		}

		[Test]
		public void Supported_FilmSizes_Test()
		{
			var supportedFilmSizes = FilmSize.StandardFilmSizes;
			Assert.IsNotEmpty(new List<FilmSize>(supportedFilmSizes));
		}

		[Test]
		public void Valid_FilmSize_Test()
		{
			var filmSizeInInches = FilmSize.FromDicomString("11INX22IN");
			Assert.AreEqual(filmSizeInInches.DicomString, "11INX22IN");
			Assert.AreEqual(filmSizeInInches.GetWidth(FilmSize.FilmSizeUnit.Inch), 11);
			Assert.AreEqual(filmSizeInInches.GetHeight(FilmSize.FilmSizeUnit.Inch), 22);
			Assert.AreEqual(filmSizeInInches.GetWidth(FilmSize.FilmSizeUnit.Centimeter), 11 * LengthInMillimeter.Inch / 10);
			Assert.AreEqual(filmSizeInInches.GetHeight(FilmSize.FilmSizeUnit.Centimeter), 22 * LengthInMillimeter.Inch / 10);

			var filmSizeInCentimeter = FilmSize.FromDicomString("11CMX22CM");
			Assert.AreEqual(filmSizeInCentimeter.DicomString, "11CMX22CM");
			Assert.AreEqual(filmSizeInCentimeter.GetWidth(FilmSize.FilmSizeUnit.Centimeter), 11);
			Assert.AreEqual(filmSizeInCentimeter.GetHeight(FilmSize.FilmSizeUnit.Centimeter), 22);
			Assert.AreEqual(filmSizeInCentimeter.GetWidth(FilmSize.FilmSizeUnit.Inch), 11 * 10 / LengthInMillimeter.Inch);
			Assert.AreEqual(filmSizeInCentimeter.GetHeight(FilmSize.FilmSizeUnit.Inch), 22 * 10 / LengthInMillimeter.Inch);
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Lower_Case_X_Test()
		{
			FilmSize.FromDicomString("11MMx22MM");
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Invalid_Units_Test()
		{
			FilmSize.FromDicomString("11MMX22MM");
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Two_Different_Units_Test()
		{
			FilmSize.FromDicomString("11INX22CM");
		}

	}
}


#endif