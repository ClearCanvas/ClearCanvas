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

using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class PublicationStepBroker : EntityBroker<PublicationStep, PublicationStepSearchCriteria>, IPublicationStepBroker
	{
		public IList<PublicationStep> FindUnprocessedSteps(int failedItemRetryDelay, SearchResultPage page)
		{
			var query = new HqlProjectionQuery(new HqlFrom(typeof(PublicationStep).Name, "ps"));
			query.Conditions.Add(new HqlCondition("ps.State = ?", ActivityStatus.SC));
			query.Conditions.Add(new HqlCondition("ps.Scheduling.Performer.Staff is not null"));
			query.Conditions.Add(new HqlCondition("ps.Scheduling.StartTime < ?", Platform.Time));
			query.Conditions.Add(new HqlCondition("(ps.LastFailureTime is null or ps.LastFailureTime < ?)", Platform.Time.AddSeconds(-failedItemRetryDelay)));
			query.Sorts.Add(new HqlSort("ps.Scheduling.StartTime", true, 0));
			query.Page = page;
			return ExecuteHql<PublicationStep>(query);
		}

	}
}