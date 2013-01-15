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
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ModalityProcedureStepBroker : EntityBroker<ModalityProcedureStep, ModalityProcedureStepSearchCriteria>, IModalityProcedureStepBroker
	{
		public IList<ModalityProcedureStep> Find(ModalityProcedureStepSearchCriteria mpsCriteria, ProcedureSearchCriteria procedureCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(ModalityProcedureStep).Name, "mps");
			hqlFrom.Joins.Add(new HqlJoin("mps.Procedure", "rp", HqlJoinMode.Inner, true));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("mps", mpsCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp", procedureCriteria));

			return ExecuteHql<ModalityProcedureStep>(query);
		}
	}
}
