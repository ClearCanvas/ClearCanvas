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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Abstract base class for NHibernate implemenations of <see cref="IEntityBroker{TEntity, TSearchCriteria}"/>.
	/// </summary>
	/// <typeparam name="TEntity">The entity class on which this broker operates</typeparam>
	/// <typeparam name="TSearchCriteria">The corresponding <see cref="SearchCriteria"/> class.</typeparam>
	public abstract class EntityBroker<TEntity, TSearchCriteria> : Broker, IEntityBroker<TEntity, TSearchCriteria>
		where TEntity : Entity
		where TSearchCriteria : SearchCriteria, new()
	{
		#region Find overloads

		public IList<TEntity> Find(TSearchCriteria criteria)
		{
			return Find(criteria, new EntityFindOptions());
		}

		public IList<TEntity> Find(TSearchCriteria criteria, EntityFindOptions options)
		{
			return Find(criteria, new SearchResultPage(), options);
		}

		public IList<TEntity> Find(TSearchCriteria[] criteria)
		{
			return Find(criteria, new EntityFindOptions());
		}

		public IList<TEntity> Find(TSearchCriteria[] criteria, EntityFindOptions options)
		{
			return Find(criteria, new SearchResultPage(), options);
		}

		public IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page)
		{
			return Find(criteria, page, new EntityFindOptions());
		}

		public IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page, EntityFindOptions options)
		{
			return Find(new[] { criteria }, page, options);
		}

		public IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page)
		{
			return Find(criteria, page, new EntityFindOptions());
		}

		public IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page, EntityFindOptions options)
		{
			var query = new HqlProjectionQuery(new HqlFrom(typeof(TEntity).Name, "x")) {Page = page, Cacheable = options.Cache};

			// add fetch joins
			foreach (var fetchJoin in GetDefaultFetchJoins())
			{
				query.Froms[0].Joins.Add(new HqlJoin("x." + fetchJoin, null, HqlJoinMode.Inner, true));
			}

			// apply lock hint
			if (options.LockForUpdate)
				query.SetLockMode("x", LockMode.Upgrade);
	
			var or = new HqlOr();
			foreach (var c in criteria)
			{
				var and = new HqlAnd(HqlCondition.FromSearchCriteria("x", c));
				if (and.Conditions.Count > 0)
					or.Conditions.Add(and);

				query.Sorts.AddRange(HqlSort.FromSearchCriteria("x", c));
			}

			if (or.Conditions.Count > 0)
				query.Conditions.Add(or);


			return ExecuteHql<TEntity>(query, options.Defer);
		}

		#endregion

		#region FindAll overloads

		public IList<TEntity> FindAll()
		{
			return FindAll(true, new EntityFindOptions());
		}

		public IList<TEntity> FindAll(EntityFindOptions options)
		{
			return FindAll(true, options);
		}

		public IList<TEntity> FindAll(bool includeDeactivated)
		{
			return FindAll(includeDeactivated, new EntityFindOptions());
		}

		public IList<TEntity> FindAll(bool includeDeactivated, EntityFindOptions options)
		{
			var where = new TSearchCriteria();

			// if the entity class supports deactivation, apply this condition
			if (!includeDeactivated && AttributeUtils.HasAttribute<DeactivationFlagAttribute>(typeof(TEntity)))
			{
				var propertyName = AttributeUtils.GetAttribute<DeactivationFlagAttribute>(typeof(TEntity)).PropertyName;
				var c = new SearchCondition<bool>(propertyName);
				c.EqualTo(false);
				where.SetSubCriteria(c);
			}

			return Find(where, options);
		}

		#endregion

		#region FindOne overloads

		public TEntity FindOne(TSearchCriteria criteria)
		{
			return FindOne(criteria, new EntityFindOptions());
		}

		public TEntity FindOne(TSearchCriteria criteria, EntityFindOptions options)
		{
			return FindOne(new[] { criteria }, options);
		}

		public TEntity FindOne(TSearchCriteria[] criteria)
		{
			return FindOne(criteria, new EntityFindOptions());
		}

		public TEntity FindOne(TSearchCriteria[] criteria, EntityFindOptions options)
		{
			// we could probably implement this using NH proxies, but too much trouble right now
			if (options.Defer)
				throw new NotSupportedException("FindOne queries do not support the 'defer' option.");

			var results = Find(criteria, new SearchResultPage(0, 1), options);

			if (results.Count == 0)
				throw new EntityNotFoundException(null);

			return results[0];
		}

		#endregion

		#region Count overloads

		public long Count(TSearchCriteria criteria)
		{
			return Count(criteria, new EntityFindOptions());
		}

		public long Count(TSearchCriteria criteria, EntityFindOptions options)
		{
			return Count(new[] { criteria }, options);
		}

		public long Count(TSearchCriteria[] criteria)
		{
			return Count(criteria, new EntityFindOptions());
		}

		public long Count(TSearchCriteria[] criteria, EntityFindOptions options)
		{
			// cannot defer count queries, because we have no way of proxying the result
			// without changing the return type of this method
			if (options.Defer)
				throw new NotSupportedException("Count queries do not support the 'defer' option.");

			var query = new HqlQuery(string.Format("select count(*) from {0} x", typeof(TEntity).Name)) { Cacheable = options.Cache };

			// for a "count" query, sort conditions that may be present in the
			// criteria object must be ignored- therefore, only the conditions are added to the query
			var or = new HqlOr();
			foreach (var c in criteria)
			{
				var and = new HqlAnd(HqlCondition.FromSearchCriteria("x", c));
				if (and.Conditions.Count > 0)
					or.Conditions.Add(and);
			}

			if (or.Conditions.Count > 0)
				query.Conditions.Add(or);

			// expect exactly one integer result
			return ExecuteHqlUnique<long>(query);
		}

		#endregion

		#region Load overloads

		public TEntity Load(EntityRef entityRef)
		{
			return this.Context.Load<TEntity>(entityRef);
		}

		public TEntity Load(EntityRef entityRef, EntityLoadFlags flags)
		{
			return this.Context.Load<TEntity>(entityRef, flags);
		}

		#endregion

		#region Delete overloads

		public void Delete(TEntity entity)
		{
			if (this.Context.ReadOnly)
				throw new InvalidOperationException("Cannot delete via read-only persistence context.");

			this.Context.Session.Delete(entity);
		}

		public int Delete(TSearchCriteria criteria)
		{
			return Delete(new[] {criteria});
		}

		public int Delete(TSearchCriteria[] criteria)
		{
			if (this.Context.ReadOnly)
				throw new InvalidOperationException("Cannot delete via read-only persistence context.");

			var query = new HqlQuery(string.Format("delete {0} x", typeof(TEntity).Name));

			var or = new HqlOr();
			foreach (var c in criteria)
			{
				var and = new HqlAnd(HqlCondition.FromSearchCriteria("x", c));
				if (and.Conditions.Count > 0)
					or.Conditions.Add(and);
			}

			if (or.Conditions.Count > 0)
				query.Conditions.Add(or);


			return ExecuteHqlDml(query);
		}

		#endregion

		/// <summary>
		/// Gets the set of fetch-joins that will be placed into the query by default.
		/// </summary>
		/// <remarks>
		/// Sub-classes may override this to provide a default set of fetch-joins, where
		/// each entry in the array is the name of a property or compound property on the
		/// entity class (e.g. for a Procedure, one might have "Order" and "Order.Patient").
		/// WARNING: this API is subject to change.
		/// </remarks>
		/// <returns></returns>
		protected virtual string[] GetDefaultFetchJoins()
		{
			return new string[] { };
		}
	}
}
