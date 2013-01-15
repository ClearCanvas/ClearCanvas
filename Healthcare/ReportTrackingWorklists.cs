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
	[WorklistCategory("WorklistCategoryReportingTracking")]
	[WorklistSupportsReportingStaffRoleFilter(true)]
	public abstract class ReportingTrackingWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// ReportingTrackingActiveWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingTrackingActiveWorklistDescription")]
	public class ReportingTrackingActiveWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var performerCriteria = BuildCommonCriteria(wqc);
			performerCriteria.ProcedureStep.Performer.Staff.IsNotNull();

			var scheduledPerformerCriteria = BuildCommonCriteria(wqc);
			scheduledPerformerCriteria.ProcedureStep.Scheduling.Performer.Staff.IsNotNull();

			return new[] { performerCriteria, scheduledPerformerCriteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		private static ReportingWorklistItemSearchCriteria BuildCommonCriteria(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			return criteria;
		}
	}

	/// <summary>
	/// ReportingTrackingDraftWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingTrackingDraftWorklistDescription")]
	public class ReportingTrackingDraftWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Report.Status.EqualTo(ReportStatus.D);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartPreliminaryTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingPreliminaryWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingTrackingPreliminaryWorklistDescription")]
	public class ReportingTrackingPreliminaryWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Report.Status.EqualTo(ReportStatus.P);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new [] { typeof(ReportingProcedureStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartPreliminaryTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingFinalWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingTrackingFinalWorklistDescription")]
	public class ReportingTrackingFinalWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.Report.Status.EqualTo(ReportStatus.F);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartCompletedTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// ReportingTrackingCorrectedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingTrackingCorrectedWorklistDescription")]
	public class ReportingTrackingCorrectedWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.Report.Status.EqualTo(ReportStatus.C);
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ReportPartCompletedTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}
}
