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

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IEnumerable"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be enumerated.</typeparam>
    public class TypeSafeEnumerableWrapper<T> : IEnumerable<T>
    {
        private IEnumerable _inner;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="inner">The untyped <see cref="IEnumerable"/> object to wrap.</param>
		public TypeSafeEnumerableWrapper(IEnumerable inner)
        {
            _inner = inner;
        }

        #region IEnumerable<T> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{T}"/> for the wrapped object.
		/// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new TypeSafeEnumeratorWrapper<T>(_inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an <see cref="IEnumerator"/> for the wrapped object.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        #endregion
    }
}
