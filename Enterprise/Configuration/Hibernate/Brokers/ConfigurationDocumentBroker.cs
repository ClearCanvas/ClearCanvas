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

namespace ClearCanvas.Enterprise.Configuration.Hibernate.Brokers
{
	partial class ConfigurationDocumentBroker
	{
		#region Implementation of IConfigurationDocumentBroker

		public IList<ConfigurationDocument> Find(ConfigurationDocumentSearchCriteria documentCriteria, ConfigurationDocumentBodySearchCriteria bodyCriteria, SearchResultPage page)
		{
			var hqlFrom = new HqlFrom(typeof(ConfigurationDocument).Name, "doc");
			hqlFrom.Joins.Add(new HqlJoin("doc.Body", "body"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Selects.Add(new HqlSelect("doc"));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("doc", documentCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("body", bodyCriteria));
			query.Page = page;

			return ExecuteHql<ConfigurationDocument>(query);
		}

		#endregion
	}
}
