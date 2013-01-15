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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Represents a "prior", which is essentially a tuple consisting of a procedure, its associated order and report.
	/// </summary>
	public class Prior
	{
		public Prior(Report r, Procedure rp, ProcedureType pt, Order o)
		{
			this.Report = r;
			this.Procedure = rp;
			this.ProcedureType = pt;
			this.Order = o;
		}

		public Report Report { get; private set; }
		public Procedure Procedure { get; private set; }
		public ProcedureType ProcedureType { get; private set; }
		public Order Order { get; private set; }
	}

	public interface IPriorReportBroker : IPersistenceBroker
	{
		/// <summary>
		/// Obtains the set of procedure types that are relevant to the specified procedure type.
		/// </summary>
		/// <param name="procType"></param>
		/// <returns></returns>
		IList<ProcedureType> GetRelevantProcedureTypes(ProcedureType procType);

		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified report,
		/// optionally filtering by relevancy to the specified report.
		/// </summary>
		/// <returns></returns>
		IList<Prior> GetPriors(Report report, bool relevantOnly);

		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified order,
		/// optionally filtering by relevancy to the specified order.
		/// </summary>
		/// <returns></returns>
		IList<Prior> GetPriors(Order order, bool relevantOnly);
	}
}
