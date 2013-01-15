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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Data
{

    /// <summary>
    /// ServiceLock configuration screen controller.
    /// </summary>
    public class ServiceLockConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set services from service table
        /// </summary>
        private ServiceLockDataAdapter _adapter = new ServiceLockDataAdapter();

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a service in the database.
        /// </summary>
        /// <param name="service"></param>
        public ServiceLock AddServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User adding new service lock { type={0}, filesystem={1} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            ServiceLock dev = _adapter.AddServiceLock(service);

            if (dev!=null)
                Platform.Log(LogLevel.Info, "New service added by user : {Key={0}, type={1}, filesystem={2}", service.Key, service.ServiceLockTypeEnum, service.FilesystemKey);
            else
                Platform.Log(LogLevel.Info, "Failed to add new service : {  type={1}, filesystem={2} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            return dev;
        }

        /// <summary>
        /// Delete a service from the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User deleting service lock {0}", service.Key);

            bool ok = _adapter.DeleteServiceLock(service);

            Platform.Log(LogLevel.Info, "User delete service lock {0}: {1}", service.Key, ok ? "Successful" : "Failed");

            return ok;
        }

        /// <summary>
        /// Update a service in the database.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><b>true</b> if the record is updated successfully. <b>false</b> otherwise.</returns>
        /// <param name="enabled"></param>
        /// <param name="scheduledDateTime"></param>
        public bool UpdateServiceLock(ServerEntityKey key, bool enabled, DateTime scheduledDateTime)
        {
            Platform.Log(LogLevel.Info, "User updating service Key={0}", key.Key);
            ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
            columns.Enabled = enabled;
            columns.ScheduledTime = scheduledDateTime;

            bool ok = _adapter.Update(key, columns);

            Platform.Log(LogLevel.Info, "ServiceLock Key={0} {1}", key.Key, ok ? "updated by user" : " failed to update");

            return ok;
        
        }

        /// <summary>
        /// Retrieve list of services.
        /// </summary>
        /// <param name="criteria"/>
        /// <returns>List of <see cref="ServiceLock"/> matches <paramref name="criteria"/></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return _adapter.GetServiceLocks(criteria);
        }

        /// <summary>
        /// Retrieve a list of server partitions.
        /// </summary>
        /// <returns>List of all <see cref="ServerPartition"/>.</returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return _serverAdapter.GetServerPartitions();
        }

        #endregion public methods
    }
}
