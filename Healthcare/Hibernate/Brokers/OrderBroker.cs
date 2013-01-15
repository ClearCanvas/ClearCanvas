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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IOrderBroker"/>. See OrderBroker.hbm.xml for queries.
	/// </summary>
	public partial class OrderBroker
	{
		#region IOrderBroker Members

		public Order FindDocumentOwner(AttachedDocument document)
		{
			var q = this.GetNamedHqlQuery("documentOrderOwner");
			q.SetParameter(0, document);
			return (Order)q.UniqueResult();
		}

		public IList<Order> FindByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria)
		{
			return FindByResultRecipient(orderSearchCriteria, recipientSearchCriteria, new SearchResultPage());
		}

		public IList<Order> FindByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria, SearchResultPage page)
		{
			var query = GetBaseResultRecipientQuery(orderSearchCriteria, recipientSearchCriteria);
			query.Page = page;
			return ExecuteHql<Order>(query);
		}

		public long CountByResultRecipient(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria)
		{
			var query = GetBaseResultRecipientQuery(orderSearchCriteria, recipientSearchCriteria);
			query.Selects.Add(new HqlSelect("count(*)"));
			return ExecuteHqlUnique<long>(query);
		}

		#endregion

		private static HqlProjectionQuery GetBaseResultRecipientQuery(OrderSearchCriteria orderSearchCriteria, ResultRecipientSearchCriteria recipientSearchCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			hqlFrom.Joins.Add(new HqlJoin("o.ResultRecipients", "rr"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rr", recipientSearchCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", orderSearchCriteria));
			return query;
		}
	}
}
