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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class WorklistBroker : EntityBroker<Worklist, WorklistSearchCriteria>, IWorklistBroker
	{

		#region IWorklistBroker Members

		public int Count(WorklistOwner owner)
		{
			var query = new HqlProjectionQuery(new HqlFrom("Worklist", "w"));
			query.Selects.Add(new HqlSelect("count(*)"));

			if (owner.IsStaffOwner)
				query.Conditions.Add(new HqlCondition("w.Owner.Staff = ?", owner.Staff));
			else if (owner.IsGroupOwner)
				query.Conditions.Add(new HqlCondition("w.Owner.Group = ?", owner.Group));
			else if (owner.IsAdminOwner)
				query.Conditions.Add(new HqlCondition("(w.Owner.Staff is null and w.Owner.Group is null)"));

			return (int)ExecuteHqlUnique<long>(query);
		}

		public IList<Worklist> Find(StaffGroup staffGroup)
		{
			var query = GetBaseQuery();
			AddStaffGroupConditions(query, staffGroup);

			return ExecuteHql<Worklist>(query);
		}

		public IList<Worklist> Find(Staff staff, IEnumerable<string> worklistClassNames)
		{
			var query = GetBaseQuery();
			AddStaffConditions(query, staff);
			AddClassConditions(query, worklistClassNames);

			return ExecuteHql<Worklist>(query);
		}

		public IList<Worklist> Find(string name, bool includeUserDefinedWorklists, IEnumerable<string> worklistClassNames, SearchResultPage page)
		{
			HqlProjectionQuery query = GetBaseQuery();

			if (!string.IsNullOrEmpty(name))
				query.Conditions.Add(new HqlCondition("w.Name like ?", string.Format("%{0}%", name)));

			if (!includeUserDefinedWorklists)
				query.Conditions.Add(new HqlCondition("(w.Owner.Staff is null and w.Owner.Group is null)"));

			AddClassConditions(query, worklistClassNames);
			query.Page = page;

			return ExecuteHql<Worklist>(query);
		}

		public Worklist FindOne(string name, string worklistClassName)
		{
			var query = new HqlQuery("from Worklist w");
			query.Conditions.Add(new HqlCondition("w.Name = ?", name));
			query.Conditions.Add(new HqlCondition("w.class = " + worklistClassName));

			var worklists = ExecuteHql<Worklist>(query);
			if (worklists.Count == 0)
				throw new EntityNotFoundException(string.Format("Worklist {0}, class {1} not found.", name, worklistClassName), null);

			return CollectionUtils.FirstElement(worklists);
		}

		#endregion

		private static HqlProjectionQuery GetBaseQuery()
		{
			var query = new HqlProjectionQuery(new HqlFrom("Worklist", "w"));
			query.Selects.Add(new HqlSelect("w"));
			query.SelectDistinct = true;
			return query;
		}

		private static void AddClassConditions(HqlProjectionQuery query, IEnumerable<string> worklistClassNames)
		{
			var classOr = new HqlOr();
			foreach (var className in worklistClassNames)
			{
				classOr.Conditions.Add(new HqlCondition("w.class = " + className));
			}
			query.Conditions.Add(classOr);
		}

		private static void AddStaffConditions(HqlProjectionQuery query, Staff staff)
		{
			query.Froms.Add(new HqlFrom("Staff", "s"));
			query.Conditions.Add(new HqlCondition("s = ?", staff));

			var staffOr = new HqlOr();
			staffOr.Conditions.Add(new HqlCondition("s in elements(w.StaffSubscribers)"));
			staffOr.Conditions.Add(new HqlCondition("s in (select elements(sg.Members) from StaffGroup sg where sg in elements(w.GroupSubscribers))"));

			query.Conditions.Add(staffOr);
		}

		private static void AddStaffGroupConditions(HqlProjectionQuery query, StaffGroup staffGroup)
		{
			query.Froms.Add(new HqlFrom("StaffGroup", "sg"));
			query.Conditions.Add(new HqlCondition("sg = ?", staffGroup));

			var staffGroupOr = new HqlOr();
			staffGroupOr.Conditions.Add(new HqlCondition("sg in elements(w.GroupSubscribers)"));
			query.Conditions.Add(staffGroupOr);
		}
	}
}
