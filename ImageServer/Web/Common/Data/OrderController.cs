#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class OrderController
    {
        private readonly OrderAdaptor _adaptor = new OrderAdaptor();

        public bool DeleteOrderItem(Order item)
        {
            return DeleteOrderItem(item.ServerPartitionKey, item.Key);
        }

        public bool DeleteOrderItem(ServerEntityKey partitionKey, ServerEntityKey orderKey)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // Disconnect studies from order
                var studyBroker = updateContext.GetBroker<IStudyEntityBroker>();
                
                var criteria = new StudySelectCriteria();
                criteria.OrderKey.EqualTo(orderKey);
                criteria.ServerPartitionKey.EqualTo(partitionKey);
                
                var updateColumns = new StudyUpdateColumns
                    {
                        OrderKey = null
                    };
                studyBroker.Update(criteria, updateColumns);

                bool retValue = _adaptor.Delete(updateContext, orderKey);

                updateContext.Commit();

                return retValue;
            }
        }

        public IList<Order> GetOrders(OrderSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }
        public IList<Order> GetRangeOrders(OrderSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetOrderCount(OrderSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }
    }
}
