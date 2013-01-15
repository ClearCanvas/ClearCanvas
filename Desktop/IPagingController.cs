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
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Event args for paged changed event.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class PageChangedEventArgs<TItem> : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="items"></param>
		public PageChangedEventArgs(IList<TItem> items)
		{
			this.Items = items;
		}

		/// <summary>
		/// Gets the new set of items for the current page.
		/// </summary>
		public IList<TItem> Items { get; private set; }
	}


	/// <summary>
	/// Defines an interface to control pagination through a list of items.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public interface IPagingController<TItem>
	{
		/// <summary>
		/// Gets the number of items per page.
		/// </summary>
		int PageSize { get; }

		/// <summary>
		/// Gets a value indicating whether there is a next page.
		/// </summary>
		/// <returns></returns>
		bool HasNext { get; }

		/// <summary>
		/// Gets a value indicating whether there is a previous page.
		/// </summary>
		/// <returns></returns>
		bool HasPrevious { get; }

		/// <summary>
		/// Gets the next page of items.
		/// </summary>
		/// <returns></returns>
		void GetNext();

		/// <summary>
		/// Gets the previous page of items.
		/// </summary>
		/// <returns></returns>
		void GetPrevious();

		/// <summary>
		/// Resets this instance to the first page of items.
		/// </summary>
		/// <returns></returns>
		void GetFirst();

		/// <summary>
		/// Occurs when the current page changes (by calling any of <see cref="GetFirst"/>, <see cref="GetNext"/> or <see cref="GetPrevious"/>.
		/// </summary>
		event EventHandler<PageChangedEventArgs<TItem>> PageChanged;
	}
}
