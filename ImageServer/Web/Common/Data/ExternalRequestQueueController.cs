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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class ExternalRequestQueueController : BaseController
    {
        private readonly ExternalRequestQueueAdaptor _adaptor = new ExternalRequestQueueAdaptor();

        public IList<ExternalRequestQueue> FindExternalRequestQueue(ExternalRequestQueueSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int Count(ExternalRequestQueueSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

		public bool DeleteExternalRequestQueueItem(ExternalRequestQueue item)
		{
			return _adaptor.Delete(item.Key);
		}

	    public bool ResetExternalRequestQueueItem(ExternalRequestQueue item)
	    {
		    if (!item.ExternalRequestQueueStatusEnum.Equals(ExternalRequestQueueStatusEnum.Failed))
			    return false;
		    var update = new ExternalRequestQueueUpdateColumns
			    {
				    ExternalRequestQueueStatusEnum = ExternalRequestQueueStatusEnum.Pending,
					Revision = item.Revision + 1
			    };
		    var criteria = new ExternalRequestQueueSelectCriteria();
			criteria.Key.EqualTo(item.Key);
		    criteria.Revision.EqualTo(item.Revision);
		    return _adaptor.Update(criteria, update);
	    }
    }
}
