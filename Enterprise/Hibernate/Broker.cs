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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Hql;
using NHibernate;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Abstract base class for all NHibernate broker implementations.
	/// </summary>
	public abstract class Broker : IPersistenceBroker
	{
		private PersistenceContext _ctx;

		/// <summary>
		/// Used by the framework to establish the <see cref="IPersistenceContext"/> in which an instance of
		/// this broker will act.
		/// </summary>
		/// <param name="context"></param>
		public void SetContext(IPersistenceContext context)
		{
			_ctx = (PersistenceContext)context;
		}

		#region Protected API

		/// <summary>
		/// Gets the persistence context associated with this broker instance.
		/// </summary>
		protected PersistenceContext Context
		{
			get { return _ctx; }
		}

		/// <summary>
		/// Gets the NHibernate configuration object.
		/// </summary>
		/// <remarks>
		/// This property is provided for use in exceptional circumstances and should be used with care.
		/// </remarks>
		protected Configuration Configuration
		{
			get { return _ctx.PersistentStore.Configuration; }
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <remarks>
		/// This property is provided for use in exceptional circumstances and should be used with care.
		/// </remarks>
		protected string ConnectionString
		{
			get { return _ctx.PersistentStore.ConnectionString; }
		}

		/// <summary>
		/// Allows a broker to create an ADO.NET command, rather than using NHibernate.  The command
		/// will execute on the same connection and within the same transaction
		/// as any other operation on this context.
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		protected IDbCommand CreateSqlCommand(string sql)
		{
			var cmd = _ctx.Session.Connection.CreateCommand();
			cmd.CommandText = sql;
			_ctx.Session.Transaction.Enlist(cmd);

			return cmd;
		}

		/// <summary>
		/// Allows a broker to create an NHibernate query.
		/// </summary>
		/// <param name="hql"></param>
		/// <returns></returns>
		protected IQuery CreateHibernateQuery(string hql)
		{
			return _ctx.Session.CreateQuery(hql);
		}

		/// <summary>
		/// Allows a broker to load a named HQL query stored in a *.hbm.xml file.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected IQuery GetNamedHqlQuery(string name)
		{
			return _ctx.Session.GetNamedQuery(name);
		}

		/// <summary>
		/// Executes the specified query against the database.
		/// </summary>
		/// <param name="query">the query to execute</param>
		/// <returns>the result set</returns>
		protected IList<T> ExecuteHql<T>(HqlQuery query)
		{
			return ExecuteHql<T>(query, false);
		}

		/// <summary>
		/// Executes the specified query against the database.
		/// </summary>
		/// <param name="query">the query to execute</param>
		/// <param name="defer"></param>
		/// <returns>the result set</returns>
		protected IList<T> ExecuteHql<T>(HqlQuery query, bool defer)
		{
			return _ctx.ExecuteHql<T>(query.BuildHibernateQueryObject(_ctx), defer);
		}

		/// <summary>
		/// Executes the specified query against the database.
		/// </summary>
		/// <param name="query">the query to execute</param>
		/// <returns>the result set</returns>
		protected IList<T> ExecuteHql<T>(IQuery query)
		{
			return ExecuteHql<T>(query, false);
		}

		/// <summary>
		/// Executes the specified query against the database.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="defer"></param>
		/// <returns></returns>
		protected IList<T> ExecuteHql<T>(IQuery query, bool defer)
		{
			return _ctx.ExecuteHql<T>(query, defer);
		}

		/// <summary>
		/// Executes the specified query, which is expected to return a unique (1 row, 1 column) result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		protected T ExecuteHqlUnique<T>(HqlQuery query)
		{
			return _ctx.ExecuteHqlUnique<T>(query.BuildHibernateQueryObject(_ctx));
		}

		/// <summary>
		/// Executes the specified query, which is expected to return a unique (1 row, 1 column) result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		protected T ExecuteHqlUnique<T>(IQuery query)
		{
			return _ctx.ExecuteHqlUnique<T>(query);
		}

		/// <summary>
		/// Executes the specified DML-style query, returning the number of affected rows.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected int ExecuteHqlDml(HqlQuery query)
		{
			return _ctx.ExecuteHqlDml(query.BuildHibernateQueryObject(_ctx));
		}

		/// <summary>
		/// Executes the specified DML-style query, returning the number of affected rows.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected int ExecuteHqlDml(IQuery query)
		{
			return _ctx.ExecuteHqlDml(query);
		}

		/// <summary>
		/// Clears the NHibernate session (i.e. first-level) cache.
		/// </summary>
		/// <remarks>
		/// This method is provided for use in exceptional circumstances and should be used with care.
		/// </remarks>
		protected void ClearSessionCache()
		{
			_ctx.Session.Clear();
		}

		/// <summary>
		/// Creates a new session on a new connection, with the specified interceptor.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		protected ISession CreateSession(IInterceptor interceptor)
		{
			return _ctx.Session.SessionFactory.OpenSession(interceptor);
		}

		#endregion

	}
}
