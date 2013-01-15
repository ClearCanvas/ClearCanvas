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

// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// NHibernate implementation of <see cref="IProcedureTypeGroupBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
	public partial class ProcedureTypeGroupBroker : IProcedureTypeGroupBroker
	{
        #region IProcedureTypeGroupBroker Members

        public IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
        	return Find(criteria, subClass, null);
        }

		public IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass, SearchResultPage page)
		{
			HqlQuery query = new HqlQuery(string.Format("from {0} x", subClass.Name));

			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("x", criteria));
			query.Sorts.AddRange(HqlSort.FromSearchCriteria("x", criteria));
			query.Page = page;

			return ExecuteHql<ProcedureTypeGroup>(query);
		}

		public ProcedureTypeGroup FindOne(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
            IList<ProcedureTypeGroup> groups = Find(criteria, subClass);

            if(groups.Count == 0)
                throw new EntityNotFoundException(null);

            return CollectionUtils.FirstElement(groups);
        }

        #endregion
    }
}