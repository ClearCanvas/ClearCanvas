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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Assembles an instance of  <see cref="WorkQueueDetails"/> based on a <see cref="WorkQueue"/> object.
    /// </summary>
    public static class WorkQueueDetailsAssembler
    {
        /// <summary>
        /// Creates an instance of <see cref="WorkQueueDetails"/> base on a <see cref="WorkQueue"/> object.
        /// </summary>
        /// <param name="workqueue"></param>
        /// <returns></returns>
        public static WorkQueueDetails CreateWorkQueueDetail(Model.WorkQueue workqueue)
        {
            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
            {
                return CreateAutoRouteWorkQueueItemDetails(workqueue);
            }
            
            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
            {
                return CreateWebMoveStudyWorkQueueItemDetails(workqueue);
            }

            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.ProcessDuplicate)
            {
                return CreateProcessDuplicateWorkQueueItemDetails(workqueue);
            }

            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy
             || workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.ExternalEdit)
                return CreateEditWorkQueueItemDetails(workqueue);

            return CreateGeneralWorkQueueItemDetails(workqueue);            
        }

        #region Private Static Methods

        private static WorkQueueDetails CreateGeneralWorkQueueItemDetails(Model.WorkQueue item)
        {
            var detail = new WorkQueueDetails();

            detail.Key = item.Key;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.FailureDescription = item.FailureDescription;
            detail.ServerDescription = item.ProcessorID;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            var ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);

            var studyAdaptor = new StudyAdaptor();
            var studycriteria = new StudySelectCriteria();
            studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
            studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
            Study study = studyAdaptor.GetFirst(studycriteria);

            // Study may not be available until the images are processed.
            if (study != null)
            {
                var studyAssembler = new StudyDetailsAssembler();
                detail.Study = studyAssembler.CreateStudyDetail(study);
            }
            return detail;
        }

        private static WorkQueueDetails CreateProcessDuplicateWorkQueueItemDetails(Model.WorkQueue item)
        {
            var detail = new WorkQueueDetails();

            detail.Key = item.Key;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.FailureDescription = item.FailureDescription;
            detail.ServerDescription = item.ProcessorID;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            XmlDocument doc = item.Data;
            XmlNodeList nodeList = doc.GetElementsByTagName("DuplicateSopFolder");
            detail.DuplicateStorageLocationPath = nodeList[0].InnerText;

            // Fetch UIDs
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            var ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);

            var studyAdaptor = new StudyAdaptor();
            var studycriteria = new StudySelectCriteria();
            studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
            studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
            Study study = studyAdaptor.GetFirst(studycriteria);

            // Study may not be available until the images are processed.
            if (study != null)
            {
                var studyAssembler = new StudyDetailsAssembler();
                detail.Study = studyAssembler.CreateStudyDetail(study);
            }
            return detail;
        }

        private static WorkQueueDetails CreateAutoRouteWorkQueueItemDetails(Model.WorkQueue item)
        {
            var studyStorageAdaptor = new StudyStorageAdaptor();
            var deviceAdaptor = new DeviceDataAdapter();
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var studyAdaptor = new StudyAdaptor();

            StudyStorage studyStorage = studyStorageAdaptor.Get(item.StudyStorageKey);

            var detail = new AutoRouteWorkQueueDetails();
            detail.Key = item.GetKey();
            detail.StudyInstanceUid = studyStorage == null ? string.Empty : studyStorage.StudyInstanceUid;

            detail.DestinationAE = deviceAdaptor.Get(item.DeviceKey).AeTitle;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.ServerDescription = item.ProcessorID;
            detail.FailureDescription = item.FailureDescription;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            if (studyStorage != null)
            {
                var studycriteria = new StudySelectCriteria();
                studycriteria.StudyInstanceUid.EqualTo(studyStorage.StudyInstanceUid);
                studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
                Study study = studyAdaptor.GetFirst(studycriteria);

                // Study may not be available until the images are processed.
                if (study != null)
                {
                    var studyAssembler = new StudyDetailsAssembler();
                    detail.Study = studyAssembler.CreateStudyDetail(study);
                }
            }

            return detail;
        }

        private static WorkQueueDetails CreateWebMoveStudyWorkQueueItemDetails(Model.WorkQueue item)
        {
            var deviceAdaptor = new DeviceDataAdapter();
            var studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorage studyStorage = studyStorageAdaptor.Get(item.StudyStorageKey);
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var studyAdaptor = new StudyAdaptor();
            Device dest = deviceAdaptor.Get(item.DeviceKey);

            var detail = new WebMoveStudyWorkQueueDetails();
            detail.Key = item.GetKey();

            detail.DestinationAE = dest == null ? string.Empty : dest.AeTitle;
            detail.StudyInstanceUid = studyStorage == null ? string.Empty : studyStorage.StudyInstanceUid;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.ServerDescription = item.ProcessorID;
            detail.FailureDescription = item.FailureDescription;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            if (studyStorage != null)
            {
                var studycriteria = new StudySelectCriteria();
                studycriteria.StudyInstanceUid.EqualTo(studyStorage.StudyInstanceUid);
                studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
                Study study = studyAdaptor.GetFirst(studycriteria);

                // Study may not be available until the images are processed.
                if (study != null)
                {
                    var studyAssembler = new StudyDetailsAssembler();
                    detail.Study = studyAssembler.CreateStudyDetail(study);
                }
            }

            return detail;
        }

        private static WorkQueueDetails CreateEditWorkQueueItemDetails(Model.WorkQueue item)
        {
            string studyPath;
            try
            {
                StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
                studyPath = storage.GetStudyPath();
            }
            catch(Exception)
            {
                studyPath = string.Empty;
            }
            var detail = new WorkQueueDetails
                             {
                                 Key = item.Key,
                                 ScheduledDateTime = item.ScheduledTime,
                                 ExpirationTime = item.ExpirationTime,
                                 InsertTime = item.InsertTime,
                                 FailureCount = item.FailureCount,
                                 Type = item.WorkQueueTypeEnum,
                                 Status = item.WorkQueueStatusEnum,
                                 Priority = item.WorkQueuePriorityEnum,
                                 FailureDescription = item.FailureDescription,
                                 ServerDescription = item.ProcessorID,
                                 StorageLocationPath = studyPath
                             };



            // Fetch UIDs
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            var ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);

            var studyAdaptor = new StudyAdaptor();
            var studycriteria = new StudySelectCriteria();
            studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
            studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
            Study study = studyAdaptor.GetFirst(studycriteria);

            // Study may not be available until the images are processed.
            if (study != null)
            {
                var studyAssembler = new StudyDetailsAssembler();
                detail.Study = studyAssembler.CreateStudyDetail(study);
            }

            var parser = new EditStudyWorkQueueDataParser();
            EditStudyWorkQueueData data = parser.Parse(item.Data.DocumentElement);

            detail.EditUpdateItems = data.EditRequest.UpdateEntries.ToArray();

            return detail;
        }
        #endregion Private Static Methods
    }
}