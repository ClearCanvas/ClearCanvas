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
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IPatientHistoryBroker : IPersistenceBroker
    {
		/// <summary>
		/// Obtains the set of all orders for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
        IList<Order> GetOrderHistory(Patient patient);

		/// <summary>
		/// Obtains the set of all procedures for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
        IList<Procedure> GetProcedureHistory(Patient patient);

		/// <summary>
		/// Obtains the set of all reports for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
		IList<Report> GetReportHistory(Patient patient);

    	/// <summary>
    	/// Obtains the set of all reports for the specified order.
    	/// </summary>
    	/// <param name="order"></param>
    	/// <returns></returns>
    	IList<Report> GetReportsForOrder(Order order);
    }
}
