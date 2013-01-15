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

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines the interface to the collection of items in a <see cref="ITable"/>.
    /// </summary>
    public interface ITableItemCollection : System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the item in the collection at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The item in the collection at the specified index.</returns>
        object this[int index] { get; }
    }
}
