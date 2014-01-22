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
using System.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertWorkQueueParameters : ProcedureParameters
    {
        public InsertWorkQueueParameters()
            : base("InsertWorkQueue")
        { }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
		public WorkQueueTypeEnum WorkQueueTypeEnum
		{
			set { SubCriteria["WorkQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueTypeEnum", value); }
		}
        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }
		public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public string SopInstanceUid
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<string>("SopInstanceUid", value); }
        }

        public bool Duplicate
        {
            set { SubCriteria["Duplicate"] = new ProcedureParameter<bool>("Duplicate", value); }
            
        }
        public string Extension
        {
            set { SubCriteria["Extension"] = new ProcedureParameter<string>("Extension", value); }           
        }
		public ServerEntityKey StudyHistoryKey
		{
			set { SubCriteria["StudyHistoryKey"] = new ProcedureParameter<ServerEntityKey>("StudyHistoryKey", value); }
		}
		public ServerEntityKey DeviceKey
		{
			set { SubCriteria["DeviceKey"] = new ProcedureParameter<ServerEntityKey>("DeviceKey", value); }
		}
		public XmlDocument WorkQueueData
		{
			set
			{
				SubCriteria["Data"] = new ProcedureParameter<XmlDocument>("Data", value);
			}
		}

        public string WorkQueueGroupID
        {
            set { SubCriteria["WorkQueueGroupID"] = new ProcedureParameter<string>("WorkQueueGroupID", value); }
        }

        public string UidRelativePath
        {
            set { SubCriteria["UidRelativePath"] = new ProcedureParameter<string>("UidRelativePath", value); }
        }

        // TODO (Rigel) - figure out how to do the DB upgrade to remove this and UidRelativePath and store it in WorkQueueUidData
        public string UidGroupID
        {
            set { SubCriteria["UidGroupID"] = new ProcedureParameter<string>("UidGroupID", value); }
        }
        public ServerEntityKey ExternalRequestQueueKey
        {
            set { SubCriteria["ExternalRequestQueueKey"] = new ProcedureParameter<ServerEntityKey>("ExternalRequestQueueKey", value); }
        }
        public WorkQueueUidData WorkQueueUidData
        {
            set
            {
                ;
                SubCriteria["WorkQueueUidData"] = new ProcedureParameter<XmlDocument>("WorkQueueUidData", ImageServerSerializer.SerializeWorkQueueUidDataToXmlDocument(value));
    }
}
    }
}
