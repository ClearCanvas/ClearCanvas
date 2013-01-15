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

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Holds arguments passed to a worklist item broker to perform a search.
	/// </summary>
	public class WorklistItemSearchArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="searchCriteria"></param>
		/// <param name="projection"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		/// <param name="threshold"></param>
		public WorklistItemSearchArgs(
			Type[] procedureStepClasses,
			WorklistItemSearchCriteria[] searchCriteria,
			WorklistItemProjection projection,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems,
			int threshold)
		{
			ProcedureStepClasses = procedureStepClasses;
			IncludeDegeneratePatientItems = includeDegeneratePatientItems;
			IncludeDegenerateProcedureItems = includeDegenerateProcedureItems;
			Threshold = threshold;
			SearchCriteria = searchCriteria;
			Projection = projection;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="searchCriteria"></param>
		/// <param name="projection"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		public WorklistItemSearchArgs(
			Type[] procedureStepClasses,
			WorklistItemSearchCriteria[] searchCriteria,
			WorklistItemProjection projection,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems)
			: this(procedureStepClasses, searchCriteria, projection, includeDegeneratePatientItems, includeDegenerateProcedureItems, 0)
		{
		}

		/// <summary>
		/// Gets a value indicating whether to include results for patients that meet the criteria but do not have any procedures.
		/// </summary>
		public bool IncludeDegeneratePatientItems { get; private set; }

		/// <summary>
		/// Gets a value indicating whether to include results for procedures that meet the criteria but do not have an active procedure step.
		/// </summary>
		public bool IncludeDegenerateProcedureItems { get; private set; }

		/// <summary>
		/// Gets the maximum number of items that the search may return.
		/// </summary>
		public int Threshold { get; private set; }

		/// <summary>
		/// Gets the procedure step classes that are considered in the search.
		/// </summary>
		public Type[] ProcedureStepClasses { get; private set; }

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		public WorklistItemSearchCriteria[] SearchCriteria { get; private set; }

		/// <summary>
		/// Gets the projection.
		/// </summary>
		public WorklistItemProjection Projection { get; private set; }
	}
}
