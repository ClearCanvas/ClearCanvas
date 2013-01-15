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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	// Note: It is important that this class retain immutable semantics.  Do not add mutator methods/properties to this class.
	
	/// <summary>
    /// Default implementation of <see cref="ISelection"/>.  
    /// </summary>
	public class Selection : ISelection, IEquatable<ISelection>
    {
		/// <summary>
		/// Represents an empty <see cref="ISelection"/>.
		/// </summary>
        public static readonly Selection Empty = new Selection();

        private List<object> _items = new List<object>();

		/// <summary>
		/// Constructor.
		/// </summary>
        public Selection()
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The single item that is selected.</param>
        public Selection(object item)
        {
            // if item == null, then don't add it, which gives us the Empty Selection
            if (item != null)
            {
                _items.Add(item);
            }
        }

		/// <summary>
		/// Constructor for multi-selection.
		/// </summary>
		/// <param name="items">The selected items.</param>
        public Selection(IEnumerable items)
        {
            foreach (object item in items)
                _items.Add(item);
        }

        #region ISelection Members

    	/// <summary>
    	/// Returns the set of items that are currently selected.
    	/// </summary>
    	public object[] Items
        {
            get { return _items.ToArray(); }
        }

		/// <summary>
		/// Returns the first item in the list of selected items, or null if nothing is selected.
		/// </summary>
        public object Item
        {
            get { return _items.Count > 0 ? _items[0] : null; }
        }

    	/// <summary>
    	/// Computes the union of this selection with another and returns it.
    	/// </summary>
    	public ISelection Union(ISelection other)
        {
            List<object> sum = new List<object>();

            // add all the items from the other selection
            sum.AddRange(other.Items);

            // add only the items from this selection not contained in the other selection
            sum.AddRange(_items.FindAll(delegate(object x) { return !other.Contains(x); }));

            return new Selection(sum);
        }

    	/// <summary>
    	/// Computes the intersection of this selection with another and returns it.
    	/// </summary>
    	public ISelection Intersect(ISelection other)
        {
            // return every item in this selection also contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return other.Contains(x); }));
        }

    	/// <summary>
    	/// Returns an <see cref="ISelection"/> that contains every item contained
    	/// in this one that doesn't exist in <param name="other" />.
    	/// </summary>
    	public ISelection Subtract(ISelection other)
        {
            // return every item in this selection not contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return !other.Contains(x); }));
        }

    	/// <summary>
    	/// Determines whether this selection contains the input object.
    	/// </summary>
    	public bool Contains(object item)
        {
            return _items.Contains(item);
        }

        #endregion

        #region IEquatable<ISelection> Members

		/// <summary>
		/// Gets whether or not this <see cref="ISelection"/> is considered equivalent to <paramref name="other"/>.
		/// </summary>
        public bool Equals(ISelection other)
        {
            if (other == null)
                return false;

            // false if not same number of elements
            if (other.Items.Length != _items.Count)
                return false;

            // because we now know that they contain the same number of elements,
            // they are equal if every item in this selection is contained in the other selection
            return _items.TrueForAll(
                delegate(object x) { return other.Contains(x); });
        }

        #endregion

		/// <summary>
		/// Gets whether or not this object is considered equal to another.
		/// </summary>
        public override bool Equals(object obj)
        {
            ISelection that = obj as ISelection;
            return this.Equals(that);
        }

		/// <summary>
		/// Gets a hash code for this object.
		/// </summary>
        public override int GetHashCode()
        {
            int n = 0;
            foreach (object item in _items)
            {
                n ^= item.GetHashCode();
            }
            return n;
        }
    }

	/// <summary>
	/// Generic implementation of <see cref="ISelection"/>.
	/// </summary>
	/// <typeparam name="T">The type of items in the selection.</typeparam>
	public class Selection<T> : ISelection, IEquatable<ISelection>, IEnumerable<T> where T : class
	{
		/// <summary>
		/// Represents an empty <see cref="Selection{T}"/>.
		/// </summary>
		public static readonly Selection<T> Empty = new Selection<T>();

		private readonly List<T> _items = new List<T>();

		/// <summary>
		/// Initializes an empty selection.
		/// </summary>
		public Selection() {}

		/// <summary>
		/// Initializes a selection that consists of a single item, or an empty selection if <typeparamref name="T"/> is a reference type and <paramref name="item"/> is null.
		/// </summary>
		/// <param name="item">The item in the selection, or null to specify an empty selection if <typeparamref name="T"/> is a reference type.</param>
		public Selection(T item)
		{
			if (!ReferenceEquals(item, null))
				_items.Add(item);
		}

		/// <summary>
		/// Initializes a selection from the specified enumeration.
		/// </summary>
		/// <param name="items">The items in the selection, or null to specify an empty selection.</param>
		public Selection(IEnumerable<T> items)
		{
			foreach (var item in items)
				if (!ReferenceEquals(item, null))
					_items.Add(item);
		}

		#region ISelection Members

		/// <summary>
		/// Returns the set of selected items.
		/// </summary>
		public T[] Items
		{
			get { return _items.ToArray(); }
		}

		object[] ISelection.Items
		{
			get { return CollectionUtils.Map<T, object>(_items, x => x).ToArray(); }
		}

		/// <summary>
		/// Gets the number of items in the selection.
		/// </summary>
		public int Count
		{
			get { return _items.Count; }
		}

		/// <summary>
		/// Returns the first item in the selection, or null if nothing is selected.
		/// </summary>
		public T Item
		{
			get { return _items.Count > 0 ? _items[0] : null; }
		}

		object ISelection.Item
		{
			get { return Item; }
		}

		/// <summary>
		/// Returns the union of this selection with another selection.
		/// </summary>
		/// <param name="other">The other selection with whom this selection is to be unioned.</param>
		public Selection<T> Union(ISelection other)
		{
			var sum = new List<T>();

			// add all the items from this selection
			sum.AddRange(_items);

			// add only the items of the correct type from the other selection not contained in this selection
			sum.AddRange(CollectionUtils.Map<object, T>(Array.FindAll(other.Items, x => x is T && !sum.Contains((T) x)), x => (T) x));

			return new Selection<T>(sum);
		}

		ISelection ISelection.Union(ISelection other)
		{
			return Union(other);
		}

		/// <summary>
		/// Returns the intersection of this selection with another selection.
		/// </summary>
		/// <param name="other">The other selection with whom this selection is to be intersected.</param>
		public Selection<T> Intersect(ISelection other)
		{
			// return every item in this selection also contained in the other selection
			return new Selection<T>(_items.FindAll(other.Contains));
		}

		ISelection ISelection.Intersect(ISelection other)
		{
			return Intersect(other);
		}

		/// <summary>
		/// Returns the subtraction of another selection from this selection.
		/// </summary>
		/// <param name="other">The other selection which is to be subtracted from this selection.</param>
		public Selection<T> Subtract(ISelection other)
		{
			// return every item in this selection not contained in the other selection
			return new Selection<T>(_items.FindAll(x => !other.Contains(x)));
		}

		ISelection ISelection.Subtract(ISelection other)
		{
			return Subtract(other);
		}

		/// <summary>
		/// Determines whether or not the specified item is in this selection.
		/// </summary>
		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		bool ISelection.Contains(object item)
		{
			return item is T && Contains((T) item);
		}

		#endregion

		#region IEquatable<ISelection> Members

		/// <summary>
		/// Gets whether or not this <see cref="ISelection"/> is considered equivalent to <paramref name="other"/>.
		/// </summary>
		public bool Equals(ISelection other)
		{
			if (other == null)
				return false;

			// false if not same number of elements
			if (other.Items.Length != _items.Count)
				return false;

			// because we now know that they contain the same number of elements,
			// they are equal if every item in this selection is contained in the other selection
			return _items.TrueForAll(other.Contains);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Gets whether or not this object is considered equal to another.
		/// </summary>
		public override bool Equals(object obj)
		{
			return Equals(obj as ISelection);
		}

		/// <summary>
		/// Gets a hash code for this object.
		/// </summary>
		public override int GetHashCode()
		{
			var hashCode = -0x1B97691D;
			foreach (var item in _items)
				hashCode ^= item.GetHashCode();
			return hashCode;
		}
	}
}
