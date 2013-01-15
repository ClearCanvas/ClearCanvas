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

using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Implementation of <see cref="IWorklistItemQueryBuilder"/> for creating worklist queries.
	/// </summary>
	public class WorklistItemQueryBuilder : QueryBuilderBase, IWorklistItemQueryBuilder
	{
		private static readonly HqlJoin[] WorklistJoins = {
                HqlConstants.JoinProcedure,
                HqlConstants.JoinProcedureType,
                HqlConstants.JoinProcedureCheckIn,
                HqlConstants.JoinOrder,
                HqlConstants.JoinDiagnosticService,
                HqlConstants.JoinVisit,
                HqlConstants.JoinPatient,
                HqlConstants.JoinPatientProfile
              };

		#region Overrides of QueryBuilderBase

		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			var procedureStepClasses = args.ProcedureStepClasses;

			// if we have 1 procedure step class, we can say "from x"
			// otherwise we need to say "from ProcedureStep where ps.class = ..."
			if (procedureStepClasses.Length == 1)
			{
				var procStepClass = CollectionUtils.FirstElement(procedureStepClasses);
				query.Froms.Add(new HqlFrom(procStepClass.Name, "ps", WorklistJoins));
			}
			else
			{
				// either 0 or > 1 classes were specified
				query.Froms.Add(new HqlFrom(typeof(ProcedureStep).Name, "ps", WorklistJoins));
				if(procedureStepClasses.Length > 1)
				{
					query.Conditions.Add(HqlCondition.IsOfClass("ps", procedureStepClasses));
				}
			}
		}

		/// <summary>
		/// Constrains the patient profile to match the performing facility.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			// constrain patient profile to performing facility
			query.Conditions.Add(HqlConstants.ConditionConstrainPatientProfile);
		}

		#endregion

		#region Implementation of IWorklistQueryBuilder

		/// <summary>
		/// Adds worklist filters to the query (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args)
		{
			QueryBuilderHelpers.AddCriteriaToQuery(HqlConstants.WorklistItemQualifier, args.FilterCriteria, query, RemapHqlExpression);
		}

		/// <summary>
		/// Adds the "active procedure step" constraint (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Conditions.Add(HqlConstants.ConditionActiveProcedureStep);
		}

		#endregion

	}
}
