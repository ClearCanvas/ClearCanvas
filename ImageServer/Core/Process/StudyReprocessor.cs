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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.Web.Enterprise.Authentication;
using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Core.Process
{
    public class StudyReprocessor
    {
        /// <summary>
        /// Creates a Study Reprocess entry and locks the study in <see cref="QueueStudyStateEnum.ReprocessScheduled"/> state.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="location"></param>
        /// <param name="scheduleTime"></param>
        /// <returns></returns>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        public WorkQueue ReprocessStudy(String reason, StudyStorageLocation location, DateTime scheduleTime)
        {
        	Platform.CheckForNullReference(location, "location");

        	IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

        	using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
        	{
				WorkQueue reprocessEntry = ReprocessStudy(ctx, reason, location, scheduleTime);
        		if (reprocessEntry != null)
        		{
        			ctx.Commit();
        		}

        		return reprocessEntry;
        	}
        }

        public WorkQueue ReprocessStudy(IUpdateContext ctx, String reason, StudyStorageLocation location, DateTime scheduleTime)
        {
            return ReprocessStudy(ctx, reason, location, null, Platform.Time);
        }

        /// <summary>
        /// Inserts a <see cref="WorkQueue"/> request to reprocess the study
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="reason"></param>
        /// <param name="location"></param>
        /// <param name="additionalPaths"></param>
        /// <param name="scheduleTime"></param>
        /// <returns></returns>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        /// 
        public WorkQueue ReprocessStudy(IUpdateContext ctx, String reason, StudyStorageLocation location, List<FilesystemDynamicPath> additionalPaths, DateTime scheduleTime)
		{
			Platform.CheckForNullReference(location, "location");

            if (location.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
            {
                if (location.IsLatestArchiveLossless)
                {
                    string message = String.Format("Study has been archived as lossless and is currently lossy. It must be restored first");
                    throw new InvalidStudyStateOperationException(message);
                }
            }

			Study study = location.LoadStudy(ctx);
            
			// Unlock first. 
			ILockStudy lockStudy = ctx.GetBroker<ILockStudy>();
			LockStudyParameters lockParms = new LockStudyParameters();
			lockParms.StudyStorageKey = location.Key;
			lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
			if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
			{
                // Note: according to the stored proc, setting study state to Idle always succeeds so
                // this will never happen
			    return null;
			}

            // Now relock into ReprocessScheduled state. If another process locks the study before this occurs,
            // 
			lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ReprocessScheduled;
			if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
			{
			    throw new InvalidStudyStateOperationException(lockParms.FailureReason);
			}

			InsertWorkQueueParameters columns = new InsertWorkQueueParameters();
			columns.ScheduledTime = scheduleTime;
			columns.ServerPartitionKey = location.ServerPartitionKey;
			columns.StudyStorageKey = location.Key;
			columns.WorkQueueTypeEnum = WorkQueueTypeEnum.ReprocessStudy;

			ReprocessStudyQueueData queueData = new ReprocessStudyQueueData();
			queueData.State = new ReprocessStudyState();
			queueData.State.ExecuteAtLeastOnce = false;
			queueData.ChangeLog = new ReprocessStudyChangeLog();
			queueData.ChangeLog.Reason = reason;
			queueData.ChangeLog.TimeStamp = Platform.Time;
			queueData.ChangeLog.User = (Thread.CurrentPrincipal is CustomPrincipal)
			                           	? (Thread.CurrentPrincipal as CustomPrincipal).Identity.Name
			                           	: String.Empty;

            if (additionalPaths != null)
                queueData.AdditionalFiles = additionalPaths.ConvertAll<string>(path => path.ToString());
            

			columns.WorkQueueData = XmlUtils.SerializeAsXmlDoc(queueData);
			IInsertWorkQueue insertBroker = ctx.GetBroker<IInsertWorkQueue>();
			WorkQueue reprocessEntry = insertBroker.FindOne(columns);
			if (reprocessEntry != null)
			{
				if (study != null)
				{
					Platform.Log(LogLevel.Info,
					             "Study Reprocess Scheduled for Study {0}, A#: {1}, Patient: {2}, ID={3}",
					             study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId);
				}
				else
				{
					Platform.Log(LogLevel.Info, "Study Reprocess Scheduled for Study {0}.", location.StudyInstanceUid);
				}
			}

			return reprocessEntry;
		}
    }
}
