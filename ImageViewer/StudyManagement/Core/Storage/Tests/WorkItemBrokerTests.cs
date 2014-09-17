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
using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Common.WorkItem;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.Tests
{
	[TestFixture]
	internal class WorkItemBrokerTests
	{
		private const string _idComplete = "Complete";
		private const string _idCancelled = "Cancelled";
		private const string _idCanceling = "Canceling";
		private const string _idDeleting = "Deleting";
		private const string _idDeleted = "Delted";
		private const string _idFailed = "Failed";
		private const string _idIdling = "Idling";
		private const string _idInProgress = "In Progress";
		private const string _idPending = "Pending";

		[TestFixtureTearDown]
		public void TearDown()
		{
			DeleteAllWorkItems();
		}

		[Test]
		public void TestAddAndGet()
		{
			DeleteAllWorkItems();

			var time = new DateTime(2014, 07, 01, 11, 12, 45);

			long oid;
			string version;
			using (var context = new DataAccessContext())
			{
				var workItem = CreateWorkItem("blah", "1", WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, 987,
				                              time, time.AddMinutes(1), time.AddMinutes(10), time.AddMinutes(30), time.AddMinutes(60),
				                              "request data", "progress data");

				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem("junk", "2"));
				broker.AddWorkItem(workItem);
				broker.AddWorkItem(CreateWorkItem("junk", "3"));

				context.Commit();

				Assert.AreNotEqual(0, oid = workItem.Oid, "Oid should have been assigned on insert");
				Assert.IsNotNull(workItem.Version, "Version should have been assigned on insert");

				version = workItem.Version.ToString();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				var workItem = broker.GetWorkItem(oid);
				Assert.AreEqual("blah", workItem.Type, "Type");
				Assert.AreEqual("1", workItem.StudyInstanceUid, "StudyInstanceUid");
				Assert.AreEqual(WorkItemPriorityEnum.High, workItem.Priority, "Priority");
				Assert.AreEqual(WorkItemStatusEnum.Idle, workItem.Status, "Status");
				Assert.AreEqual(987, workItem.FailureCount, "FailureCount");
				Assert.AreEqual(time, workItem.RequestedTime, "RequestedTime");
				Assert.AreEqual(time.AddMinutes(1), workItem.ScheduledTime, "ScheduledTime");
				Assert.AreEqual(time.AddMinutes(10), workItem.ProcessTime, "ProcessTime");
				Assert.AreEqual(time.AddMinutes(30), workItem.ExpirationTime, "ExpirationTime");
				Assert.AreEqual(time.AddMinutes(60), workItem.DeleteTime, "DeleteTime");
				Assert.IsNotNull(workItem.Request, "Request");
				Assert.AreEqual("request data", workItem.Request.WorkItemType, "Request.WorkItemType");
				Assert.IsNotNull(workItem.Progress, "Progress");
				Assert.AreEqual("progress data", workItem.Progress.StatusDetails, "Progress.StatusDetails");

				workItem.Status = WorkItemStatusEnum.Pending;
				context.Commit();

				Assert.AreNotEqual(version, workItem.Version, "Version should have been changed on update");
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				var workItem = broker.GetWorkItem(oid);
				Assert.AreEqual("blah", workItem.Type, "Type after update");
				Assert.AreEqual("1", workItem.StudyInstanceUid, "StudyInstanceUid after update");
				Assert.AreEqual(WorkItemPriorityEnum.High, workItem.Priority, "Priority after update");
				Assert.AreEqual(WorkItemStatusEnum.Pending, workItem.Status, "Status after update");
				Assert.AreEqual(987, workItem.FailureCount, "FailureCount after update");
				Assert.AreEqual(time, workItem.RequestedTime, "RequestedTime after update");
				Assert.AreEqual(time.AddMinutes(1), workItem.ScheduledTime, "ScheduledTime after update");
				Assert.AreEqual(time.AddMinutes(10), workItem.ProcessTime, "ProcessTime after update");
				Assert.AreEqual(time.AddMinutes(30), workItem.ExpirationTime, "ExpirationTime after update");
				Assert.AreEqual(time.AddMinutes(60), workItem.DeleteTime, "DeleteTime after update");
				Assert.IsNotNull(workItem.Request, "Request after update");
				Assert.AreEqual("request data", workItem.Request.WorkItemType, "Request.WorkItemType after update");
				Assert.IsNotNull(workItem.Progress, "Progress after update");
				Assert.AreEqual("progress data", workItem.Progress.StatusDetails, "Progress.StatusDetails after update");
			}
		}

		[Test]
		public void TestGetWorkItemsToDelete()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1", status : WorkItemStatusEnum.Idle, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1", status : WorkItemStatusEnum.Pending, deleteTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2", status : WorkItemStatusEnum.Idle, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2", status : WorkItemStatusEnum.Pending, deleteTime : futureTime));

				broker.AddWorkItem(CreateWorkItem(_idComplete, "3", status : WorkItemStatusEnum.Complete, deleteTime : time.AddMinutes(-1)));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "4", status : WorkItemStatusEnum.Complete, deleteTime : time.AddMinutes(-2)));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "5", status : WorkItemStatusEnum.Complete, deleteTime : time.AddMinutes(-3)));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				// there are 4 items eligible for deletion here (must have status complete and delete time in past)
				var topDeletable = broker.GetWorkItemsToDelete(3);
				var allDeletable = broker.GetWorkItemsToDelete(7);

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.AreEqual(3, topDeletable.Count, "Query top should yield 3 items");
				Assert.IsTrue(topDeletable.All(x => x.Type == _idComplete), "Query top yielded item with wrong identity");
				Assert.IsTrue(topDeletable.All(x => x.Status == WorkItemStatusEnum.Complete), "Query top yielded item with wrong status");
				Assert.IsTrue(topDeletable.All(x => x.DeleteTime < time), "Query top yielded item with wrong status");
				Assert.IsFalse(topDeletable.Any(x => x.StudyInstanceUid == "2"), "Query top yielded item with wrong delete time");

				Assert.AreEqual(4, allDeletable.Count, "Query all should yield 4 items");
				Assert.IsTrue(allDeletable.All(x => x.Type == _idComplete), "Query all yielded item with wrong identity");
				Assert.IsTrue(allDeletable.All(x => x.Status == WorkItemStatusEnum.Complete), "Query all yielded item with wrong status");
				Assert.IsTrue(allDeletable.All(x => x.DeleteTime < time), "Query all yielded item with wrong status");
				Assert.AreEqual(1, allDeletable.Count(x => x.StudyInstanceUid == "1"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeletable.Count(x => x.StudyInstanceUid == "3"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeletable.Count(x => x.StudyInstanceUid == "4"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeletable.Count(x => x.StudyInstanceUid == "5"), "Query all failed to yield deletable item");
			}
		}

		[Test]
		public void TestGetWorkItemsDeleted()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1", status : WorkItemStatusEnum.Idle, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, deleteTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1", status : WorkItemStatusEnum.Pending, deleteTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2", status : WorkItemStatusEnum.Idle, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, deleteTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2", status : WorkItemStatusEnum.Pending, deleteTime : futureTime));

				broker.AddWorkItem(CreateWorkItem(_idDeleted, "3", status : WorkItemStatusEnum.Deleted, deleteTime : time.AddMinutes(-1)));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "4", status : WorkItemStatusEnum.Deleted, deleteTime : time.AddMinutes(-2)));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "5", status : WorkItemStatusEnum.Deleted, deleteTime : time.AddMinutes(-3)));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				// there are 5 deleted items here (status is deleted)
				var topDeleted = broker.GetWorkItemsDeleted(3);
				var allDeleted = broker.GetWorkItemsDeleted(7);

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.AreEqual(3, topDeleted.Count, "Query top should yield 3 items");
				Assert.IsTrue(topDeleted.All(x => x.Type == _idDeleted), "Query top yielded item with wrong identity");
				Assert.IsTrue(topDeleted.All(x => x.Status == WorkItemStatusEnum.Deleted), "Query top yielded item with wrong status");

				Assert.AreEqual(5, allDeleted.Count, "Query all should yield 5 items");
				Assert.IsTrue(allDeleted.All(x => x.Type == _idDeleted), "Query all yielded item with wrong identity");
				Assert.IsTrue(allDeleted.All(x => x.Status == WorkItemStatusEnum.Deleted), "Query all yielded item with wrong status");
				Assert.AreEqual(1, allDeleted.Count(x => x.StudyInstanceUid == "1"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeleted.Count(x => x.StudyInstanceUid == "2"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeleted.Count(x => x.StudyInstanceUid == "3"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeleted.Count(x => x.StudyInstanceUid == "4"), "Query all failed to yield deletable item");
				Assert.AreEqual(1, allDeleted.Count(x => x.StudyInstanceUid == "5"), "Query all failed to yield deletable item");
			}
		}

		[Test]
		public void TestGetWorkItemsForProcessingAnyPriority()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1a", status : WorkItemStatusEnum.Idle, processTime : pastTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1b", status : WorkItemStatusEnum.Pending, processTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2a", status : WorkItemStatusEnum.Idle, processTime : futureTime, priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2b", status : WorkItemStatusEnum.Pending, processTime : futureTime, priority : WorkItemPriorityEnum.Stat));

				broker.AddWorkItem(CreateWorkItem(_idIdling, "3", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-3), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idPending, "4", status : WorkItemStatusEnum.Pending, processTime : time.AddMinutes(-1), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "5", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-1)));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				// there are 5 processable items here (status is idle or pending, and process time in the past)
				var topProcessable = broker.GetWorkItemsForProcessing(3);
				var allProcessable = broker.GetWorkItemsForProcessing(7);

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.AreEqual(3, topProcessable.Count, "Query top should yield 3 items");
				Assert.IsTrue(topProcessable.All(x => x.Status == WorkItemStatusEnum.Idle || x.Status == WorkItemStatusEnum.Pending), "Query top yielded item with wrong status");
				Assert.AreEqual("3", topProcessable[0].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("4", topProcessable[1].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("1a", topProcessable[2].StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.AreEqual(5, allProcessable.Count, "Query all should yield 5 items");
				Assert.IsTrue(allProcessable.All(x => x.Status == WorkItemStatusEnum.Idle || x.Status == WorkItemStatusEnum.Pending), "Query top yielded item with wrong status");
				Assert.AreEqual("3", allProcessable[0].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("4", allProcessable[1].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("1a", allProcessable[2].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("1b", allProcessable[3].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("5", allProcessable[4].StudyInstanceUid, "Query top yielded item with wrong identity");
			}
		}

		[Test]
		public void TestGetWorkItemsForProcessingWithPriority()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1a", status : WorkItemStatusEnum.Idle, processTime : pastTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1b", status : WorkItemStatusEnum.Pending, processTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2a", status : WorkItemStatusEnum.Idle, processTime : futureTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2b", status : WorkItemStatusEnum.Pending, processTime : futureTime, priority : WorkItemPriorityEnum.Stat));

				broker.AddWorkItem(CreateWorkItem(_idIdling, "3", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-3), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idPending, "4", status : WorkItemStatusEnum.Pending, processTime : time.AddMinutes(-1), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "5", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-1)));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				// there are 5 processable items here (status is idle or pending, and process time in the past)
				var highPriority = broker.GetWorkItemsForProcessing(1, WorkItemPriorityEnum.High).SingleOrDefault();
				var statPriority = broker.GetWorkItemsForProcessing(1, WorkItemPriorityEnum.Stat).SingleOrDefault();
				var normPriority = broker.GetWorkItemsForProcessing(1, WorkItemPriorityEnum.Normal).SingleOrDefault();
				var allHighPriority = broker.GetWorkItemsForProcessing(9, WorkItemPriorityEnum.High);
				var allStatPriority = broker.GetWorkItemsForProcessing(9, WorkItemPriorityEnum.Stat);
				var allNormPriority = broker.GetWorkItemsForProcessing(9, WorkItemPriorityEnum.Normal);

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.IsNotNull(highPriority, "Query top did not yield expected result for high");
				Assert.AreEqual("1a", highPriority.StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.IsNotNull(statPriority, "Query top did not yield expected result for stat");
				Assert.AreEqual("3", statPriority.StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.IsNotNull(normPriority, "Query top did not yield expected result for normal");
				Assert.AreEqual("1b", normPriority.StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.AreEqual(1, allHighPriority.Count, "Query top did not yield expected result for high");
				Assert.AreEqual("1a", allHighPriority[0].StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.AreEqual(2, allStatPriority.Count, "Query top did not yield expected result for stat");
				Assert.AreEqual("3", allStatPriority[0].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("4", allStatPriority[1].StudyInstanceUid, "Query top yielded item with wrong identity");

				Assert.AreEqual(2, allNormPriority.Count, "Query top did not yield expected result for normal");
				Assert.AreEqual("1b", allNormPriority[0].StudyInstanceUid, "Query top yielded item with wrong identity");
				Assert.AreEqual("5", allNormPriority[1].StudyInstanceUid, "Query top yielded item with wrong identity");
			}
		}

		[Test]
		public void TestGetWorkItemsScheduledBeforeOrHighPriority()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, scheduledTime : pastTime, priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1", status : WorkItemStatusEnum.Idle, scheduledTime : pastTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, scheduledTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1", status : WorkItemStatusEnum.Pending, scheduledTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2", status : WorkItemStatusEnum.Idle, scheduledTime : futureTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, scheduledTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2", status : WorkItemStatusEnum.Pending, scheduledTime : futureTime, priority : WorkItemPriorityEnum.Stat));

				broker.AddWorkItem(CreateWorkItem(_idIdling, "3", status : WorkItemStatusEnum.Idle, scheduledTime : time.AddMinutes(-3), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idPending, "4", status : WorkItemStatusEnum.Pending, scheduledTime : time.AddMinutes(-1), priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "5", status : WorkItemStatusEnum.Idle, scheduledTime : time.AddMinutes(-1)));

				context.Commit();
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				var highPriority = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time, WorkItemPriorityEnum.High, null, null, null).ToList();
				var highPriorityFuture = broker.GetWorkItemsScheduledBeforeOrHigherPriority(futureTime.AddMinutes(10), WorkItemPriorityEnum.High, null, null, null).ToList();
				var highPriorityPast = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time.AddMinutes(-10), WorkItemPriorityEnum.High, null, null, null).ToList();
				var highPriorityRunningOrIdle = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time, WorkItemPriorityEnum.High, WorkItemStatusFilter.RunningOrIdle, null, null).ToList();
				var highPriorityStudyFilter = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time, WorkItemPriorityEnum.High, null, "1", null).ToList();
				var highPriorityTypeFilter = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time, WorkItemPriorityEnum.High, null, null, _idCanceling, _idInProgress, _idPending).ToList();
				var highPriorityStudyTypeFilter = broker.GetWorkItemsScheduledBeforeOrHigherPriority(time, WorkItemPriorityEnum.High, null, "1", _idCanceling, _idInProgress, _idPending).ToList();

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.AreEqual(4, highPriority.Count(), "Query yielded unexpected results for high or higher priority");
				Assert.AreEqual("1" + _idCanceling, highPriority[0].StudyInstanceUid + highPriority[0].Type, "Query yield unexpected item");
				Assert.AreEqual("3" + _idIdling, highPriority[1].StudyInstanceUid + highPriority[1].Type, "Query yield unexpected item");
				Assert.AreEqual("1" + _idIdling, highPriority[2].StudyInstanceUid + highPriority[2].Type, "Query yield unexpected item");
				Assert.AreEqual("4" + _idPending, highPriority[3].StudyInstanceUid + highPriority[3].Type, "Query yield unexpected item");

				Assert.AreEqual(highPriority, highPriorityFuture, "Query should not yield results scheduled for future");
				Assert.AreEqual(3, highPriorityPast.Count(), "Query yielded unexpected results for high or higher priority (past query)");
				Assert.AreEqual("1" + _idCanceling, highPriorityPast[0].StudyInstanceUid + highPriorityPast[0].Type, "Query yield unexpected item (past query)");
				Assert.AreEqual("3" + _idIdling, highPriorityPast[1].StudyInstanceUid + highPriorityPast[1].Type, "Query yield unexpected item (past query)");
				Assert.AreEqual("1" + _idIdling, highPriorityPast[2].StudyInstanceUid + highPriorityPast[2].Type, "Query yield unexpected item (past query)");

				Assert.AreEqual(3, highPriorityRunningOrIdle.Count(), "Query yielded unexpected results for high or higher priority (running or idle status filter)");
				Assert.AreEqual("1" + _idCanceling, highPriorityRunningOrIdle[0].StudyInstanceUid + highPriorityRunningOrIdle[0].Type, "Query yield unexpected item (running or idle status filter)");
				Assert.AreEqual("3" + _idIdling, highPriorityRunningOrIdle[1].StudyInstanceUid + highPriorityRunningOrIdle[1].Type, "Query yield unexpected item (running or idle status filter)");
				Assert.AreEqual("1" + _idIdling, highPriorityRunningOrIdle[2].StudyInstanceUid + highPriorityRunningOrIdle[2].Type, "Query yield unexpected item (running or idle status filter)");

				Assert.AreEqual(2, highPriorityStudyFilter.Count(), "Query yielded unexpected results for high or higher priority (study filter)");
				Assert.AreEqual("1" + _idCanceling, highPriorityStudyFilter[0].StudyInstanceUid + highPriorityStudyFilter[0].Type, "Query yield unexpected item (study filter)");
				Assert.AreEqual("1" + _idIdling, highPriorityStudyFilter[1].StudyInstanceUid + highPriorityStudyFilter[1].Type, "Query yield unexpected item (study filter)");

				Assert.AreEqual(2, highPriorityTypeFilter.Count(), "Query yielded unexpected results for high or higher priority (type filter)");
				Assert.AreEqual("1" + _idCanceling, highPriorityTypeFilter[0].StudyInstanceUid + highPriorityTypeFilter[0].Type, "Query yield unexpected item (type filter)");
				Assert.AreEqual("4" + _idPending, highPriorityTypeFilter[1].StudyInstanceUid + highPriorityTypeFilter[1].Type, "Query yield unexpected item (type filter)");

				Assert.AreEqual(1, highPriorityStudyTypeFilter.Count(), "Query yielded unexpected results for high or higher priority (study and type filter)");
				Assert.AreEqual("1" + _idCanceling, highPriorityStudyTypeFilter[0].StudyInstanceUid + highPriorityStudyTypeFilter[0].Type, "Query yield unexpected item (study and type filter)");
			}
		}

		[Test]
		public void TestGetWorkItems()
		{
			DeleteAllWorkItems();

			var time = DateTime.Now.Truncate();
			var pastTime = time.AddMinutes(-30);
			var futureTime = time.AddMinutes(30);

			long oidOfActive, oidOfDeleted, oidOfDeleting;
			using (var context = new DataAccessContext())
			{
				WorkItem itemA, itemB, itemC;
				var broker = context.GetWorkItemBroker();

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "1", status : WorkItemStatusEnum.Canceled, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "1", status : WorkItemStatusEnum.Canceling, processTime : pastTime, priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "1", status : WorkItemStatusEnum.Complete, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleting, "1", status : WorkItemStatusEnum.DeleteInProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idDeleted, "1", status : WorkItemStatusEnum.Deleted, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "1", status : WorkItemStatusEnum.Failed, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "1", status : WorkItemStatusEnum.Idle, processTime : pastTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idInProgress, "1", status : WorkItemStatusEnum.InProgress, processTime : pastTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "1", status : WorkItemStatusEnum.Pending, processTime : pastTime));

				broker.AddWorkItem(CreateWorkItem(_idCancelled, "2", status : WorkItemStatusEnum.Canceled, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idCanceling, "2", status : WorkItemStatusEnum.Canceling, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idComplete, "2", status : WorkItemStatusEnum.Complete, processTime : futureTime));
				broker.AddWorkItem(itemB = CreateWorkItem(_idDeleting, "2", status : WorkItemStatusEnum.DeleteInProgress, processTime : futureTime));
				broker.AddWorkItem(itemA = CreateWorkItem(_idDeleted, "2", status : WorkItemStatusEnum.Deleted, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idFailed, "2", status : WorkItemStatusEnum.Failed, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "2", status : WorkItemStatusEnum.Idle, processTime : futureTime, priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(itemC = CreateWorkItem(_idInProgress, "2", status : WorkItemStatusEnum.InProgress, processTime : futureTime));
				broker.AddWorkItem(CreateWorkItem(_idPending, "2", status : WorkItemStatusEnum.Pending, processTime : futureTime, priority : WorkItemPriorityEnum.Stat));

				broker.AddWorkItem(CreateWorkItem(_idIdling, "3", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-3), priority : WorkItemPriorityEnum.Stat));
				broker.AddWorkItem(CreateWorkItem(_idPending, "4", status : WorkItemStatusEnum.Pending, processTime : time.AddMinutes(-1), priority : WorkItemPriorityEnum.High));
				broker.AddWorkItem(CreateWorkItem(_idIdling, "5", status : WorkItemStatusEnum.Idle, processTime : time.AddMinutes(-1)));

				broker.AddWorkItem(CreateWorkItem(_idIdling, "999", status : WorkItemStatusEnum.Deleted, processTime : time.AddMinutes(-1)));

				context.Commit();

				oidOfDeleted = itemA.Oid;
				oidOfDeleting = itemB.Oid;
				oidOfActive = itemC.Oid;
			}

			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();

				var getByOid = broker.GetWorkItems(null, null, null, oidOfActive).SingleOrDefault();
				var getByDeletedOid = broker.GetWorkItems(null, null, null, oidOfDeleted).ToList();
				var getByOidAndStudy = broker.GetWorkItems(null, null, "1", oidOfDeleted).ToList();
				var getByStudy = broker.GetWorkItems(null, null, "2").ToList();
				var getByStatus = broker.GetWorkItems(null, WorkItemStatusFilter.RunningOrIdle, string.Empty).ToList();
				var getByStatusAndStudy = broker.GetWorkItems(null, WorkItemStatusFilter.RunningOrIdle, "2").ToList();
				var getByType = broker.GetWorkItems(_idIdling, null, null).ToList();
				var getByTypeAndStatus = broker.GetWorkItems(_idIdling, WorkItemStatusFilter.Nil, null).ToList();
				var getByTypeAndStatusAndStudy = broker.GetWorkItems(_idIdling, WorkItemStatusFilter.Nil, "2").SingleOrDefault();

				// this ensures that the results are valid by checking that future time is indeed still in the future
				Assert.Less(DateTime.Now, futureTime, "Rerun Test! - Results are invalid due to an unreasonable delay during test setup, causing future timestamps to not be in the future");

				Assert.IsNotNull(getByOid, "Query by OID did not return expected result");
				Assert.AreEqual(oidOfActive, getByOid.Oid, "Query by OID did not return expected result");
				Assert.IsFalse(getByDeletedOid.Any(), "Query by OID (Deleted) should not return any results");
				Assert.IsFalse(getByOidAndStudy.Any(), "Query by OID and Study should not return any results");

				Assert.AreEqual(7, getByStudy.Count, "Query by Study did not expected number of results");
				Assert.AreEqual(7, getByStudy.Select(x => x.Oid).Distinct().Count(), "Query by Study did not expected number of results");
				Assert.IsFalse(getByStudy.Any(x => x.Oid == oidOfDeleted || x.Oid == oidOfDeleting), "Query by Study returned deleted items");
				Assert.AreEqual(getByStudy.OrderBy(x => x.Priority).ThenBy(x => x.ProcessTime).ToList(), getByStudy, "Query by Study returned items out of order");

				Assert.AreEqual(8, getByStatus.Count, "Query by Status did not expected number of results");
				Assert.AreEqual(8, getByStatus.Select(x => x.Oid).Distinct().Count(), "Query by Status did not expected number of results");
				Assert.IsTrue(getByStatus.All(x => x.Status == WorkItemStatusEnum.Canceling || x.Status == WorkItemStatusEnum.InProgress || x.Status == WorkItemStatusEnum.Idle),
				              "Query by Status returned deleted items");
				Assert.AreEqual(getByStatus.OrderBy(x => x.Priority).ThenBy(x => x.ProcessTime).ToList(), getByStatus, "Query by Status returned items out of order");

				Assert.AreEqual(3, getByStatusAndStudy.Count, "Query by Status and Study did not expected number of results");
				Assert.AreEqual(3, getByStatusAndStudy.Select(x => x.Oid).Distinct().Count(), "Query by Status and Study did not expected number of results");
				Assert.IsTrue(getByStatusAndStudy.All(x => x.Status == WorkItemStatusEnum.Canceling || x.Status == WorkItemStatusEnum.InProgress || x.Status == WorkItemStatusEnum.Idle),
				              "Query by Status and Study returned deleted items");
				Assert.AreEqual(getByStatusAndStudy.OrderBy(x => x.Priority).ThenBy(x => x.ProcessTime).ToList(), getByStatusAndStudy, "Query by Status and Study returned items out of order");

				Assert.AreEqual(4, getByType.Count, "Query by Type did not expected number of results");
				Assert.AreEqual(4, getByType.Select(x => x.Oid).Distinct().Count(), "Query by Type did not expected number of results");
				Assert.IsTrue(getByType.All(x => x.Status == WorkItemStatusEnum.Canceling || x.Status == WorkItemStatusEnum.InProgress || x.Status == WorkItemStatusEnum.Idle),
				              "Query by Type returned deleted items");
				Assert.AreEqual(getByType.OrderBy(x => x.Priority).ThenBy(x => x.ProcessTime).ToList(), getByType, "Query by Type returned items out of order");

				Assert.AreEqual(5, getByTypeAndStatus.Count, "Query by Type and Status did not expected number of results");
				Assert.AreEqual(5, getByTypeAndStatus.Select(x => x.Oid).Distinct().Count(), "Query by Type and Status did not expected number of results");
				Assert.IsTrue(getByTypeAndStatus.All(x => x.Status == WorkItemStatusEnum.Canceling || x.Status == WorkItemStatusEnum.InProgress || x.Status == WorkItemStatusEnum.Idle || x.StudyInstanceUid == "999"),
				              "Query by Type and Status returned unexpected items");
				Assert.AreEqual(getByTypeAndStatus.OrderBy(x => x.Priority).ThenBy(x => x.ProcessTime).ToList(), getByTypeAndStatus, "Query by Type and Status returned items out of order");

				Assert.IsNotNull(getByTypeAndStatusAndStudy, "Query by Type and Status and Study did not expected results");
			}
		}

		internal static WorkItem CreateWorkItem(string type,
		                                        string studyInstanceUid,
		                                        WorkItemPriorityEnum priority = WorkItemPriorityEnum.Normal,
		                                        WorkItemStatusEnum status = WorkItemStatusEnum.Pending,
		                                        int failureCount = 0,
		                                        DateTime? requestedTime = null,
		                                        DateTime? scheduledTime = null,
		                                        DateTime? processTime = null,
		                                        DateTime? expirationTime = null,
		                                        DateTime? deleteTime = null,
		                                        string requestData = null,
		                                        string progressData = null)
		{
			var requestedTimeValue = requestedTime.GetValueOrDefault(DateTime.Now.Truncate());
			var scheduledTimeValue = scheduledTime.GetValueOrDefault(requestedTimeValue);
			var processTimeValue = processTime.GetValueOrDefault(scheduledTimeValue.AddSeconds(10));
			var expirationTimeValue = expirationTime.GetValueOrDefault(processTimeValue.AddSeconds(10));
			var deleteTimeValue = deleteTime.GetValueOrDefault(expirationTimeValue.AddMinutes(10));
			return new WorkItem
			       	{
			       		Type = type,
			       		StudyInstanceUid = studyInstanceUid,
			       		Priority = priority,
			       		Status = status,
			       		FailureCount = failureCount,
			       		RequestedTime = ConstrainSqlDateTime(requestedTimeValue),
			       		ScheduledTime = ConstrainSqlDateTime(scheduledTimeValue),
			       		ProcessTime = ConstrainSqlDateTime(processTimeValue),
			       		ExpirationTime = ConstrainSqlDateTime(expirationTimeValue),
			       		DeleteTime = ConstrainSqlDateTime(deleteTimeValue),
			       		Request = new MockRequest {WorkItemType = requestData},
			       		Progress = new MockProgress {StatusDetails = progressData}
			       	};
		}

		private static DateTime ConstrainSqlDateTime(DateTime value)
		{
			if (value < SqlDateTime.MinValue.Value) return SqlDateTime.MinValue.Value;
			else if (value > SqlDateTime.MaxValue.Value) return SqlDateTime.MaxValue.Value;
			else return value;
		}

		private static void DeleteAllWorkItems()
		{
			using (var context = new DataAccessContext())
			{
				var broker = context.GetWorkItemBroker();
				broker.DeleteAll();
				context.Commit();
			}
		}

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemRequestDataContract("{97CD6B97-A7B2-4759-9FED-9C2444527F93}")]
		private class MockRequest : WorkItemRequest
		{
			public override WorkItemConcurrency ConcurrencyType
			{
				get { return WorkItemConcurrency.Exclusive; }
			}

			public override string ActivityDescription
			{
				get { return string.Empty; }
			}

			public override string ActivityTypeString
			{
				get { return string.Empty; }
			}
		}

		[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
		[WorkItemProgressDataContract("{1EA8CC6A-04D3-4E32-B64A-B1E0A1D4BE4B}")]
		private class MockProgress : WorkItemProgress {}
	}
}

#endif