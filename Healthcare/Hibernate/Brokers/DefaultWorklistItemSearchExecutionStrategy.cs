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

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Encapsulates the default worklist search execution strategy.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The default strategy is the naive implementation.  It sends the minimum possible number of queries
	/// to the database to perform the search, which means it relies on the database entirely to optimize
	/// the execution of those queries.
	/// </para>
	/// <para>
	/// The concept of "degenerate" worklist items requires some explanation.  If a worklist item is thought of
	/// as a tuple containing Patient Profile, Order, Procedure, and Procedure Step information
	/// e.g.
	/// ( pp, o, rp, ps )
	/// the a "degenerate procedure" worklist item is an item of the form
	/// ( pp, o, rp, null )
	/// and a "degenerate patient" worklist item is an item of the form
	/// ( pp, null, null, null )
	/// 
	/// </para>
	/// </remarks>
	class DefaultWorklistItemSearchExecutionStrategy : WorklistItemSearchExecutionStrategy
	{
		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public override IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc)
		{
			var where = wisc.SearchCriteria;
			var results = new List<WorklistItem>();

			results = UnionMerge(results, wisc.FindWorklistItems(where), item => item.ProcedureRef);

			// include degenerate procedure items if requested
			if (wisc.IncludeDegenerateProcedureItems)
			{
				// search for procedures
				results = UnionMerge(results, wisc.FindProcedures(where), item => item.ProcedureRef);
			}

			// include degenerate patient items if requested
			if (wisc.IncludeDegeneratePatientItems)
			{
				// add any patients for which there is no result
				results = UnionMerge(results, wisc.FindPatients(where), item => item.PatientRef);
			}

			return results;
		}

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override bool EstimateSearchResultsCount(IWorklistItemSearchContext wisc, out int count)
		{
			count = 0;

			// if no degenerate items are included, we need to do exactly one query for worklist items,
			// no estimation is possible
			if (!wisc.IncludeDegeneratePatientItems && !wisc.IncludeDegenerateProcedureItems)
			{
				// search for worklist items, delegating the task of designing the query to the subclass
				count = wisc.CountWorklistItems(wisc.SearchCriteria);

				// return whether the count exceeded the threshold
				return count <= wisc.Threshold;
			}

			// if some degenerate items are to be included, then we can omit querying for "active worklist items", e.g. ProcedureSteps,
			// because the degenerate set is by definition a superset of the active items
			// Strategy:
			// Valid search fields are:
			// - Patient: Name, MRN, Healthcard
			// - Order/Procedure: Accession, Ordering Prac, Procedure Type, Date Range
			// The approach taken here is to perform a patient count query and a procedure count query.
			// The patient query will count all potential patient matches based on Patient-applicable search fields.
			// The procedure count query will count all potential procedure matches based on both Patient and Order/Procedure search fields.
			// If either count exceeds the threshold, we can bail immediately.
			// Otherwise, the counts must be combined.  Note that each count represents a potentially overlapping
			// set of items, so there is no possible way to determine an 'exact' count (hence the word Estimate).
			// However, we know that the true count is a) greater than or equal to the maximum of either independent count, and
			// b) less than or equal to the sum of both counts.  Therefore, choose the midpoint of this number as a
			// 'good enough' estimate.

			var numPatients = 0;
			if (wisc.IncludeDegeneratePatientItems)
			{
				// count number of patient matches
				numPatients = wisc.CountPatients(wisc.SearchCriteria);

				// if this number exceeds threshold, bail
				if (numPatients > wisc.Threshold)
					return false;
			}

			var numProcedures = 0;
			if (wisc.IncludeDegenerateProcedureItems)
			{
				// count number of procedure matches
				numProcedures = wisc.CountProcedures(wisc.SearchCriteria);

				// if this number exceeds threshold, bail
				if (numProcedures > wisc.Threshold)
					return false;
			}

			// combine the two numbers to produce a guess at the actual number of results
			count = (Math.Max(numPatients, numProcedures) + numPatients + numProcedures) / 2;

			// return whether the count exceeded the threshold
			return count <= wisc.Threshold;
		}
	}
}
