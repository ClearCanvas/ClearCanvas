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
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface to an object that supports a lookup field on the user-interface.
    /// </summary>
    public interface ILookupHandler
    {
        /// <summary>
        /// Attempts to resolve a query to a single item, optionally interacting with the user.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="query">The text query.</param>
        /// <param name="interactive">True if interaction with the user is allowed.</param>
        /// <param name="result">The singular result.</param>
        /// <returns>True if the query was resolved to a singular item, otherwise false.</returns>
        bool Resolve(string query, bool interactive, out object result);

        /// <summary>
        /// Formats an item for display in the user-interface.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string FormatItem(object item);

        /// <summary>
        /// Gets a suggestion provider that provides suggestions for the lookup field.
        /// </summary>
        ISuggestionProvider SuggestionProvider { get; }
    }
}
