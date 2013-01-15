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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IReportingWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ReportingWorklistItemBroker : WorklistItemBrokerBase, IReportingWorklistItemBroker
	{
		public ReportingWorklistItemBroker()
			: base(new ReportingWorklistItemQueryBuilder())
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected ReportingWorklistItemBroker(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
			:base(worklistItemQueryBuilder, procedureSearchQueryBuilder, patientSearchQueryBuilder)
		{
		}

		#region IReportingWorklistItemBroker Members

		/// <summary>
		/// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="reportingSteps"></param>
		/// <param name="timeField"></param>
		/// <returns></returns>
		public IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps, WorklistItemField timeField)
		{
			var worklistItemCriteria =
				CollectionUtils.Map(reportingSteps,
				delegate(ReportingProcedureStep ps)
				{
					var criteria = new ReportingWorklistItemSearchCriteria();
					criteria.ProcedureStep.EqualTo(ps);
					return criteria;
				}).ToArray();

			var projection = WorklistItemProjection.GetReportingProjection(timeField);
			var args = new SearchQueryArgs(typeof(ReportingProcedureStep), worklistItemCriteria, projection);
			var query = this.BuildWorklistSearchQuery(args);

			return DoQuery<ReportingWorklistItem>(query, this.WorklistItemQueryBuilder, args);
		}

		/// <summary>
		/// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="interpreter"></param>
		/// <returns></returns>
		public IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter)
		{
			var q = this.GetNamedHqlQuery("linkedInterpretationCandidates");
			q.SetParameter(0, step);
			q.SetParameter(1, interpreter);
			return q.List<InterpretationStep>();
		}

		#endregion
	}
}
