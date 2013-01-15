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
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.Dicom.ServiceModel.Query.Tests
{
	[TestFixture]
	public class SortStudyTests
	{
		public SortStudyTests()
		{
		}

		[Test]
		public void Test()
		{
			List<StudyRootStudyIdentifier> identifiers = new List<StudyRootStudyIdentifier>();

			identifiers.Add(CreateStudyIdentifier("3", "20080101", "112300"));
			identifiers.Add(CreateStudyIdentifier("4", "20080101", ""));
			identifiers.Add(CreateStudyIdentifier("2", "20080104", "184400"));
			identifiers.Add(CreateStudyIdentifier("1", "20080104", "184500"));
			identifiers.Add(CreateStudyIdentifier("5", "", ""));
			identifiers.Add(CreateStudyIdentifier("6", "", ""));

			identifiers.Sort(new StudyDateTimeComparer());

			int i = 1;
			foreach (StudyRootStudyIdentifier identifier in identifiers)
			{
				Assert.AreEqual(i.ToString(), identifier.StudyInstanceUid);
				++i;
			}
		}

		private static StudyRootStudyIdentifier CreateStudyIdentifier(string uid, string date, string time)
		{
			StudyRootStudyIdentifier id = new StudyRootStudyIdentifier();
			id.StudyInstanceUid = uid;
			id.StudyDate = date;
			id.StudyTime = time;
			return id;
		}
	}
}

#endif