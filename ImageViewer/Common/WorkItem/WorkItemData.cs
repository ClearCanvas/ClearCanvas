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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	[DataContract(Name = "WorkItemPriority", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum WorkItemPriorityEnum
	{
		[EnumMember]
		Stat = 1,

		[EnumMember]
		High = 2,

		[EnumMember]
		Normal = 3
	}

	[DataContract(Name = "WorkItemStatus", Namespace = ImageViewerWorkItemNamespace.Value)]
	public enum WorkItemStatusEnum
	{
		[EnumMember]
		Pending = 1,

		[EnumMember]
		InProgress = 2,

		[EnumMember]
		Complete = 3,

		[EnumMember]
		Idle = 4,

		[EnumMember]
		Deleted = 5,

		[EnumMember]
		Canceled = 6,

		[EnumMember]
		Failed = 7,

		[EnumMember]
		DeleteInProgress = 8,

		[EnumMember]
		Canceling = 9,
	}

	/// <summary>
	/// Base WorkItem representing a unit of Work to be done.
	/// </summary>
	[DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	public class WorkItemData : DataContractBase
	{
		public WorkItemData()
		{
			Priority = WorkItemPriorityEnum.Normal;
			Status = WorkItemStatusEnum.Pending;
		}

		/// <summary>
		/// The Identifier for the WorkItem.
		/// </summary>
		[DataMember(IsRequired = true)]
		public long Identifier { get; set; }

		/// <summary>
		/// The Priority of the WorkItem
		/// </summary>
		[DataMember(IsRequired = true)]
		public WorkItemPriorityEnum Priority { get; set; }

		/// <summary>
		/// The current status of the WorkItem
		/// </summary>
		[DataMember(IsRequired = true)]
		public WorkItemStatusEnum Status { get; set; }

		[DataMember(IsRequired = true)]
		public string Type { get; set; }

		[DataMember(IsRequired = false)]
		public string StudyInstanceUid { get; set; }

		[DataMember(IsRequired = true)]
		public DateTime ProcessTime { get; set; }

		[DataMember(IsRequired = true)]
		public DateTime RequestedTime { get; set; }

		[DataMember(IsRequired = true)]
		public DateTime ScheduledTime { get; set; }

		[DataMember(IsRequired = true)]
		public DateTime ExpirationTime { get; set; }

		[DataMember(IsRequired = false)]
		public DateTime? DeleteTime { get; set; }

		//TODO (CR Jun 2012) - This is stored as a smallint in the database, but modeled as an int, should probabaly change the DB.
		[DataMember(IsRequired = true)]
		public int FailureCount { get; set; }

		[DataMember(IsRequired = false)]
		public WorkItemRequest Request { get; set; }

		[DataMember(IsRequired = false)]
		public WorkItemProgress Progress { get; set; }

		public string RetryStatus
		{
			get
			{
				if (FailureCount == 0 || Status != WorkItemStatusEnum.Pending)
					return string.Empty;

				return string.Format(SR.RetryStatus, FailureCount,
				                     ProcessTime.ToString("H:mm"));
			}
		}

		public IPatientData Patient
		{
			get
			{
				var request = Request as WorkItemStudyRequest;
				if (request == null) return null;
				return request.Patient;
			}
		}

		public IStudyData Study
		{
			get
			{
				var request = Request as WorkItemStudyRequest;
				if (request == null) return null;
				return request.Study;
			}
		}
	}
}