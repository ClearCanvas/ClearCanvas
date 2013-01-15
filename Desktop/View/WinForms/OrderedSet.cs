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

using System.Collections.Generic;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Implements an ordered set, where the order of enumeration reflects the order in which items
    /// were added to the set.  If the same item is added to the set again, it is moved to the end of the ordering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class OrderedSet<T> : IEnumerable<T>
    {
        private LinkedList<T> _items;

        public OrderedSet()
        {
            _items = new LinkedList<T>();
        }

        public void Add(T item)
        {
            _items.Remove(item);
            _items.AddLast(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public T FirstElement
        {
            get { return _items.Count > 0 ? _items.First.Value : default(T); }
        }

        public T LastElement
        {
            get { return _items.Count > 0 ? _items.Last.Value : default(T); }
        }

        public T SecondLastElement
        {
            get { return _items.Count > 1 ? _items.Last.Previous.Value : default(T); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }
}
