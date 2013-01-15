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
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryRegistration")]
	public abstract class RegistrationWorklist : Worklist
	{
		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IRegistrationWorklistItemBroker>().GetWorklistItems<WorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IRegistrationWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IRegistrationWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetDefaultProjection(timeField);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(RegistrationProcedureStep) };
		}
	}

	/// <summary>
	/// RegistrationScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationScheduledWorklistDescription")]
	public class RegistrationScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			criteria.ProcedureCheckIn.CheckInTime.IsNull();     // exclude anything already checked-in
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
	/// RegistrationCheckedInWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationCheckedInWorklistDescription")]
	public class RegistrationCheckedInWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			criteria.ProcedureCheckIn.CheckInTime.IsNotNull();  // include only items that have been checked-in
			criteria.ProcedureCheckIn.CheckOutTime.IsNull();
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
	/// RegistrationInProgessWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationInProgressWorklistDescription")]
	public class RegistrationInProgressWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.IP);
			criteria.ProcedureCheckIn.CheckOutTime.IsNull();    // exclude any item already checked-out
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// RegistrationPerformedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationPerformedWorklistDescription")]
	public class RegistrationPerformedWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			// "completed" in this context just means the patient has checked-out
			// the order may still be in progress
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.In(new[] { ProcedureStatus.IP, ProcedureStatus.CM });
			criteria.ProcedureCheckIn.CheckOutTime.IsNotNull();
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureCheckOutTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

	/// <summary>
	/// RegistrationCancelledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("RegistrationCancelledWorklistDescription")]
	public class RegistrationCancelledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.In(new[] { ProcedureStatus.DC, ProcedureStatus.CA });

			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}