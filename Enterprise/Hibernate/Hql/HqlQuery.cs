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
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
	/// <summary>
	/// Provides support for building HQL queries dynamically.
	/// </summary>
	public class HqlQuery : HqlElement
	{
		private readonly string _baseQuery;
		private readonly HqlAnd _where;
		private readonly List<HqlSort> _sorts;
		private SearchResultPage _page;
		private readonly Dictionary<string, LockMode> _lockHints = new Dictionary<string, LockMode>();

		private bool _cacheable;
		private string _cacheRegion;

		/// <summary>
		/// Constructs an empty query
		/// </summary>
		protected HqlQuery()
			: this(null, new HqlCondition[] { }, new HqlSort[] { }, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseHql">The base HQL statement, without the "where" or "order by" clauses</param>
		public HqlQuery(string baseHql)
			: this(baseHql, new HqlCondition[] { }, new HqlSort[] { }, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseHql"></param>
		/// <param name="conditions"></param>
		/// <param name="sorts"></param>
		/// <param name="page"></param>
		public HqlQuery(string baseHql, IEnumerable<HqlCondition> conditions, IEnumerable<HqlSort> sorts, SearchResultPage page)
		{
			_baseQuery = baseHql;
			_page = page;
			_where = new HqlAnd(conditions);
			_sorts = new List<HqlSort>(sorts);
		}

		/// <summary>
		/// Exposes the set of conditions that will form the "where" clause.
		/// </summary>
		public List<HqlCondition> Conditions
		{
			get { return _where.Conditions; }
		}

		/// <summary>
		/// Gets the parameter values that have been supplied to the query.
		/// </summary>
		public object[] Parameters
		{
			get { return _where.Parameters; }
		}

		/// <summary>
		/// Exposes the collection of sorts, which will form the "order by" clause.
		/// </summary>
		public List<HqlSort> Sorts
		{
			get { return _sorts; }
		}

		/// <summary>
		/// Gets or sets the result page
		/// </summary>
		public SearchResultPage Page
		{
			get { return _page; }
			set { _page = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether NHibernate should attempt to cache the results of this query.
		/// </summary>
		/// <remarks>
		/// Caching is useful only if the same query is re-run frequently with the same parameters, and the tables
		/// involved in the query are updated infrequenctly.  In other words, most queries do not benefit from caching.
		/// </remarks>
		public bool Cacheable
		{
			get { return _cacheable; }
			set { _cacheable = value; }
		}

		/// <summary>
		/// Gets or sets a custom cache region for this query.  See <see cref="Cacheable"/> for more information
		/// about query caching.
		/// </summary>
		public string CacheRegion
		{
			get { return _cacheRegion; }
			set { _cacheRegion = value; }
		}

		public void SetLockMode(string alias, LockMode lockMode)
		{
			_lockHints[alias] = lockMode;
		}

		/// <summary>
		/// Returns the HQL text of this query.
		/// </summary>
		public override string Hql
		{
			get
			{
				// build the order by clause
				var orderBy = new StringBuilder();

				// order the sorts first, so that they are injected into the hql string in the right order
				_sorts.Sort();
				foreach (var s in _sorts)
				{
					if (orderBy.Length != 0)
						orderBy.Append(", ");

					orderBy.Append(s.Hql);
				}

				// append where and order by to base query
				var hql = new StringBuilder();
				hql.Append(BaseQueryHql);
				if (_where.Conditions.Count > 0)
				{
					hql.Append(" where ");
					hql.Append(_where.Hql);
				}
				if (orderBy.Length > 0)
				{
					hql.Append(" order by ");
					hql.Append(orderBy.ToString());
				}
				return hql.ToString();
			}
		}

		protected virtual string BaseQueryHql
		{
			get { return _baseQuery; }
		}

		/// <summary>
		/// Constructs an NHibernate IQuery object from this object.
		/// </summary>
		/// <param name="ctx">The persistence context that wraps the NHibernate session</param>
		/// <returns>an IQuery object</returns>
		internal IQuery BuildHibernateQueryObject(PersistenceContext ctx)
		{
			var q = ctx.Session.CreateQuery(this.Hql);

			// add the parameters to the query
			var i = 0;
			foreach (var val in _where.Parameters)
			{
				q.SetParameter(i++, val);
			}

			// if limits were specified, pass them to nhibernate
			if (_page != null)
			{
				if (_page.FirstRow > -1)
					q.SetFirstResult(_page.FirstRow);
				if (_page.MaxRows > -1)
					q.SetMaxResults(_page.MaxRows);
			}

			// configure caching
			q.SetCacheable(_cacheable);
			if (!string.IsNullOrEmpty(_cacheRegion))
				q.SetCacheRegion(_cacheRegion);

			// apply lock hints
			foreach (var kvp in _lockHints)
			{
				q.SetLockMode(kvp.Key, kvp.Value);
			}

			return q;
		}
	}
}
