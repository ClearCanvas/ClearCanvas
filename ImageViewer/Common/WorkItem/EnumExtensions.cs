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


namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public static class EnumExtensions
	{
		public static string GetDescription(this WorkItemStatusEnum value)
		{
			switch (value)
			{
				case WorkItemStatusEnum.Pending:
					return SR.WorkItemStatusEnumPending;
				case WorkItemStatusEnum.InProgress:
					return SR.WorkItemStatusEnumInProgress;
				case WorkItemStatusEnum.Complete:
					return SR.WorkItemStatusEnumComplete;
				case WorkItemStatusEnum.Idle:
					return SR.WorkItemStatusEnumIdle;
				case WorkItemStatusEnum.Deleted:
					return SR.WorkItemStatusEnumDeleted;
				case WorkItemStatusEnum.Canceled:
					return SR.WorkItemStatusEnumCanceled;
				case WorkItemStatusEnum.Failed:
					return SR.WorkItemStatusEnumFailed;
                case WorkItemStatusEnum.DeleteInProgress:
                    return SR.WorkItemStatusEnumDeleteInProgress;
                case WorkItemStatusEnum.Canceling:
                    return SR.WorkItemStatusEnumCanceling;
            }
			throw new NotImplementedException();
		}

		public static string GetDescription(this WorkItemPriorityEnum value)
		{
			switch (value)
			{
				case WorkItemPriorityEnum.Normal:
					return SR.WorkItemPriorityEnumNormal;
				case WorkItemPriorityEnum.Stat:
					return SR.WorkItemPriorityEnumStat;
                case WorkItemPriorityEnum.High:
                    return SR.WorkItemPriorityEnumHigh;
            }
			throw new NotImplementedException();
		}    
	}
}
