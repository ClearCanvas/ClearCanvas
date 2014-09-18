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
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class RestoreQueueController
	{
        private readonly RestoreQueueAdaptor _adaptor = new RestoreQueueAdaptor();


		/// <summary>
		/// Gets a list of <see cref="RestoreQueue"/> items with specified criteria
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList<RestoreQueue> FindRestoreQueue(WebQueryRestoreQueueParameters parameters)
		{
			try
			{
				IWebQueryRestoreQueue broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IWebQueryRestoreQueue>();
				IList<RestoreQueue> list = broker.Find(parameters);

				return list;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, "FindRestoreQueue failed", e);
				return new List<RestoreQueue>();
			}
		}

        public bool DeleteRestoreQueueItem(RestoreQueue item)
        {
        	using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
				LockStudyParameters parms = new LockStudyParameters
				                            	{
				                            		StudyStorageKey = item.StudyStorageKey,
				                            		QueueStudyStateEnum = QueueStudyStateEnum.Idle
				                            	};
				if (!lockStudyBroker.Execute(parms))
					return false;
				if (!parms.Successful)
					return false;

				bool retValue = _adaptor.Delete(updateContext, item.Key);

				updateContext.Commit();

				return retValue;
			}
        }

        
	}
}
