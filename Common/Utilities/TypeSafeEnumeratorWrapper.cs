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
    /// Utility class used to wrap an untyped <see cref="IEnumerator"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be enumerated.</typeparam>
    public class TypeSafeEnumeratorWrapper<T> : IEnumerator<T>
    {
        private IEnumerator _inner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inner">The untyped <see cref="IEnumerator" /> to be wrapped.</param>
        public TypeSafeEnumeratorWrapper(IEnumerator inner)
        {
            _inner = inner;
        }

        #region IEnumerator<T> Members

		/// <summary>
		/// Gets the object at the current <see cref="IEnumerator{T}"/> position.
		/// </summary>
        public T Current
        {
            get { return (T)_inner.Current; }
        }

        #endregion

        #region IEnumerator Members

		/// <summary>
		/// Gets the object at the current <see cref="IEnumerator"/> position.
		/// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return _inner.Current; }
        }

		/// <summary>
		/// Moves to the next element.
		/// </summary>
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

		/// <summary>
		/// Resets the <see cref="IEnumerator"/>.
		/// </summary>
        public void Reset()
        {
            _inner.Reset();
        }

        #endregion

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}
