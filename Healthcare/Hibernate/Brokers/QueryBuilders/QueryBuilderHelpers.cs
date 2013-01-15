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
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Static utility class that defines some helper methods for building HQL queries.
	/// </summary>
	public static class QueryBuilderHelpers
	{
		/// <summary>
		/// Adds the specified criteria to the specified query, pre-pending the specified qualifier.
		/// </summary>
		/// <param name="qualifier"></param>
		/// <param name="criteria"></param>
		/// <param name="query"></param>
		/// <param name="remapHqlExprFunction"></param>
		/// <remarks>
		/// All HQL dot expressions are passed through the <paramref name="remapHqlExprFunction"/>, allowing the expression
		/// to be modified prior to be added to the query.
		/// </remarks>
		public static void AddCriteriaToQuery(string qualifier, WorklistItemSearchCriteria[] criteria, HqlProjectionQuery query,
			Converter<string, string> remapHqlExprFunction)
		{
			var or = new HqlOr();
			foreach (var c in criteria)
			{
				if(c.IsEmpty)
					continue;

				var conditions = HqlCondition.FromSearchCriteria(qualifier, c, remapHqlExprFunction);
				var and = new HqlAnd(conditions);
				if (and.Conditions.Count > 0)
					or.Conditions.Add(and);
			}

			if (or.Conditions.Count > 0)
				query.Conditions.Add(or);
		}

		/// <summary>
		/// Adds the specified ordering to the specified query, pre-pending the specified qualifier.
		/// </summary>
		/// <param name="qualifier"></param>
		/// <param name="query"></param>
		/// <param name="criteria"></param>
		/// <param name="remapHqlExprFunction"></param>
		/// <remarks>
		/// All HQL dot expressions are passed through the <paramref name="remapHqlExprFunction"/>, allowing the expression
		/// to be modified prior to be added to the query.
		/// </remarks>
		public static void AddOrderingToQuery(string qualifier, HqlProjectionQuery query, WorklistItemSearchCriteria[] criteria,
			Converter<string, string> remapHqlExprFunction)
		{
			// use the sorting information from the first WorklistItemSearchCriteria object only
			// (the assumption is that they are all identical)
			var c = CollectionUtils.FirstElement(criteria);
			if (c == null)
				return;

			var sorts = HqlSort.FromSearchCriteria(qualifier, c, remapHqlExprFunction);
			query.Sorts.AddRange(sorts);
		}

		/// <summary>
		/// NHibernate has a bug where criteria that de-reference properties not joined into the From clause are not
		/// always handled properly.  For example, in order to evaluate a criteria such as "ps.Scheduling.Performer.Staff.Name like ?",
		/// NHiberate will inject a theta-join on Staff into the SQL.  This works ok by itself.  However, when evaluating a criteria
		/// such as "ps.Scheduling.Performer.Staff.Name.FamilyName like ? or ps.Performer.Staff.Name.FamilyName like ?", NHibernate
		/// injects two Staff theta-joins into the SQL, which incorrectly results in a cross-join situation.
		/// This method modifies any query that has criteria on ps.Scheduling.Performer.Staff or ps.Performer.Staff,
		/// by adding in explicit joins to Staff for these objects, and then substituting the original conditions
		/// with conditions based on these joins.
		/// </summary>
		public static void NHibernateBugWorkaround(HqlFrom from, IList<HqlCondition> conditions, Converter<string, string> remapHqlExprFunction)
		{
			var scheduledStaff = remapHqlExprFunction("ps.Scheduling.Performer.Staff");
			var performerStaff = remapHqlExprFunction("ps.Performer.Staff");

			for (var i = 0; i < conditions.Count; i++)
			{
				var condition = conditions[i];
				if (condition is HqlJunction)
				{
					NHibernateBugWorkaround(from, ((HqlJunction)condition).Conditions, remapHqlExprFunction);
				}
				else if (condition.Hql.StartsWith(scheduledStaff))
				{
					// add join for sst (scheduled staff) if not added
					if (!CollectionUtils.Contains(from.Joins, j => j.Alias == "sst"))
					{
						from.Joins.Add(new HqlJoin(scheduledStaff, "sst", HqlJoinMode.Left));
					}

					// replace the condition with a new condition, using the joined Staff
					var newHql = condition.Hql.Replace(scheduledStaff, "sst");
					conditions[i] = new HqlCondition(newHql, condition.Parameters);
				}
				else if (condition.Hql.StartsWith(performerStaff))
				{
					// add join for pst (performer staff) if not added
					if (!CollectionUtils.Contains(from.Joins, j => j.Alias == "pst"))
					{
						from.Joins.Add(new HqlJoin(performerStaff, "pst", HqlJoinMode.Left));
					}
					// replace the condition with a new condition, using the joined Staff
					var newHql = condition.Hql.Replace(performerStaff, "pst");
					conditions[i] = new HqlCondition(newHql, condition.Parameters);
				}
			}
		}
	}
}
