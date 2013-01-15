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
    public class FilesystemQueueInsertParameters : ProcedureParameters
    {
        public FilesystemQueueInsertParameters()
            : base("InsertFilesystemQueue")
        {
        }

        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        {
            set { SubCriteria["FilesystemQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("FilesystemQueueTypeEnum", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey FilesystemKey
        {
            set { SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }
        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }
        public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }
		public XmlDocument QueueXml
		{
			set { SubCriteria["QueueXml"] = new ProcedureParameter<XmlDocument>("QueueXml", value); }
		}
	}
}
