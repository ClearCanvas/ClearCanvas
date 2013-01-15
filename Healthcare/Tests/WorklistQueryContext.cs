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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Tests
{
	public class WorklistQueryContext : IWorklistQueryContext
	{
		private readonly IPersistenceContext _ctx;

		public WorklistQueryContext(IPersistenceContext ctx, Staff staff, Facility workingFacility, SearchResultPage page, bool downtimeMode)
		{
			_ctx = ctx;
			this.ExecutingStaff = staff;
			this.WorkingFacility = workingFacility;
			this.Page = page;
			this.DowntimeRecoveryMode = downtimeMode;
		}

		#region IWorklistQueryContext Members

		public Staff ExecutingStaff { get; private set; }

		public Facility WorkingFacility { get; private set; }

		public bool DowntimeRecoveryMode { get; private set; }

		public SearchResultPage Page { get; private set; }

		public TBrokerInterface GetBroker<TBrokerInterface>()
			where TBrokerInterface : IPersistenceBroker
		{
			return _ctx.GetBroker<TBrokerInterface>();
		}

		#endregion
	}
}
