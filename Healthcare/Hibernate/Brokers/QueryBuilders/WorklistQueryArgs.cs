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

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="QueryBuilderArgs"/> used for worklist item queries.
	/// </summary>
	public class WorklistQueryArgs : QueryBuilderArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <param name="countQuery"></param>
		public WorklistQueryArgs(Worklist worklist, IWorklistQueryContext wqc, bool countQuery)
		{
			this.Worklist = worklist;
			this.QueryContext = wqc;
			this.FilterCriteria = worklist.GetFilterCriteria(wqc);

			// init base class
			Initialize(
				worklist.GetProcedureStepSubclasses(),
				worklist.GetInvariantCriteria(wqc),
				countQuery ? null : worklist.GetProjection(),
				wqc.Page);
		}

		/// <summary>
		/// Gets the worklist that is generating the query.
		/// </summary>
		public Worklist Worklist { get; private set; }

		/// <summary>
		/// Gets the worklist query context.
		/// </summary>
		public IWorklistQueryContext QueryContext { get; private set; }

		/// <summary>
		/// Gets the worklist filter criteria.
		/// </summary>
		public WorklistItemSearchCriteria[] FilterCriteria { get; private set; }
	}
}
