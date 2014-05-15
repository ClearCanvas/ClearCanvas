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
	/// Implementation of <see cref="ITableColumn"/> for use with the <see cref="Table{TItem}"/> class.
	/// </summary>
	/// <typeparam name="TItem">The type of item on which the table is based.</typeparam>
	/// <typeparam name="TColumn">The type of value that this column holds.</typeparam>
	public class TableColumn<TItem, TColumn> : TableColumnBase<TItem>
	{
		/// <summary>
		/// Delegate that is used to pull the value of a column from an object.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <typeparam name="TValue">The expected type of the value to pull.</typeparam>
		/// <param name="obj">The object from which to pull the value.</param>
		/// <returns>The value.</returns>
		public delegate TValue GetColumnValueDelegate<TObject, TValue>(TObject obj);

		/// <summary>
		/// Delegate that is used to push the value of a column to an object.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <typeparam name="TValue">The type of the value to push.</typeparam>
		/// <param name="obj">The object to which the value is pushed.</param>
		/// <param name="val">The value.</param>
		public delegate void SetColumnValueDelegate<TObject, TValue>(TObject obj, TValue val);

		private readonly GetColumnValueDelegate<TItem, TColumn> _valueGetter;

		private readonly SetColumnValueDelegate<TItem, TColumn> _valueSetter;

		private Action<TItem> _linkActionDelegate;

		private Converter<TItem, string> _tooltipTextProvider;

		private Converter<TColumn, object> _valueFormatter = value => value;

		/// <summary>
		/// Constructs a multi-cellrow table column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(
			[param : Localizable(true)] string columnName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			Comparison<TItem> comparison,
			int cellRow)
			: base(columnName, typeof (TColumn), widthFactor, comparison, cellRow)
		{
			_valueGetter = valueGetter;
			_valueSetter = valueSetter;
		}

		/// <summary>
		/// Constructs a multi-cellrow table column.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(
			string columnName, [param : Localizable(true)] string columnDisplayName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			Comparison<TItem> comparison,
			int cellRow)
			: base(columnName, columnDisplayName, typeof (TColumn), widthFactor, comparison, cellRow)
		{
			_valueGetter = valueGetter;
			_valueSetter = valueSetter;
		}

		#region Multi-CellRow constructors

		/// <summary>
		/// Constructs a Multi-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter, int cellRow)
			: this(columnName, valueGetter, valueSetter, 1.0f, null, cellRow) {}

		/// <summary>
		/// Constructs a Multi-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(
			[param : Localizable(true)] string columnName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			int cellRow)
			: this(columnName, valueGetter, valueSetter, widthFactor, null, cellRow) {}

		/// <summary>
		/// Constructs a read-only Multi-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="cellRow">The cell row this column will be display in.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, int cellRow)
			: this(columnName, valueGetter, null, 1.0f, null, cellRow) {}

		/// <summary>
		/// Constructs a read-only Multi-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor, int cellRow)
			: this(columnName, valueGetter, null, widthFactor, null, cellRow) {}

		/// <summary>
		/// Constructs a Multi-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter, int cellRow)
			: this(columnName, columnDisplayName, valueGetter, valueSetter, 1.0f, null, cellRow) {}

		/// <summary>
		/// Constructs a Multi-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(
			string columnName, [param : Localizable(true)] string columnDisplayName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			int cellRow)
			: this(columnName, columnDisplayName, valueGetter, valueSetter, widthFactor, null, cellRow) {}

		/// <summary>
		/// Constructs a read-only Multi-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="cellRow">The cell row this column will be display in.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter, int cellRow)
			: this(columnName, columnDisplayName, valueGetter, null, 1.0f, null, cellRow) {}

		/// <summary>
		/// Constructs a read-only Multi-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor, int cellRow)
			: this(columnName, columnDisplayName, valueGetter, null, widthFactor, null, cellRow) {}

		#endregion

		#region Single-CellRow constructors

		/// <summary>
		/// Constructs a single-cellrow table column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		public TableColumn(
			[param : Localizable(true)] string columnName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			Comparison<TItem> comparison)
			: this(columnName, valueGetter, valueSetter, widthFactor, comparison, 0) {}

		/// <summary>
		/// Constructs a single-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter)
			: this(columnName, valueGetter, valueSetter, 1.0f, null) {}

		/// <summary>
		/// Constructs a single-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public TableColumn(
			[param : Localizable(true)] string columnName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor)
			: this(columnName, valueGetter, valueSetter, widthFactor, null) {}

		/// <summary>
		/// Constructs a read-only single-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter)
			: this(columnName, valueGetter, null, 1.0f, null) {}

		/// <summary>
		/// Constructs a read-only single-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public TableColumn([param : Localizable(true)] string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor)
			: this(columnName, valueGetter, null, widthFactor, null) {}

		/// <summary>
		/// Constructs a single-cellrow table column.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		public TableColumn(
			string columnName, [param : Localizable(true)] string columnDisplayName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor,
			Comparison<TItem> comparison)
			: this(columnName, columnDisplayName, valueGetter, valueSetter, widthFactor, comparison, 0) {}

		/// <summary>
		/// Constructs a single-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter)
			: this(columnName, columnDisplayName, valueGetter, valueSetter, 1.0f, null) {}

		/// <summary>
		/// Constructs a single-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public TableColumn(
			string columnName, [param : Localizable(true)] string columnDisplayName,
			GetColumnValueDelegate<TItem, TColumn> valueGetter,
			SetColumnValueDelegate<TItem, TColumn> valueSetter,
			float widthFactor)
			: this(columnName, columnDisplayName, valueGetter, valueSetter, widthFactor, null) {}

		/// <summary>
		/// Constructs a read-only single-cellrow table column with no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter)
			: this(columnName, columnDisplayName, valueGetter, null, 1.0f, null) {}

		/// <summary>
		/// Constructs a read-only single-cellrow table column with specific width factor but no comparison delegate.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		public TableColumn(string columnName, [param : Localizable(true)] string columnDisplayName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor)
			: this(columnName, columnDisplayName, valueGetter, null, widthFactor, null) {}

		#endregion

		/// <summary>
		/// Gets and sets the delegate that executes when the link is clicked.
		/// </summary>
		public Action<TItem> ClickLinkDelegate
		{
			get { return _linkActionDelegate; }
			set { _linkActionDelegate = value; }
		}

		///<summary>
		/// Indicates whether this column is read-only.
		///</summary>
		public override bool ReadOnly
		{
			get { return _valueSetter == null; }
		}

		/// <summary>
		/// Indicates whether this column is clickable
		/// </summary>
		public override bool HasClickableLink
		{
			get { return _linkActionDelegate != null; }
		}

		/// <summary>
		/// Gets or sets the tooltip text provider for this binding.
		/// </summary>
		public Converter<TItem, string> TooltipTextProvider
		{
			get { return _tooltipTextProvider; }
			set { _tooltipTextProvider = value; }
		}

		/// <summary>
		/// Gets the tooltip of this column for the specified item.
		/// </summary>
		public override string GetTooltipText(object item)
		{
			return _tooltipTextProvider == null ? null : _tooltipTextProvider((TItem) item);
		}

		///<summary>
		/// Gets the value of this column for the specified item.
		///</summary>
		///<param name="item">The item from which the value is to be obtained.</param>
		public override object GetValue(object item)
		{
			return _valueGetter((TItem) item);
		}

		///<summary>
		/// Sets the value of this column on the specified item, assuming this is not a read-only column.
		///</summary>
		///<param name="item">The item on which the value is to be set.</param>
		///<param name="value">The value.</param>
		public override void SetValue(object item, object value)
		{
			_valueSetter((TItem) item, (TColumn) value);
		}

		/// <summary>
		/// Gets or sets the value formatter for this binding.
		/// </summary>
		public Converter<TColumn, object> ValueFormatter
		{
			get { return _valueFormatter; }
			set { _valueFormatter = value; }
		}

		/// <summary>
		/// Format the value of this column for the specified item.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The formatted value</returns>
		public override object FormatValue(object value)
		{
			return _valueFormatter((TColumn) value);
		}

		/// <summary>
		/// Sets the click action of this column on the specified item.
		/// </summary>
		/// <param name="item">The item on which the value is to be set.</param>
		public override void ClickLink(object item)
		{
			_linkActionDelegate((TItem) item);
		}
	}
}