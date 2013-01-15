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

namespace ClearCanvas.Healthcare.Extended
{
	[WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryBooking")]
	public abstract class RegistrationProtocolWorklist : Worklist
	{
		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			// TODO: ProtocollingWorklistQueryBuilder may not be exactly correct because it contains an additional constraint
			return (IList)wqc.GetBroker<IProtocolWorklistItemBroker>().GetWorklistItems<WorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IProtocolWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			// TODO: ProtocollingWorklistQueryBuilder may not be exactly correct because it contains an additional constraint
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
	/// RegistrationPendingProtocolWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationPendingProtocolWorklistDescription")]
	public class RegistrationPendingProtocolWorklist : RegistrationProtocolWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 
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

	/// <summary>
	/// RegistrationAsapPendingProtocolWorklist entity 
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationAsapPendingProtocolWorklistDescription")]
	public class RegistrationAsapPendingProtocolWorklist : RegistrationProtocolWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);	//bug #3498: exclude procedures that are no longer in SC status 

			// any procedures with pending protocol assignment, where the procedure scheduled start time is filtered
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureScheduledStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// RegistrationRejectedProtocolWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationRejectedProtocolWorklistDescription")]
	public class RegistrationRejectedProtocolWorklist : RegistrationProtocolWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new [] { typeof(ProtocolResolutionStep) };
		}
	}

	/// <summary>
	/// RegistrationCompletedProtocolWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationCompletedProtocolWorklistDescription")]
	public class RegistrationCompletedProtocolWorklist : RegistrationProtocolWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);

			// only unscheduled procedures should be in this list
			criteria.Procedure.ScheduledStartTime.IsNull();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);

			// checked in procedures are also in the scheduled status and may or may not have no scheduled start time
			// but they should be excluded since there is no reason to schedule a patient who is already here
			criteria.ProcedureCheckIn.CheckInTime.IsNull();

			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				null,
				WorklistOrdering.PrioritizeNewestItems);
		}

		/// <summary>
		/// RegistrationToBeScheduledWorklist entity
		/// </summary>
		[ExtensionOf(typeof(WorklistExtensionPoint))]
		[WorklistCategory("WorklistCategoryBooking")]
		[WorklistClassDescription("RegistrationToBeScheduledWorklistDescription")]
		public class RegistrationToBeScheduledWorklist : RegistrationWorklist
		{
			protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
			{
				var criteria = new RegistrationWorklistItemSearchCriteria();
				criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);

				// only unscheduled items should appear in this list
				criteria.Procedure.ScheduledStartTime.IsNull();

				return new WorklistItemSearchCriteria[] { criteria };
			}

			protected override TimeDirective GetTimeDirective()
			{
				return new TimeDirective(
					WorklistItemField.OrderSchedulingRequestTime,
					null,
					WorklistOrdering.PrioritizeOldestItems);
			}
		}
	}
}
