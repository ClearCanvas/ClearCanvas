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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Core.Helpers
{
    public static class WorkQueueHelper
    {
        /// <summary>
        /// Checks for the existinance of a SOP for a given Study in the <see cref="WorkQueue"/> for a <see cref="WorkQueueTypeEnum.ReconcileStudy"/>.
        /// </summary>
        /// <param name="studyStorageKey">The StudyStorage primary key</param>
        /// <param name="seriesInstanceUid">The Series Instance Uid of the Sop</param>
        /// <param name="sopInstanceUid">The Sop Instance to look for</param>
        /// <returns>true if an entry exists, false if it doesn't</returns>
        static public bool WorkQueueUidExists(ServerEntityKey studyStorageKey, string seriesInstanceUid, string sopInstanceUid)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            using (var scope = new ServerExecutionContext())
            {
                var broker = scope.PersistenceContext.GetBroker<IWorkQueueEntityBroker>();
                var uidSelectCriteria = new WorkQueueUidSelectCriteria();
                uidSelectCriteria.SeriesInstanceUid.EqualTo(seriesInstanceUid);
                uidSelectCriteria.SopInstanceUid.EqualTo(sopInstanceUid);
                var selectCriteria = new WorkQueueSelectCriteria();
                selectCriteria.StudyStorageKey.EqualTo(studyStorageKey);
                selectCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.ReconcileStudy);
                selectCriteria.WorkQueueUidRelatedEntityCondition.Exists(uidSelectCriteria);

                return broker.Count(selectCriteria) > 0;
            }
        }

        /// <summary>
        /// Finds a list of <see cref="WorkQueue"/> related to the specified <see cref="studyStorageKey"/>.
        /// </summary>
        /// <param name="studyStorageKey"></param>
        /// <param name="filter">A delegate that will be used to filter the returned list. Pass in Null to get the entire list.</param>
        /// <returns>A list of  <see cref="WorkQueue"/></returns>
        static public IList<WorkQueue> FindWorkQueueEntries(ServerEntityKey studyStorageKey, Predicate<WorkQueue> filter)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            using (var scope = new ServerExecutionContext())
            {
                var broker = scope.PersistenceContext.GetBroker<IWorkQueueEntityBroker>();
                var criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorageKey);
                criteria.InsertTime.SortDesc(0);
                IList<WorkQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether the entry is still "active"
        /// </summary>
        /// <remarks>
        /// </remarks>
        static public bool IsActiveWorkQueue(WorkQueue item)
        {
            // The following code assumes InactiveWorkQueueMinTime is set appropirately

            if (item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed))
                return false;

            if (item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Pending) ||
                item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Idle))
            {
                // Assuming that if the entry is picked up and rescheduled recently (the ScheduledTime would have been updated), 
                // the item is inactive if its ScheduledTime still indicated it was scheduled long time ago.
                // Note: this logic still works if the entry has never been processed (new). It will be
                // considered as "inactive" if it was scheduled long time ago and had never been updated.

                DateTime lastActiveTime = item.LastUpdatedTime.GetValueOrDefault(item.ScheduledTime);
	            return (Platform.Time - lastActiveTime < Settings.Default.InactiveWorkQueueMinTime);
            }
            else if (item.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.InProgress))
            {
                if (String.IsNullOrEmpty(item.ProcessorID))
                {
                    // This is a special case, the item is not assigned but is set to InProgress. 
                    // It's definitely stuck cause it won't be picked up by any servers.
                    return false;
                }
                // TODO: Need more elaborate logic to detect if it's stuck when the status is InProgress.
                // Ideally, we can assume item is stuck if it has not been updated for a while. 
                // Howerver, some operations were designed to process everything in a single run 
                // instead of batches.One example is the StudyProcess, research studies may take days to process 
                // and the item stays in "InProgress" for the entire period without any update 
                // (eventhough the WorkQueueUid records are removed)
                // For now, we assume it's stucked if it is not updated for long time.
                if (item.ScheduledTime < Platform.Time - Settings.Default.InactiveWorkQueueMinTime)
                    return false;
            }

            return true;
        }
    }
}
