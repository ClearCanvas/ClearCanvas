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

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Sort parameters that can be applied to a table.
    /// </summary>
    public class TableSortParams
    {
    	/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="column">The column to sort by.</param>
        /// <param name="ascending">True if the items should be sorted in ascending orders.</param>
        public TableSortParams(ITableColumn column, bool ascending)
        {
            Column = column;
            Ascending = ascending;
        }

    	/// <summary>
    	/// Gets or sets the column to sort by.
    	/// </summary>
    	public ITableColumn Column { get; set; }

    	/// <summary>
    	/// Gets or sets whether the items should be sorted in ascending or descending order.
    	/// </summary>
    	public bool Ascending { get; set; }

		/// <summary>
		/// Gets a comparer representing this sort.
		/// </summary>
		/// <returns></returns>
		public IComparer GetComparer()
		{
			return Column.GetComparer(Ascending);
		}
    }
}
