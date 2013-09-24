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

namespace ClearCanvas.Common.Utilities
{
	internal sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private const string _errorDictionaryIsReadOnly = "Dictionary is read-only.";
		private readonly IDictionary<TKey, TValue> _dictionary;

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			_dictionary = dictionary;
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return true; }
		}

		public ICollection<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}

		public ICollection<TValue> Values
		{
			get { return _dictionary.Values; }
		}

		public TValue this[TKey key]
		{
			get { return _dictionary[key]; }
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _dictionary.Contains(item);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return _dictionary[key]; }
			set { throw new InvalidOperationException(_errorDictionaryIsReadOnly); }
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException(_errorDictionaryIsReadOnly);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw new InvalidOperationException(_errorDictionaryIsReadOnly);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException(_errorDictionaryIsReadOnly);
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw new InvalidOperationException(_errorDictionaryIsReadOnly);
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw new InvalidOperationException(_errorDictionaryIsReadOnly);
		}
	}
}