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
using System.Linq;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Default implementation of <see cref="IItemCollection{TItem}"/>.
	/// </summary>
	/// <remarks>
	/// Do not subclass this class.  It is likely to be removed
	/// in subsequent versions of the framework and is 
	/// not considered part of the public API.
	/// </remarks>
	///<typeparam name="TItem">The type of item that the table holds.</typeparam>
	public class ItemCollection<TItem> : IItemCollection<TItem>
	{
		public class TransactionScope : IDisposable
		{
			private readonly ItemCollection<TItem> _collection;
			private bool _disposed;

			internal TransactionScope(ItemCollection<TItem> collection)
			{
				_collection = collection;
			}

			public void Dispose()
			{
				if (_disposed)
					throw new InvalidOperationException("Already disposed.");

				_disposed = true;
				_collection.EndTransactionScope(this);
			}
		}

		private class ComparisonComparer<T> : IComparer<T>
		{
			private readonly Comparison<T> _comparison;

			internal ComparisonComparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			public int Compare(T x, T y)
			{
				return _comparison(x, y);
			}
		}

		private readonly object _syncRoot = new object();
		private readonly List<TItem> _list;
		private event EventHandler<ItemChangedEventArgs> _itemsChanged;
		private TransactionScope _rootTransaction;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ItemCollection()
		{
			_list = new List<TItem>();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		protected ItemCollection(ItemCollection<TItem> itemCollection)
		{
			_list = itemCollection.List;
		}

		/// <summary>
		/// Begins a transaction, returning a scope object that must be disposed to denote
		/// the end of the transaction.
		/// </summary>
		/// <remarks>
		/// This is not a transaction in the true sense, because it cannot be rolled back.
		/// The purpose of the transaction is to allow objects that are listening for
		/// <see cref="ItemsChanged"/> events to know that a number of changes are being made
		/// in close succession that ought to be considered part of a single logical change. 
		/// </remarks>
		/// <returns></returns>
		public TransactionScope BeginTransaction()
		{
			var transaction = new TransactionScope(this);
			if (_rootTransaction == null)
			{
				// we only fire the TransactionStarted event if we're the root transaction
				// (nested transactions are irrelevant)
				_rootTransaction = transaction;
				EventsHelper.Fire(TransactionStarted, this, EventArgs.Empty);
			}
			return transaction;
		}

		/// <summary>
		/// Finds the appropriate insertion point for the specified item, given that the collection is sorted according
		/// to the specified comparison.
		/// </summary>
		/// <param name="item">The item for which to determine the insertion point.</param>
		/// <param name="comparison">The comparison by which the collection is sorted.</param>
		/// <returns>A positive integer indicating the appropriate insertion point.</returns>
		/// <remarks>
		/// Assuming the item collection is already sorted by the specified comparison, this method
		/// will locate the correct insertion point for the specified item using a binary search.
		/// This index value can then be passed to the <see cref="Insert(int, TItem)"/> method.
		/// </remarks>
		public int FindInsertionPoint(TItem item, Comparison<TItem> comparison)
		{
			var comparer = new ComparisonComparer<TItem>(comparison);
			var i = _list.BinarySearch(item, comparer);
			return i >= 0 ? i : ~i;
		}

		#region IItemCollection Members

		/// <summary>
		/// Notifies the table that the item at the specified index has changed in some way.
		/// </summary>
		/// <remarks>
		/// Use this method to cause the view to update itself to reflect the changed item.
		/// </remarks>
		public void NotifyItemUpdated(int index)
		{
			ChangeCollection(() => new ItemChangedEventArgs(ItemChangeType.ItemChanged, index, this[index]));
		}

		/// <summary>
		/// Occurs when the collection is about to change.
		/// </summary>
		public event EventHandler TransactionStarted;

		/// <summary>
		/// Occurs when an item in the collection has changed.
		/// </summary>
		public event EventHandler<ItemChangedEventArgs> ItemsChanged
		{
			add { _itemsChanged += value; }
			remove { _itemsChanged -= value; }
		}

		/// <summary>
		/// Occurs after the collection has changed.
		/// </summary>
		public event EventHandler TransactionCompleted;

		#endregion

		#region IItemCollection<TItem> Members

		/// <summary>
		/// Notifies the table that the specified item has changed in some way.
		/// </summary>
		/// <remarks>
		/// Use this method to cause the view to update itself to reflect the changed item.
		/// </remarks>
		public void NotifyItemUpdated(TItem item)
		{
			int index = this.IndexOf(item);
			if (index > -1)
			{
				NotifyItemUpdated(index);
			}
			else
			{
				throw new ArgumentException(SR.ExceptionTableItemNotFoundInCollection);
			}
		}

		/// <summary>
		/// Adds all items in the specified enumeration.
		/// </summary>
		public virtual void AddRange(IEnumerable<TItem> enumerable)
		{
			// optimize degenerate cases
			var items = enumerable.ToList(); // see if we have 0, 1, or more elements
			if (items.Count == 0)
				return;
			if (items.Count == 1)
			{
				this.Add(items[0]);
				return;
			}

			ChangeCollection(() =>
			                 {
				                 _list.AddRange(items);
				                 return new ItemChangedEventArgs(ItemChangeType.Reset, -1, default(TItem));
			                 });
		}

		/// <summary>
		/// Removes all items in the specified enumeration.
		/// </summary>
		public virtual bool RemoveRange(IEnumerable<TItem> enumerable)
		{
			// optimize degenerate cases
			var items = enumerable.Where(_list.Contains).ToList(); // see if we have 0, 1, or more elements
			if (items.Count == 0)
				return false;
			if (items.Count == 1)
				return this.Remove(items[0]);

			ChangeCollection(() =>
			                 {
				                 foreach (var item in items)
					                 _list.Remove(item);
				                 return new ItemChangedEventArgs(ItemChangeType.Reset, -1, default(TItem));
			                 });
			return true;
		}

		/// <summary>
		/// Sorts items in the collection using the specified <see cref="IComparer{TItem}"/>.
		/// </summary>
		public virtual void Sort(IComparer<TItem> comparer)
		{
			ChangeCollection(() =>
			                 {
				                 // We don't call _list.Sort(...) because .NET internally uses an unstable sort
				                 MergeSort(comparer, _list, 0, _list.Count);

				                 // notify that the list has been sorted
				                 return new ItemChangedEventArgs(ItemChangeType.Reset, -1, default(TItem));
			                 });
		}

		/// <summary>
		/// Sets any items in the collection matching the specified constraint to the specified new value. 
		/// </summary>
		/// <param name="constraint">A predicate against which all items in the collection will be compared, and replaced with the new value.</param>
		/// <param name="newValue">The new value with which to replace all matching items in the collection.</param>
		public virtual void Replace(Predicate<TItem> constraint, TItem newValue)
		{
			// bug #10550: capture the set of indices that require updating, prior to updating any entries
			var indices = (from i in Enumerable.Range(0, this.Count)
			               where constraint(_list[i])
			               select i).ToList();

			foreach (var i in indices)
			{
				// this assignment will automatically fire a notification event
				this[i] = newValue;
			}
		}

		/// <summary>
		/// Searches the collection for an item that satisfies the specified constraint and returns
		/// the index of the first such item.
		/// </summary>
		/// <returns>The index of the first matching item, or -1 if no matching items are found.</returns>
		public virtual int FindIndex(Predicate<TItem> constraint)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (constraint(_list[i]))
					return i;
			}
			return -1;
		}

		#endregion

		#region IList Members

		public virtual bool AddRange(IEnumerable enumerable)
		{
			var items = enumerable.OfType<TItem>().ToList();
			if (items.Any())
			{
				AddRange(items);
				return true;
			}
			return false;
		}

		public virtual bool RemoveRange(IEnumerable enumerable)
		{
			var items = enumerable.OfType<TItem>().ToList();
			if (items.Any())
			{
				return RemoveRange(items);
			}
			return false;
		}

		/// <summary>
		/// Adds an item to the collection, returning the index of the item's position in the collection.
		/// </summary>
		/// <remarks>
		/// The method returns -1 if the item is not of the correct type.
		/// </remarks>
		public virtual int Add(object value)
		{
			if (value is TItem)
			{
				Add((TItem) value);
				return IndexOf((TItem) value);
			}
			else return -1;
		}

		/// <summary>
		/// Gets whether or not the item is in the collection.
		/// </summary>
		public virtual bool Contains(object value)
		{
			if (value is TItem)
			{
				return Contains((TItem) value);
			}
			else return false;
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public virtual void Clear()
		{
			ChangeCollection(() =>
			                 {
				                 _list.Clear();
				                 return new ItemChangedEventArgs(ItemChangeType.Reset, -1, default(TItem));
			                 });
		}

		/// <summary>
		/// Gets the index of the item in the collection, or -1 if it doesn't exist.
		/// </summary>
		public virtual int IndexOf(object value)
		{
			if (value is TItem)
			{
				return IndexOf((TItem) value);
			}
			else return -1;
		}

		/// <summary>
		/// Inserts the specified item at the given index.
		/// </summary>
		public virtual void Insert(int index, object value)
		{
			if (value is TItem)
			{
				Insert(index, (TItem) value);
			}
		}

		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
		public virtual void Remove(object value)
		{
			if (value is TItem)
			{
				Remove((TItem) value);
			}
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		public virtual void RemoveAt(int index)
		{
			ChangeCollection(() =>
			                 {
				                 var item = _list[index];
				                 _list.RemoveAt(index);
				                 return new ItemChangedEventArgs(ItemChangeType.ItemRemoved, index, item);
			                 });
		}

		/// <summary>
		/// gets the item at the specified index.
		/// </summary>
		object IList.this[int index]
		{
			get { return ((IList<TItem>) this)[index]; }
			set { ((IList<TItem>) this)[index] = (TItem) value; }
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region IList<TItem> Members

		/// <summary>
		/// Gets the index of the specified item, or -1 if it doesn't exist.
		/// </summary>
		public virtual int IndexOf(TItem item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// Inserts the specified item at the given index.
		/// </summary>
		public virtual void Insert(int index, TItem item)
		{
			ChangeCollection(() =>
			                 {
				                 _list.Insert(index, item);
				                 return new ItemChangedEventArgs(ItemChangeType.ItemInserted, index, item);
			                 });
		}

		/// <summary>
		/// Gets the item at the given index.
		/// </summary>
		public virtual TItem this[int index]
		{
			get { return _list[index]; }
			set
			{
				ChangeCollection(() =>
				                 {
					                 _list[index] = value;
					                 return new ItemChangedEventArgs(ItemChangeType.ItemChanged, index, value);
				                 });
			}
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// Copies the entire contents of the collection to <paramref name="array"/>, starting at <paramref name="index"/>.
		/// </summary>
		public virtual void CopyTo(Array array, int index)
		{
			if (array is TItem[])
			{
				CopyTo((TItem[]) array, index);
			}
		}

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public virtual int Count
		{
			get { return _list.Count; }
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual object SyncRoot
		{
			get { return _syncRoot; }
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region ICollection<TItem> Members

		/// <summary>
		/// Adds the specified item to the collection.
		/// </summary>
		public virtual void Add(TItem item)
		{
			ChangeCollection(() =>
			                 {
				                 _list.Add(item);
				                 return new ItemChangedEventArgs(ItemChangeType.ItemAdded, this.Count - 1, item);
			                 });
		}

		/// <summary>
		/// Gets whether or not the collection contains the specified item.
		/// </summary>
		public virtual bool Contains(TItem item)
		{
			return _list.Contains(item);
		}

		/// <summary>
		/// Copies the entire contents of the collection to the specified <paramref name="array"/>, starting
		/// at the given <paramref name="arrayIndex"/>.
		/// </summary>
		public virtual void CopyTo(TItem[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
		/// <returns>True if the item existed in the collection and was removed, otherwise false.</returns>
		public virtual bool Remove(TItem item)
		{
			var index = IndexOf(item);
			if (index < 0)
				return false;

			ChangeCollection(() =>
			                 {
				                 _list.Remove(item);
				                 return new ItemChangedEventArgs(ItemChangeType.ItemRemoved, index, item);
			                 });
			return true;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gets an <see cref="IEnumerator"/> for the collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TItem>) this).GetEnumerator();
		}

		#endregion

		#region IEnumerable<TItem> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{TItem}"/> for the collection.
		/// </summary>
		public virtual IEnumerator<TItem> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Gets the internal list of items.
		/// </summary>
		protected List<TItem> List
		{
			get { return _list; }
		}

		/// <summary>
		/// Wraps code blocks that modify the collection, ensuring the proper
		/// event notifications are made.
		/// </summary>
		/// <param name="action"></param>
		protected void ChangeCollection(Func<ItemChangedEventArgs> action)
		{
			// always use a transaction!
			using (BeginTransaction())
			{
				var args = action();
				EventsHelper.Fire(_itemsChanged, this, args);
			}
		}

		private void EndTransactionScope(TransactionScope transaction)
		{
			if (_rootTransaction == transaction)
			{
				// we only fire the TransactionCompleted event if we're the root transaction
				// (nested transactions are irrelevant)
				_rootTransaction = null;
				EventsHelper.Fire(TransactionCompleted, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Raises the <see cref="ItemsChanged"/> event.
		/// </summary>
		protected virtual void NotifyItemsChanged(ItemChangeType itemChangeType, int index, TItem item)
		{
			EventsHelper.Fire(_itemsChanged, this, new ItemChangedEventArgs(itemChangeType, index, item));
		}

		#region Stable Sort Implementation

		/// <summary>
		/// Performs a stable merge sort on the given <paramref name="list"/> using the given <paramref name="comparer"/>.
		/// The range of items sorted is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
		/// </summary>
		private static void MergeSort(IComparer<TItem> comparer, IList<TItem> list, int rangeStart, int rangeStop)
		{
			int rangeLength = rangeStop - rangeStart;
			if (rangeLength > 1)
			{
				// sort halves
				int rangeMid = rangeStart + rangeLength/2;
				MergeSort(comparer, list, rangeStart, rangeMid);
				MergeSort(comparer, list, rangeMid, rangeStop);

				// merge halves
				List<TItem> merged = new List<TItem>(rangeLength);
				int j = rangeStart;
				int k = rangeMid;

				for (int n = 0; n < rangeLength; n++)
				{
					// if left half has run out of items, add item from right half
					// else if right half has run out of items, add item from left half
					// else if the left item is before or equal to the right item, add left half item
					// else add right half item
					if (k >= rangeStop || (j < rangeMid && comparer.Compare(list[j], list[k]) <= 0))
						merged.Add(list[j++]);
					else
						merged.Add(list[k++]);
				}

				// copy merged to list
				k = rangeStart;
				foreach (TItem item in merged)
					list[k++] = item;
			}
		}

		#endregion
	}
}