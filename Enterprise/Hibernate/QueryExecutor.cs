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
using NHibernate;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Responsible for scheduling the execution of HQL queries, deferring where appropriate.
	/// </summary>
	internal class QueryExecutor
	{
		private class QueryBatch
		{
			private readonly IMultiQuery _multiQuery;
			private readonly List<object> _tokens;

			internal QueryBatch(PersistenceContext context)
			{
				_multiQuery = context.Session.CreateMultiQuery();
				_tokens = new List<object>();
			}

			/// <summary>
			/// Adds specified query to batch, returning a token that can be used to retrieve the result set.
			/// </summary>
			/// <param name="query"></param>
			/// <returns></returns>
			internal object Add(IQuery query)
			{
				var token = new object();
				_multiQuery.Add(query);
				_tokens.Add(token);
				return token;
			}

			/// <summary>
			/// Executes all queries in this batch, adding the (token, result-set) pair to the map.
			/// </summary>
			/// <param name="resultMap"></param>
			internal void Execute(IDictionary<object, object> resultMap)
			{
				var results = _multiQuery.List();
				for(var i = 0; i < results.Count; i++)
				{
					resultMap.Add(_tokens[i], results[i]);
				}
			}
		}

		private readonly PersistenceContext _context;
		private QueryBatch _currentBatch;
		private readonly Dictionary<object, object> _mapTokenToResult = new Dictionary<object, object>();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context"></param>
		internal QueryExecutor(PersistenceContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Executes the specified <see cref="NHibernate.IQuery"/> against the database, returning the results
		/// as an untyped <see cref="IList"/>.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="defer"></param>
		/// <returns></returns>
		internal IList<T> ExecuteHql<T>(IQuery query, bool defer)
		{

			return defer ? 
				new DeferredQueryResultList<T>(this, Enqueue(query)) :
				query.List<T>();
		}

		/// <summary>
		/// Executes the specified query, which is expected to return a unique (1 row, 1 column) result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		internal T ExecuteHqlUnique<T>(IQuery query)
		{
			// queries that return unique results do not support deferral (because this would require proxying the result type)
			return query.UniqueResult<T>();
		}

		/// <summary>
		/// Executes the specified DML-style query, returning the number of affected rows.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public int ExecuteHqlDml(IQuery query)
		{
			return query.ExecuteUpdate();
		}

		/// <summary>
		/// Obtains the result set associated with the specified token, forcing the current batch to execute if necessary.
		/// </summary>
		/// <remarks>
		/// A given token can only be used once.  Attempting to claim the same result more than once will throw an exception.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <param name="token"></param>
		/// <returns></returns>
		internal IList<T> GetResult<T>(object token)
		{
			IList<T> result;

			// try to claim the result without executing current batch
			if (ClaimResult(token, out result))
				return result;

			// not found, so we need to execute the batch
			ExecuteBatch();

			// try again
			if (ClaimResult(token, out result))
				return result;

			throw new InvalidOperationException("Invalid token.");
		}

		#region Helpers

		private object Enqueue(IQuery query)
		{
			if (_currentBatch == null)
			{
				_currentBatch = new QueryBatch(_context);
			}

			return _currentBatch.Add(query);
		}

		private void ExecuteBatch()
		{
			_currentBatch.Execute(_mapTokenToResult);
			_currentBatch = null;
		}

		private bool ClaimResult<T>(object token, out IList<T> result)
		{
			object r;
			if (_mapTokenToResult.TryGetValue(token, out r))
			{
				// a result can only be claimed once
				_mapTokenToResult.Remove(token);
				result = new TypeSafeListWrapper<T>((IList)r);
				return true;
			}
			result = null;
			return false;
		}

		#endregion

	}
}
