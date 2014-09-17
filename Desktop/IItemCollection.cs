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
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumerates the types of item changes.
	/// </summary>
	public enum ItemChangeType
	{
		/// <summary>
		/// An item was added to the table.
		/// </summary>
		ItemAdded,

		/// <summary>
		/// An item was inserted to the table.
		/// </summary>
		ItemInserted,

		/// <summary>
		/// An item in the table was changed.
		/// </summary>
		ItemChanged,

		/// <summary>
		/// An item was removed from the table.
		/// </summary>
		ItemRemoved,

		/// <summary>
		/// All items in the table may have changed.
		/// </summary>
		Reset
	}

	/// <summary>
	/// Provides data for the <see cref="IItemCollection.ItemsChanged"/> event.
	/// </summary>
	public class ItemChangedEventArgs : EventArgs
	{
		private readonly object _item;
		private readonly int _itemIndex;
		private readonly ItemChangeType _changeType;

		internal ItemChangedEventArgs(ItemChangeType changeType, int itemIndex, object item)
		{
			_changeType = changeType;
			_itemIndex = itemIndex;
			_item = item;
		}

		/// <summary>
		/// Gets the type of change that occured.
		/// </summary>
		public ItemChangeType ChangeType
		{
			get { return _changeType; }
		}

		/// <summary>
		/// Gets the index of the item that changed.
		/// </summary>
		public int ItemIndex
		{
			get { return _itemIndex; }
		}

		/// <summary>
		/// Gets the item that has changed.
		/// </summary>
		public object Item
		{
			get { return _item; }
		}
	}

	/// <summary>
	/// Defines the interface to the collection of items.
	/// </summary>
	/// <remarks>
	/// Do not implement this interface.  It is 
	/// not considered part of the public API and is subject to change.
	/// </remarks>
	public interface IItemCollection : IList
	{
		/// <summary>
		/// Occurs when a transaction has started.
		/// </summary>
		event EventHandler TransactionStarted;

		/// <summary>
		/// Occurs when an item in the collection has changed.
		/// </summary>
		event EventHandler<ItemChangedEventArgs> ItemsChanged;

		/// <summary>
		/// Occurs when a transaction has completed.
		/// </summary>
		event EventHandler TransactionCompleted;

		/// <summary>
		/// Adds multiple items to the collection. Returns true if one or more items are successfully added.
		/// </summary>
		bool AddRange(IEnumerable items);

		/// <summary>
		/// Removes multiple items from the collection. Returns true if one or more items are successfully removed.
		/// </summary>
		bool RemoveRange(IEnumerable items);
	}

	/// <summary>
	/// Defines the interface to the collection of items.
	/// </summary>
	/// <remarks>
	/// Do not implement this interface.  It is likely to be removed
	/// in subsequent versions of the framework and is 
	/// not considered part of the public API.
	/// </remarks>
	/// <typeparam name="TItem">The item type.</typeparam>
	public interface IItemCollection<TItem> : IItemCollection, IList<TItem>
	{
		/// <summary>
		/// Adds multiple items to the collection.
		/// </summary>
		void AddRange(IEnumerable<TItem> items);

		/// <summary>
		/// Removes multiple items from the collection. Returns true if one or more items are successfully removed.
		/// </summary>
		bool RemoveRange(IEnumerable<TItem> items);
	}
}