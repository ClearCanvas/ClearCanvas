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
	/// Defines an interface for building queries.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This interface provides a number of methods that assemble different parts of a query,
	/// allowing for a number of different variations of the same query to be constructed by
	/// calling different combinations of methods.  The order in which the methods are called
	/// generally does not matter, with the exception that <see cref="AddRootQuery"/> must
	/// always be called, and must be called first.
	/// </para>
	/// <para>
	/// The <see cref="PreProcessResult"/> method is called to pre-process result tuples, which 
	/// allows the query builder the freedom to create a 'select' clause that differs from what
	/// is expected by the broker.
	/// </para>
	/// </remarks>
	public interface IQueryBuilder
	{
		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Constrains the patient profile to match the performing facility.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds criteria to the query (the 'where' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds ordering to the query (the 'rder by' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddOrdering(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds a count projection to the query (e.g. 'select count(*)').
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddCountProjection(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds an item projection to the query (the 'select' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddItemProjection(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds a paging restriction to the query (the 'top' or 'limit' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		void AddPagingRestriction(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Query result tuples are passed through this method in order to perform
		/// any pre-processing.
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		object[] PreProcessResult(object[] tuple, QueryBuilderArgs args);
	}
}
