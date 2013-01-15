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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// <summary>
    /// Enum telling if a work queue entry had a fatal or nonfatal error.
    /// </summary>
    public enum WorkItemFailureType
    {
        /// <summary>
        /// Fatal errors cause the WorkItem to fail immediately.
        /// </summary>
        Fatal,
        /// <summary>
        /// Non-fatal errors cause a retry of the WorkItem
        /// </summary>
        NonFatal
    }

    /// <summary>
    /// Proxy class for updating the status and progress of a <see cref="WorkItem"/>.
    /// </summary>
    public class WorkItemStatusProxy
    {
        #region Public Properties

        // Hard-coded log level for proxy
        public LogLevel LogLevel = LogLevel.Debug;

        public WorkItem Item { get; private set; }
        public WorkItemProgress Progress { get; set; }
        public WorkItemRequest Request { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">The WorkItem to create a proxy for.</param>
        public WorkItemStatusProxy(WorkItem item)
        {
            Item = item;
            Progress = item.Progress;
            Request = item.Request;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and save a reason.
        /// </summary>
        /// <param name="reason">A non-localized reason for the failure.</param>
        /// <param name="failureType">The type of failure.</param>
        public void Fail(string reason, WorkItemFailureType failureType)
        {
            Progress.StatusDetails = reason;
            Fail(failureType);
        }

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and save a reason.
        /// </summary>
        /// <param name="reason">A non-localized reason for the failure.</param>
        /// <param name="failureType">The type of failure.</param>
        /// <param name="scheduledTime">The time to reschedule the WorkItem if it isn't a fatal error. </param>
        public void Fail(string reason, WorkItemFailureType failureType, DateTime scheduledTime)
        {
            Progress.StatusDetails = reason;
            Fail(failureType, scheduledTime, WorkItemServiceSettings.Default.RetryCount);
        }

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and save a reason.
        /// </summary>
        /// <param name="reason">A non-localized reason for the failure.</param>
        /// <param name="failureType">The type of failure.</param>
        /// <param name="scheduledTime">The time to reschedule the WorkItem if it isn't a fatal error. </param>
        /// <param name="retryCount"> </param>
        public void Fail(string reason, WorkItemFailureType failureType, DateTime scheduledTime, int retryCount)
        {
            Progress.StatusDetails = reason;
            Fail(failureType, scheduledTime, retryCount);
        }

        /// <summary>
        /// SImple routine for failing a <see cref="WorkItem"/>
        /// </summary>
        /// <param name="failureType"></param>
        public void Fail(WorkItemFailureType failureType)
        {
            Fail(failureType, Platform.Time.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds),WorkItemServiceSettings.Default.RetryCount);
        }

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and rescheduling it at a specified time.
        /// </summary>
        /// <param name="failureType"></param>
        /// <param name="failureTime">The time to reschedule the WorkItem if it isn't a fatal error. </param>
        /// <param name="maxRetryCount">The maximum number of times the WorkItem should be retried before a fatal error occurs.</param>
        public void Fail(WorkItemFailureType failureType, DateTime failureTime, int maxRetryCount)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.FailureCount = Item.FailureCount + 1;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);

                if (Item.FailureCount >= maxRetryCount
                    || failureType == WorkItemFailureType.Fatal )
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                }
                else
                {
                    Item.ProcessTime = failureTime;
                    if (Item.ExpirationTime < Item.ProcessTime)
                        Item.ExpirationTime = Item.ProcessTime;
                    Item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Failing {0} WorkItem for OID {1}: {2}", Item.Type, Item.Oid, Item.Request.ActivityDescription);
        }

                /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        public void Postpone()
        {
            Postpone(TimeSpan.FromSeconds(WorkItemServiceSettings.Default.PostponeSeconds));
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        public void Postpone(TimeSpan delay)
        {
            DateTime now = Platform.Time;

            var workItem = Item.Request as IWorkItemRequestTimeWindow;

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                if (workItem != null && Item.Priority != WorkItemPriorityEnum.Stat)
                {
                    DateTime scheduledTime = workItem.GetScheduledTime(now, delay.Seconds);
                    Item.ProcessTime = scheduledTime;
                    Item.ScheduledTime = scheduledTime;      
                }
                else
                {
                    Item.ProcessTime = now.Add(delay);
                }
                Item.Progress = Progress;

                if (Item.ProcessTime > Item.ExpirationTime)
                    Item.ExpirationTime = Item.ProcessTime;
                Item.Status = WorkItemStatusEnum.Pending;
                context.Commit();


                Publish(false);
                Platform.Log(LogLevel, "Postponing {0} WorkItem for OID {1} until {2}, expires {3}", Item.Type, Item.Oid,
                             Item.ProcessTime.ToLongTimeString(), Item.ExpirationTime.ToLongTimeString());
            }
        }

        /// <summary>
        /// Complete a <see cref="WorkItem"/>.
        /// </summary>
        public void Complete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
           
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;
                
                // Since we're completing, no need for additional status, its done.
                Progress.StatusDetails = string.Empty;

                Item.Progress = Progress;
                Item.ProcessTime = now;
                Item.ExpirationTime = now;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Complete;

                var uidBroker = context.GetWorkItemUidBroker();
                foreach (var entity in Item.WorkItemUids)
                {
                    uidBroker.Delete(entity);
                }

                context.Commit();
            }

            Publish(false);
            var studyRequest = Item.Request as WorkItemStudyRequest;
            if (studyRequest != null)
                Platform.Log(LogLevel.Info, "Completing {0} WorkItem for OID {1}: {2}, {3}:{4}", Item.Type, Item.Oid,
                             Item.Request.ActivityDescription,
                             studyRequest.Patient.PatientsName, studyRequest.Patient.PatientId);
            else
                Platform.Log(LogLevel.Info, "Completing {0} WorkItem for OID {1}: {2}", Item.Type, Item.Oid,
                             Item.Request.ActivityDescription);
        }

        /// <summary>
        /// Make a <see cref="WorkItem"/> Idle.
        /// </summary>
        public void Idle()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
              
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds);
                if (Item.ProcessTime > Item.ExpirationTime)
                    Item.ProcessTime = Item.ExpirationTime;
                Item.Status = WorkItemStatusEnum.Idle;

                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Idling {0} WorkItem for OID {1} until {2}, expires {3}", Item.Type, Item.Oid, Item.ProcessTime.ToLongTimeString(), Item.ExpirationTime.ToLongTimeString());
        }

        /// <summary>
        /// Mark <see cref="WorkItem"/> as being in the process of canceling
        /// </summary>
        public void Canceling()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.Progress = Progress;
                Item.Status = WorkItemStatusEnum.Canceling;
                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Canceling {0} WorkItem for OID {1}: {2}", Item.Type, Item.Oid, Item.Request.ActivityDescription);
        }

        /// <summary>
        /// Cancel a <see cref="WorkItem"/>
        /// </summary>
        public void Cancel()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                
                Item = broker.GetWorkItem(Item.Oid);
                
                DateTime now = Platform.Time;

                Item.ProcessTime = now;
                Item.ExpirationTime = now;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Canceled;
                Item.Progress = Progress;
                
                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Canceling {0} WorkItem for OID {1}: {2}", Item.Type, Item.Oid, Item.Request.ActivityDescription);
        }

        /// <summary>
        /// Delete a <see cref="WorkItem"/>.
        /// </summary>
        public void Delete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                Item = broker.GetWorkItem(Item.Oid);
                Item.Status = WorkItemStatusEnum.Deleted;
                broker.Delete(Item);

                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Deleting {0} WorkItem for OID {1}: {2}", Item.Type, Item.Oid, Item.Request.ActivityDescription);
        }

        /// <summary>
        /// Update the progress for a <see cref="WorkItem"/>.  Progress will be published.
        /// </summary>
        public void UpdateProgress()
        {
            // We were originally committing to the database here, but decided to only commit when done processing the WorkItem.
            // This could lead to some misleading progress if a Refresh is done.
             Publish(false);
        }

        /// <summary>
        /// Update the progress for a <see cref="WorkItem"/>.  Progress will be published.
        /// </summary>
        public void UpdateProgress(bool updateDatabase)
        {
            // We were originally committing to the database here, but decided to only commit when done processing the WorkItem.
            // This could lead to some misleading progress if a Refresh is done.
            Publish(updateDatabase);
        }
        #endregion

        #region Private Methods

        private void Publish(bool saveToDatabase)
        {
            if (saveToDatabase)
            {
                try
                {
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetWorkItemBroker();

                        Item = broker.GetWorkItem(Item.Oid);

                        Item.Progress = Progress;

                        context.Commit();
                    }
                }
                catch (Exception)
                {
                    // Saw ChangeCOnflictException here a few times
                }
            }
            else
                Item.Progress = Progress;

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemsChangedEventType.Update, WorkItemDataHelper.FromWorkItem(Item));
        }

        #endregion
    }
}
