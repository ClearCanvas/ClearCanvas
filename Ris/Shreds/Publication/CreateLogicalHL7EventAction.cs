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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(PublicationActionExtensionPoint))]
	public class CreateLogicalHL7EventAction : IPublicationAction
	{
		private readonly bool _enabled;
		private readonly int _retryCount;

		public CreateLogicalHL7EventAction()
		{
			var settings = new PublicationShredSettings();
			_enabled = settings.HL7PublicationEnabled;
			_retryCount = settings.HL7FailedItemRetryCount;
		}

		public bool Enabled
		{
			get { return _enabled; }
		}

		public int RetryCount
		{
			get { return _retryCount; }
		}

		public void Execute(ReportPart reportPart, IPersistenceContext context)
		{
			if (_enabled == false)
				return;

			LogicalHL7Event.ReportPublished.EnqueueEvents(reportPart.Report);
		}
	}
}
