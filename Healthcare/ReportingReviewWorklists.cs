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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// ReportingAssignedReviewWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedReviewWorklistDescription")]
	public class ReportingAssignedReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var assignedToMe = BaseCriteria();
			assignedToMe.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);

			var bySupervisor = BaseCriteria();
			bySupervisor.ReportPart.Supervisor.EqualTo(wqc.ExecutingStaff);

			return new[] { assignedToMe, bySupervisor };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}

		private ReportingWorklistItemSearchCriteria BaseCriteria()
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			return criteria;
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAwaitingReviewWorklistDescription")]
	public class ReportingAwaitingReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteriaNotEqual = new ReportingWorklistItemSearchCriteria();
			criteriaNotEqual.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNotEqual.ReportPart.Interpreter.EqualTo(wqc.ExecutingStaff);
			criteriaNotEqual.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.ExecutingStaff);

			var criteriaNull = new ReportingWorklistItemSearchCriteria();
			criteriaNull.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNull.ReportPart.Interpreter.EqualTo(wqc.ExecutingStaff);
			criteriaNull.ProcedureStep.Scheduling.Performer.Staff.IsNull();

			return new WorklistItemSearchCriteria[] { criteriaNotEqual, criteriaNull };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep), typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingToBeReviewedReportWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingToBeReviewedReportWorklistDescription")]
	public class ReportingToBeReviewedReportWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });

			criteria.ReportPart.Interpreter.NotEqualTo(wqc.ExecutingStaff);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ReportPart.Supervisor.IsNull();

			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems,
				true);
		}
	}

}
