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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionAwaitingReviewWorklistDescription")]
	public class TranscriptionAwaitingReviewWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.IP, ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.ExecutingStaff);
			criteria.ReportPart.Transcriber.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("TranscriptionToBeReviewedWorklistDescription")]
	public class TranscriptionToBeReviewedWorklist : TranscriptionWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}

}
