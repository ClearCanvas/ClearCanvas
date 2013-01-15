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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Event used to notify observers of a change in a collection.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the <see cref="ObservableList{TItem}"/>, but can be used
	/// for any collection-related event.
	/// </remarks>
	/// <typeparam name="TItem">The type of item in the collection.</typeparam>
	public class ListEventArgs<TItem> : EventArgs
	{
		private TItem _item;
		private int _index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The item that has changed.</param>
		/// <param name="index">The index of the item that has changed.</param>
		public ListEventArgs(TItem item, int index)
		{
			_item = item;
			_index = index;
		}

		/// <summary>
		/// Gets the item that has somehow changed in the related collection.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
		}

		/// <summary>
		/// Gets the index of the item that has somehow changed in the related collection.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}
	}
}
