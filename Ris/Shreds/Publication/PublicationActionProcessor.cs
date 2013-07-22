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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
	/// Responsible for processing work items that represent Publication Actions.
	/// </summary>
	public class PublicationActionProcessor : WorkQueueProcessor
	{
		private readonly TimeSpan _failedItemRetryDelay;

		internal PublicationActionProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_failedItemRetryDelay = TimeSpan.FromSeconds(settings.FailedItemRetryDelay);
		}

		#region WorkQueueProcessor overrides

		protected override void ActOnItem(WorkQueueItem item)
		{
			var actionType = item.ExtendedProperties["ActionType"];
			var action = (IPublicationAction)new PublicationActionExtensionPoint().CreateExtension(new ClassNameExtensionFilter(actionType));
			var reportPartRef = new EntityRef(item.ExtendedProperties["ReportPartRef"]);
			var reportPart = PersistenceScope.CurrentContext.Load<ReportPart>(reportPartRef, EntityLoadFlags.None);

			Platform.Log(LogLevel.Info, String.Format("Processing Publication Action {0} for report part {1}", actionType, reportPart.OID));

			action.Execute(reportPart, PersistenceScope.CurrentContext);
		}

		protected override string WorkQueueItemType
		{
			get { return "Publication Action"; }
		}

		protected override bool ShouldReschedule(WorkQueueItem item, Exception error, out DateTime rescheduleTime)
		{
			var actionType = item.ExtendedProperties["ActionType"];
			var action = (IPublicationAction)new PublicationActionExtensionPoint().CreateExtension(new ClassNameExtensionFilter(actionType));

			if (error == null)
				return base.ShouldReschedule(item, null, out rescheduleTime);

			if (action.RetryCount >= 0 && item.FailureCount > action.RetryCount)
				return base.ShouldReschedule(item, null, out rescheduleTime);

			//todo: should we retry? things might end up being processed out of order
			rescheduleTime = Platform.Time + _failedItemRetryDelay;
			return true;
		}

		public override string Name
		{
			get { return this.GetType().Name; }
		}

		#endregion

	}
}