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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A dictionary class, changes to which can be observed via events.
	/// </summary>
	/// <remarks>
	/// Internally, a simple <see cref="Dictionary{TKey,TValue}"/> object is used.  
	/// For exception details on individual methods, see <see cref="Dictionary{TKey,TValue}"/>.
	/// </remarks>
	/// <typeparam name="TKey">The type of the key in the dictionary.</typeparam>
	/// <typeparam name="TItem">The type of the items stored in the dictionary.</typeparam>
	public class ObservableDictionary<TKey, TItem> : IDictionary<TKey, TItem>
	{
		private readonly Dictionary<TKey, TItem> _dictionary;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemAddedEvent;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemChanging;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemChanged;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemRemovedEvent;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ObservableDictionary()
		{
			_dictionary = new Dictionary<TKey, TItem>();
		}

		/// <summary>
		/// Copy constructor that accepts a set of key-value pairs as input.
		/// </summary>
		public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TItem>> other)
			: this()
		{
			foreach (KeyValuePair<TKey, TItem> pair in other)
				this.Add(pair);
		}

		/// <summary>
		/// Fired when an item is added to the dictionary.
		/// </summary>
		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value; }
		}

		/// <summary>
		/// Fired when an item is removed from the dictionary.
		/// </summary>
		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		/// <summary>
		/// Fired when an item in the dictionary is changing.
		/// </summary>
		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemChanging
		{
			add { _itemChanging += value; }
			remove { _itemChanging -= value; }
		}

		/// <summary>
		/// Fired when an item in the dictionary has changed.
		/// </summary>
		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemChanged
		{
			add { _itemChanged += value; }
			remove { _itemChanged -= value; }
		}
		
		#region IDictionary<TKey,TItem> Members

		/// <summary>
		/// Adds a key-value pair to the dictionary.
		/// </summary>
		/// <param name="key">The key at which to add the <paramref name="value"/>.</param>
		/// <param name="value">The value to be added to the dictionary.</param>
		public void Add(TKey key, TItem value)
		{
			_dictionary.Add(key, value);
			OnItemAdded(new DictionaryEventArgs<TKey, TItem>(key, value));
		}

		/// <summary>
		/// Gets whether or not the dictionary contains a particular key.
		/// </summary>
		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Gets all of the keys in the dictionary.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}

		/// <summary>
		/// Removes an item from the dictionary stored using the specified <paramref name="key"/>.
		/// </summary>
		/// <returns>
		/// True if an object existed for the given <param name="key" /> and was removed.
		/// </returns>
		public bool Remove(TKey key)
		{
			if (_dictionary.ContainsKey(key))
			{
				DictionaryEventArgs<TKey, TItem> args = new DictionaryEventArgs<TKey, TItem>(key, _dictionary[key]);
				_dictionary.Remove(key);
				OnItemRemoved(args);
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Tries to get a value stored with the specified <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The key at which to try and get the <paramref name="value"/>.</param>
		/// <param name="value">Returns the value stored at <paramref name="key"/>.</param>
		/// <returns>True if a value exists for the given key.</returns>
		public bool TryGetValue(TKey key, out TItem value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets all of the values in the dictionary.
		/// </summary>
		public ICollection<TItem> Values
		{
			get { return _dictionary.Values; }
		}

		/// <summary>
		/// Indexer; gets the value given a <paramref name="key"/>.
		/// </summary>
		public TItem this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				if (_dictionary.ContainsKey(key))
				{
					DictionaryEventArgs<TKey, TItem> args = new DictionaryEventArgs<TKey, TItem>(key, _dictionary[key]);
					OnItemChanging(args);

					_dictionary[key] = value;

					args = new DictionaryEventArgs<TKey, TItem>(key, value);
					OnItemChanged(args);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TItem>> Members

		/// <summary>
		/// Adds a key-value pair to the dictionary.
		/// </summary>
		public void Add(KeyValuePair<TKey, TItem> item)
		{
			(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Add(item);
			OnItemAdded(new DictionaryEventArgs<TKey, TItem>(item.Key, item.Value));
		}

		/// <summary>
		/// Clears the dictionary.
		/// </summary>
		public void Clear()
		{
			// If we don't have any subscribers to the ItemRemovedEvent, then
			// make it faster and just call Clear().
			if (_itemRemovedEvent == null)
			{
				_dictionary.Clear();
			}
			// But if we do, then remove items one by one...a little tricky with a dictionary.
			else
			{
				// Normally, if we want to remove all the elements in an indexed collection 
				// (like an array or list) one by one, we iterate backwards through it, removing
				// the last item during each iteration.  But, since there's no concept of
				// an index in a dictionary, we can't do that.  And we can't "foreach" through
				// the dictionary either, since foreach requires that we not be changing
				// the collection while we're iterating (which we'd be doing, since we'd
				// be removing an item each itereation).  So, instead, we copy the dictionary
				// (expensive, I know) into a collection, then iterate through that using
				// the keys as indices into the dictionary, removing items one at a time.

				KeyValuePair<TKey, TItem>[] pairs = new KeyValuePair<TKey, TItem>[_dictionary.Count];
				(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).CopyTo(pairs, 0);

				foreach (KeyValuePair<TKey, TItem> pair in pairs)
					Remove(pair.Key);
			}
		}

		/// <summary>
		/// Gets whether the input key-value pair exists in the dictionary.
		/// </summary>
		public bool Contains(KeyValuePair<TKey, TItem> item)
		{
			return (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Contains(item);
		}

		/// <summary>
		/// Copies the entire contents of the dictionary to an array of key-value pairs, starting at the specified index.
		/// </summary>
		/// <param name="array">The array to copy the contents of the dictionary to.</param>
		/// <param name="arrayIndex">The index in the <paramref name="array"/> at which to begin copying.</param>
		public void CopyTo(KeyValuePair<TKey, TItem>[] array, int arrayIndex)
		{
			(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the number of entries in the dictionary.
		/// </summary>
		public int Count
		{
			get { return _dictionary.Count; }
		}

		/// <summary>
		/// Gets whether or not the dictionary is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).IsReadOnly; }
		}

		/// <summary>
		/// Removes a particular key-value pair from the dictionary.
		/// </summary>
		/// <returns>True if an item was actually removed.</returns>
		public bool Remove(KeyValuePair<TKey, TItem> item)
		{
			bool result = (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Remove(item);

			// Only raise event if the item was actually removed
			if (result == true)
				OnItemRemoved(new DictionaryEventArgs<TKey, TItem>(item.Key, item.Value));

			return result;
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TItem>> Members

		/// <summary>
		/// Gets an enumerator for the dictionary.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TItem>> GetEnumerator()
		{
			return (_dictionary as IEnumerable<KeyValuePair<TKey, TItem>>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Gets an enumerator for the dictionary.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Called internally when an item is added to the dictionary.
		/// </summary>
		protected virtual void OnItemAdded(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemAddedEvent, this, e);
		}

		/// <summary>
		/// Called internally when an item is removed from the dictionary.
		/// </summary>
		protected virtual void OnItemRemoved(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemRemovedEvent, this, e);
		}

		/// <summary>
		/// Called internally when an item in the dictionary is changing.
		/// </summary>
		protected virtual void OnItemChanging(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemChanging, this, e);
		}

		/// <summary>
		/// Called internally when an item in the dictionary has changed.
		/// </summary>
		protected virtual void OnItemChanged(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemChanged, this, e);
		}
	}
}
