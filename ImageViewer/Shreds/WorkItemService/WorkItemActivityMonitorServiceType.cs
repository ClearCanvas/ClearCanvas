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
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class WorkItemActivityMonitorServiceType : IWorkItemActivityMonitorService, IDisposable
    {
        private readonly IWorkItemActivityCallback _callback;

        public WorkItemActivityMonitorServiceType()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IWorkItemActivityCallback>();
        }

        #region Implementation of IWorkItemActivityMonitorService

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            try
            {
				WorkItemPublishSubscribeHelper.Subscribe(_callback);
                return new WorkItemSubscribeResponse();
            }
            catch (Exception e)
            {
                var message = SR.ExceptionErrorProcessingSubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            try
            {
				WorkItemPublishSubscribeHelper.Unsubscribe(_callback);
				return new WorkItemUnsubscribeResponse();
            }
            catch (Exception e)
            {
                var message = SR.ExceptionErrorProcessingUnsubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }

        public void Refresh(WorkItemRefreshRequest request)
        {
        	ThreadPool.QueueUserWorkItem(
				delegate
        		{
					try
					{
						using (var context = new DataAccessContext())
						{
							var broker = context.GetWorkItemBroker();

							var dbList = broker.GetWorkItems(null, null, null);

							// send in batches of 200
							foreach (var batch in BatchItems(dbList, 200))
							{
								WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(WorkItemsChangedEventType.Refresh, batch.Select(WorkItemDataHelper.FromWorkItem).ToList());
							}
						}
					}
					catch (Exception e)
					{						
						var message = SR.ExceptionErrorProcessingRefresh;
						var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                        Platform.Log(LogLevel.Error, e,exceptionMessage);
                        // Don't rethrow here, we're in a thread pool anyways.
					}
				});
        }

        public WorkItemPublishResponse Publish(WorkItemPublishRequest request)
        {
            try
            {
				WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemsChangedEventType.Update, request.Item);
                return new WorkItemPublishResponse();
            }
            catch (Exception e)
            {
                var message = SR.ExceptionErrorProcessingPublish;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }
        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            try
            {
				WorkItemPublishSubscribeHelper.Unsubscribe(_callback);
			}
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        #endregion

		private static IEnumerable<List<T>> BatchItems<T>(IEnumerable<T> items, int batchSize)
		{
			var batch = new List<T>();
			foreach (var item in items)
			{
				batch.Add(item);
				if(batch.Count == batchSize)
				{
					yield return batch;
					batch = new List<T>();
				}
			}

			if (batch.Count > 0)
				yield return batch;
		}
    }
}
