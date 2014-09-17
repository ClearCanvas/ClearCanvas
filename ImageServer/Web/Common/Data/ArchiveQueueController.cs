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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class ArchiveQueueController
	{
        private readonly ArchiveQueueAdaptor _adaptor = new ArchiveQueueAdaptor();


		/// <summary>
		/// Gets a list of <see cref="ArchiveQueue"/> items with specified criteria
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList<ArchiveQueue> FindArchiveQueue(WebQueryArchiveQueueParameters parameters)
		{
			try
			{
				IList<ArchiveQueue> list;

				IWebQueryArchiveQueue broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IWebQueryArchiveQueue>();
				list = broker.Find(parameters);

				return list;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "FindArchiveQueue failed", e);
				return new List<ArchiveQueue>();
			}
		}

        public bool DeleteArchiveQueueItem(ArchiveQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

		public bool ResetArchiveQueueItem(IList<ArchiveQueue> items, DateTime time)
		{
			if (items == null || items.Count == 0)
				return false;

			ArchiveQueueUpdateColumns columns = new ArchiveQueueUpdateColumns();
			columns.ArchiveQueueStatusEnum = ArchiveQueueStatusEnum.Pending;
			columns.ProcessorId = "";
			columns.ScheduledTime = time;

			bool result = true;
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
			using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IArchiveQueueEntityBroker archiveQueueBroker = ctx.GetBroker<IArchiveQueueEntityBroker>();
				
				foreach (ArchiveQueue item in items)
				{
					// Only do an update if its a failed status currently
					ArchiveQueueSelectCriteria criteria = new ArchiveQueueSelectCriteria();
					criteria.ArchiveQueueStatusEnum.EqualTo(ArchiveQueueStatusEnum.Failed);
					criteria.StudyStorageKey.EqualTo(item.StudyStorageKey);

					if (!archiveQueueBroker.Update(criteria, columns))
					{
						result = false;
						break;
					}
				}

				if (result)
					ctx.Commit();
			}

			return result;
		}
	}
}
