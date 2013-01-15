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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IReportingWorkflowItemToolContext : IWorkflowItemToolContext<ReportingWorklistItemSummary>
	{
	}

	public interface IReportingWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public abstract class ReportingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ReportingWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IReportingWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
	{
		class ReportingWorkflowItemToolContext : WorkflowItemToolContext, IReportingWorkflowItemToolContext
		{
			public ReportingWorkflowItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class ReportingWorkflowFolderToolContext : WorkflowFolderToolContext, IReportingWorkflowFolderToolContext
		{
			public ReportingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		protected ReportingWorkflowFolderSystemBase(string title)
			: base(title)
		{
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new ReportingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new ReportingWorkflowItemToolContext(this);
		}

		protected static bool CurrentStaffCanSupervise()
		{
			string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			List<string> staffTypes = string.IsNullOrEmpty(filters)
										? new List<string>()
										: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); });
			string currentUserStaffType = LoginSession.Current.Staff.StaffType.Code;
			return staffTypes.Contains(currentUserStaffType);
		}
	}
}
