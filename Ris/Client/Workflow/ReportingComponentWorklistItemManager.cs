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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportingComponentWorklistItemManager : WorklistItemManager<ReportingWorklistItemSummary, IReportingWorkflowService>
	{
		public ReportingComponentWorklistItemManager(string folderName, EntityRef worklistRef, string worklistClassName)
			: base(folderName, worklistRef, worklistClassName, ReportingSettings.Default.NextItemQueueCacheSize)
		{
		}

		protected override IContinuousWorkflowComponentMode GetMode(ReportingWorklistItemSummary worklistItem)
		{
			if (worklistItem == null)
				return ReportingComponentModes.Review;

			if (worklistItem.ProcedureStepName == StepType.Publication)
				return ReportingComponentModes.Review;

			if (worklistItem.ProcedureStepName == StepType.Verification)
				return ReportingComponentModes.Verify;

			if (worklistItem.ProcedureStepName == StepType.TranscriptionReview)
				return ReportingComponentModes.ReviewTranscription;

			switch (worklistItem.ActivityStatus.Code)
			{
				case StepState.Scheduled:
					return worklistItem.IsAddendumStep 
						? ReportingComponentModes.CreateAddendum
						: worklistItem.ReportRef == null ? ReportingComponentModes.Create : ReportingComponentModes.Edit;
				case StepState.InProgress:
					return ReportingComponentModes.Edit;
				default:
					return ReportingComponentModes.Review;
			}
		}

		protected override string TaskName
		{
			get { return "Reporting"; }
		}
	}

	public class ReportingComponentModes
	{
		public static IContinuousWorkflowComponentMode Create = new CreateReportComponentMode();
		public static IContinuousWorkflowComponentMode CreateAddendum = new CreateReportAddendumComponentMode();
		public static IContinuousWorkflowComponentMode Edit = new EditReportComponentMode();
		public static IContinuousWorkflowComponentMode ReviewTranscription = new ReviewTranscriptionReportComponentMode();
		public static IContinuousWorkflowComponentMode Review = new ReviewReportComponentMode();
		public static IContinuousWorkflowComponentMode Verify = new VerifyReportComponentMode();
	}

	public class EditReportComponentMode : ContinuousWorkflowComponentMode
	{
		public EditReportComponentMode()
			: base(false, false, false)
		{
		}
	}

	public class CreateReportComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateReportComponentMode()
			: base(true, true, true)
		{
		}
	}

	public class CreateReportAddendumComponentMode : ContinuousWorkflowComponentMode
	{
		public CreateReportAddendumComponentMode()
			: base(true, false, false)
		{
		}
	}

	public class ReviewReportComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewReportComponentMode()
			: base(false, false, false)
		{
		}
	}

	public class VerifyReportComponentMode : ContinuousWorkflowComponentMode
	{
		public VerifyReportComponentMode()
			: base(false, false, true)
		{
		}
	}

	public class ReviewTranscriptionReportComponentMode : ContinuousWorkflowComponentMode
	{
		public ReviewTranscriptionReportComponentMode()
			: base(false, false, true)
		{
		}
	}
}