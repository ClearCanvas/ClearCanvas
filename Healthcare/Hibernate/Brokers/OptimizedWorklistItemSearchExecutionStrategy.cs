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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// </summary>
	/// <remarks>
	/// The current ad-hoc query design consists of a single string query
	/// which is parsed into a set of names and identifiers, which are matched on only a small subset of 
	/// fields:
	/// 
	/// M = Patient MRN
	/// N = Patient Name
	/// H = Healthcard Number
	/// A = Accession Number
	/// 
	/// If the query string parsing results in multiple values, those values are combined using OR logic.
	/// eg. M + N + H + A
	/// 
	/// The search criteria also include a condition on the Downtime Recovery Mode flag (D), in order to
	/// partition the procedure space into Downtime-Recovery procedures vs Live procedures.  Hence,
	/// the boolean expression for the complete query is always of the form:
	/// 
	/// D(M + N + H + A)
	/// 
	/// When this query form is processed into a set of <see cref="WorklistItemSearchCriteria"/> objects,
	/// the resulting array can be visualized as a sparse matrix of boolean variables. 
	/// The fixed query form DM + DN + DH + DA means the resulting condition matrix will *always* have
	/// the following general form:
	/// 
	/// [M 1 D]
	/// [N 1 D]
	/// [H 1 D]
	/// [1 A D]
	/// 
	/// The optimization employed by this strategy is to split the above matrix into two parts:
	/// 
	/// [M 1 D]
	/// [N 1 D]
	/// [H 1 D]
	/// 
	/// and 
	/// 
	/// [1 A D]
	/// 
	/// (eg rows that contain patient conditions, and rows that do not).
	/// Two queries are then executed: one query for the first 3 rows, and a second query for 
	/// the last row.  Because the matrix was split horizontally, the result sets of each query
	/// can simply be unioned together.
	/// 
	/// The rational for this strategy is based on the empirical observation that SQL Server
	/// is able to determine an efficient query plan for a D(M + N + H) query and a DA query
	/// independently, but it does not seem to be able to find an efficient query plan for the
	/// full D(M + N + H + A) query.  It is not known why this is the case, but clearly the fact
	/// that M, N, H reside in the same table, whereas A resides in a different table, plays a role.
	/// 
	/// In contrast, the so-called "Advanced Search" design yields a condition matrix of an entirely
	/// different form that generally looks something like:
	/// 
	/// [M N H A X.... D]
	/// [M N H A Y.... D]
	/// [M N H A Z.... D]
	/// 
	/// where X, Y, Z are conditions on the procedure scheduled, start, and end times.  Hence it is not
	/// clear whether this strategy performs better or worse than the <see cref="DefaultWorklistItemSearchExecutionStrategy"/>
	/// in this case.
	/// </remarks>
	class OptimizedWorklistItemSearchExecutionStrategy : WorklistItemSearchExecutionStrategy
	{
		private const string PatientProfileKey = "PatientProfile";

		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public override IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc)
		{
			var results = new List<WorklistItem>();

			// the search criteria is broken up to gain performance. See #2416.
			var criteriaWithPatientConditions = SelectCriteriaWithPatientConditions(wisc.SearchCriteria);
			var criteriaWithoutPatientConditions = ExcludeCriteriaWithPatientConditions(wisc.SearchCriteria);

			if (criteriaWithPatientConditions.Length > 0)
			{
				results = UnionMerge(results, wisc.FindWorklistItems(criteriaWithPatientConditions),
					item => item.ProcedureRef);
			}

			if (criteriaWithoutPatientConditions.Length > 0)
			{
				results = UnionMerge(results, wisc.FindWorklistItems(criteriaWithoutPatientConditions),
					item => item.ProcedureRef);
			}

			// include procedure degenerate items if requested
			if (wisc.IncludeDegenerateProcedureItems)
			{
				// search for procedures
				if (criteriaWithPatientConditions.Length > 0)
				{
					results = UnionMerge(results, wisc.FindProcedures(criteriaWithPatientConditions),
					                     item => item.ProcedureRef);
				}

				if (criteriaWithoutPatientConditions.Length > 0)
				{
					results = UnionMerge(results, wisc.FindProcedures(criteriaWithoutPatientConditions),
					                     item => item.ProcedureRef);
				}
			}

			// include patient degenerate items if requested
			if (wisc.IncludeDegeneratePatientItems)
			{
				// search for patients
				if (criteriaWithPatientConditions.Length > 0)
				{
					// add any patients for which there is no result
					results = UnionMerge(results, wisc.FindPatients(criteriaWithPatientConditions),
						item => item.PatientRef);
				}
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
			// - Order/Procedure: Accession
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
				// find procedures based on the patient search fields
				var criteriaWithPatientConditions = SelectCriteriaWithPatientConditions(wisc.SearchCriteria);
				if (criteriaWithPatientConditions.Length > 0)
				{
					numProcedures += wisc.CountProcedures(criteriaWithPatientConditions);

					// if this number exceeds threshold, bail
					if (numProcedures > wisc.Threshold)
						return false;
				}

				// find procedures based on the order search fields
				var criteriaWithoutPatientConditions = ExcludeCriteriaWithPatientConditions(wisc.SearchCriteria);
				if (criteriaWithoutPatientConditions.Length > 0)
				{
					numProcedures += wisc.CountProcedures(criteriaWithoutPatientConditions);

					// if this number exceeds threshold, bail
					if (numProcedures > wisc.Threshold)
						return false;
				}
			}

			// combine the two numbers to produce a guess at the actual number of results
			count = (Math.Max(numPatients, numProcedures) + numPatients + numProcedures) / 2;

			// return whether the count exceeded the threshold
			return count <= wisc.Threshold;
		}

		/// <summary>
		/// Returns only those <see cref="WorklistItemSearchCriteria"/> elements that have patient search conditions present.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		private static WorklistItemSearchCriteria[] SelectCriteriaWithPatientConditions(IEnumerable<WorklistItemSearchCriteria> where)
		{
			return CollectionUtils.Select(where, sc => !sc.PatientProfile.IsEmpty).ToArray();
		}

		/// <summary>
		/// Returns only those <see cref="WorklistItemSearchCriteria"/> elements that do not have patient search conditions present.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		private static WorklistItemSearchCriteria[] ExcludeCriteriaWithPatientConditions(IEnumerable<WorklistItemSearchCriteria> where)
		{
			return CollectionUtils.Select(where, sc => sc.PatientProfile.IsEmpty).ToArray();
		}
	}
}
