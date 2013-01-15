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
    /// Used to create/update/delete device entries in the database.
    /// </summary>
    /// 
    public class DeviceDataAdapter : BaseAdaptor<Device, IDeviceEntityBroker, DeviceSelectCriteria, DeviceUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of devices.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices()
        {
            return Get();
        }

        /// <summary>
        /// Delete a device in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteDevice(Device dev)
        {
            return Delete(dev.Key);
        }

        /// <summary>
        /// Update a device entry in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool Update(Device dev)
        {
            bool ok = true;

            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = dev.ServerPartitionKey;
            param.Enabled = dev.Enabled;
            param.AeTitle = dev.AeTitle;
            param.Description = dev.Description;
            param.Dhcp = dev.Dhcp;
            param.IpAddress = dev.IpAddress;
            param.Port = dev.Port;
            param.AllowQuery = dev.AllowQuery;
            param.AcceptKOPR = dev.AcceptKOPR;
            param.AllowRetrieve = dev.AllowRetrieve;
            param.AllowStorage = dev.AllowStorage;
            param.AllowAutoRoute = dev.AllowAutoRoute;
            param.ThrottleMaxConnections = dev.ThrottleMaxConnections;
            param.DeviceTypeEnum = dev.DeviceTypeEnum;
            Update(dev.Key, param);

            return ok;
        }

        public IList<Device> DummyList
        {
            get
            {
                // return dummy list
                List<Device> list = new List<Device>();
                Device dev = new Device();
                dev.AeTitle = "Checking";

                dev.ServerPartitionKey = new ServerEntityKey("Testing", "Checking");
                list.Add(dev);

                return list;
            }
        }

        /// <summary>
        /// Retrieve a list of devices with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices(DeviceSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Create a new device.
        /// </summary>
        /// <param name="newDev"></param>
        /// <returns></returns>
        public Device AddDevice(Device newDev)
        {
            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = newDev.ServerPartitionKey;
            param.AeTitle = newDev.AeTitle;
            param.Description = newDev.Description;
            param.IpAddress = newDev.IpAddress;
            param.Port = newDev.Port;
            param.Enabled = newDev.Enabled;
            param.Dhcp = newDev.Dhcp;
            param.AllowQuery = newDev.AllowQuery;
            param.AcceptKOPR = newDev.AcceptKOPR;
            param.AllowRetrieve = newDev.AllowRetrieve;
            param.AllowStorage = newDev.AllowStorage;
            param.AllowAutoRoute = newDev.AllowAutoRoute;
            param.ThrottleMaxConnections = newDev.ThrottleMaxConnections;
            param.DeviceTypeEnum = newDev.DeviceTypeEnum;
            return Add(param);
        }
    }
}

