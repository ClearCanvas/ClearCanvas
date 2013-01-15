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
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.HL7
{
	/// <summary>
	/// Responsible for processing a single LogicalHL7Event into one or more HL7Event objects per peer
	/// </summary>
	public class LogicalHL7EventProcessor : WorkQueueProcessor
	{
		private TimeSpan _failedItemRetryDelay;

		internal LogicalHL7EventProcessor(int batchSize, TimeSpan sleepTime, TimeSpan failedItemRetryDelay)
			: base(batchSize, sleepTime)
		{
			_failedItemRetryDelay = failedItemRetryDelay;
		}

		#region WorkQueueProcessor overrides

		protected override void ActOnItem(WorkQueueItem item)
		{
			var logicalEvent = new LogicalHL7EventArgs(item);
			Platform.Log(LogLevel.Info, String.Format("Procssing HL7LogicalEvent {0}", item.OID));

			foreach (ILogicalHL7EventListener listener in new LogicalHL7EventListenerExtensionPoint().CreateExtensions())
			{
				listener.OnEvent(logicalEvent);
			}
		}

		protected override string WorkQueueItemType
		{
			get { return LogicalHL7Event.WorkQueueItemType; }
		}

		protected override bool ShouldReschedule(WorkQueueItem item, Exception error, out DateTime rescheduleTime)
		{
			if (error == null)
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