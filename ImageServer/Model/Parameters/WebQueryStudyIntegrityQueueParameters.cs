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
	public class WebQueryStudyIntegrityQueueParameters : ProcedureParameters
    {
		public WebQueryStudyIntegrityQueueParameters()
            : base("WebQueryStudyIntegrityQueue")
        {
			//Declare output parameters here
			SubCriteria["ResultCount"] = new ProcedureParameter<int>("ResultCount");
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

		public string Description
		{
			set { SubCriteria["Description"] = new ProcedureParameter<string>("Description", value); }
		}

        public string StudyData
        {
			set { SubCriteria["StudyData"] = new ProcedureParameter<string>("StudyData", value); }
        }

        public DateTime? InsertTime
        {
            set { SubCriteria["InsertTime"] = new ProcedureParameter<DateTime?>("InsertTime", value); }
        }

		public StudyIntegrityReasonEnum StudyIntegrityReasonEnum
        {
			set { SubCriteria["StudyIntegrityReasonEnum"] = new ProcedureParameter<ServerEnum>("StudyIntegrityReasonEnum", value); }
        }

		public int StartIndex
        {
			set { SubCriteria["StartIndex"] = new ProcedureParameter<int>("StartIndex", value); }
	    }

		public int MaxRowCount
		{
			set { SubCriteria["MaxRowCount"] = new ProcedureParameter<int>("MaxRowCount", value); }
		}

		public int ResultCount
		{
			get { return (SubCriteria["ResultCount"] as ProcedureParameter<int>).Value; }
		}
	}
}
