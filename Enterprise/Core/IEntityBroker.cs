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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Used by <see cref="IEntityBroker{TEntity, TSearchCriteria}.Load"/> to provide fine control over the loading of entities.
	/// </summary>
	[Flags]
	public enum EntityLoadFlags
	{
		/// <summary>
		/// Default value
		/// </summary>
		None = 0x0000,

		/// <summary>
		/// Forces a version check, causing an exception to be thrown if the version does not match
		/// </summary>
		CheckVersion = 0x0001,

		/// <summary>
		/// Asks for a proxy to the entity, rather than loading the full entity.  There is no guarantee
		/// that this flag will be obeyed, because the underlying implementation may not support proxies,
		/// or the entity may not be proxiable.
		/// </summary>
		Proxy = 0x0002,
	}

	public class EntityFindOptions
	{
		/// <summary>
		/// Gets or sets a value indicating whether the query results should be cached.
		/// </summary>
		public bool Cache { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the query can be deferred until the results are needed.
		/// </summary>
		/// <remarks>
		/// Deferred queries can be queued up and sent to the server in a batch when the first
		/// deferred result set is needed.
		/// </remarks>
		public bool Defer { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether pessimistic locking should be used.
		/// </summary>
		/// <remarks>
		/// Implements pessimistic locking by instructing the database to acquire an update lock on selected entities.
		/// This helps to reduce deadlocks in scenarios where a selected entity will likely be updated.
		/// </remarks>
		public bool LockForUpdate { get; set; }
	}


	/// <summary>
	/// Base interface for all entity broker interfaces.
	/// </summary>
	/// <remarks>
	/// An entity broker allows entities to be retrieved from persistent storage. This interface should not be implemented directly.
	/// Instead, a sub-interface should be defined that extends this interface for a given entity.
	/// </remarks>
	/// <typeparam name="TEntity">The <see cref="Entity"/> sub-class on which this broker acts</typeparam>
	/// <typeparam name="TSearchCriteria">The <see cref="SearchCriteria"/> subclass corresponding to the entity</typeparam>
	public interface IEntityBroker<TEntity, TSearchCriteria> : IPersistenceBroker
		where TEntity : Entity
		where TSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Loads the entity referred to by the specified entity reference.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <returns></returns>
		TEntity Load(EntityRef entityRef);

		/// <summary>
		/// Loades the entity referred to by the specified reference, obeying the specified flags
		/// where possible.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		TEntity Load(EntityRef entityRef, EntityLoadFlags flags);

		/// <summary>
		/// Retrieves all entities matching the specified criteria.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <param name="criteria"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria criteria);

		/// <summary>
		/// Retrieves all entities matching the specified criteria.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria criteria, EntityFindOptions options);

		/// <summary>
		/// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR).
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <param name="criteria"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria[] criteria);

		/// <summary>
		/// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR).
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria[] criteria, EntityFindOptions options);

		/// <summary>
		/// Retrieves all entities matching the specified criteria,
		/// constrained by the specified page constraint.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page);

		/// <summary>
		/// Retrieves all entities matching the specified criteria,
		/// constrained by the specified page constraint.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="page"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page, EntityFindOptions options);

		/// <summary>
		/// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR),
		/// constrained by the specified page constraint.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page);

		/// <summary>
		/// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR),
		/// constrained by the specified page constraint.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="page"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page, EntityFindOptions options);

		/// <summary>
		/// Retrieves the entire set of entities of this class.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <returns></returns>
		IList<TEntity> FindAll();

		/// <summary>
		/// Retrieves the entire set of entities of this class.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <returns></returns>
		IList<TEntity> FindAll(EntityFindOptions options);

		/// <summary>
		/// Retrieves the entire set of entities of this class, optionally including de-activated instances.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <returns></returns>
		IList<TEntity> FindAll(bool includeDeactivated);

		/// <summary>
		/// Retrieves the entire set of entities of this class, optionally including de-activated instances.
		/// </summary>
		/// <remarks>
		/// Caution: this method may return an arbitrarily large result set.
		/// </remarks>
		/// <returns></returns>
		IList<TEntity> FindAll(bool includeDeactivated, EntityFindOptions options);

		/// <summary>
		/// Retrieves the first entity matching the specified criteria, or throws a <see cref="EntityNotFoundException"/>
		/// if no matching entity is found.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		TEntity FindOne(TSearchCriteria criteria);

		/// <summary>
		/// Retrieves the first entity matching the specified criteria, or throws a <see cref="EntityNotFoundException"/>
		/// if no matching entity is found.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		TEntity FindOne(TSearchCriteria criteria, EntityFindOptions options);

		/// <summary>
		/// Retrieves the first entity matching any of the specified criteria (the criteria are combined using OR),
		/// or throws a <see cref="EntityNotFoundException"/> if no matching entity is found.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		TEntity FindOne(TSearchCriteria[] criteria);

		/// <summary>
		/// Retrieves the first entity matching any of the specified criteria (the criteria are combined using OR),
		/// or throws a <see cref="EntityNotFoundException"/> if no matching entity is found.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		TEntity FindOne(TSearchCriteria[] criteria, EntityFindOptions options);

		/// <summary>
		/// Returns the number of entities matching the specified criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		long Count(TSearchCriteria criteria);

		/// <summary>
		/// Returns the number of entities matching the specified criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		long Count(TSearchCriteria criteria, EntityFindOptions options);

		/// <summary>
		/// Returns the number of entities matching any of the specified criteria (the criteria are combined using OR).
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		long Count(TSearchCriteria[] criteria);

		/// <summary>
		/// Returns the number of entities matching any of the specified criteria (the criteria are combined using OR).
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		long Count(TSearchCriteria[] criteria, EntityFindOptions options);

		/// <summary>
		/// Makes the specified entity transient (removes it from the persistent store).
		/// </summary>
		/// <param name="entity">The entity instance to remove</param>
		void Delete(TEntity entity);

		/// <summary>
		/// Deletes all entities that match the specified criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		int Delete(TSearchCriteria criteria);

		/// <summary>
		/// Deletes all entities that match the specified criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		int Delete(TSearchCriteria[] criteria);
	}
}
