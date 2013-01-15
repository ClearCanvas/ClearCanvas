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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class ProtocolWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ProtocolWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.Protocolling)]
	public class ProtocolWorkflowFolderSystem
		: ReportingWorkflowFolderSystemBase<ProtocolWorkflowFolderExtensionPoint, ProtocolWorkflowFolderToolExtensionPoint,
			ProtocolWorkflowItemToolExtensionPoint>
	{
		public ProtocolWorkflowFolderSystem()
			: base(SR.TitleProtocollingFolderSystem)
		{
		}

		protected override void AddDefaultFolders()
		{
			// add the personal folders, since they are not extensions and will not be automatically added
			this.Folders.Add(new Folders.Reporting.AssignedToBeProtocolFolder());

			if (CurrentStaffCanSupervise())
			{
				this.Folders.Add(new Folders.Reporting.AssignedForReviewProtocolFolder());
			}
			this.Folders.Add(new Folders.Reporting.DraftProtocolFolder());

			if (Thread.CurrentPrincipal.IsInRole(Application.Extended.Common.AuthorityTokens.Workflow.Protocol.SubmitForReview))
				this.Folders.Add(new Folders.Reporting.AwaitingApprovalProtocolFolder());

			this.Folders.Add(new Folders.Reporting.CompletedProtocolFolder());
			this.Folders.Add(new Folders.Reporting.RejectedProtocolFolder());
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.ProtocollingFolderSystemUrl;
		}

		protected override PreviewOperationAuditData[] GetPreviewAuditData(WorkflowFolder folder, ICollection<ReportingWorklistItemSummary> items)
		{
			return items.Select(item => new PreviewOperationAuditData("Protocolling", item)).ToArray();
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new Folders.Reporting.ProtocollingSearchFolder();
		}
	}
}
