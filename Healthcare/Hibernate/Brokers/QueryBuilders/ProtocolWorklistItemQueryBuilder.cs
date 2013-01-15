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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="WorklistItemQueryBuilder"/> for protocol worklists.
	/// </summary>
	public class ProtocolWorklistItemQueryBuilder : WorklistItemQueryBuilder
	{
		private static readonly HqlCondition ConditionMostRecentProtocolAssignmentStepIfRejected = new HqlCondition(
			"((pr.Status not in (?)) or (ps.EndTime = (select max(ps2.EndTime) from ProcedureStep ps2 where ps.Protocol = ps2.Protocol)))", ProtocolStatus.RJ);

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			base.AddRootQuery(query, args);

			var from = query.Froms[0];	// this would be added by the base.AddWorklistRootQuery call

			// join protocol object, because may have criteria on this object
			from.Joins.Add(HqlConstants.JoinProtocol);

			// check if we need to apply the "most recent step" condition
			// this is essentially a workaround to avoid showing duplicates in some worklist results
			// we can only apply this workaround when there is exactly one ps class specified
			// fortunately, there are no use cases yet where more than one ps class is specified
			// that require the workaround
			if (args.ProcedureStepClasses.Length == 1)
			{
				var psClass = CollectionUtils.FirstElement(args.ProcedureStepClasses);
				// the proc step class may be set to the more general "ProtocolProcedureStep" so
				// we need to check for both
				if (psClass == typeof(ProtocolAssignmentStep) || psClass == typeof(ProtocolProcedureStep))
				{
					// when querying for Rejected protocols, only show the most recent ProtocolAssignmentStep
					// There may be many ProtocolAssignmentStep if a protocol is rejected, resubmitted and rejected again.
					// For rejected protocols, the condition "pr.Status not in ('RJ')" is always false.  So the max(EndTime) condition is used
					// For non-rejected protocols, the first condition is always true, and the max(EndTime) condition is never used.
					query.Conditions.Add(ConditionMostRecentProtocolAssignmentStepIfRejected);
				}
			}
		}
	}
}
