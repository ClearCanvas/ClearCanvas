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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.ExternalRequest;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Edit
{
    public class StudyDeleteHelper
    {
        /// <summary>
        /// Lock a specific study for deletion.
        /// </summary>
        /// <param name="studyStorageKey"></param>
        /// <param name="failureReason">An english reason as to why the lock failed.</param>
        /// <returns>true if the study has been locked, falst if not.</returns>
        public static bool LockStudyForDeletion(ServerEntityKey studyStorageKey, out string failureReason)
        {
            return ServerHelper.LockStudy(studyStorageKey, QueueStudyStateEnum.WebDeleteScheduled, out failureReason);
        }

        /// <summary>
        /// Unlock a study for deletion.
        /// </summary>
        /// <param name="studyStorageKey"></param>
        /// <returns></returns>
        public static bool ReleaseDeletionLock(ServerEntityKey studyStorageKey)
        {
            return ServerHelper.UnlockStudy(studyStorageKey);
        }

        /// <summary>
        /// Inserts delete request(s) to delete a series in a study.
        /// </summary>
        /// <param name="context">The persistence context used for database connection.</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study resides</param>
        /// <param name="studyInstanceUid">The Study Instance Uid of the study</param>
        /// <param name="reason">The reason for deleting the series.</param>
        /// <returns>A list of DeleteSeries <see cref="WorkQueue"/> entries inserted into the system.</returns>
        /// <exception cref="InvalidStudyStateOperationException"></exception>
        public static WorkQueue DeleteStudy(IUpdateContext context, ServerPartition partition, string studyInstanceUid,
                                             string reason)
        {
			StudyStorageLocation location = FindStudyStorageLocation(context, partition, studyInstanceUid);

            string failureReason;

            try
            {
                if (LockStudyForDeletion(location.Key, out failureReason))
                {
                    WorkQueue deleteRequest = InsertDeleteStudyRequest(context, location, reason);
                        if (deleteRequest == null)
                            throw new ApplicationException(
                                String.Format("Unable to insert a Delete Study request for study {0}",
                                              location.StudyInstanceUid));
                    

                    return deleteRequest;
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Errors occurred when trying to insert study delete request");

                if (!ReleaseDeletionLock(location.Key))
                    Platform.Log(LogLevel.Error, "Unable to unlock the study: " + location.StudyInstanceUid);

                throw;
            }

            throw new ApplicationException(
                String.Format("Unable to lock storage location {0} for deletion : {1}", location.Key, failureReason));
        }

	    
	    /// <summary>
        /// Inserts delete request(s) to delete a series in a study.
        /// </summary>
        /// <param name="context">The persistence context used for database connection.</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study resides</param>
        /// <param name="studyInstanceUid">The Study Instance Uid of the study</param>
        /// <param name="seriesInstanceUids">The Series Instance Uid of the series to be deleted.</param>
        /// <param name="reason">The reason for deleting the series.</param>
        /// <returns>A list of DeleteSeries <see cref="WorkQueue"/> entries inserted into the system.</returns>
        /// <exception cref="InvalidStudyStateOperationException"></exception>
        public static WorkQueue DeleteSeries(IUpdateContext context, ServerPartition partition, string studyInstanceUid,
                                             List<string> seriesInstanceUids, string reason)
        {
			StudyStorageLocation location = FindStudyStorageLocation(context, partition, studyInstanceUid);

            string failureReason;
            
            try
            {
                if (LockStudyForDeletion(location.Key, out failureReason))
                {
                    WorkQueue deleteRequest = null;

                    // insert a delete series request
                    foreach (var uid in seriesInstanceUids)
                    {
                        deleteRequest = InsertDeleteSeriesRequest(context, location, uid, reason);
                        if (deleteRequest == null)
                                throw new ApplicationException(
                                    String.Format("Unable to insert a Delete Series request for series {0} in study {1}",
                                                  uid, location.StudyInstanceUid));
                    }

                    return deleteRequest;
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Errors occurred when trying to insert series delete request");
                
                if (!ReleaseDeletionLock(location.Key))
                    Platform.Log(LogLevel.Error, "Unable to unlock the study: " + location.StudyInstanceUid);

                throw;
            }

            throw new ApplicationException(
                String.Format("Unable to lock storage location {0} for deletion : {1}", location.Key, failureReason));
        }

        /// <summary>
        /// Inserts delete request(s) to delete a sop instance in a study.
        /// </summary>
        /// <param name="context">The persistence context used for database connection.</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study resides</param>
        /// <param name="studyInstanceUid">The Study Instance Uid of the study</param>
        /// <param name="seriesInstanceUid">The Series Instance Uid of the study</param>
        /// <param name="sopInstanceUids">The SOP Instance Uid of the instances to be deleted.</param>
        /// <param name="reason">The reason for deleting the sops.</param>
        /// <returns>A list of DeleteStudy <see cref="WorkQueue"/> entries inserted into the system.</returns>
        /// <exception cref="InvalidStudyStateOperationException"></exception>
        /// <exception cref="ExternalRequestDelayProcessingException"></exception>
        public static WorkQueue DeleteInstances(IUpdateContext context, ServerPartition partition,
                                                string studyInstanceUid, string seriesInstanceUid,
                                                List<string> sopInstanceUids, string reason)
        {
            StudyStorageLocation location;

            FilesystemMonitor.Instance.GetWritableStudyStorageLocation(partition.Key, studyInstanceUid,
                                                                       StudyRestore.False, StudyCache.True, out location);
            string failureReason;

            try
            {
                if (LockStudyForDeletion(location.Key, out failureReason))
                {
                    WorkQueue deleteItem = null;

                    // insert a delete series request
                    foreach (string uid in sopInstanceUids)
                    {

                        deleteItem =  InsertDeleteInstanceRequest(context, location, seriesInstanceUid, uid,
                                                            reason);
                        if (deleteItem == null)
                            throw new ApplicationException(
                                String.Format("Unable to insert a Delete Series request for SOP Instance {0} in study {1}",
                                              uid, location.StudyInstanceUid));
                    }

                    return deleteItem;
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Errors occurred when trying to insert delete instance request");

                if (!ReleaseDeletionLock(location.Key))
                    Platform.Log(LogLevel.Error, "Unable to unlock the study: " + location.StudyInstanceUid);

                throw;
            }

            throw new ApplicationException(String.Format("Unable to lock storage location {0} for deletion of sop instances: {1}",
                                                         location.Key, failureReason));
        }


        /// <summary>
        /// Inserts a WebDeleteStudy work queue entry that deletes specific series within the study.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="reason"></param>
        /// <param name="externalRequest"></param>
        /// <exception cref="ApplicationException">If the "DeleteSeries" Work Queue entry cannot be inserted.</exception>
        public static WorkQueue InsertDeleteSeriesRequest(IUpdateContext context, StudyStorageLocation location,
                                                          string seriesInstanceUid, string reason,
                                                          ExternalRequestQueue externalRequest = null)
        {
            // Create a work queue entry and append the series instance uid into the WorkQueueUid table

            var broker = context.GetBroker<IInsertWorkQueue>();
            DateTime now = Platform.Time;

            var criteria = new InsertWorkQueueParameters
                {
                    WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy,
                    StudyStorageKey = location.Key,
                    ServerPartitionKey = location.ServerPartitionKey,
                    ScheduledTime = now,
                    SeriesInstanceUid = seriesInstanceUid,
                    WorkQueueData = XmlUtils.SerializeAsXmlDoc(
                        new WebDeleteSeriesLevelQueueData
                            {
                                Reason = reason,
                                Timestamp = now,
                                UserId = ServerHelper.CurrentUserId,
                                UserName = ServerHelper.CurrentUserName
                            })
                };

            if (externalRequest != null)
                criteria.ExternalRequestQueueKey = externalRequest.Key;

            return broker.FindOne(criteria);
        }

        /// <summary>
        /// Inserts a WebDeleteStudy work queue entry that deletes specific SOP Instances within the study.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="sopInstanceUid"></param>
        /// <param name="reason"></param>
        /// <param name="externalRequest"></param>
        public static WorkQueue InsertDeleteInstanceRequest(IUpdateContext context, StudyStorageLocation location,
                                                            string seriesInstanceUid, string sopInstanceUid,
                                                            string reason, ExternalRequestQueue externalRequest = null)
        {
            var broker = context.GetBroker<IInsertWorkQueue>();

            DateTime now = Platform.Time;

            var criteria = new InsertWorkQueueParameters
                {
                    WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy,
                    StudyStorageKey = location.Key,
                    ServerPartitionKey = location.ServerPartitionKey,
                    ScheduledTime = now,
                    SeriesInstanceUid = seriesInstanceUid,
                    SopInstanceUid = sopInstanceUid,
                    WorkQueueData = XmlUtils.SerializeAsXmlDoc(
                        new WebDeleteInstanceLevelQueueData
                            {
                                Reason = reason,
                                Timestamp = now,
                                UserId = ServerHelper.CurrentUserId,
                                UserName = ServerHelper.CurrentUserName
                            }),
                };

            if (externalRequest != null)
                criteria.ExternalRequestQueueKey = externalRequest.Key;

            return broker.FindOne(criteria);
        }

        /// <summary>
        /// Inserts a WebDeleteStudy request for a specific Study
        /// </summary>
        /// <param name="updateContext">The database context to insert under.</param>
        /// <param name="location">The StudyStorageLocation of the study to delete.</param>
        /// <param name="requestDescription">A description of the reason for the delete.</param>
        /// <param name="externalRequest">An optional external request that triggered this deletion</param>
        /// <returns>The inserted WorkQueue entry</returns>
        public static WorkQueue InsertDeleteStudyRequest(IUpdateContext updateContext, StudyStorageLocation location, string requestDescription, ExternalRequestQueue externalRequest = null)
        {
            var extendedData = new WebDeleteStudyLevelQueueData
            {
                Level = DeletionLevel.Study,
                Reason = requestDescription,
                UserId = ServerHelper.CurrentUserId,
                UserName = ServerHelper.CurrentUserName
            };

            var insertParms = new InsertWorkQueueParameters
                {
                    WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy,
                    ServerPartitionKey = location.ServerPartitionKey,
                    StudyStorageKey = location.Key,
                    ScheduledTime = Platform.Time,
                    WorkQueueData = XmlUtils.SerializeAsXmlDoc(extendedData)
                };

            if (externalRequest != null)
                insertParms.ExternalRequestQueueKey = externalRequest.Key;
   
            var insertWorkQueue = updateContext.GetBroker<IInsertWorkQueue>();

            return insertWorkQueue.FindOne(insertParms);
        }

		private static StudyStorageLocation FindStudyStorageLocation(IUpdateContext context, ServerPartition partition, string studyInstanceUid)
		{
			var procedure = context.GetBroker<IQueryStudyStorageLocation>();
			var parms = new StudyStorageLocationQueryParameters
			{
				ServerPartitionKey = partition.GetKey(),
				StudyInstanceUid = studyInstanceUid
			};
			return procedure.FindOne(parms);
		}

    } 
}
