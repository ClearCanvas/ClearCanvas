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
    public class ServerPartitionInsertParameters : ProcedureParameters
    {
        public ServerPartitionInsertParameters()
            : base("InsertServerPartition")
        {
        }

        public bool Enabled
        {
            set { SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }
        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }
        public String AeTitle
        {
            set { SubCriteria["AeTitle"] = new ProcedureParameter<String>("AeTitle", value); }
        }
        public int Port
        {
            set { SubCriteria["Port"] = new ProcedureParameter<int>("Port", value); }
        }
        public String PartitionFolder
        {
            set { SubCriteria["PartitionFolder"] = new ProcedureParameter<String>("PartitionFolder", value); }
        }
        public DuplicateSopPolicyEnum DuplicateSopPolicyEnum
        {
            set { SubCriteria["DuplicateSopPolicyEnum"] = new ProcedureParameter<ServerEnum>("DuplicateSopPolicyEnum", value); }
        }
        public int DefaultRemotePort
        {
            set { SubCriteria["DefaultRemotePort"] = new ProcedureParameter<int>("DefaultRemotePort", value); }
        }
        public bool AcceptAnyDevice
        {
            set { SubCriteria["AcceptAnyDevice"] = new ProcedureParameter<bool>("AcceptAnyDevice", value); }
        }
        public bool AutoInsertDevice
        {
            set { SubCriteria["AutoInsertDevice"] = new ProcedureParameter<bool>("AutoInsertDevice", value); }
        }
        public bool MatchPatientsName
        {
            set { SubCriteria["MatchPatientsName"] = new ProcedureParameter<bool>("MatchPatientsName", value); }
        }
        public bool MatchPatientId
        {
            set { SubCriteria["MatchPatientId"] = new ProcedureParameter<bool>("MatchPatientId", value); }
        }
        public bool MatchAccessionNumber
        {
            set { SubCriteria["MatchAccessionNumber"] = new ProcedureParameter<bool>("MatchAccessionNumber", value); }
        }
        public bool MatchPatientsBirthDate
        {
            set { SubCriteria["MatchPatientsBirthDate"] = new ProcedureParameter<bool>("MatchPatientsBirthDate", value); }
        }
        public bool MatchIssuerOfPatientId
        {
            set { SubCriteria["MatchIssuerOfPatientId"] = new ProcedureParameter<bool>("MatchIssuerOfPatientId", value); }
        }
        public bool MatchPatientsSex
        {
            set { SubCriteria["MatchPatientsSex"] = new ProcedureParameter<bool>("MatchPatientsSex", value); }
        }
        public bool AuditDeleteStudy
        {
            set { SubCriteria["AuditDeleteStudy"] = new ProcedureParameter<bool>("AuditDeleteStudy", value); }
        }
        public bool AcceptLatestReport
        {
            set { SubCriteria["AcceptLatestReport"] = new ProcedureParameter<bool>("AcceptLatestReport", value); }
        }
        public ServerPartitionTypeEnum ServerPartitionTypeEnum
        {
            set { SubCriteria["ServerPartitionTypeEnum"] = new ProcedureParameter<ServerEnum>("ServerPartitionTypeEnum", value); }
        }
    }
}
