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

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended.Folders
{
	public class Reporting
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ProtocollingAdminAssignedWorklist)]
		[FolderPath("Active Protocolling Items", true)]
		[FolderDescription("ProtocollingAdminAssignedFolderDescription")]
		public class ProtocollingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingToBeProtocolledWorklist)]
		[FolderPath("To be Protocolled", true)]
		[FolderDescription("ReportingToBeProtocolledFolderDescription")]
		public class ToBeProtocolledFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAssignedProtocolWorklist)]
		[FolderPath("My Items/To be Protocolled")]
		[FolderDescription("ReportingAssignedToBeProtocolFolderDescription")]
		public class AssignedToBeProtocolFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ProtocolWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingToBeReviewedProtocolWorklist)]
		[FolderPath("To be Reviewed", true)]
		[FolderDescription("ReportingToBeReviewedProtocolFolderDescription")]
		public class ToBeReviewedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAssignedReviewProtocolWorklist)]
		[FolderPath("My Items/To be Reviewed")]
		[FolderDescription("ReportingAssignedForReviewProtocolFolderDescription")]
		public class AssignedForReviewProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingDraftProtocolWorklist)]
		[FolderPath("My Items/Draft")]
		[FolderDescription("ReportingDraftProtocolFolderDescription")]
		public class DraftProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingCompletedProtocolWorklist)]
		[FolderPath("My Items/Completed")]
		[FolderDescription("ReportingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingAwaitingApprovalProtocolWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		[FolderDescription("ReportingAwaitingApprovalProtocolFolderDescription")]
		public class AwaitingApprovalProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.ReportingRejectedProtocolWorklist)]
		[FolderPath("My Items/Rejected")]
		[FolderDescription("ReportingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : ReportingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class ProtocollingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public ProtocollingSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ProtocolAssignmentStep"; }
			}
		}
	}
}
