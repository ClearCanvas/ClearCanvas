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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public delegate void SelectCallback<T>(T row);

    /// <summary>
    /// An interface for an <see cref="IPersistenceBroker"/> for accessing <see cref="ServerEntity"/>
    /// objects in the persistent store.
    /// </summary>
    /// <remarks>
    /// This interface allows for loading, updating, inserting, and selecting <see cref="ServerEntity"/>
    /// derived objects from the database.  
    /// </remarks>
    /// <typeparam name="TServerEntity"></typeparam>
    /// <typeparam name="TSelectCriteria"></typeparam>
    /// <typeparam name="TUpdateColumns"></typeparam>
    public interface IEntityBroker<TServerEntity, TSelectCriteria, TUpdateColumns> : IPersistenceBroker
        where TServerEntity : ServerEntity, new()
        where TSelectCriteria : EntitySelectCriteria
        where TUpdateColumns : EntityUpdateColumns
    {
        /// <summary>
        /// Loads the <see cref="ServerEntity"/> referred to by the specified entity reference.
        /// </summary>
        /// <param name="entityRef">The key for the entity to load.</param>
        /// <returns></returns>
        TServerEntity Load(ServerEntityKey entityRef);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>A list of <see cref="ServerEntity"/> objects.</returns>
        IList<TServerEntity> Find(TSelectCriteria criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxRows">Number of  rows to return.</param>
        /// <param name="startIndex">The start index (zero based) from which rows should be returned.</param>
        /// <returns>A list of <see cref="ServerEntity"/> objects.</returns>
        IList<TServerEntity> Find(TSelectCriteria criteria, int startIndex, int maxRows);

		/// <summary>
		/// Retrieves an entity matching the specified criteria.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <returns>A <see cref="ServerEntity"/> object, or null if no results.</returns>
		TServerEntity FindOne(TSelectCriteria criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="callback">A callback which is called for each result found.</param>
        void Find(TSelectCriteria criteria, SelectCallback<TServerEntity> callback);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="maxRows">Number of  rows to return.</param>
		/// <param name="startIndex">The start index (zero based) from which rows should be returned.</param>
		/// <param name="callback">A callback which is called for each result found.</param>
		void Find(TSelectCriteria criteria, int startIndex, int maxRows, SelectCallback<TServerEntity> callback);

        /// <summary>
        /// Retrieves a count of the entities within the persistent store that
        /// match the specified criteria.
        /// </summary>
        /// <param name="criteria">The input criteria</param>
        /// <returns>The number or resultant rows.</returns>
        int Count(TSelectCriteria criteria);

        /// <summary>
        /// Updates the entity specified by the <paramref name="entityKey"/> with values specified in <paramref="parameters"/>.
        /// </summary>
        /// <param name="entityKey">The <see cref="ServerEntityKey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <param name="parameters">The <see cref="EntityUpdateColumns"/> specifying the columns to be updated.</param>
        /// <returns></returns>
        bool Update(ServerEntityKey entityKey, TUpdateColumns parameters);

        /// <summary>
        /// Updates the entity specified by the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ServerEntityKey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <returns></returns>
        bool Update(TServerEntity entity);


		/// <summary>
		/// Updates the entity specified by the <paramref name="criteria"/> with values updated specified in <paramref="parameters"/>.
		/// </summary>
		/// <param name="criteria">The criteria for the WHERE clause of the update.</param>
		/// <param name="parameters">The <see cref="EntityUpdateColumns"/> specifying the columns to be updated.</param>
		/// <returns></returns>
		bool Update(TSelectCriteria criteria, TUpdateColumns parameters);

        /// <summary>
        /// Inserts a new entity with field values indicated in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">The <see cref="EntityUpdateColumns"/> object which specifies the values for the columns in the new entity.</param>
        /// <returns>The newly inserted entity.</returns>
        TServerEntity Insert(TUpdateColumns parameters);

        /// <summary>
        /// Delete an entity.
        /// </summary>
        /// <param name="entityKey">The key for the entity to delete.</param>
        /// <returns>true on success, false on failure</returns>
        bool Delete(ServerEntityKey entityKey);

        /// <summary>
        /// Delete entities matching specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria for the entities to delete.</param>
        /// <returns>Number of records deleted</returns>
        int Delete(TSelectCriteria criteria);
    }
}
