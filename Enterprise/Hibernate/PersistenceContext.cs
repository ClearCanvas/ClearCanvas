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
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using NHibernate;
using NHibernate.Exceptions;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Defines the extension point for all NHibernate broker classes.
	/// </summary>
	[ExtensionPoint]
	public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
	{
	}

	/// <summary>
	/// Abstract base class for NHibernate persistence context implementations.
	/// </summary>
	public abstract class PersistenceContext : IPersistenceContext
	{
		private readonly PersistentStore _persistentStore;
		private readonly QueryExecutor _queryExecutor;
		private ISession _session;

		internal PersistenceContext(PersistentStore persistentStore)
		{
			_persistentStore = persistentStore;
			_queryExecutor = new QueryExecutor(this);
		}

		#region IPersistenceContext members

		/// <summary>
		/// Gets a broker that implements the specified interface.
		/// </summary>
		/// <typeparam name="TBrokerInterface"></typeparam>
		/// <returns></returns>
		public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
		{
			return (TBrokerInterface)GetBroker(typeof(TBrokerInterface));
		}

		public object GetBroker(Type brokerInterface)
		{
			var xp = new BrokerExtensionPoint();
			var broker = (IPersistenceBroker)xp.CreateExtension(new TypeExtensionFilter(brokerInterface));
			broker.SetContext(this);
			return broker;
		}

		/// <summary>
		/// Locks the specified domain object into this context.
		/// </summary>
		/// <param name="domainObject"> </param>
		public void Lock(object domainObject)
		{
			LockCore((DomainObject)domainObject, DirtyState.Clean);
		}

		/// <summary>
		/// Locks the specified domain object into this context with the specified dirty state.
		/// </summary>
		/// <param name="domainObject"> </param>
		/// <param name="dirtyState"></param>
		public void Lock(object domainObject, DirtyState dirtyState)
		{
			LockCore((DomainObject)domainObject, dirtyState);
		}

		/// <summary>
		/// Loads the specified entity into this context using the default load flags for the context.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <returns></returns>
		public TEntity Load<TEntity>(EntityRef entityRef)
			//where TEntity : Entity
		{
			return this.Load<TEntity>(entityRef, this.DefaultEntityLoadFlags);
		}

		/// <summary>
		/// Loads the specified entity into this context using the specified load flags.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entityRef"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags)
			//where TEntity : Entity
		{
			return (TEntity)Load(entityRef, flags);
		}

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		public object Load(EntityRef entityRef, EntityLoadFlags flags)
		{
			try
			{
				// use a proxy if EntityLoadFlags.Proxy is specified and EntityLoadFlags.CheckVersion is not specified (CheckVersion overrides Proxy)
				var useProxy = (flags & EntityLoadFlags.CheckVersion) == 0 && (flags & EntityLoadFlags.Proxy) == EntityLoadFlags.Proxy;
				var entity = (Entity)Load(EntityRefUtils.GetClass(entityRef), EntityRefUtils.GetOID(entityRef), useProxy);

				// check version if necessary
				if ((flags & EntityLoadFlags.CheckVersion) == EntityLoadFlags.CheckVersion && !EntityRefUtils.GetVersion(entityRef).Equals(entity.Version))
					throw new EntityVersionException(EntityRefUtils.GetOID(entityRef), null);

				return entity;

			}
			catch (ObjectNotFoundException hibernateException)
			{
				// note that we will only get here if useProxy == false in the above block
				// if the entity is proxied, verification of its existence is deferred until the proxy is realized
				throw new EntityNotFoundException(hibernateException);
			}
		}

		/// <summary>
		/// Synchronizes the state of the context with the persistent store, ensuring that any new entities
		/// have OIDs generated.
		/// </summary>
		public void SynchState()
		{
			try
			{
				if (_session != null)
				{
					SynchStateCore();
				}
			}
			catch (Exception e)
			{
				HandleHibernateException(e, SR.ExceptionSynchState);
			}
		}

		#endregion

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion


		#region Protected abstract members

		/// <summary>
		/// Default <see cref="EntityLoadFlags"/> to be used by this context.
		/// </summary>
		protected abstract EntityLoadFlags DefaultEntityLoadFlags { get; }

		/// <summary>
		/// Factory method to create the NHibernate session.
		/// </summary>
		/// <returns></returns>
		protected abstract ISession CreateSession();

		/// <summary>
		/// Implementation of SynchState logic.
		/// </summary>
		protected abstract void SynchStateCore();

		/// <summary>
		/// Implementation of Lock logic.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="dirtyState"></param>
		protected abstract void LockCore(DomainObject entity, DirtyState dirtyState);

		/// <summary>
		/// True if this context is read-only.
		/// </summary>
		internal abstract bool ReadOnly { get; }

		#endregion

		#region Helper methods

		/// <summary>
		/// Commits the current transaction.
		/// </summary>
		protected void CommitTransaction()
		{
			if (_session != null)
			{
				_session.Transaction.Commit();
				_session.Close();
				_session = null;
			}
		}

		/// <summary>
		/// Rollsback the current transaction.
		/// </summary>
		protected void RollbackTransaction()
		{
			if (_session != null)
			{
				_session.Transaction.Rollback();
				_session.Close();
				_session = null;
			}
		}

		/// <summary>
		/// Loads the specified enum value into this context.
		/// </summary>
		/// <param name="enumValueClass"></param>
		/// <param name="code"></param>
		/// <param name="proxy"></param>
		/// <returns></returns>
		internal EnumValue LoadEnumValue(Type enumValueClass, string code, bool proxy)
		{
			try
			{
				return (EnumValue)Load(enumValueClass, code, proxy);
			}
			catch (ObjectNotFoundException hibernateException)
			{
				// note that we will only get here if proxy == false in the above block
				// if the entity is proxied, verification of its existence is deferred until the proxy is realized
				throw new EnumValueNotFoundException(enumValueClass, code, hibernateException);
			}
		}

		/// <summary>
		/// Loads the specified persistent object into this context.
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="oid"></param>
		/// <param name="useProxy"></param>
		/// <returns></returns>
		private object Load(Type persistentClass, object oid, bool useProxy)
		{
			return useProxy ?
				this.Session.Load(persistentClass, oid, LockMode.None)
				: this.Session.Get(persistentClass, oid);
		}

		/// <summary>
		/// Wraps an NHibernate exception and rethrows it
		/// </summary>
		/// <param name="e"></param>
		/// <param name="message"></param>
		protected void HandleHibernateException(Exception e, string message)
		{
			if (e is StaleObjectStateException)
			{
				throw new EntityVersionException((e as StaleObjectStateException).Identifier, e);
			}
			if (e is ObjectNotFoundException)
			{
				throw new EntityNotFoundException(e);
			}
			if (e is EntityValidationException)
			{
				// don't wrap EntityValidationException, which we throw from the interceptor
				throw e;
			}

			// see if we can determine a SQL-specific error
			e = TranslateSqlException(e);

			if(e is LockAcquisitionException)
			{
				throw new DeadlockException(e);
			}

			//TODO any other specific kinds of exceptions we need to consider?

			throw new PersistenceException(message, e);
		}

		/// <summary>
		/// Implementation of the Dispose pattern
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_session != null)
				{
					_session.Close();
					_session = null;
				}
			}
		}


		#endregion

		#region Protected and Internal members

		/// <summary>
		/// Executes the specified <see cref="NHibernate.IQuery"/> against the database.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="defer"></param>
		/// <returns></returns>
		internal IList<T> ExecuteHql<T>(IQuery query, bool defer)
		{
			return _queryExecutor.ExecuteHql<T>(query, defer);
		}

		/// <summary>
		/// Executes the specified query, which is expected to return a unique (1 row, 1 column) result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		internal T ExecuteHqlUnique<T>(IQuery query)
		{
			return _queryExecutor.ExecuteHqlUnique<T>(query);
		}

		/// <summary>
		/// Executes the specified DML-style query, returning the number of affected rows.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		internal int ExecuteHqlDml(IQuery query)
		{
			return _queryExecutor.ExecuteHqlDml(query);
		}

		/// <summary>
		/// Gets the NHibernate Session object, instantiating it if it does not already exist.
		/// </summary>
		internal ISession Session
		{
			get
			{
				if (_session == null)
				{
					_session = CreateSession();
					_session.BeginTransaction();
				}

				return _session;
			}
		}

		/// <summary>
		/// Gets the persistent store with which this context is associated.
		/// </summary>
		internal PersistentStore PersistentStore
		{
			get { return _persistentStore; }
		}

		#endregion

		private static Exception TranslateSqlException(Exception e)
		{
			// code here is based on ideas found in the following blog post:
			// http://fabiomaulo.blogspot.com/2009/06/improving-ado-exception-management-in.html

			var sqle = ADOExceptionHelper.ExtractDbException(e) as SqlException;

			// sqle is non-null only for MSSQL (SqlClient driver)
			if (sqle != null)
			{
				switch (sqle.Number)
				{
					case 547:
						return new ConstraintViolationException(e.Message, sqle.InnerException, null);
					case 208:
						return new SQLGrammarException(e.Message, sqle.InnerException);
					case 1205:
						return new LockAcquisitionException(e.Message, sqle.InnerException);
				}
			}
			return e;
		}
	}
}
