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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class AlertController
	{
        private readonly AlertAdaptor _adaptor = new AlertAdaptor();

        public IList<Alert> GetAllAlerts()
        {
            AlertSelectCriteria searchCriteria = new AlertSelectCriteria();
            searchCriteria.InsertTime.SortAsc(0);
            return GetAlerts(searchCriteria);
        }

        public IList<Alert> GetAlerts(AlertSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<Alert> GetRangeAlerts(AlertSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetAlertsCount(AlertSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteAlert(Alert item)
        {
            return _adaptor.Delete(item.Key);
        }

        public bool DeleteAlertItems(IList<Alert> items)
        {
            foreach (Alert item in items)
            {
                if (_adaptor.Delete(item.Key) == false)
                    return false;
            }

            return true;
        }

        public bool DeleteAlertItem(ServerEntityKey key)
        {
            return _adaptor.Delete(key);   
        }

        public bool DeleteAllAlerts()
        {
            return _adaptor.Delete(new AlertSelectCriteria());
        }
	}
}
