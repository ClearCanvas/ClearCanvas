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
using System.Data.SqlClient;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// <summary>
    /// Abstract base class for processing WorkItems.
    /// </summary>
    /// <typeparam name="TRequest">The request object for the work item.</typeparam>
    /// <typeparam name="TProgress">The progress object for the work item.</typeparam>
    public abstract class BaseItemProcessor<TRequest, TProgress> : IWorkItemProcessor
        where TProgress : WorkItemProgress, new()
        where TRequest : WorkItemRequest
    {
        #region Private Fields

        private const int MAX_DB_RETRY = 5;
        private string _name = "Work Item";
        private IList<WorkItemUid> _uidList;
        private volatile bool _cancelPending;
        private volatile bool _stopPending;
        private readonly object _syncRoot = new object();
        private StorageConfiguration _storageConfig;
        #endregion

        #region Properties

        /// <summary>
        /// The progress object for the WorkItem.  Note that all updates to the progress should
        /// be done through this object, and not through the <see cref="Proxy"/> property.
        /// </summary>
        public TProgress Progress
        {
            get { return Proxy.Progress as TProgress; }
        }

        /// <summary>
        /// The request object for the WorkItem.
        /// </summary>
        public TRequest Request
        {
            get { return Proxy.Request as TRequest; }
        }

        protected IList<WorkItemUid> WorkQueueUidList
        {
            get
            {
                if (_uidList == null)
                {
                    LoadUids();
                }
                return _uidList;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public WorkItemStatusProxy Proxy { get; set; }

        public StudyLocation Location { get; set; }

        protected bool CancelPending
        {
            get { return _cancelPending; }
        }

        protected bool StopPending
        {
            get { return _stopPending; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by the base to initialize the processor.
        /// </summary>
        public virtual bool Initialize(WorkItemStatusProxy proxy)
        {
            Proxy = proxy;
            if (proxy.Progress == null)
                proxy.Progress = new TProgress();
            else if (!(proxy.Progress is TProgress))
                proxy.Progress = new TProgress();

            if (Request == null)
                throw new ApplicationException(SR.InternalError);

            if (!string.IsNullOrEmpty(proxy.Item.StudyInstanceUid))
                Location = new StudyLocation(proxy.Item.StudyInstanceUid);

            return true;
        }

        public virtual void Cancel()
        {
            Proxy.Canceling();

            lock (_syncRoot)
                _cancelPending = true;
        }

        public virtual void Stop()
        {
            lock (_syncRoot)
                _stopPending = true;
        }
       

        public abstract void Process();

        public virtual void Delete()
        {
            Proxy.Delete();
        }

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Ensure minimum amount of space available in the local data store
        /// </summary>
        /// <exception cref="NotEnoughStorageException"/>
        protected void EnsureMaxUsedSpaceNotExceeded()
        {
            if (LocalStorageMonitor.IsMaxUsedSpaceExceeded)
            {
                Platform.Log(LogLevel.Error, "Not enough storage space. Max Used={0}%, Total Used={1} %",
                                    LocalStorageMonitor.MaximumUsedSpacePercent,
                                    LocalStorageMonitor.UsedSpacePercent);
                throw new NotEnoughStorageException();
            }
        }


        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        protected void LoadUids()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemUidBroker();

                _uidList = broker.GetWorkItemUidsForWorkItem(Proxy.Item.Oid);
            }
        }

        /// <summary>
        /// Routine for failing a work queue uid record.
        /// </summary>
        /// <param name="uid">The WorkItemUid entry to fail.</param>
        /// <param name="retry">A boolean value indicating whether a retry will be attempted later.</param>
        protected WorkItemUid FailWorkItemUid(WorkItemUid uid, bool retry)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemUidBroker();
                var sop = broker.GetWorkItemUid(uid.Oid);

                if (!sop.FailureCount.HasValue)
                    sop.FailureCount = 1;
                else
                    sop.FailureCount = (byte) (sop.FailureCount.Value + 1);

                if (sop.FailureCount > WorkItemServiceSettings.Default.RetryCount)
                    sop.Failed = true;

                context.Commit();
                return sop;
            }
        }

        protected static DicomServerConfiguration GetServerConfiguration()
        {
            return DicomServer.GetConfiguration();
        }

        protected Study LoadRelatedStudy()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();           
                    
                if (!string.IsNullOrEmpty(Proxy.Item.StudyInstanceUid))
                    return broker.GetStudy(Proxy.Item.StudyInstanceUid);

                return null;
            }
        }

        #endregion
    }

    public class NotEnoughStorageException : Exception
    {
        public NotEnoughStorageException():base("Additional storage space is required")
        {
        }
    }
}
