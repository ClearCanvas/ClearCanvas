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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Command
{
	/// <summary>
	/// Insert a <see cref="WorkItem"/> and <see cref="WorkItemUid"/> entry for a specific SOP Instance UID.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="WorkItem"/> is inserted if it doesn't exist already for the Study.  Otherwise, the <see cref="WorkItemUid"/>
	/// is only inserted.
	/// </para>
	/// </remarks>
	public class InsertWorkItemCommand : DataAccessCommand
	{
		private readonly WorkItemRequest _request;
		private readonly WorkItemProgress _progress;
		private readonly string _studyInstanceUid;

		public int ExpirationDelaySeconds { get; set; }

		public WorkItemUid WorkItemUid { get; set; }

		public WorkItem WorkItem { get; set; }

		public InsertWorkItemCommand(WorkItemRequest request, WorkItemProgress progress, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
			: base("Insert a WorkItem")
		{
			_request = request;
			_progress = progress;
			_studyInstanceUid = studyInstanceUid;
			ExpirationDelaySeconds = 60;

			WorkItemUid = new WorkItemUid
			              	{
			              		Complete = false,
			              		FailureCount = 0,
			              		SeriesInstanceUid = seriesInstanceUid,
			              		SopInstanceUid = sopInstanceUid,
			              		Failed = false
			              	};
		}

		public InsertWorkItemCommand(WorkItem item, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
			: base("Insert a WorkItem")
		{
			_request = item.Request;
			_studyInstanceUid = studyInstanceUid;
			ExpirationDelaySeconds = 60;

			WorkItem = item;

			WorkItemUid = new WorkItemUid
			              	{
			              		Complete = false,
			              		FailureCount = 0,
			              		WorkItemOid = WorkItem.Oid,
			              		SeriesInstanceUid = seriesInstanceUid,
			              		SopInstanceUid = sopInstanceUid,
			              		Failed = false
			              	};
		}

		public InsertWorkItemCommand(WorkItemRequest request, WorkItemProgress progress, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, string filename)
			: base("Insert a WorkItem")
		{
			_request = request;
			_progress = progress;
			_studyInstanceUid = studyInstanceUid;
			ExpirationDelaySeconds = 60;

			WorkItemUid = new WorkItemUid
			              	{
			              		Complete = false,
			              		FailureCount = 0,
			              		SeriesInstanceUid = seriesInstanceUid,
			              		SopInstanceUid = sopInstanceUid,
			              		Failed = false,
			              		File = filename
			              	};
		}

		public InsertWorkItemCommand(WorkItem item, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, string filename)
			: base("Insert a WorkItem")
		{
			_request = item.Request;
			_studyInstanceUid = studyInstanceUid;
			ExpirationDelaySeconds = 60;

			WorkItem = item;

			WorkItemUid = new WorkItemUid
			              	{
			              		Complete = false,
			              		FailureCount = 0,
			              		WorkItemOid = WorkItem.Oid,
			              		SeriesInstanceUid = seriesInstanceUid,
			              		SopInstanceUid = sopInstanceUid,
			              		Failed = false,
			              		File = filename
			              	};
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			var workItemBroker = DataAccessContext.GetWorkItemBroker();

			DateTime now = Platform.Time;

			if (WorkItem != null)
			{
				// Already have a committed WorkItem, just set the Oid
				WorkItemUid.WorkItemOid = WorkItem.Oid;

				WorkItem = workItemBroker.GetWorkItem(WorkItem.Oid);
				if (WorkItem.Status == WorkItemStatusEnum.Idle
				    || WorkItem.Status == WorkItemStatusEnum.Pending
				    || WorkItem.Status == WorkItemStatusEnum.InProgress)
				{
					WorkItem.ExpirationTime = now.AddSeconds(ExpirationDelaySeconds);

					var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
					workItemUidBroker.AddWorkItemUid(WorkItemUid);
				}
				else
				{
					WorkItem = null;
				}
			}

			if (WorkItem == null)
			{
				var list = workItemBroker.GetWorkItems(_request.WorkItemType, WorkItemStatusFilter.Active, _studyInstanceUid);
				if (list != null)
				{
					foreach (var item in list)
					{
						if (item.Request.GetType() == _request.GetType())
						{
							var thisRequest = _request as DicomReceiveRequest;
							if (thisRequest != null)
							{
								var request = item.Request as DicomReceiveRequest;
								if (request != null)
								{
									if (request.SourceServerName.Equals(thisRequest.SourceServerName))
									{
										WorkItem = item;
										break;
									}
								}
							}
							else
							{
								WorkItem = item;
								break;
							}
						}
					}
				}

				if (WorkItem == null)
				{
					WorkItem = new WorkItem
					           	{
					           		ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
					           		Request = _request,
					           		ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
					           		Priority = _request.Priority,
					           		Type = _request.WorkItemType,
					           		DeleteTime = now.AddHours(2),
					           		ExpirationTime = now.AddSeconds(ExpirationDelaySeconds),
					           		RequestedTime = now,
					           		StudyInstanceUid = _studyInstanceUid,
					           		Status = WorkItemStatusEnum.Pending,
					           		Progress = _progress
					           	};

					workItemBroker.AddWorkItem(WorkItem);
					WorkItemUid.WorkItem = WorkItem;

					var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
					workItemUidBroker.AddWorkItemUid(WorkItemUid);
				}
				else
				{
					WorkItem.ExpirationTime = now.AddSeconds(ExpirationDelaySeconds);
					WorkItemUid.WorkItemOid = WorkItem.Oid;
					var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
					workItemUidBroker.AddWorkItemUid(WorkItemUid);
				}
			}
		}
	}
}