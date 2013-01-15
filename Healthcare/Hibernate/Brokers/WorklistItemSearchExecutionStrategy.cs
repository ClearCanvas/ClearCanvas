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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Defines an interface to an object that encapsulates information about a search to be executed.
	/// </summary>
	interface IWorklistItemSearchContext
	{
		/// <summary>
		/// Gets a value indicating whether degenerate procedure worklist items should be included
		/// in the search results.
		/// </summary>
		bool IncludeDegenerateProcedureItems { get; }

		/// <summary>
		/// Gets a value indicating whether degenerate patient worklist items should be included
		/// in the search results.
		/// </summary>
		bool IncludeDegeneratePatientItems { get; }

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		WorklistItemSearchCriteria[] SearchCriteria { get; }

		/// <summary>
		/// Gets the maximum number of allowed hits.  A search that would return more hits should not execute.
		/// </summary>
		int Threshold { get; }

		/// <summary>
		/// Find matching worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindWorklistItems(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountWorklistItems(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Find matching procedures.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindProcedures(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching procedures.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountProcedures(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Find matching patients.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindPatients(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching patients.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountPatients(WorklistItemSearchCriteria[] where);

	}

	/// <summary>
	/// Defines an interface to a class that encapsulates a strategy for executing a worklist search.
	/// </summary>
	interface IWorklistItemSearchExecutionStrategy
	{
		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		bool EstimateSearchResultsCount(IWorklistItemSearchContext wisc, out int count);
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IWorklistItemSearchExecutionStrategy"/>.
	/// </summary>
	abstract class WorklistItemSearchExecutionStrategy : IWorklistItemSearchExecutionStrategy
	{
		#region IWorklistItemSearchExecutionStrategy Members

		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public abstract IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public abstract bool EstimateSearchResultsCount(IWorklistItemSearchContext wisc, out int count);

		#endregion

		/// <summary>
		/// Performs a union of the primary and secondary sets.
		/// </summary>
		/// <remarks>
		/// The specified identity-provider is used to determine identity of two items, hence whether the same logical
		/// item appears in both sets.  If the same logical item appears in both sets, the actual item from the primary set will
		/// always be chosen.  The union is returned as a new list (the arguments are not modified).
		/// </remarks>
		/// <param name="primary"></param>
		/// <param name="secondary"></param>
		/// <param name="identityProvider"></param>
		/// <returns></returns>
		protected static List<WorklistItem> UnionMerge(IList<WorklistItem> primary, IList<WorklistItem> secondary,
			Converter<WorklistItem, EntityRef> identityProvider)
		{
			// note that we do not modify the arguments
			var merged = new List<WorklistItem>(primary);
			foreach (var s in secondary)
			{
				if (!CollectionUtils.Contains(primary,
						p => identityProvider(s).Equals(identityProvider(p), true)))
				{
					merged.Add(s);
				}
			}
			return merged;
		}
	}
}
