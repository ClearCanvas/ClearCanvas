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
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines a column in an <see cref="ITable"/>.
    /// </summary>
    public interface ITableColumn
    {
        /// <summary>
        /// The identifying name of the column.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The heading text of the column.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The type of data that the column holds.
        /// </summary>
        Type ColumnType { get; }

		/// <summary>
		/// Gets or sets a value indicating whether this column is visible.
		/// </summary>
		bool Visible { get; }

        /// <summary>
        /// Gets or sets a resource resolver.
        /// </summary>
        IResourceResolver ResourceResolver { get; }
        
        /// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		event EventHandler VisibleChanged;

        /// <summary>
        /// A factor that influences the width of the column relative to other columns.
        /// </summary>
        /// <remarks>
		/// A value of 1.0 is default.
        /// </remarks>
        float WidthFactor { get; }

        /// <summary>
        /// Gets the width of this column as a percentage of the overall table width.
        /// </summary>
        int WidthPercent { get; }

        /// <summary>
        /// Indicates whether this column is read-only.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Indicates whether this column is clickable.
        /// </summary>
        bool HasClickableLink { get; }

		/// <summary>
		/// Gets the tooltip of this column for the specified item.
		/// </summary>
		string GetTooltipText(object item);

        /// <summary>
        /// Gets the value of this column for the specified item.
        /// </summary>
        /// <param name="item">The item from which the value is to be obtained</param>
        object GetValue(object item);

        /// <summary>
        /// Sets the value of this column on the specified item, assuming this is not a read-only column.
        /// </summary>
        /// <param name="item">The item on which the value is to be set.</param>
        /// <param name="value">The value.</param>
        void SetValue(object item, object value);

		/// <summary>
		/// Format the value of this column for the specified item.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The formatted value</returns>
    	object FormatValue(object value);

        /// <summary>
        /// Sets the click action of this column on the specified item.
        /// </summary>
        /// <param name="item">The item on which the value is to be set.</param>
        void ClickLink(object item);

        /// <summary>
        /// Get a comparer that can be used to sort items in the specified direction.
        /// </summary>
        IComparer GetComparer(bool ascending);

        /// <summary>
        /// Gets the cell row for which this column will be displayed in.
        /// </summary>
        int CellRow { get; }

		/// <summary>
		/// Gets the editor that allows cells in this column to be edited, or null if no custom editor is provided.
		/// </summary>
		/// <returns></returns>
    	ITableCellEditor GetCellEditor();

		/// <summary>
		/// Gets a value indicating whether the specified item is editable.
		/// This method will only ever be called if <see cref="ReadOnly"/> is false.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool IsEditable(object item);
	}
}
