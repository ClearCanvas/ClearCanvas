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

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Abstract base class for <see cref="IUndoableOperation{T}"/>.
	/// </summary>
	public abstract class UndoableOperation<T> : IUndoableOperation<T> where T : class
	{
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected UndoableOperation()
		{
		}

		#region IUndoableOperation<T> Members

		/// <summary>
		/// In the memento pattern, the 'originator' is the object whose state is being
		/// captured and restored via a memento.
		/// </summary>
		/// <remarks>
		/// In this interface definition, the originator is purposely not of <typeparamref name="T">type T</typeparamref>
		/// because you may actually want to perform the operation on an object that is not itself
		/// <see cref="IMemorable">memorable</see>, but rather on some <see cref="IMemorable">memorable</see> property.
		/// </remarks>
		public abstract IMemorable GetOriginator(T item);

		/// <summary>
		/// Gets whether or not this operation applies to the given item.
		/// </summary>
		/// <remarks>
		/// By default, simply returns whether or not <see cref="GetOriginator"/> returns for the given item.
		/// Subclasses can override this method to customize the behaviour.
		/// </remarks>
		public virtual bool AppliesTo(T item)
		{
			return GetOriginator(item) != null;
		}

		/// <summary>
		/// Applies the operation to the given item.
		/// </summary>
		public abstract void Apply(T item);

		#endregion
	}
}
