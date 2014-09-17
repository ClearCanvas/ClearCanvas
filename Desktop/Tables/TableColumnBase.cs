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
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
	/// <summary>
	/// Abstract base implementation of <see cref="ITableColumn"/> for use with the <see cref="Table"/> class.
	/// </summary>
	/// <remarks>
	/// Application code should use the concrete <see cref="TableColumn{TItem,TColumn}"/> class.
	/// </remarks>
	/// <typeparam name="TItem">The type of item on which the table is based.</typeparam>
	public abstract class TableColumnBase<TItem> : ITableColumn
	{
		/// <summary>
		/// Comparer for sorting operations
		/// </summary>
		private class SortComparer : IComparer
		{
			private readonly int _direction;
			private readonly Comparison<TItem> _comp;

			public SortComparer(Comparison<TItem> comparison, bool ascending)
			{
				_comp = comparison;
				_direction = ascending ? 1 : -1;
			}

			public int Compare(object x, object y)
			{
				return _comp((TItem) x, (TItem) y)*_direction;
			}
		}

		private Table<TItem> _table;
		private readonly string _name;
		private readonly string _displayName;
		private bool _visible = true;
		private readonly Type _columnType;
		private float _widthFactor;
		private readonly int _cellRow;

		private Comparison<TItem> _comparison;
		private Predicate<TItem> _editableHandler;
		private event EventHandler _visibilityChangedEvent;

		private IResourceResolver _resolver;

		private ITableCellEditor _cellEditor;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="columnType">The type of value that the column holds.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		protected TableColumnBase(
			[param : Localizable(true)] string columnName,
			Type columnType,
			float widthFactor,
			Comparison<TItem> comparison)
			: this(columnName, columnType, widthFactor, comparison, 0) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="columnType">The type of value that the column holds.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		protected TableColumnBase(
			string columnName,
			[param : Localizable(true)] string columnDisplayName,
			Type columnType,
			float widthFactor,
			Comparison<TItem> comparison)
			: this(columnName, columnDisplayName, columnType, widthFactor, comparison, 0) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="columnType">The type of value that the column holds.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		protected TableColumnBase(
			[param : Localizable(true)] string columnName,
			Type columnType,
			float widthFactor,
			Comparison<TItem> comparison,
			int cellRow)
			: this(columnName, columnName, columnType, widthFactor, comparison, cellRow) {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="columnName">The identifying name of the column.</param>
		/// <param name="columnDisplayName">The display name of the column.</param>
		/// <param name="columnType">The type of value that the column holds.</param>
		/// <param name="widthFactor">A weighting factor that is applied to the width of the column.</param>
		/// <param name="comparison">A custom comparison operator that is used for sorting based on this column.</param>
		/// <param name="cellRow">The cell row this column will be displayed in.</param>
		protected TableColumnBase(
			string columnName,
			[param : Localizable(true)] string columnDisplayName,
			Type columnType,
			float widthFactor,
			Comparison<TItem> comparison,
			int cellRow)
		{
			_name = columnName;
			_displayName = columnDisplayName;
			_widthFactor = widthFactor;
			_columnType = columnType;
			_comparison = comparison;
			_cellRow = cellRow;

			if (_cellRow > 0)
				this.Visible = false;

			// if no comparison operator was specified, assign a default comparison
			if (_comparison == null)
			{
				// if TColumn implements IComparable, can compare by column value
				if (typeof (IComparable).IsAssignableFrom(_columnType))
					_comparison = ValueComparsion;
				else
					_comparison = NopComparison; // no comparison is possible
			}

			// assign a default resource resolver that looks for resources
			// in the TItem assembly
			if (_resolver == null)
			{
				_resolver = new ApplicationThemeResourceResolver(typeof (TItem).Assembly);
			}
		}

		/// <summary>
		/// Gets or sets the comparison delegate that will be used to sort the table according to this column.
		/// </summary>
		public Comparison<TItem> Comparison
		{
			get { return _comparison; }
			set { _comparison = value; }
		}

		/// <summary>
		/// Gets or sets the delegate that determines whether a given item is editable.
		/// </summary>
		public Predicate<TItem> EditableHandler
		{
			get { return _editableHandler; }
			set { _editableHandler = value; }
		}

		/// <summary>
		/// Gets or sets a custom cell editor for this column.
		/// </summary>
		public ITableCellEditor CellEditor
		{
			get { return _cellEditor; }
			set
			{
				_cellEditor = value;
				_cellEditor.SetColumn(this);
			}
		}

		/// <summary>
		/// Used by the framework to associate this column with a parent <see cref="Table"/>.
		/// </summary>
		internal Table<TItem> Table
		{
			get { return _table; }
			set { _table = value; }
		}

		#region ITableColumn members

		/// <summary>
		/// The identifying name of the column.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// The heading text of the column.
		/// </summary>
		public string DisplayName
		{
			get { return _displayName; }
		}

		/// <summary>
		/// The type of data that the column holds.
		/// </summary>
		public Type ColumnType
		{
			get { return _columnType; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this column is visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				EventsHelper.Fire(_visibilityChangedEvent, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a resource resolver.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resolver; }
			set { _resolver = value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property has changed.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { _visibilityChangedEvent += value; }
			remove { _visibilityChangedEvent -= value; }
		}

		/// <summary>
		/// A factor that influences the width of the column relative to other columns.
		/// </summary>
		/// <remarks>
		/// A value of 1.0 is default.
		/// </remarks>
		public float WidthFactor
		{
			get { return _widthFactor; }
			set
			{
				_widthFactor = value;
				if (_table != null)
				{
					_table.NotifyColumnChanged(this);
				}
			}
		}

		/// <summary>
		/// Gets the width of this column as a percentage of the overall table width.
		/// </summary>
		public int WidthPercent
		{
			get
			{
				if (_table == null)
					throw new InvalidOperationException(SR.ExceptionTableColumnMustBeAddedToDetermineWidth);

				return _table.GetColumnWidthPercent(this);
			}
		}

		/// <summary>
		/// Indicates whether this column is read-only.
		/// </summary>
		public abstract bool ReadOnly { get; }

		/// <summary>
		/// Indicates whether this column is clickable
		/// </summary>
		public abstract bool HasClickableLink { get; }

		///<summary>
		/// Gets the tooltip of this column for the specified item.
		///</summary>
		public abstract string GetTooltipText(object item);

		/// <summary>
		/// Gets the value of this column for the specified item.
		/// </summary>
		/// <param name="item">The item from which the value is to be obtained</param>
		public abstract object GetValue(object item);

		/// <summary>
		/// Sets the value of this column on the specified item, assuming this is not a read-only column.
		/// </summary>
		/// <param name="item">The item on which the value is to be set.</param>
		/// <param name="value">The value.</param>
		public abstract void SetValue(object item, object value);

		/// <summary>
		/// Format the value of this column for the specified item.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The formatted value</returns>
		public abstract object FormatValue(object value);

		/// <summary>
		/// Sets the click action of this column on the specified item.
		/// </summary>
		/// <param name="item">The item on which the value is to be set.</param>
		public abstract void ClickLink(object item);

		/// <summary>
		/// Get a comparer that can be used to sort items in the specified direction.
		/// </summary>
		public IComparer GetComparer(bool ascending)
		{
			return new SortComparer(_comparison, ascending);
		}

		/// <summary>
		/// Gets the cell row for which this column will be displayed in.
		/// </summary>
		public int CellRow
		{
			get { return _cellRow; }
		}

		/// <summary>
		/// Gets the editor that allows cells in this column to be edited, or null if no custom editor is provided.
		/// </summary>
		/// <returns></returns>
		public ITableCellEditor GetCellEditor()
		{
			return _cellEditor;
		}

		/// <summary>
		/// Gets whether or not the specified <paramref name="item"/> can be edited.
		/// </summary>
		public bool IsEditable(object item)
		{
			if (this.ReadOnly)
				return false;

			// assume it is editable if no handler is supplied
			// otherwise, ask the handler
			return _editableHandler == null ? true : _editableHandler((TItem) item);
		}

		#endregion

		/// <summary>
		/// Default comparison used when TColumn is IComparable.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int ValueComparsion(TItem x, TItem y)
		{
			var valueX = GetValue(x);
			var valueY = GetValue(y);
			if (valueX == null)
			{
				return valueY == null ? 0 : -1;
			}
			if (valueY == null)
			{
				return 1;
			}

			return ((IComparable) valueX).CompareTo(valueY);
		}

		/// <summary>
		/// Default comparison used when TColumn is not IComparable (in which case, sorting is not possible).
		/// </summary>
		private static int NopComparison(TItem x, TItem y)
		{
			return 0;
		}
	}
}