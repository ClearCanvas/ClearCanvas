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
using System.ComponentModel;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Subclass of the <see cref="TableColumn{TItem,TColumn}"/> class for a nullable DateTime column type.
	/// The value is formatted as DateTime.
	/// </summary>
	/// <typeparam name="TItem">The type of item on which the table is based.</typeparam>
	public class DateTimeTableColumn<TItem> : TableColumn<TItem, DateTime?>
	{
		/// <summary>
		/// Constructs a read-only single-cellrow DateTime table column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		public DateTimeTableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, DateTime?> valueGetter)
			: this(columnName, valueGetter, 1.0f) {}

		/// <summary>
		/// Constructs a read-only single-cellrow DateTime table column with specific width factor.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public DateTimeTableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, DateTime?> valueGetter, float widthFactor)
			: this(columnName, columnName, valueGetter, widthFactor) {}

		/// <summary>
		/// Constructs a read-only single-cellrow DateTime table column.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		public DateTimeTableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, DateTime?> valueGetter)
			: this(columnName, columnDisplayName, valueGetter, 1.0f) {}

		/// <summary>
		/// Constructs a read-only single-cellrow DateTime table column with specific width factor.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public DateTimeTableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, DateTime?> valueGetter, float widthFactor)
			: base(columnName, columnDisplayName, valueGetter, widthFactor)
		{
			this.ValueFormatter = delegate(DateTime? value) { return Format.DateTime(value); };
			this.Comparison = delegate(TItem item1, TItem item2) { return Nullable.Compare(valueGetter(item1), valueGetter(item2)); };
		}
	}
}