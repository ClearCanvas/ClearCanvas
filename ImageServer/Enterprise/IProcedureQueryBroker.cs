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
    public delegate void ProcedureQueryCallback<T>(T row);

    /// <summary>
    /// Interface used to define stored procedures that that input parameters and return resultant rows.
    /// </summary>
    /// <typeparam name="TInput">Input parameters</typeparam>
    /// <typeparam name="TOutput">The return type</typeparam>
    public interface IProcedureQueryBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>A list of entities.</returns>
        IList<TOutput> Find(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="callback">The delegate which is supplied query results.</param>
        void Find(TInput criteria, ProcedureQueryCallback<TOutput> callback);

		/// <summary>
		/// Retrieves the first entity matching the specified crtiera.
		/// </summary>
		/// <param name="criteria">The search criteria.</param>
		/// <returns>The entity.</returns>
    	TOutput FindOne(TInput criteria);
    }
}
