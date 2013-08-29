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

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
    /// This publication action sends the report to the outbound fax/mail queue.
    /// </summary>
	// JR: Disabled as of Yen - this action is not used
	//[ExtensionOf(typeof(PublicationActionExtensionPoint))]
	public class MailFaxAction : IPublicationAction
	{
		#region IPublicationAction Members

		public bool Enabled
		{
			get { return false; }
		}

		public int RetryCount
		{
			get { return -1; }
		}

		public void Execute(ReportPart reportPart, IPersistenceContext context)
		{
			var order = reportPart.Report.Procedures.First().Order;
			foreach (var recipient in order.ResultRecipients)
			{
				var item = Create(
					order.AccessionNumber,
					reportPart.Report.GetRef(),
					recipient.PractitionerContactPoint.Practitioner.GetRef(),
					recipient.PractitionerContactPoint.GetRef());

				context.Lock(item, DirtyState.New);
			}
		}

		#endregion

		private static WorkQueueItem Create(
			string accessionNumber,
			EntityRef reportRef,
			EntityRef practitionerRef,
			EntityRef contactPointRef)
		{
			var workQueueItem = new WorkQueueItem("Mail/Fax Report");
			workQueueItem.ExtendedProperties.Add("AccessionNumber", accessionNumber);
			workQueueItem.ExtendedProperties.Add("ReportOID", reportRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerOID", practitionerRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerContactPointOID", contactPointRef.ToString(false, false));

			return workQueueItem;
		}
	}
}
