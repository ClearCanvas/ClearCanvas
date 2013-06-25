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
using ClearCanvas.Common;
using vtk;

namespace ClearCanvas.ImageViewer.Vtk
{
	internal static class VtkCollectionHelper
	{
		public static IEnumerable<vtkObject> AsEnumerable(this vtkCollection collection)
		{
			return new Enumerable(collection);
		}

		public static IEnumerable<T> AsEnumerable<T>(this vtkCollection collection, Func<vtkObject, T> safeCastDelegate)
			where T : vtkObject
		{
			return new Enumerable<T>(collection, safeCastDelegate);
		}

		private class Enumerable : IEnumerable<vtkObject>
		{
			private readonly vtkCollection _collection;

			public Enumerable(vtkCollection collection)
			{
				_collection = collection;
			}

			public IEnumerator<vtkObject> GetEnumerator()
			{
				return new VtkCollectionEnumerator(_collection);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private class Enumerable<T> : IEnumerable<T>
			where T : vtkObject
		{
			private readonly vtkCollection _collection;
			private readonly Func<vtkObject, T> _safeCastDelegate;

			public Enumerable(vtkCollection collection, Func<vtkObject, T> safeCastDelegate)
			{
				_collection = collection;
				_safeCastDelegate = safeCastDelegate;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return new VtkCollectionEnumerator<T>(_collection, _safeCastDelegate);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}

	internal class VtkCollectionEnumerator : IEnumerator<vtkObject>
	{
		private const int _vtkNo = 0;

		private vtkCollection _collection;
		private vtkCollectionIterator _iterator;
		private bool _reset = true;

		public VtkCollectionEnumerator(vtkCollection collection)
		{
			_collection = collection;

			if (_collection != null)
			{
				_iterator = new vtkCollectionIterator();
				_iterator.SetCollection(_collection);
				_iterator.InitTraversal();
			}
		}

		public void Dispose()
		{
			if (_iterator != null)
			{
				_iterator.Dispose();
				_iterator = null;
			}

			_collection = null;
		}

		public bool MoveNext()
		{
			if (_iterator == null)
				return false;

			if (_reset)
			{
				_iterator.GoToFirstItem();
				_reset = false;
			}
			else if (_iterator.IsDoneWithTraversal() == _vtkNo)
			{
				_iterator.GoToNextItem();
			}

			return _iterator.IsDoneWithTraversal() == _vtkNo;
		}

		public void Reset()
		{
			if (_iterator == null)
				return;

			_reset = true;
		}

		public vtkObject Current
		{
			get
			{
				if (_iterator == null || _iterator.IsDoneWithTraversal() != _vtkNo)
					throw new InvalidOperationException();

				return _iterator.GetCurrentObject();
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}

	internal class VtkCollectionEnumerator<T> : IEnumerator<T>
		where T : vtkObject
	{
		private Func<vtkObject, T> _safeCastDelegate;
		private VtkCollectionEnumerator _enumerator;

		public VtkCollectionEnumerator(vtkCollection collection, Func<vtkObject, T> safeCastDelegate)
		{
			Platform.CheckForNullReference(safeCastDelegate, "safeCastDelegate");
			_safeCastDelegate = safeCastDelegate;
			_enumerator = new VtkCollectionEnumerator(collection);
		}

		public void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}

			_safeCastDelegate = null;
		}

		public bool MoveNext()
		{
			return _enumerator != null && _enumerator.MoveNext();
		}

		public void Reset()
		{
			if (_enumerator == null)
				return;

			_enumerator.Reset();
		}

		public T Current
		{
			get
			{
				if (_enumerator == null)
					throw new InvalidOperationException();

				var current = _safeCastDelegate.Invoke(_enumerator.Current);
				if (current == null)
					throw new InvalidCastException();

				return current;
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}