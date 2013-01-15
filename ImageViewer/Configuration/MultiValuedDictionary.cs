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

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Represents a generic collection of 1-to-n key-item mappings.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TItem">The type of items in the dictionary.</typeparam>
	internal interface IMultiValuedDictionary<TKey, TItem> : IEnumerable<KeyValuePair<TKey, IEnumerable<TItem>>>
	{
		/// <summary>
		/// Gets an <see cref="IList{T}"/> of items mapped to the specified <paramref name="key"/>.
		/// </summary>
		/// <remarks>
		/// This interface does not specify whether or not implementations of this property must make the resulting lists
		/// static copies and, if not a static copy, whether or not the list should be read-only indepdendent of the
		/// dictionary's <see cref="IsReadOnly"/> property.
		/// </remarks>
		/// <param name="key">The key for which the list of mapped items is to be retrieved.</param>
		/// <returns>Returns a list of items mapped to the specified key. If no items are mapped to the specified key, this returns an empty list.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		IList<TItem> this[TKey key] { get; }

		/// <summary>
		/// Gets the total number of mapped items the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Accessing this property yields the same result as the <see cref="ICollection{T}.Count"/> property on <see cref="Items"/>.
		/// </remarks>
		int Count { get; }

		/// <summary>
		/// Gets an <see cref="ICollection{T}"/> of unique keys in the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		ICollection<TKey> Keys { get; }

		/// <summary>
		/// Gets an <see cref="ICollection{T}"/> of mapped items in the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		ICollection<TItem> Items { get; }

		/// <summary>
		/// Gets a value indicating whether or not the <see cref="IMultiValuedDictionary{TKey,TItem}"/> is read-only and thus immutable.
		/// </summary>
		bool IsReadOnly { get; }

		/// <summary>
		/// Adds a mapping for the provided item and not-necessarily unique key to the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key to which the item should be mapped.</param>
		/// <param name="item">The item to be mapped.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		/// <exception cref="NotSupportedException">Thrown if the <see cref="IMultiValuedDictionary{TKey,TItem}"/> is read-only.</exception>
		void Add(TKey key, TItem item);

		/// <summary>
		/// Removes a mapping for the provided item and key from the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key of the mapped item to be removed.</param>
		/// <param name="item">The mapped item to be removed.</param>
		/// <returns>True if the item mapping was removed; False if no mapping for the specified key and item existed, and thus no mapping was removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		/// <exception cref="NotSupportedException">Thrown if the <see cref="IMultiValuedDictionary{TKey,TItem}"/> is read-only.</exception>
		bool Remove(TKey key, TItem item);

		/// <summary>
		/// Removes all mapped items to the specified key from the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key for which all mapped items should be removed.</param>
		/// <returns>True if mappings were removed; False if no items were mapped to the provided key, and thus no mappings were removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		/// <exception cref="NotSupportedException">Thrown if the <see cref="IMultiValuedDictionary{TKey,TItem}"/> is read-only.</exception>
		bool RemoveAll(TKey key);

		/// <summary>
		/// Clears all mapped items in the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <exception cref="NotSupportedException">Thrown if the <see cref="IMultiValuedDictionary{TKey,TItem}"/> is read-only.</exception>
		void Clear();

		/// <summary>
		/// Checks for the presence of any items mapped to the specified key in the <see cref="IMultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key for which to check for the presence of mapped items.</param>
		/// <returns>True if there is at least one item mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		bool ContainsKey(TKey key);

		/// <summary>
		/// Copies the mappings of the <see cref="IMultiValuedDictionary{TKey,TItem}"/> to an array of <see cref="KeyValuePair{TKey,TValue}"/>s.
		/// </summary>
		/// <param name="array">The destination array to which the mappings are to be copied.</param>
		/// <param name="arrayIndex">The index in the destination array at which to start copying.</param>
		void CopyTo(KeyValuePair<TKey, IEnumerable<TItem>>[] array, int arrayIndex);

		/// <summary>
		/// Gets an <see cref="IEnumerable{T}"/> of items mapped to the specified key.
		/// </summary>
		/// <param name="key">The key for which the enumerable of mapped items is to be retrieved.</param>
		/// <param name="item">The retrieved enumerable of items mapped to the specified key. If no items were mapped to the specified key, this returns an empty enumerable.</param>
		/// <returns>True if there were items mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		bool TryGetItems(TKey key, out IEnumerable<TItem> item);
	}

	/// <summary>
	/// Represents a generic collection of 1-to-n key-item mappings.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TItem">The type of items in the dictionary.</typeparam>
	internal class MultiValuedDictionary<TKey, TItem> : IMultiValuedDictionary<TKey, TItem>, IDictionary<TKey, IEnumerable<TItem>>
	{
		private readonly Dictionary<TKey, List<TItem>> _dictionary;
		private readonly KeyCollection _keyCollection;
		private readonly ItemCollection _itemCollection;
		private readonly ItemListCollecion _itemListCollection;

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that is empty, has the
		/// default initial capacity, and uses the default equality comparer for the <typeparamref name="TKey"/>.
		/// </summary>
		public MultiValuedDictionary()
			: this((IEqualityComparer<TKey>) null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that is empty, has the
		/// default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
		/// </summary>
		/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or NULL to use the default equality comparer for the <typeparamref name="TKey"/>.</param>
		public MultiValuedDictionary(IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, List<TItem>>(comparer)) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that is empty, has the
		/// specified initial capacity, and uses the default equality comparer for the <typeparamref name="TKey"/>.
		/// </summary>
		/// <param name="capacity"></param>
		public MultiValuedDictionary(int capacity)
			: this(capacity, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that is empty, has the
		/// specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or NULL to use the default equality comparer for the <typeparamref name="TKey"/>.</param>
		public MultiValuedDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, List<TItem>>(capacity, comparer)) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that contains values copied
		/// from the specified source, and uses the default equality comparer for the <typeparamref name="TKey"/>.
		/// </summary>
		/// <remarks>
		/// Any source values with duplicate keys are automatically collapsed to a single key entry with the individual items
		/// concatenanted sequentially into a single list.
		/// </remarks>
		/// <param name="source">The source enumerable whose values are to be coped to the new <see cref="MultiValuedDictionary{TKey,TItem}"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is NULL.</exception>
		public MultiValuedDictionary(IEnumerable<KeyValuePair<TKey, TItem>> source)
			: this(source, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that contains values copied
		/// from the specified source, and uses the specified <see cref="IEqualityComparer{T}"/>.
		/// </summary>
		/// <remarks>
		/// Any source values with duplicate keys are automatically collapsed to a single key entry with the individual items
		/// concatenanted sequentially into a single list.
		/// </remarks>
		/// <param name="source">The source enumerable whose values are to be coped to the new <see cref="MultiValuedDictionary{TKey,TItem}"/>.</param>
		/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or NULL to use the default equality comparer for the <typeparamref name="TKey"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is NULL.</exception>
		public MultiValuedDictionary(IEnumerable<KeyValuePair<TKey, TItem>> source, IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, List<TItem>>(comparer))
		{
			if (ReferenceEquals(source, null))
				throw new ArgumentNullException("source");
			foreach (KeyValuePair<TKey, TItem> pair in source)
				Add(pair.Key, pair.Value);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that contains values copied
		/// from the specified source, and uses the default equality comparer for the <typeparamref name="TKey"/>.
		/// </summary>
		/// <remarks>
		/// Any source values with duplicate keys are automatically collapsed to a single key entry with the individual items
		/// concatenanted sequentially into a single list.
		/// </remarks>
		/// <param name="source">The source enumerable whose values are to be coped to the new <see cref="MultiValuedDictionary{TKey,TItem}"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is NULL.</exception>
		public MultiValuedDictionary(IEnumerable<KeyValuePair<TKey, IEnumerable<TItem>>> source)
			: this(source, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="MultiValuedDictionary{TKey,TItem}"/> that contains values copied
		/// from the specified source, and uses the specified <see cref="IEqualityComparer{T}"/>.
		/// </summary>
		/// <remarks>
		/// Any source values with duplicate keys are automatically collapsed to a single key entry with the individual items
		/// concatenanted sequentially into a single list.
		/// </remarks>
		/// <param name="source">The source enumerable whose values are to be coped to the new <see cref="MultiValuedDictionary{TKey,TItem}"/>.</param>
		/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or NULL to use the default equality comparer for the <typeparamref name="TKey"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is NULL.</exception>
		public MultiValuedDictionary(IEnumerable<KeyValuePair<TKey, IEnumerable<TItem>>> source, IEqualityComparer<TKey> comparer)
			: this(new Dictionary<TKey, List<TItem>>(comparer))
		{
			if (ReferenceEquals(source, null))
				throw new ArgumentNullException("source");
			foreach (KeyValuePair<TKey, IEnumerable<TItem>> pair in source)
				AddRange(pair.Key, pair.Value);
		}

		private MultiValuedDictionary(Dictionary<TKey, List<TItem>> dictionary)
		{
			_dictionary = dictionary;
			_keyCollection = new KeyCollection(this);
			_itemCollection = new ItemCollection(this);
			_itemListCollection = new ItemListCollecion(this);
		}

		/// <summary>
		/// Gets an <see cref="IList{T}"/> of items mapped to the specified <paramref name="key"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="IList{T}"/> is not a static copy; instead, the list is a wrapper that refers back to the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes to either the original dictionary or this list
		/// continue to be reflected in the other.
		/// </remarks>
		/// <param name="key">The key for which the list of mapped items is to be retrieved.</param>
		/// <returns>Returns a list of items mapped to the specified key. If no items are mapped to the specified key, this returns an empty list.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public IList<TItem> this[TKey key]
		{
			get
			{
				if (ReferenceEquals(key, null))
					throw new ArgumentNullException("key");
				return new ItemList(key, this);
			}
		}

		/// <summary>
		/// Gets the total number of mapped items in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Accessing this property yields the same result as the <see cref="ItemCollection.Count"/> property on <see cref="Items"/>.
		/// </remarks>
		public int Count
		{
			get { return _itemCollection.Count; }
		}

		/// <summary>
		/// Gets a <see cref="KeyCollection"/> of unique keys in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="KeyCollection"/> is not a static copy; instead, the collection refers back to the keys in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// </remarks>
		public KeyCollection Keys
		{
			get { return _keyCollection; }
		}

		/// <summary>
		/// Gets an <see cref="ItemCollection"/> of mapped items in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="ItemCollection"/> is not a static copy; instead, the collection refers back to the items in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// </remarks>
		public ItemCollection Items
		{
			get { return _itemCollection; }
		}

		/// <summary>
		/// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine the equality of keys for the dictionary.
		/// </summary>
		public IEqualityComparer<TKey> Comparer
		{
			get { return _dictionary.Comparer; }
		}

		/// <summary>
		/// Adds a mapping for the provided item and not-necessarily unique key to the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key to which the item should be mapped.</param>
		/// <param name="item">The item to be mapped.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public void Add(TKey key, TItem item)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			if (!_dictionary.ContainsKey(key))
				_dictionary.Add(key, new List<TItem>(new[] {item}));
			else
				_dictionary[key].Add(item);
		}

		/// <summary>
		/// Adds mappings for the provided items and not-necessarily unique key to the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key to which the provided items should be mapped.</param>
		/// <param name="collection">The items to be mapped.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public void AddRange(TKey key, IEnumerable<TItem> collection)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			if (!_dictionary.ContainsKey(key))
				_dictionary.Add(key, new List<TItem>(collection));
			else
				_dictionary[key].AddRange(collection);
		}

		/// <summary>
		/// Removes a mapping for the provided item and key from the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key of the mapped item to be removed.</param>
		/// <param name="item">The mapped item to be removed.</param>
		/// <returns>True if the item mapping was removed; False if no mapping for the specified key and item existed, and thus no mapping was removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public bool Remove(TKey key, TItem item)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			if (!_dictionary.ContainsKey(key))
				return false;
			try
			{
				return _dictionary[key].Remove(item);
			}
			finally
			{
				Compact(key);
			}
		}

		/// <summary>
		/// Removes all mapped items to the specified key from the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key for which all mapped items should be removed.</param>
		/// <returns>True if mappings were removed; False if no items were mapped to the provided key, and thus no mappings were removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public bool RemoveAll(TKey key)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			return _dictionary.Remove(key);
		}

		/// <summary>
		/// Clears all mapped items in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		public void Clear()
		{
			_dictionary.Clear();
		}

		/// <summary>
		/// Checks for the presence of any items mapped to the specified key in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="key">The key for which to check for the presence of mapped items.</param>
		/// <returns>True if there is at least one item mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public bool ContainsKey(TKey key)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			return _keyCollection.Contains(key);
		}

		/// <summary>
		/// Checks for the presence of the a mapping for the specified item.
		/// </summary>
		/// <remarks>
		/// Calling this method yields the same result as calling the <see cref="ItemCollection.Contains"/> method on <see cref="Items"/>.
		/// </remarks>
		/// <param name="item">The item for which the presence of a mapping is to be checked.</param>
		/// <returns>True if a mapping exists fo the specified item; False otherwsie.</returns>
		public bool ContainsValue(TItem item)
		{
			return _itemCollection.Contains(item);
		}

		/// <summary>
		/// Searches for the first instance of a mapped item and returns the corresponding key.
		/// </summary>
		/// <remarks>
		/// If <typeparamref name="TKey"/> is a value type then the result will be indistinguishable as to whether or not
		/// the item was found and its key was the empty value, or the item was not found. In this scenario, consider
		/// using <see cref="FindKey(TItem,TKey)"/> with a value that is known to not be in the dictionary, or
		/// <see cref="TryFindKey(TItem,out TKey)"/>.
		/// </remarks>
		/// <param name="item">The item for which a key is to be found.</param>
		/// <returns>The key for the first instance of the specified item, or the default value for <typeparamref name="TKey"/> if the specified item was not found.</returns>
		public TKey FindKey(TItem item)
		{
			return FindKey(item, default(TKey));
		}

		/// <summary>
		/// Searches for the first instance of a mapped item and returns the corresponding key.
		/// </summary>
		/// <param name="item">The item for which a key is to be found.</param>
		/// <param name="defaultValue">The value to be returned if the specified item is not found.</param>
		/// <returns>The key for the first instance of the specified item, or <paramref name="defaultValue"/> if the specified item was not found.</returns>
		public TKey FindKey(TItem item, TKey defaultValue)
		{
			TKey key;
			if (TryFindKey(item, out key))
				return key;
			return defaultValue;
		}

		/// <summary>
		/// Searches for the first instance of a mapped item and returns the corresponding key.
		/// </summary>
		/// <param name="item">The item for which a key is to be found.</param>
		/// <param name="key">The key for the first instance of the specified item, or the default value for <typeparamref name="TKey"/> if the specified item was not found.</param>
		/// <returns>True if the item was found; False otherwise.</returns>
		public bool TryFindKey(TItem item, out TKey key)
		{
			foreach (KeyValuePair<TKey, List<TItem>> pair in _dictionary)
			{
				if (pair.Value.Contains(item))
				{
					key = pair.Key;
					return true;
				}
			}
			key = default(TKey);
			return false;
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions and returns the corresponding key.
		/// </summary>
		/// <remarks>
		/// If <typeparamref name="TKey"/> is a value type then the result will be indistinguishable as to whether or not
		/// an item was found and its key was the empty value, or an item was not found. In this scenario, consider
		/// using <see cref="FindKey(System.Predicate{TItem},TKey)"/> with a value that is known to not be in the dictionary, or
		/// <see cref="TryFindKey(System.Predicate{TItem},out TKey)"/>.
		/// </remarks>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <returns>The key for the first instance of an item matching the <paramref name="predicate"/>, or the default value for <typeparamref name="TKey"/> if no matching item was found.</returns>
		public TKey FindKey(Predicate<TItem> predicate)
		{
			return FindKey(predicate, default(TKey));
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions and returns the corresponding key.
		/// </summary>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <param name="defaultValue">The value to be returned if no matching item was found.</param>
		/// <returns>The key for the first instance of an item matching the <paramref name="predicate"/>, or <paramref name="defaultValue"/> if no matching item was found.</returns>
		public TKey FindKey(Predicate<TItem> predicate, TKey defaultValue)
		{
			TKey key;
			if (TryFindKey(predicate, out key))
				return key;
			return defaultValue;
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions and returns the corresponding key.
		/// </summary>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <param name="key">The key for the first instance of an item matching the <paramref name="predicate"/>, or the default value for <typeparamref name="TKey"/> if no matching item was found.</param>
		/// <returns>True if a match was found; False otherwise.</returns>
		public bool TryFindKey(Predicate<TItem> predicate, out TKey key)
		{
			foreach (KeyValuePair<TKey, List<TItem>> pair in _dictionary)
			{
				int index = pair.Value.FindIndex(predicate);
				if (index >= 0)
				{
					key = pair.Key;
					return true;
				}
			}
			key = default(TKey);
			return false;
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions.
		/// </summary>
		/// <remarks>
		/// Tthe result of this method will be indistinguishable as to whether or not a match was found and it was the
		/// default value for <typeparamref name="TItem"/>, or if no matching item was found. In this scenario, consider
		/// using <see cref="Find(System.Predicate{TItem},TItem)"/> with a value that is known to not be in the dictionary, or
		/// <see cref="TryFind(System.Predicate{TItem},out TItem)"/>.
		/// </remarks>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <returns>The key for the first instance of an item matching the <paramref name="predicate"/>, or the default value for <typeparamref name="TKey"/> if no matching item was found.</returns>
		public TItem Find(Predicate<TItem> predicate)
		{
			return Find(predicate, default(TItem));
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions.
		/// </summary>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <param name="defaultValue">The value to be returned if no matching item was found.</param>
		/// <returns>The key for the first instance of an item matching the <paramref name="predicate"/>, or <paramref name="defaultValue"/> if no matching item was found.</returns>
		public TItem Find(Predicate<TItem> predicate, TItem defaultValue)
		{
			TItem item;
			if (TryFind(predicate, out item))
				return item;
			return defaultValue;
		}

		/// <summary>
		/// Searches for the first item matching a given set of conditions.
		/// </summary>
		/// <param name="predicate">A <see cref="Predicate{T}"/> delegate defining the conditions of the item to be found.</param>
		/// <param name="match">The key for the first instance of an item matching the <paramref name="predicate"/>, or the default value for <typeparamref name="TItem"/> if no matching item was found.</param>
		/// <returns>True if a match was found; False otherwise.</returns>
		public bool TryFind(Predicate<TItem> predicate, out TItem match)
		{
			foreach (TItem item in _itemCollection)
			{
				if (predicate.Invoke(item))
				{
					match = item;
					return true;
				}
			}
			match = default(TItem);
			return false;
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable{T}"/> of items mapped to the specified key.
		/// </summary>
		/// <param name="key">The key for which the enumerable of mapped items is to be retrieved.</param>
		/// <param name="item">The retrieved enumerable of items mapped to the specified key. If no items were mapped to the specified key, this returns an empty enumerable.</param>
		/// <returns>True if there were items mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		public bool TryGetItems(TKey key, out IEnumerable<TItem> item)
		{
			if (ReferenceEquals(key, null))
				throw new ArgumentNullException("key");
			List<TItem> value;
			if (_dictionary.TryGetValue(key, out value))
			{
				item = value.AsReadOnly();
				return true;
			}
			item = new TItem[] {};
			return false;
		}

		/// <summary>
		/// Returns a read-only <see cref="IMultiValuedDictionary{TKey,TItem}"/> wrapper for the current dictionary.
		/// </summary>
		/// <remarks>
		/// To prevent any modifications to <see cref="MultiValuedDictionary{TKey,TItem}"/>, only expose the dictionary through this wrapper.
		/// The returned wrapper is not a static copy; instead, it refers back to the items in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the wrapper.
		/// </remarks>
		/// <returns>A read-only wrapper around the current <see cref="MultiValuedDictionary{TKey,TItem}"/>.</returns>
		public IMultiValuedDictionary<TKey, TItem> AsReadOnly()
		{
			return new ReadOnlyMultiValuedDictionaryWrapper(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <typeparamref name="TKey"/> and <see cref="IEnumerable{T}"/> pairs.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
		public IEnumerator<KeyValuePair<TKey, IEnumerable<TItem>>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, List<TItem>> pair in _dictionary)
				yield return new KeyValuePair<TKey, IEnumerable<TItem>>(pair.Key, pair.Value.AsReadOnly());
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <typeparamref name="TKey"/> and <see cref="IEnumerable{T}"/> pairs.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void Compact(TKey key)
		{
			if (_dictionary[key].Count == 0)
				_dictionary.Remove(key);
		}

		#region IMultiValuedDictionary<TKey, TItem> Members

		/// <summary>
		/// Gets an <see cref="KeyCollection"/> of unique keys in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="KeyCollection"/> is not a static copy; instead, the collection refers back to the keys in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// </remarks>
		ICollection<TKey> IMultiValuedDictionary<TKey, TItem>.Keys
		{
			get { return Keys; }
		}

		/// <summary>
		/// Gets an <see cref="ItemCollection"/> of mapped items in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="ItemCollection"/> is not a static copy; instead, the collection refers back to the items in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// </remarks>
		ICollection<TItem> IMultiValuedDictionary<TKey, TItem>.Items
		{
			get { return Items; }
		}

		/// <summary>
		/// Copies the mappings of the <see cref="MultiValuedDictionary{TKey,TItem}"/> to an array of <see cref="KeyValuePair{TKey,TValue}"/>s.
		/// </summary>
		/// <param name="array">The destination array to which the mappings are to be copied.</param>
		/// <param name="arrayIndex">The index in the destination array at which to start copying.</param>
		void IMultiValuedDictionary<TKey, TItem>.CopyTo(KeyValuePair<TKey, IEnumerable<TItem>>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<TKey, IEnumerable<TItem>> pair in this)
				array[arrayIndex++] = pair;
		}

		/// <summary>
		/// This implementation of <see cref="IMultiValuedDictionary{TKey,TItem}"/> is never read-only.
		/// </summary>
		bool IMultiValuedDictionary<TKey, TItem>.IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IDictionary<TKey, IList<TItem>> Members

		/// <summary>
		/// Gets or sets all the items mapped to a given key.
		/// </summary>
		/// <remarks>
		/// This property retrieves an enumerable of all items mapped to the specified key
		/// and replaces all items mapped to the specified key with those of the enumerable value.
		/// </remarks>
		/// <param name="key">The key for which all mapped items are to be retrieved or replaced.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		IEnumerable<TItem> IDictionary<TKey, IEnumerable<TItem>>.this[TKey key]
		{
			get
			{
				if (ReferenceEquals(key, null))
					throw new ArgumentNullException("key");
				return _dictionary[key].AsReadOnly();
			}
			set
			{
				if (ReferenceEquals(key, null))
					throw new ArgumentNullException("key");
				_dictionary[key] = new List<TItem>(value);
			}
		}

		/// <summary>
		/// Gets the total number of unique keys in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Accessing this property yields the same result as the <see cref="KeyCollection.Count"/> property on <see cref="Keys"/>.
		/// </remarks>
		int ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.Count
		{
			get { return _dictionary.Count; }
		}

		/// <summary>
		/// Gets a <see cref="KeyCollection"/> of unique keys in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="KeyCollection"/> is not a static copy; instead, the collection refers back to the keys in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// Accessing this property yields the same instance as the <see cref="Keys"/> property.
		/// </remarks>
		ICollection<TKey> IDictionary<TKey, IEnumerable<TItem>>.Keys
		{
			get { return _keyCollection; }
		}

		/// <summary>
		/// Gets a collection of mapped item enumerations grouped by key in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// The returned collection is not a static copy; instead, the collection refers back to the items in the original
		/// <see cref="MultiValuedDictionary{TKey,TItem}"/>. Therefore, changes in the original dictionary continue to be reflected
		/// in the collection.
		/// </remarks>
		ICollection<IEnumerable<TItem>> IDictionary<TKey, IEnumerable<TItem>>.Values
		{
			get { return _itemListCollection; }
		}

		/// <summary>
		/// This implementation of <see cref="IDictionary{TKey,TValue}"/> is never read-only.
		/// </summary>
		bool ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.IsReadOnly
		{
			get { return ((IMultiValuedDictionary<TKey, TItem>) this).IsReadOnly; }
		}

		/// <summary>
		/// Adds mappings for the provided items and not-necessarily unique key to the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Calling this method yields the same result as calling <see cref="AddRange"/>.
		/// </remarks>
		/// <param name="key">The key to which the provided items should be mapped.</param>
		/// <param name="value">The items to be mapped.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		void IDictionary<TKey, IEnumerable<TItem>>.Add(TKey key, IEnumerable<TItem> value)
		{
			AddRange(key, value);
		}

		/// <summary>
		/// Adds mappings for the provided items and not-necessarily unique key to the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Calling this method yields the same result as calling <see cref="AddRange"/>.
		/// </remarks>
		/// <param name="pair">The key-value pair specifying the key and associated items to be mapped.</param>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and the provided <paramref name="pair"/> has a NULL <see cref="KeyValuePair{TKey,TValue}.Key"/>.</exception>
		void ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.Add(KeyValuePair<TKey, IEnumerable<TItem>> pair)
		{
			AddRange(pair.Key, pair.Value);
		}

		/// <summary>
		/// Removes all mapped items to the specified key from the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <remarks>
		/// Calling this method yields the same result as calling <see cref="RemoveAll"/>.
		/// </remarks>
		/// <param name="key">The key for which all mapped items should be removed.</param>
		/// <returns>True if mappings were removed; False if no items were mapped to the provided key, and thus no mappings were removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		bool IDictionary<TKey, IEnumerable<TItem>>.Remove(TKey key)
		{
			return RemoveAll(key);
		}

		/// <summary>
		/// Removes mappings for all the specified items from the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="pair">The key-value pair specifying the key and associated items to be removed.</param>
		/// <returns>True if any of the speified item mappings were removed; False if none of the specified key and items existed, and thus no mappings were removed.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and the provided <paramref name="pair"/> has a NULL <see cref="KeyValuePair{TKey,TValue}.Key"/>.</exception>
		bool ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.Remove(KeyValuePair<TKey, IEnumerable<TItem>> pair)
		{
			if (ReferenceEquals(pair.Key, null))
				throw new ArgumentNullException("pair");
			bool result = false;
			if (_dictionary.ContainsKey(pair.Key))
			{
				List<TItem> list = _dictionary[pair.Key];
				foreach (TItem item in pair.Value)
					result |= list.Remove(item);
			}
			return result;
		}

		/// <summary>
		/// Checks for the presence of all items mapped to the specified key in the <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		/// <param name="pair">The key-value pair specifying the key and associated items for which to check for presence in the dictionary.</param>
		/// <returns>True if there is all the specified items are mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and the provided <paramref name="pair"/> has a NULL <see cref="KeyValuePair{TKey,TValue}.Key"/>.</exception>
		bool ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.Contains(KeyValuePair<TKey, IEnumerable<TItem>> pair)
		{
			if (ReferenceEquals(pair.Key, null))
				throw new ArgumentNullException("pair");
			bool result = true;
			if (_dictionary.ContainsKey(pair.Key))
			{
				List<TItem> list = _dictionary[pair.Key];
				foreach (TItem item in pair.Value)
					result &= list.Contains(item);
			}
			return result;
		}

		/// <summary>
		/// Copies the mappings of the <see cref="MultiValuedDictionary{TKey,TItem}"/> to an array of <see cref="KeyValuePair{TKey,TValue}"/>s.
		/// </summary>
		/// <param name="array">The destination array to which the mappings are to be copied.</param>
		/// <param name="arrayIndex">The index in the destination array at which to start copying.</param>
		void ICollection<KeyValuePair<TKey, IEnumerable<TItem>>>.CopyTo(KeyValuePair<TKey, IEnumerable<TItem>>[] array, int arrayIndex)
		{
			((IMultiValuedDictionary<TKey, TItem>) this).CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable{T}"/> of items mapped to the specified key.
		/// </summary>
		/// <remarks>
		/// Calling this method yields the same result as calling <see cref="TryGetItems"/>.
		/// </remarks>
		/// <param name="key">The key for which the enumerable of mapped items is to be retrieved.</param>
		/// <param name="value">The retrieved enumerable of items mapped to the specified key. If no items were mapped to the specified key, this returns an empty enumerable.</param>
		/// <returns>True if there were items mapped to the specified key; False otherwise.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <typeparamref name="TKey"/> is a reference type and <paramref name="key"/> is NULL.</exception>
		bool IDictionary<TKey, IEnumerable<TItem>>.TryGetValue(TKey key, out IEnumerable<TItem> value)
		{
			return TryGetItems(key, out value);
		}

		#endregion

		#region KeyCollection Class

		/// <summary>
		/// Represents the collection of keys in a <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		public sealed class KeyCollection : ICollection<TKey>
		{
			private readonly Dictionary<TKey, List<TItem>> _dictionary;

			/// <summary>
			/// Initializes a new instance of <see cref="KeyCollection"/> whose values reflect the keys in the specified <see cref="MultiValuedDictionary{TKey,TItem}"/>.
			/// </summary>
			/// <param name="multiValuedDictionary">The dictionary whose keys are to be reflected in this collection.</param>
			public KeyCollection(MultiValuedDictionary<TKey, TItem> multiValuedDictionary)
			{
				if (ReferenceEquals(multiValuedDictionary, null))
					throw new ArgumentNullException("multiValuedDictionary");
				_dictionary = multiValuedDictionary._dictionary;
			}

			public int Count
			{
				get { return _dictionary.Count; }
			}

			public bool Contains(TKey key)
			{
				if (ReferenceEquals(key, null))
					throw new ArgumentNullException("key");
				return _dictionary.ContainsKey(key);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				foreach (TKey key in _dictionary.Keys)
					array[arrayIndex++] = key;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return _dictionary.Keys.GetEnumerator();
			}

			void ICollection<TKey>.Add(TKey key)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Remove(TKey key)
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get { return true; }
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion

		#region ItemCollection Class

		/// <summary>
		/// Represents the collection of items regardless of key in a <see cref="MultiValuedDictionary{TKey,TItem}"/>.
		/// </summary>
		public class ItemCollection : ICollection<TItem>
		{
			private readonly Dictionary<TKey, List<TItem>> _dictionary;

			/// <summary>
			/// Initializes a new instance of <see cref="ItemCollection"/> whose values reflect the items in the specified <see cref="MultiValuedDictionary{TKey,TItem}"/>.
			/// </summary>
			/// <param name="multiValuedDictionary">The dictionary whose items are to be reflected in this collection.</param>
			public ItemCollection(MultiValuedDictionary<TKey, TItem> multiValuedDictionary)
			{
				if (ReferenceEquals(multiValuedDictionary, null))
					throw new ArgumentNullException("multiValuedDictionary");
				_dictionary = multiValuedDictionary._dictionary;
			}

			public int Count
			{
				get
				{
					int count = 0;
					foreach (List<TItem> list in _dictionary.Values)
						count += list.Count;
					return count;
				}
			}

			public bool Contains(TItem item)
			{
				foreach (List<TItem> list in _dictionary.Values)
				{
					if (list.Contains(item))
						return true;
				}
				return false;
			}

			public void CopyTo(TItem[] array, int arrayIndex)
			{
				foreach (List<TItem> list in _dictionary.Values)
				{
					foreach (TItem item in list)
						array[arrayIndex++] = item;
				}
			}

			public IEnumerator<TItem> GetEnumerator()
			{
				foreach (List<TItem> list in _dictionary.Values)
				{
					foreach (TItem item in list)
						yield return item;
				}
			}

			void ICollection<TItem>.Add(TItem item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TItem>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TItem>.Remove(TItem item)
			{
				throw new NotSupportedException();
			}

			bool ICollection<TItem>.IsReadOnly
			{
				get { return true; }
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion

		#region ItemListCollection Class

		private class ItemListCollecion : ICollection<IEnumerable<TItem>>
		{
			private readonly MultiValuedDictionary<TKey, TItem> _multiValuedDictionary;

			public ItemListCollecion(MultiValuedDictionary<TKey, TItem> multiValuedDictionary)
			{
				if (ReferenceEquals(multiValuedDictionary, null))
					throw new ArgumentNullException("multiValuedDictionary");
				_multiValuedDictionary = multiValuedDictionary;
			}

			public int Count
			{
				get { return _multiValuedDictionary._dictionary.Count; }
			}

			public void CopyTo(IEnumerable<TItem>[] array, int arrayIndex)
			{
				foreach (List<TItem> value in _multiValuedDictionary._dictionary.Values)
					array[arrayIndex++] = value.AsReadOnly();
			}

			public IEnumerator<IEnumerable<TItem>> GetEnumerator()
			{
				foreach (List<TItem> value in _multiValuedDictionary._dictionary.Values)
					yield return value.AsReadOnly();
			}

			void ICollection<IEnumerable<TItem>>.Add(IEnumerable<TItem> item)
			{
				throw new NotSupportedException();
			}

			void ICollection<IEnumerable<TItem>>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<IEnumerable<TItem>>.Contains(IEnumerable<TItem> items)
			{
				foreach (TKey key in _multiValuedDictionary._dictionary.Keys)
				{
					bool result = true;
					List<TItem> list = _multiValuedDictionary._dictionary[key];
					foreach (TItem item in items)
						result &= list.Contains(item);
					if (result)
						return true;
				}
				return false;
			}

			bool ICollection<IEnumerable<TItem>>.IsReadOnly
			{
				get { return true; }
			}

			bool ICollection<IEnumerable<TItem>>.Remove(IEnumerable<TItem> item)
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion

		#region ItemList Members

		private class ItemList : IList<TItem>
		{
			private readonly MultiValuedDictionary<TKey, TItem> _source;
			private readonly TKey _key;

			public ItemList(TKey key, MultiValuedDictionary<TKey, TItem> source)
			{
				if (ReferenceEquals(key, null))
					throw new ArgumentNullException("key");
				if (ReferenceEquals(source, null))
					throw new ArgumentNullException("source");
				_key = key;
				_source = source;
			}

			private bool GetList(out IList<TItem> list)
			{
				if (_source._dictionary.ContainsKey(_key))
				{
					list = _source._dictionary[_key];
					return true;
				}
				list = null;
				return false;
			}

			public override int GetHashCode()
			{
				return 0x53836841 ^ _key.GetHashCode() ^ _source.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is ItemList)
				{
					ItemList other = (ItemList) obj;
					return ReferenceEquals(_source, other._source) && _source.Comparer.Equals(_key, other._key);
				}
				return false;
			}

			public int IndexOf(TItem item)
			{
				IList<TItem> list;
				if (GetList(out list))
					return list.IndexOf(item);
				return -1;
			}

			public void Insert(int index, TItem item)
			{
				IList<TItem> list;
				if (GetList(out list))
					list.Insert(index, item);
				else if (index == 0)
					_source.Add(_key, item);
				else
					throw new IndexOutOfRangeException();
			}

			public void RemoveAt(int index)
			{
				try
				{
					IList<TItem> list;
					if (GetList(out list))
						list.RemoveAt(index);
					else
						throw new IndexOutOfRangeException();
				}
				finally
				{
					_source.Compact(_key);
				}
			}

			public TItem this[int index]
			{
				get
				{
					IList<TItem> list;
					if (GetList(out list))
						return list[index];
					throw new IndexOutOfRangeException();
				}
				set
				{
					IList<TItem> list;
					if (GetList(out list))
					{
						list[index] = value;
						return;
					}
					throw new IndexOutOfRangeException();
				}
			}

			public void Add(TItem item)
			{
				// this functionality is already directly exposed on the owner
				_source.Add(_key, item);
			}

			public void Clear()
			{
				// this functionality is already directly exposed on the owner
				_source.RemoveAll(_key);
			}

			public bool Contains(TItem item)
			{
				IList<TItem> list;
				if (GetList(out list))
					return list.Contains(item);
				return false;
			}

			public void CopyTo(TItem[] array, int arrayIndex)
			{
				IList<TItem> list;
				if (GetList(out list))
					list.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get
				{
					IList<TItem> list;
					if (GetList(out list))
						return list.Count;
					return 0;
				}
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(TItem item)
			{
				// this functionality is already directly exposed on the owner
				return _source.Remove(_key, item);
			}

			public IEnumerator<TItem> GetEnumerator()
			{
				IList<TItem> list;
				if (GetList(out list))
					return list.GetEnumerator();
				return GetNullEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private static IEnumerator<TItem> GetNullEnumerator()
			{
				yield break;
			}
		}

		#endregion

		#region ReadOnlyMultiValuedDictionaryWrapper Class

		private class ReadOnlyMultiValuedDictionaryWrapper : IMultiValuedDictionary<TKey, TItem>
		{
			private readonly MultiValuedDictionary<TKey, TItem> _source;

			public ReadOnlyMultiValuedDictionaryWrapper(MultiValuedDictionary<TKey, TItem> source)
			{
				_source = source;
			}

			public IList<TItem> this[TKey key]
			{
				get { return _source._dictionary[key].AsReadOnly(); }
			}

			public int Count
			{
				get { return _source.Count; }
			}

			public ICollection<TKey> Keys
			{
				get { return _source.Keys; }
			}

			public ICollection<TItem> Items
			{
				get { return _source.Items; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public void Add(TKey key, TItem item)
			{
				throw new NotSupportedException();
			}

			public bool Remove(TKey key, TItem item)
			{
				throw new NotSupportedException();
			}

			public bool RemoveAll(TKey key)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool ContainsKey(TKey key)
			{
				return _source.ContainsKey(key);
			}

			public void CopyTo(KeyValuePair<TKey, IEnumerable<TItem>>[] array, int arrayIndex)
			{
				((IMultiValuedDictionary<TKey, TItem>) _source).CopyTo(array, arrayIndex);
			}

			public bool TryGetItems(TKey key, out IEnumerable<TItem> item)
			{
				return _source.TryGetItems(key, out item);
			}

			public IEnumerator<KeyValuePair<TKey, IEnumerable<TItem>>> GetEnumerator()
			{
				return _source.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#endregion
	}
}