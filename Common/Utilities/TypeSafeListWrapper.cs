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
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IList"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    public class TypeSafeListWrapper<T> : IList<T>, IList
    {
        private IList _inner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="innerList">The untyped <see cref="IList"/> to wrap.</param>
        public TypeSafeListWrapper(IList innerList)
        {
            _inner = innerList;
        }

        #region IList<T> Members

		/// <summary>
		/// Gets the index of <paramref name="item"/> in the list.
		/// </summary>
		/// <returns>The index of the input item, or -1 if it doesn't exist.</returns>
        public int IndexOf(T item)
        {
            return _inner.IndexOf(item);
        }

		/// <summary>
		/// Inserts <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
        public void Insert(int index, T item)
        {
            _inner.Insert(index, item);
        }

        /// <summary>
        /// Removes the item at <paramref name="index"/>.
        /// </summary>
		public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

		/// <summary>
		/// Gets or sets the item at <paramref name="index"/>.
		/// </summary>
        public T this[int index]
        {
            get
            {
                return (T)_inner[index];
            }
            set
            {
                _inner[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

		/// <summary>
		/// Adds the <paramref name="item"/> to the list.
		/// </summary>
        public void Add(T item)
        {
            _inner.Add(item);
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
		public void Clear()
        {
            _inner.Clear();
        }

		/// <summary>
		/// Gets whether or not <paramref name="item"/> is in the list.
		/// </summary>
		/// <returns>
		/// True if the item is in the list, false otherwise.
		/// </returns>
        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

		/// <summary>
		/// Copies the entire contents of the list to <paramref name="array"/>, 
		/// starting at the specified <paramref name="arrayIndex"/>.
		/// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
        public int Count
        {
            get { return _inner.Count; }
        }

		/// <summary>
		/// Gets whether or not the list is read-only.
		/// </summary>
        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }

        /// <summary>
        /// Removes <paramref name="item"/> from the list.
        /// </summary>
        /// <returns>
        /// True if the item was in the list and was successfully removed, otherwise false.
        /// </returns>
		public bool Remove(T item)
        {
            if (_inner.Contains(item))
            {
                _inner.Remove(item);
                return !_inner.Contains(item);
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{T}"/> for the list.
		/// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new TypeSafeEnumeratorWrapper<T>(_inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an <see cref="IEnumerator"/> for the list.
		/// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        #endregion

        #region IList Members

		/// <summary>
		/// Adds <paramref name="value"/> to the list.
		/// </summary>
        public int Add(object value)
        {
            return _inner.Add(value);
        }

		/// <summary>
		/// Gets whether or not <paramref name="value"/> is in the list.
		/// </summary>
        public bool Contains(object value)
        {
            return _inner.Contains(value);
        }

		/// <summary>
		/// Gets the index of <paramref name="value"/>, or -1 if it is not in the list.
		/// </summary>
        public int IndexOf(object value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Inserts <paramref name="value"/> at the specified <paramref name="index"/>.
        /// </summary>
		public void Insert(int index, object value)
        {
            _inner.Insert(index, value);
        }

		/// <summary>
		/// Gets whether or not the list is of fixed size.
		/// </summary>
        public bool IsFixedSize
        {
            get { return _inner.IsFixedSize; }
        }
		
		/// <summary>
		/// Removes <paramref name="value"/> from the list.
		/// </summary>
        public void Remove(object value)
        {
            _inner.Remove(value);
        }

		/// <summary>
		/// Gets the item at the specified <paramref name="index"/>.
		/// </summary>
        object IList.this[int index]
        {
            get
            {
                return _inner[index];
            }
            set
            {
                _inner[index] = value;
            }
        }

        #endregion

        #region ICollection Members

		/// <summary>
		/// Copies the entire contents of the list to <paramref name="array"/>, 
		/// starting at the specified <paramref name="index"/>.
		/// </summary>
        public void CopyTo(Array array, int index)
        {
            _inner.CopyTo(array, index);
        }

		/// <summary>
		/// Gets whether or not the list is synchronized.
		/// </summary>
        public bool IsSynchronized
        {
            get { return _inner.IsSynchronized; }
        }

		/// <summary>
		/// Gets the sync root object for synchronization of the list.
		/// </summary>
        public object SyncRoot
        {
            get { return _inner.SyncRoot; }
        }

        #endregion
    }
}
