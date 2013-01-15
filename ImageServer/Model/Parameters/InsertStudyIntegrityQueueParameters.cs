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
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertStudyIntegrityQueueParameters: ProcedureParameters
    {
        public InsertStudyIntegrityQueueParameters()
            : base("InsertStudyIntegrityQueue")
        {
			// This is output from the stored procedure
			SubCriteria["Inserted"] = new ProcedureParameter<bool>("Inserted");
        }
        public String StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public String SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<String>("SeriesInstanceUid", value); }
        }
        public String SopInstanceUid 
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<String>("SopInstanceUid", value); }
        }
        public String SeriesDescription
        {
            set { SubCriteria["SeriesDescription"] = new ProcedureParameter<String>("SeriesDescription", value); }
        }
        public XmlDocument StudyData
        {
            set { SubCriteria["StudyData"] = new ProcedureParameter<XmlDocument>("StudyData", value); }
        }
		public XmlDocument Details
        {
            set { SubCriteria["Details"] = new ProcedureParameter<XmlDocument>("Details", value); }
        }
        public ServerEnum StudyIntegrityReasonEnum
        {
            set { SubCriteria["StudyIntegrityReasonEnum"] = new ProcedureParameter<ServerEnum>("StudyIntegrityReasonEnum", value); }
        }
        public String GroupID
        {
            set { SubCriteria["GroupID"] = new ProcedureParameter<String>("GroupID", value); }
        }
        public String UidRelativePath
        {
            set { SubCriteria["UidRelativePath"] = new ProcedureParameter<String>("UidRelativePath", value); }
        }
		public bool Inserted
		{
			get
			{
				ProcedureParameter<bool> val = SubCriteria["Inserted"] as ProcedureParameter<bool>;
				return val == null ? false : val.Value;
			}
		}
    }
}
