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
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IModalityWorklistItemBroker"/>.
	/// </summary>
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class ModalityWorklistItemBroker : WorklistItemBrokerBase, IModalityWorklistItemBroker
	{
		/// <summary>
		/// No-args constructor for extension construction.
		/// </summary>
		public ModalityWorklistItemBroker()
			: base(new WorklistItemQueryBuilder())
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		/// <param name="worklistItemQueryBuilder"></param>
		/// <param name="procedureSearchQueryBuilder"></param>
		/// <param name="patientSearchQueryBuilder"></param>
		protected ModalityWorklistItemBroker(IWorklistItemQueryBuilder worklistItemQueryBuilder,
			IQueryBuilder procedureSearchQueryBuilder, IQueryBuilder patientSearchQueryBuilder)
			:base(worklistItemQueryBuilder, procedureSearchQueryBuilder, patientSearchQueryBuilder)
		{
		}
	}
}
