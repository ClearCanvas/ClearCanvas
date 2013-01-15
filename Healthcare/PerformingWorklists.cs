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
using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryPerforming")]
	public abstract class PerformingWorklist : Worklist
    {
        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
			return (IList)wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItems<WorklistItem>(this, wqc);
        }

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IModalityWorklistItemBroker>().CountWorklistItems(this, wqc);
        }

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetDefaultProjection(timeField);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] {typeof (ModalityProcedureStep)};
		}
    }
	
    /// <summary>
	/// PerformingScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingScheduledWorklistDescription")]
	public class PerformingScheduledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingCheckedInWorklistDescription")]
	public class PerformingCheckedInWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureCheckInTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingInProgressWorklistDescription")]
	public class PerformingInProgressWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistPerformedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingPerformedWorklistDescription")]
	public class PerformingPerformedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
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

    /// <summary>
	/// PerformingCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingCancelledWorklistDescription")]
	public class PerformingCancelledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
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

    /// <summary>
	/// PerformingUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingUndocumentedWorklistDescription")]
	public class PerformingUndocumentedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new [] { typeof(DocumentationProcedureStep) };
		}
	}
}
