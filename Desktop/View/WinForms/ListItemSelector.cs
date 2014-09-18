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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class ListItemSelector : UserControl
	{
		#region Private Members

		private ITable _availableItemsTable = null;
		private ITable _selectedItemsTable = null;

		private event EventHandler _itemAdded;
		private event EventHandler _itemRemoved;

		private bool _readOnly;

		#endregion

		#region Constructor

		public ListItemSelector()
		{
			InitializeComponent();

			_availableItems.DataGridKeyDown += new KeyEventHandler(_availableItems_DataGridKeyDown);
			_selectedItems.DataGridKeyDown += new KeyEventHandler(_selectedItems_DataGridKeyDown);
		}

		#endregion

		public void _availableItems_DataGridKeyDown(object sender, KeyEventArgs args)
		{
			if (args.KeyCode.Equals(Keys.Enter))
			{
				args.Handled = true;
				AddSelection(sender, args);
			}
		}

		public void _selectedItems_DataGridKeyDown(object sender, KeyEventArgs args)
		{
			if (args.KeyCode.Equals(Keys.Delete))
			{
				args.Handled = true;
				RemoveSelection(sender, args);
			}
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets whether or not the component is read only.  When true, the add item and remove item buttons are disabled.
		/// </summary>
		public bool ReadOnly
		{
			get { return _readOnly; }
			set
			{
				_readOnly = value;
				_addItemButton.Enabled = !value;
				_removeItemButton.Enabled = !value;
			}
		}

		/// <summary>
		/// Indicates if toolbars should be displayed
		/// </summary>
		/// <remarks>
		/// Value is applied to both the Available list and the selected List
		/// </remarks>
		[DefaultValue(true)]
		public bool ShowToolbars
		{
			get { return _availableItems.ShowToolbar; }
			set
			{
				_availableItems.ShowToolbar = value;
				_selectedItems.ShowToolbar = value;
			}
		}

		/// <summary>
		/// Indicates if column headings should be displayed
		/// </summary>
		/// <remarks>
		/// Value is applied to both the Available list and the selected List
		/// </remarks>
		[DefaultValue(true)]
		public bool ShowColumnHeading
		{
			get { return _availableItems.ShowColumnHeading; }
			set
			{
				_availableItems.ShowColumnHeading = value;
				_selectedItems.ShowColumnHeading = value;
			}
		}

		public ITable AvailableItemsTable
		{
			get { return _availableItemsTable; }
			set
			{
				_availableItemsTable = value;
				_availableItems.Table = _availableItemsTable;

				if (_availableItemsTable != null)
				{
					_availableItemsTable.Sort();
				}
			}
		}

		public ITable SelectedItemsTable
		{
			get { return _selectedItemsTable; }
			set
			{
				_selectedItemsTable = value;
				_selectedItems.Table = _selectedItemsTable;

				if (_selectedItemsTable != null) _selectedItemsTable.Sort();
			}
		}

		public void OnAvailableItemsChanged(object sender, EventArgs args)
		{
			if (_availableItemsTable.IsFiltered) _availableItemsTable.Filter();
			_availableItems.Table = _availableItemsTable;
		}

		public void AppendToSelectedItemsActionModel(ActionModelNode model)
		{
			AppendActionModel(_selectedItems, model);
		}

		public void AppendToAvailableItemsActionModel(ActionModelNode model)
		{
			AppendActionModel(_availableItems, model);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISelection SelectedItemsTableSelection
		{
			get { return _selectedItems.Selection; }
			set { _selectedItems.Selection = value; }
		}

		public event EventHandler SelectedItemsTableSelectionChanged
		{
			add { _selectedItems.SelectionChanged += value; }
			remove { _selectedItems.SelectionChanged -= value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISelection AvailableItemsTableSelection
		{
			get { return _availableItems.Selection; }
			set { _availableItems.Selection = value; }
		}

		public event EventHandler AvailableItemsTableSelectionChanged
		{
			add { _availableItems.SelectionChanged += value; }
			remove { _availableItems.SelectionChanged -= value; }
		}

		public event KeyEventHandler AvailableItemsKeyDown
		{
			add { _availableItems.DataGridKeyDown += value; }
			remove { _availableItems.DataGridKeyDown -= value; }
		}

		public event KeyEventHandler SelectedItemsKeyDown
		{
			add { _selectedItems.DataGridKeyDown += value; }
			remove { _selectedItems.DataGridKeyDown -= value; }
		}

		#endregion

		#region Public Events

		public event EventHandler ItemAdded
		{
			add { _itemAdded += value; }
			remove { _itemAdded -= value; }
		}

		public event EventHandler ItemRemoved
		{
			add { _itemRemoved += value; }
			remove { _itemRemoved -= value; }
		}

		#endregion

		#region Private Methods

		private void AddSelection(object sender, EventArgs e)
		{
			if (_readOnly)
				return;

			int oldFirstDisplayedScrollingRowIndex = 0;
			if (_availableItems.Table.Items.Count != 0)
				oldFirstDisplayedScrollingRowIndex = _availableItems.FirstDisplayedScrollingRowIndex;

			ISelection selection = _availableItems.Selection;
			ISelection originalSelectedSelection = _selectedItems.Selection;
			ISelection availableSelectionAfterRemove = GetSelectionAfterRemove(_availableItemsTable, selection);

			foreach (object item in selection.Items)
			{
				_selectedItemsTable.Items.Add(item);
				_availableItemsTable.Items.Remove(item);

				// BUG 12483: the Table{T} class is so broken when filtering is used in conjunction a mutable collection that rebinding after *each* change is absolutely required
				_selectedItems.Table = _selectedItemsTable;
				_availableItems.Table = _availableItemsTable;
			}

			_selectedItemsTable.Sort();

			_selectedItems.Table = _selectedItemsTable;
			_availableItems.Table = _availableItemsTable;

			_selectedItems.Selection = originalSelectedSelection;
			_availableItems.Selection = availableSelectionAfterRemove;

			if (oldFirstDisplayedScrollingRowIndex != 0 && _availableItems.Table.Items.Count != 0)
				_availableItems.FirstDisplayedScrollingRowIndex = oldFirstDisplayedScrollingRowIndex;

			EventsHelper.Fire(_itemAdded, this, EventArgs.Empty);
		}

		private void RemoveSelection(object sender, EventArgs e)
		{
			if (_readOnly)
				return;

			int oldFirstDisplayedScrollingRowIndex = 0;
			if (_selectedItems.Table.Items.Count != 0)
				oldFirstDisplayedScrollingRowIndex = _selectedItems.FirstDisplayedScrollingRowIndex;

			ISelection selection = _selectedItems.Selection;
			ISelection originalAvailableSelection = _availableItems.Selection;
			ISelection selectedSelectionAfterRemove = GetSelectionAfterRemove(_selectedItemsTable, selection);

			foreach (object item in selection.Items)
			{
				_selectedItemsTable.Items.Remove(item);
				_availableItemsTable.Items.Add(item);

				// BUG 12483: the Table{T} class is so broken when filtering is used in conjunction a mutable collection that rebinding after *each* change is absolutely required
				_selectedItems.Table = _selectedItemsTable;
				_availableItems.Table = _availableItemsTable;
			}

			_availableItemsTable.Sort();

			_selectedItems.Table = _selectedItemsTable;
			_availableItems.Table = _availableItemsTable;

			_availableItems.Selection = originalAvailableSelection;
			_selectedItems.Selection = selectedSelectionAfterRemove;

			if (oldFirstDisplayedScrollingRowIndex != 0 && _selectedItems.Table.Items.Count != 0)
				_selectedItems.FirstDisplayedScrollingRowIndex = oldFirstDisplayedScrollingRowIndex;

			EventsHelper.Fire(_itemRemoved, this, EventArgs.Empty);
		}

		private ISelection GetSelectionAfterRemove(ITable table, ISelection selectionToBeRemoved)
		{
			// if nothing is selected, next selection should be empty too
			if (selectionToBeRemoved.Items.Length == 0)
				return new Selection();

			List<int> toBeRemovedIndices = CollectionUtils.Map<object, int>(selectionToBeRemoved.Items,
			                                                                delegate(object item) { return table.Items.IndexOf(item); });
			toBeRemovedIndices.Sort();

			// use the first unselected item between the to-be-removed items as the next selection
			foreach (int index in toBeRemovedIndices)
			{
				if (index < table.Items.Count - 1 &&
				    toBeRemovedIndices.Contains(index + 1) == false)
				{
					return new Selection(table.Items[index + 1]);
				}
			}

			// if there is no item between all the to-be-removed items
			// use the item after the last to-be-removed item as the next selection
			int lastToBeRemovedIndex = toBeRemovedIndices[toBeRemovedIndices.Count - 1];
			if (lastToBeRemovedIndex < table.Items.Count - 1)
				return new Selection(table.Items[lastToBeRemovedIndex + 1]);

			// otherwise, use the item before the first to-be-removed item as the next selection
			if (toBeRemovedIndices[0] > 0)
				return new Selection(table.Items[toBeRemovedIndices[0] - 1]);

			// empty selection
			return new Selection();
		}

		private void AppendActionModel(TableView table, ActionModelNode model)
		{
			if (table.ToolbarModel == null)
				table.ToolbarModel = model;
			else
				table.ToolbarModel.Merge(model);

			if (table.MenuModel == null)
				table.MenuModel = model;
			else
				table.MenuModel.Merge(model);
		}

		#endregion
	}
}