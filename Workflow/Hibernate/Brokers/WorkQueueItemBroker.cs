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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common;

namespace ClearCanvas.Workflow.Hibernate.Brokers
{
	public partial class WorkQueueItemBroker
	{
		#region IWorkQueueItemBroker Members

		public IList<WorkQueueItem> GetPendingItems(string type, int maxItems)
		{
			var query = new HqlQuery("from WorkQueueItem item");
			query.Conditions.Add(new HqlCondition("item.Type = ?", type));
			query.Conditions.Add(new HqlCondition("item.Status = ?", WorkQueueStatus.PN));

			var now = Platform.Time;
			query.Conditions.Add(new HqlCondition("item.ScheduledTime < ?", now));
			query.Conditions.Add(new HqlCondition("(item.ExpirationTime is null or item.ExpirationTime > ?)", now));
			query.Sorts.Add(new HqlSort("item.ScheduledTime", true, 0));
			query.Page = new SearchResultPage(0, maxItems);

			return ExecuteHql<WorkQueueItem>(query);
		}

		#endregion
	}
}
