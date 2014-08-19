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
using System.Web;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.Model;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Provides methods to interact with the database to query/update work queue.
    /// </summary>
    public class WorkQueueController : BaseController
    {
        #region Private Members

        
        #endregion

		#region Static Public Methods

        /// <summary>
        /// Gets the <see cref="StudyStorageLocation"/> for the study associated with the specified <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static StudyStorageLocation GetLoadStorageLocation(WorkQueue item)
        {

			var select = HttpContext.Current.GetSharedPersistentContext().GetBroker<IQueryStudyStorageLocation>();

            var parms = new StudyStorageLocationQueryParameters();
            parms.StudyStorageKey = item.StudyStorageKey;

            IList<StudyStorageLocation> storages = select.Find(parms);

            if (storages == null || storages.Count == 0)
            {
                Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}",
                             item.Key.ToString());
                throw new ApplicationException("Unable to find storage location for WorkQueue item.");
            }

            if (storages.Count > 1)
            {
                Platform.Log(LogLevel.Warn,
                             "WorkQueueController:LoadWritableStorageLocation: multiple study storage found for work queue item {0}",
                             item.Key.Key);
            }

            return storages[0];

        }

        /// <summary>
        /// Gets the <see cref="StudyStorageLocation"/> for the study associated with the specified <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public String GetLoadDuplicateStorageLocation(WorkQueue item)
        {
            XmlDocument document = item.Data;
            
           
            return document.ToString();

        }


        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually rescheduled.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReschedule(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                // it's pending
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Pending
                // it's idle
                || item.WorkQueueStatusEnum == WorkQueueStatusEnum.Idle;
                
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually reset.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReset(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed
                /* nobody claimed it */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress && String.IsNullOrEmpty(item.ProcessorID))
                /* allow reset "stuck" items (except items that are InProgress)*/
                || (!item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress) && !WorkQueueHelper.IsActiveWorkQueue(item));

        }


        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually deleted from the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanDelete(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed
                /* completed item */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.Completed)
                /* nobody claimed it */
                ||
                (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress &&
                 String.IsNullOrEmpty(item.ProcessorID))
                // allow deletes of some pending entries
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
                ||
                (item.WorkQueueStatusEnum != WorkQueueStatusEnum.InProgress &&
                 item.WorkQueueTypeEnum == WorkQueueTypeEnum.WebDeleteStudy)

				/* allow deletes of "stuck" items (except items that are InProgress)*/
                || (!item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress) && !WorkQueueHelper.IsActiveWorkQueue(item));
		}


        static public bool CanReprocess(WorkQueue item)
        {
            return
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed && item.WorkQueueTypeEnum == WorkQueueTypeEnum.StudyProcess;
        }
		#endregion Static Public Methods

		#region Public Methods

		/// <summary>
        /// Gets a list of <see cref="WorkQueue"/> items with specified criteria
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<WorkQueue> FindWorkQueue(WebWorkQueueQueryParameters parameters)
        {
            try
            {
                IList<WorkQueue> list;

                IWebQueryWorkQueue broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IWebQueryWorkQueue>();
                list = broker.Find(parameters);
                return list;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to retrieve work queue.");
            	return new List<WorkQueue>();
            }
        }

        public IList<WorkQueueInfo> GetWorkQueueOverview()
        {            
            IList<WorkQueueInfo> workQueueInfo = new List<WorkQueueInfo>();
            
            WorkQueueProcessorIDsParameters parameters = new WorkQueueProcessorIDsParameters();
            IWorkQueueProcessorIDs broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IWorkQueueProcessorIDs>();
            IList<WorkQueue> processorIDList = broker.Find(parameters);

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
            int numItems = 0;
            foreach(WorkQueue row in processorIDList)
            {
                criteria.ProcessorID.EqualTo(row.ProcessorID);
                numItems = adaptor.GetCount(criteria);
                workQueueInfo.Add(new WorkQueueInfo(row.ProcessorID, numItems));
            }
            return workQueueInfo;
        }

        /// <summary>
        /// Deletes a list of <see cref="WorkQueue"/> items from the system.
        /// </summary>
        /// <param name="items">The list of <see cref="WorkQueue"/> items to be deleted</param>
        /// <returns>A value indicating whether all items have been successfully deleted.</returns>
        /// 
        /// <remarks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be deleted, the method will return <b>false</b>
        /// and the deletion will be undone (i.e., All of the <see cref="WorkQueue"/> items will remain in the database)
        /// </remarks>
        public bool DeleteWorkQueueItems(IList<WorkQueue> items)
        {
            if (items == null || items.Count == 0)
                return false;

            bool result = true;

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext uctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IDeleteWorkQueue delete = uctx.GetBroker<IDeleteWorkQueue>();
                    
                foreach (WorkQueue item in items)
                {
					WorkQueueDeleteParameters parms = new WorkQueueDeleteParameters();
                    parms.ServerPartitionKey = item.ServerPartitionKey;
                    parms.StudyStorageKey = item.StudyStorageKey;
                    parms.WorkQueueKey = item.Key;
                    parms.WorkQueueTypeEnum = item.WorkQueueTypeEnum;
					// NOTE: QueueStudyState is reset by the stored procedure
                    if (!delete.Execute(parms))
                    {
                        Platform.Log(LogLevel.Error, "Unexpected error trying to delete WorkQueue entry");
                        result = false;
                    }
                }

                if (result)
                    uctx.Commit();
            }

            return result;
        }

        /// <summary>
        /// Reschedule a list of <see cref="WorkQueue"/> items
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/> items to be rescheduled</param>
        /// <param name="newScheduledTime">New schedule start date/time</param>
        /// <param name="expirationTime">New expiration date/time</param>
        /// <param name="priority">New priority</param>
        /// <returns>A value indicating whether all <see cref="WorkQueue"/> items in <paramref name="items"/> are updated successfully.</returns>
        /// <remarks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be rescheduled, all changes will be 
        /// reverted and <b>false</b> will be returned.
        /// </remarks>
        public bool RescheduleWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime, WorkQueuePriorityEnum priority)
        {
            if (items == null || items.Count == 0)
                return false;

            WorkQueueUpdateColumns updatedColumns = new WorkQueueUpdateColumns();
            updatedColumns.WorkQueuePriorityEnum = priority;
            updatedColumns.ScheduledTime = newScheduledTime;
            updatedColumns.ExpirationTime = expirationTime;
            updatedColumns.FailureCount = 0;
            updatedColumns.FailureDescription = String.Empty;
            updatedColumns.LastUpdatedTime = Platform.Time;

            bool result = false;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                foreach (WorkQueue item in items)
                {
                    result = workQueueBroker.Update(item.Key, updatedColumns);
                    if (!result)
                    {
                        break;
                    }
                }
                if (result)
                    ctx.Commit();
            }

            return result;
        }


        /// <summary>
        /// Resets a list of <see cref="WorkQueue"/> items.
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/>  to be reset</param>
        /// <param name="newScheduledTime">The new scheduled start date/time for the entries</param>
        /// <param name="expirationTime">The new expiration start date/time for the entries</param>
        public void ResetWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime)
        {
            if (items == null || items.Count==0)
                return;

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                IWebResetWorkQueue broker = ctx.GetBroker<IWebResetWorkQueue>();
                foreach(WorkQueue item in items)
                {
                    WebResetWorkQueueParameters parameters = new WebResetWorkQueueParameters
                                                                 {
                                                                     WorkQueueKey = item.Key,
                                                                     NewScheduledTime= newScheduledTime,
                                                                     NewExpirationTime = expirationTime
                                                                 };

                    if (!broker.Execute(parameters))
                    {
                        Platform.Log(LogLevel.Error,
                                     "Unexpected error when calling WebResetWorkQueue stored procedure. Could not reset {0} work queue entry {1}",
                                     item.WorkQueueTypeEnum.Description, item.Key);
                    }
                }
            }
        }

		public bool ReprocessWorkQueueItem(WorkQueue item)
		{
            // #10620: Get a list of remaining WorkQueueUids which need to be reprocess
            // Note: currently only WorkQueueUIDs in failed StudyProcess will be reprocessed
            var remainingWorkQueueUidPaths = item.GetAllWorkQueueUidPaths();

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				// delete current workqueue
				IWorkQueueUidEntityBroker uidBroker = ctx.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
				criteria.WorkQueueKey.EqualTo(item.GetKey());

				if (uidBroker.Delete(criteria) >= 0)
				{
					IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
					if (workQueueBroker.Delete(item.GetKey()))
					{
					    IList<StudyStorageLocation> locations = item.LoadStudyLocations(ctx);
                        if (locations!=null && locations.Count>0)
                        {
                            StudyReprocessor reprocessor = new StudyReprocessor();
                            String reason = String.Format("User reprocesses failed {0}", item.WorkQueueTypeEnum);
                            WorkQueue reprocessEntry = reprocessor.ReprocessStudy(ctx, reason, locations[0], remainingWorkQueueUidPaths, Platform.Time);
							if (reprocessEntry!=null)
								ctx.Commit();
                        	return reprocessEntry!=null;
                        }	
					}
				}
			}
			return false;
		}

    	#endregion
    }

    
}
