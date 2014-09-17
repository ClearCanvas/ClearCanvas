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
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.HL7
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	[ShredIsolation(Level = ShredIsolationLevel.None)]
	public class LogicalHL7EventShred : QueueProcessorShred, IApplicationRoot
	{
		public LogicalHL7EventShred()
		{
		}

		public override string GetDisplayName()
		{
			return SR.LogicalHL7EventShredName;
		}

		public override string GetDescription()
		{
			return SR.LogicalHL7EventShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var settings = new LogicalHL7EventShredSettings();
			var p = new LogicalHL7EventProcessor(
				settings.BatchSize,
				TimeSpan.FromSeconds(settings.EmptyQueueSleepTime),
				TimeSpan.FromSeconds(settings.FailedItemRetryDelay));
			return new QueueProcessor[] { p };
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Start();
		}

		#endregion
	}
}