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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An observable list of points defining an <see cref="IGraphic"/>.
	/// </summary>
	public sealed class PointsList : IPointsList
	{
		private readonly List<PointF> _sourcePoints = new List<PointF>();
		private readonly IGraphic _owner;
		private bool _enableEvents = true;

		/// <summary>
		/// Constructs a new <see cref="PointsList"/> with the specified owning graphic.
		/// </summary>
		/// <param name="owner">The owner graphic whose <see cref="CoordinateSystem"/> is used when interpreting points in the list.</param>
		public PointsList(IGraphic owner)
		{
			_owner = owner;
		}

		/// <summary>
		/// Constructs a new <see cref="PointsList"/> with the specified owning graphic.
		/// </summary>
		/// <param name="points">A list of points in the <see cref="CoordinateSystem.Source"/> coordinate system with which to initialize the list.</param>
		/// <param name="owner">The owner graphic whose <see cref="CoordinateSystem"/> is used when interpreting points in the list.</param>
		public PointsList(IEnumerable<PointF> points, IGraphic owner)
			: this(owner)
		{
			foreach (PointF point in points)
				_sourcePoints.Add(point);
		}

		/// <summary>
		/// Gets a value indicating if the first and last points of the list are coincident.
		/// </summary>
		public bool IsClosed
		{
			get { return _sourcePoints.Count > 2 && FloatComparer.AreEqual(_sourcePoints[0], _sourcePoints[_sourcePoints.Count - 1]); }
		}

		/// <summary>
		/// Suspends notification of the <see cref="IPointsList.PointAdded"/>, <see cref="IPointsList.PointChanged"/>, <see cref="IPointsList.PointRemoved"/> and <see cref="IPointsList.PointsCleared"/> events.
		/// </summary>
		public void SuspendEvents()
		{
			_enableEvents = false;
		}

		/// <summary>
		/// Resumes notification of the <see cref="IPointsList.PointAdded"/>, <see cref="IPointsList.PointChanged"/>, <see cref="IPointsList.PointRemoved"/> and <see cref="IPointsList.PointsCleared"/> events.
		/// </summary>
		public void ResumeEvents()
		{
			_enableEvents = true;
		}

		/// <summary>
		/// Occurs when a point is added to the list.
		/// </summary>
		public event EventHandler<IndexEventArgs> PointAdded;

		/// <summary>
		/// Occurs when the value of a point in the list has changed.
		/// </summary>
		public event EventHandler<IndexEventArgs> PointChanged;

		/// <summary>
		/// Occurs when a point is removed from the list.
		/// </summary>
		public event EventHandler<IndexEventArgs> PointRemoved;

		/// <summary>
		/// Occurs when the list is cleared.
		/// </summary>
		public event EventHandler PointsCleared;

		private void NotifyPointAdded(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointAdded, this, new IndexEventArgs(index));
		}

		private void NotifyPointRemoved(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointRemoved, this, new IndexEventArgs(index));
		}

		private void NotifyPointChanged(int index)
		{
			if (_enableEvents)
				EventsHelper.Fire(PointChanged, this, new IndexEventArgs(index));
		}

		private void NotifyPointsCleared()
		{
			if (_enableEvents)
				EventsHelper.Fire(PointsCleared, this, new EventArgs());
		}

		///<summary>
		///Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
		///</summary>
		///
		///<returns>
		///The index of item if found in the list; otherwise, -1.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
		public int IndexOf(PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);

			return _sourcePoints.IndexOf(item);
		}

		///<summary>
		///Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
		///</summary>
		///
		///<param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
		///<param name="index">The zero-based index at which item should be inserted.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public void Insert(int index, PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);

			_sourcePoints.Insert(index, item);
			NotifyPointAdded(index);
		}

		///<summary>
		///Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
		///</summary>
		///
		///<param name="index">The zero-based index of the item to remove.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public void RemoveAt(int index)
		{
			_sourcePoints.RemoveAt(index);
			NotifyPointRemoved(index);
		}

		///<summary>
		///Gets or sets the element at the specified index.
		///</summary>
		///
		///<returns>
		///The element at the specified index.
		///</returns>
		///
		///<param name="index">The zero-based index of the element to get or set.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
		public PointF this[int index]
		{
			get
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					return _owner.SpatialTransform.ConvertToDestination(_sourcePoints[index]);
				return _sourcePoints[index];
			}
			set
			{
				if (_owner.CoordinateSystem == CoordinateSystem.Destination)
					value = _owner.SpatialTransform.ConvertToSource(value);

				if (!FloatComparer.AreEqual(_sourcePoints[index], value))
				{
					_sourcePoints[index] = value;
					NotifyPointChanged(index);
				}
			}
		}

		///<summary>
		///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public void Add(PointF item)
		{
			this.Insert(_sourcePoints.Count, item);
		}

		public void AddRange(IEnumerable<PointF> points)
		{
			foreach (var point in points)
				Insert(_sourcePoints.Count, point);
		}

		///<summary>
		///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
		public void Clear()
		{
			_sourcePoints.Clear();
			NotifyPointsCleared();
		}

		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
		///</summary>
		///
		///<returns>
		///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		public bool Contains(PointF item)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				item = _owner.SpatialTransform.ConvertToSource(item);
			return _sourcePoints.Contains(item);
		}

		///<summary>
		///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
		///</summary>
		///
		///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
		///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		///<exception cref="T:System.ArgumentNullException">array is null.</exception>
		///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
		public void CopyTo(PointF[] array, int arrayIndex)
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
			{
				foreach (PointF point in this)
					array[arrayIndex++] = point;

				return;
			}
			_sourcePoints.CopyTo(array, arrayIndex);
		}

		///<summary>
		///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<returns>
		///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</returns>
		///
		public int Count
		{
			get { return _sourcePoints.Count; }
		}

		///<summary>
		///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		///</summary>
		///
		///<returns>
		///true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
		///</returns>
		///
		bool ICollection<PointF>.IsReadOnly
		{
			get { return false; }
		}

		///<summary>
		///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<returns>
		///true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</returns>
		///
		///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		public bool Remove(PointF item)
		{
			int index = _sourcePoints.IndexOf(item);
			if (index < 0)
				return false;
			this.RemoveAt(index);
			return true;
		}

		private IEnumerator<PointF> EnumerateDestinationPoints()
		{
			foreach (PointF point in _sourcePoints)
				yield return _owner.SpatialTransform.ConvertToDestination(point);
		}

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		public IEnumerator<PointF> GetEnumerator()
		{
			if (_owner.CoordinateSystem == CoordinateSystem.Destination)
				return this.EnumerateDestinationPoints();
			return _sourcePoints.GetEnumerator();
		}

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}