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
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IStudyFilter
	{
		IList<IStudyItem> Items { get; }
		StudyItemSelection Selection { get; }

		event EventHandler ItemAdded;
		event EventHandler ItemRemoved;

		bool IsStale { get; }
		event EventHandler IsStaleChanged;

		bool FilterPredicatesEnabled { get; set; }
		event EventHandler FilterPredicatesEnabledChanged;

		/// <summary>
		/// Gets the collection of columns available on the Study Filter.
		/// </summary>
		IStudyFilterColumnCollection Columns { get; }

		/// <summary>
		/// Gets the current sort predicates (i.e. relational predicates) applied to the Study Filter.
		/// </summary>
		IList<SortPredicate> SortPredicates { get; }

		/// <summary>
		/// Gets the current filter predicates (i.e. functional predicates) applied to the Study Filter.
		/// </summary>
		IList<FilterPredicate> FilterPredicates { get; }

		/// <summary>
		/// If the displayed data is stale, reapplies the predicates to the dataset and updates the display.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Reapplies the predicates to the dataset and updates the display.
		/// </summary>
		/// <param name="force">A value indicating whether or not to perform the refresh even if the data is not stale.</param>
		void Refresh(bool force);
		event EventHandler FilterPredicatesChanged;
		event EventHandler SortPredicatesChanged;
		
	}
}