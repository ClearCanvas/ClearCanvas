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

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Implementation of <see cref="IQueryBuilder"/> for creating procedure search queries.
	/// </summary>
	public class ProcedureQueryBuilder : QueryBuilderBase
	{
		private static readonly HqlFrom DefaultFrom = new HqlFrom(typeof(Procedure).Name, "rp", new[]
                {
                    HqlConstants.JoinProcedureType,
                    HqlConstants.JoinOrder,
                    HqlConstants.JoinDiagnosticService,
                    HqlConstants.JoinVisit,
                    HqlConstants.JoinPatient,
                    HqlConstants.JoinPatientProfile
                });

		#region Overrides of QueryBuilderBase

		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public override void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Froms.Add(DefaultFrom);
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
	}
}
