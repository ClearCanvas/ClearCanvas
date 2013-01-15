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
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Extended
{
	/// <summary>
	/// Abstract base class for protocoling worklists.
	/// </summary>
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryProtocoling")]
	public abstract class ProtocolingWorklist : Worklist
	{
		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IProtocolWorklistItemBroker>().GetWorklistItems<ReportingWorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IProtocolWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IProtocolWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetDefaultProjection(timeField);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(ProtocolAssignmentStep) };
		}
	}

	/// <summary>
	/// ReportingToBeProtocolledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingToBeProtocolledWorklistDescription")]
	public class ReportingToBeProtocolledWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PN);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedProtocolWorklistDescription")]
	public class ReportingAssignedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PN);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ReportingToBeReviewedProtocolWorklistDescription")]
	public class ReportingToBeReviewedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedReviewProtocolWorklistDescription")]
	public class ReportingAssignedReviewProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingDraftProtocolWorklistDescription")]
	public class ReportingDraftProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.ExecutingStaff);
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
	[WorklistClassDescription("ReportingCompletedProtocolWorklistDescription")]
	public class ReportingCompletedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.PR);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingRejectedProtocolWorklistDescription")]
	public class ReportingRejectedProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
			criteria.ProcedureStep.Performer.Staff.EqualTo(wqc.ExecutingStaff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAwaitingApprovalProtocolWorklistDescription")]
	public class ReportingAwaitingApprovalProtocolWorklist : ProtocolingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ProtocolingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new [] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Protocol.Author.EqualTo(wqc.ExecutingStaff);
			criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				null,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}
