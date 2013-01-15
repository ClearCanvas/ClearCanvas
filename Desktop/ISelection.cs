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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single or multiple selection.
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Returns the set of items that are currently selected.
        /// </summary>
        object[] Items { get; }

        /// <summary>
        /// Convenience method to obtain the currently selected item in a single-select scenario.
        /// </summary>
		/// <remarks>
		/// If no rows are selected, the method returns null.  If more than one row is selected,
        /// it is undefined which item will be returned.
		/// </remarks>
		object Item { get; }

		/// <summary>
		/// Computes the union of this selection with another and returns it.
		/// </summary>
        ISelection Union(ISelection other);

		/// <summary>
		/// Computes the intersection of this selection with another and returns it.
		/// </summary>
		ISelection Intersect(ISelection other);
        
		/// <summary>
		/// Returns an <see cref="ISelection"/> that contains every item contained
		/// in this one that doesn't exist in <param name="other" />.
		/// </summary>
		ISelection Subtract(ISelection other);

		/// <summary>
		/// Determines whether this selection contains the input object.
		/// </summary>
		bool Contains(object item);
    }
}
