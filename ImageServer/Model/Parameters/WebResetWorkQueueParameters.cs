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
    public class WebResetWorkQueueParameters : ProcedureParameters
    {
        public WebResetWorkQueueParameters()
            : base("WebResetWorkQueueParameters")
        {
        }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public DateTime? NewScheduledTime
        {
            set { SubCriteria["NewScheduledTime"] = new ProcedureParameter<DateTime?>("NewScheduledTime", value); }
        }

        public DateTime? NewExpirationTime
        {
            set { SubCriteria["NewExpirationTime"] = new ProcedureParameter<DateTime?>("NewExpirationTime", value); }
        }

        public WorkQueuePriorityEnum NewPriority
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<ServerEnum>("NewPriority", value); }
        }

    }
}