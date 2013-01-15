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
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="StudyIntegrityQueue"/>
    /// </summary>
    public class InconsistentDataSIQEntry : StudyIntegrityQueue
    {

        public InconsistentDataSIQEntry()
        {
            
        }

        public InconsistentDataSIQEntry(StudyIntegrityQueue studyIntegrityQueueEntry)
        {
            Platform.CheckTrue(studyIntegrityQueueEntry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.InconsistentData,
                               String.Format("Cannot copy data from StudyIntegrityQueue record of type {0}",
                                             studyIntegrityQueueEntry.StudyIntegrityReasonEnum));

            this.SetKey(studyIntegrityQueueEntry.Key);
            this.Description = studyIntegrityQueueEntry.Description;
            this.InsertTime = studyIntegrityQueueEntry.InsertTime;
            this.Details = studyIntegrityQueueEntry.Details;
            this.ServerPartitionKey = studyIntegrityQueueEntry.ServerPartitionKey;
            this.StudyData = studyIntegrityQueueEntry.StudyData;
            this.StudyIntegrityReasonEnum = studyIntegrityQueueEntry.StudyIntegrityReasonEnum;
            this.StudyStorageKey = studyIntegrityQueueEntry.StudyStorageKey;
            this.GroupID = studyIntegrityQueueEntry.GroupID;
            
        }


        public new static InconsistentDataSIQEntry Load(IPersistenceContext context, ServerEntityKey key)
        {
            return new InconsistentDataSIQEntry(StudyIntegrityQueue.Load(context, key));
        }

        public string GetFolderPath()
        {
            Platform.CheckForNullReference(Details, "Details");
            // TODO: We should use ReconcileStudyWorkQueueData instead here. But that is impossible 
            // because of the Model<--> COmmon dependency.
 
            XmlNode xmlStoragePath = this.Details.SelectSingleNode("//StoragePath");
            Platform.CheckForNullReference(xmlStoragePath, "xmlStoragePath");
            // TODO: end

            String storagePath = xmlStoragePath.InnerText;
            return storagePath;
        }

        public string GetSopPath(string seriesUid, string instanceUid)
        {
            string path = Path.Combine(GetFolderPath(), instanceUid);
            path += "." + "dcm";
            return path;
        }
    }
}