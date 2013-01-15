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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Extended
{
	/// <summary>
	/// Abstract base class for protocoling admin worklists.
	/// </summary>
	[WorklistCategory("WorklistCategoryRadiologistAdmin")]
	public abstract class ProtocolingAdminWorklist : ProtocolingWorklist
	{
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("ProtocollingAdminAssignedWorklist")]
	public class ProtocollingAdminAssignedWorklist : ProtocolingAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var performerCriteria = BuildCommonCriteria(wqc);
			performerCriteria.ProcedureStep.Performer.Staff.IsNotNull();

			var scheduledPerformerCriteria = BuildCommonCriteria(wqc);
			scheduledPerformerCriteria.ProcedureStep.Scheduling.Performer.Staff.IsNotNull();

			return new[] { performerCriteria, scheduledPerformerCriteria };
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
}
