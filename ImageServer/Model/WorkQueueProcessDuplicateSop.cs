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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="WorkQueue"/> for handling duplicates.
    /// </summary>
    public class WorkQueueProcessDuplicateSop : WorkQueue
    {

        #region Static Members
        static readonly XmlSerializer _serializer = new XmlSerializer(typeof(ProcessDuplicateQueueEntryQueueData));
        #endregion

        #region Private Members
        private ProcessDuplicateQueueEntryQueueData _queueData;
        #endregion

        #region Constructors
        public WorkQueueProcessDuplicateSop()
        {

        }

        public WorkQueueProcessDuplicateSop(WorkQueue workQueue)
        {
            SetKey(workQueue.GetKey());
            Data = workQueue.Data;
            ExpirationTime = workQueue.ExpirationTime;
            FailureCount = workQueue.FailureCount;
            FailureDescription = workQueue.FailureDescription;
            InsertTime = workQueue.InsertTime;
            ProcessorID = workQueue.ProcessorID;
            ScheduledTime = workQueue.ScheduledTime;
            ServerPartitionKey = workQueue.ServerPartitionKey;
            StudyHistoryKey = workQueue.StudyHistoryKey;
            StudyStorageKey = workQueue.StudyStorageKey;
            WorkQueuePriorityEnum = workQueue.WorkQueuePriorityEnum;
            WorkQueueStatusEnum = workQueue.WorkQueueStatusEnum;
            WorkQueueTypeEnum = workQueue.WorkQueueTypeEnum;

            _queueData = (ProcessDuplicateQueueEntryQueueData)_serializer.Deserialize(new XmlNodeReader(workQueue.Data.DocumentElement));

        }
        #endregion

        #region Public Properties

        public ProcessDuplicateQueueEntryQueueData QueueData
        {
            get { return _queueData; }
            set
            {
                _queueData = value;

                StringWriter sw = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(sw);
                _serializer.Serialize(xmlTextWriter, _queueData);

                Data = new XmlDocument();
                Data.LoadXml(sw.ToString());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// NOTE: This location is not updated if the filesystem path is changed.
        /// </summary>
        /// <returns></returns>
        public string GetDuplicateSopFolder()
        {
            return QueueData.DuplicateSopFolder;
        }
        #endregion
    }
}