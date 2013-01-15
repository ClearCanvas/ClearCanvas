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
    /// Filter parameters that can be applied to a table.
    /// </summary>
    public class TableFilterParams
    {
        private ITableColumn _column;
        private object value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_column">The column to filter by, null to filter by any column.</param>
        /// <param name="value">The value to filter by.</param>
        public TableFilterParams(ITableColumn _column, object value)
        {
            this._column = _column;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the column to filter by.
        /// </summary>
        public ITableColumn Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// Gets or sets the value to filter by.
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}