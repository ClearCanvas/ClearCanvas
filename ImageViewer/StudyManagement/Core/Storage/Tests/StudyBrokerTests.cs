#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.Data.SqlTypes;
using System.Linq;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.Tests
{
	[TestFixture]
	internal class StudyBrokerTests
	{
		[TestFixtureTearDown]
		public void TearDown()
		{
			DeleteAllStudies();
		}

		[Test]
		public void TestAddAndGet()
		{
			DeleteAllStudies();

			var time = DateTime.Now.Truncate();

			long oid;
			string version;
			using (var context = new DataAccessContext())
			{
				var study = CreateStudy("123", "A#", "MRN", "BOBSON^BOB", new DateTime(2014, 07, 01), "1234", "DESU", @"CT\PT\KO\PR\SC", 6, 2101, time, true, time.AddMinutes(100));

				var broker = context.GetStudyBroker();
				broker.AddStudy(CreateStudy("456", "2"));
				broker.AddStudy(study);
				broker.AddStudy(CreateStudy("789", "3"));

				context.Commit();

				Assert.AreNotEqual(0, oid = study.Oid, "Oid should have been assigned on insert");
				Assert.IsNotNull(study.Version, "Version should have been assigned on insert");

				version = study.Version.ToString();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var study = broker.GetStudy(oid);
				Assert.AreEqual("123", study.StudyInstanceUid, "StudyInstanceUid");
				Assert.AreEqual("A#", study.AccessionNumber, "AccessionNumber");
				Assert.AreEqual("MRN", study.PatientId, "PatientId");
				Assert.AreEqual("BOBSON^BOB", study.PatientsName, "PatientsName");
				Assert.AreEqual(new DateTime(2014, 07, 01), study.StudyDate, "StudyDate");
				Assert.AreEqual("20140701", study.StudyDateRaw, "StudyDateRaw");
				Assert.AreEqual("1234", study.StudyId, "StudyId");
				Assert.AreEqual("DESU", study.StudyDescription, "StudyDescription");
				Assert.AreEqual(@"CT\PT\KO\PR\SC", study.ModalitiesInStudy, "ModalitiesInStudy");
				Assert.AreEqual(6, study.NumberOfStudyRelatedSeries, "NumberOfStudyRelatedSeries");
				Assert.AreEqual(2101, study.NumberOfStudyRelatedInstances, "NumberOfStudyRelatedInstances");
				Assert.AreEqual(time, study.StoreTime, "StoreTime");
				Assert.AreEqual(true, study.Reindex, "Reindex");
				Assert.AreEqual(time.AddMinutes(100), study.DeleteTime, "DeleteTime");

				study.ModalitiesInStudy = @"CT\PT";
				context.Commit();

				Assert.AreNotEqual(version, study.Version, "Version should have been changed on update");
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var study = broker.GetStudy(oid);
				Assert.AreEqual("123", study.StudyInstanceUid, "StudyInstanceUid");
				Assert.AreEqual("A#", study.AccessionNumber, "AccessionNumber");
				Assert.AreEqual("MRN", study.PatientId, "PatientId");
				Assert.AreEqual("BOBSON^BOB", study.PatientsName, "PatientsName");
				Assert.AreEqual(new DateTime(2014, 07, 01), study.StudyDate, "StudyDate");
				Assert.AreEqual("20140701", study.StudyDateRaw, "StudyDateRaw");
				Assert.AreEqual("1234", study.StudyId, "StudyId");
				Assert.AreEqual("DESU", study.StudyDescription, "StudyDescription");
				Assert.AreEqual(@"CT\PT", study.ModalitiesInStudy, "ModalitiesInStudy");
				Assert.AreEqual(6, study.NumberOfStudyRelatedSeries, "NumberOfStudyRelatedSeries");
				Assert.AreEqual(2101, study.NumberOfStudyRelatedInstances, "NumberOfStudyRelatedInstances");
				Assert.AreEqual(time, study.StoreTime, "StoreTime");
				Assert.AreEqual(true, study.Reindex, "Reindex");
				Assert.AreEqual(time.AddMinutes(100), study.DeleteTime, "DeleteTime");
			}
		}

		[Test]
		public void TestGetStudyByUid()
		{
			DeleteAllStudies();

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				broker.AddStudy(CreateStudy("12.3", "A123"));
				broker.AddStudy(CreateStudy("45.6", "A456", deleted : true));
				broker.AddStudy(CreateStudy("78.9", "A789"));
				broker.AddStudy(CreateStudy("", "A"));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var study456 = broker.GetStudy("45.6");
				var study123 = broker.GetStudy("12.3");
				var study120 = broker.GetStudy("12.0");
				var study890 = broker.GetStudy("89.0");
				var studyBlank = broker.GetStudy("");

				Assert.IsNotNull(study456);
				Assert.AreEqual(study456.StudyInstanceUid, "45.6");
				Assert.AreEqual(study456.AccessionNumber, "A456");
				Assert.IsNotNull(study123);
				Assert.AreEqual(study123.StudyInstanceUid, "12.3");
				Assert.AreEqual(study123.AccessionNumber, "A123");
				Assert.IsNull(study120);
				Assert.IsNull(study890);
				Assert.IsNotNull(studyBlank);
				Assert.AreEqual(studyBlank.StudyInstanceUid, "");
				Assert.AreEqual(studyBlank.AccessionNumber, "A");
			}
		}

		[Test]
		public void TestGetStudyCount()
		{
			DeleteAllStudies();

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				broker.AddStudy(CreateStudy("12.3", "A123"));
				broker.AddStudy(CreateStudy("45.6", "A456", deleted : true));
				broker.AddStudy(CreateStudy("78.9", "A789"));
				broker.AddStudy(CreateStudy("", "A"));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var count = broker.GetStudyCount();

				Assert.AreEqual(4, count);
			}
		}

		[Test]
		public void TestGetStudyOids()
		{
			DeleteAllStudies();

			long oid123, oid456, oid789, oidBlank;
			using (var context = new DataAccessContext())
			{
				Study study123, study456, study789, studyBlank;

				var broker = context.GetStudyBroker();
				broker.AddStudy(study123 = CreateStudy("12.3", "A123"));
				broker.AddStudy(study456 = CreateStudy("45.6", "A456", deleted : true));
				broker.AddStudy(study789 = CreateStudy("78.9", "A789"));
				broker.AddStudy(studyBlank = CreateStudy("", "A"));

				context.Commit();

				oid123 = study123.Oid;
				oid456 = study456.Oid;
				oid789 = study789.Oid;
				oidBlank = studyBlank.Oid;
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var oids = broker.GetStudyOids();

				Assert.AreEqual(new[] {oid123, oid456, oid789, oidBlank}.OrderBy(x => x), oids.OrderBy(x => x));
			}
		}

		[Test]
		public void TestGetStudies()
		{
			DeleteAllStudies();

			long oid123, oid456, oid789, oidBlank;
			using (var context = new DataAccessContext())
			{
				Study study123, study456, study789, studyBlank;

				var broker = context.GetStudyBroker();
				broker.AddStudy(study123 = CreateStudy("12.3", "A123"));
				broker.AddStudy(study456 = CreateStudy("45.6", "A456", deleted : true));
				broker.AddStudy(study789 = CreateStudy("78.9", "A789"));
				broker.AddStudy(studyBlank = CreateStudy("", "A"));

				context.Commit();

				oid123 = study123.Oid;
				oid456 = study456.Oid;
				oid789 = study789.Oid;
				oidBlank = studyBlank.Oid;
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var studies = broker.GetStudies();

				Assert.AreEqual(new[] {oid123, oid456, oid789, oidBlank}.OrderBy(x => x), studies.Select(x => x.Oid).OrderBy(x => x));
			}
		}

		[Test]
		public void TestGetReindexStudies()
		{
			DeleteAllStudies();

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				broker.AddStudy(CreateStudy("12.3", "A123"));
				broker.AddStudy(CreateStudy("45.6", "A456", deleted : true, reindex : true));
				broker.AddStudy(CreateStudy("78.9", "A789", reindex : true));
				broker.AddStudy(CreateStudy("", "A", deleted : true));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var studies = broker.GetReindexStudies();

				Assert.AreEqual(2, studies.Count);
				Assert.IsTrue(studies.Any(s => s.StudyInstanceUid == "45.6"), "Missing study 45.6");
				Assert.IsTrue(studies.Any(s => s.StudyInstanceUid == "78.9"), "Missing study 78.9");
			}
		}

		[Test]
		public void TestGetStudiesForDeletion()
		{
			DeleteAllStudies();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				broker.AddStudy(CreateStudy("12.3", "A123"));
				broker.AddStudy(CreateStudy("45.6", "A456", deleted : true, deleteTime : pastTime));
				broker.AddStudy(CreateStudy("78.9", "A789", deleteTime : pastTime.AddMinutes(-1)));
				broker.AddStudy(CreateStudy("89.0", "A890", deleteTime : futureTime));
				broker.AddStudy(CreateStudy("", "A", deleted : true));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();

				var deletableOld = broker.GetStudiesForDeletion(time, 5);
				var deletableOldest = broker.GetStudiesForDeletion(time, 1).SingleOrDefault();

				var deletableAny = broker.GetStudiesForDeletion(futureTime.AddMinutes(100), 9);

				Assert.AreEqual(1, deletableOld.Count);
				Assert.AreEqual("78.9", deletableOld[0].StudyInstanceUid, "Missing deletable old study 78.9");

				Assert.IsNotNull(deletableOldest, "Missing oldest deletable");
				Assert.AreEqual("78.9", deletableOldest.StudyInstanceUid, "missing deletable oldest study");

				Assert.AreEqual(2, deletableAny.Count);
				Assert.AreEqual("78.9", deletableAny[0].StudyInstanceUid, "Missing deletable old study 78.9");
				Assert.AreEqual("89.0", deletableAny[1].StudyInstanceUid, "Missing deletable newer study 89.0");
			}
		}

		private static Study CreateStudy(string studyInstanceUid, string accessionNumber = "ACCESSION",
		                                 string patientId = "PATIENTID", string patientsName = "PATIENTNAME",
		                                 DateTime? studyDate = null, string studyId = "STUDYID", string studyDescription = "DESCRIPTION",
		                                 string modalitiesInStudy = "OT", int? numberOfStudyRelatedSeries = null, int? numberOfStudyRelatedInstances = null,
		                                 DateTime? storeTime = null, bool reindex = false, DateTime? deleteTime = null, bool deleted = false)
		{
			return new Study
			       	{
			       		StudyInstanceUid = studyInstanceUid,
			       		AccessionNumber = accessionNumber,
			       		PatientId = patientId,
			       		PatientsName = patientsName,
			       		StudyDate = ConstrainSqlDateTime(studyDate),
			       		StudyDateRaw = studyDate.HasValue ? DateParser.ToDicomString(studyDate.Value) : string.Empty,
			       		StudyId = studyId,
			       		StudyDescription = studyDescription,
			       		ModalitiesInStudy = modalitiesInStudy,
			       		NumberOfStudyRelatedSeries = numberOfStudyRelatedSeries,
			       		NumberOfStudyRelatedInstances = numberOfStudyRelatedInstances,
			       		StoreTime = ConstrainSqlDateTime(storeTime.GetValueOrDefault(DateTime.Now)),
			       		Reindex = reindex,
			       		DeleteTime = ConstrainSqlDateTime(deleteTime),
			       		Deleted = deleted
			       	};
		}

		private static DateTime? ConstrainSqlDateTime(DateTime? value)
		{
			if (!value.HasValue) return null;
			else if (value < SqlDateTime.MinValue.Value) return SqlDateTime.MinValue.Value;
			else if (value > SqlDateTime.MaxValue.Value) return SqlDateTime.MaxValue.Value;
			else return value;
		}

		private static void DeleteAllStudies()
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetStudyBroker();
				broker.DeleteAll();
				context.Commit();
			}
		}
	}
}

#endif