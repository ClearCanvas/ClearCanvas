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

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	internal class DicomUidTests
	{
		[Test]
		public void TestGuidConversion()
		{
			// this is the reference sample for 2.25 style UIDs given on David Clunie's website (http://www.dclunie.com/medical-image-faq/html/part2.html)
			Assert.AreEqual("329800735698586629295641978511506172918", DicomUid.ConvertGuid(new Guid("f81d4fae-7dec-11d0-a765-00a0c91e6bf6")));
		}
	}
}

#endif