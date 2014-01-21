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
using ClearCanvas.Enterprise.Common;
namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Base interface for a persistence context. See <see cref="IReadContext"/> and <see cref="IUpdateContext"/>.
	/// </summary>
	/// <remarks>
	/// A persistence context is an implementation of the unit-of-work
	/// and identity map patterns, and defines a scope in which the application can perform a set of operations on
	/// a persistent store.  This interface is not implemented directly.
	/// </remarks>
	/// <seealso cref="IReadContext"/>
	/// <seealso cref="IUpdateContext"/>
	public interface IPersistenceContext : IPersistenceBrokerFactory, IDisposable
	{
		/// <summary>
		/// Locks the specified domain object into the context. 
		/// </summary>
		/// <remarks>
		/// If this is an update context, the entity will be treated as "clean".
		/// Use the other overload to specify that the entity is new or dirty.</remarks>
		/// <param name="domainObject"> </param>
		void Lock(object domainObject);

		/// <summary>
		/// Locks the specified domain object into the context with the specified <see cref="DirtyState"/>.
		/// </summary>
		/// <remarks>
		/// Note that it does not make sense to lock an entity into a read context with <see cref="DirtyState.Dirty"/>,
		/// and an exception will be thrown.
		/// </remarks>
		/// <param name="domainObject"> </param>
		/// <param name="state"></param>
		void Lock(object domainObject, DirtyState state);


		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <returns></returns>
		TEntity Load<TEntity>(EntityRef entityRef);// where TEntity : Entity;

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		/// <param name="entityRef"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags);// where TEntity : Entity;

		/// <summary>
		/// Loads the specified entity into this context.
		/// </summary>
		object Load(EntityRef entityRef, EntityLoadFlags flags);

		/// <summary>
		/// Synchronizes the state of the persistent store (database) with the state of this context.
		/// </summary>
		/// <remarks>
		/// This method will ensure that any pending writes to the persistent store are flushed, and that
		/// any generated object identifiers for new persistent objects are generated and assigned to those objects. 
		/// </remarks>
		void SynchState();
	}
}
