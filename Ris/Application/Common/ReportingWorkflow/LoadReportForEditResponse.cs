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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class LoadReportForEditResponse : DataContractBase
    {
		public LoadReportForEditResponse(ReportDetail report, int reportPartIndex, OrderDetail order, List<EnumValueInfo> priorityChoices)
    	{
    		Report = report;
    		ReportPartIndex = reportPartIndex;
    		Order = order;
			PriorityChoices = priorityChoices;
    	}

    	/// <summary>
		/// Gets the report detail.
		/// </summary>
        [DataMember]
        public ReportDetail Report;

		/// <summary>
		/// Gets the index of the active report part.
		/// </summary>
		[DataMember]
		public int ReportPartIndex;

		/// <summary>
		/// Gets the order detail.
		/// </summary>
		[DataMember]
		public OrderDetail Order;

		/// <summary>
		/// Gets the priority choices, for setting the priority.
		/// </summary>
		[DataMember]
    	public List<EnumValueInfo> PriorityChoices;
    }
}
