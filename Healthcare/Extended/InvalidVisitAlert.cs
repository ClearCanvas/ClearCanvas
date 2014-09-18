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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Alerts;

namespace ClearCanvas.Healthcare.Extended
{
	[ExtensionOf(typeof(OrderAlertExtensionPoint))]
	public class InvalidVisitAlert : OrderAlertBase
	{
		public override string Id
		{
			get { return "InvalidVisitAlert"; }
		}

		public override AlertNotification Test(Order order, IPersistenceContext context)
		{
			var reasons = new List<string>();
			if (order.Visit == null)
			{
				// This should never happen in production because an order must have a visit
				reasons.Add(SR.AlertOrderMissingVisit);
			}
			else
			{
				// Check Visit status
				if (order.Visit.Status != VisitStatus.AA)
					reasons.Add(SR.AlertVisitStatusNotActive);

				// Check Visit date
				if (order.Visit.AdmitTime == null)
				{
					// This should never happen in production since visit admit date should always be created from HIS
					reasons.Add(SR.AlertVisitDateMissing);
				}
				else if (order.ScheduledStartTime != null)
				{
					if (order.Visit.AdmitTime.Value.Date > order.ScheduledStartTime.Value.Date)
						reasons.Add(SR.AlertVisitDateIsInFuture);
					else if (order.Visit.AdmitTime.Value.Date < order.ScheduledStartTime.Value.Date)
						reasons.Add(SR.AlertVisitDateIsInPast);
				}
			}

			if (reasons.Count > 0)
				return new AlertNotification(this.Id, reasons);

			return null;
		}
	}
}
