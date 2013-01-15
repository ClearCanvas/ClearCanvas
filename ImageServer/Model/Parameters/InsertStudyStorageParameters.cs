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
    public class InsertStudyStorageParameters : ProcedureParameters
    {
        public InsertStudyStorageParameters()
            : base("InsertStudyStorage")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public String StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
        public ServerEntityKey FilesystemKey
        {
            set { SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }
        public String Folder
        {
            set { SubCriteria["Folder"] = new ProcedureParameter<String>("Folder", value); }
        }
		public String TransferSyntaxUid
		{
			set { SubCriteria["TransferSyntaxUid"] = new ProcedureParameter<String>("TransferSyntaxUid", value); }
		}
		public StudyStatusEnum StudyStatusEnum
		{
			set { SubCriteria["StudyStatusEnum"] = new ProcedureParameter<ServerEnum>("StudyStatusEnum", value); }
		}
		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}
	}
}
