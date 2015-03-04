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
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.DeleteStudy;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomRetrieve;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.Import;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.ProcessStudy;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.ReapplyRules;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Tests
{
	internal class SchedulingTest
	{
		public IWorkItemProcessor Processor { get; set; }
		public WorkItemStatusEnum ExpectedStatus { get; set; }
		public string Message { get; set; }
		public WorkItemPriorityEnum Priority { get; set; }
	}

	[TestFixture]
	public class WorkItemSchedulingTest : AbstractTest
	{
		private IWorkItemProcessor InsertImportFiles(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new ImportFilesRequest()
			         		          	{
			         		          		Priority = priority,
			         		          		BadFileBehaviour = BadFileBehaviourEnum.Delete,
			         		          		FileImportBehaviour = FileImportBehaviourEnum.Save,
			         		          		FilePaths = new List<string>(),
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			// TODO (CR Jul 2012): Can I actually force an item to "In Progress" this way? Probably shouldn't be able to do that.
			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();

				var d = new ImportItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertStudyDelete(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new DeleteStudyRequest
			         		          	{
			         		          		Patient = new WorkItemPatient(msg.DataSet),
			         		          		Study = new WorkItemStudy(msg.DataSet),
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();

				var d = new DeleteStudyItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertStudyProcess(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status, DateTime? processTime = null)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new DicomReceiveRequest
			         		          	{
			         		          		Patient = new WorkItemPatient(msg.DataSet),
			         		          		Study = new WorkItemStudy(msg.DataSet),
			         		          		SourceServerName = "TEST",
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier,
			                    		ProcessTime = processTime
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();
				var d = new StudyProcessProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertSendStudy(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new DicomSendStudyRequest
			         		          	{
			         		          		Patient = new WorkItemPatient(msg.DataSet),
			         		          		Study = new WorkItemStudy(msg.DataSet),
			         		          		DestinationServerName = "Dest AE",
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();
				var d = new DicomSendItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertRetrieveStudy(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new DicomRetrieveStudyRequest()
			         		          	{
			         		          		Patient = new WorkItemPatient(msg.DataSet),
			         		          		Study = new WorkItemStudy(msg.DataSet),
			         		          		ServerName = "Dest AE",
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();
				var d = new DicomRetrieveItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertReindex(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new ReindexRequest
			         		          	{
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();
				var d = new ReindexItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private IWorkItemProcessor InsertReapplyRules(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
		{
			var rq = new WorkItemInsertRequest
			         	{
			         		Request = new ReapplyRulesRequest
			         		          	{
			         		          		ApplyDeleteActions = true,
			         		          		ApplyRouteActions = true,
			         		          		Priority = priority
			         		          	}
			         	};
			var rsp = WorkItemService.Instance.Insert(rq);

			var updateRequest = new WorkItemUpdateRequest
			                    	{
			                    		Status = status,
			                    		Identifier = rsp.Item.Identifier
			                    	};

			WorkItemService.Instance.Update(updateRequest);

			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();
				var d = new ReapplyRulesItemProcessor();
				d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
				return d;
			}
		}

		private void DeleteWorkItems(IEnumerable<SchedulingTest> list)
		{
			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();

				foreach (var test in list)
				{
					var item = broker.GetWorkItem(test.Processor.Proxy.Item.Oid);

					broker.Delete(item);
				}

				context.Commit();
			}
		}

		private void DeleteAllWorkItems()
		{
			using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
			{
				var broker = context.GetWorkItemBroker();

				foreach (var test in broker.GetWorkItems(null, null, null))
					broker.Delete(test);

				context.Commit();
			}
		}

		private void DoTest(IList<SchedulingTest> list, int expectedItemsCount)
		{
			var allItems = new List<WorkItem>();
			List<WorkItem> items;
			do
			{
				items = WorkItemProcessor.GetWorkItems(10, 10);
				allItems.AddRange(items);
			} while (items.Count > 0);

			foreach (var test in list)
			{
				using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
				{
					var broker = context.GetWorkItemBroker();
					var realWorkItem = broker.GetWorkItem(test.Processor.Proxy.Item.Oid);
					Assert.AreEqual(test.ExpectedStatus, realWorkItem.Status);
				}
			}

			Assert.AreEqual(expectedItemsCount, allItems.Count);
			DeleteWorkItems(list);
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			TestSettingsStore.Instance.Reset();

			Platform.SetExtensionFactory(new UnitTestExtensionFactory
			                             	{
			                             		{typeof (CacheProviderExtensionPoint), typeof (MemoryCacheProvider)},
			                             		{typeof (ServiceProviderExtensionPoint), typeof (TestSystemConfigurationServiceProvider)},
			                             		{typeof (ServiceProviderExtensionPoint), typeof (StudyStoreTestServiceProvider)}
			                             	});
		}

		[Test]
		public void Test01()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test02()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test03()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 3",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 4",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 3);
		}

		[Test]
		public void Test04()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process 1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process 2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test05Reindex()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test06Reapply()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg = new DicomMessage();
			SetupMR(msg.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test07Reapply()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules 1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules 2",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test08()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg2",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test09Priorities()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test10Priorities()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending // Updates wait for reads
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test11Priorities()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test12Priorities()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			// Stat Read + Stat Update scheduled later, Noraml priority must wait

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1 Stat",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test13ReindexStat()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();

			// Reindex causes later scheduled/lower priority non-exclusive entries to wait
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules 1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test14ReindexStat2()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();

			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules 2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test15Priorities()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study send msg2 normal",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg2",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 3);
		}

		[Test]
		public void Test16InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			// Lower Priority later scheduled item in progress, delete should wait
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Study Send msg1 In Progress",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			// In Progress reads
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Study send msg2 normal in progress",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg2 Normal",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg2 Stat",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test17InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			// Lower Priority later scheduled item in progress, delete should wait
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Study Send msg1 In Progress",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			// In Progress reads
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Study send msg2 normal in progress",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg2 Normal",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg2 Stat",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete msg2",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 3);
		}

		[Test]
		public void Test18InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.InProgress),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor =
			         			InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			//This should start because it can run at the same time as the import files that is already running.
			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor =
			         			InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test19InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			// Reindex waits for lower priority item
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor =
			         			InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test20InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);
			var msg2 = new DicomMessage();
			SetupMR(msg2.DataSet);

			// Reindex in progress, all others wait, including those with higher priorities
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress,
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor =
			         			InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Send msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReapplyRules(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Reapply Rules",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test21InProgress()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();

			// Reindex waits for lower priority item
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.InProgress),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test22IdleImport()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Process 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test23IdleImport()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, DateTime.Now + TimeSpan.FromSeconds(30)),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Idle
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test24IdleImport()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
			         		Message = "Study Process msg1",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process 2 msg1",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test25IdleImportStatReindex()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test26IdleImportStatReindex()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, DateTime.Now + TimeSpan.FromSeconds(30)),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Idle
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test27IdleImportWithDelete()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test28IdleImportWithDelete()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, DateTime.Now + TimeSpan.FromSeconds(30)),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Idle
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test29IdleImportStatDelete()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test30IdleImportStatDelete()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, DateTime.Now + TimeSpan.FromSeconds(30)),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Idle
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test31IdleImportWithSend()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Send Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test32IdleImportWithStatSend()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Idle, DateTime.Now + TimeSpan.FromSeconds(30)),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Idle
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Send Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test33CancellingImportStatReindex()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.Canceling),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.Canceling
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending,
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test34CancellingReindexStatImport()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			//A re-index that is canceling is still in the process of restoring study xml, etc. An import
			//cannot run at the same time as a re-index that is canceling.
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.High, WorkItemStatusEnum.Canceling),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Canceling,
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertImportFiles(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
			         		Message = "Import Files",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 0);
		}

		[Test]
		public void Test35SendWithRetrieve()
		{
			DeleteAllWorkItems();

			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Send Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress,
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test36RetrieveAndReceive()
		{
			DeleteAllWorkItems();

			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Receive Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test37RetrieveAndDelete()
		{
			DeleteAllWorkItems();

			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);
			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test38DeleteAndRetrieve()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Delete",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test39ImportAndRetrieve()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test40ImportAndRetrieve()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test41RetrieveMultipleImports()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Receive Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Import Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test42MultipleRetrievesFollowedByImport()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertRetrieveStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Retrieve Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Import Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Import Study",
			         		ExpectedStatus = WorkItemStatusEnum.Pending
			         	});

			DoTest(list, 2);
		}

		[Test]
		public void Test43FailedImportWithReindex()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Failed),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Failed
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertReindex(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Reindex",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test44FailedImportWithSend()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Failed),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Failed
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Send Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}

		[Test]
		public void Test45CompleteImportWithSend()
		{
			DeleteAllWorkItems();
			var list = new List<SchedulingTest>();
			var msg1 = new DicomMessage();
			SetupMR(msg1.DataSet);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Complete),
			         		Message = "Study Process",
			         		ExpectedStatus = WorkItemStatusEnum.Complete
			         	});

			Thread.Sleep(2);

			list.Add(new SchedulingTest
			         	{
			         		Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
			         		Message = "Send Study",
			         		ExpectedStatus = WorkItemStatusEnum.InProgress
			         	});

			DoTest(list, 1);
		}
	}
}

#endif