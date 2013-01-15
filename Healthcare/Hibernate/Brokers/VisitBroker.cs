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
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class VisitBroker
	{
		#region IVisitBroker members

		public IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			return FindByVisitPractitioner(visitSearchCriteria, practitionerSearchCriteria, new SearchResultPage());
		}

		public IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria, SearchResultPage page)
		{
			var query = GetBaseVisitPractitionerQuery(visitSearchCriteria, practitionerSearchCriteria);
			query.Page = page;
			return ExecuteHql<Visit>(query);
		}

		public long CountByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			var query = GetBaseVisitPractitionerQuery(visitSearchCriteria, practitionerSearchCriteria);
			query.Selects.Add(new HqlSelect("count(*)"));
			return ExecuteHqlUnique<long>(query);
		}

		#endregion

		private static HqlProjectionQuery GetBaseVisitPractitionerQuery(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(Visit).Name, "v");
			hqlFrom.Joins.Add(new HqlJoin("v.Practitioners", "vp"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("vp", practitionerSearchCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("v", visitSearchCriteria));
			return query;
		}
	}
}
