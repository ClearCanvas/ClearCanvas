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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Processes in-progress procedures, and schedules corresponding Image Availability work queue items.
	/// </summary>
	public class ImageAvailabilityProcedureProcessor : EntityQueueProcessor<Procedure>
	{
		private readonly ImageAvailabilityShredSettings _settings;

		internal ImageAvailabilityProcedureProcessor(ImageAvailabilityShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
		}

		public override string Name
		{
			get
			{
				return SR.ImageAvailabilityProcedureProcessor;
			}
		}

		protected override IList<Procedure> GetNextEntityBatch(int batchSize)
		{
			// Find a list of procedures that match the criteria
			ProcedureSearchCriteria criteria = new ProcedureSearchCriteria();
			criteria.Status.EqualTo(ProcedureStatus.IP);
			criteria.ImageAvailability.EqualTo(Healthcare.ImageAvailability.X);

			SearchResultPage page = new SearchResultPage(0, batchSize);
			return PersistenceScope.CurrentContext.GetBroker<IProcedureBroker>().Find(criteria, page);
		}

		protected override void MarkItemClaimed(Procedure item)
		{
			// do nothing
		}

		protected override void ActOnItem(Procedure procedure)
		{
			// create the workqueue item
			TimeSpan expirationTime = TimeSpan.FromHours(_settings.ExpirationTime);
			WorkQueueItem item = ImageAvailabilityWorkQueue.CreateWorkQueueItem(procedure, expirationTime);
			PersistenceScope.CurrentContext.Lock(item, DirtyState.New);
		}

		protected override void OnItemSucceeded(Procedure item)
		{
			// Set this to Not Available so the worklist item doesn't get created again for this procedure
			item.ImageAvailability = Healthcare.ImageAvailability.N;
		}

		protected override void OnItemFailed(Procedure item, Exception error)
		{
			// do nothing
		}
	}
}
