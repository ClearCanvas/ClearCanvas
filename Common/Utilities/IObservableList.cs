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

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Interface to an observable list.
	/// </summary>
	public interface IObservableList<TItem> : IList<TItem>
	{
		/// <summary>
		/// Fired when an item is added to the list.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemAdded;

		/// <summary>
		/// Fired when an item is removed from the list.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemRemoved;

		/// <summary>
		/// Fired when an item in the list has changed.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemChanged;

		/// <summary>
		/// Fires when an item in the list is about to change.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemChanging;
	}
}