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
using System.ComponentModel;
using System.Linq;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.WorkItem;
using NUnit.Framework;


namespace ClearCanvas.ImageViewer.StudyManagement.Tests
{

	[TestFixture]
	public class ActivityMonitorComponentTests
	{
		private static readonly Predicate<ActivityMonitorComponent.WorkItem> NoFilter = (w => true);
		private static readonly Predicate<ActivityMonitorComponent.WorkItem> NormalPriorityFilter = (w => w.Priority == WorkItemPriorityEnum.Normal);

		private static readonly Comparison<ActivityMonitorComponent.WorkItem> NopComparison = (x, y) => 0;
		private static readonly Comparison<ActivityMonitorComponent.WorkItem> StatusComparison = 
			(x, y) => String.CompareOrdinal(x.Status.ToString(), y.Status.ToString()); 

		public ActivityMonitorComponentTests()
		{
		}

	    [Test]
		public void Test_add_new_item()
	    {
	    	var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

	    	var data = new WorkItemData {Identifier = 1};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			Assert.AreEqual(1, items.Count);
			Assert.IsTrue(items.Any(item => item.Id == 1));
	    }

		[Test]
		public void Test_add_deleted_item()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

			var data = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Deleted};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_add_new_item_filtered()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();

			Assert.AreEqual(0, items.Count);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			// item not added because of filter
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_update_item()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Normal }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Normal, items[0].Priority);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Stat, items[0].Priority);
		}

		[Test]
		public void Test_update_item_delete()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Complete }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemStatusEnum.Complete, items[0].Status);

			var data = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Deleted};
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			// item removed from collection, because of deleted status
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_update_item_filtered()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			{ new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Normal }) };

			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(WorkItemPriorityEnum.Normal, items[0].Priority);

			var data = new WorkItemData { Identifier = 1, Priority = WorkItemPriorityEnum.Stat };
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(data) }, NopComparison);

			// item removed from collection, because filtered by priority
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void Test_clear_items()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});

			// add a failed item
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(failedItem) }, NopComparison);

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			manager.Clear();

			// items collection is cleared
			Assert.AreEqual(0, items.Count);

			// failed item count is cleared
			Assert.AreEqual(0, manager.FailedItemCount);
		}

		[Test]
		public void Test_failed_item_tracking()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => {});

			// add a failed item
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(failedItem) }, NopComparison);

			Assert.AreEqual(2, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			// add another failed item
			failedItem = new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(failedItem) }, NopComparison);
			
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(2, manager.FailedItemCount);

			// update one of the items to Pending (e.g. retry)
			var item = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Pending };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(item) }, NopComparison);

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

			// update other item to Pending (e.g. retry)
			item = new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Pending };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(item) }, NopComparison);

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(0, manager.FailedItemCount);

			// update an item to failed
			item = new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Failed };
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(item) }, NopComparison);

			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(1, manager.FailedItemCount);

		}

		[Test]
		public void Test_failed_item_tracking_ignores_filter()
		{
			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>
			            	{
			            		new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1 })
			            	};

			Assert.AreEqual(1, items.Count);

			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NormalPriorityFilter, () => { });

			// add a failed item that does not meet the filter criteria
			var failedItem = new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Failed, Priority = WorkItemPriorityEnum.Stat};
			manager.Update(new[] { new ActivityMonitorComponent.WorkItem(failedItem) }, NopComparison);

			// verify that the item is not added to the collection, but is tracked as a failed item
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(1, items[0].Id);
			Assert.AreEqual(1, manager.FailedItemCount);
		}

		[Test]
		public void Test_sorting_items_inserted_in_sorted_order()
		{
			var item1 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Canceled });
			var item2 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Complete });
			var item3 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Failed });
			var item4 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 4, Status = WorkItemStatusEnum.Idle });
			var item5 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 5, Status = WorkItemStatusEnum.Pending });

			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => { });

			// insert items in sorted order, and verify that this order is preserved
			manager.Update(new [] { item1, item2, item3, item4, item5 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));

			// insert items in reverse order, and verify that they still end up in sorted order
			items.Clear();
			manager.Update(new[] { item5, item4, item3, item2, item1 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));

			// insert items out of sorted order, and verify that they still end up in sorted order
			items.Clear();
			manager.Update(new[] { item3, item5, item4, item1, item2 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));
		}

		[Test]
		public void Test_sorting_update_item_changes_order()
		{
			var item1 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Canceled });
			var item2 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Complete });
			var item3 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Failed });
			var item4 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 4, Status = WorkItemStatusEnum.Idle });
			var item5 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 5, Status = WorkItemStatusEnum.Pending });

			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => { });

			// insert items in sorted order, and verify that this order is preserved
			manager.Update(new[] { item1, item2, item3, item4, item5 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));

			// item 1 changes to Complete - should not cause a change in list order
			item1 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Canceling });
			manager.Update(new[] { item1 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));

			// item 4 changes to Complete, causing it to be repositioned in the list
			item4 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 4, Status = WorkItemStatusEnum.Complete });
			manager.Update(new[] { item4 }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item4, item3, item5 }) || items.SequenceEqual(new[] { item1, item4, item2, item3, item5 }));
		}

		[Test]
		public void Test_sorting_update_nonsort_property_retains_order()
		{
			var item1 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Normal });
			var item2 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 2, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Normal });
			var item3 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 3, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Normal });
			var item4 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 4, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Normal });
			var item5 = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 5, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Normal });

			var items = new ItemCollection<ActivityMonitorComponent.WorkItem>();
			items.AddRange(new[] { item1, item2, item3, item4, item5 });

			// the idea here is that if an item is updated, but the sorty property (Status) has not changed,
			// then the item does not move around in the list, but is updated in place
			var manager = new ActivityMonitorComponent.WorkItemUpdateManager(items, NoFilter, () => { });

			// item 1 Priority changes - should not cause a change in list order
			var x = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 1, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.Stat});
			manager.Update(new[] { x }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));

			// item 4 Priority changes - should not cause a change in list order
			var y = new ActivityMonitorComponent.WorkItem(new WorkItemData { Identifier = 4, Status = WorkItemStatusEnum.Pending, Priority = WorkItemPriorityEnum.High});
			manager.Update(new[] { y }, StatusComparison);
			Assert.IsTrue(items.SequenceEqual(new[] { item1, item2, item3, item4, item5 }));
		}
	}
}

#endif