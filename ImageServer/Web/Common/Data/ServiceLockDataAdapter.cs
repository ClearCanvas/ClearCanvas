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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete service  entries in the database.
    /// </summary>
    /// 
    public class ServiceLockDataAdapter : BaseAdaptor<ServiceLock, IServiceLockEntityBroker, ServiceLockSelectCriteria, ServiceLockUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of service s.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks()
        {
            return Get();
        }

        /// <summary>
        /// Delete a service  in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteServiceLock(ServiceLock dev)
        {
            return base.Delete(dev.Key);
        }

        /// <summary>
        /// Update a service  entry in the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool Update(ServiceLock service)
        {
            bool ok = true;

            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = service.Enabled;
            param.FilesystemKey = service.FilesystemKey;
            param.Lock = service.Lock;
            param.ProcessorId = service.ProcessorId;
            param.ScheduledTime = service.ScheduledTime;
            param.ServiceLockTypeEnum = service.ServiceLockTypeEnum;

            ok = base.Update(service.Key,param);

            return ok;
        }

        

        /// <summary>
        /// Retrieve a list of service s with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return base.Get(criteria);
        }

        /// <summary>
        /// Create a new service .
        /// </summary>
        /// <param name="newService"></param>
        /// <returns></returns>
        public ServiceLock AddServiceLock(ServiceLock newService)
        {
            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = newService.Enabled;
            param.FilesystemKey = newService.FilesystemKey;
            param.Lock = newService.Lock;
            param.ProcessorId = newService.ProcessorId;
            param.ScheduledTime = newService.ScheduledTime;
            param.ServiceLockTypeEnum = newService.ServiceLockTypeEnum;

            return base.Add(param);
        }
    }
}

