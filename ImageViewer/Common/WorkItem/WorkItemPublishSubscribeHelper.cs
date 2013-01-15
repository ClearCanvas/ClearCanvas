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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    // TODO (CR Jun 2012): I think this should go back in Shreds, or at least be put in a sub-namespace somewhere. Feels weird in here.
	public static class WorkItemPublishSubscribeHelper
	{
		private const string WorkItemsChanged = "WorkItemsChanged";
		private const string StudiesCleared = "StudiesCleared";

        public static void Subscribe(IWorkItemActivityCallback callback)
        {
            try
            {
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(callback, WorkItemsChanged);
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(callback, StudiesCleared);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw;
            }
        }

        public static void Unsubscribe(IWorkItemActivityCallback callback)
        {
            try
            {
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(callback, WorkItemsChanged);
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(callback, StudiesCleared);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw;
            }
        }
        
        public static void PublishWorkItemChanged(WorkItemsChangedEventType eventType, WorkItemData workItem)
		{
			PublishWorkItemsChanged(eventType, new List<WorkItemData> { workItem });
		}

		public static void PublishWorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish(WorkItemsChanged, eventType, workItems);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItemsChanged notification.");
			}
		}

		public static void PublishStudiesCleared()
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish(StudiesCleared);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish StudiesCleared notification.");
			}
		}
	}
}
