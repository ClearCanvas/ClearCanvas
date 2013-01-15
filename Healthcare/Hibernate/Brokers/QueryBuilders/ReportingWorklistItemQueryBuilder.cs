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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="WorklistItemQueryBuilder"/> for reporting worklists.
	/// </summary>
	public class ReportingWorklistItemQueryBuilder : WorklistItemQueryBuilder
	{
		private static readonly HqlCondition ConditionMostRecentPublicationStep = new HqlCondition(
			"(ps.Scheduling.StartTime = (select max(ps2.Scheduling.StartTime) from ProcedureStep ps2 where ps.ReportPart.Report = ps2.ReportPart.Report and ps2.State not in (?)))", ActivityStatus.DC);

		private static readonly HqlCondition ConditionMostRecentTranscriptionStep = new HqlCondition(
			"(ps.Scheduling.StartTime = (select max(ps2.Scheduling.StartTime) from TranscriptionStep ps2 where ps.ReportPart.Report = ps2.ReportPart.Report and ps2.State not in (?)))", ActivityStatus.DC);

		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			base.AddRootQuery(query, args);

			var from = query.Froms[0]; // added by base.AddRootQuery

			// join report/report part object, because may have criteria or projections on this object
			from.Joins.Add(HqlConstants.JoinReportPart);
			from.Joins.Add(HqlConstants.JoinReport);

			// check if we need to apply the "most recent step" condition
			// this is essentially a workaround to avoid showing duplicates in some worklist results
			// we can only apply this workaround when there is exactly one ps class specified
			// fortunately, there are no use cases yet where more than one ps class is specified
			// that require the workaround
			if (args.ProcedureStepClasses.Length == 1)
			{
				var psClass = CollectionUtils.FirstElement(args.ProcedureStepClasses);
				if (psClass == typeof(PublicationStep))
					query.Conditions.Add(ConditionMostRecentPublicationStep);
				if (psClass == typeof(TranscriptionStep))
					query.Conditions.Add(ConditionMostRecentTranscriptionStep);
			}
		}
	}
}
