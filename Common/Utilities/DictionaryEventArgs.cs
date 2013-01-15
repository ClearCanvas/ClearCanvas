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
	/// Event used to notify observers of a change in a dictionary.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the <see cref="ObservableDictionary{TKey,TItem}"/>, but can be used
	/// for any dictionary-related event.
	/// </remarks>
	/// <typeparam name="TKey">The type of key in the dictionary.</typeparam>
	/// <typeparam name="TItem">The type of item in the dictionary.</typeparam>
	public class DictionaryEventArgs<TKey, TItem> : EventArgs
	{
		private TKey _key;
		private TItem _item;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key">The key for the <paramref name="item"/> that has changed.</param>
		/// <param name="item">The item that has changed.</param>
		public DictionaryEventArgs(TKey key, TItem item)
		{
			Platform.CheckForNullReference(key, "key");
			_key = key;
			_item = item;
		}

		/// <summary>
		/// Gets the key for the <see cref="Item"/> that has changed.
		/// </summary>
		public TKey Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets the item that has changed.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
		}
	}
}
