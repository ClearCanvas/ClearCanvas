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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IDisplaySet"/> objects.
	/// </summary>
	public class DisplaySetCollection : ObservableList<IDisplaySet>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="DisplaySetCollection"/>.
		/// </summary>
		public DisplaySetCollection()
		{
		}

		internal static IComparer<IDisplaySet> GetDefaultComparer()
		{
			return new DisplaySetNumberComparer();
		}

		/// <summary>
		/// The comparer that was last used to sort the collection, via <see cref="Sort"/>.
		/// </summary>
		/// <remarks>
		/// When an item is added to or replaced, this value is set to null.  When an item is
		/// simply removed, the sort order is maintained, so this value also will not change.
		/// </remarks>
		public IComparer<IDisplaySet> SortComparer { get; internal set; }

		/// <summary>
		/// Sorts the collection using <see cref="SortComparer"/>.
		/// </summary>
		/// <remarks>
		/// If <see cref="SortComparer"/> is null, it is first set to a default one.
		/// </remarks>
		public void Sort()
		{
			if (SortComparer == null)
				SortComparer = GetDefaultComparer();
			Sort(SortComparer);
		}

		/// <summary>
		/// Sorts the collection with the given comparer.
		/// </summary>
		public sealed override void Sort(IComparer<IDisplaySet> sortComparer)
		{
			Platform.CheckForNullReference(sortComparer, "comparer");
			SortComparer = sortComparer;
			base.Sort(SortComparer);
		}

		protected override void OnItemAdded(ListEventArgs<IDisplaySet> e)
		{
			SortComparer = null; //we don't know the sort order anymore.
			base.OnItemAdded(e);
		}

		protected override void OnItemChanged(ListEventArgs<IDisplaySet> e)
		{
			SortComparer = null;//we don't know the sort order anymore.
			base.OnItemChanged(e);
		}
	}
}