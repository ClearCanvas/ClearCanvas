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

using System.Data.Linq;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	partial class WorkItem : ILinq2SqlEntity
	{
		public override string ToString()
		{
			return string.Format("{0}\nType:{1}\nPriority: {2}\nScheduled: {3}\nProcess:{4}", Oid, Type, Priority, ScheduledTime, ProcessTime);
		}

		public WorkItemRequest Request
		{
			get { return Serializer.DeserializeWorkItemRequest(SerializedRequest); }
			set { SerializedRequest = Serializer.SerializeWorkItemRequest(value); }
		}

		public WorkItemProgress Progress
		{
			get { return Serializer.DeserializeWorkItemProgress(SerializedProgress); }
			set { SerializedProgress = Serializer.SerializeWorkItemProgress(value); }
		}

		long ILinq2SqlEntity.RowId
		{
			get { return Oid; }
			set { Oid = value; }
		}

		Binary ILinq2SqlEntity.RowVersion
		{
			get { return Version; }
			set { Version = value; }
		}
	}
}