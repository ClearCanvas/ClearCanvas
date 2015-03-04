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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Reporting
	{
		#region Reporting Worklists

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeReportedWorklist)]
		[FolderPath("FolderToBeReported", true)]
		[FolderDescription("ReportingToBeReportedFolderDescription")]
		public class ToBeReportedFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedWorklist)]
		[FolderPath("FolderMyItems/FolderToBeReported")]
		[FolderDescription("ReportingAssignedFolderDescription")]
		public class AssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingToBeReviewedReportWorklist)]
		[FolderPath("FolderToBeReviewed", true)]
		[FolderDescription("ReportingToBeReviewedFolderDescription")]
		public class ToBeReviewedFolder : ReportingWorkflowFolder
		{
			public ToBeReviewedFolder()
			{
				if(!new WorkflowConfigurationReader().EnableInterpretationReviewWorkflow)
					throw new NotSupportedException();
			}
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAssignedReviewWorklist)]
		[FolderPath("FolderMyItems/FolderToBeReviewed")]
		[FolderDescription("ReportingAssignedForReviewFolderDescription")]
		public class AssignedForReviewFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingDraftWorklist)]
		[FolderPath("FolderMyItems/FolderDraft")]
		[FolderDescription("ReportingDraftFolderDescription")]
		public class DraftFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingInTranscriptionWorklist)]
		[FolderPath("FolderMyItems/FolderInTranscription")]
		[FolderDescription("ReportingInTranscriptionFolderDescription")]
		public class InTranscriptionFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingReviewTranscriptionWorklist)]
		[FolderPath("FolderMyItems/FolderReviewTranscription")]
		[FolderDescription("ReportingReviewTranscriptionFolderDescription")]
		public class ReviewTranscriptionFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingAwaitingReviewWorklist)]
		[FolderPath("FolderMyItems/FolderAwaitingReview")]
		[FolderDescription("ReportingAwaitingReviewFolderDescription")]
		public class AwaitingReviewFolder : ReportingWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.ReportingVerifiedWorklist)]
		[FolderPath("FolderMyItems/FolderVerified")]
		[FolderDescription("ReportingVerifiedFolderDescription")]
		public class VerifiedFolder : ReportingWorkflowFolder
		{
		}

		#endregion

		#region Reporting Tracking Worklists

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingActiveWorklist)]
		[FolderPath("FolderTracking/FolderActive", true)]
		[FolderDescription("ReportingTrackingActiveFolderDescription")]
		public class ReportingTrackingActiveFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingDraftWorklist)]
		[FolderPath("FolderTracking/FolderDraft", true)]
		[FolderDescription("ReportingTrackingDraftFolderDescription")]
		public class ReportingTrackingDraftFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingPreliminaryWorklist)]
		[FolderPath("FolderTracking/FolderPreliminary", true)]
		[FolderDescription("ReportingTrackingPreliminaryFolderDescription")]
		public class ReportingTrackingPreliminaryFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingFinalWorklist)]
		[FolderPath("FolderTracking/FolderFinalized", true)]
		[FolderDescription("ReportingTrackingFinalFolderDescription")]
		public class ReportingTrackingFinalFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(ReportingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingTrackingCorrectedWorklist)]
		[FolderPath("FolderTracking/FolderCorrected", true)]
		[FolderDescription("ReportingTrackingCorrectedFolderDescription")]
		public class ReportingTrackingCorrectedFolder : ReportingWorkflowFolder
		{
		}

		#endregion


		[FolderPath("FolderSearchResults")]
		public class ReportingSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public ReportingSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ReportingProcedureStep"; }
			}

		}
	}
}
