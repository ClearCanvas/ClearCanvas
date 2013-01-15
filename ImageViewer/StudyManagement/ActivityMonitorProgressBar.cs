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

using ClearCanvas.ImageViewer.Common.WorkItem;
using N = System.Collections.Generic.Dictionary<bool, ClearCanvas.ImageViewer.StudyManagement.ProgressBarColor>;
using M = System.Collections.Generic.Dictionary<ClearCanvas.ImageViewer.Common.WorkItem.WorkItemStatusEnum, System.Collections.Generic.Dictionary<bool, ClearCanvas.ImageViewer.StudyManagement.ProgressBarColor>>;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal class ActivityMonitorProgressBar
	{
		private static readonly M _stateMap = new M
		{
															// has failures					// no failures
			{ WorkItemStatusEnum.Pending,			new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.InProgress,		new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Idle,				new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Canceling,			new N {{true, ProgressBarColor.Yellow}, {false, ProgressBarColor.Yellow}} },
			{ WorkItemStatusEnum.Canceled,			new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Yellow}} },
			{ WorkItemStatusEnum.Complete,			new N {{true, ProgressBarColor.Red},	{false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Failed,			new N {{true, ProgressBarColor.Red},	{false, ProgressBarColor.Red}} },

			// these states should not appear in the work item list at all, but if they were to for some reason, we'll make them yellow
			{ WorkItemStatusEnum.DeleteInProgress,	new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Green}} },
			{ WorkItemStatusEnum.Deleted,			new N {{true, ProgressBarColor.Yellow},	{false, ProgressBarColor.Green}} },
		};

		public static ProgressBarColor GetColor(WorkItemProgress progress, WorkItemStatusEnum status)
		{
			var hasFailures = progress.PercentFailed > 0;
			return _stateMap[status][hasFailures];
		}
	}
}
