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
using System.Web;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class PartitionArchiveAdaptor : BaseAdaptor<PartitionArchive, IPartitionArchiveEntityBroker, PartitionArchiveSelectCriteria, PartitionArchiveUpdateColumns>
	{
		#region Private Members

		private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

		#endregion Private Members

		public bool RestoreStudy(Study theStudy)
		{
			RestoreQueue restore = ServerHelper.InsertRestoreRequest(theStudy.LoadStudyStorage(HttpContext.Current.GetSharedPersistentContext()));
            if (restore==null)
                throw new ApplicationException("Unable to restore the study. See the log file for details.");

			return true;
		}
	}
}
