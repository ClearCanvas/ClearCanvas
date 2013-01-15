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
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex
{
    internal class ReindexItemProcessor : BaseItemProcessor<ReindexRequest, ReindexProgress>
    {
        #region Private Members

        private ReindexUtility _reindexUtility;

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override bool Initialize(WorkItemStatusProxy proxy)
        {
            bool initResult = base.Initialize(proxy);
                       
            return initResult;
        }

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the ReindexUtility's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            base.Cancel();

            if (_reindexUtility != null)
                _reindexUtility.Cancel();            
        }

        /// <summary>
        /// Override of Stop() routine.
        /// </summary>
        /// <remarks>
        /// The Stop must be override to call the ReindexUtility's Cancel routine.
        /// </remarks>
        public override void Stop()
        {
            if (_reindexUtility != null)
                _reindexUtility.Cancel();

            base.Stop();
        }

        public override void Process()
        {
            if (CancelPending)
            {
                Proxy.Cancel();
                return;
            }
            if (StopPending)
            {
                Proxy.Postpone();
                return;
            }

            Progress.IsCancelable = true;
            Progress.Complete = false;
            Progress.StudiesToProcess = 0;
            Progress.TotalStudyFolders = 0;
            Progress.StudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;

            Proxy.UpdateProgress();

            _reindexUtility = new ReindexUtility();

            _reindexUtility.Initialize();

            try
            {
                WorkItemPublishSubscribeHelper.PublishStudiesCleared();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItem StudiesCleared status");
            }

            // Reset progress, in case of retry
            Progress.StudiesToProcess = _reindexUtility.DatabaseStudiesToScan;
            Progress.TotalStudyFolders = _reindexUtility.StudyFoldersToScan;
            Progress.StudiesDeleted = 0;
            Progress.StudyFoldersProcessed = 0;
            Progress.StudiesProcessed = 0;
            Progress.StudiesFailed = 0;
            Progress.Complete = false;

            Proxy.UpdateProgress();

            _reindexUtility.StudyFolderProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                             {
                                                                 Progress.StudyFoldersProcessed++;
                                                                 Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                                 Proxy.UpdateProgress();
                                                             };

            _reindexUtility.StudyDeletedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                     {
                                                         Progress.StudiesDeleted++;
                                                         Progress.StudiesProcessed++;
                                                         Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                         Proxy.UpdateProgress();
                                                     };

            _reindexUtility.StudyProcessedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                       {
                                                           Progress.StudiesProcessed++;
                                                           Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                           Proxy.UpdateProgress();
                                                       };

            _reindexUtility.StudiesRestoredEvent += delegate(object sender, ReindexUtility.StudiesEventArgs e)
                                                      {
                                                          foreach (var studyInstanceUid in e.StudyInstanceUids)
                                                          {
                                                              Proxy.Item.StudyInstanceUid = studyInstanceUid;
                                                              Proxy.UpdateProgress();
                                                          }
                                                      };

            _reindexUtility.StudyReindexFailedEvent += delegate(object sender, ReindexUtility.StudyEventArgs e)
                                                           {
                                                               Progress.StudiesFailed++;
                                                               Proxy.Item.StudyInstanceUid = e.StudyInstanceUid;
                                                               if (!string.IsNullOrEmpty(e.Message))
                                                                   Progress.StatusDetails = e.Message;
                                                               Proxy.UpdateProgress();
                                                           };
            
            _reindexUtility.Process();

            if (StopPending)
            {
                Progress.Complete = true;
                Proxy.Postpone();
            }
            else if (CancelPending)
            {
                Progress.Complete = true;
                Proxy.Cancel();
            }
            else
            {
                Progress.Complete = true;
                if (Progress.StudiesFailed > 0)
                    Proxy.Fail(Progress.StatusDetails,WorkItemFailureType.Fatal);                
                else
                    Proxy.Complete();
            }           
        }

        #endregion

    }
}
