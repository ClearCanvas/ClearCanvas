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
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ExtensionPoint]
	public class ReportingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ReportingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Common.AuthorityTokens.FolderSystems.Reporting)]
	public class ReportingWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ReportingWorkflowFolderExtensionPoint, ReportingWorkflowFolderToolExtensionPoint,
			ReportingWorkflowItemToolExtensionPoint>
	{
		public ReportingWorkflowFolderSystem()
			: base(SR.TitleReportingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedFolder());

			var workflowConfig = new WorkflowConfigurationReader();
			if (workflowConfig.EnableInterpretationReviewWorkflow && CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewFolder());
			}

			this.Folders.Add(new Folders.Reporting.DraftFolder());

			if (workflowConfig.EnableTranscriptionWorkflow)
			{
				this.Folders.Add(new Folders.Reporting.InTranscriptionFolder());
				this.Folders.Add(new Folders.Reporting.ReviewTranscriptionFolder());
			}

			if (workflowConfig.EnableInterpretationReviewWorkflow && 
				Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Report.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingReviewFolder());

			this.Folders.Add(new Folders.Reporting.VerifiedFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.ReportingFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Reporting", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ReportingSearchFolder();
		}
	}
}
