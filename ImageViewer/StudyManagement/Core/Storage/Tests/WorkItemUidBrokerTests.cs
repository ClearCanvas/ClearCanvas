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

using System.Linq;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.Tests
{
	[TestFixture]
	internal class WorkItemUidBrokerTests
	{
		[TestFixtureTearDown]
		public void TearDown()
		{
			DeleteAllWorkItemUids();
		}

		[Test]
		public void TestAddAndGet()
		{
			DeleteAllWorkItemUids();

			long oid, workItemOid;
			string version;
			using (var context = new DataAccessContext())
			{
				WorkItem workItem;

				var workItemBroker = context.GetWorkItemBroker();
				workItemBroker.AddWorkItem(workItem = CreateWorkItem("A"));

				var workItemUid = CreateWorkItemUid(workItem, "ABCDEF", "2", "3", true, true, 123);
				var broker = context.GetWorkItemUidBroker();
				broker.AddWorkItemUid(workItemUid);

				context.Commit();

				workItemOid = workItem.Oid;
				Assert.AreNotEqual(0, oid = workItemUid.Oid, "Oid should have been assigned on insert");
				Assert.IsNotNull(workItemUid.Version, "Version should have been assigned on insert");

				version = workItemUid.Version.ToString();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemUidBroker();

				var workItemUid = broker.GetWorkItemUid(oid);
				Assert.AreEqual("ABCDEF", workItemUid.File, "File");
				Assert.AreEqual("2", workItemUid.SeriesInstanceUid, "SeriesInstanceUid");
				Assert.AreEqual("3", workItemUid.SopInstanceUid, "SopInstanceUid");
				Assert.AreEqual(workItemOid, workItemUid.WorkItemOid, "WorkItemOid");
				Assert.AreEqual(true, workItemUid.Complete, "Complete");
				Assert.AreEqual(true, workItemUid.Failed, "Failed");
				Assert.AreEqual(123, workItemUid.FailureCount, "FailureCount");

				workItemUid.Failed = false;
				workItemUid.FailureCount = null;
				context.Commit();

				Assert.AreNotEqual(version, workItemUid.Version, "Version should have been changed on update");
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemUidBroker();

				var workItemUid = broker.GetWorkItemUid(oid);
				Assert.AreEqual("ABCDEF", workItemUid.File, "File");
				Assert.AreEqual("2", workItemUid.SeriesInstanceUid, "SeriesInstanceUid");
				Assert.AreEqual("3", workItemUid.SopInstanceUid, "SopInstanceUid");
				Assert.AreEqual(workItemOid, workItemUid.WorkItemOid, "WorkItemOid");
				Assert.AreEqual(true, workItemUid.Complete, "Complete");
				Assert.AreEqual(false, workItemUid.Failed, "Failed");
				Assert.AreEqual(null, workItemUid.FailureCount, "FailureCount");
			}
		}

		[Test]
		public void TestGetWorkItemUidsForWorkItem()
		{
			DeleteAllWorkItemUids();

			long workItemOidA, workItemOidB, workItemOidC, workItemOidD;
			using (var context = new DataAccessContext())
			{
				WorkItem workItemA, workItemB, workItemC, workItemD;
				var workItemBroker = context.GetWorkItemBroker();
				workItemBroker.AddWorkItem(workItemA = CreateWorkItem("A"));
				workItemBroker.AddWorkItem(workItemB = CreateWorkItem("B"));
				workItemBroker.AddWorkItem(workItemC = CreateWorkItem("C"));
				workItemBroker.AddWorkItem(workItemD = CreateWorkItem("D"));

				var broker = context.GetWorkItemUidBroker();
				broker.AddWorkItemUid(CreateWorkItemUid(workItemA, "file1", "series1", "sop1"));
				broker.AddWorkItemUid(CreateWorkItemUid(workItemA, "file4", "series2", "sop4"));
				broker.AddWorkItemUid(CreateWorkItemUid(workItemB, "file6", "series4", "sop6"));

				broker.AddWorkItemUid(CreateWorkItemUid(workItemA, "file3", "series2", "sop3"));
				broker.AddWorkItemUid(CreateWorkItemUid(workItemA, "file2", "series1", "sop2"));
				broker.AddWorkItemUid(CreateWorkItemUid(workItemB, "file5", "series3", "sop5"));

				context.Commit();

				workItemOidA = workItemA.Oid;
				workItemOidB = workItemB.Oid;
				workItemOidC = workItemC.Oid;
				workItemOidD = workItemD.Oid;
			}

			using (var context = new DataAccessContext())
			{
				var workItemBroker = context.GetWorkItemBroker();
				workItemBroker.Delete(workItemBroker.GetWorkItem(workItemOidD));
				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemUidBroker();

				var listA = broker.GetWorkItemUidsForWorkItem(workItemOidA).OrderBy(x => x.File).ToList();
				var listB = broker.GetWorkItemUidsForWorkItem(workItemOidB).OrderBy(x => x.File).ToList();
				var listC = broker.GetWorkItemUidsForWorkItem(workItemOidC);
				var listD = broker.GetWorkItemUidsForWorkItem(workItemOidD);

				Assert.AreEqual(4, listA.Count, "Unexpected number of results for work item A");
				Assert.AreEqual("file1", listA[0].File, "Unexpected result for work item A");
				Assert.AreEqual("file2", listA[1].File, "Unexpected result for work item A");
				Assert.AreEqual("file3", listA[2].File, "Unexpected result for work item A");
				Assert.AreEqual("file4", listA[3].File, "Unexpected result for work item A");

				Assert.AreEqual(2, listB.Count, "Unexpected number of results for work item B");
				Assert.AreEqual("file5", listB[0].File, "Unexpected result for work item A");
				Assert.AreEqual("file6", listB[1].File, "Unexpected result for work item A");

				Assert.AreEqual(0, listC.Count, "Unexpected results for work item C");
				Assert.AreEqual(0, listD.Count, "Unexpected results for work item D");
			}
		}

		private static WorkItem CreateWorkItem(string studyInstanceUid)
		{
			return WorkItemBrokerTests.CreateWorkItem("test work item", studyInstanceUid);
		}

		private static WorkItemUid CreateWorkItemUid(WorkItem workItem, string file, string seriesInstanceUid, string sopInstanceUid, bool complete = false, bool failed = false, byte? failureCount = null)
		{
			return new WorkItemUid
			       	{
			       		WorkItem = workItem,
			       		File = file,
			       		SeriesInstanceUid = seriesInstanceUid,
			       		SopInstanceUid = sopInstanceUid,
			       		Complete = complete,
			       		Failed = failed,
			       		FailureCount = failureCount
			       	};
		}

		private static void DeleteAllWorkItemUids()
		{
			using (var context = new DataAccessContext())
			{
				var workItemUidBroker = context.GetWorkItemUidBroker();
				workItemUidBroker.DeleteAll();

				var workItemBroker = context.GetWorkItemBroker();
				workItemBroker.DeleteAll();

				context.Commit();
			}
		}
	}
}

#endif