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

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Acts as a proxy to a <see cref="IList{T}"/> for deferred query results.
	/// </summary>
	/// <remarks>
	/// Accessing any member of the <see cref="IList{T}"/> interface on this object will cause
	/// the deferred query to execute, if it has not already been executed, so as to obtain
	/// the contents of the list.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	internal class DeferredQueryResultList<T> : IList<T>
	{
		private readonly object _token;
		private readonly QueryExecutor _executor;
		private IList<T> _innerList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="executor"></param>
		/// <param name="token"></param>
		internal DeferredQueryResultList(QueryExecutor executor, object token)
		{
			_token = token;
			_executor = executor;
		}

		#region IList<T> members

		public IEnumerator<T> GetEnumerator()
		{
			return MaterializedList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			MaterializedList.Add(item);
		}

		public void Clear()
		{
			MaterializedList.Clear();
		}

		public bool Contains(T item)
		{
			return MaterializedList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			MaterializedList.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return MaterializedList.Remove(item);
		}

		public int Count
		{
			get { return MaterializedList.Count; }
		}

		public bool IsReadOnly
		{
			get { return MaterializedList.IsReadOnly; }
		}

		public int IndexOf(T item)
		{
			return MaterializedList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			MaterializedList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			MaterializedList.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return MaterializedList[index]; }
			set { MaterializedList[index] = value; }
		}

		#endregion

		private IList<T> MaterializedList
		{
			get
			{
				if (_innerList == null)
				{
					_innerList = _executor.GetResult<T>(_token);
				}
				return _innerList;
			}
		}

	}
}
