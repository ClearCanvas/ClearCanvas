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
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateWorkQueueParameters : ProcedureParameters
    {
        public UpdateWorkQueueParameters()
            : base("UpdateWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public WorkQueueStatusEnum WorkQueueStatusEnum
        {
            set { SubCriteria["WorkQueueStatusEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueStatusEnum", value); }
        }

        public DateTime? ExpirationTime
        {
            set { SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime?>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public int FailureCount
        {
            set { SubCriteria["FailureCount"] = new ProcedureParameter<int>("FailureCount", value); }
        }

        public string ProcessorID
        {
            set { SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

        public string FailureDescription
        {
            set { SubCriteria["FailureDescription"] = new ProcedureParameter<string>("FailureDescription", value); }
        }

		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}
    }
}
