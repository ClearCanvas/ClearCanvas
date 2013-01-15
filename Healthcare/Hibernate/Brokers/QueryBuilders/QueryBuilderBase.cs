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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Provides base implementation of <see cref="IQueryBuilder"/>.
	/// </summary>
	public abstract partial class QueryBuilderBase : IQueryBuilder
	{
		#region Implementation of IQueryBuilder

		/// <summary>
		/// Establishes the root query (the 'from' clause and any 'join' clauses).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public abstract void AddRootQuery(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Constrains the patient profile to match the performing facility.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public abstract void AddConstrainPatientProfile(HqlProjectionQuery query, QueryBuilderArgs args);

		/// <summary>
		/// Adds criteria to the query (the 'where' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddCriteria(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			QueryBuilderHelpers.AddCriteriaToQuery(HqlConstants.WorklistItemQualifier, args.Criteria, query, RemapHqlExpression);

			// modify the query to workaround some NHibernate bugs
			QueryBuilderHelpers.NHibernateBugWorkaround(query.Froms[0], query.Conditions, a => a);
		}

		/// <summary>
		/// Adds ordering to the query (the 'rder by' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddOrdering(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			QueryBuilderHelpers.AddOrderingToQuery(HqlConstants.WorklistItemQualifier, query, args.Criteria, RemapHqlExpression);
		}

		/// <summary>
		/// Adds a count projection to the query (e.g. 'select count(*)').
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddCountProjection(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Selects.AddRange(HqlConstants.DefaultCountProjection);
		}

		/// <summary>
		/// Adds an item projection to the query (the 'select' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddItemProjection(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Selects.AddRange(GetWorklistItemSelects(args.Projection));
		}

		/// <summary>
		/// Adds a paging restriction to the query (the 'top' or 'limit' clause).
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		public virtual void AddPagingRestriction(HqlProjectionQuery query, QueryBuilderArgs args)
		{
			query.Page = args.Page;
		}

		/// <summary>
		/// Query result tuples are passed through this method in order to perform
		/// any pre-processing.
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public object[] PreProcessResult(object[] tuple, QueryBuilderArgs args)
		{
			// assume we will return a tuple of the same dimension as the input
			// therefore, we just map field for field, calling to PreProcessResultField on each field
			var projection = args.Projection;
			for (var i = 0; i < projection.Fields.Count; i++)
			{
				tuple[i] = PreProcessResultField(tuple[i], projection.Fields[i]);
			}
			return tuple;
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Overridden to provide custom processing of the specified field.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		protected virtual object PreProcessResultField(object value, WorklistItemField field)
		{
			// special handling of this field, because Name is not a persistent property
			if (field == WorklistItemField.ProcedureStepName)
				return ((ProcedureStep) value).Name;

			// convert entities to entity refs
			if (field.IsEntityRefField)
				return GetEntityRef(value);

			// no special processing required
			return value;
		}

		/// <summary>
		/// Overridden to provide custom handling of the HQL dot expressions.
		/// </summary>
		/// <param name="hqlExpression"></param>
		/// <returns></returns>
		protected static string RemapHqlExpression(string hqlExpression)
		{
			foreach (var kvp in HqlConstants.MapCriteriaKeyToHql)
			{
				var target = string.Format("{0}.{1}.", HqlConstants.WorklistItemQualifier, kvp.Key);
				var hqlReplacement = string.Format("{0}.", kvp.Value);
				if (hqlExpression.StartsWith(target))
					return hqlExpression.Replace(target, hqlReplacement);
			}
			return hqlExpression;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the HQL select list for the specified projection.
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		private static HqlSelect[] GetWorklistItemSelects(WorklistItemProjection projection)
		{
			return CollectionUtils.Map<WorklistItemField, HqlSelect>(projection.Fields, MapWorklistItemFieldToHqlSelect).ToArray();
		}

		/// <summary>
		/// Maps the <see cref="WorklistItemField"/> value to an <see cref="HqlSelect"/> object.
		/// </summary>
		private static HqlSelect MapWorklistItemFieldToHqlSelect(WorklistItemField itemField)
		{
			return HqlConstants.MapWorklistItemFieldToHqlSelect[itemField];
		}

		/// <summary>
		/// Gets the entity-ref of the specified entity, or null if the argument is null.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected static EntityRef GetEntityRef(object entity)
		{
			return entity == null ? null : ((Entity) entity).GetRef();
		}

		#endregion
	}
}
