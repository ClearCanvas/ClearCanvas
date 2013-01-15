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
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
	public partial class AuthorityGroupBroker
	{
		public int GetUserCountForGroup(AuthorityGroup group)
		{
			var q = new HqlQuery("select count(elements(g.Users)) from AuthorityGroup g");
			q.Conditions.Add(new HqlCondition("g = ?", group));
			return (int)ExecuteHqlUnique<long>(q);
		}

        public Guid[] FindDataGroupsByUserName(string userName)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            AuthorityGroupSearchCriteria groupWhere = new AuthorityGroupSearchCriteria();
            groupWhere.DataGroup.EqualTo(true);

            // want this to be as fast as possible - use joins and only select the AuthorityToken names
            HqlQuery query = new HqlQuery("select distinct g.OID from User u join u.AuthorityGroups g");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("u", where));
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("g", groupWhere));

            // take advantage of query caching if possible
            query.Cacheable = true;

            IList<Guid> oids = ExecuteHql<Guid>(query);
            var result = new Guid[oids.Count];
            oids.CopyTo(result, 0);
            return result;
        }
	}
}
