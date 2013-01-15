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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Merge handler implementation for merging instances of <see cref="ExternalPractitioner"/>.
	/// </summary>
	[ExtensionOf(typeof(MergeHandlerExtensionPoint))]
	public class ExternalPractitionerMergeHandler : MergeHandlerBase<ExternalPractitioner>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerMergeHandler()
			:base(new MergeShredSettings().ItemsProcessedPerTransaction)
		{
		}

		/// <summary>
		/// Gets the set of merge steps to be performed.
		/// </summary>
		/// <remarks>
		/// Defines a set of migration steps to be executed. The first step in the list is always executed first.
		/// The execution of each step returns an integer indicating which step to execute next.
		/// </remarks>
		protected override MergeStep[] MergeSteps
		{
			get
			{
				return new MergeStep[]
				{
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.CompletedRecently, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.CompletedRecently, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.InProgress, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.InProgress, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.Scheduled, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.Scheduled, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByOrderingPractitioner, OrderMigrationPriority.All, MigrateOrder, context),
					(item, stage, context) => Migrate<Order, OrderSearchCriteria>(item, stage, GetOrderBatchByResultRecipient, OrderMigrationPriority.All, MigrateOrder, context),
					(item, stage, context) => Migrate<Visit, VisitSearchCriteria>(item, stage, GetVisitBatch, VisitMigrationPriority.All, MigrateVisit, context),
					(item, stage, context) => DeletePractitioner(item, stage, context)
				};
			}
		}

		private static int DeletePractitioner(ExternalPractitioner practitioner, int stage, IPersistenceContext context)
		{
			Platform.Log(LogLevel.Debug, "Attempting to delete practitioner {0}", practitioner.OID);

			try
			{
				// since there are no more referencing orders or visits, we can delete the practitioner
				// and its contact points
				context.GetBroker<IExternalPractitionerBroker>().Delete(practitioner);

				// force the delete to occur, to ensure it will succeed
				context.SynchState();

				// merge completed
				return -1;
			}

			catch (PersistenceException e)
			{
				throw new MergeProcessor.CannotDeleteException(e);
			}
		}

		private static void MigrateOrder(ExternalPractitioner practitioner, Order order)
		{
			var destPractitioner = practitioner.GetUltimateMergeDestination();

			// debug logging
			Platform.Log(LogLevel.Debug, "Migrating order A# {0} from practitioner {1} to {2}",
				order.AccessionNumber, practitioner.OID, destPractitioner.OID);


			// update ordering practitioner
			if (order.OrderingPractitioner.Equals(practitioner))
				order.OrderingPractitioner = destPractitioner;

			// update result recipients
			foreach (var contactPoint in practitioner.ContactPoints)
			{
				foreach (var rr in order.ResultRecipients)
				{
					if (rr.PractitionerContactPoint.Equals(contactPoint))
						rr.PractitionerContactPoint = contactPoint.GetUltimateMergeDestination();
				}
			}
		}

		private static void MigrateVisit(ExternalPractitioner practitioner, Visit visit)
		{
			var dest = practitioner.GetUltimateMergeDestination();

			// debug logging
			Platform.Log(LogLevel.Debug, "Migrating visit {0} from practitioner {1} to {2}",
				visit.VisitNumber, practitioner.OID, dest.OID);

			foreach (var visitPractitioner in visit.Practitioners)
			{
				if (visitPractitioner.Practitioner.Equals(practitioner))
					visitPractitioner.Practitioner = dest;
			}
		}

		private static IList<Order> GetOrderBatchByOrderingPractitioner(ExternalPractitioner practitioner, Action<OrderSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var ordersWhere = new OrderSearchCriteria();
			priorityFilter(ordersWhere);

			ordersWhere.OrderingPractitioner.EqualTo(practitioner);
			return context.GetBroker<IOrderBroker>().Find(ordersWhere, new SearchResultPage(0, batchSize));
		}

		private static IList<Order> GetOrderBatchByResultRecipient(ExternalPractitioner practitioner, Action<OrderSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var ordersWhere = new OrderSearchCriteria();
			priorityFilter(ordersWhere);

			var recipientWhere = new ResultRecipientSearchCriteria();
			recipientWhere.PractitionerContactPoint.In(practitioner.ContactPoints);

			return context.GetBroker<IOrderBroker>().FindByResultRecipient(ordersWhere, recipientWhere, new SearchResultPage(0, batchSize));
		}

		private static IList<Visit> GetVisitBatch(ExternalPractitioner practitioner, Action<VisitSearchCriteria> priorityFilter, int batchSize, IPersistenceContext context)
		{
			var visitsWhere = new VisitSearchCriteria();
			priorityFilter(visitsWhere);

			var practitionersWhere = new VisitPractitionerSearchCriteria();
			practitionersWhere.Practitioner.EqualTo(practitioner);
			return context.GetBroker<IVisitBroker>().FindByVisitPractitioner(new VisitSearchCriteria(), practitionersWhere, new SearchResultPage(0, batchSize));
		}
	}
}
