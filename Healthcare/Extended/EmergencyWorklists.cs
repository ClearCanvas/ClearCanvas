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

namespace ClearCanvas.Healthcare.Extended
{
	/// <summary>
	/// EmergencyScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyScheduledWorklistDescription")]
	public class EmergencyScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			// this is slightly different than the registration scheduled worklist, because we include
			// 'checked in' items here, rather than having a separate 'checked in' worklist
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			//criteria.Order.Status.EqualTo(OrderStatus.SC);
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
	/// EmergencyInProgressWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyInProgressWorklistDescription")]
	public class EmergencyInProgressWorklist : RegistrationInProgressWorklist
	{
	}

	/// <summary>
	/// EmergencyPerformedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyPerformedWorklistDescription")]
	public class EmergencyPerformedWorklist : RegistrationPerformedWorklist
	{
	}

	/// <summary>
	/// EmergencyCancelledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyCancelledWorklistDescription")]
	public class EmergencyCancelledWorklist : RegistrationCancelledWorklist
	{
	}
}
