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
using System.Collections;
using System.Collections.Generic;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class ProtocolGroupBroker
    {
        #region IProtocolGroupBroker Members

        public IList<ProtocolGroup> FindAll(ProcedureType procedureType)
        {
            if (procedureType == null)
                return new List<ProtocolGroup>();

            string hql =
                "select distinct p from ProtocolGroup p"
                + " join p.ReadingGroups r "
                + " join r.ProcedureTypes t"
                + " where t = :requesteProcedureType";

            IQuery query = this.CreateHibernateQuery(hql);
            query.SetParameter("requesteProcedureType", procedureType);
            return query.List<ProtocolGroup>();
        }

        #endregion
    }
}
