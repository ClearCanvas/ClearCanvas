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
	/// Extends <see cref="IQueryBuilder"/> with functionality specific to worklist item queries.
	/// </summary>
	public interface IWorklistItemQueryBuilder : IQueryBuilder
	{
		/// <summary>
		/// Adds worklist filters to the query (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddFilters(HqlProjectionQuery query, WorklistQueryArgs args);

		/// <summary>
		/// Adds the "active procedure step" constraint (affects the 'from' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddActiveProcedureStepConstraint(HqlProjectionQuery query, QueryBuilderArgs args);
	}
}
